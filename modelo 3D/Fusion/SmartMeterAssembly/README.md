# Gerador de montagem Smart Metering

Pasta oficial: F:\DevIA\Smart-Metering\modelo 3D\Fusion\SmartMeterAssembly.

Execute SmartMeterAssembly.py pelo painel Scripts e complementos do Autodesk Fusion. A versão atual lê geometry_interfaces.json e mounting.json, gera 11 componentes e exporta F3D, 11 arquivos STEP e interferencias.json para uma nova pasta montagens. O arquivo SmartMeter_Interfaces_Mecanicas.f3d contém a montagem; SmartMeter_Vista_Interna.f3d permite inspecionar os suportes.

A exportação verificada em 20261003_173930_283634 contém 12 sólidos válidos e 55 verificações sem sobreposição volumétrica. Leia ../INTERFACES_MECANICAS.md para as alterações e pendências.

geometry.json mantém a referência original e geometry_fit.json mantém a revisão anterior. Alterações manuais no F3D não são sincronizadas com os arquivos de parâmetros. Tela, placas e bateria reais ainda precisam de modelos e dimensões definidos.
