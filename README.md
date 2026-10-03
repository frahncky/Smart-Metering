# Smart-Metering

Analisador trifásico de energia e qualidade de energia com interface gráfica touchscreen.

## Objetivo

Desenvolver um instrumento completo para medição, visualização e registro de grandezas elétricas em sistemas monofásicos e trifásicos, incluindo:

- tensões e correntes RMS por fase;
- potência ativa, reativa e aparente;
- fator de potência;
- frequência;
- energia importada e exportada;
- demanda;
- diagrama fasorial;
- sequência de fases;
- desequilíbrio de tensão e corrente;
- formas de onda;
- THD;
- espectro harmônico;
- registro de afundamentos, elevações, interrupções e outros eventos de qualidade de energia;
- armazenamento histórico;
- comunicação Ethernet, Wi-Fi e RS-485/Modbus;
- interface local em touchscreen de 7".

## Arquitetura inicial

```text
Rede elétrica
   |
   +--> Proteção e condicionamento de tensão ----+
   |                                             |
   +--> TCs / condicionamento de corrente -------+--> ADE9430
                                                        |
                                                        | SPI
                                                        v
                                              MCU de metrologia
                                                        |
                                              Interface isolada
                                                        |
                                                        v
                                                   ESP32-P4
                                                        |
                                            Touchscreen 7" + LVGL

                     +--> microSD
                     +--> RTC
                     +--> Ethernet
                     +--> RS-485 / Modbus
                     +--> Wi-Fi / Bluetooth via coprocessador
```

## Componentes centrais

- **ADE9430** — front-end de medição polifásica e qualidade de energia.
- **MCU ARM** — aquisição do ADE9430, cálculo complementar, gerenciamento metrológico, calibração e eventos.
- **ESP32-P4** — interface gráfica, touchscreen e serviços de alto nível.
- **ESP32-C6** — opção para Wi-Fi/Bluetooth quando necessário.
- **Touchscreen 7"** — alvo inicial: 1024 × 600, interface capacitiva.

A seleção final do MCU ARM, TCs, display, isoladores, fontes e conectores será feita antes do fechamento do esquemático.

## Estrutura do repositório

```text
Smart-Metering/
├── hardware/
│   ├── README.md
│   └── Smart-Metering/
│       ├── Smart-Metering.kicad_pro
│       ├── Smart-Metering.kicad_sch      # folha raiz hierárquica
│       ├── 01_Power_Input.kicad_sch … 09_Power_Supplies.kicad_sch
│       ├── Smart-Metering.kicad_pcb
│       ├── SmartMetering.kicad_sym       # símbolos próprios
│       └── sym-lib-table
├── cad/                                  # FeatureScripts Onshape do gabinete
├── docs/
│   ├── ARCHITECTURE.md
│   └── REQUIREMENTS.md
└── README.md
```

## Estado

**Fase 2 — esquemático completo da placa de metrologia (rev. 0.1, a revisar no KiCad).**

Todas as 9 folhas estão desenhadas e os sinais entre folhas foram conferidos (215 componentes):

| Folha | Conteúdo |
|---|---|
| `01_Power_Input` | Alimentação pelas fases medidas (OU de diodos, qualquer fase presente) ou entrada auxiliar, por montagem; módulo AC/DC isolado 5 V / 2 A → `VIN_ACDC` |
| `02_Voltage_Sensing` | 3 fases: fusível + varistor, divisor 1/1001 (5 × 200 kΩ + 1 kΩ), anti-aliasing 7,2 kHz; R201 liga neutro ao GND metrológico |
| `03_Current_Sensing` | IA/IB/IC/IN configuráveis por montagem: TC 333 mV (padrão), Rogowski ou SCT-013 de 1 V; TVS, anti-aliasing 7,2 kHz, pino de identificação do sensor |
| `04_ADE9430` | ADE9430 (U401), desacoplamento, cristal 24,576 MHz, reset |
| `05_Metrology_MCU` | STM32F413RHT6 (mesma família da referência da biblioteca ADSW-PQ-CLS), HSE 8 MHz, RTC com LSE + CR2032, SWD, LEDs |
| `06_Isolation` | Barreira reforçada logo após o ADE9430: ISO7762 + ISO7761, MCP3204 (ID dos sensores), DC/DC isolado + TPS7A20 → `+3V3_ADE` |
| `07_HMI_Interface` | Conector para a placa HMI (ESP32-P4): +5V com PTC, UART, IRQ e EN |
| `08_Communications` | RS-485/Modbus isolada (ADM2587E) e USB-C de serviço (USB FS + carga) |
| `09_Power_Supplies` | OU de entradas 5 V, carregador Li-ion 1S com power path (BQ24074), `+3V3` (TPS63001) e `+5V` (TPS61089) |

A HMI (ESP32-P4 + ESP32-C6, display 7", Ethernet, Wi-Fi/BT, microSD) fica em placa separada (REQ-010). No protótipo: Espressif ESP32-P4-Function-EV-Board.

Componentes de terceiros usam os símbolos oficiais da biblioteca KiCad 10; a biblioteca `SmartMetering` guarda só os símbolos próprios. Referências numeradas por folha (1xx na folha 01, 2xx na 02, …).

### Pendências antes do layout

- Rodar ERC no KiCad e revisar as folhas.
- Escolher: sensores de corrente (333 mV e Rogowski) e conector circular das entradas; fusíveis e varistores; DC/DC isolado PS601 e módulo AC/DC PS101 (isolação para CAT III 300 V); bateria 1S com NTC de 10 kΩ.
- Confirmar no TI WEBENCH os resistores de frequência, limite de corrente e compensação do TPS61089 (R912–R914, C912).
- Confirmar no datasheet do ADE9430 que MISO fica em alta impedância com SS em nível alto (barramento compartilhado com o MCP3204).
- Definir os footprints em aberto (fusíveis, varistores, PS101, PS601, conectores dos sensores).

## Referências principais

- Analog Devices ADE9430: https://www.analog.com/en/products/ade9430.html
- Espressif ESP32-P4: https://documentation.espressif.com/esp32-p4_datasheet_en.html
