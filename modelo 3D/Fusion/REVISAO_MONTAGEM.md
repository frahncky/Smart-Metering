# Revisão da montagem — resultado verificado

Pasta oficial: F:\DevIA\Smart-Metering\modelo 3D\Fusion.

## Versões

- Originais: nove FeatureScripts em Onshape e geometry.json. O gerador Fusion preserva sua geometria padrão.
- Montagem original diagnosticada: montagens/20261003_144344_814599; o Fusion confirmou interferências de 743,0495 mm³ entre carcaça e suporte da tela, e 942,4778 mm³ entre carcaça e bandeja.
- Variante revisada: montagens/20261003_144531_822532. Pilares recuados em 3 mm, alojamentos de insertos iniciando em Z3, quatro alívios de canto 17×12 mm na base da bandeja. Folga nominal mínima dos pilares à bandeja: 1 mm.

## Verificação

Todos os sólidos finais com volume positivo e posições registradas. Análise nativa do Fusion dos dez pares das cinco peças principais: zero interpenetração volumétrica na variante. Revisão independente confirmou coerência das dimensões e do relatório. Contatos coincidentes foram excluídos da análise.

## Correção do gerador

Cortes agora usam somente participantBodies do próprio componente. Uniões usam corpos e alvo explicitamente do próprio componente. A primeira versão afetava peças alheias e gerou exportações inválidas; as exportações antigas e os relatórios sem diagnóstico não devem ser usados para fabricação.

## Pendências

Não há montagem aprovada para fabricação. Fixação da bandeja, padrões dos suportes de PCB, retenção da moldura de painel, interfaces dos pés/grade, dimensões reais da eletrônica, display e bateria, tolerâncias e folgas elétricas continuam pendentes. Os quatro acessórios são mostrados ao lado, sem posição final definida.
