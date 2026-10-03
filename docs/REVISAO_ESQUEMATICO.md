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

## 5. Pendências — situação

| # | Item | Situação |
|---|---|---|
| P1 | Pad exposto do ADE9430 | **Resolvida por projeto:** footprint com EP de 4,15 mm (Texas RHA0040B VQFN-40 6×6), compatível com as variantes CP-40 de 6×6 mm da ADI (EP de 3,9 a 4,7 mm). Ainda vale confirmar a variante no datasheet antes de pedir a PCB. |
| P2 | MISO do ADE9430 e NC1/NC2 | **Resolvida:** com SS alto, o ADE9430 para de acionar o MISO e liga um pull-up fraco de 100 kΩ, então compartilhar com o MCP3204 é seguro. O datasheet recomenda NC1/NC2 em GND, como já está. |
| P3 | Conector dos sensores de corrente | **Resolvida:** J301–J304 são headers internos JST PH de 4 vias, cabeados até conectores M8 fêmea de 4 vias no painel (IEC 61076-2-104, contatos à prova de toque). |
| P4 | DC/DC isolado do domínio metrológico | **Resolvida:** U606 ADuM6000 (isoPower, 5 kVrms, até 500 mW; símbolo oficial). Os módulos 5 V → 5 V da biblioteca têm só 1–3 kVDC. Confirmar no datasheet a certificação reforçada e a tensão de trabalho. |
| P5 | Faixa de entrada do AC/DC | **Resolvida:** PS101 = RECOM RAC20-05SK/277 (85–305 VAC / 120–430 VDC, 20 W). 264 VAC (+20 %) dá 373 Vpk, dentro do limite. Fusíveis T500mA, R101 de 3 W e C101 de 10 µF/450 V. Confirmar com a RECOM que a /277 usa o mesmo encapsulamento. |
| P6 | Carregador | **Resolvida** (BQ25895, seção 6). Com o RAC20, o limite de entrada subiu para 3 A (R902 = 120 Ω). |
| P7 | Valores do TPS61089 | **Resolvida:** substituído pelo TPS61022 (chave de 8 A, compensação interna). Só usa divisor de realimentação (732k/100k → 5,0 V), indutor de 1 µH e capacitores. |
| P8 | Alimentar a placa EV pela interface | **Resolvida:** o J1 da ESP32-P4-Function-EV-Board tem 5 V nos pinos 2 e 4, previstos para embarcar a placa num sistema maior. Alimentar por eles com o USB-C da placa EV desconectado. O mapeamento J701 → J1 segue o esquema da placa EV. |
| P9 | EEPROM de calibração | **Resolvida:** U502 24LC64 (0x50) no I2C1, junto com o BQ25895 (0x6A). |
| P10 | ERC e labels globais | **Parcial:** labels globais usados numa só folha viraram locais (0 restantes). O ERC do KiCad ainda precisa ser rodado. |

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

### Resolução das pendências P1–P10

- 01: PS101 → RAC20-05SK/277; F101–F104 T500mA; R101 22 Ω 3 W; C101 10 µF/450 V.
- 03: J301–J304 com footprint JST PH 4 vias (cabo até o M8 do painel).
- 04: footprint do ADE9430 com EP de 4,15 mm; AVDDOUT, DVDDOUT, REF_ADE, XIN_ADE e XOUT_ADE como labels locais.
- 05: U502 24LC64 + C515.
- 06: PS601 → U606 ADuM6000 (RC_SEL em VDD1, RC_IN em GND1, V_SEL em V_ISO) com desacoplamento.
- 09: U903 TPS61089 → TPS61022 (L902 de 1 µH, 3 × 22 µF na saída); R902 = 120 Ω (entrada de 3 A).
- Projeto: 223 componentes; todas as verificações automáticas sem erro; todos os footprints existem na biblioteca oficial.
