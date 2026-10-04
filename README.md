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
│       ├── Smart-Metering.kicad_pcb      # layout roteado (todas as ligações feitas; DRC do KiCad pendente)
│       ├── Smart-Metering.kicad_dru      # regras de isolação (clearance/creepage)
│       ├── SmartMetering.kicad_sym       # símbolos próprios
│       ├── sym-lib-table
│       └── 3d/                           # modelos 3D simplificados do projeto (gen3d.py)
├── modelo 3D/                            # gabinete: Onshape (FeatureScripts) e Fusion (montagem, STEP)
├── docs/                                 # arquitetura, requisitos, decisões, layout, revisões
├── AGENTS.md                             # papéis e regras de trabalho dos agentes
└── README.md
```

## Estado

**Fase 2 — esquemático completo da placa de metrologia (rev. 0.1, a revisar no KiCad).**

Todas as 9 folhas estão desenhadas e os sinais entre folhas foram conferidos (223 componentes):

| Folha | Conteúdo |
|---|---|
| `01_Power_Input` | Alimentação pelas fases medidas (OU de diodos, qualquer fase presente) ou entrada auxiliar, por montagem; AC/DC isolado RECOM RAC20-05SK/277 (85–305 VAC, 5 V / 4 A) → `VIN_ACDC` |
| `02_Voltage_Sensing` | 3 fases: fusível + varistor, divisor 1/1001 (5 × 200 kΩ + 1 kΩ), anti-aliasing 7,2 kHz; R201 liga neutro ao GND metrológico |
| `03_Current_Sensing` | IA/IB/IC/IN configuráveis por montagem: TC 333 mV (padrão), Rogowski ou SCT-013 de 1 V; TVS, anti-aliasing 7,2 kHz, pino de identificação; headers internos para conectores M8 de painel |
| `04_ADE9430` | ADE9430 (U401), desacoplamento, cristal 24,576 MHz, reset |
| `05_Metrology_MCU` | STM32F413RHT6 (mesma família da referência da biblioteca ADSW-PQ-CLS), HSE 8 MHz, RTC com LSE + CR2032, EEPROM de calibração 24LC64, SWD, LEDs |
| `06_Isolation` | Barreira reforçada logo após o ADE9430: ISO7762 + ISO7761, MCP3204 (ID dos sensores), ADuM6000 + TPS7A20 → `+3V3_ADE` |
| `07_HMI_Interface` | Conector para a placa HMI (ESP32-P4): +5V com PTC, UART, IRQ e EN |
| `08_Communications` | RS-485/Modbus isolada (ADM2587E) e USB-C de serviço (USB FS + carga) |
| `09_Power_Supplies` | OU de entradas 5 V, carregador chaveado BQ25895 (entrada até 3 A, I2C), `+3V3` (TPS63001) e `+5V` (TPS61022) |

A HMI (ESP32-P4 + ESP32-C6, display 7", Ethernet, Wi-Fi/BT, microSD) fica em placa separada (REQ-010). No protótipo: Espressif ESP32-P4-Function-EV-Board.

Componentes de terceiros usam os símbolos oficiais da biblioteca KiCad 10; a biblioteca `SmartMetering` guarda só os símbolos próprios. Referências numeradas por folha (1xx na folha 01, 2xx na 02, …).

### Próximos passos

Revisão completa em [`docs/REVISAO_ESQUEMATICO.md`](docs/REVISAO_ESQUEMATICO.md); as pendências P1–P9 estão resolvidas.

- Rodar o ERC no KiCad e revisar.
- Confirmações de compra: variante CP-40 do ADE9430, certificação reforçada do ADuM6000, encapsulamento do RAC20-05SK/277.
- Firmware: configurar o BQ25895 por I2C (ICHG, VREG, VINDPM) e desabilitar o VBUS sensing do OTG no STM32.
- Layout da PCB (ver [`docs/LAYOUT.md`](docs/LAYOUT.md)): placa roteada (domínio quente, lado seguro e ilha RS-485) com creepage de 8 mm na barreira; todas as ligações feitas (rev. 0.6); falta preencher as zonas e rodar o DRC do KiCad (inclui creepage); os avisos de largura esperados estão listados no LAYOUT. Todos os componentes têm modelo 3D.

## Bateria interna

Bateria Li-Po plana 1S recarregável, com NTC, carregada pelo BQ25895 com o medidor ligado à rede (folha 09, J901). Autonomia a definir; a meta anterior de 13 horas foi retirada. Ver [bateria e alimentação](docs/INTERNAL_BATTERY.md).

## Método de trabalho

Papéis dos agentes e regras de colaboração: [AGENTS.md](AGENTS.md). Etapas, revisão e rastreabilidade: [docs/WORKFLOW.md](docs/WORKFLOW.md). Decisões de projeto: [docs/DECISIONS.md](docs/DECISIONS.md). Dimensionamentos: [entradas de tensão](docs/VOLTAGE_SENSING.md), [fonte metrológica e clock](docs/POWER_CLOCK.md).

## Referências principais

- Analog Devices ADE9430: https://www.analog.com/en/products/ade9430.html
- Espressif ESP32-P4: https://documentation.espressif.com/esp32-p4_datasheet_en.html
