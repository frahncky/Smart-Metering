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

## Arquitetura de referência aprovada

```text
Rede elétrica
   |
   +--> Proteção e condicionamento de tensão ----+
   |                                             |
   +--> TCs / condicionamento de corrente -------+--> ADE9430
                                                        |
                                                        | SPI
                                                        v
                                                   MAX32650
                                                        |
                                           ADuM4152 / ADuM6424A
                                                        |
                                                        v
                                                   ESP32-P4
                                              ┌─────────┼─────────┐
                                              │         │         │
                                      Touchscreen     microSD   Ethernet
                                      7" MIPI-DSI              DP83825I
                                              │
                                              +--> RTC MAX31343
                                              +--> RS-485 / Modbus via ADM2867E
                                              +--> Wi-Fi / Bluetooth via ESP32-C5
```

## Componentes centrais

- **ADE9430** — front-end polifásico para medição e qualidade de energia.
- **MAX32650** — MCU de metrologia para aquisição do ADE9430, processamento complementar, calibração e eventos.
- **ADuM4152** — isolamento digital da interface SPI entre os domínios.
- **ADuM6424A** — isolamento e alimentação isolada auxiliar entre os domínios.
- **ESP32-P4** — processador principal da HMI, gráficos, touchscreen e serviços de alto nível.
- **ESP32-C5** — coprocessador de conectividade Wi-Fi/Bluetooth.
- **DP83825I** — PHY Ethernet 10/100 conectado ao MAC do ESP32-P4.
- **ADM2867E** — transceptor RS-485 isolado para Modbus RTU.
- **MAX31343** — RTC para timestamp de eventos e registros.
- **microSD** — armazenamento local de histórico, eventos e formas de onda.
- **Touchscreen 7"** — alvo de 1024 × 600, capacitivo, preferencialmente MIPI-DSI.

## Princípio de arquitetura

O equipamento será dividido em dois domínios:

1. **Domínio metrológico** — entradas de tensão e corrente, ADE9430, MAX32650, calibração, eventos e processamento de qualidade de energia.
2. **Domínio HMI/comunicação** — ESP32-P4, touchscreen, armazenamento, Ethernet, RS-485 e conectividade sem fio.

A separação entre os domínios reduz a influência da carga gráfica e das comunicações sobre a aquisição metrológica e facilita os requisitos de isolamento e segurança.

## Estrutura do repositório

```text
Smart-Metering/
├── hardware/
│   ├── README.md
│   └── Smart-Metering/
│       ├── Smart-Metering.kicad_pro
│       ├── Smart-Metering.kicad_sch
│       └── Smart-Metering.kicad_pcb
├── docs/
│   ├── ARCHITECTURE.md
│   └── REQUIREMENTS.md
└── README.md
```

## Estado

**Fase 1 — núcleo metrológico ADE9430 iniciado no KiCad.**

A arquitetura eletrônica de referência foi consolidada com **ADE9430 + MAX32650 + ESP32-P4 + ESP32-C5**, isolamento com **ADuM4152/ADuM6424A**, Ethernet com **DP83825I**, RS-485 com **ADM2867E** e RTC **MAX31343**.

Próximas etapas:

1. completar alimentação, desacoplamento, clock e reset do ADE9430;
2. implementar os front-ends de tensão e corrente;
3. inserir o MAX32650 e a interface SPI isolada;
4. implementar alimentação e isolamento entre os domínios;
5. desenvolver o bloco ESP32-P4/HMI e as interfaces de comunicação;
6. fechar o PCB e a integração mecânica/3D.

## Referências principais

- Analog Devices ADE9430: https://www.analog.com/en/products/ade9430.html
- Analog Devices AD-PQMON-SL: https://www.analog.com/en/resources/evaluation-hardware-and-software/evaluation-boards-kits/ad-pqmon-sl.html
- Espressif ESP32-P4: https://documentation.espressif.com/esp32-p4_datasheet_en.html
