; Unshipped analyzer release

### New Rules

Rule ID | Category | Severity | Notes
--------|----------|----------|-------
NKLOC001 | Localization | Error | Card localization arguments must be string literals.
NKLOC002 | Localization | Error | Card description variables must be declared by the card.
NKLOC003 | Localization | Error | Localization attribute arguments must use named-argument syntax.
NKLOC004 | Localization | Error | Localized models must declare the localization attribute matching their model hierarchy.
NKLOC005 | Localization | Error | Keyword localization attributes may only be applied to static CardKeyword fields.
NKLOC006 | Localization | Error | Static hover-tip localization attributes may only be applied to static StaticHoverTip fields declared with CustomEnum.
