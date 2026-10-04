# Registro de decisões

Estado de cada decisão em relação ao esquemático e ao layout da `main` (PRs #1 e #2, outubro de 2026).

## D-001 — Separar metrologia e HMI

Estado: implementado.

ADE9430 e MCU de metrologia (STM32F413) fazem a aquisição; a HMI (ESP32-P4) fica em placa separada, ligada por J701. Objetivo: reduzir a interferência da carga gráfica e da rede na aquisição. Ver `ARCHITECTURE.md`.

## D-002 — Iniciar tensão em 3P4W

Estado: implementado no esquemático e no layout, sem ensaio físico.

Três canais fase-neutro com divisor 5 × 200 kΩ + 1 kΩ (1/1001) e filtros de 22 nF (~7,2 kHz). Ganho de tensão 1. Neutro ligado ao GND metrológico por R201 (0 Ω). A versão anterior usava 3 × 330 kΩ (1/991). A topologia sem neutro (R201 não montado) continua pendente de validação. Ver `VOLTAGE_SENSING.md`.

## D-003 — Revisão independente e edição exclusiva por arquivo

Estado: adotado.

Executores e revisores têm responsabilidades separadas. O coordenador integra as mudanças; um único executor modifica cada arquivo KiCad por vez. Isso evita conflitos em arquivos estruturados e permite conferir a implementação com evidência independente. Ver `AGENTS.md` e `WORKFLOW.md`.

## D-004 — Regulador dedicado ao ADE9430

Estado: implementado.

U605 TPS7A2033PDBVR gera `+3V3_ADE` a partir de `+5V_ISO`, a saída do DC/DC isolado U606 ADuM6000. O conector de alimentação externa de protótipo (J1) da versão anterior saiu. Ver `POWER_CLOCK.md`.

## D-005 — Cristal 3225 com CL de 8 pF

Estado: implementado no esquemático (Y401, C409/C410 = 10 pF C0G); confirmação comercial pendente.

Y401: Abracon ABM8-24.576MHZ-8-D1X-T, com sinais nos pinos 1/3 e terra nos pinos 2/4. C409/C410 de 10 pF pressupõem 2 pF de parasita por pino; ajustar depois do layout e do ensaio. Disponibilidade, carga e erro total de frequência ainda precisam ser confirmados.

## D-006 — Li-Po plana interna, sem meta de 13 h

Estado: carregador e power path implementados; pack, capacidade e autonomia pendentes.

Pack 1S protegido com NTC no J901, carregador BQ25895 com power path, `VSYS` alimentando os conversores. Substitui a proposta anterior de pack LiFePO4 4S externo. Ver `INTERNAL_BATTERY.md`.

## D-007 — MCU de metrologia STM32F413

Estado: implementado.

STM32F413RHT6 (mesma família da referência da biblioteca ADSW-PQ-CLS da ADI), com RTC próprio (LSE + CR2032), EEPROM de calibração 24LC64 e SWD. O MAX32650 foi avaliado como alternativa (`ARCHITECTURE.md`, seção 9).

## D-008 — Barreira logo após o ADE9430

Estado: implementado.

ISO7762 + ISO7761 (sinais) e ADuM6000 + TPS7A2033 (alimentação) isolam o domínio metrológico. O MCU, o USB e a HMI ficam no domínio seguro. Creepage mínima de 8 mm na barreira, conferida no layout. As alternativas ADuM4152/ADuM6424A estão em `ARCHITECTURE.md`, seção 9.

## D-009 — AC/DC RECOM RAC20-05SK/277

Estado: implementado.

A RAC20 aceita 85–305 VAC e cobre 220 V +20 % (264 V) com margem. A HLK-20M05 (85–265 VAC) foi considerada e não foi adotada.

## D-010 — Prioridade do AC/DC e detecção da fonte USB

Estado: implementado (revisão do PR #1).

Q901 corta o VBUS quando o AC/DC está presente; Q902 une D+/D− do BQ25895 só com o AC/DC (DCP, 3 A). Só pelo USB-C, a entrada fica limitada a 500 mA por hardware.

## D-011 — Carga desabilitada até a configuração (proposta)

Estado: pendente (P1).

Hoje ~CE está em GND e o BQ25895 carrega com os valores padrão antes do firmware. Proposta: ~CE em GPIO do MCU com pull-up. Ver `INTERNAL_BATTERY.md`.
