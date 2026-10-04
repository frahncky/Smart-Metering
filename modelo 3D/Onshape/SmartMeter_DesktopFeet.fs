FeatureScript 2892;
import(path : "onshape/std/common.fs", version : "2892.0");

/**
 * SMART METERING
 * Peça 08 - Pés de Bancada
 *
 * Gera dois pés independentes para uso desktop/laboratório.
 * Cada pé possui dois furos de fixação M3.
 */

annotation {
    "Feature Type Name" : "Smart Meter - Pes de Bancada",
    "Feature Type Description" :
        "Gera dois pes parametrizados para apoio do Smart Metering em bancada."
}
export const smartMeterDesktopFeet = defineFeature(
    function(context is Context, id is Id, definition is map)

    precondition
    {
        annotation { "Group Name" : "Pes", "Collapsed By Default" : false }
        {
            annotation { "Name" : "Comprimento" }
            isLength(
                definition.footLength,
                { (millimeter) : [30, 55, 90] } as LengthBoundSpec);

            annotation { "Name" : "Largura" }
            isLength(
                definition.footWidth,
                { (millimeter) : [15, 24, 40] } as LengthBoundSpec);

            annotation { "Name" : "Altura" }
            isLength(
                definition.footHeight,
                { (millimeter) : [5, 10, 20] } as LengthBoundSpec);

            annotation { "Name" : "Separacao entre os pes" }
            isLength(
                definition.footSpacing,
                { (millimeter) : [80, 150, 190] } as LengthBoundSpec);
        }

        annotation { "Group Name" : "Fixacao", "Collapsed By Default" : false }
        {
            annotation { "Name" : "Espacamento dos furos em cada pe" }
            isLength(
                definition.holeSpacing,
                { (millimeter) : [15, 35, 60] } as LengthBoundSpec);

            annotation { "Name" : "Diametro dos furos" }
            isLength(
                definition.holeDiameter,
                { (millimeter) : [2.5, 3.2, 5] } as LengthBoundSpec);
        }
    }

    {
        var L = definition.footLength;
        var W = definition.footWidth;
        var H = definition.footHeight;

        var cx1 = -definition.footSpacing / 2;
        var cx2 =  definition.footSpacing / 2;

        // ============================================================
        // 1 - DOIS PES
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
            "leftFoot",
            {
                "firstCorner" : vector(cx1 - L / 2, -W / 2),
                "secondCorner" : vector(cx1 + L / 2,  W / 2)
            }
        );

        skRectangle(
            baseSketch,
            "rightFoot",
            {
                "firstCorner" : vector(cx2 - L / 2, -W / 2),
                "secondCorner" : vector(cx2 + L / 2,  W / 2)
            }
        );

        skSolve(baseSketch);

        opExtrude(
            context,
            id + "feetExtrude",
            {
                "entities" : qSketchRegion(id + "baseSketch"),
                "direction" : vector(0, 0, 1),
                "endBound" : BoundingType.BLIND,
                "endDepth" : H
            }
        );

        var footBodies =
            qCreatedBy(
                id + "feetExtrude",
                EntityType.BODY
            );

        // ============================================================
        // 2 - FUROS DE FIXACAO
        // ============================================================

        var holeSketch = newSketchOnPlane(
            context,
            id + "holeSketch",
            {
                "sketchPlane" :
                    plane(
                        vector(0, 0, 0) * millimeter,
                        vector(0, 0, 1)
                    )
            }
        );

        var dx = definition.holeSpacing / 2;
        var r = definition.holeDiameter / 2;

        skCircle(holeSketch, "l1", {
            "center" : vector(cx1 - dx, 0 * millimeter),
            "radius" : r
        });
        skCircle(holeSketch, "l2", {
            "center" : vector(cx1 + dx, 0 * millimeter),
            "radius" : r
        });

        skCircle(holeSketch, "r1", {
            "center" : vector(cx2 - dx, 0 * millimeter),
            "radius" : r
        });
        skCircle(holeSketch, "r2", {
            "center" : vector(cx2 + dx, 0 * millimeter),
            "radius" : r
        });

        skSolve(holeSketch);

        opExtrude(
            context,
            id + "holeTool",
            {
                "entities" : qSketchRegion(id + "holeSketch"),
                "direction" : vector(0, 0, 1),
                "endBound" : BoundingType.BLIND,
                "endDepth" : H + 1 * millimeter
            }
        );

        opBoolean(
            context,
            id + "cutFootHoles",
            {
                "tools" :
                    qCreatedBy(
                        id + "holeTool",
                        EntityType.BODY
                    ),
                "targets" : footBodies,
                "operationType" :
                    BooleanOperationType.SUBTRACTION
            }
        );
    }
);
