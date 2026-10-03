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

        annotation { "Group Name" : "Passagens do painel traseiro", "Collapsed By Default" : false }
        {
            annotation { "Name" : "Espacamento horizontal dos parafusos" }
            isLength(
                definition.rearMountSpacingX,
                { (millimeter) : [170, 192, 205] } as LengthBoundSpec);

            annotation { "Name" : "Espacamento vertical dos parafusos" }
            isLength(
                definition.rearMountSpacingY,
                { (millimeter) : [100, 117, 130] } as LengthBoundSpec);

            annotation { "Name" : "Diametro dos furos traseiros" }
            isLength(
                definition.rearMountHoleDiameter,
                { (millimeter) : [2.5, 3.2, 5] } as LengthBoundSpec);
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

        // ============================================================
        // 5 - PASSAGENS NO FUNDO PARA O PAINEL TRASEIRO
        //
        // O painel traseiro externo continua sendo uma peça separada,
        // mas o fundo da carcaça recebe os mesmos recortes principais.
        // Isso mantém a rigidez do gabinete sem deixar uma grande
        // abertura traseira.
        // ============================================================

        var rearSketch = newSketchOnPlane(
            context,
            id + "rearSketch",
            {
                "sketchPlane" :
                    plane(
                        vector(
                            0 * millimeter,
                            0 * millimeter,
                            D - back
                        ),
                        vector(0, 0, 1)
                    )
            }
        );

        // Bornes de tensão
        var voltageCenterX = -55 * millimeter;
        var voltageCenterY = -42 * millimeter;
        var voltageWidth = 78 * millimeter;
        var voltageHeight = 16 * millimeter;

        skRectangle(
            rearSketch,
            "rearVoltage",
            {
                "firstCorner" :
                    vector(
                        voltageCenterX - voltageWidth / 2,
                        voltageCenterY - voltageHeight / 2
                    ),
                "secondCorner" :
                    vector(
                        voltageCenterX + voltageWidth / 2,
                        voltageCenterY + voltageHeight / 2
                    )
            }
        );

        // Quatro entradas de corrente
        var currentY = 5 * millimeter;
        var currentSpacing = 22 * millimeter;
        var currentStartX =
            -55 * millimeter - 1.5 * currentSpacing;
        var currentR = 6 * millimeter;

        skCircle(rearSketch, "rearCurrent1", {
            "center" : vector(currentStartX, currentY),
            "radius" : currentR
        });
        skCircle(rearSketch, "rearCurrent2", {
            "center" : vector(currentStartX + currentSpacing, currentY),
            "radius" : currentR
        });
        skCircle(rearSketch, "rearCurrent3", {
            "center" : vector(currentStartX + 2 * currentSpacing, currentY),
            "radius" : currentR
        });
        skCircle(rearSketch, "rearCurrent4", {
            "center" : vector(currentStartX + 3 * currentSpacing, currentY),
            "radius" : currentR
        });

        // Ethernet
        var ethernetX = 62 * millimeter;
        var ethernetY = 34 * millimeter;
        var ethernetW = 16 * millimeter;
        var ethernetH = 14 * millimeter;

        skRectangle(
            rearSketch,
            "rearEthernet",
            {
                "firstCorner" :
                    vector(
                        ethernetX - ethernetW / 2,
                        ethernetY - ethernetH / 2
                    ),
                "secondCorner" :
                    vector(
                        ethernetX + ethernetW / 2,
                        ethernetY + ethernetH / 2
                    )
            }
        );

        // RS-485
        var rsX = 62 * millimeter;
        var rsY = 4 * millimeter;
        var rsW = 24 * millimeter;
        var rsH = 12 * millimeter;

        skRectangle(
            rearSketch,
            "rearRS485",
            {
                "firstCorner" :
                    vector(
                        rsX - rsW / 2,
                        rsY - rsH / 2
                    ),
                "secondCorner" :
                    vector(
                        rsX + rsW / 2,
                        rsY + rsH / 2
                    )
            }
        );

        // USB-C
        var usbX = 62 * millimeter;
        var usbY = -28 * millimeter;
        var usbW = 10 * millimeter;
        var usbH = 4.5 * millimeter;

        skRectangle(
            rearSketch,
            "rearUSB",
            {
                "firstCorner" :
                    vector(
                        usbX - usbW / 2,
                        usbY - usbH / 2
                    ),
                "secondCorner" :
                    vector(
                        usbX + usbW / 2,
                        usbY + usbH / 2
                    )
            }
        );

        // Furos de fixação do painel traseiro
        var rhx = definition.rearMountSpacingX / 2;
        var rhy = definition.rearMountSpacingY / 2;
        var rearHoleR = definition.rearMountHoleDiameter / 2;

        skCircle(rearSketch, "rearMount1", {
            "center" : vector(-rhx, rhy),
            "radius" : rearHoleR
        });
        skCircle(rearSketch, "rearMount2", {
            "center" : vector(rhx, rhy),
            "radius" : rearHoleR
        });
        skCircle(rearSketch, "rearMount3", {
            "center" : vector(-rhx, -rhy),
            "radius" : rearHoleR
        });
        skCircle(rearSketch, "rearMount4", {
            "center" : vector(rhx, -rhy),
            "radius" : rearHoleR
        });

        skSolve(rearSketch);

        opExtrude(
            context,
            id + "rearCutTool",
            {
                "entities" :
                    qSketchRegion(id + "rearSketch"),
                "direction" :
                    vector(0, 0, 1),
                "endBound" :
                    BoundingType.BLIND,
                "endDepth" :
                    back + 1 * millimeter
            }
        );

        opBoolean(
            context,
            id + "cutRearPassages",
            {
                "tools" :
                    qCreatedBy(
                        id + "rearCutTool",
                        EntityType.BODY
                    ),
                "targets" :
                    housingBody,
                "operationType" :
                    BooleanOperationType.SUBTRACTION
            }
        );
    }
);
