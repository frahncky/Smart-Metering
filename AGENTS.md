# Agentes e regras do Smart Metering

## Coordenação

O agente principal coordena requisitos, distribui tarefas e integra resultados. Use subagentes quando uma revisão independente ou trabalhos em arquivos distintos trouxerem benefício. Não é necessário iniciar todos os papéis em toda tarefa. Os papéis abaixo são instruções de trabalho, não serviços persistentes.

## Papéis

- **Hardware e metrologia:** ADE9430, sensores, proteção, fontes, isolação e MCU. Entregar cálculo, premissas, fontes oficiais, arquivos modificados e pendências.
- **Firmware e interface:** aquisição, calibração, eventos, armazenamento, protocolos e HMI. Definir interfaces com hardware e testes pertinentes antes de integrar.
- **Revisor técnico:** conferir requisitos, cálculos, datasheets, pinagem, netlist e ERC de forma independente. Trabalhar somente em leitura por padrão; reportar achados com evidência e prioridade.

## Trabalho paralelo

- Antes de editar, verificar Git e preservar alterações locais. Não reverter trabalho alheio.
- O coordenador atribui arquivos a cada executor. Somente um agente edita cada arquivo KiCad por vez, incluindo bibliotecas e configuração.
- Revisores não modificam os arquivos que estão revisando. Correções são integradas pelo executor responsável e verificadas novamente.
- Não criar chats, serviços, automações ou agentes permanentes para executar estes papéis sem solicitação do usuário.

## Critérios técnicos

- Usar datasheets e documentos oficiais para limites elétricos e pinagem. Registrar revisão e seção consultadas.
- Manter símbolos e footprints próprios no repositório, com caminhos relativos. Atualizar biblioteca e símbolos incorporados de forma coerente.
- Exportar netlist e executar ERC após alterações de conexão. Sucesso do comando não significa ERC aprovado. Classificar erros, avisos e exceções aceitas explicitamente.
- Verificar visualmente o esquemático e conferir na netlist as conexões relevantes. Para componentes polarizados, conferir também números dos pinos.
- Não afirmar validação física, certificação ou segurança elétrica a partir de simulação/ERC. Marcar circuitos preliminares e registrar pendências antes de energização.
- Documentar mudanças importantes antes do layout. Não avançar ao layout de um bloco com erros de conexão não resolvidos.
- Não fazer commit ou push automaticamente sem instrução do usuário.

## Método

Seguir `docs/WORKFLOW.md`. Toda entrega deve informar alteração, evidências de validação e limitações materiais. Atualizar decisões e rastreabilidade quando houver mudança de arquitetura, interface ou requisito.
