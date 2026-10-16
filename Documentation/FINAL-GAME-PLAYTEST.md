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
| R1 | Dialogue showed the large calendar and map over characters. | Dialogue displays its story panel and the menu; exploration HUD is hidden. | Confirmed in the revised standalone window review. |
| R2 | Crate placement showed duplicate instructions on both sides. | One main action card, validated 3D landing outline; H opens detail. | Confirmed in the revised standalone window review. |
| R3 | Long formal-game cards blocked the factory and nearby floor route. | Formal-game card reduced to a next action; full checklist is in J. Opening teaching retains extra detail. | Final 161-screen, seven-language layout review passed without overflow. |
| R4 | The route could ask the player to circle furniture after an interaction was already available. | Reachable current E actions take priority over walking directions. | Engine regressions passed using a real visible socket and Pip at the dispatch desk; the actual E delivery reaches the ending. |
| R5 | A running crafting job could become a pointless walking objective. | Empty-key crafting tasks retain their wait/observe instruction; route and compass hide. Engraving still offers useful parallel crate preparation. | Engine regression passed for the running Moulding job. |
| R6 | Packaging ended with a technical unlock notification and little character connection. | Button comments on the deliberately crooked ribbon, ties it to the childhood drawing and asks the player to take the core into the archive. | Both new lines were read in the completed standalone session; seven-language catalog updated. |
| R7 | A safety retry erased an already successful gate-crate placement. | Crates already holding access remain in place; unrelated crates recover home. Completed outputs and drawing remain safe. | Engine regression passed with an actual placed final gate crate. |
| R8 | Inspector warning allowed little reaction time for a novice. | Detection builds over 1.6 seconds rather than 0.95; retry/area-entry grace lasts five seconds rather than 2.5. Inspector still uses sight, patrol, sound and approach states. | State/occlusion checks retained; completed native route with active inspector and zero retries. |
| R9 | Interaction depended too heavily on reading the E strip. | A depth-tested gold outline marks a reachable focused pickup/socket; story characters retain their name and E prompt without a surrounding cage. It introduces no collider or new rule. | Confirmed in the revised standalone window review. |
| R10 | During the revised native run, a nearby socketed core took priority over a directly aimed archive crate. | Direct ray-hit interactables take priority over off-centre core pickup assistance; assistance still works when no object is directly aimed at. | Actual crate-focus and E pickup regression checks passed. Five physical-route profiles rerun on the corrected source. |
| R11 | The thin drawing board still needed an exact ray hit despite showing the nearby reading objective. | Story objects receive the same visibility-checked interaction assistance as pickups, aimed at their actual collider centres. Solid walls still block reach. | Off-centre drawing-focus regression passed. |
| R12 | The finished lantern occupied too much of the delivery view. | A smaller, lower carry pose exposes more of the path; world scale restores on placement. | Final staged delivery view inspected; carrying changes only scale/pose, with world scale restored on placement. |


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

163 engine integration checks passed on the revised progression/UI logic, including animated ending, actual core insertion, wall rejection, placement safety, reachable-task guidance and crate-preserving retries. Final visual refinements are compiled by the release build. The first multilingual render found clipping in one English action, Spanish/German chapter titles and a French instruction. Their bounds/copy were corrected before a second final render. The five profile runs were repeated after the final interaction corrections; all reached delivery. Native completion and its later focused corrections are distinguished below.

Independent first-time completion duration, fun, physical mouse feel, motion comfort, audio mix listening and native-language editorial accuracy still require actual player feedback. A 5-10-minute/full-score rubric result is not established by development checks alone.

### Five completed scenario runs

All five deterministic profile runs completed successfully: newcomer (3 added checks), wayfinding (5), story (3), fast (3), explorer (3). Each run performed 20 interactions, four placements and four authored circuit solutions. Final logs: `Work/scenario-<profile>-v012-final.log`; reports: `Work/scenario-<profile>.txt`. Five successful scripted traversals are not five independent player playtests.

### Completed standalone review

`Work/full-native-v012.log` and `Work/Release-v0.12/native-completed.json` record one continuous revised standalone session from the normal title through all four circuit boards, Chime practice, drawing, final crate, sole-core reuse, physical lantern pickup and delivery. The inspector remained enabled. Safety retries: 0. Completion: true. The reviewer watched the complete departure animation into the village and quit normally (exit 0). No teleports, progress setters or route autoplay were used in this window session. It used English and the arrow-camera accessibility mode; tool-controlled pointer clicks were unreliable, so physical mouse click/hover feel is not declared verified.

That native run discovered R10-R12 and preceded their last focused corrections. Those corrections received new engine checks, a fresh five-profile physical-route rerun and a fresh package/layout review; they are not described as a second complete native session. Earlier v0.11 baseline remained incomplete and is retained above. Neither session establishes independent human comprehension or duration.

### Final visual correction

The final delivery render exposed an oversized box around Pip when viewed closely. The highlight now applies to pickups and sockets, while story characters use their existing named E prompt. This is a presentation-only change; target selection, reach and progression are unchanged.

### Release package checks

The corrected v0.12 Mac package passed archive integrity, installed/extracted application signature verification and both installed/extracted startup checks (scene references, title action, dialogue and notebook). Seven-language staging rendered 161 screens and advanced seven pagination stress cases without layout-overflow or standalone runtime-exception entries. Selected captures are in the portable evidence directory; they are staged visual checks, not player screenshots. Some rapid staged late-game captures retain a temporary crate-override toast from fixture setup; this was not observed during the native delivery.

Release archive: `Builds/MidnightShift-Mac-v0.12.zip`. SHA-256: `333cb1b5bb1529b33667eac6777347e0f3597416855d310a158be9062db35a1f`.

### Handoff preference check

The final native opening check was interrupted by the Mac lock screen. The earlier complete review used the saved arrow-only accessibility option; restoring normal mouse-look mode and confirming English on the final opening remain pending until the Mac is unlocked. No locked-screen check is reported as passed.

## Portable evidence

Selected anonymous records and check results are preserved in [PlaytestEvidence/v0.12](PlaytestEvidence/v0.12/README.md). They remain distinguishable from participant feedback and are available with the source repository, without relying on ignored local Work logs.
