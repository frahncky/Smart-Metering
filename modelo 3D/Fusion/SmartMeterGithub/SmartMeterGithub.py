"""Recreates the nine GitHub FeatureScripts at their default dimensions.
Parts are arranged on a grid for inspection, not as a validated assembly.
"""
import adsk.core
import adsk.fusion
import json
import math
import os
import traceback
from datetime import datetime

HERE = os.path.dirname(os.path.abspath(__file__))
# Shared Fusion folder (modelo 3D/Fusion), resolved from this script's location.
OFFICIAL_FOLDER = os.path.dirname(HERE)

def point(x, y):
    return adsk.core.Point3D.create(x / 10, y / 10, 0)

def sketch_for(comp, z):
    plane = comp.xYConstructionPlane
    if abs(z) > 1e-9:
        inp = comp.constructionPlanes.createInput()
        inp.setByOffset(plane, adsk.core.ValueInput.createByReal(z / 10))
        plane = comp.constructionPlanes.add(inp)
    return comp.sketches.add(plane)

def outline(sketch, spec):
    x, y = spec['x'], spec['y']
    if spec['shape'] == 'circle':
        sketch.sketchCurves.sketchCircles.addByCenterRadius(
            point(x, y), spec['dia'] / 20)
        return
    a, b = spec['w'] / 2, spec['h'] / 2
    lines = sketch.sketchCurves.sketchLines
    if spec['shape'] == 'rect':
        lines.addTwoPointRectangle(point(x-a, y-b), point(x+a, y+b))
        return
    r = spec['r']
    k = r / math.sqrt(2)
    # Clockwise outline, exact circular corner arcs.
    corners = [
        ((-a+r,b), (a-r,b), (a-r+k,b-r+k), (a,b-r)),
        ((a,b-r), (a,-b+r), (a-r+k,-b+r-k), (a-r,-b)),
        ((a-r,-b), (-a+r,-b), (-a+r-k,-b+r-k), (-a,-b+r)),
        ((-a,-b+r), (-a,b-r), (-a+r-k,b-r+k), (-a+r,b)),
    ]
    for start, end, mid, arcend in corners:
        lines.addByTwoPoints(point(x+start[0],y+start[1]),
                             point(x+end[0],y+end[1]))
        sketch.sketchCurves.sketchArcs.addByThreePoints(
            point(x+end[0],y+end[1]),
            point(x+mid[0],y+mid[1]),
            point(x+arcend[0],y+arcend[1]))

def build(comp, recipe):
    modes = {
        'new': adsk.fusion.FeatureOperations.NewBodyFeatureOperation,
        'cut': adsk.fusion.FeatureOperations.CutFeatureOperation,
        'join': adsk.fusion.FeatureOperations.JoinFeatureOperation,
    }
    for index, spec in enumerate(recipe['operations']):
        sketch = sketch_for(comp, spec['z'])
        sketch.name = '%02d_%s_%s' % (index+1, spec['op'], spec['shape'])
        outline(sketch, spec)
        if sketch.profiles.count != 1:
            raise RuntimeError('%s operation %s has %s profiles' %
                               (recipe['name'], index+1, sketch.profiles.count))
        own_bodies = [body for body in comp.bRepBodies]
        operation = (adsk.fusion.FeatureOperations.NewBodyFeatureOperation
                     if spec['op'] == 'join' else modes[spec['op']])
        extrudes = comp.features.extrudeFeatures
        extrusion_input = extrudes.createInput(sketch.profiles.item(0), operation)
        extent = adsk.fusion.DistanceExtentDefinition.create(
            adsk.core.ValueInput.createByReal(spec['t'] / 10))
        if not extrusion_input.setOneSideExtent(
                extent, adsk.fusion.ExtentDirections.PositiveExtentDirection):
            raise RuntimeError('Could not define extrusion extent')
        if spec['op'] == 'cut':
            extrusion_input.participantBodies = own_bodies
        feature = extrudes.add(extrusion_input)
        if spec['op'] == 'join':
            if not own_bodies:
                raise RuntimeError('Join requires a component-local target')
            tools = adsk.core.ObjectCollection.create()
            for body in feature.bodies:
                tools.add(body)
            combine_input = comp.features.combineFeatures.createInput(own_bodies[0], tools)
            combine_input.operation = adsk.fusion.FeatureOperations.JoinFeatureOperation
            combine_input.isKeepToolBodies = False
            feature = comp.features.combineFeatures.add(combine_input)
        if not feature:
            raise RuntimeError('Extrusion failed: '+sketch.name)
        feature.name = sketch.name
        sketch.isVisible = False
    expected = 2 if recipe['name'].endswith('DesktopFeet') else (
        3 if recipe['name'].endswith('PCBSupports') else 1)
    if comp.bRepBodies.count != expected:
        raise RuntimeError('%s: expected %s bodies, got %s' %
                           (recipe['name'], expected, comp.bRepBodies.count))
    for body in comp.bRepBodies:
        if not body.isSolid or body.volume <= 0:
            raise RuntimeError('Invalid solid: '+recipe['name'])

