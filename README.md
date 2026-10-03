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

**Fase 1 — núcleo metrológico ADE9430 no KiCad.**

Concluído:

- folha `04_ADE9430` com ADE9430, desacoplamento, cristal de 24,576 MHz e rede de reset;
- símbolos próprios corrigidos para a convenção de coordenadas do KiCad (eixo Y para cima), com todos os pinos ligados aos nets previstos;
- FeatureScripts Onshape do gabinete em `cad/`.

Próxima etapa: implementar os front-ends de tensão (`02_Voltage_Sensing`) e corrente (`03_Current_Sensing`); em seguida `09_Power_Supplies`.

## Referências principais

- Analog Devices ADE9430: https://www.analog.com/en/products/ade9430.html
- Espressif ESP32-P4: https://documentation.espressif.com/esp32-p4_datasheet_en.html
