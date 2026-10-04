# Bateria interna e alimentação

## Decisão confirmada

Bateria Li-Po plana, integrada ao gabinete e recarregada com o medidor ligado à rede de 127/220 V. Não existe mais o requisito de 13 h. Capacidade e autonomia continuam abertas; o projeto não pressupõe tela sempre apagada nem uso apenas para salvar dados no desligamento.

Configuração: pack 1S protegido (tipicamente 3,7 V nominal, 4,2 V de carga), com NTC de 10 kΩ. Não usar pack 4S de 12,8 V nem bateria externa de 20 Ah como referência.

## Arquitetura implementada (folhas 01, 06, 07 e 09)

Rede → fusíveis e varistores → PS101 RECOM RAC20-05SK/277 (85–305 VAC, 5 V / 4 A) → `VIN_ACDC` → U901 BQ25895 (carregador chaveado com power path NVDC) → `VSYS` →
- U902 TPS63001 (buck-boost) → `+3V3` (MCU e lógica do lado seguro);
- U903 TPS61022 (boost) → `+5V` → F701 (PTC 2 A) → `+5V_HMI` (placa HMI, J701) e U606 ADuM6000 → domínio metrológico isolado.

- **Bateria:** J901 (JST PH de 3 vias): 1 = BAT+ (`VBAT`), 2 = NTC (`BAT_NTC`), 3 = BAT− (`GND_SYS`). Os polos são os terminais protegidos do pack (P+/P−), não a célula bruta.
- **NTC:** divisor do TS a partir do REGN, R901 = 5,23 kΩ (REGN-TS) e R903 = 30,1 kΩ (TS-GND), para NTC 103AT de 10 kΩ.
- **Limite de entrada:** R902 = 120 Ω no ILIM, ~3,0 A.
- **Detecção da fonte:** Q902 une D+/D− do BQ25895 só com o AC/DC presente (DCP, 3 A); só pelo USB-C a entrada fica em 500 mA. Q901 corta o VBUS quando o AC/DC está presente.
- **Medição:** `VBAT_SENSE` = VBAT/2 (R906/R907) para o ADC do MCU, além do ADC interno do BQ25895.
- **Liga/desliga:** SW901 desliga os conversores (PWR_EN).
- **Domínios:** bateria, carregador e HMI ficam no `GND_SYS`, o domínio seguro (o `GND_HMI` da proposta anterior). Nenhuma ligação direta com o GND metrológico, que é o neutro.

O sistema funciona sem bateria quando há rede (NVDC).

## Pendências

- **P1 — carga habilitada por hardware na partida:** hoje ~CE do BQ25895 está em GND, então o carregador começa com os valores padrão (ICHG 2,048 A, VREG 4,208 V) antes de o firmware configurar o pack. Proposta: ligar ~CE a um GPIO do MCU com pull-up, deixando a carga desabilitada até a configuração por I2C, com readback e watchdog.
- **P2 — corte físico de carga:** com EN baixo, o TPS61022 não desconecta a saída (o caminho indutor + diodo de corpo mantém `+5V` ≈ VSYS). Avaliar uma chave de carga em `+5V_HMI` se a HMI precisar ficar realmente desligada.
- **P2 — pack e NTC:** escolher pack, capacidade, conector (corrente nominal e polarização) e a curva real do NTC; conferir os limites do TS.
- **P2 — consumo real:** medir consumo e picos da HMI (tela e backlight) para fechar autonomia, corrente de carga e dimensionamento de conectores e trilhas.

## Candidatos avaliados

| Função | Implementado | Alternativa avaliada |
| --- | --- | --- |
| AC/DC | RECOM RAC20-05SK/277: 85–305 VAC, cobre 264 VAC (+20 %) | Hi-Link HLK-20M05: 85–265 VAC, margem pequena em 220 V +20 % |
| Carregador 1S | BQ25895RTW | — |
| Boost 5 V | TPS61022: chave de 8 A, compensação interna | TPS61088 |

## Partida e firmware

Configurar ICHG, VREG e VINDPM por I2C (sugestão: ICHG 1,0 A, VREG 4,20 V, VINDPM absoluto 4,2 V por causa da queda nos diodos D901/D902). Não substituir o NTC por resistor fixo. Implementar supervisão de bateria baixa, histerese e desligamento controlado antes do corte da proteção do pack. Testar a transição rede/bateria sem perda de aquisição, sem assumir.

## Exemplo de autonomia

A 10 W de carga, 3 V de bateria e 85 % de eficiência, a corrente é ~3,9 A antes de perdas e picos. Um pack 1S de 3 Ah a 3,7 V tem 11,1 Wh: ~0,94 h ideais a 10 W, antes de reserva, corte, envelhecimento e temperatura. Isto não fixa capacidade nem garante duração.

## Histórico

A proposta anterior (branch `minhas-alteracoes`) criava uma folha `10_Battery_Interface` só com os conectores do pack (J2) e do NTC (J3), sem carregador. Na versão implementada, a interface da bateria é o J901 da folha 09, já ligado ao BQ25895.

## Fontes oficiais

- [TI BQ25895, limites e registradores](https://www.ti.com/lit/ds/symlink/bq25895.pdf).
- [TI TPS61022](https://www.ti.com/lit/ds/symlink/tps61022.pdf).
- [RECOM RAC20-K](https://recom-power.com/pdf/Powerline_AC-DC/RAC20-K.pdf).
- [Hi-Link HLK-20M05](https://www.hlktech.net/index.php?id=125).
- [TI, caminho de descarga do boost com EN baixo](https://e2e.ti.com/support/power-management-group/power-management/f/power-management-forum/649719/tps61088-shut-down-problem).
