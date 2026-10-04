# STEP vazios nesta exportação

`SmartMeter_FrontPanel.step` e `SmartMeter_RearHousing.step` desta pasta (e de `20261003_151619_561311`) não têm sólido: o script ocultava essas duas peças para a vista interna antes de exportar os STEP.

O `SmartMeterAssembly.py` agora exporta todos os STEP antes de ocultar as peças. Rode o script de novo no Fusion para gerar uma exportação completa. Até lá, a geometria isolada das duas peças está nos STEP da raiz de `modelo 3D/Fusion/`.