def run(context):
    app = adsk.core.Application.get()
    ui = app.userInterface
    try:
        with open(os.path.join(HERE, 'geometry.json'), encoding='utf-8') as handle:
            recipes = json.load(handle)
        doc = app.documents.add(adsk.core.DocumentTypes.FusionDesignDocumentType)
        doc.name = 'Smart Metering - GitHub'
        design = adsk.fusion.Design.cast(app.activeProduct)
        if not design:
            raise RuntimeError('A Fusion design is required')
        design.designType = adsk.fusion.DesignTypes.ParametricDesignType
        # Fusion internal geometry units are cm; helpers convert mm to cm.
        root = design.rootComponent
        for index, recipe in enumerate(recipes):
            transform = adsk.core.Matrix3D.create()
            occurrence = root.occurrences.addNewComponent(transform)
            occurrence.isGroundToParent = False
            occurrence.isGrounded = False
            occurrence.component.name = recipe['name']
            build(occurrence.component, recipe)
            adsk.doEvents()
        validation = []
        for index, occurrence in enumerate(root.occurrences):
            matrix = adsk.core.Matrix3D.create()
            matrix.translation = adsk.core.Vector3D.create((index % 3)*40, -(index//3)*23, 0)
            occurrence.transform2 = matrix
            for body in occurrence.component.bRepBodies:
                if not body.isSolid or body.volume <= 0:
                    raise RuntimeError('Invalid final solid: '+occurrence.component.name)
                validation.append({'component':occurrence.component.name,'volume_mm3':body.volume*1000})
        if design.snapshots.hasPendingSnapshot:
            design.snapshots.add()
        output = os.path.join(OFFICIAL_FOLDER, 'exportados',
                              datetime.now().strftime('%Y%m%d_%H%M%S_%f'))
        os.makedirs(output, exist_ok=True)
        manager = design.exportManager
        archive = os.path.join(output, 'SmartMeter_Github.f3d')
        if not manager.execute(manager.createFusionArchiveExportOptions(archive)):
            raise RuntimeError('F3D export failed')
        for occurrence in root.occurrences:
            comp = occurrence.component
            step = os.path.join(output, comp.name+'.step')
            if not manager.execute(manager.createSTEPExportOptions(step, comp)):
                raise RuntimeError('STEP export failed: '+comp.name)
        app.activeViewport.fit()
        with open(os.path.join(output, 'resultado.json'), 'w', encoding='utf-8') as handle:
            json.dump({'status':'generated', 'parts':len(recipes),
                       'source':'frahncky/Smart-Metering/cad',
                       'layout':'inspection grid, not assembly', 'solids':validation, 'scoped_operations':True}, handle, indent=2)
        ui.messageBox('Nove peças geradas. F3D e STEP salvos em:\n'+output)
    except Exception:
        error = traceback.format_exc()
        with open(os.path.join(HERE, 'erro.txt'), 'w', encoding='utf-8') as handle:
            handle.write(error)
        ui.messageBox('A geração foi interrompida. Detalhes em erro.txt.\n'+error)
