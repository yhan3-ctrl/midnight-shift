# Final game review and playtesting record

Project: Midnight Shift: The Last Lantern. Review date: October 15, 2026 (America/Chicago). Baseline: v0.11. Revised build: v0.12.

## Method and scope

This is a developer usability review informed by direct player feedback, not five recruited participants. The reviewer knows the game and its solutions. Five scenario lenses represent different experience levels and priorities; they are inspection questions, not observations attributed to fictional people. Raw local session data and deterministic checks are kept separate. Tool delays, pauses, solution knowledge and accelerated test time make these sessions unsuitable for measuring first-play duration.

## Baseline window playthrough

A continuous standalone v0.11 session began at the title, selected English, read the introduction, placed the foyer crate, crossed the first gate, picked up the sole core, powered the workshop relay, brought back its crate, reused the core, solved Moulding and Ribbon, sounded the Chime, placed the archive crate, read all four drawing-reveal lines, recovered the core, solved Engraving and entered the final gate task. The inspector remained active. Keyboard camera input was selected during the archive to make tool operation repeatable. No teleport, progress flag setter or route autoplay was used in this window session.

The session ended before lantern assembly/delivery, with four recorded safety retries and completed=false. It is deliberately not reported as a complete playthrough. The final-area inspection exposed retry friction, although observation/tool delays contributed to exposure to the inspector and cannot be treated as ordinary player difficulty evidence.

Raw evidence: `Work/full-native-v011-review.log` and `Work/Release-v0.12/baseline-window-session.json`.

## Findings, revisions and retest evidence

| ID | Observed problem | Revision | Verification |
|---|---|---|---|
| R1 | Dialogue showed the large calendar and map over characters. | Dialogue displays its story panel and the menu; exploration HUD is hidden. | Final standalone screen review pending. |
| R2 | Crate placement showed duplicate instructions on both sides. | One main action card, validated 3D landing outline; H opens detail. | Final standalone screen review pending. |
| R3 | Long formal-game cards blocked the factory and nearby floor route. | Formal-game card reduced to a next action; full checklist is in J. Opening teaching retains extra detail. | Seven-language layout review pending. |
| R4 | The route could ask the player to circle furniture after an interaction was already available. | Reachable current E actions take priority over walking directions. | Engine regression passed using a real visible socket and current task. |
| R5 | A running crafting job could become a pointless walking objective. | Empty-key crafting tasks retain their wait/observe instruction; route and compass hide. Engraving still offers useful parallel crate preparation. | Engine regression passed for the running Moulding job. |
| R6 | Packaging ended with a technical unlock notification and little character connection. | Button comments on the deliberately crooked ribbon, ties it to the childhood drawing and asks the player to take the core into the archive. | Seven-language catalog updated; final story flow review pending. |
| R7 | A safety retry erased an already successful gate-crate placement. | Crates already holding access remain in place; unrelated crates recover home. Completed outputs and drawing remain safe. | Engine regression passed with an actual placed final gate crate. |
| R8 | Inspector warning allowed little reaction time for a novice. | Detection builds over 1.6 seconds rather than 0.95; retry/area-entry grace lasts five seconds rather than 2.5. Inspector still uses sight, patrol, sound and approach states. | Inspector state/occlusion checks retained; final native route pending. |
| R9 | Interaction depended too heavily on reading the E strip. | A depth-tested gold outline marks a reachable focused object/socket; it introduces no collider or new rule. | Final standalone screen review pending. |

## Five scenario lenses

Each deterministic run starts from the real scene, uses CharacterController movement and physical collisions, performs all 20 route interactions, four validated Q placements, solves the four authored circuit boards and reaches the actual delivery action. Its inspector locomotion is disabled to isolate geometry; inspector behavior is checked separately. These runs do not simulate a person's feelings, discover solutions unaided or establish enjoyment.

| Lens | Inspection question | Added scenario behavior | Result |
|---|---|---|---|
| First-person newcomer | Can the player access safe help and recover a disorienting view? | Open/close map before movement; look and level-view recovery retaining heading. | Passed: actual delivery reached; scenario-specific checks passed. |
| Navigation-dependent | Do map and notebook preserve the real goal instead of changing progress? | Map-to-notebook exploration at the opening and after the drawing. | Passed: actual delivery reached; scenario-specific checks passed. |
| Story-focused | Can story be reread and interrupted without loss? | Reread drawing; pause/resume its dialogue. | Passed: actual delivery reached; scenario-specific checks passed. |
| Fast, experienced | Can rapid confirmation bypass a required puzzle? | Attempt Start while Moulding/Ribbon boards are unsolved. | Passed: actual delivery reached; scenario-specific checks passed. |
| Exploratory, out-of-order | Can unusual actions strand the only core or break pause safety? | Try the final machine before its prerequisite; interrupt for desktop use. | Passed: actual delivery reached; scenario-specific checks passed. |

## Acceptance status

159 engine integration checks passed on the revised progression/UI logic, including animated ending, actual core insertion, wall rejection, placement safety, reachable-task guidance and crate-preserving retries. Final visual refinements are compiled by the release build. The first multilingual render found clipping in one English action, Spanish/German chapter titles and a French instruction. Their bounds/copy were corrected before a second final render. Five scenario runs and the revised standalone native completion are recorded below when executed; pending entries are not a claim of completion.

Independent first-time completion duration, fun, physical mouse feel, motion comfort, audio mix listening and native-language editorial accuracy still require actual player feedback. A 5-10-minute/full-score rubric result is not established by development checks alone.

### Five completed scenario runs

All five deterministic profile runs completed successfully: newcomer (3 added checks), wayfinding (5), story (3), fast (3), explorer (3). Each run performed 20 interactions, four placements and four authored circuit solutions. Logs: `Work/scenario-<profile>-v012.log`; reports: `Work/scenario-<profile>.txt`. Five successful scripted traversals are not five independent player playtests.
