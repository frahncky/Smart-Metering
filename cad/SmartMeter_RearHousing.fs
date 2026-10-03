FeatureScript 2892;
import(path : "onshape/std/common.fs", version : "2892.0");

/**
 * SMART METERING
 * Peça 02 - Corpo Traseiro
 *
 * Compatível com:
 * cad/SmartMeter_FrontPanel.fs
 *
 * Dimensões padrão:
 * Corpo externo: 220 x 145 x 75 mm
 * Parede: 3 mm
 * Fundo: 3 mm
 * Fixação frontal: 180 x 115 mm
 * Bosses: Ø10 mm, ligados ao fundo da carcaça
 * Alojamento para inserto M3: Ø4.6 mm x 9 mm
 *
 * Convenção:
 * Plano frontal aberto = Z0
 * Fundo externo = Z positivo
 */

annotation {
    "Feature Type Name" : "Smart Meter - Corpo Traseiro",
    "Feature Type Description" :
        "Gera o corpo traseiro parametrico do Smart Metering com cavidade interna e pilares para fixacao da moldura frontal."
}
export const smartMeterRearHousing = defineFeature(
    function(context is Context, id is Id, definition is map)

    precondition
    {
        annotation { "Group Name" : "Gabinete", "Collapsed By Default" : false }
        {
            annotation { "Name" : "Largura externa" }
            isLength(
                definition.bodyWidth,
                { (millimeter) : [150, 220, 400] } as LengthBoundSpec);

            annotation { "Name" : "Altura externa" }
            isLength(
                definition.bodyHeight,
                { (millimeter) : [100, 145, 300] } as LengthBoundSpec);

            annotation { "Name" : "Profundidade" }
            isLength(
                definition.bodyDepth,
                { (millimeter) : [40, 75, 160] } as LengthBoundSpec);

            annotation { "Name" : "Espessura das paredes" }
            isLength(
                definition.wallThickness,
                { (millimeter) : [2, 3, 8] } as LengthBoundSpec);

            annotation { "Name" : "Espessura do fundo" }
            isLength(
                definition.backThickness,
                { (millimeter) : [2, 3, 8] } as LengthBoundSpec);
        }

        annotation { "Group Name" : "Fixacao da moldura frontal", "Collapsed By Default" : false }
        {
            annotation { "Name" : "Distancia horizontal entre pilares" }
            isLength(
                definition.bossSpacingX,
                { (millimeter) : [100, 180, 205] } as LengthBoundSpec);

            annotation { "Name" : "Distancia vertical entre pilares" }
            isLength(
                definition.bossSpacingY,
                { (millimeter) : [60, 115, 135] } as LengthBoundSpec);

            annotation { "Name" : "Diametro externo dos pilares" }
            isLength(
                definition.bossDiameter,
                { (millimeter) : [7, 10, 18] } as LengthBoundSpec);

            annotation { "Name" : "Diametro para inserto M3" }
            isLength(
                definition.insertDiameter,
                { (millimeter) : [3.5, 4.6, 6] } as LengthBoundSpec);

            annotation { "Name" : "Profundidade do alojamento do inserto" }
            isLength(
                definition.insertDepth,
                { (millimeter) : [4, 9, 20] } as LengthBoundSpec);
        }
    }

    {
        var W = definition.bodyWidth;
        var H = definition.bodyHeight;
        var D = definition.bodyDepth;
        var wall = definition.wallThickness;
        var back = definition.backThickness;

        // ============================================================
        // 1 - BLOCO EXTERNO
        // ============================================================

        var outerSketch = newSketchOnPlane(
            context,
            id + "outerSketch",
            {
                "sketchPlane" :
                    plane(
                        vector(0, 0, 0) * millimeter,
                        vector(0, 0, 1)
                    )
            }
        );

        skRectangle(
            outerSketch,
            "outerRectangle",
            {
                "firstCorner" : vector(-W / 2, -H / 2),
                "secondCorner" : vector(W / 2, H / 2)
            }
        );

        skSolve(outerSketch);

        opExtrude(
            context,
            id + "outerExtrude",
            {
                "entities" : qSketchRegion(id + "outerSketch"),
                "direction" : vector(0, 0, 1),
                "endBound" : BoundingType.BLIND,
                "endDepth" : D
            }
        );

        var housingBody =
            qCreatedBy(
                id + "outerExtrude",
                EntityType.BODY
            );

        // ============================================================
        // 2 - CAVIDADE INTERNA
        //    O corte parte de Z=0 e para antes do fundo.
        // ============================================================

        var innerWidth = W - 2 * wall;
        var innerHeight = H - 2 * wall;
        var cavityDepth = D - back;

        var cavitySketch = newSketchOnPlane(
            context,
            id + "cavitySketch",
            {
                "sketchPlane" :
                    plane(
                        vector(0, 0, 0) * millimeter,
                        vector(0, 0, 1)
                    )
            }
        );

        skRectangle(
            cavitySketch,
            "innerRectangle",
            {
                "firstCorner" :
                    vector(
                        -innerWidth / 2,
                        -innerHeight / 2
                    ),
                "secondCorner" :
                    vector(
                        innerWidth / 2,
                        innerHeight / 2
                    )
            }
        );

        skSolve(cavitySketch);

        opExtrude(
            context,
            id + "cavityTool",
            {
                "entities" : qSketchRegion(id + "cavitySketch"),
                "direction" : vector(0, 0, 1),
                "endBound" : BoundingType.BLIND,
                "endDepth" : cavityDepth
            }
        );

        opBoolean(
            context,
            id + "cutCavity",
            {
                "tools" :
                    qCreatedBy(
                        id + "cavityTool",
                        EntityType.BODY
                    ),
                "targets" : housingBody,
                "operationType" : BooleanOperationType.SUBTRACTION
            }
        );

        // ============================================================
        // 3 - PILARES INTERNOS PARA FIXACAO DA MOLDURA
        //
        // CORRECAO:
        // Na versão anterior os pilares tinham somente 12 mm e
        // ficavam desconectados do restante da caixa. Agora eles
        // atravessam toda a profundidade D, intersectando o fundo.
        // ============================================================

        var bossSketch = newSketchOnPlane(
            context,
            id + "bossSketch",
            {
                "sketchPlane" :
                    plane(
                        vector(0, 0, 0) * millimeter,
                        vector(0, 0, 1)
                    )
            }
        );

        var bx = definition.bossSpacingX / 2;
        var by = definition.bossSpacingY / 2;
        var bossR = definition.bossDiameter / 2;

        skCircle(
            bossSketch,
            "boss1",
            {
                "center" : vector(-bx, by),
                "radius" : bossR
            }
        );

        skCircle(
            bossSketch,
            "boss2",
            {
                "center" : vector(bx, by),
                "radius" : bossR
            }
        );

        skCircle(
            bossSketch,
            "boss3",
            {
                "center" : vector(-bx, -by),
                "radius" : bossR
            }
        );

        skCircle(
            bossSketch,
            "boss4",
            {
                "center" : vector(bx, -by),
                "radius" : bossR
            }
        );

        skSolve(bossSketch);

        opExtrude(
            context,
            id + "bossExtrude",
            {
                "entities" : qSketchRegion(id + "bossSketch"),
                "direction" : vector(0, 0, 1),
                "endBound" : BoundingType.BLIND,
                "endDepth" : D
            }
        );

        var bossBodies =
            qCreatedBy(
                id + "bossExtrude",
                EntityType.BODY
            );

        // O corpo principal entra primeiro na query para preservar
        // sua identidade após o UNION.
        opBoolean(
            context,
            id + "joinBosses",
            {
                "tools" :
                    qUnion([
                        housingBody,
                        bossBodies
                    ]),
                "operationType" : BooleanOperationType.UNION
            }
        );

        // ============================================================
        // 4 - ALOJAMENTOS PARA INSERTOS ROSCADOS M3
        //    Abertos pelo plano frontal Z=0.
        // ============================================================

        var insertSketch = newSketchOnPlane(
            context,
            id + "insertSketch",
            {
                "sketchPlane" :
                    plane(
                        vector(0, 0, 0) * millimeter,
                        vector(0, 0, 1)
                    )
            }
        );

        var insertR = definition.insertDiameter / 2;

        skCircle(
            insertSketch,
            "insert1",
            {
                "center" : vector(-bx, by),
                "radius" : insertR
            }
        );

        skCircle(
            insertSketch,
            "insert2",
            {
                "center" : vector(bx, by),
                "radius" : insertR
            }
        );

        skCircle(
            insertSketch,
            "insert3",
            {
                "center" : vector(-bx, -by),
                "radius" : insertR
            }
        );

        skCircle(
            insertSketch,
            "insert4",
            {
                "center" : vector(bx, -by),
                "radius" : insertR
            }
        );

        skSolve(insertSketch);

        opExtrude(
            context,
            id + "insertTool",
            {
                "entities" : qSketchRegion(id + "insertSketch"),
                "direction" : vector(0, 0, 1),
                "endBound" : BoundingType.BLIND,
                "endDepth" : definition.insertDepth
            }
        );

        opBoolean(
            context,
            id + "cutInserts",
            {
                "tools" :
                    qCreatedBy(
                        id + "insertTool",
                        EntityType.BODY
                    ),
                "targets" : housingBody,
                "operationType" : BooleanOperationType.SUBTRACTION
            }
        );
    }
);
