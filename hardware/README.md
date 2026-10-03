# Hardware

## Projeto KiCad

O projeto principal está em:

```text
hardware/Smart-Metering/
```

Arquivos principais:

- `Smart-Metering.kicad_pro`
- `Smart-Metering.kicad_sch`
- `Smart-Metering.kicad_pcb`

## Organização planejada do esquemático

O esquemático será hierárquico.

Folhas previstas:

1. `Power_Input`
2. `Voltage_Sensing`
3. `Current_Sensing`
4. `ADE9430`
5. `Metrology_MCU`
6. `Isolation`
7. `HMI_Interface`
8. `Communications`
9. `Power_Supplies`

A primeira implementação deverá começar por `Power_Supplies`, `Voltage_Sensing`, `Current_Sensing` e `ADE9430`.

## Regras

- manter símbolos e footprints personalizados dentro do repositório;
- não usar caminhos absolutos para bibliotecas próprias;
- registrar no Git os arquivos `.kicad_pro`, `.kicad_sch` e `.kicad_pcb`;
- não registrar backups e arquivos locais do KiCad;
- documentar alterações importantes de hardware antes de iniciar layout.
