; Unshipped analyzer release

### New Rules

Rule ID | Category | Severity | Notes
--------|----------|----------|-------
NKMOVE001 | CodeGeneration | Error | Monster classes must inherit their generated base class.
NKMOVE002 | CodeGeneration | Error | Generated move state machines require an InitialMove method.
NKMOVE004 | CodeGeneration | Error | A single initial move cannot specify a custom weight.
NKMOVE005 | CodeGeneration | Error | Generated move state machines require at least one valid Move or InitialMove method.
NKMOVE006 | CodeGeneration | Error | Move and InitialMove methods require boolean or enum parameters followed by a final Creature target list.
