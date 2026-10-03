# Layout da placa de metrologia — rev. 0.1 (posicionamento inicial)

![Posicionamento inicial](img/layout_posicionamento_rev01.png)

*Vermelho escuro: nets de rede. Vermelho claro: domínio metrológico (potencial da rede). Azul: domínio seguro. Verde: RS-485 isolada. Amarelo: faixas da barreira (sem cobre).*

## Estado

`hardware/Smart-Metering/Smart-Metering.kicad_pcb` contém:

- contorno de **130 × 130 mm**, 4 camadas (F.Cu, In1.Cu = GND, In2.Cu = PWR, B.Cu);
- **4 furos M3** a 10 mm das bordas (espaçamento de 110 × 110 mm);
- **223 componentes** com footprints oficiais do KiCad, nets nos pads e vínculo com o esquemático (`path`), de modo que *Tools → Update PCB from Schematic* reconhece todos;
- **posicionamento inicial** por domínio: cada componente fica perto do CI ao qual se liga;
- **faixas de barreira** como áreas proibidas para cobre: 7 mm entre metrologia e lado seguro (os pads dos isoladores SOIC-16W ficam a 7,3 mm entre fileiras) e 7/4 mm em volta da ilha RS-485;
- **planos de terra por domínio** em In1.Cu e B.Cu (GND, GND_SYS, GND_485), ainda sem preenchimento (pressionar **B** no KiCad);
- **sem roteamento**.

Componentes que atravessam a barreira:

| Ref | Função | Lado da rede / isolado | Lado seguro |
|---|---|---|---|
| U601, U602 | isoladores ISO776x | lado 2 / lado 1 | lado 1 / lado 2 |
| U606 | ADuM6000 (alimentação isolada) | pinos 9–16 | pinos 1–8 |
| PS101 | AC/DC RAC20 | entrada (pinos 1, 2) | saída (pinos 4, 5) |
| U801 | ADM2587E (RS-485) | — | lógica (pinos 1–10); barramento (11–20) na ilha |

## Classes de net e regras

Classes definidas em `Smart-Metering.kicad_pro`:

| Classe | Conteúdo | Trilha |
|---|---|---|
| MAINS | fases, nós dos fusíveis, barramento HV, entrada auxiliar | 0,5 mm |
| DIVIDER | nós internos das cadeias de resistores dos divisores | 0,3 mm |
| HOT | domínio metrológico (GND = neutro) | 0,2 mm |
| ISO485 | barramento RS-485 isolado | 0,25 mm |
| POWER | alimentação do lado seguro (VSYS, +5V, +3V3, comutação dos conversores) | 0,6 mm |
| Default | sinais do lado seguro | 0,2 mm |

Regras em `Smart-Metering.kicad_dru` (valores iniciais, **conferir na IEC 61010-1, CAT III 300 V, grau de poluição 2**):

| Regra | Clearance | Creepage |
|---|---|---|
| Barreira reforçada: HOT/MAINS/DIVIDER ↔ lado seguro/ISO485 | 6,0 mm | 6,4 mm |
| Rede entre fases e neutro: MAINS ↔ MAINS/HOT | 3,0 mm | 4,0 mm |
| RS-485 isolada ↔ lado seguro | 3,0 mm | — |

## Antes do roteamento (ajustes manuais)

1. **Placa e gabinete:** ajustar o FeatureScript `cad/SmartMeter_PCBSupports.fs` (metrologia): largura 130 mm, altura 130 mm, furos 110 × 110 mm.
2. **Conectores de borda:** conferir o lado de entrada dos fios dos bornes (J201, J101, J801) e a abertura do USB-C (J802), que devem ficar voltados para fora da placa.
3. **Cadeias dos divisores (R211–R237):** alinhar em linha reta, uma cadeia por fase, com pelo menos 3 mm entre cadeias de fases diferentes.
4. **ADE9430 (U401):**
   - desacoplamentos C401–C408 encostados nos pinos;
   - cristal Y401 a menos de 5 mm;
   - filtros anti-aliasing (C2x1/C2x2, C3x1/C3x2) perto dos pinos de entrada;
   - EP com vias para o GND.
5. **Conversores chaveados** (U901, U902, U903, U606): laço de comutação curto, conforme o layout recomendado de cada datasheet. Vias térmicas nos pads expostos.
6. **ADuM6000 (isoPower):** seguir as regras de EMI do datasheet (capacitância de costura entre GND e GND_SYS através da barreira, se a norma permitir).
7. **Retorno do AC/DC (HV_RTN):** trilha própria até o borne do neutro, sem passar pelo plano GND metrológico.

## Ordem sugerida de roteamento

1. Rede e fonte: entradas, fusíveis, varistores e PS101, com trilhas largas e distâncias de rede.
2. Núcleo metrológico: ADE9430, divisores, entradas de corrente e filtros.
3. Barreira: isoladores, U606 e LDO U605.
4. Lado seguro: MCU, alimentação (BQ25895, TPS63001, TPS61022), USB, HMI, RS-485.
5. Preencher zonas (B), rodar DRC (inclui creepage) e corrigir.
