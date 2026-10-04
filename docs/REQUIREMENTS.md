# Requisitos iniciais

Documento de requisitos preliminar. Valores metrológicos definitivos serão fechados após a definição da classe de precisão e dos sensores.

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

O suporte integral à IEC 61000-4-30 Classe S dependerá da implementação e, quando aplicável, da biblioteca específica da Analog Devices.

## REQ-006 — Interface local

O equipamento deverá possuir:

- touchscreen colorido;
- alvo inicial de 7";
- resolução alvo de 1024 × 600;
- interface gráfica responsiva;
- operação sem necessidade de computador externo.

## REQ-007 — Armazenamento

Deverá haver armazenamento local para:

- histórico;
- eventos;
- configurações;
- registros de calibração;
- capturas de formas de onda.

Tecnologia inicial: microSD + memória não volátil interna.

## REQ-008 — Comunicação

Previstas:

- Ethernet;
- RS-485 / Modbus RTU;
- Wi-Fi;
- Bluetooth;
- USB-C para manutenção.

Protocolos adicionais poderão ser definidos posteriormente.

## REQ-009 — Segurança

O projeto deverá contemplar:

- proteção de entrada;
- isolamento adequado;
- fusíveis/proteções quando aplicável;
- separação física entre alta e baixa tensão;
- distâncias de isolação adequadas;
- gabinete que impeça acesso do usuário às partes energizadas.

## REQ-010 — Modularidade

A placa de metrologia deverá ser funcionalmente separável da placa de HMI, permitindo:

- desenvolvimento independente;
- substituição futura da HMI;
- ensaios metrológicos sem interface gráfica;
- redução da interferência entre subsistemas.

## REQ-011 — Alimentação e bateria interna

Entrada nominal 127 V ou 220 V AC, 60 Hz, sem seletor manual. Bateria interna Li-Po plana recarregável, com proteção independente, sensor de temperatura e gerenciamento automático entre fonte e bateria. Capacidade, dimensões, peso e autonomia ainda serão definidos pelo gabinete e consumo reais. A exigência anterior de 13 horas foi retirada pelo usuário; não limitar implicitamente o uso a salvar dados ou desligar.

O pack deve ser acessível para manutenção, acomodado sem compressão e com folga mecânica conforme fabricante. Avaliar configuração 1S, com tensão de carga compatível com a célula escolhida. A bateria e HMI pertencem ao domínio GND_HMI; não conectar diretamente ao GND metrológico referenciado à rede. Carregamento e descarga devem respeitar limites térmicos e elétricos do pack.

Esquemático: folha 10 contém apenas as interfaces físicas preliminares de bateria e sensor; carregador, boost, corte de descarga, fonte AC/DC e isolação ainda não implementados. Arquitetura e critérios em INTERNAL_BATTERY.md.

## Pendências de definição

- classe de precisão alvo;
- corrente nominal e corrente máxima;
- modelo e relação dos TCs;
- necessidade de corrente de neutro;
- MCU ARM definitivo;
- modelo exato do display;
- estratégia de alimentação;
- topologia de isolação;
- requisitos de certificação;
- gabinete e dimensões mecânicas.
