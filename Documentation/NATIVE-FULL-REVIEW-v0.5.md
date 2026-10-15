# v0.5 complete native review — 2026-10-15

## Executed

The developer agent completed one continuous game session from the normal title screen through the normal delivery action and watched the 15-second departure animation. Used native keyboard input, no teleports, progress setters, disabled inspector or authored autoplayer in this session. The inspector remained active. Paused between some observations and used the visible agent-only position/look overlay. Prior code and route knowledge means this is an informed developer playthrough, not a blind human test.

Confirmed in the standalone signed Mac application:
- Foyer crate, first gate, sole core pickup, powered workshop gate, override crate and core recovery.
- Moulding and Ribbon puzzles, visible shell/bow and completion dialogue.
- Chime, archive crate, shelf side passage, drawing gate, all four story-reveal lines and core recovery.
- Engraving puzzle with active inspector, one final-area safety retry, retained engraving/drawing and recoverable core.
- Assembly gate, final two-output puzzle, physical finished lantern pickup, delivery to Pip.
- Ending door opens and three helpers leave together with the lantern.

## Problems found and fixed

1. **Arrow look also moved the player.** Unity legacy Horizontal/Vertical axes included arrow keys. This caused substantial unintended motion during keyboard looking. Movement now reads WASD explicitly. Native before/after inspection confirmed a down-arrow look changes pitch while position stays at (-4.85, 2.29). The later archive plate placement succeeded after turning in place.
2. **Crate placement on shelves / over player.** Restrict Q placement to floor-level surfaces and reject player-overlapping plate snaps; retain obstacle checks and valid plate assist.
3. **Shelf penetration and retry confusion.** Solid shelf collision; retry restores pitch and shows a frozen, acknowledged explanation of progress/crate/core recovery.
4. **Premature READY captions and floating growth.** Captions reflect actual machine state; growing shell stays anchored to tray.
5. **Long-session time display wrapped every hour.** Display total minutes rather than the minute component of a TimeSpan. This display-only correction followed the complete playthrough and received a fresh build/startup check; it does not change progression.

## Evidence and limits

- 79 scene/engine integration checks passed, including placement regressions and animated ending, before the explicit-WASD input correction. Native complete playthrough validated the corrected input afterward.
- Deterministic physical route passed: 24 raycast interactions, four validated Q placements and four circuits. Inspector locomotion disabled only in that separate geometry test.
- Native session JSON: `shift-20261015-055357-155.json`; marked automated, completed=true, safetyRetries=1, all progress masks reached through 63. Evidence copy: Work/native-v05-completed.json.
- Native runtime log: Work/full-native-v05-fixed.log; no Exception/Error/NullReference matches.
- **Duration is invalid for grading.** The session contains a very long interruption around Engraving, tool observation delays and pauses. Raw logged active time is 33,783.91 seconds; the old ending incorrectly displayed its minute component as 23:03. Neither value is a first-time human completion duration. No 5–10-minute claim follows from this run.
- Music was not evaluated by ear. No claim about speaker/headphone mix quality.
- Still require independent first-time human feedback/timing, learning-transfer evidence, Windows/Linux builds and execution, and submission deliverables.
