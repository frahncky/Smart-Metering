# Método de desenvolvimento

## Etapas e critérios de saída

| Etapa | Evidência necessária para concluir |
| --- | --- |
| Requisitos | Faixas, topologias, precisão alvo e condições de uso definidas; incertezas identificadas |
| Dimensionamento | Cálculos de nominal/pior caso, tolerâncias e limites dos componentes com fontes oficiais |
| Esquemático | Revisão visual, pinagem e netlist conferidas; ERC classificado; erros de conexão resolvidos |
| Layout | Footprints conferidos; regras de isolação e mecânica definidas; DRC e revisão de retorno de corrente |
| Protótipo | Plano de ensaios, pontos de teste e critérios de aceitação registrados; condições para energização verificadas |
| Firmware e calibração | Aquisição e protocolos testados; ganho/fase calibrados com referência conhecida; resultados registrados |

O avanço é por bloco: uma folha concluída não libera todo o instrumento. Requisitos ainda abertos devem permanecer identificados, com impacto e próxima ação.

## Ciclo de trabalho

1. Coordenador registra objetivo e arquivos atribuídos, após verificar alterações locais.
2. Executor implementa uma alteração funcional delimitada e registra decisões.
3. Revisor independente examina evidências, cálculos e conexões, sem editar o trabalho revisado.
4. Executor corrige achados; coordenador repete apenas os checks afetados e integra os resultados.
5. Registrar resultado, limitações e próximo passo. Commits funcionais são preparados quando solicitados; não incluir mudanças locais não relacionadas.

## Prioridades de revisão

- **P0:** risco imediato ou ação que deve ser interrompida.
- **P1:** erro que bloqueia funcionamento, layout ou energização do bloco.
- **P2:** precisão, robustez ou coerência que precisa ser corrigida antes da conclusão da etapa.
- **P3:** melhoria de documentação ou organização.

Um aviso ERC pode representar problema real e um comando com código de saída zero pode conter erros. Não silenciar regras para obter um relatório limpo. Exceções devem ter justificativa específica.

## Rastreabilidade

Estado da `main` (PRs #1 e #2).

| Requisito | Implementação/evidência | Estado |
| --- | --- | --- |
| REQ-001 topologias | Folha 02; `VOLTAGE_SENSING.md` | 3P4W implementado; 3 fios (sem R201) pendente de validação |
| REQ-002 redes | Divisor 5 × 200 kΩ + 1 kΩ (1/1001), 127/220 V fase-neutro | Dimensionado e roteado; sem ensaio físico |
| REQ-003 canais | VAP/VAN, VBP/VBN, VCP/VCN; IA/IB/IC/IN (folha 03) | Tensão e corrente implementadas; TCs a escolher |
| REQ-004/005 metrologia e qualidade | ADE9430 (folha 04), MCU STM32F413 (folha 05) | Hardware completo; firmware e ensaios pendentes |
| REQ-009 segurança | Folhas 01, 02 e 06; regras de isolação em `Smart-Metering.kicad_dru` | Barreira com creepage de 8 mm no layout; DRC do KiCad e IEC 61010-1 pendentes |
| REQ-010 modularidade | Placa HMI separada (J701) | Interface implementada |
| REQ-011 alimentação/bateria | Folha 09, J901; `INTERNAL_BATTERY.md` | Carregador e power path implementados; pack e ~CE (D-011) pendentes |
| Layout | `Smart-Metering.kicad_pcb`; `LAYOUT.md` | Roteado, todas as ligações feitas (rev. 0.6); zonas e DRC do KiCad pendentes |

## Decisões

Registrar escolhas em `DECISIONS.md`, com contexto, alternativa relevante, consequência e condição de revisão. Para testes de bancada, registrar fonte/instrumento, configuração, resultado medido e limite de aceitação; não substituir medição por cálculo ideal.
