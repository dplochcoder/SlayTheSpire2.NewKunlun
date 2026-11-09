; Unshipped analyzer release

### New Rules

Rule ID | Category | Severity | Notes
--------|----------|----------|-------
NKMOVE001 | CodeGeneration | Error | Monster classes must inherit their generated base class.
NKMOVE002 | CodeGeneration | Error | Move methods require a matching intent property.
NKMOVE003 | CodeGeneration | Error | Move intent properties must return an intent sequence.
NKMOVE004 | CodeGeneration | Error | Generated move state machines require an initial move.
NKMOVE005 | CodeGeneration | Error | InitialState can only annotate move methods.
NKMOVE006 | CodeGeneration | Error | A single initial move cannot specify a custom weight.
