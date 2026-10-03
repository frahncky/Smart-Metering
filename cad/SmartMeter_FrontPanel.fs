FeatureScript 2892;
import(path : "onshape/std/common.fs", version : "2892.0");

/**
 * SMART METERING
 * Peça 01 - Moldura Frontal
 *
 * Dimensões iniciais:
 * Gabinete: 220 x 145 mm
 * Tela: módulo 167 x 102 mm
 * Área visível: 155 x 87 mm
 * Espessura frontal: 4 mm
 *
 * Origem:
 * Centro da moldura = X0 Y0
 * Frente = Z0
 * Traseira = Z positivo
 */

annotation {
    "Feature Type Name" : "Smart Meter - Moldura Frontal",
    "Feature Type Description" :
        "Gera a moldura frontal parametrica do Smart Metering com janela da touchscreen, rebaixo traseiro e furos M3."
}
export const smartMeterFrontPanel = defineFeature(
    function(context is Context, id is Id, definition is map)

    precondition
    {
        annotation { "Group Name" : "Gabinete", "Collapsed By Default" : false }
        {
            annotation { "Name" : "Largura" }
            isLength(
                definition.panelWidth,
                { (millimeter) : [150, 220, 400] } as LengthBoundSpec);

            annotation { "Name" : "Altura" }
            isLength(
                definition.panelHeight,
                { (millimeter) : [100, 145, 300] } as LengthBoundSpec);

            annotation { "Name" : "Espessura frontal" }
            isLength(
                definition.panelThickness,
                { (millimeter) : [2, 4, 10] } as LengthBoundSpec);

            annotation { "Name" : "Raio dos cantos" }
            isLength(
                definition.cornerRadius,
                { (millimeter) : [1, 8, 25] } as LengthBoundSpec);
        }

        annotation { "Group Name" : "Touchscreen 7 polegadas", "Collapsed By Default" : false }
        {
            annotation { "Name" : "Largura do modulo" }
            isLength(
                definition.displayWidth,
                { (millimeter) : [100, 167, 220] } as LengthBoundSpec);

            annotation { "Name" : "Altura do modulo" }
            isLength(
                definition.displayHeight,
                { (millimeter) : [60, 102, 140] } as LengthBoundSpec);

            annotation { "Name" : "Largura da janela visivel" }
            isLength(
                definition.windowWidth,
                { (millimeter) : [100, 155, 210] } as LengthBoundSpec);

            annotation { "Name" : "Altura da janela visivel" }
            isLength(
                definition.windowHeight,
                { (millimeter) : [50, 87, 130] } as LengthBoundSpec);

            annotation { "Name" : "Profundidade do rebaixo" }
            isLength(
                definition.recessDepth,
                { (millimeter) : [0.5, 2.5, 5] } as LengthBoundSpec);
        }

        annotation { "Group Name" : "Fixacao da tela", "Collapsed By Default" : false }
        {
            annotation { "Name" : "Distancia horizontal entre furos" }
            isLength(
                definition.holeSpacingX,
                { (millimeter) : [100, 180, 205] } as LengthBoundSpec);

            annotation { "Name" : "Distancia vertical entre furos" }
            isLength(
                definition.holeSpacingY,
                { (millimeter) : [60, 115, 135] } as LengthBoundSpec);

            annotation { "Name" : "Diametro dos furos" }
            isLength(
                definition.holeDiameter,
                { (millimeter) : [2, 3.2, 6] } as LengthBoundSpec);
        }
    }

    {
        var W = definition.panelWidth;
        var H = definition.panelHeight;
        var T = definition.panelThickness;
        var R = definition.cornerRadius;

        var halfW = W / 2;
        var halfH = H / 2;
        var k = R / sqrt(2);

        var frontSketch = newSketchOnPlane(
            context,
            id + "frontSketch",
            {
                "sketchPlane" :
                    plane(
                        vector(0, 0, 0) * millimeter,
                        vector(0, 0, 1)
                    )
            }
        );

        skLineSegment(
            frontSketch,
            "top",
            {
                "start" : vector(-halfW + R, halfH),
                "end"   : vector( halfW - R, halfH)
            }
        );

        skArc(
            frontSketch,
            "topRight",
            {
                "start" : vector(halfW - R, halfH),
                "mid"   : vector(
                                halfW - R + k,
                                halfH - R + k
                          ),
                "end"   : vector(
                                halfW,
                                halfH - R
                          )
            }
        );

        skLineSegment(
            frontSketch,
            "right",
            {
                "start" : vector(halfW, halfH - R),
                "end"   : vector(halfW, -halfH + R)
            }
        );

        skArc(
            frontSketch,
            "bottomRight",
            {
                "start" : vector(halfW, -halfH + R),
                "mid"   : vector(
                                halfW - R + k,
                                -halfH + R - k
                          ),
                "end"   : vector(
                                halfW - R,
                                -halfH
                          )
            }
        );

        skLineSegment(
            frontSketch,
            "bottom",
            {
                "start" : vector( halfW - R, -halfH),
                "end"   : vector(-halfW + R, -halfH)
            }
        );

        skArc(
            frontSketch,
            "bottomLeft",
            {
                "start" : vector(-halfW + R, -halfH),
                "mid"   : vector(
                                -halfW + R - k,
                                -halfH + R - k
                          ),
                "end"   : vector(
                                -halfW,
                                -halfH + R
                          )
            }
        );

        skLineSegment(
            frontSketch,
            "left",
            {
                "start" : vector(-halfW, -halfH + R),
                "end"   : vector(-halfW, halfH - R)
            }
        );

        skArc(
            frontSketch,
            "topLeft",
            {
                "start" : vector(-halfW, halfH - R),
                "mid"   : vector(
                                -halfW + R - k,
                                halfH - R + k
                          ),
                "end"   : vector(
                                -halfW + R,
                                halfH
                          )
            }
        );

        skSolve(frontSketch);

        opExtrude(
            context,
            id + "panelExtrude",
            {
                "entities" :
                    qSketchRegion(id + "frontSketch"),

                "direction" :
                    vector(0, 0, 1),

                "endBound" :
                    BoundingType.BLIND,

                "endDepth" :
                    T
            }
        );

        var panelBody =
            qCreatedBy(
                id + "panelExtrude",
                EntityType.BODY
            );

        var windowSketch = newSketchOnPlane(
            context,
            id + "windowSketch",
            {
                "sketchPlane" :
                    plane(
                        vector(0, 0, 0) * millimeter,
                        vector(0, 0, 1)
                    )
            }
        );

        skRectangle(
            windowSketch,
            "displayWindow",
            {
                "firstCorner" :
                    vector(
                        -definition.windowWidth / 2,
                        -definition.windowHeight / 2
                    ),

                "secondCorner" :
                    vector(
                        definition.windowWidth / 2,
                        definition.windowHeight / 2
                    )
            }
        );

        skSolve(windowSketch);

        opExtrude(
            context,
            id + "windowTool",
            {
                "entities" :
                    qSketchRegion(id + "windowSketch"),

                "direction" :
                    vector(0, 0, 1),

                "endBound" :
                    BoundingType.THROUGH_ALL,

                "startBound" :
                    BoundingType.THROUGH_ALL
            }
        );

        opBoolean(
            context,
            id + "cutWindow",
            {
                "tools" :
                    qCreatedBy(
                        id + "windowTool",
                        EntityType.BODY
                    ),

                "targets" :
                    panelBody,

                "operationType" :
                    BooleanOperationType.SUBTRACTION
            }
        );

        var recessZ =
            T - definition.recessDepth;

        var recessSketch = newSketchOnPlane(
            context,
            id + "recessSketch",
            {
                "sketchPlane" :
                    plane(
                        vector(
                            0 * millimeter,
                            0 * millimeter,
                            recessZ
                        ),
                        vector(0, 0, 1)
                    )
            }
        );

        skRectangle(
            recessSketch,
            "displayRecess",
            {
                "firstCorner" :
                    vector(
                        -definition.displayWidth / 2,
                        -definition.displayHeight / 2
                    ),

                "secondCorner" :
                    vector(
                        definition.displayWidth / 2,
                        definition.displayHeight / 2
                    )
            }
        );

        skSolve(recessSketch);

        opExtrude(
            context,
            id + "recessTool",
            {
                "entities" :
                    qSketchRegion(id + "recessSketch"),

                "direction" :
                    vector(0, 0, 1),

                "endBound" :
                    BoundingType.BLIND,

                "endDepth" :
                    definition.recessDepth
                    + 0.2 * millimeter
            }
        );

        opBoolean(
            context,
            id + "cutRecess",
            {
                "tools" :
                    qCreatedBy(
                        id + "recessTool",
                        EntityType.BODY
                    ),

                "targets" :
                    panelBody,

                "operationType" :
                    BooleanOperationType.SUBTRACTION
            }
        );

        var holesSketch = newSketchOnPlane(
            context,
            id + "holesSketch",
            {
                "sketchPlane" :
                    plane(
                        vector(0, 0, 0) * millimeter,
                        vector(0, 0, 1)
                    )
            }
        );

        var hx =
            definition.holeSpacingX / 2;

        var hy =
            definition.holeSpacingY / 2;

        var holeR =
            definition.holeDiameter / 2;

        skCircle(
            holesSketch,
            "hole1",
            {
                "center" : vector(-hx, hy),
                "radius" : holeR
            }
        );

        skCircle(
            holesSketch,
            "hole2",
            {
                "center" : vector(hx, hy),
                "radius" : holeR
            }
        );

        skCircle(
            holesSketch,
            "hole3",
            {
                "center" : vector(-hx, -hy),
                "radius" : holeR
            }
        );

        skCircle(
            holesSketch,
            "hole4",
            {
                "center" : vector(hx, -hy),
                "radius" : holeR
            }
        );

        skSolve(holesSketch);

        opExtrude(
            context,
            id + "holesTool",
            {
                "entities" :
                    qSketchRegion(id + "holesSketch"),

                "direction" :
                    vector(0, 0, 1),

                "endBound" :
                    BoundingType.THROUGH_ALL,

                "startBound" :
                    BoundingType.THROUGH_ALL
            }
        );

        opBoolean(
            context,
            id + "cutHoles",
            {
                "tools" :
                    qCreatedBy(
                        id + "holesTool",
                        EntityType.BODY
                    ),

                "targets" :
                    panelBody,

                "operationType" :
                    BooleanOperationType.SUBTRACTION
            }
        );
    }
);
