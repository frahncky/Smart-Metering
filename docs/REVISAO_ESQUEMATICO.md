# Revisão do esquemático — rev. 0.1

Data: 2026-10-03. Escopo: as 9 folhas de `hardware/Smart-Metering/` (placa de metrologia).

## 1. Método

O KiCad não está disponível no ambiente onde a revisão foi feita, então o ERC do KiCad **ainda precisa ser rodado**. No lugar dele:

1. **Netlist global** montado diretamente dos arquivos `.kicad_sch` (215 componentes, 680 pinos, 176 nets), com labels globais entre folhas e labels locais por folha.
2. **Verificações automáticas**: pinos sem ligação, nets de alimentação sem fonte, saídas em conflito, entradas sem acionamento, domínios de terra (isolação) e footprints inexistentes.
3. **Conferência com a biblioteca oficial do KiCad** (símbolos e footprints, gitlab.com/kicad/libraries).
4. **Revisão de engenharia** folha por folha: faixas, valores, orçamento de potência e segurança.

## 2. Resultado das verificações automáticas (após correções)

| Verificação | Resultado |
|---|---|
| Pinos sem ligação e sem flag de não conectado | nenhum |
| Nets de alimentação sem fonte (ERC "power input not driven") | nenhuma (PWR_FLAG adicionados) |
| Nets com mais de uma saída | nenhuma (MISO do ADE9430 marcado como tri-state) |
| Entradas sem acionamento | nenhuma |
| Isolação: componentes que unem terras diferentes | só as barreiras previstas: PS101, PS601, U601, U602, U801 |
| Footprints inexistentes na biblioteca oficial | nenhum |
| Referências duplicadas | nenhuma |

Domínios de terra encontrados:

- `GND` + `HV_RTN`: domínio metrológico e entrada de energia, no potencial da rede;
- `GND_SYS`: domínio seguro;
- `GND_485`: barramento RS-485 isolado.

## 3. Correções feitas nesta revisão

| # | Folha | Problema | Correção |
|---|---|---|---|
| 1 | 04 | Footprint do ADE9430 (`LFCSP-40-1EP_6x6mm…EP4.5x4.5mm`) **não existe** na biblioteca KiCad | Trocado por `QFN-40-1EP_6x6mm_P0.5mm_EP4.6x4.6mm` (ver pendência P1) |
| 2 | 04/06 | MISO do ADE9430 e Dout do MCP3204 no mesmo net, ambos como saída (conflito no ERC) | Pino MISO do símbolo ADE9430 passa a tri-state |
| 3 | 01 | Símbolo genérico do AC/DC com AC/L e AC/N trocados em relação ao IRM-10-5 real | Símbolo e footprint oficiais `Converter_ACDC:IRM-10-5` |
| 4 | 01, 05, 08, 09 | Seis nets de alimentação sem fonte declarada (HV_DC, HV_RTN, VBAT_RTC, VDDA, GND_485, VIN_CHG) | PWR_FLAG em cada uma |
| 5 | 01, 02 | Fusíveis e varistores sem footprint | Porta-fusível 5×20 mm (fusíveis cerâmicos de 500 VAC) e varistor de disco 15,5 mm |
| 6 | 09 | Indutores sem footprint | L901 Bourns SRN4018 (2,2 µH), L902 Bourns SRP7028A (2,2 µH, Isat alto) |
| 7 | 09 | SW901 sem footprint | Chave no painel, ligada por conector JST PH de 2 vias |
| 8 | 08 | RS-485 sem polarização de fail-safe | R807/R808 (680 Ω, não montados por padrão) |
| 9 | 01 | Retorno do AC/DC (ruído de comutação) compartilha o neutro com o GND metrológico | Nota de layout: HV_RTN em trilha própria até o borne |
| 10 | 08 | Detecção de VBUS do STM32 espera 5 V em PA9, mas o divisor entrega 3,3 V | Nota de firmware: desabilitar VBUS sensing do OTG e ler PA9 como GPIO |

Antes desta revisão, já tinham sido corrigidos ao trocar símbolos desenhados de memória pelos oficiais:

