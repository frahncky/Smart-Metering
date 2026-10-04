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

## Rastreabilidade inicial

| Requisito | Implementação/evidência | Estado |
| --- | --- | --- |
| REQ-001 topologias | Folha 02; `VOLTAGE_SENSING.md` | 3P4W preliminar; 3 fios pendente |
| REQ-002 redes | Divisor 990 kΩ/1 kΩ, cálculo de 127/220 V fase-neutro | Dimensionado; sem ensaio físico |
| REQ-003 canais | VAP/VAN, VBP/VBN, VCP/VCN na netlist | Tensão implementada; corrente pendente |
| REQ-004/005 metrologia e qualidade | ADE9430 na folha 04 | Hardware parcial; firmware e ensaios pendentes |
| REQ-009 segurança | Folhas 01, 06 e 09 | Proteção e isolação não concluídas |
| REQ-010 modularidade | Hierarquia KiCad e arquitetura | Estrutura inicial; interfaces pendentes |
| REQ-011 alimentação/bateria | Folha 10 e INTERNAL_BATTERY.md | Li-Po interna; interfaces preliminares; carga/boost pendentes; 13h retiradas |

## Decisões

Registrar escolhas em `DECISIONS.md`, com contexto, alternativa relevante, consequência e condição de revisão. Para testes de bancada, registrar fonte/instrumento, configuração, resultado medido e limite de aceitação; não substituir medição por cálculo ideal.
