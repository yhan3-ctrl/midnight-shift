# Game Script / v0.3 implemented rules

This records the current playable, not every feature proposed in Design-v1.zh.md. All distances are Unity world units and timings are game seconds. A future final report must update this document to match its submitted version.

## Player role, controls and modes

The player is Aunt May's visitor helping magical toy-factory workers finish a childhood order. Exploration/puzzle play uses a first-person camera, WASD movement at 4.1 units/s, mouse/arrow-key look, E interaction, Q placement, H hint, J journal and Esc pause. Circuit routing has its own mode with clickable tiles, arrow-key selection, Space rotation, Return activation and Esc close. No jumping, combat, health pool or global time limit. Interactions use the first collider on a camera ray, maximum distance 3.1 units.

Exploration permits walking, looking, carrying and interacting. Reading/dialogue restricts locomotion and freezes world simulation; Space, Return or the visible button advances. The journal freezes simulation and displays the current task, rules and discovered story, then restores the previous carrying state. Pause freezes simulation and offers music/effects/sensitivity sliders, resume and restart. The title screen begins the night; the ending offers restart. Reading and pause are not secretly timed hazards.

## Items and placement

One item can be held at a time. A held item has its colliders disabled and follows the carry pose. Carrying does not reduce movement speed. Q attempts placement 1.5 units ahead, using a downward surface query. A level surface and clear volume are required. Failed placement retains the item and explains the failure. No throwing or item destruction.

Crates measure 1.2 by 1.65 by 1.2 units. They appear at 65% scale while held, below the view centre, and return to full scale on valid placement. Crates hold pressure plates and block inspector sight when placed. There is one reusable power core. A lantern exists only after final pressing. No inventory consumption or random drops.

## Gates

Plate gates are open while a player or an uncarried crate overlaps their plate volume. Removing the crate closes the gate. The panel moves vertically at 4 units/s. Closure is deferred only if the panel is already high enough and the player occupies the actual doorway (within 0.65 units of its plane and 1.9 units horizontally from its centre). Approaching a closed gate never opens it. Walls span the room, preventing a walk-around shortcut.

The workshop power gate opens while its relay holds the one core, or while its manual override plate is held. Its override crate begins behind that gate. The player must power the gate, retrieve the crate, bring it back to the plate, then remove and reuse the core. This creates a resource dependency with a reversible solution.

The Packaging gate permanently unlocks after Moulding and Ribbon work completes. The Assembly entrance unlocks after reading the archive drawing. These two gates do not require a continuously powered socket.

## Machines

A machine accepts one core only if its prerequisite is satisfied. An out-of-order attempt keeps the core in the player’s hands and states what is missing. E with the core inserts it. For production machines, E with empty hands opens a power-routing board until that board has a valid solution; after a solution is accepted the machine starts. The Chime starts directly. E while running stops work and removes the core. E after completion retrieves the core. Removing power stops processing, but partial processing and completed output persist.

Every current job lasts five seconds. These short processing times communicate action results and are not designed as a mechanism to reach the length requirement.

- Moulding Table produces the shell.
- Ribbon Conveyor requires the completed shell and transfers it to assembly.
- Break-time Chime accepts the same core and, when started, asks the inspector to investigate its location. After its five-second cycle it marks itself complete, allowing E to remove the core. Removing, reinserting and starting the core permits another cycle. It does not advance the order.
- Engraving Table requires reading the old drawing and engraves the shell.
- Lantern Press requires engraving and creates a physical lantern beside its pedestal.

Machine work starts emit the same inspector stimulus. He responds to sources in his current duty region: archive z = 31–46 or assembly z = 47–54. After reading the drawing and entering Final Assembly, Hush changes to a final-inspection route. Machine starts in that region then draw his attention as well. There is no random machine failure.

## Power-routing mode

The world and inspector freeze while the player manipulates a circuit. Active play time still counts because solving is gameplay. Click a tile to rotate it clockwise, or select with arrows and rotate with Space. Return or Send power starts a valid circuit. Esc / Close preserves tile orientation and resumes the world. Remove core closes the board and retrieves the core. Reset tiles restores the board's initial orientation.

Each tile has connectors on some of its north, east, south and west edges. Power begins only if the leftmost source tile faces the incoming supply. It crosses to an adjacent tile only if both touching edges have connectors. Straight pieces carry, elbows turn and T pieces split power. Unused open ends do not cause failure. The board succeeds only when every marked contact is powered and every marked right-edge outlet receives power with its east connector facing out.

- Moulding: 3 by 3 board, one outlet and one required contact, introducing rotation and visible connectivity.
- Ribbon: 3 by 3 board, two outlets and one contact, applying a T junction to split one input.
- Engraving: 4 by 4 board, one outlet and two required contacts, requiring a route through intermediate points rather than only the shortest connection.
- Press: 4 by 4 board, two outlets and two contacts, combining turning, branching and intermediate routing.

All four are authored and have validated solutions. Tile rotations persist when power is removed. The machine cannot start while the board is disconnected or a contact is missing. No hint claims a unique solution. Automated tests use known authored solutions and do not measure real puzzle-solving time or enjoyment.

## Inspector