- BQ24074, pino 15: era ITERM, não SYSOFF; agora tem R911;
- ISO774x, TPS63021 e TPS61023 substituídos por equivalentes com símbolo oficial.

## 4. Revisão de engenharia por folha

### 01 — Entrada de energia
- OU de diodos: o instrumento segue alimentado com qualquer fase presente. ✔
- R101 (22 Ω): energia de partida em C101 ≈ V²C/2R = 0,012 A²s, abaixo do I²t de um fusível T250mA. ✔
- **Risco — tensão máxima do IRM-10-5:** em CC ele aceita até 370 V. Uma elevação de 20 % sobre 220 V (264 V) dá 373 V de pico no barramento, no limite. Ver P5.

### 02 — Tensão
- Divisor de 1/1001: 220 V dá 0,311 V de pico no pino. O limite de projeto de 0,5 V de pico permite medir até cerca de 354 Vrms. ✔
- Cinco resistores 1206 em série: cada um vê no máximo cerca de 100 V de pico na faixa de medição, abaixo dos 200 V nominais. ✔
- Filtro anti-aliasing: 999 Ω com 22 nF, corte em 7,2 kHz, igual ao dos canais de corrente. ✔

### 03 — Corrente
- Ra // Rb ≈ 1 kΩ nas três opções de sensor, então o filtro fica em 7,2 kHz em todas. ✔
- SCT-013 de 1 V: o divisor 2k49/1k65 atenua por 0,40, deixando folga de 1,8× sobre a corrente nominal. ✔
- Os contatos de J301–J304 ficam no potencial da rede, então o conector precisa ser à prova de toque. Ver P3.

### 04 — ADE9430
- PM0/PM1 em GND (modo normal) e PULL_HIGH em +3V3_ADE. ✔
- IRQ0/IRQ1 e CFx têm pull-ups na folha 06. ✔
- Ver P1 e P2.

### 05 — MCU
- O STM32F413 tem SPI1, timers, USART2/3 e USB FS nos pinos atribuídos.
- Para o USB, o HSE de 8 MHz permite gerar 48 MHz pelo PLL.
- RTC com LSE de 32,768 kHz e CR2032 direto no pino VBAT (faixa de 1,65 a 3,6 V). ✔
- Calibração: sem EEPROM externa, os coeficientes ficam na flash interna. Ver P9.

### 06 — Isolação
- ISO7762 + ISO7761 reforçados de 5 kVrms. Sem alimentação, as saídas ficam em nível alto: SS desativado e RESET solto. ✔
- TPS7A2033 (300 mA) para menos de 100 mA de carga. ✔
- Ver P4 (PS601).

### 07 — Interface HMI
- +5V protegido por PTC de 2 A; linhas de sinal com 33 Ω. ✔
- Ver P8: a placa ESP32-P4-Function-EV-Board precisa aceitar 5 V pelo conector.

### 08 — Comunicações
- ADM2587E: alimentação isolada interna (Visoout → Visoin), DE e ~RE unidos. ✔
- USB-C com CC de 5,1 kΩ: recebe só 5 V, sem negociação PD. ✔

### 09 — Alimentação
- **Orçamento de potência:**

  | Item | Potência |
  |---|---|
  | Placa HMI com LCD | ~3,5 W em +5V |
  | PS601 | ~0,4 W |
  | +3V3 (MCU, isoladores, ADM2587E) | ~0,9 W |
  | **Total em VSYS** | **~5,2 W ≈ 1,16 A a 4,5 V** |

  O BQ24074 limita a entrada em 1,48 A, então sobram só cerca de 0,3 A para carregar a bateria: uma carga completa leva mais de 8 h com a tela ligada. Em picos de consumo a bateria complementa a entrada. **Não é falha de segurança, mas limita o uso portátil.** Ver P6.
- **Dissipação do BQ24074:** cerca de (4,55 − 3,7) V × 0,89 A ≈ 0,8 W no VQFN-16. Precisa de vias térmicas no pad.
- **Margem do TPS61089:** com entrada de 4,55 V e saída de 5,04 V, a margem é pequena, mas o conversor ainda trabalha como boost. ✔
- Ver P7 (valores do TPS61089).

