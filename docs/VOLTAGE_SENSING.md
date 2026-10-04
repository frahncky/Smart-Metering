# Entradas de tensão — dimensionamento

Implementadas na folha `02_Voltage_Sensing.kicad_sch`: três canais fase-neutro para sistemas 3P4W de 127/220 V e 220/380 V, 60 Hz. As etiquetas VAP/VAN, VBP/VBN e VCP/VCN ligam os divisores aos canais de tensão do ADE9430 (U401).

## Dimensionamento

Por fase:
- fusível (F201–F203) e varistor (RV201–RV203) na entrada, folha 02;
- cadeia de cinco resistores de 200 kΩ, 0,1 %, 1206 (R2k1–R2k5), e resistor inferior de 1 kΩ, 0,1 % (R2k6). Razão ideal 1/1001;
- capacitor de 22 nF C0G (C2k1) na entrada positiva; a entrada negativa recebe 1 kΩ (R2k7) e 22 nF (C2k2) para casar impedância e fase;
- ganho do PGA de tensão: 1.

A cadeia de cinco resistores divide a tensão de trabalho e aumenta a distância de escoamento ao longo do divisor no layout (ver `LAYOUT.md`). A resistência de Thévenin vista pela entrada é ~999 Ω, o que dá corte nominal de ~7,24 kHz com 22 nF. A impedância do ADC, as tolerâncias e os parasitas exigem calibração de ganho e fase.

| Tensão fase-neutro | Saída ideal RMS | Pico senoidal ideal |
| --- | --- | --- |
| 127 V | 126,9 mV | 179,4 mV |
| 220 V | 219,8 mV | 310,8 mV |
| 242 V (+10 %) | 241,8 mV | 341,9 mV |
| 264 V (+20 %) | 263,7 mV | 373,0 mV |

O datasheet especifica ±0,6 V por pino e ±1 V diferencial com ganho 1. Os cálculos consideram senoide e não qualificam o circuito para transientes. Em 264 V, cada resistor de 200 kΩ dissipa ~13,9 mW e fica com ~52,7 V RMS (~74,6 V de pico). Especificação: 1206, ≥ 0,25 W, tensão de trabalho ≥ 200 V, TC ≤ 25 ppm/K. Confirmar tensão de pulso e derating no datasheet do resistor escolhido antes de fechar a BOM.

## Referência e pendências

O neutro liga ao GND metrológico por R201 (0 Ω), montado em 3F+N e 1F+N: todo o domínio do ADE9430 fica referenciado à rede. Em rede de três fios sem neutro, R201 não é montado e o GND assume o neutro artificial formado pelos divisores; essa topologia ainda precisa de validação. O valor 380 V é fase-fase no sistema 220/380 V, não a tensão destas entradas.

Antes de energizar: conferir categoria de medição, proteção de surtos, classificação dos componentes e as distâncias no PCB (regras em `Smart-Metering.kicad_dru`).

## Histórico

A versão anterior desta folha (branch `minhas-alteracoes`, 02/10) usava três resistores de 330 kΩ por fase (razão 1/991). A versão implementada usa 5 × 200 kΩ + 1 kΩ, com o mesmo filtro de 7,2 kHz.

## Fontes

- [ADE9430 — datasheet Rev. 0, tabela 1](https://www.analog.com/media/en/technical-documentation/data-sheets/ade9430.pdf).
- [Manual técnico — configuração analógica, filtros e exemplo de calibração](https://wiki.analog.com/resources/eval/user-guides/ade9430).
