# Structural Secondary Quantity Ratios

Default ratios used by the construction-takeoff skill when no bar schedule or specific quantities are shown on the drawings. These are starting points. Override with your firm's actual data.

Replace these defaults by adding your own production rates and ratios to your business context (Notion / Google Drive / CONTEXT.md).

---

## Foundations

### Pad footing (per footing)

| Secondary quantity | Default calculation | Notes |
|-------------------|---------------------|-------|
| Excavation (m3) | L x W x (D + 0.1m) | 100mm working space added to depth |
| Concrete (m3) | L x W x D | From footing schedule dimensions |
| Formwork (m2) | (L x 2 + W x 2) x D | Four sides only, no base |
| Reinforcement (kg) | Concrete volume x 100 kg/m3 | Use bar schedule if shown |

### Strip footing (per LM)

| Secondary quantity | Default calculation |
|-------------------|---------------------|
| Excavation (m3) | W x (D + 0.1m) x 1m |
| Concrete (m3) | W x D x 1m |
| Formwork (m2) | D x 2 x 1m (both sides) |
| Reinforcement (kg) | Concrete volume x 80 kg/m3 |

### Bored pier (per pier)

| Secondary quantity | Default calculation |
|-------------------|---------------------|
| Excavation (m3) | PI x (dia/2)^2 x depth |
| Concrete (m3) | PI x (dia/2)^2 x depth |
| Reinforcement (kg) | Concrete volume x 120 kg/m3 |
| Casing (LM) | Depth (if temporary casing used) |

### Raft slab (per m2)

| Secondary quantity | Default calculation |
|-------------------|---------------------|
| Excavation (m3) | Area x (thickness + 0.1m) |
| Concrete (m3) | Area x thickness |
| Formwork edge (LM) | Perimeter |
| Mesh (m2) | Area x 1.1 (10% laps) |
| Reinforcement (kg) | Concrete volume x 100 kg/m3 |

---

## Columns

### Reinforced concrete column (per column)

| Secondary quantity | Default calculation |
|-------------------|---------------------|
| Concrete (m3) | Section area x height |
| Formwork (m2) | Perimeter x height |
| Reinforcement (kg) | Concrete volume x 200 kg/m3 |

### Structural steel column (per column)

| Secondary quantity | Default calculation |
|-------------------|---------------------|
| Weight (kg) | Length x kg/m from section tables |
| Base plate (EA) | 1 per column |
| Holding-down bolts (EA) | 4 per column (standard) |

---

## Beams and slabs

### Reinforced concrete beam (per LM)

| Secondary quantity | Default calculation |
|-------------------|---------------------|
| Concrete (m3) | Width x depth x 1m |
| Soffit formwork (m2) | Width x 1m |
| Side formwork (m2) | Depth x 2 x 1m |
| Reinforcement (kg) | Concrete volume x 180 kg/m3 |

### Suspended slab (per m2)

| Secondary quantity | Default calculation |
|-------------------|---------------------|
| Concrete (m3) | Area x thickness |
| Formwork underside (m2) | Area |
| Edge formwork (LM) | Perimeter |
| Mesh (m2) | Area x 1.1 |
| Reinforcement (kg) | Concrete volume x 100 kg/m3 (standard slab) |
| Reinforcement (kg) | Concrete volume x 120 kg/m3 (transfer slab) |

### Structural steel beam (per beam)

| Secondary quantity | Default calculation |
|-------------------|---------------------|
| Weight (kg) | Span x kg/m from section tables |
| End plates (EA) | 2 per beam |

---

## Common steel section weights (kg/m)

| Section | kg/m |
|---------|------|
| 150UB18 | 18.0 |
| 180UB18 | 18.1 |
| 200UB18 | 18.2 |
| 250UB26 | 25.7 |
| 310UB32 | 32.0 |
| 310UB40 | 40.4 |
| 310UB46 | 46.2 |
| 360UB45 | 44.7 |
| 360UB57 | 56.7 |
| 410UB54 | 53.7 |
| 250UC73 | 72.9 |
| 310UC97 | 97.0 |
| 150x150x5 RHS | 21.1 |
| 200x100x5 RHS | 22.3 |

---

## Notes

These ratios are defaults for preliminary estimating. Before using in a bid:

1. Check whether the drawings include a bar schedule. If they do, use the schedule quantities, not these ratios.
2. Verify concrete grades match the specification (higher grade concrete may have different reinforcement requirements).
3. For transfer slabs, post-tensioned slabs, or anything with a non-standard structural system, these defaults will not be accurate. Use a structural engineer's assessment.

To use your own ratios instead of these defaults, add them to your business context under "Secondary quantity rules" and the construction-takeoff skill will use those in preference to this reference file.