Hush is a safety inspector, not a lethal enemy. His four patrol waypoints form a rectangle in the archive. Patrol speed is 1.45 units/s. He sees the player within 7 units and a 110-degree horizontal field of view, provided a wall or placed crate does not block the physics ray. In archive duty, only player positions z = 31.5–44.8 are detectable. In assembly duty this region becomes z = 47–53.3. Assembly patrol waypoints are (4,52), (-1,50), (-1,52), (4,50). This version uses a magical relocation between duties, announced by a message, rather than pathing the entire inter-room transition.

Continuous visibility fills alert over 0.95 seconds. Alert pauses patrol and faces the player. Full alert begins Approach at 2.8 units/s. Lost sight begins Search of the last seen position; search lasts 4 seconds, then Return. Sounds initiate Investigate at 1.8 units/s with 5 seconds at the destination, then Return. The destination is a walkable point 1.6 units in front of the sounding machine, not the occupied centre of its cabinet. New sound cannot cancel Alert or Approach. Movement checks world obstacles and attempts a lateral detour; complex navigation still needs gameplay evaluation.

Within 1.1 units of the detected player, a safety retry moves the player to the current region checkpoint (archive z = 31.7; assembly z = 47.2), resets that region’s crates and the inspector, and releases the held item. A held core is recovered beside the local checkpoint, avoiding a long return trip. Finished jobs and the story reveal remain complete. After a safety reset or a change of duty, 2.5 game seconds of detection grace allow the player to reorient. No damage, jump scare or global restart occurs. The hint and message explain the retry. The notebook also names the current location or occupied machine of the shared core, reducing accidental backtracking.

## Story progression and ending

Aunt May's foyer note explains the helpers return at Halloween and the old drawing is in the archive. Completing moulding provides Button's hint about the child who asked who went home last. Reading the framed drawing reveals the player's own note: 'For the last one to leave. So the way home is bright, too.'

The reveal unlocks assembly. The player engraves, powers the press, picks up the finished lantern and interacts with Pip at dispatch. Delivery ends the night. The exit opens and the camera follows the helpers walking with their lantern into the illuminated village. Ending text remains visible beside the scene. Reading again does not repeat progression rewards.

## State machines

Player: Active.EmptyHands / Active.Carrying. Pickup enters Carrying; placement, insertion or delivery leaves it. Movement may coexist with either state. Reading, Journal, Routing and Paused retain carrying state while freezing time. Safety reset returns to Active.EmptyHands. Delivery enters Ending.

Inspector: Patrol -> Alert -> Approach -> Search -> Return -> Patrol. A machine sound may replace Patrol/Search/Return with Investigate; seeing the player replaces investigation with Alert. The return route and sound priority are specified above.

## Rules at three levels

Operational: the controls, interaction prompts, visible socket/plate feedback and actions described above.
Constitutive: one core cannot occupy two machines, one item can be held, dependency flags gate later jobs, completion persists, and delivery requires the finished lantern.
Implicit: players engage through the supplied controls. No rule relies on an unwritten assumption that players will avoid valid shortcuts; discovered shortcuts must be evaluated in iteration.

## Schemas and evidence

Systems of Information: the circuit’s lit connections and outlet/contact counters expose connectivity without showing the answer. Machine labels, job states, plate state, objectives, inspector alert and dialogue reveal different information needed for planning. See Rules of Play slides 86–95.
Systems of Uncertainty: outcomes depend on the player's incomplete layout knowledge, sequence and traversal timing, without random inspector detection. See slides 68–84.
Emergence remains a design hypothesis. Do not claim emergent depth until different viable player strategies are observed.

## Current validation limits

Engine integration checks exercise serialized scene references and actual components for power transfer, gate conditions, job progression, inspector noise, retries and delivery. They place objects and accelerate time deliberately. They do not prove unaided navigation, time-to-completion, music quality, fun or learning. Record real player observations before claiming any of those.

Crate placement has a 1.35-unit horizontal assist around a pressure plate at the same floor level. Placement still rejects solid obstructions. The opening objective describes the foyer crate before introducing the workshop power problem.

## v0.4 / visible craft and responsive helpers

Manufacturing now has a visible object on each relevant worktop. Moulding forms a plain pumpkin shell as processing advances; its output remains when power is removed. Ribbon displays the shell with a plum bow when finished. Engraving adds a small handwritten promise tag after the drawing is found. The Press displays the engraved unlit shell during processing, then clears its display when the sole portable lantern appears on its pedestal. Worktop displays have no pickup, collider or independent progression flag. The final pickup includes bow, tag, lit eyes/smile and warm point light.

Button moves from the foyer to the Moulding work area after the player enters Packaging. Nearby helpers orient toward the player and wave once; Button celebrates completed Packaging jobs. Talking to Button is optional, pauses the world through the normal dialogue mode, and responds to the current manufacturing/story state. It introduces no gate or required wait. During the ending, helper behaviour yields to the authored departure sequence.

Workshop objectives distinguish initial relay power, bringing back the override crate, retrieving the core and using the Moulding Table. These are instructions about current mechanics, not extra tasks.

The archive now contains a framed childlike line drawing of a pumpkin lantern and small helper, in addition to its readable reverse-side note. It is the same ordinary E interaction and has no extra collectible requirement.
