# Arquitetura do sistema

## 1. Princípio de projeto

O instrumento será dividido em dois domínios funcionais:

1. **domínio metrológico**, responsável pela aquisição elétrica, cálculos, calibração e eventos;
2. **domínio HMI/comunicação**, responsável pela tela, gráficos, armazenamento de alto nível e conectividade.

Essa separação evita que carga gráfica, rede ou interface do usuário interfiram na medição e permite tratar adequadamente isolamento, segurança elétrica e EMC.

## 2. Arquitetura eletrônica de referência

```text
VA/VB/VC/VN ── proteção + condicionamento ──┐
                                           │
IA/IB/IC/IN ── TCs + condicionamento ──────┼──> ADE9430
                                           │       │
                                           │       │ SPI
                                           │       v
                                           │   MAX32650
                                           │       │
                                           │   metrologia
                                           │   calibração
                                           │   eventos PQ
                                           │       │
                                           │  ADuM4152
                                           │  ADuM6424A
                                           │       │
                                           └───────┼────────> ESP32-P4
                                                   │
                            ┌──────────────────────┼──────────────────────┐
                            │                      │                      │
                    Touchscreen 7"             microSD               Ethernet
                    1024 × 600                                       DP83825I
                    MIPI-DSI
                            │
                            ├── MAX31343 — RTC
                            ├── ADM2867E — RS-485 / Modbus RTU
                            └── ESP32-C5 — Wi-Fi / Bluetooth
```

## 3. Cadeia de medição

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

A solução inicial será baseada em transformadores de corrente (TCs). O circuito deverá permitir a escolha posterior do TC e do burden sem alterar a arquitetura geral.

### Conversão e metrologia

O **ADE9430** será o núcleo de aquisição.

Funções previstas:

- aquisição simultânea polifásica;
- tensão e corrente RMS;
- potências;
- energias;
- frequência;
- fator de potência;
- ângulos;
- formas de onda;
- THD e informações para análise harmônica;
- grandezas necessárias para qualidade de energia;
- suporte à estratégia de implementação IEC 61000-4-30 Classe S.

Comunicação ADE9430 ↔ MAX32650: **SPI**.

## 4. Processador de metrologia

Processador selecionado: **MAX32650**.

Responsabilidades:

- configuração e leitura do ADE9430;
- calibração;
- processamento de formas de onda;
- análise harmônica complementar;
- sequência e desequilíbrio;
- detecção e registro de eventos;
- timestamp e sincronização com o RTC;
- protocolo com a HMI;
- watchdog e autodiagnóstico.

A escolha do MAX32650 aproxima o projeto da arquitetura de referência AD-PQMON-SL da Analog Devices e reduz o risco do desenvolvimento do domínio metrológico.

## 5. Isolação

Componentes de referência:

- **ADuM4152** — isolamento da interface SPI;
- **ADuM6424A** — isolamento digital e alimentação isolada auxiliar.

O dimensionamento definitivo deverá considerar:

- tensão de trabalho;
- categoria de sobretensão;
- creepage e clearance;
- estratégia de aterramento;
- EMC;
- requisitos de segurança aplicáveis ao instrumento.

## 6. HMI

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
- atualização de firmware;
- coordenação das interfaces de comunicação e armazenamento.

Tela alvo:

- 7 polegadas;
- 1024 × 600;
- touch capacitivo;
- interface preferencial **MIPI-DSI**.

## 7. Conectividade e periféricos

### Wi-Fi e Bluetooth

Coprocessador selecionado: **ESP32-C5**.

Responsável pela conectividade sem fio do equipamento, mantendo o ESP32-P4 dedicado à HMI e serviços de alto nível.

### Ethernet

PHY selecionado: **DP83825I**.

Ligação ao MAC Ethernet do ESP32-P4 por RMII.

### RS-485 / Modbus RTU

Transceptor selecionado: **ADM2867E**.

A interface deverá ser galvanicamente isolada e protegida para uso industrial.

### RTC

RTC selecionado: **MAX31343**.

Utilizado para:

- timestamp de eventos;
- registros históricos;
- sincronização temporal local;
- manutenção da data/hora durante desligamentos.

### Armazenamento

Armazenamento principal removível: **microSD**.

Uso previsto:

- históricos;
- eventos;
- formas de onda;
- arquivos de configuração;
- registros de calibração;
- atualizações e logs.

## 8. Telas planejadas

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

## 9. Interfaces externas

Previstas:

- Ethernet 10/100;
- RS-485 / Modbus RTU;
- USB-C de serviço;
- microSD;
- RTC;
- Wi-Fi;
- Bluetooth.

## 10. Ordem de desenvolvimento

1. requisitos elétricos;
2. alimentação;
3. entrada de tensão;
4. entrada de corrente;
5. ADE9430;
6. MAX32650;
7. isolamento ADuM4152/ADuM6424A;
8. protocolo entre os domínios;
9. ESP32-P4;
10. display/touch;
11. Ethernet DP83825I;
12. RS-485 ADM2867E;
13. ESP32-C5;
14. RTC MAX31343;
15. PCB;
16. firmware metrológico;
17. HMI;
18. calibração e ensaios.
