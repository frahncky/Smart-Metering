FeatureScript 2892;
import(path : "onshape/std/common.fs", version : "2892.0");

/**
 * SMART METERING
 * Peça 09 - Grade de Ventilacao
 *
 * Gera uma grade retangular removível com seis rasgos.
 * Pode ser aplicada na lateral ou traseira do gabinete.
 */

annotation {
    "Feature Type Name" : "Smart Meter - Grade de Ventilacao",
    "Feature Type Description" :
        "Gera uma grade de ventilacao parametrica com rasgos e quatro furos de fixacao."
}
export const smartMeterVentilationGrille = defineFeature(
    function(context is Context, id is Id, definition is map)

    precondition
    {
        annotation { "Group Name" : "Grade", "Collapsed By Default" : false }
        {
            annotation { "Name" : "Largura" }
            isLength(
                definition.grilleWidth,
                { (millimeter) : [50, 90, 140] } as LengthBoundSpec);

            annotation { "Name" : "Altura" }
            isLength(
                definition.grilleHeight,
                { (millimeter) : [20, 36, 70] } as LengthBoundSpec);

            annotation { "Name" : "Espessura" }
            isLength(
                definition.grilleThickness,
                { (millimeter) : [1.5, 2.5, 5] } as LengthBoundSpec);
        }

        annotation { "Group Name" : "Rasgos", "Collapsed By Default" : false }
        {
            annotation { "Name" : "Comprimento dos rasgos" }
            isLength(
                definition.slotLength,
                { (millimeter) : [30, 68, 110] } as LengthBoundSpec);

            annotation { "Name" : "Altura dos rasgos" }
            isLength(
                definition.slotHeight,
                { (millimeter) : [2, 3.5, 7] } as LengthBoundSpec);

            annotation { "Name" : "Passo vertical" }
            isLength(
                definition.slotPitch,
                { (millimeter) : [4, 5, 10] } as LengthBoundSpec);
        }

        annotation { "Group Name" : "Fixacao", "Collapsed By Default" : false }
        {
            annotation { "Name" : "Espacamento horizontal" }
            isLength(
                definition.mountSpacingX,
                { (millimeter) : [55, 78, 120] } as LengthBoundSpec);

            annotation { "Name" : "Espacamento vertical" }
            isLength(
                definition.mountSpacingY,
                { (millimeter) : [18, 24, 50] } as LengthBoundSpec);

            annotation { "Name" : "Diametro dos furos" }
            isLength(
                definition.mountHoleDiameter,
                { (millimeter) : [2, 3.2, 5] } as LengthBoundSpec);
        }
    }

    {
        var W = definition.grilleWidth;
        var H = definition.grilleHeight;
        var T = definition.grilleThickness;

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
            "grilleBase",
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

        var grilleBody =
            qCreatedBy(
                id + "baseExtrude",
                EntityType.BODY
            );

        // ============================================================
        // 2 - SEIS RASGOS + 4 FUROS
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

        var halfSlotL = definition.slotLength / 2;
        var halfSlotH = definition.slotHeight / 2;

        var y1 = -2.5 * definition.slotPitch;
        var y2 = -1.5 * definition.slotPitch;
        var y3 = -0.5 * definition.slotPitch;
        var y4 =  0.5 * definition.slotPitch;
        var y5 =  1.5 * definition.slotPitch;
        var y6 =  2.5 * definition.slotPitch;

        skRectangle(cutSketch, "slot1", {
            "firstCorner" : vector(-halfSlotL, y1 - halfSlotH),
            "secondCorner" : vector(halfSlotL, y1 + halfSlotH)
        });
        skRectangle(cutSketch, "slot2", {
            "firstCorner" : vector(-halfSlotL, y2 - halfSlotH),
            "secondCorner" : vector(halfSlotL, y2 + halfSlotH)
        });
        skRectangle(cutSketch, "slot3", {
            "firstCorner" : vector(-halfSlotL, y3 - halfSlotH),
            "secondCorner" : vector(halfSlotL, y3 + halfSlotH)
        });
        skRectangle(cutSketch, "slot4", {
            "firstCorner" : vector(-halfSlotL, y4 - halfSlotH),
            "secondCorner" : vector(halfSlotL, y4 + halfSlotH)
        });
        skRectangle(cutSketch, "slot5", {
            "firstCorner" : vector(-halfSlotL, y5 - halfSlotH),
            "secondCorner" : vector(halfSlotL, y5 + halfSlotH)
        });
        skRectangle(cutSketch, "slot6", {
            "firstCorner" : vector(-halfSlotL, y6 - halfSlotH),
            "secondCorner" : vector(halfSlotL, y6 + halfSlotH)
        });

        var hx = definition.mountSpacingX / 2;
        var hy = definition.mountSpacingY / 2;
        var holeR = definition.mountHoleDiameter / 2;

        skCircle(cutSketch, "mount1", {
            "center" : vector(-hx, hy),
            "radius" : holeR
        });
        skCircle(cutSketch, "mount2", {
            "center" : vector(hx, hy),
            "radius" : holeR
        });
        skCircle(cutSketch, "mount3", {
            "center" : vector(-hx, -hy),
            "radius" : holeR
        });
        skCircle(cutSketch, "mount4", {
            "center" : vector(hx, -hy),
            "radius" : holeR
        });

        skSolve(cutSketch);

        opExtrude(
            context,
            id + "cutTool",
            {
                "entities" : qSketchRegion(id + "cutSketch"),
                "direction" : vector(0, 0, 1),
                "endBound" : BoundingType.BLIND,
                "endDepth" : T + 1 * millimeter
            }
        );

        opBoolean(
            context,
            id + "cutGrille",
            {
                "tools" :
                    qCreatedBy(
                        id + "cutTool",
                        EntityType.BODY
                    ),
                "targets" : grilleBody,
                "operationType" :
                    BooleanOperationType.SUBTRACTION
            }
        );
    }
);
