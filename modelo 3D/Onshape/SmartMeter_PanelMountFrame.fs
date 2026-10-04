FeatureScript 2892;
import(path : "onshape/std/common.fs", version : "2892.0");

/**
 * SMART METERING
 * Peça 07 - Moldura para Montagem em Painel
 *
 * Função:
 * - Permitir montagem embutida do gabinete em porta de quadro/bancada
 * - Criar flange externo de acabamento
 * - Criar abertura central com folga para o corpo 220 x 145 mm
 * - Criar quatro furos de fixação do conjunto ao painel
 *
 * Valores padrão:
 * Moldura externa: 240 x 165 x 4 mm
 * Abertura central: 222 x 147 mm
 * Fixação: 228 x 153 mm
 * Furos: Ø4.5 mm
 */

annotation {
    "Feature Type Name" : "Smart Meter - Moldura de Painel",
    "Feature Type Description" :
        "Gera a moldura parametrica para instalacao embutida do Smart Metering em painel."
}
export const smartMeterPanelMountFrame = defineFeature(
    function(context is Context, id is Id, definition is map)

    precondition
    {
        annotation { "Group Name" : "Moldura", "Collapsed By Default" : false }
        {
            annotation { "Name" : "Largura externa" }
            isLength(
                definition.frameWidth,
                { (millimeter) : [225, 240, 280] } as LengthBoundSpec);

            annotation { "Name" : "Altura externa" }
            isLength(
                definition.frameHeight,
                { (millimeter) : [150, 165, 200] } as LengthBoundSpec);

            annotation { "Name" : "Espessura" }
            isLength(
                definition.frameThickness,
                { (millimeter) : [2, 4, 8] } as LengthBoundSpec);
        }

        annotation { "Group Name" : "Abertura do gabinete", "Collapsed By Default" : false }
        {
            annotation { "Name" : "Largura da abertura" }
            isLength(
                definition.cutoutWidth,
                { (millimeter) : [220, 222, 230] } as LengthBoundSpec);

            annotation { "Name" : "Altura da abertura" }
            isLength(
                definition.cutoutHeight,
                { (millimeter) : [145, 147, 160] } as LengthBoundSpec);
        }

        annotation { "Group Name" : "Fixacao no painel", "Collapsed By Default" : false }
        {
            annotation { "Name" : "Espacamento horizontal" }
            isLength(
                definition.mountSpacingX,
                { (millimeter) : [220, 228, 250] } as LengthBoundSpec);

            annotation { "Name" : "Espacamento vertical" }
            isLength(
                definition.mountSpacingY,
                { (millimeter) : [145, 153, 180] } as LengthBoundSpec);

            annotation { "Name" : "Diametro dos furos" }
            isLength(
                definition.mountHoleDiameter,
                { (millimeter) : [3.5, 4.5, 7] } as LengthBoundSpec);
        }
    }

    {
        var W = definition.frameWidth;
        var H = definition.frameHeight;
        var T = definition.frameThickness;

        // ============================================================
        // 1 - BASE DA MOLDURA
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
            "frame",
            {
                "firstCorner" : vector(-W / 2, -H / 2),
                "secondCorner" : vector(W / 2, H / 2)
            }
        );

        skSolve(baseSketch);

        opExtrude(
            context,
            id + "frameExtrude",
            {
                "entities" : qSketchRegion(id + "baseSketch"),
                "direction" : vector(0, 0, 1),
                "endBound" : BoundingType.BLIND,
                "endDepth" : T
            }
        );

        var frameBody =
            qCreatedBy(
                id + "frameExtrude",
                EntityType.BODY
            );

        // ============================================================
        // 2 - ABERTURA CENTRAL + FUROS
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

        skRectangle(
            cutSketch,
            "housingCutout",
            {
                "firstCorner" :
                    vector(
                        -definition.cutoutWidth / 2,
                        -definition.cutoutHeight / 2
                    ),
                "secondCorner" :
                    vector(
                        definition.cutoutWidth / 2,
                        definition.cutoutHeight / 2
                    )
            }
        );

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
            id + "cutFrame",
            {
                "tools" :
                    qCreatedBy(
                        id + "cutTool",
                        EntityType.BODY
                    ),
                "targets" :
                    frameBody,
                "operationType" :
                    BooleanOperationType.SUBTRACTION
            }
        );
    }
);
