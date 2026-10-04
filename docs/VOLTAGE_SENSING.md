# Entradas de tensão — revisão preliminar

Implementadas na folha `02_Voltage_Sensing.kicad_sch`: três canais fase-neutro, destinados inicialmente a sistemas 3P4W de 127/220 V e 220/380 V, 60 Hz. As etiquetas VAP/VAN, VBP/VBN e VCP/VCN conectam diretamente aos canais existentes do ADE9430.

## Dimensionamento

Cada fase utiliza três resistores de 330 kΩ em série e um resistor inferior de 1 kΩ, todos de 0,1%. Razão ideal: 1/991. Configurar o ganho de tensão do PGA em 1. Capacitor de 22 nF C0G em cada entrada; a entrada negativa recebe 1 kΩ para GND. A resistência de Thévenin positiva é 998,99 Ω, produzindo corte nominal de 7,24 kHz; a negativa resulta em 7,23 kHz. A impedância do ADC, tolerâncias e parasitas exigem calibração de ganho e fase.

| Tensão fase-neutro | Saída ideal RMS | Pico senoidal ideal |
| --- | --- | --- |
| 127 V | 128,15 mV | 181,24 mV |
| 220 V | 221,998 mV | 313,95 mV |
| 242 V (+10%) | 244,20 mV | 345,35 mV |
| 264 V (+20%) | 266,40 mV | 376,75 mV |

O datasheet especifica ±0,6 V por pino e ±1 V diferencial em ganho 1. Estes cálculos consideram uma senoide e não qualificam o circuito para transientes. Em 264 V, cada resistor de 330 kΩ dissipa aproximadamente 23,4 mW e suporta aproximadamente 124,3 V de pico. Especificação preliminar: encapsulamento 1206, potência mínima 0,25 W, tensão de trabalho mínima 200 V e coeficiente térmico até 25 ppm/K. Confirmar tensão de pulso, derating e disponibilidade no datasheet do componente escolhido antes de fechar a BOM. Capacitores: 22 nF, C0G, 5%, mínimo 25 V; footprints preliminares 0805.

## Referência e pendências

VN_SENSE conecta diretamente ao GND metrológico: este domínio fica referenciado à rede. PHASE_A_SENSE, PHASE_B_SENSE, PHASE_C_SENSE e VN_SENSE são interfaces para a futura folha Power Input; ainda não há conector físico ou proteção de entrada implementados. O circuito não está liberado para energização na rede.

Antes do protótipo energizado: definir categoria de medição, proteção de surtos, componentes com classificação adequada, isolação da alimentação/comunicação e distâncias no PCB. A topologia trifásica 3 fios precisa de referência própria e validação; não conectar fase ao VN_SENSE assumindo equivalência com neutro. O valor 380 V é fase-fase no sistema 220/380 V, não a tensão nominal destas entradas.

Correntes, TCs e burden continuam pendentes. Foi corrigida a orientação vertical dos pinos do símbolo ADE9430 na biblioteca local e na cópia incorporada ao esquemático: as etiquetas existentes estavam espelhadas em relação aos pinos e não conectavam corretamente. O arquivo de configuração local do KiCad foi preservado.

## Fontes

- [ADE9430 — datasheet Rev. 0, tabela 1](https://www.analog.com/media/en/technical-documentation/data-sheets/ade9430.pdf).
- [Manual técnico — configuração analógica, filtros e exemplo de calibração](https://wiki.analog.com/resources/eval/user-guides/ade9430).

## Validação

Exportação de netlist, SVG e PDF pelo KiCad 10.0.3. Os seis sinais de tensão foram conferidos na netlist para ligação a U1. O ERC caiu de 217 para 147 ocorrências após a correção do ADE9430 e para 142 (13 erros, 129 avisos) após a revisão independente e correção dos pads GND do cristal Y1; permanecem avisos de bibliotecas de footprints não configuradas, pontos fora da grade e pendências do circuito existente e das interfaces ainda não implementadas. O ERC não está limpo e esta verificação não constitui aprovação elétrica do instrumento.
