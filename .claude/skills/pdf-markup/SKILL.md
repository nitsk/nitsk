---
name: pdf-markup
description: Perform bulk markup operations on PDF drawing sets without needing Bluebeam Max. Searches markups by criteria and changes them in bulk, adds standard stamps to multiple sheets, updates comment text consistently, applies stamps like "Not for Construction" or "Revised" across a drawing set, or extracts all markups into a register. Trigger on phrases like "mark up these drawings", "bulk markup change", "add a stamp to all sheets", "use my pdf markup skill", or any request to apply or modify markups on PDF drawings programmatically.
---

# PDF Markup Skill

Performs bulk markup operations on PDF drawings without requiring a Bluebeam Max subscription.

Does the same bulk markup work the Bluebeam Claude integration does, using standard Claude capabilities. Any task you would use the Bluebeam integration for (mass stamps, text updates, colour changes, markup extraction) can be done with this skill.

---

## Why this exists

The Bluebeam Max Claude integration is genuinely useful for bulk markup operations. It is also the main thing that integration does well. Bluebeam Max requires a subscription upgrade. This skill replicates the same bulk markup functionality on standard Claude with no Bluebeam Max needed.

---

## What this skill does

1. Apply a stamp or text markup to every sheet in a drawing set
2. Search markups across the drawing set by criteria (text content, colour, author)
3. Bulk edit markup properties (change colour, update text, replace strings)
4. Extract all markups from a drawing set into a markup register
5. Apply revision clouds or callouts to specified regions on multiple sheets

---

## Context sources

**Input:**
- A PDF drawing set in `inputs/` or specified location
- A markup specification (what to add or change)

**Output:**
- Modified PDF saved to `outputs/[project]-marked-up.pdf` (original is never modified)
- A markup register CSV listing all changes made

---

## Workflow

### Phase 1: Confirm the markup operation

Ask the user to specify exactly what operation to run:

```
PDF MARKUP OPERATION

Source PDF:    [filename and path]
Target sheets: [All sheets / Specific sheets / Sheets matching a filter]

Operation type:
  1. Apply stamp to all sheets
  2. Add text markup to specified location on all sheets
  3. Search and replace text within existing markups
  4. Change markup colour by criteria
  5. Extract all markups to a register
  6. Apply revision clouds to specified regions

Operation details: [Specifics depending on type]

Output file: outputs/[project]-marked-up.pdf

Confirm before I proceed.
```

### Phase 2: Apply the operation

Use Python libraries (PyMuPDF, pdfplumber, reportlab) to perform the markup operation programmatically.

For each operation type:

**Stamp application:**
Add a stamp markup to a specified position (e.g. top right corner) on every sheet.
Standard stamps: "Not for Construction", "Approved", "Revised", "Superseded", "For Information".
Customisable: text content, colour, font size, position, opacity.

**Bulk text markup:**
Add the same text annotation to every sheet at a specified position.
Useful for adding project name, revision number, or notes consistently across a set.

**Search and replace within markups:**
Find every text markup matching a string and replace with new text.
Useful for updating project numbers, revision references, or correcting typos across the set.

**Colour change by criteria:**
Find every markup matching criteria (author, current colour, text content) and change its colour.
Useful for re-categorising markups (e.g. all "QA" markups change from red to orange).

**Markup extraction:**
Read every markup in the PDF and export to a register CSV:
| Sheet | Page | Markup type | Text content | Colour | Author | Date | Position |

**Revision clouds:**
Apply revision cloud markups around specified regions on specified sheets.
Region can be defined by coordinates, by element tag, or by reference to the Drawing Analyzer output.

### Phase 3: Save the output

- Original PDF is never modified
- Marked-up PDF saved to `outputs/[project]-marked-up.pdf`
- Markup register saved to `outputs/[project]-markups-register.csv`

### Phase 4: Completion report

```
PDF MARKUP COMPLETE

Operation:         [Type of operation performed]
Sheets affected:   [N] of [N] total sheets
Markups added:     [N]
Markups modified:  [N]
Markups extracted: [N]

Files saved:
  outputs/[project]-marked-up.pdf
  outputs/[project]-markups-register.csv

Original file unchanged: inputs/[project].pdf
```

---

## Comparison with Bluebeam Max Claude integration

| Task | Bluebeam Max Claude | This skill |
|------|---------------------|-----------|
| Apply stamp to all sheets | Yes | Yes |
| Bulk text edits | Yes | Yes |
| Search markups by criteria | Yes | Yes |
| Change markup colours in bulk | Yes | Yes |
| Extract markup register | Yes | Yes |
| Apply revision clouds | Limited | Yes |
| Read text layer of drawings | Yes | Yes (combined with Drawing Analyzer) |
| Measure areas or lengths | No | No (use Construction Takeoff skill) |
| Read line types and visual symbols | No | Yes (combined with Drawing Analyzer) |
| Cost | Bluebeam Max subscription | Standard Claude only |

---

## Rules

- Never modify the original PDF file. Always save to outputs/
- Always confirm the operation details before applying changes
- For destructive operations (deleting or replacing existing markups), require explicit user confirmation
- Output a markup register CSV for every operation so changes are auditable
- For stamp positions and colours, use the firm's standard if defined in business context, otherwise ask the user

---

## Output format

Modified PDF saved to outputs/ folder.
Markup register CSV saved to outputs/ folder.
Completion report in chat.
Tone: operational, factual, no narrative padding.
