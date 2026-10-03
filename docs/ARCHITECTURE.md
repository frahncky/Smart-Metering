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

MCU: **STM32F413RHT6** (Cortex-M4F, 100 MHz, 1,5 MB flash, 320 KB RAM, LQFP-64).

Escolhido por ser da mesma família da plataforma de referência da biblioteca ADSW-PQ-CLS (IEC 61000-4-30 Classe S) da Analog Devices, a NUCLEO-F413ZH, o que reduz o risco de portar a biblioteca.

Responsabilidades:

- configuração e leitura do ADE9430 (SPI1 isolado) e do ADC de identificação dos sensores;
- calibração;
- processamento de formas de onda e análise harmônica complementar;
- sequência e desequilíbrio;
- detecção e registro de eventos com timestamp (RTC interno + LSE + CR2032);
- protocolo com a HMI (USART2) e Modbus RTU (USART3 + RS-485 isolada);
- USB de serviço (USB FS);
- watchdog e autodiagnóstico.

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

A HMI fica em **placa separada** (REQ-010), ligada à placa de metrologia pelo conector J701 (+5V, UART, IRQ, EN).

Processador: **ESP32-P4**, com **ESP32-C6** para Wi-Fi 6 e Bluetooth 5.

Protótipo: **Espressif ESP32-P4-Function-EV-Board** com o kit de LCD 7" 1024 × 600 MIPI-DSI. A placa já inclui Ethernet, microSD e USB, cobrindo REQ-007 e REQ-008 do lado da HMI.

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

## 9. Alternativas avaliadas

Em 2026-10-03 foi registrada no `main` uma arquitetura de referência alternativa. A rev. 0.1 do esquemático e do layout mantém os componentes já implementados; a tabela fica como registro para revisões futuras.

| Função | Implementado (rev. 0.1) | Alternativa avaliada |
|---|---|---|
| MCU de metrologia | STM32F413RHT6 | MAX32650 |
| Isolamento SPI/sinais | ISO7762 + ISO7761 | ADuM4152 |
| Alimentação isolada do domínio metrológico | ADuM6000 + TPS7A20 | ADuM6424A |
| RS-485 isolado | ADM2587E | ADM2867E |
| RTC | RTC interno do STM32 com LSE e CR2032 | MAX31343 |
| Wi-Fi/Bluetooth (placa HMI) | ESP32-C6 (placa de avaliação do ESP32-P4) | ESP32-C5 |
| Ethernet PHY (placa HMI) | da placa de avaliação do ESP32-P4 | DP83825I |

Os itens da placa HMI (Wi-Fi/Bluetooth, Ethernet, display MIPI-DSI) não afetam a placa de metrologia e podem ser adotados quando a HMI própria for projetada.
