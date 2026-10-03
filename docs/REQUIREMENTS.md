# Requisitos iniciais

Documento de requisitos do Smart-Metering. Valores metrológicos definitivos serão fechados após a definição da classe de precisão, faixa de corrente e sensores.

## REQ-001 — Topologias elétricas

O instrumento deverá suportar, como objetivo de projeto:

- monofásico;
- trifásico 3 fios;
- trifásico 4 fios.

## REQ-002 — Redes alvo

O primeiro protótipo será orientado às redes:

- 127/220 V;
- 220/380 V;
- 60 Hz.

A faixa final de entrada será definida no dimensionamento do front-end.

## REQ-003 — Canais

Mínimo:

- 3 canais de tensão de fase;
- 3 canais de corrente;
- neutro como referência de tensão.

Desejável:

- canal de corrente de neutro.

## REQ-004 — Grandezas

O equipamento deverá medir ou calcular:

- tensão RMS;
- corrente RMS;
- potência ativa;
- potência reativa;
- potência aparente;
- fator de potência;
- frequência;
- energia ativa;
- energia reativa;
- demanda;
- ângulo de fase;
- sequência de fases;
- desequilíbrio.

## REQ-005 — Qualidade de energia

O projeto deverá prever:

- formas de onda;
- THD de tensão;
- THD de corrente;
- harmônicos individuais;
- afundamentos;
- elevações;
- interrupções;
- eventos com timestamp;
- captura de forma de onda associada ao evento.

O núcleo de aquisição será o **ADE9430**. O suporte à IEC 61000-4-30 Classe S será tratado como requisito de projeto e dependerá da implementação e da biblioteca/algoritmos adotados.

## REQ-006 — Arquitetura de processamento

O equipamento deverá utilizar dois domínios separados:

### Domínio metrológico

- ADE9430;
- MAX32650;
- processamento de medição, calibração e eventos;
- isolamento da comunicação com o domínio HMI.

### Domínio HMI/comunicação

- ESP32-P4;
- touchscreen;
- armazenamento;
- Ethernet;
- RS-485;
- Wi-Fi/Bluetooth.

A comunicação entre os domínios deverá ser isolada.

## REQ-007 — Interface local

O equipamento deverá possuir:

- touchscreen colorido;
- alvo de 7";
- resolução de 1024 × 600;
- touch capacitivo;
- interface preferencial MIPI-DSI;
- interface gráfica responsiva;
- operação sem necessidade de computador externo.

## REQ-008 — Armazenamento

Deverá haver armazenamento local para:

- histórico;
- eventos;
- configurações;
- registros de calibração;
- capturas de formas de onda.

Tecnologia inicial: **microSD + memória não volátil interna**.

## REQ-009 — Comunicação

Arquitetura definida:

- Ethernet 10/100 via **DP83825I**;
- RS-485 / Modbus RTU via **ADM2867E**;
- Wi-Fi/Bluetooth via **ESP32-C5**;
- USB-C para manutenção.

Protocolos adicionais poderão ser definidos posteriormente.

## REQ-010 — Isolamento

Arquitetura de referência:

- **ADuM4152** para isolamento da interface SPI;
- **ADuM6424A** para isolamento digital e alimentação isolada auxiliar.

O projeto deverá validar creepage, clearance, tensão de trabalho, categoria de sobretensão e requisitos de segurança antes do fechamento da PCB.

## REQ-011 — RTC

O equipamento deverá usar o **MAX31343** ou equivalente aprovado como RTC principal.

O RTC deverá fornecer timestamp para:

- eventos de qualidade de energia;
- histórico;
- alarmes;
- logs;
- calibração.

## REQ-012 — Segurança

O projeto deverá contemplar:

- proteção de entrada;
- isolamento adequado;
- fusíveis/proteções quando aplicável;
- separação física entre alta e baixa tensão;
- distâncias de isolação adequadas;
- gabinete que impeça acesso do usuário às partes energizadas.

## REQ-013 — Modularidade

A placa de metrologia deverá ser funcionalmente separável da placa de HMI, permitindo:

- desenvolvimento independente;
- substituição futura da HMI;
- ensaios metrológicos sem interface gráfica;
- redução da interferência entre subsistemas.

## Componentes de referência aprovados

| Função | Componente |
|---|---|
| Medição polifásica / PQ | ADE9430 |
| MCU de metrologia | MAX32650 |
| Processador HMI | ESP32-P4 |
| Wi-Fi / Bluetooth | ESP32-C5 |
| Isolamento SPI | ADuM4152 |
| Isolamento / alimentação auxiliar | ADuM6424A |
| Ethernet PHY | DP83825I |
| RS-485 isolado | ADM2867E |
| RTC | MAX31343 |
| Armazenamento | microSD |
| Display | 7", 1024 × 600, capacitivo, preferencialmente MIPI-DSI |

## Pendências de definição

- classe de precisão alvo;
- corrente nominal e corrente máxima;
- modelo e relação dos TCs;
- necessidade de corrente de neutro;
- modelo exato do display;
- estratégia detalhada das fontes de alimentação;
- requisitos de certificação;
- gabinete e dimensões mecânicas finais.
