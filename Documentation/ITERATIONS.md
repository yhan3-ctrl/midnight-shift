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


## v0.2 / physical checks and deeper dependencies

Developer observations and changes (not human playtest feedback):
- A physics ray hit the player’s own controller instead of a low object. Moved the player to the Ignore Raycast layer and excluded that layer from interaction queries. Retested via actual camera-ray interactions.
- The original final pressure plate overlapped its nearby machine’s crate-placement space. Moved it into a clear area. Validated using the same Q placement method used by the player.
- A safety notice occupied a narrow archive aisle. Raised it to clear the player’s capsule, and moved the drawing off the central exit route.
- Crates were too short to obstruct the inspector’s actual sight ray. Increased their height and checked visible / occluded / carried cases using physics. Scaled their held appearance down to preserve view comfort, restoring world dimensions on placement.
- The completed Chime returned to idle, causing its E interaction to start again when a player wanted the core back. Added a completed state so the same retrieval rule applies to every finished machine.
- The inspector tried to investigate the solid centre of a machine and could remain stuck there. Changed investigation destinations to a walkable point in front of it.
- Added a continuous-power gate whose crate begins on the other side. The solution requires transferring access from electricity to the crate before reusing the core, making resource planning necessary.
- Required a practical Chime interaction before accessing the drawing. Brought the inspector into Final Assembly so machine sounds receive a later use.
- Changed safety retries to the current region and recovered a held core locally, reducing repetition after a mistake.
- Added a pausable notebook, softer wooden footsteps, warm menu buttons, local anonymous session records and a staged village ending with moving helpers.

Automated navigation isolates geometry by disabling inspector movement, then uses CharacterController.Move, real camera-ray interactions and Q placement. Inspector behaviour has separate physics and state checks. Neither test is recorded as a human playtest or a valid target-duration measurement.


## Adversarial rule review during v0.2

- Found that an overly broad anti-crush rule could lift even a closed gate when the player approached it, bypassing the pressure-plate puzzles. Changed the rule to hold an already-open panel only while the player occupies its actual doorway. Added a physical test that repeatedly moves the real controller toward an unpowered gate and requires it to remain closed.
- Added a short detection grace period after a safety retry and after the inspector changes duty, preventing immediate repeat captures.
- Added the current core location to the notebook so a player can recover from forgetting a socket without searching every room.
- Added automatic pause on loss of window focus and ensured keyboard start does not also skip the first dialogue line in the same frame.

These improvements come from implementation review and automated/visual inspection. Human feedback remains unrecorded until a real participant tests the build.


## v0.3 / depth and sequence review

- Expert review identified a duration/content risk: the original jobs mostly required carrying a core and waiting for a result. Added four authored power-routing boards, progressing from a single path to split outputs and multiple required contacts. No processing time was increased to make the game longer.
- Added visible power propagation, contact/outlet counters, persistent rotations and both pointer and keyboard operation. The simulation pauses during planning; that time counts as active gameplay.
- An out-of-order interaction review found that inserting the sole core into a machine before its prerequisite could strand it there. Machines now reject premature insertion without taking the core, and report the missing prerequisite. Added an explicit recovery check.
- Added checks that a connected outlet cannot replace a missing contact, touching edges must match reciprocally, the inlet must face its supply, and incomplete circuits cannot start machines.
- The full physical walkthrough now solves the boards through their rotation API before continuing the existing crate/core route. These are known authored solutions, not evidence of player skill or a measured first-play duration.

Next priority is observation of first-time players, especially whether they understand contact markers, branching and why the final lantern belongs to the helpers. No duration is claimed until those sessions occur.

## v0.3 / native application operation review

The developer agent operated the signed macOS application through native keyboard input. This was an agent-controlled inspection, not a first-time human playtest. Verified title start, all introductory dialogue, walking, arrow-key camera movement, crate pickup/placement, notebook open/close and pause display. The first placement attempt near a plate was rejected; a later placement succeeded but did not provide a reliable full-door traversal in this native session.

Changes from this observation:
- The opening HUD had described the later workshop gate before the foyer door. It now teaches the immediate crate/plate/centre-door task.
- Added a 1.35-unit placement assist around nearby pressure plates; collision validation still runs at the resulting position. An actual engine check places from outside the plate edge, verifies its final position and verifies the gate condition.
- Added arrow-key look for keyboard users and for robust operation without relative mouse input.
- Explicitly mark agent-operated logs as automated and other sessions as unattributed.

The deterministic full route and inspector checks remain separate from the native UI inspection. Native uninterrupted evasion and a first-time player's duration still need to be observed.

Updated-build native retest: the same near-edge Q placement now visibly aligned the crate on the plate and opened the foyer gate. Traversal through the opening switched the HUD to the workshop objective. Low-core pickup also succeeded. Full uninterrupted native traversal remains unverified.

## Simulated first-contact review

Native operation completed both packaging machines and both routing boards, including recovering the core and maintaining access with the override crate. This was an informed agent's simulation of a newcomer, not a first-time human test. The session stopped at the archive approach. See FIRST-CONTACT-REVIEW.md for scope, confounds and unresolved findings.

Observed and changed: removed recipient spoiler from title copy; reduced the held core's screen footprint; added explicit confirmation and accurate relay prompts for crate-supported access; moved a suspended notice out of the central approach. Visible manufacturing output and the complete native inspector/finale experience remain outstanding.

## v0.4 / visible craft and helper response

Addresses observations from the prior native simulated-first-contact review:
- Added visible manufacturing stages so job completion has a physical result: shell, bow, engraved tag and light. Outputs persist when their core is removed; the Press hands off to one portable lantern.
- Added optional context-dependent conversation with Button, a workshop welcome wave, gaze and completion celebration. Interaction is optional and does not extend the required route with waiting.
- Replaced the repeated workshop objective with state-specific guidance for relay, crate override, core recovery and routing.

Validation must check serialized references, stage visibility and retained outputs, optional dialogue, and the unchanged physical route. Staged screenshots may assess presentation but do not establish timing, fun, or human learning.

Added an actual framed childhood sketch at the archive reveal. The running-machine objective explicitly says to leave the core in until completion; the post-Chime objective no longer implies Hush remains distracted forever.

The first new validation attempt was stopped before flow checks to combine edits. A later attempt caught an unreliable animation-duration assertion under accelerated time. Replaced it with event-trigger and paused-state checks and explicitly froze gesture timers in reading/pause modes. The completed rerun passed 71 engine checks including the ending.

After the 71-check engine rerun, complete deterministic physical traversal passed again (24 camera-ray interactions, four Q placements and four known circuit solutions; inspector movement disabled). Inspected actual runtime craft/drawing previews and successfully used Button's optional conversation in the native app. The release bundle passed integrity/signature checks.
