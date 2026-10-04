# Fonte metrológica e clock — seleção preliminar

## Fonte local implementada

J1 recebe 5 V regulados (4,75–5,25 V) de fonte externa; pin1 positivo, pin2 retorno. U2 TPS7A2033PDBVR, SOT-23-5, gera +3V3_ADE dedicado ao ADE9430. Pinagem: 1 IN, 2 GND, 3 EN ligado a IN, 4 NC explicitamente não conectado, 5 OUT. C30/C31: 2,2 µF X7R 10 V, 0805; selecionar MPN garantindo pelo menos 1 µF efetivo na tensão e temperatura de trabalho e ESR até 100 mΩ. Manter desacoplamentos existentes próximos ao ADE.

O TPS7A20 suporta entrada até 6 V em operação e tem tolerância de saída ±1,5% nas condições especificadas: 3,2505–3,3495 V, dentro de 2,97–3,63 V do ADE9430. Reservar 50 mA para o ramo ADE; seu consumo máximo normal especificado é 17 mA. O MCU e a HMI não estão incluídos neste orçamento.

Estimativa térmica usando RthetaJA=187,1 °C/W da placa de referência TI: a 5,25 V e 50 mA, P~97,5 mW e deltaT~18,2 °C, sem parcela de corrente de terra. A 85 °C ambiente, Tj estimada~103,2 °C. Não usar a corrente nominal de 300 mA como orçamento disponível: a 5,25 V/300 mA, P~585 mW e deltaT~109,5 °C. Recalcular com layout, corrente de terra, carga e gabinete finais.

Flags em +5V_MET_IN e GND representam o contrato explícito de alimentação EXTERNA pelo conector J1. Não representam uma fonte AC/DC implementada ou isolação certificada. O domínio GND fica referenciado à rede pelo VN_SENSE. Para bancada inicial, manter sensores de rede desconectados. Antes de operação energizada: definir fonte/isolador com requisitos de tensão de trabalho, categoria, isolação e distâncias compatíveis, além de proteção e isolação das interfaces. Não ligar retorno da HMI/USB ao GND metrológico por padrão. O header J1 é interface de protótipo e não uma barreira de segurança.

## Cristal

Y1: candidato de BOM ABM8-24.576MHZ-8-D1X-T, quatro pads, corpo 3,2 x 2,5 mm, sinais1/3 e terras2/4. Configuração suportada pela tabela da família Abracon: CL8 pF, fundamental, ESR máximo50 ohms, C0 máximo3 pF, tolerância±10 ppm, estabilidade±20 ppm, -40/+85 °C, drive máximo100 µW. Confirmar cotação/disponibilidade do código e desenho de land pattern antes da compra/layout: não foi confirmada disponibilidade comercial de SKU específico.

A soma tolerância+estabilidade chega a30 ppm; aging±2 ppm e erro por carga são adicionais. Não assumir precisão total garantida de±30 ppm. Calibração e ensaio de frequência permanecem necessários conforme a precisão final exigida.

C9/C10 alterados de18 pF para10 pF C0G1% como ponto inicial. Hipótese: capacitância interna4 pF e parasita2 pF POR PINO. CL=(10+4+2)/2=8 pF. Se os parasitas reais mudarem, ajustar os capacitores. Pela fórmula do manual ADI, gmcrit~0,577 mA/V com ESR50 ohms e CL+C0=11 pF; gm mínimo5 mA/V resulta em margem~8,68 vezes. Isto não valida drive ou partida fisicamente.

## Fontes oficiais consultadas em 2026-10-02

- [TI TPS7A20 Rev.H, seções4,5 e6](https://www.ti.com/lit/ds/symlink/tps7a20.pdf): pinagem DBV, capacitores, limites e térmica.
- [ADE9430 Rev.0, tabela1](https://www.analog.com/media/en/technical-documentation/data-sheets/ade9430.pdf): alimentação, consumo e clock.
- [Manual ADI, Crystal Selection e Load Capacitor Calculation](https://wiki.analog.com/resources/eval/user-guides/ade9430): método de gm e exemplo de carga.
- [Abracon ABM8, especificações e Part Numbering](https://abracon.com/Resonators/abm8.pdf): configuração de cristal candidata.

## Próximas verificações

Conferir netlist, ERC e apresentação das folhas; confirmar MPN dos capacitores/cristal, land patterns, partida, drive, ruído da alimentação e resposta do reset. Corrente/TCs, MCU e fonte isolada upstream continuam pendentes. Nenhuma simulação ou medição física foi executada.
## Resultado da revisão e ERC

Revisor independente confirmou pinagem DBV, J1/IN/EN, saída para alimentação do ADE, capacitores e retorno GND. Netlist XML: U2.1/3 em +5V_MET_IN; U2.5 em +3V3_ADE; U2.2 e Y1.2/4 em GND; U2.4 marcado NC. ERC atual: 146 ocorrências, 11 erros e135 avisos. Os11 erros são entradas de corrente/SPI sem driver. Não há mais erro de alimentação. Avisos:72 pontos fora da grade,39 links de footprints não configurados no ambiente,21 interfaces isoladas,2 links da biblioteca power não configurada e1 nomenclatura GND/VN_SENSE. Os avisos de configuração aumentaram com os novos componentes; não foram ocultados.

Comparação: antes142=13 erros+129 avisos; depois146=11 erros+135 avisos. O total aumentou, enquanto os dois erros de alimentação foram resolvidos pela fonte local e pelo contrato explícito de alimentação externa. O instrumento permanece incompleto.

PDF e SVG exportados pelo KiCad. A rasterização para inspeção visual automatizada não foi concluída neste ambiente; a apresentação visual final precisa ser conferida no editor KiCad. Conectividade foi conferida pela netlist e por revisão independente.
