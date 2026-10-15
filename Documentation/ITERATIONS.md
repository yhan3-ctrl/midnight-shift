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
