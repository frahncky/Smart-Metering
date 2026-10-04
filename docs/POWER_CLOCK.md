# Fonte metrológica e clock do ADE9430

## Alimentação do domínio metrológico

Cadeia implementada (folhas 06 e 09):

`+5V` (domínio seguro, U903 TPS61022) → U606 ADuM6000 (DC/DC isolado isoPower, 5 kVrms) → `+5V_ISO` → U605 TPS7A2033PDBVR → `+3V3_ADE`

- U605 (SOT-23-5): 1 IN e 3 EN em `+5V_ISO`, 2 GND, 5 OUT em `+3V3_ADE`. Capacitores: C611 1 µF na entrada, C612 10 µF e C613 100 nF na saída. Desacoplamentos do ADE9430 (C401–C408) junto dos pinos.
- O TPS7A20 opera com entrada até 6 V e tem tolerância de saída de ±1,5 % (3,2505–3,3495 V), dentro dos 2,97–3,63 V do ADE9430.
- Consumo do domínio metrológico (ADE9430 + lado 2 dos isoladores + MCP3204) estimado abaixo de 100 mA; o ADuM6000 fornece até ~500 mW.
- Estimativa térmica do U605 (RθJA = 187,1 °C/W da placa de referência TI): a 5,25 V e 50 mA, ~97,5 mW e ΔT ~18 °C; a 100 mA, ~195 mW e ΔT ~36 °C. Recalcular com a corrente real e o gabinete.

O GND do domínio metrológico é o neutro (ver `VOLTAGE_SENSING.md`). Nenhum retorno da HMI ou do USB se liga a ele: toda comunicação passa por U601/U602 e a alimentação pelo U606.

## Cristal

Y401: Abracon ABM8-24.576MHZ-8-D1X-T, 3,2 × 2,5 mm, quatro pads (sinais 1/3, terras 2/4). Configuração da família Abracon: CL 8 pF, fundamental, ESR máx. 50 Ω, C0 máx. 3 pF, tolerância ±10 ppm, estabilidade ±20 ppm (−40/+85 °C), drive máx. 100 µW. Confirmar disponibilidade e land pattern antes da compra.

C409/C410: 10 pF C0G 1 % (decisão D-005). Hipótese: capacitância interna de 4 pF e parasita de 2 pF por pino, CL = (10 + 4 + 2)/2 = 8 pF. Ajustar se os parasitas medidos forem outros. Pelo manual da ADI, gmcrit ~0,577 mA/V com ESR 50 Ω e CL + C0 = 11 pF; com gm mínimo de 5 mA/V, margem ~8,7×. Isso não valida partida ou drive em bancada.

Tolerância + estabilidade somam 30 ppm; envelhecimento (±2 ppm) e erro de carga são adicionais. A calibração de frequência continua necessária conforme a precisão final.

## Reset

R401 10 kΩ de pull-up e C411 1 µF: constante de ~10 ms na partida. O MCU também pode reiniciar o ADE9430 pelo isolador U602 (RESET_DRV → R601 1 kΩ → RESET_ADE). Validar com as rampas reais da fonte.

## Histórico

Na versão anterior (branch `minhas-alteracoes`), o ADE9430 era alimentado por 5 V externo num conector de protótipo (J1) com o mesmo regulador TPS7A2033. A versão implementada alimenta o domínio metrológico pelo ADuM6000 a partir do lado seguro.

## Fontes oficiais

- [TI TPS7A20 Rev. H, seções 4, 5 e 6](https://www.ti.com/lit/ds/symlink/tps7a20.pdf): pinagem DBV, capacitores, limites e térmica.
- [ADE9430 Rev. 0, tabela 1](https://www.analog.com/media/en/technical-documentation/data-sheets/ade9430.pdf): alimentação, consumo e clock.
- [Manual ADI, Crystal Selection e Load Capacitor Calculation](https://wiki.analog.com/resources/eval/user-guides/ade9430).
- [Abracon ABM8, especificações e part numbering](https://abracon.com/Resonators/abm8.pdf).

Nenhuma simulação ou medição física foi executada.
