# v0.4 validation record

Build: Unity 6000.6.5f1, macOS, 2026-10-14 local project session. Evidence describes executed checks, not a promised grade.

| Check | Result | Limits |
|---|---|---|
| Saved-scene integration | 71 checks passed, including animated ending | Exercises components, not independent human understanding |
| Physical traversal | Reached delivery/ending; 24 camera-ray interactions, 4 Q placements, 4 circuits | Inspector locomotion disabled; accelerated; known circuit solutions |
| Patrol and safety rules | Physics vision, crate occlusion, carried collider, investigation, local reset/grace checked separately | Uninterrupted native evasion difficulty still unverified |
| Premature machine use | Sole core remains recoverable when prerequisites absent | Not a guarantee against every possible interaction sequence |
| Gate exploitation | Real controller repeatedly cannot pass a closed unpowered gate | Specific anti-crush regression covered |
| Craft and helpers | Persistent visible shell/bow/tag/light, optional contextual Button dialogue, response trigger and paused gesture state checked | Visual quality assessed separately; human emotional response unverified |
| Foyer usability | Correct immediate HUD objective and near-edge plate snap verified | New player comprehension not yet observed |
| Actual native application operation | Start, dialogue, movement, keyboard look, pickup/place, first-door traversal and objective transition, low-core pickup, notebook and pause inspected | Agent controlled; no valid first-play duration or complete native run |
| Runtime visual inspection | Title, dialogue, routing board and staged ending rendered/inspected; world text depth and ending camera corrected | Staged capture is not evidence of a natural playthrough |
| macOS packaging | Local signed universal app built; ZIP integrity and extracted-app strict signature verification passed; extracted-app startup smoke check passed | Current host is Apple Silicon; Intel execution untested |

Source test tools: Assets/Editor/FactoryVerification.cs, FactoryNavigation.cs, FactoryRebuild.cs. Build script: Tools/build-mac.zsh. Local raw logs: Work/playcheck.log, playcheck.txt, navigation.log, navigation.txt, packaged-smoke.log, runtime-preview.log and agent-playtest.log. Work is deliberately excluded from version control.

## Still required

- First-time human playtests with measured active/reading time and observations; test whether the game actually meets 5–10 minutes without filler.
- Listen to music/effects on speakers and headphones; check relative levels, repetition and emotional fit.
- Observe full-speed inspector gameplay with no external instructions.
- Validate real learning transfer using player actions/explanations, especially shared power and crate occlusion.
- Windows/Linux builds and actual execution; Intel Mac execution if available.
- GitHub remote history, required collaborator, Box distribution and Blackboard submission.

No human names, feedback or successful first-time durations have been invented.

A subsequent simulated-first-contact native session completed both packaging machines and their routing boards. It stopped at the archive approach. See FIRST-CONTACT-REVIEW.md; no complete native new-player run is claimed.

## v0.4 visual and native follow-up

Standalone runtime captures inspected: visible plain shell and helper at Moulding, bow and promise tag at Engraving, actual framed childhood sketch, and ending render. These are explicitly staged previews. The shell, bow and tag are visually distinguishable, and the drawing/prompt remain legible.

Native keyboard inspection through the normal title/intro: walked to Button in the foyer, observed the greeting gesture and Talk prompt, opened the optional conversation with E, read both context-specific lines and returned to active play with the initial job unchanged. This does not extend the prior simulated-first-contact run into a full native playthrough.

The v0.4 Mac ZIP passed archive integrity and strict signature verification after extraction. Local package smoke test and native app startup passed; full inspector/finale manual play and independent human timing are still pending.

## v0.5 complete native follow-up

79 engine checks and deterministic geometry traversal passed. A subsequent native run with corrected WASD-only movement completed from title to delivery, with inspector active, one safety retry, all four circuit boards, story reveal and full departure animation. See NATIVE-FULL-REVIEW-v0.5.md for limitations and evidence. This supersedes the earlier “complete native run pending” status, but does not establish first-time human duration, emotional response or audio quality. A long interruption invalidates the native session's timing; raw evidence is retained without relabeling it as human feedback.

