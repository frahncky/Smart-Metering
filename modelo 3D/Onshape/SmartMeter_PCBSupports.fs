FeatureScript 2892;
import(path : "onshape/std/common.fs", version : "2892.0");

/**
 * SMART METERING
 * Peça 05 - Suportes / Adaptadores de PCB
 *
 * Gera três placas independentes:
 * 1) Metrologia
 * 2) HMI / ESP32-P4
 * 3) Comunicação / ESP32-C6
 *
 * Todas as dimensões são paramétricas.
 */

annotation {
    "Feature Type Name" : "Smart Meter - Suportes PCB",
    "Feature Type Description" :
        "Gera tres placas adaptadoras independentes para as PCBs de metrologia, HMI e comunicacao."
}
export const smartMeterPCBSupports = defineFeature(
    function(context is Context, id is Id, definition is map)

    precondition
    {
        annotation { "Group Name" : "Geral", "Collapsed By Default" : false }
        {
            annotation { "Name" : "Espessura das placas" }
            isLength(
                definition.plateThickness,
                { (millimeter) : [2, 3, 6] } as LengthBoundSpec);

            annotation { "Name" : "Diametro dos furos" }
            isLength(
                definition.holeDiameter,
                { (millimeter) : [2.5, 3.2, 5] } as LengthBoundSpec);

            annotation { "Name" : "Separacao entre placas" }
            isLength(
                definition.partGap,
                { (millimeter) : [10, 25, 60] } as LengthBoundSpec);
        }

        annotation { "Group Name" : "Metrologia", "Collapsed By Default" : false }
        {
            annotation { "Name" : "Largura" }
            isLength(
                definition.metWidth,
                { (millimeter) : [50, 85, 130] } as LengthBoundSpec);

            annotation { "Name" : "Altura" }
            isLength(
                definition.metHeight,
                { (millimeter) : [50, 95, 130] } as LengthBoundSpec);

            annotation { "Name" : "Espacamento X dos furos" }
            isLength(
                definition.metHoleX,
                { (millimeter) : [30, 65, 110] } as LengthBoundSpec);

            annotation { "Name" : "Espacamento Y dos furos" }
            isLength(
                definition.metHoleY,
                { (millimeter) : [30, 75, 110] } as LengthBoundSpec);
        }

        annotation { "Group Name" : "HMI / ESP32-P4", "Collapsed By Default" : false }
        {
            annotation { "Name" : "Largura" }
            isLength(
                definition.hmiWidth,
                { (millimeter) : [60, 105, 150] } as LengthBoundSpec);

            annotation { "Name" : "Altura" }
            isLength(
                definition.hmiHeight,
                { (millimeter) : [45, 75, 120] } as LengthBoundSpec);

            annotation { "Name" : "Espacamento X dos furos" }
            isLength(
                definition.hmiHoleX,
                { (millimeter) : [40, 85, 130] } as LengthBoundSpec);

            annotation { "Name" : "Espacamento Y dos furos" }
            isLength(
                definition.hmiHoleY,
                { (millimeter) : [30, 55, 100] } as LengthBoundSpec);
        }

        annotation { "Group Name" : "Comunicacao / ESP32-C6", "Collapsed By Default" : false }
        {
            annotation { "Name" : "Largura" }
            isLength(
                definition.commWidth,
                { (millimeter) : [40, 75, 120] } as LengthBoundSpec);

            annotation { "Name" : "Altura" }
            isLength(
                definition.commHeight,
                { (millimeter) : [35, 55, 100] } as LengthBoundSpec);

            annotation { "Name" : "Espacamento X dos furos" }
            isLength(
                definition.commHoleX,
                { (millimeter) : [25, 55, 100] } as LengthBoundSpec);

            annotation { "Name" : "Espacamento Y dos furos" }
            isLength(
                definition.commHoleY,
                { (millimeter) : [20, 35, 80] } as LengthBoundSpec);
        }
    }

    {
        var T = definition.plateThickness;
        var gap = definition.partGap;

        // Posiciona as três placas lado a lado no mesmo Part Studio.
        var metCenterX =
            -(definition.metWidth / 2 + gap + definition.hmiWidth / 2);

        var hmiCenterX =
            0 * millimeter;

        var commCenterX =
            definition.hmiWidth / 2 + gap + definition.commWidth / 2;

        // ============================================================
        // 1 - SKETCH DAS TRES PLACAS
        // ============================================================

        var plateSketch = newSketchOnPlane(
            context,
            id + "plateSketch",
            {
                "sketchPlane" :
                    plane(
                        vector(0, 0, 0) * millimeter,
                        vector(0, 0, 1)
                    )
            }
        );

        skRectangle(
            plateSketch,
            "metPlate",
            {
                "firstCorner" :
                    vector(
                        metCenterX - definition.metWidth / 2,
                        -definition.metHeight / 2
                    ),
                "secondCorner" :
                    vector(
                        metCenterX + definition.metWidth / 2,
                        definition.metHeight / 2
                    )
            }
        );

        skRectangle(
            plateSketch,
            "hmiPlate",
            {
                "firstCorner" :
                    vector(
                        hmiCenterX - definition.hmiWidth / 2,
                        -definition.hmiHeight / 2
                    ),
                "secondCorner" :
                    vector(
                        hmiCenterX + definition.hmiWidth / 2,
                        definition.hmiHeight / 2
                    )
            }
        );

        skRectangle(
            plateSketch,
            "commPlate",
            {
                "firstCorner" :
                    vector(
                        commCenterX - definition.commWidth / 2,
                        -definition.commHeight / 2
                    ),
                "secondCorner" :
                    vector(
                        commCenterX + definition.commWidth / 2,
                        definition.commHeight / 2
                    )
            }
        );

        skSolve(plateSketch);

        opExtrude(
            context,
            id + "plateExtrude",
            {
                "entities" :
                    qSketchRegion(id + "plateSketch"),
                "direction" :
                    vector(0, 0, 1),
                "endBound" :
                    BoundingType.BLIND,
                "endDepth" :
                    T
            }
        );

        var plateBodies =
            qCreatedBy(
                id + "plateExtrude",
                EntityType.BODY
            );

        // ============================================================
        // 2 - SKETCH DOS FUROS
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

        var holeR =
            definition.holeDiameter / 2;

        var metHX =
            definition.metHoleX / 2;
        var metHY =
            definition.metHoleY / 2;

        var hmiHX =
            definition.hmiHoleX / 2;
        var hmiHY =
            definition.hmiHoleY / 2;

        var commHX =
            definition.commHoleX / 2;
        var commHY =
            definition.commHoleY / 2;

        // Metrologia
        skCircle(holeSketch, "met1", {
            "center" : vector(metCenterX - metHX, metHY),
            "radius" : holeR
        });
        skCircle(holeSketch, "met2", {
            "center" : vector(metCenterX + metHX, metHY),
            "radius" : holeR
        });
        skCircle(holeSketch, "met3", {
            "center" : vector(metCenterX - metHX, -metHY),
            "radius" : holeR
        });
        skCircle(holeSketch, "met4", {
            "center" : vector(metCenterX + metHX, -metHY),
            "radius" : holeR
        });

        // HMI / ESP32-P4
        skCircle(holeSketch, "hmi1", {
            "center" : vector(hmiCenterX - hmiHX, hmiHY),
            "radius" : holeR
        });
        skCircle(holeSketch, "hmi2", {
            "center" : vector(hmiCenterX + hmiHX, hmiHY),
            "radius" : holeR
        });
        skCircle(holeSketch, "hmi3", {
            "center" : vector(hmiCenterX - hmiHX, -hmiHY),
            "radius" : holeR
        });
        skCircle(holeSketch, "hmi4", {
            "center" : vector(hmiCenterX + hmiHX, -hmiHY),
            "radius" : holeR
        });

        // Comunicação / ESP32-C6
        skCircle(holeSketch, "comm1", {
            "center" : vector(commCenterX - commHX, commHY),
            "radius" : holeR
        });
        skCircle(holeSketch, "comm2", {
            "center" : vector(commCenterX + commHX, commHY),
            "radius" : holeR
        });
        skCircle(holeSketch, "comm3", {
            "center" : vector(commCenterX - commHX, -commHY),
            "radius" : holeR
        });
        skCircle(holeSketch, "comm4", {
            "center" : vector(commCenterX + commHX, -commHY),
            "radius" : holeR
        });

        skSolve(holeSketch);

        // ============================================================
        // 3 - CILINDROS DE CORTE
        // ============================================================

        opExtrude(
            context,
            id + "holeTool",
            {
                "entities" :
                    qSketchRegion(id + "holeSketch"),
                "direction" :
                    vector(0, 0, 1),
                "endBound" :
                    BoundingType.BLIND,
                "endDepth" :
                    T + 1 * millimeter
            }
        );

        // ============================================================
        // 4 - CORTE DOS FUROS NAS TRES PLACAS
        // ============================================================

        opBoolean(
            context,
            id + "cutMountingHoles",
            {
                "tools" :
                    qCreatedBy(
                        id + "holeTool",
                        EntityType.BODY
                    ),
                "targets" :
                    plateBodies,
                "operationType" :
                    BooleanOperationType.SUBTRACTION
            }
        );
    }
);
