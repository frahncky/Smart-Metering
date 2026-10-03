FeatureScript 2892;
import(path : "onshape/std/common.fs", version : "2892.0");

/**
 * SMART METERING
 * Peça 06 - Suporte da Touchscreen
 *
 * Função:
 * - Prender o módulo de 7" por trás da moldura frontal
 * - Alinhar com os quatro parafusos da moldura
 * - Deixar abertura para a traseira do display
 * - Deixar passagem inferior para flat/cabo
 *
 * Valores padrão:
 * Suporte: 190 x 125 x 3 mm
 * Abertura do módulo: 169 x 104 mm
 * Fixação: 180 x 115 mm
 * Furos: Ø3.2 mm
 */

annotation {
    "Feature Type Name" : "Smart Meter - Suporte Touchscreen",
    "Feature Type Description" :
        "Gera o suporte traseiro parametrico para o modulo touchscreen de 7 polegadas."
}
export const smartMeterDisplayBracket = defineFeature(
    function(context is Context, id is Id, definition is map)

    precondition
    {
        annotation { "Group Name" : "Suporte", "Collapsed By Default" : false }
        {
            annotation { "Name" : "Largura externa" }
            isLength(
                definition.bracketWidth,
                { (millimeter) : [170, 190, 210] } as LengthBoundSpec);

            annotation { "Name" : "Altura externa" }
            isLength(
                definition.bracketHeight,
                { (millimeter) : [110, 125, 140] } as LengthBoundSpec);

            annotation { "Name" : "Espessura" }
            isLength(
                definition.bracketThickness,
                { (millimeter) : [2, 3, 6] } as LengthBoundSpec);
        }

        annotation { "Group Name" : "Abertura do display", "Collapsed By Default" : false }
        {
            annotation { "Name" : "Largura da abertura" }
            isLength(
                definition.displayClearanceWidth,
                { (millimeter) : [165, 169, 180] } as LengthBoundSpec);

            annotation { "Name" : "Altura da abertura" }
            isLength(
                definition.displayClearanceHeight,
                { (millimeter) : [100, 104, 115] } as LengthBoundSpec);

            annotation { "Name" : "Largura da passagem de cabo" }
            isLength(
                definition.cableNotchWidth,
                { (millimeter) : [10, 24, 45] } as LengthBoundSpec);

            annotation { "Name" : "Profundidade da passagem de cabo" }
            isLength(
                definition.cableNotchDepth,
                { (millimeter) : [5, 10, 18] } as LengthBoundSpec);
        }

        annotation { "Group Name" : "Fixacao", "Collapsed By Default" : false }
        {
            annotation { "Name" : "Espacamento horizontal" }
            isLength(
                definition.mountSpacingX,
                { (millimeter) : [160, 180, 195] } as LengthBoundSpec);

            annotation { "Name" : "Espacamento vertical" }
            isLength(
                definition.mountSpacingY,
                { (millimeter) : [100, 115, 125] } as LengthBoundSpec);

            annotation { "Name" : "Diametro dos furos" }
            isLength(
                definition.mountHoleDiameter,
                { (millimeter) : [2.5, 3.2, 5] } as LengthBoundSpec);
        }
    }

    {
        var W = definition.bracketWidth;
        var H = definition.bracketHeight;
        var T = definition.bracketThickness;

        // ============================================================
        // 1 - PLACA BASE
        // ============================================================

        var baseSketch = newSketchOnPlane(
            context,
            id + "baseSketch",
            {
                "sketchPlane" :
                    plane(
                        vector(0, 0, 0) * millimeter,
                        vector(0, 0, 1)
                    )
            }
        );

        skRectangle(
            baseSketch,
            "base",
            {
                "firstCorner" : vector(-W / 2, -H / 2),
                "secondCorner" : vector(W / 2, H / 2)
            }
        );

        skSolve(baseSketch);

        opExtrude(
            context,
            id + "baseExtrude",
            {
                "entities" : qSketchRegion(id + "baseSketch"),
                "direction" : vector(0, 0, 1),
                "endBound" : BoundingType.BLIND,
                "endDepth" : T
            }
        );

        var bracketBody =
            qCreatedBy(
                id + "baseExtrude",
                EntityType.BODY
            );

        // ============================================================
        // 2 - RECORTES
        // ============================================================

        var cutSketch = newSketchOnPlane(
            context,
            id + "cutSketch",
            {
                "sketchPlane" :
                    plane(
                        vector(0, 0, 0) * millimeter,
                        vector(0, 0, 1)
                    )
            }
        );

        // Abertura principal do display
        skRectangle(
            cutSketch,
            "displayOpening",
            {
                "firstCorner" :
                    vector(
                        -definition.displayClearanceWidth / 2,
                        -definition.displayClearanceHeight / 2
                    ),
                "secondCorner" :
                    vector(
                        definition.displayClearanceWidth / 2,
                        definition.displayClearanceHeight / 2
                    )
            }
        );

        // Passagem inferior para flat/cabos
        var notchY1 =
            -H / 2;

        var notchY2 =
            -H / 2 + definition.cableNotchDepth;

        skRectangle(
            cutSketch,
            "cableNotch",
            {
                "firstCorner" :
                    vector(
                        -definition.cableNotchWidth / 2,
                        notchY1
                    ),
                "secondCorner" :
                    vector(
                        definition.cableNotchWidth / 2,
                        notchY2
                    )
            }
        );

        // Quatro furos alinhados à moldura frontal
        var hx =
            definition.mountSpacingX / 2;

        var hy =
            definition.mountSpacingY / 2;

        var holeR =
            definition.mountHoleDiameter / 2;

        skCircle(
            cutSketch,
            "mount1",
            {
                "center" : vector(-hx, hy),
                "radius" : holeR
            }
        );

        skCircle(
            cutSketch,
            "mount2",
            {
                "center" : vector(hx, hy),
                "radius" : holeR
            }
        );

        skCircle(
            cutSketch,
            "mount3",
            {
                "center" : vector(-hx, -hy),
                "radius" : holeR
            }
        );

        skCircle(
            cutSketch,
            "mount4",
            {
                "center" : vector(hx, -hy),
                "radius" : holeR
            }
        );

        skSolve(cutSketch);

        // ============================================================
        // 3 - FERRAMENTA DE CORTE
        // ============================================================

        opExtrude(
            context,
            id + "cutTool",
            {
                "entities" :
                    qSketchRegion(id + "cutSketch"),
                "direction" :
                    vector(0, 0, 1),
                "endBound" :
                    BoundingType.BLIND,
                "endDepth" :
                    T + 1 * millimeter
            }
        );

        // ============================================================
        // 4 - CORTE UNICO
        // ============================================================

        opBoolean(
            context,
            id + "cutBracket",
            {
                "tools" :
                    qCreatedBy(
                        id + "cutTool",
                        EntityType.BODY
                    ),
                "targets" :
                    bracketBody,
                "operationType" :
                    BooleanOperationType.SUBTRACTION
            }
        );
    }
);