Final v0.5 packaging (2026-10-15): successful Unity build, local strict deep signature verification, packaged startup smoke check, ZIP integrity check and strict signature verification after extraction all passed. Universal executable includes x86_64 and arm64; execution on this host covers arm64 only. ZIP SHA-256: `5cae5aa8d0ae1d5864de841efb8f48966068ed165f6a55a948ca00fb411393a4`.

## v0.6 / actual first-use feedback and whole-game guidance

The user's report revealed comprehension and exit problems despite the earlier complete informed-developer run. Changes and observation questions are in FIRST-USE-REVIEW-v0.6.md. Fresh preferences default to English; Chinese is optional after explicit selection.

- 91 engine integration checks passed, including the animated ending. Added checks cover opening guide stages, retaining dialogue while opening/resuming the menu, recovery of stopped music, rebuilding clips on configuration change, safe archive detection before the first Chime, useful engraving-time preparation, current-action H hints, and muted automatic tests even after audio restoration.
- Complete deterministic physical route passed: 24 camera-ray interactions, four validated Q placements and four authored circuit solutions, reaching delivery. Actual controller/wall/gate/item geometry was exercised. Inspector locomotion was disabled for this isolated route; separate visibility/cover/noise/reset checks passed. Timing was accelerated and solutions known.
- Native agent-operated English opening review reached crate pickup/placement and observed the immediate doorway guide. Esc opened the menu during dialogue, Tab resumed the same line, and menu X exited with process code 0. This verifies those executed controls, not an independent player's comprehension or a complete v0.6 native run. The final rendering pass separately checks the packaged title, dialogue, routing, craft, drawing and ending. A further final native check crossed the foyer gate, picked up the sole core, powered the workshop relay, fetched the crate through the doorway, placed it on the override plate and observed the instruction to retrieve/reuse the core. Menu X exited normally. Native review found the arrow caption and floor-placement caption could touch the coach card; those screen-space bounds were corrected. CUA synthetic clicks did not activate Unity buttons; this limitation is not labeled a successful mouse-button test. Keyboard operation was used for the executed route.
- Mac packaging: Unity build, packaged startup smoke, ZIP integrity and strict deep signature verification after extraction passed. The executable contains x86_64 and arm64; this host executes arm64. Final ZIP SHA-256: `adbc414a47817de00144827f8e676f4cc6f1fdfc7a68dec2b4afc766da95312b`.
- Automated batch/preview/smoke/agent-review sessions mute their AudioSources. Normal launches retain sound. No global output, headset or system volume setting was changed. The user hearing background tests does not establish that the reported intermittent normal-start silence is fixed; that exact incident was not reproduced, and sound quality still needs listening.

One Editor run logged an exception in UnityEditor.Search startup indexing before gameplay checks began. The scene integration runner subsequently passed; this stack is in Unity's editor indexing, not a recorded game-runtime failure. Packaged runtime logs are checked separately.

No independent human first-run timing, learning result or fun rating is claimed. Windows/Linux builds, Intel execution, external publication and course submission remain pending.

Final UI-only follow-up increased measured label heights in the menu/action coach, separated the interaction strip from the footer, preserved clear space below the direction arrow, and hid the floating floor caption where it would sit behind the coach. Doorway guidance now names the destination it leads to. These copy/layout changes were compiled and packaged; visual previews include menu, active guide and blocked placement as well as the previous staged screens.

The final packaged preview pass completed with no UI_LAYOUT_OVERFLOW warnings or game-runtime exceptions in its log. Inspected the title, dialogue, routing, ending, menu and blocked-placement renders. The blocked-placement card clearly says to take one step back; its location is separate from the action keycap. These captures are staged visual checks, not a playthrough.

## v0.7 / seven-language follow-up

105 engine checks passed, including complete catalog resources, actual translated guide/story output with unchanged progression, per-language glyph coverage and English fallback. The standalone package passed startup smoke, archive integrity and extracted-app signature verification. A completed 105-screen, seven-language rendering/pagination pass and its layout fixes are described in LOCALIZATION-v0.7.md. The final native selector/restart check and last render rerun were blocked when the Mac locked; no success is claimed for those pending checks. Release ZIP SHA-256: `823a6a15355cd0323f6dbf39e93f22969c66ea3d3e8a419ff396a3b0a40f4d73`.
