# Smart Metering — interfaces mecânicas

Versão de referência: montagens/20261003_173930_283634.
Arquivo principal: SmartMeter_Interfaces_Mecanicas.f3d.

## Resultado verificado no Autodesk Fusion

11 componentes, 12 corpos com volume positivo. Os 55 pares de componentes foram analisados pela API de interferência do Fusion e apresentaram volume de sobreposição zero. Contatos entre faces são permitidos. O relatório completo está em interferencias.json na pasta versionada. Esta verificação geométrica não comprova resistência, isolamento elétrico ou adequação à fabricação.

## Alterações

- Bandeja: quatro furos de 3,2 mm e apoios correspondentes no gabinete, com alojamentos de insertos de 4,6 mm.
- Três suportes separados: metrologia, HMI e comunicação. Furações de fixação compatíveis com a bandeja; comunicação sobre apoios da HMI.
- Moldura: abertura de 202 × 127 mm e fixação correspondente no painel frontal e gabinete.
- Pés: posicionados sob o gabinete, com furos correspondentes na parede inferior.
- Grade: posicionada sobre o gabinete, com abertura e furações correspondentes na parede superior.

Os suportes são adaptadores mecânicos provisórios, não modelos das placas eletrônicas reais. A distância livre entre a base da HMI e o suporte de comunicação é de 9 mm; depende das alturas dos componentes e conectores. As folgas laterais entre suportes e divisória são geométricas, não distâncias de isolamento elétrico certificadas.

## Arquivos e edição

A pasta oficial é `modelo 3D/Fusion` (relativa à raiz do repositório).
O gerador da montagem é SmartMeterAssembly/SmartMeterAssembly.py; suas dimensões ficam em geometry_interfaces.json e as posições em mounting.json, na mesma subpasta. Executar esse script no Fusion cria um novo documento e exporta uma nova pasta versionada em montagens. Alterações manuais no F3D não retornam automaticamente ao gerador.

geometry.json preserva a geometria de origem; geometry_fit.json preserva a revisão anterior. Os arquivos originais do GitHub e suas exportações continuam separados desta variante.

## Pendências para fechar os encaixes

Selecionar os modelos e obter desenhos dimensionais da tela, das placas e da bateria interna leve. Verificar conectores, cabos, alturas e acesso aos parafusos. Definir material, tolerâncias de impressão, parafusos, insertos e espessura do painel de instalação antes de fabricar. Os diâmetros atuais dos alojamentos são provisórios e devem seguir o fabricante dos insertos escolhidos.

## Ordem preliminar de montagem

Com o gabinete aberto pela frente, instalar fixações dos pés e da grade que precisem de acesso interno. Fixar a bandeja nos quatro apoios; instalar suporte de metrologia, suporte HMI e, depois, suporte de comunicação. Instalar a tela e seu suporte após definir o módulo real. Fechar o painel frontal e montar a moldura. Confirmar o acesso às ferramentas e aos cabos com os componentes reais antes de liberar a fabricação.
