FeatureScript 2892;
import(path : "onshape/std/common.fs", version : "2892.0");

/**
 * SMART METERING
 * Peça 03 - Bandeja Interna
 *
 * Compatível com:
 * cad/SmartMeter_RearHousing.fs
 *
 * Objetivo:
 * - Bandeja removível para as PCBs internas
 * - Separação física entre metrologia/potência e HMI/comunicação
 * - Oito standoffs para duas placas
 * - Passagens de cabos na base
 *
 * Dimensões padrão:
 * Bandeja: 202 x 127 x 3 mm
 * Divisória: 3 mm de espessura x 25 mm de altura
 * Zona de metrologia: 90 mm de largura
 * Standoffs: Ø10 mm x 8 mm acima da bandeja
 * Furos dos standoffs: Ø3.2 mm
 */

annotation {
    "Feature Type Name" : "Smart Meter - Bandeja Interna",
    "Feature Type Description" :
        "Gera a bandeja interna do Smart Metering com divisoria de seguranca, standoffs para duas PCBs e passagens de cabos."
}
export const smartMeterInternalTray = defineFeature(
    function(context is Context, id is Id, definition is map)

    precondition
    {
        annotation { "Group Name" : "Bandeja", "Collapsed By Default" : false }
        {
            annotation { "Name" : "Largura da bandeja" }
            isLength(
                definition.trayWidth,
                { (millimeter) : [150, 202, 212] } as LengthBoundSpec);

            annotation { "Name" : "Altura da bandeja" }
            isLength(
                definition.trayHeight,
                { (millimeter) : [90, 127, 137] } as LengthBoundSpec);

            annotation { "Name" : "Espessura da bandeja" }
            isLength(
                definition.trayThickness,
                { (millimeter) : [2, 3, 6] } as LengthBoundSpec);
        }

        annotation { "Group Name" : "Separacao interna", "Collapsed By Default" : false }
        {
            annotation { "Name" : "Largura da zona de metrologia" }
            isLength(
                definition.metrologyZoneWidth,
                { (millimeter) : [65, 90, 120] } as LengthBoundSpec);

            annotation { "Name" : "Espessura da divisoria" }
            isLength(
                definition.dividerThickness,
                { (millimeter) : [2, 3, 6] } as LengthBoundSpec);

            annotation { "Name" : "Altura da divisoria acima da bandeja" }
            isLength(
                definition.dividerHeight,
                { (millimeter) : [10, 25, 45] } as LengthBoundSpec);
        }

        annotation { "Group Name" : "Standoffs das PCBs", "Collapsed By Default" : false }
        {
            annotation { "Name" : "Diametro externo" }
            isLength(
                definition.standoffDiameter,
                { (millimeter) : [7, 10, 16] } as LengthBoundSpec);

            annotation { "Name" : "Altura acima da bandeja" }
            isLength(
                definition.standoffHeight,
                { (millimeter) : [4, 8, 18] } as LengthBoundSpec);

            annotation { "Name" : "Diametro dos furos" }
            isLength(
                definition.standoffHoleDiameter,
                { (millimeter) : [2.5, 3.2, 5] } as LengthBoundSpec);

            annotation { "Name" : "Espacamento X - PCB metrologia" }
            isLength(
                definition.metrologyHoleSpacingX,
                { (millimeter) : [35, 60, 80] } as LengthBoundSpec);

            annotation { "Name" : "Espacamento Y - PCB metrologia" }
            isLength(
                definition.metrologyHoleSpacingY,
                { (millimeter) : [30, 60, 90] } as LengthBoundSpec);

            annotation { "Name" : "Espacamento X - PCB comunicacao" }
            isLength(
                definition.commHoleSpacingX,
                { (millimeter) : [40, 70, 95] } as LengthBoundSpec);

            annotation { "Name" : "Espacamento Y - PCB comunicacao" }
            isLength(
                definition.commHoleSpacingY,
                { (millimeter) : [30, 50, 90] } as LengthBoundSpec);
        }

        annotation { "Group Name" : "Passagens de cabos", "Collapsed By Default" : false }
        {
            annotation { "Name" : "Largura das passagens" }
            isLength(
                definition.cableSlotWidth,
                { (millimeter) : [5, 8, 16] } as LengthBoundSpec);

            annotation { "Name" : "Comprimento das passagens" }
            isLength(
                definition.cableSlotLength,
                { (millimeter) : [12, 24, 40] } as LengthBoundSpec);
        }
    }

    {
        var W = definition.trayWidth;
        var H = definition.trayHeight;
        var T = definition.trayThickness;

        var dividerX = -W / 2 + definition.metrologyZoneWidth;
        var dividerTotalHeight = T + definition.dividerHeight;
        var standoffTotalHeight = T + definition.standoffHeight;

        // ============================================================
        // 1 - BASE DA BANDEJA
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
            "baseRectangle",
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

        var baseBody =
            qCreatedBy(
                id + "baseExtrude",
                EntityType.BODY
            );

        // ============================================================
        // 2 - DIVISORIA ENTRE METROLOGIA E HMI/COMUNICACAO
        //
        // A divisória começa em Z=0 para intersectar a base,
        // garantindo uma união booleana robusta.
        // ============================================================

        var dividerSketch = newSketchOnPlane(
            context,
            id + "dividerSketch",
            {
                "sketchPlane" :
                    plane(
                        vector(0, 0, 0) * millimeter,
                        vector(0, 0, 1)
                    )
            }
        );

        skRectangle(
            dividerSketch,
            "dividerRectangle",
            {
                "firstCorner" :
                    vector(
                        dividerX - definition.dividerThickness / 2,
                        -H / 2
                    ),
                "secondCorner" :
                    vector(
                        dividerX + definition.dividerThickness / 2,
                        H / 2
                    )
            }
        );

        skSolve(dividerSketch);

        opExtrude(
            context,
            id + "dividerExtrude",
            {
                "entities" : qSketchRegion(id + "dividerSketch"),
                "direction" : vector(0, 0, 1),
                "endBound" : BoundingType.BLIND,
                "endDepth" : dividerTotalHeight
            }
        );

        var dividerBody =
            qCreatedBy(
                id + "dividerExtrude",
                EntityType.BODY
            );

        // ============================================================
        // 3 - STANDOFFS DAS DUAS PCBs
        // ============================================================

        var leftZoneCenterX =
            -W / 2 + definition.metrologyZoneWidth / 2;

        var rightZoneCenterX =
            dividerX + (W / 2 - dividerX) / 2;

        var leftDx = definition.metrologyHoleSpacingX / 2;
        var leftDy = definition.metrologyHoleSpacingY / 2;

        var rightDx = definition.commHoleSpacingX / 2;
        var rightDy = definition.commHoleSpacingY / 2;

        var standR = definition.standoffDiameter / 2;

        var standSketch = newSketchOnPlane(
            context,
            id + "standSketch",
            {
                "sketchPlane" :
                    plane(
                        vector(0, 0, 0) * millimeter,
                        vector(0, 0, 1)
                    )
            }
        );

        // PCB de metrologia - lado esquerdo
        skCircle(standSketch, "m1", {
            "center" : vector(leftZoneCenterX - leftDx, leftDy),
            "radius" : standR
        });
        skCircle(standSketch, "m2", {
            "center" : vector(leftZoneCenterX + leftDx, leftDy),
            "radius" : standR
        });
        skCircle(standSketch, "m3", {
            "center" : vector(leftZoneCenterX - leftDx, -leftDy),
            "radius" : standR
        });
        skCircle(standSketch, "m4", {
            "center" : vector(leftZoneCenterX + leftDx, -leftDy),
            "radius" : standR
        });

        // PCB de comunicação/HMI - lado direito
        skCircle(standSketch, "c1", {
            "center" : vector(rightZoneCenterX - rightDx, rightDy),
            "radius" : standR
        });
        skCircle(standSketch, "c2", {
            "center" : vector(rightZoneCenterX + rightDx, rightDy),
            "radius" : standR
        });
        skCircle(standSketch, "c3", {
            "center" : vector(rightZoneCenterX - rightDx, -rightDy),
            "radius" : standR
        });
        skCircle(standSketch, "c4", {
            "center" : vector(rightZoneCenterX + rightDx, -rightDy),
            "radius" : standR
        });

        skSolve(standSketch);

        opExtrude(
            context,
            id + "standExtrude",
            {
                "entities" : qSketchRegion(id + "standSketch"),
                "direction" : vector(0, 0, 1),
                "endBound" : BoundingType.BLIND,
                "endDepth" : standoffTotalHeight
            }
        );

        var standBodies =
            qCreatedBy(
                id + "standExtrude",
                EntityType.BODY
            );

        // ============================================================
        // 4 - UNE BASE + DIVISORIA + STANDOFFS
        // ============================================================

        opBoolean(
            context,
            id + "joinStructure",
            {
                "tools" :
                    qUnion([
                        baseBody,
                        dividerBody,
                        standBodies
                    ]),
                "operationType" :
                    BooleanOperationType.UNION
            }
        );

        var trayBody =
            qCreatedBy(
                id + "joinStructure",
                EntityType.BODY
            );

        // ============================================================
        // 5 - PASSAGENS DE CABOS NA BASE
        //
        // Duas aberturas próximas à divisória, uma de cada lado.
        // ============================================================

        var cableSketch = newSketchOnPlane(
            context,
            id + "cableSketch",
            {
                "sketchPlane" :
                    plane(
                        vector(0, 0, 0) * millimeter,
                        vector(0, 0, 1)
                    )
            }
        );

        var slotOffset = definition.cableSlotWidth * 1.5;

        var leftSlotCenterX =
            dividerX
            - definition.dividerThickness / 2
            - slotOffset;

        var rightSlotCenterX =
            dividerX
            + definition.dividerThickness / 2
            + slotOffset;

        var slotHalfW = definition.cableSlotWidth / 2;
        var slotHalfL = definition.cableSlotLength / 2;

        skRectangle(
            cableSketch,
            "leftCableSlot",
            {
                "firstCorner" :
                    vector(
                        leftSlotCenterX - slotHalfW,
                        -slotHalfL
                    ),
                "secondCorner" :
                    vector(
                        leftSlotCenterX + slotHalfW,
                        slotHalfL
                    )
            }
        );

        skRectangle(
            cableSketch,
            "rightCableSlot",
            {
                "firstCorner" :
                    vector(
                        rightSlotCenterX - slotHalfW,
                        -slotHalfL
                    ),
                "secondCorner" :
                    vector(
                        rightSlotCenterX + slotHalfW,
                        slotHalfL
                    )
            }
        );

        skSolve(cableSketch);

        opExtrude(
            context,
            id + "cableTool",
            {
                "entities" : qSketchRegion(id + "cableSketch"),
                "direction" : vector(0, 0, 1),
                "endBound" : BoundingType.THROUGH_ALL,
                "startBound" : BoundingType.THROUGH_ALL
            }
        );

        opBoolean(
            context,
            id + "cutCableSlots",
            {
                "tools" :
                    qCreatedBy(
                        id + "cableTool",
                        EntityType.BODY
                    ),
                "targets" : trayBody,
                "operationType" :
                    BooleanOperationType.SUBTRACTION
            }
        );

        // ============================================================
        // 6 - FUROS DOS STANDOFFS
        //
        // Começam no topo da bandeja para não perfurar a base.
        // ============================================================

        var holeSketch = newSketchOnPlane(
            context,
            id + "holeSketch",
            {
                "sketchPlane" :
                    plane(
                        vector(
                            0 * millimeter,
                            0 * millimeter,
                            T
                        ),
                        vector(0, 0, 1)
                    )
            }
        );

        var holeR =
            definition.standoffHoleDiameter / 2;

        skCircle(holeSketch, "hm1", {
            "center" : vector(leftZoneCenterX - leftDx, leftDy),
            "radius" : holeR
        });
        skCircle(holeSketch, "hm2", {
            "center" : vector(leftZoneCenterX + leftDx, leftDy),
            "radius" : holeR
        });
        skCircle(holeSketch, "hm3", {
            "center" : vector(leftZoneCenterX - leftDx, -leftDy),
            "radius" : holeR
        });
        skCircle(holeSketch, "hm4", {
            "center" : vector(leftZoneCenterX + leftDx, -leftDy),
            "radius" : holeR
        });

        skCircle(holeSketch, "hc1", {
            "center" : vector(rightZoneCenterX - rightDx, rightDy),
            "radius" : holeR
        });
        skCircle(holeSketch, "hc2", {
            "center" : vector(rightZoneCenterX + rightDx, rightDy),
            "radius" : holeR
        });
        skCircle(holeSketch, "hc3", {
            "center" : vector(rightZoneCenterX - rightDx, -rightDy),
            "radius" : holeR
        });
        skCircle(holeSketch, "hc4", {
            "center" : vector(rightZoneCenterX + rightDx, -rightDy),
            "radius" : holeR
        });

        skSolve(holeSketch);

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
                    definition.standoffHeight
            }
        );

        opBoolean(
            context,
            id + "cutStandoffHoles",
            {
                "tools" :
                    qCreatedBy(
                        id + "holeTool",
                        EntityType.BODY
                    ),
                "targets" :
                    trayBody,
                "operationType" :
                    BooleanOperationType.SUBTRACTION
            }
        );
    }
);