## 5. Pendências (decisão ou conferência necessária)

| # | Prioridade | Item |
|---|---|---|
| P1 | Alta | Confirmar no datasheet do ADE9430 a variante CP-40 e o tamanho do pad exposto. Ajustar o footprint (hoje EP 4,6 × 4,6 mm). |
| P2 | Alta | Confirmar no datasheet do ADE9430: (a) MISO em alta impedância com SS alto, que o barramento compartilhado com o MCP3204 exige; (b) ligação recomendada dos pinos NC1/NC2, hoje em GND. |
| P3 | Alta | Escolher o conector circular à prova de toque para J301–J304, com isolação para CAT III 300 V, e definir o footprint. |
| P4 | Alta | Escolher PS601: DC/DC isolado reforçado com pelo menos 4 kVAC e tensão de trabalho de pelo menos 300 VAC. Definir footprint e pinout. |
| P5 | Média | Avaliar um módulo AC/DC com faixa de entrada maior (85–305 VAC / até cerca de 430 VDC) no lugar do IRM-10-5, para suportar elevações de tensão. |
| P6 | ~~Média~~ | **Resolvida:** BQ24074 substituído pelo BQ25895 (ver seção 6). |
| P7 | Média | Confirmar no TI WEBENCH os valores de R912 (frequência), R913 (limite de corrente), R914/C912 (compensação) e C913 do TPS61089. |
| P8 | Média | Confirmar que a ESP32-P4-Function-EV-Board aceita 5 V pelo conector de expansão e mapear J701 nos pinos dela. |
| P9 | Baixa | Avaliar uma EEPROM I2C de calibração (PB6/PB7 estão livres) para não depender da flash do MCU. |
| P10 | Baixa | Rodar o ERC no KiCad e revisar os avisos restantes, como os labels globais usados numa única folha. |

## 6. Alterações após a revisão

### Carregador: BQ24074 → BQ25895 (resolve P6)

| | Antes (BQ24074) | Depois (BQ25895) |
|---|---|---|
| Tipo | linear | chaveado, 1,5 MHz, power path NVDC |
| Limite de entrada | 1,48 A | 2,0 A (R902 = 180 Ω; máximo do IRM-10-5) |
| Carga com a tela ligada | ~0,3 A (> 8 h para 2500 mAh) | ~0,85 A (~3 h) |
| Dissipação no CI | ~0,8 W | baixa (conversão chaveada) |
| Configuração | resistores | registradores via I2C (MCU PB6/PB7) |
| Telemetria | ~PGOOD, ~CHG | ~INT, STAT e ADC interno (VBUS, VBAT, VSYS, ICHG) |

Detalhes:

- **Detecção de carregador:** D+ e D− do BQ25895 unidos (CHG_DPDM), para ele se reconhecer ligado a um carregador dedicado (DCP) e não ficar limitado em 500 mA.
- **Sensor de temperatura:** divisor do NTC (R901 30,1 kΩ / R903 5,23 kΩ, a partir de REGN) para um NTC de 10 kΩ. Conferir com a tabela do datasheet.
- **Componentes externos:** L903 2,2 µH, C914 bootstrap 47 nF, REGN 4,7 µF, PMID 10 µF, SYS 2 × 10 µF, BAT 10 µF.
- **Diodos de entrada:** D901/D902 trocados para SS54 em SMC, de 5 A, para a corrente maior.
- **ERC:** PWR_FLAG em VSYS, porque o pino SYS do símbolo oficial é passivo.
- **Firmware:** na inicialização, configurar por I2C:
  - corrente de carga (sugestão: 1,0 A);
  - tensão de fim de carga (4,20 V);
  - VINDPM absoluto em cerca de 4,2 V, por causa da queda nos diodos de entrada.

  Também ler ~INT para eventos de entrada e bateria.
- **Folha 05:** PC2 passa a receber CHG_INT_N, e PB6/PB7 viram I2C1.

Alternativa não adotada: usar o boost OTG do BQ25895 para gerar o +5V e eliminar o TPS61089. Isso exige que o MCU troque de modo quando a rede cai, o que deixa a HMI sem alimentação por alguns milissegundos.
