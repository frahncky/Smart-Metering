# Bateria interna e alimentação — arquitetura atual

## Decisão confirmada

Bateria Li-Po plana, integrada ao gabinete, recarregada com o medidor conectado à rede127/220V. Não existe mais requisito de13h. Capacidade e autonomia permanecem abertas; o projeto não pressupõe tela sempre apagada nem apenas backup de desligamento.

Configuração inicial a avaliar:1S, tensão nominal conforme pack (tipicamente3,7V), carga compatível com seu datasheet. Não usar pack4S12,8V nem bateria externa20Ah como referência atual. O modelo/volume/peso precisam caber no gabinete com espaço para manutenção e expansão conforme fabricante.

## Arquitetura proposta, ainda sem circuito completo

Rede -> proteção -> AC/DC5V -> carregador com power path -> SYS variável -> boost5V -> corte físico de carga -> HMI/display e DC/DC isolado -> metrologia.

Bateria protegida conecta ao BAT do carregador; sensor NTC da bateria conecta ao circuito térmico do carregador. GND_HMI reúne retorno do pack/carregador/HMI. GND existente nas folhas02/04/09 é GND metrológico ligado ao neutro. Nenhum fio une os dois domínios nesta alteração. Fonte AC/DC e isolação entre domínios continuam pendentes.

## Candidatos e envelope de avaliação

- HLK-20M05:5V20W, entrada85–265VAC conforme fabricante, candidata para127/220V. Confirmar dimensões, derating e proteção da entrada. Não conectar bateria diretamente à fonte.
- BQ25895RTWR (sem M): candidato de carregador1S com gerenciamento de alimentação e controleI2C. Não confundir limite de entrada3,25A, capacidadeSYS5A e descargaBAT6A. VersãoM não é substituição direta para célula4,2V por seus defaults.
- TPS61088: candidato de boost5V a partir deSYS; precisa de circuito de corte físico, pois ENbaixo não oferece desconexão completa.

Estes componentes não estão colocados como circuitos implementados no esquemático. O limite de10W é um caso de cálculo para comparar componentes, não consumo confirmado ou restrição de produto.

A10W,3V de bateria e85% eficiência, corrente~3,92A antes perdas/picos. Assim pack, proteção e conector devem ser avaliados para corrente real, não apenas capacidadeAh. Como exemplo,1S3Ah a3,7V contém11,1Wh; a10W e85% eficiência, autonomia ideal~0,94h antes reserva, corte, envelhecimento e temperatura. Não fixa capacidade final nem garante duração.

## Partida e firmware

Carregamento deve permanecer desabilitado por hardware até configuração de tensão/corrente adequada ao pack e checagem térmica. Para o candidatoBQ25895, defaults de corrente não são autorização para carregar a bateria escolhida. Prever CE com estado seguro, configuraçãoI2C, readback e tratamento de reset/watchdog. O circuito deve alimentar o sistema sem bateria quando a rede estiver presente e tolerar transição AC/bateria sem perda de aquisição: testar, não assumir.

Não substituir NTC por resistor fixo para ignorar temperatura. Implementar supervisão de bateria baixa, histerese e desligamento controlado antes do corte da proteção. Prever capacidade de partida do boost e picos da iluminação/display.

## Implementação desta etapa

Folha10_Battery_Interface: J2 pin1=BAT_PACK_P+, pin2=GND_HMI; J3 pin1=BAT_NTC_RAW, pin2=GND_HMI. Polos da bateria são terminais protegidosP+/P-, não ligação à célula bruta. Conector da bateria deverá ser polarizado e ter corrente aprovada; footprint/MPN pendentes. Sensor externo fica fisicamente em contato térmico com pack; valor e curva ainda não definidos. Não conectar carregador enquanto polaridade/sensor não forem confirmados.

Estas interfaces estão sem carregador/boost; a netlist não fornece energia à HMI nem à metrologia a partir da bateria. A alteração cria uma folha de integração e retira o requisito antigo; não libera montagem ou energização.

## Próximas definições

Escolher tela/placaESP32-P4, medir/orçar consumo e picos, definir envelope mecânico, escolher pack e NTC. Depois fechar passivos/indutores, correntes de carga, corte físico e firmware do carregador. Revisar isolação e proteção antes do layout.

## Fontes oficiais

- [Hi-Link20M05](https://www.hlktech.net/index.php?id=125).
- [TI BQ25895, limites e registros](https://www.ti.com/lit/ds/symlink/bq25895.pdf).
- [TI TPS61088](https://www.ti.com/lit/ds/symlink/tps61088.pdf).
- [TI, caminho de descarga com ENbaixo](https://e2e.ti.com/support/power-management-group/power-management/f/power-management-forum/649719/tps61088-shut-down-problem).

## Validação desta etapa

Revisão independente confirmou os terminaisJ2/J3 e a separaçãoGND_HMI/GND. NetlistXML exportada peloKiCad10.0.3, comJ2.1BAT_PACK_P+, J2.2GND_HMI, J3.1BAT_NTC_RAW eJ3.2GND_HMI. ERC148 ocorrências:11 erros e137 avisos. Nenhum erro novo; dois avisos adicionais são footprints ainda não atribuídos aos conectores. Sem testes físicos ou circuito de carregamento completo. Diagrama de arquitetura foi inspecionado visualmente; o PDF do esquemático foi exportado, mas sua rasterização visual não foi verificada neste ambiente.