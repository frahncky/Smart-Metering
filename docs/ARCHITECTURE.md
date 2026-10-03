# Arquitetura do sistema

## 1. Princípio de projeto

O instrumento será dividido em dois domínios funcionais:

1. **domínio metrológico**, responsável pela aquisição elétrica, cálculos e eventos;
2. **domínio HMI/comunicação**, responsável pela tela, gráficos, armazenamento de alto nível e conectividade.

Essa separação evita que carga gráfica, rede ou interface do usuário interfiram na medição.

## 2. Cadeia de medição

### Tensão

Entradas previstas:

- VA;
- VB;
- VC;
- VN.

A entrada deverá conter proteção contra surtos/transientes, limitação, divisor resistivo de precisão e filtro anti-aliasing compatível com o ADE9430.

### Corrente

Entradas previstas:

- IA;
- IB;
- IC;
- IN opcional.

A solução inicial será baseada em transformadores de corrente (TCs). O circuito deverá permitir escolha posterior do TC e do burden sem alterar a arquitetura geral.

### Conversão e metrologia

O ADE9430 será o núcleo de aquisição.

Funções previstas:

- aquisição simultânea polifásica;
- tensão e corrente RMS;
- potências;
- energias;
- frequência;
- fator de potência;
- ângulos;
- formas de onda;
- grandezas necessárias para qualidade de energia.

Comunicação ADE9430 ↔ MCU: **SPI**.

## 3. Processador de metrologia

O MCU será da família ARM Cortex-M, com modelo ainda a definir.

Responsabilidades:

- configuração e leitura do ADE9430;
- calibração;
- processamento de formas de onda;
- análise harmônica complementar;
- sequência e desequilíbrio;
- detecção e registro de eventos;
- timestamp;
- protocolo com a HMI;
- watchdog e autodiagnóstico.

Critérios para escolha:

- SPI rápido;
- RAM suficiente para buffers de formas de onda;
- FPU;
- DMA;
- timers precisos;
- boa disponibilidade;
- ferramentas de desenvolvimento maduras.

## 4. Isolação

A barreira de isolação reforçada fica **logo após o ADE9430** (folha `06_Isolation`).

- **Domínio metrológico** (potencial da rede, GND = neutro): entradas de tensão e corrente, ADE9430, ADC de identificação dos sensores.
- **Domínio seguro** (`GND_SYS`): MCU de metrologia, ESP32-P4, display, USB-C, Ethernet, RS-485, microSD.

Elementos da barreira:

- isoladores digitais reforçados (família ISO774x, 5 kVrms) para SPI, IRQ0/IRQ1, DREADY, ZX, CF1/CF2 e RESET;
- DC/DC isolado reforçado (≥ 4 kVAC, tensão de trabalho ≥ 300 VAC) + LDO gerando `+3V3_ADE`;
- distância de escoamento ≥ 8 mm sob a barreira, a confirmar pela IEC 61010-1 para CAT III 300 V.

Com isso o MCU pode ser gravado e depurado sem isolação de bancada, e todas as interfaces acessíveis ao usuário ficam no domínio seguro.

## 5. HMI

Processador: **ESP32-P4**.

Funções:

- LVGL;
- touchscreen;
- gráficos em tempo real;
- formas de onda;
- diagrama fasorial;
- espectro harmônico;
- histórico;
- menus de configuração;
- diagnóstico;
- atualização de firmware.

Tela alvo inicial:

- 7 polegadas;
- 1024 × 600;
- touch capacitivo;
- MIPI-DSI ou RGB, conforme painel escolhido.

## 6. Telas planejadas

### Visão geral

- VA/VB/VC;
- IA/IB/IC;
- P/Q/S;
- FP;
- frequência;
- energia;
- demanda;
- THD;
- sequência de fases.

### Formas de onda

- tensões;
- correntes;
- seleção por fase;
- zoom;
- congelamento;
- evento capturado.

### Fasores

- vetores VA/VB/VC;
- vetores IA/IB/IC;
- ângulos;
- sequência ABC/ACB;
- desequilíbrio.

### Harmônicos

- seleção de tensão ou corrente;
- harmônicos por ordem;
- THD;
- tabela e gráfico de barras.

### Qualidade de energia

- sag;
- swell;
- interrupção;
- desequilíbrio;
- eventos;
- tendência;
- registro associado de forma de onda.

### Energia e demanda

- energia ativa;
- energia reativa;
- importação/exportação;
- demanda;
- máximos;
- histórico.

## 7. Interfaces externas

Previstas:

- Ethernet;
- RS-485 / Modbus RTU;
- USB-C de serviço;
- microSD;
- RTC;
- Wi-Fi;
- Bluetooth.

## 8. Ordem de desenvolvimento

1. requisitos elétricos;
2. alimentação;
3. entrada de tensão;
4. entrada de corrente;
5. ADE9430;
6. MCU de metrologia;
7. isolação;
8. comunicação entre placas;
9. ESP32-P4;
10. display/touch;
11. Ethernet/RS-485/Wi-Fi;
12. PCB;
13. firmware metrológico;
14. HMI;
15. calibração e ensaios.
