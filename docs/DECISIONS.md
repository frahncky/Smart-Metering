# Registro de decisões

## D-001 — Separar metrologia e HMI

Estado: arquitetura inicial, sujeita à escolha de MCU e isolação.

ADE9430 e MCU metrológico fazem aquisição; ESP32-P4 executa HMI e serviços. Objetivo: reduzir interferência da carga gráfica/rede na aquisição. Interface isolada e alimentação ainda precisam ser dimensionadas. Fonte: `ARCHITECTURE.md`.

## D-002 — Iniciar tensão em 3P4W

Estado: implementação preliminar.

Três canais fase-neutro com divisor 990 kΩ/1 kΩ e filtros de 22 nF. Ganho de tensão previsto: 1. Neutro referenciado ao GND metrológico. Fonte e cálculos: `VOLTAGE_SENSING.md`. A alternativa sem neutro permanece pendente; exige estratégia própria, não apenas mudança de etiqueta. Rever após escolha de proteção, componentes e topologia final.

## D-003 — Revisão independente e edição exclusiva por arquivo

Estado: adotado.

Executores e revisores têm responsabilidades separadas. O coordenador integra mudanças; um único executor modifica cada arquivo KiCad por vez. Evita conflitos em arquivos estruturados e permite conferir a implementação com evidência independente. Os agentes são acionados por tarefa e não permanecem executando em segundo plano.

## D-004 — Regulador dedicado e entrada externa de protótipo

Estado: implementado no esquemático, sem ensaio físico.

TPS7A2033PDBVR regula 5V externo para3,3V dedicado ao ADE9430, orçamento50mA. O orçamento do MCU será separado. J1 define interface de alimentação do protótipo; fonte isolada upstream e requisitos da barreira ainda não estão fechados. Não apresentar este bloco como fonte completa de instrumento conectado à rede. Cálculos e critérios: POWER_CLOCK.md.

## D-005 — Cristal3225 CL8pF

Estado: configuração selecionada, confirmação comercial pendente.

Candidato ABM8-24.576MHZ-8-D1X-T preserva pinagem1/3 sinais e2/4 terra. ABLS de referência ADI não é substituição direta por encapsulamento diferente. C9/C10=10pF pressupõem parasita2pF por pino; ajustar após layout e ensaio. Disponibilidade, carga e erro total de frequência devem ser confirmados.

## D-006 — Li-Po plana interna, sem meta de13h

Estado: formato aprovado; capacidade, autonomia e circuito de carga pendentes.

Substitui a proposta anterior de packLiFePO4 4S externo/grande. Avaliar1S protegido comNTC e carregadorpower path, barramentoHMI e ramo metrológico isolado. Candidatos/cálculos emINTERNAL_BATTERY.md; não representam BOM fechada. D-004 mantém-se válida como regulador local do ADE, não como origem da bateria.
