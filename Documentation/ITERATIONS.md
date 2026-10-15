# Development and iteration record

## v0.1 / first playable

Implemented the foyer, shared-core packaging, archive inspector, drawing reveal, assembly and physical delivery. Added dialogue pause, settings, safe placement, reversible item handling, procedural ghost animation and an original three-arrangement score.

During visual inspection of actual Unity-rendered frames:
- Observed mirrored text on world signs. Corrected sign orientation.
- Observed labels visible through walls because the default font material ignored world depth. Added a dedicated depth-tested world-text shader.
- Added the missing ceiling to make the factory read as an interior.

During implementation review:
- Corrected initial cursor lock so the title's Begin button can be clicked.
- Corrected the verification tool's callback across Unity's domain reload so its play-mode checks actually execute.

These are developer findings, not player feedback. Future playtest-driven changes must identify the observed problem, modification and retest separately. No claim of multiple human-feedback iterations is made.
