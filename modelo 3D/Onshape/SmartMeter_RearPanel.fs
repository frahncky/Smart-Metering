FeatureScript 2892;
import(path : "onshape/std/common.fs", version : "2892.0");

/**
 * SMART METERING
 * Peça 04 - Painel Traseiro
 *
 * Compatível com:
 * cad/SmartMeter_RearHousing.fs
 *
 * Dimensões padrão:
 * Painel: 208 x 133 x 3 mm
 *
 * Recortes:
 * - Bloco de bornes de tensão
 * - 4 entradas de corrente
 * - RS-485
 * - Ethernet RJ45
 * - USB-C
 * - 4 furos de fixação M3
 */

annotation {
    "Feature Type Name" : "Smart Meter - Painel Traseiro",
    "Feature Type Description" :
        "Gera o painel traseiro do Smart Metering com recortes para bornes, corrente, RS-485, Ethernet e USB-C."
}
export const smartMeterRearPanel = defineFeature(
    function(context is Context, id is Id, definition is map)

    precondition
    {
        annotation { "Group Name" : "Painel", "Collapsed By Default" : false }
        {
            annotation { "Name" : "Largura" }
            isLength(
                definition.panelWidth,
                { (millimeter) : [160, 208, 214] } as LengthBoundSpec);

            annotation { "Name" : "Altura" }
            isLength(
                definition.panelHeight,
                { (millimeter) : [100, 133, 139] } as LengthBoundSpec);

            annotation { "Name" : "Espessura" }
            isLength(
                definition.panelThickness,
                { (millimeter) : [2, 3, 6] } as LengthBoundSpec);
        }

        annotation { "Group Name" : "Fixacao", "Collapsed By Default" : false }
        {
            annotation { "Name" : "Espacamento horizontal" }
            isLength(
                definition.mountSpacingX,
                { (millimeter) : [150, 192, 202] } as LengthBoundSpec);

            annotation { "Name" : "Espacamento vertical" }
            isLength(
                definition.mountSpacingY,
                { (millimeter) : [90, 117, 127] } as LengthBoundSpec);

            annotation { "Name" : "Diametro dos furos" }
            isLength(
                definition.mountHoleDiameter,
                { (millimeter) : [2.5, 3.2, 5] } as LengthBoundSpec);
        }

        annotation { "Group Name" : "Bornes de tensao", "Collapsed By Default" : false }
        {
            annotation { "Name" : "Largura do recorte" }
            isLength(
                definition.voltageWidth,
                { (millimeter) : [50, 78, 100] } as LengthBoundSpec);

            annotation { "Name" : "Altura do recorte" }
            isLength(
                definition.voltageHeight,
                { (millimeter) : [10, 16, 25] } as LengthBoundSpec);
        }

        annotation { "Group Name" : "Entradas de corrente", "Collapsed By Default" : false }
        {
            annotation { "Name" : "Diametro" }
            isLength(
                definition.currentDiameter,
                { (millimeter) : [8, 12, 18] } as LengthBoundSpec);

            annotation { "Name" : "Espacamento entre entradas" }
            isLength(
                definition.currentSpacing,
                { (millimeter) : [16, 22, 30] } as LengthBoundSpec);
        }

        annotation { "Group Name" : "Comunicacao", "Collapsed By Default" : false }
        {
            annotation { "Name" : "Ethernet - largura" }
            isLength(
                definition.ethernetWidth,
                { (millimeter) : [12, 16, 22] } as LengthBoundSpec);

            annotation { "Name" : "Ethernet - altura" }
            isLength(
                definition.ethernetHeight,
                { (millimeter) : [10, 14, 20] } as LengthBoundSpec);

            annotation { "Name" : "RS485 - largura" }
            isLength(
                definition.rs485Width,
                { (millimeter) : [16, 24, 36] } as LengthBoundSpec);

            annotation { "Name" : "RS485 - altura" }
            isLength(
                definition.rs485Height,
                { (millimeter) : [8, 12, 20] } as LengthBoundSpec);

            annotation { "Name" : "USB-C - largura" }
            isLength(
                definition.usbWidth,
                { (millimeter) : [8, 10, 14] } as LengthBoundSpec);

            annotation { "Name" : "USB-C - altura" }
            isLength(
                definition.usbHeight,
                { (millimeter) : [3, 4.5, 8] } as LengthBoundSpec);
        }
    }

    {
        var W = definition.panelWidth;
        var H = definition.panelHeight;
        var T = definition.panelThickness;

        // ============================================================
        // 1 - PAINEL BASE
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
            "panel",
            {
                "firstCorner" : vector(-W / 2, -H / 2),
                "secondCorner" : vector(W / 2, H / 2)
            }
        );

        skSolve(baseSketch);

        opExtrude(
            context,
            id + "panelExtrude",
            {
                "entities" : qSketchRegion(id + "baseSketch"),
                "direction" : vector(0, 0, 1),
                "endBound" : BoundingType.BLIND,
                "endDepth" : T
            }
        );

        var panelBody =
            qCreatedBy(
                id + "panelExtrude",
                EntityType.BODY
            );

        // ============================================================
        // 2 - TODOS OS RECORTES EM UM UNICO SKETCH
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

        // ------------------------------------------------------------
        // BORNES DE TENSAO - lado esquerdo inferior
        // ------------------------------------------------------------

        var voltageCenterX = -55 * millimeter;
        var voltageCenterY = -42 * millimeter;

        skRectangle(
            cutSketch,
            "voltageTerminalCut",
            {
                "firstCorner" :
                    vector(
                        voltageCenterX - definition.voltageWidth / 2,
                        voltageCenterY - definition.voltageHeight / 2
                    ),
                "secondCorner" :
                    vector(
                        voltageCenterX + definition.voltageWidth / 2,
                        voltageCenterY + definition.voltageHeight / 2
                    )
            }
        );

        // ------------------------------------------------------------
        // ENTRADAS DE CORRENTE - quatro circulos
        // ------------------------------------------------------------

        var currentY = 5 * millimeter;
        var currentStartX =
            -55 * millimeter
            - 1.5 * definition.currentSpacing;

        var currentR =
            definition.currentDiameter / 2;

        skCircle(
            cutSketch,
            "current1",
            {
                "center" :
                    vector(
                        currentStartX,
                        currentY
                    ),
                "radius" :
                    currentR
            }
        );

        skCircle(
            cutSketch,
            "current2",
            {
                "center" :
                    vector(
                        currentStartX + definition.currentSpacing,
                        currentY
                    ),
                "radius" :
                    currentR
            }
        );

        skCircle(
            cutSketch,
            "current3",
            {
                "center" :
                    vector(
                        currentStartX + 2 * definition.currentSpacing,
                        currentY
                    ),
                "radius" :
                    currentR
            }
        );

        skCircle(
            cutSketch,
            "current4",
            {
                "center" :
                    vector(
                        currentStartX + 3 * definition.currentSpacing,
                        currentY
                    ),
                "radius" :
                    currentR
            }
        );

        // ------------------------------------------------------------
        // ETHERNET RJ45 - lado direito superior
        // ------------------------------------------------------------

        var ethernetX = 62 * millimeter;
        var ethernetY = 34 * millimeter;

        skRectangle(
            cutSketch,
            "ethernetCut",
            {
                "firstCorner" :
                    vector(
                        ethernetX - definition.ethernetWidth / 2,
                        ethernetY - definition.ethernetHeight / 2
                    ),
                "secondCorner" :
                    vector(
                        ethernetX + definition.ethernetWidth / 2,
                        ethernetY + definition.ethernetHeight / 2
                    )
            }
        );

        // ------------------------------------------------------------
        // RS-485 - lado direito central
        // ------------------------------------------------------------

        var rsX = 62 * millimeter;
        var rsY = 4 * millimeter;

        skRectangle(
            cutSketch,
            "rs485Cut",
            {
                "firstCorner" :
                    vector(
                        rsX - definition.rs485Width / 2,
                        rsY - definition.rs485Height / 2
                    ),
                "secondCorner" :
                    vector(
                        rsX + definition.rs485Width / 2,
                        rsY + definition.rs485Height / 2
                    )
            }
        );

        // ------------------------------------------------------------
        // USB-C - lado direito inferior
        // ------------------------------------------------------------

        var usbX = 62 * millimeter;
        var usbY = -28 * millimeter;

        skRectangle(
            cutSketch,
            "usbCut",
            {
                "firstCorner" :
                    vector(
                        usbX - definition.usbWidth / 2,
                        usbY - definition.usbHeight / 2
                    ),
                "secondCorner" :
                    vector(
                        usbX + definition.usbWidth / 2,
                        usbY + definition.usbHeight / 2
                    )
            }
        );

        // ------------------------------------------------------------
        // FUROS DE FIXACAO
        // ------------------------------------------------------------

        var hx =
            definition.mountSpacingX / 2;

        var hy =
            definition.mountSpacingY / 2;

        var mountR =
            definition.mountHoleDiameter / 2;

        skCircle(
            cutSketch,
            "mount1",
            {
                "center" : vector(-hx, hy),
                "radius" : mountR
            }
        );

        skCircle(
            cutSketch,
            "mount2",
            {
                "center" : vector(hx, hy),
                "radius" : mountR
            }
        );

        skCircle(
            cutSketch,
            "mount3",
            {
                "center" : vector(-hx, -hy),
                "radius" : mountR
            }
        );

        skCircle(
            cutSketch,
            "mount4",
            {
                "center" : vector(hx, -hy),
                "radius" : mountR
            }
        );

        skSolve(cutSketch);

        // ============================================================
        // 3 - EXTRUSAO DOS RECORTES
        // ============================================================

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

        // ============================================================
        // 4 - CORTE UNICO
        // ============================================================

        opBoolean(
            context,
            id + "cutPanel",
            {
                "tools" :
                    qCreatedBy(
                        id + "cutTool",
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
