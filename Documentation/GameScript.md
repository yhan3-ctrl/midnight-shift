# Game Script / v0.13 implemented rules

This records the current playable, not every feature proposed in Design-v1.zh.md. All distances are Unity world units and timings are game seconds. A future final report must update this document to match its submitted version.

## Player role, controls and modes

The player is Aunt May's visitor helping magical toy-factory workers finish a childhood order. Exploration uses a first-person camera, WASD at 3.3 units/s by default (adjustable 2–4.5), relative mouse look after explicit click/Enter (arrows also turn), E interaction, Q placement (or core insertion near a machine), R level view, H hint, J journal, M map, F2 screenshot view and Tab/F1/Esc menu. The hardware cursor is locked only after deliberate mouse-look entry, and is released by Esc, modal interfaces, system modifiers or lost focus. Resuming never silently captures it. Free-pointer/right-drag and arrow-only alternatives are available in Controls & comfort. Esc opens the menu during dialogue, routing and notebook; Backspace returns from routing or notebook. M/Esc closes the expanded map. The circuit supports clicks, arrows, Space rotation and Return activation. Interactions use a 3.8-unit ray with a nearby visible-object assist cone (dot product at least .38). The active focus prompt shows the actual action; solid walls still block assisted pickup. No jumping, combat, health pool or global time limit.

Exploration permits walking, looking, carrying and interacting. Reading/dialogue restricts locomotion and freezes world simulation; Space, Return or the visible button advances. The journal freezes simulation and displays the current task, rules and discovered story, then restores the previous carrying state. Pause freezes simulation and offers music/effects sliders, audio test/recovery, a Controls & comfort screen (C) with independent horizontal/vertical sensitivity, vertical FOV (55–90 degrees), walking speed, three input modes and optional hand motion (off by default), optional language switching, resume, restart and Quit (also X). The current dialogue or circuit is retained. The title screen begins the night; the ending offers restart. Reading and pause are not secretly timed hazards.

## Items and placement

One item can be held at a time. A held item has its colliders disabled and follows the carry pose. Carrying does not reduce movement speed. Q normally attempts placement 1.5 units ahead, using a downward surface query. A visible target plate within 3 units in front can be selected directly; a 1.35-unit near-plate assist also applies. The validated landing point is shown as a full-size outline, green on the target plate. Player separation, solid overlap and line of sight still apply. A floor-level surface (height at most 0.25 units), clear volume and separation from the player are required. Raised worktops and shelf tops are rejected. A crate must remain at least 1 unit horizontally from the player after plate assistance (0.65 for other pickups). Failed placement retains the item and explains the failure. No throwing or item destruction.

Crates measure 1.2 by 1.65 by 1.2 units. They appear at 26% scale while held, below the view centre, and return to full scale on valid placement. Crates hold pressure plates and block inspector sight when placed. There is one reusable power core. A lantern exists only after final pressing. No inventory consumption or random drops.

## Gates

Plate gates are open while a player or an uncarried crate overlaps their plate volume. Removing the crate closes the gate. The panel moves vertically at 4 units/s. Closure is deferred only if the panel is already high enough and the player occupies the actual doorway (within 0.65 units of its plane and 1.9 units horizontally from its centre). Approaching a closed gate never opens it. Walls span the room, preventing a walk-around shortcut.

The workshop power gate opens while its relay holds the one core, or while its manual override plate is held. Its override crate begins behind that gate. The player must power the gate, retrieve the crate, bring it back to the plate, then remove and reuse the core. This creates a resource dependency with a reversible solution.

The Packaging gate permanently unlocks after Moulding and Ribbon work completes. The Assembly entrance unlocks after reading the archive drawing. These two gates do not require a continuously powered socket.

## Machines

A machine accepts one core only if its prerequisite is satisfied. An out-of-order attempt keeps the core in the player’s hands and states what is missing. E with the core inserts it and opens the production routing board in one action. E with empty hands near an unfinished console reopens the board; accepting a valid solution starts the machine. The Chime starts directly. E while running stops work and removes the core. E after completion retrieves the core. Removing power stops processing, but partial processing and completed output persist.

Every current job lasts five seconds. These short processing times communicate action results and are not designed as a mechanism to reach the length requirement.

- Moulding Table produces the shell.
- Ribbon Conveyor requires the completed shell and transfers it to assembly.
- Break-time Chime accepts the same core and, when started, asks the inspector to investigate its location. E can remove the core immediately, including during its five-second machine cycle. The 16-second Hush visit continues without power; no waiting is required. Removing, reinserting and starting the core permits another cycle. It does not advance the order.
- Engraving Table requires reading the old drawing and engraves the shell.
- Lantern Press requires engraving and creates a physical lantern beside its pedestal.

Machine work starts emit the same inspector stimulus. He responds to sources in his current duty region: archive z = 31–46 or assembly z = 47–54. After reading the drawing and entering Final Assembly, Hush changes to a final-inspection route. Machine starts in that region then draw his attention as well. There is no random machine failure.

## Power-routing mode

The world and inspector freeze while the player manipulates a circuit. Active play time still counts because solving is gameplay. Click a tile to rotate it clockwise, or select with arrows and rotate with Space. Return or Start machine starts a valid circuit. Esc / Close preserves tile orientation and resumes the world. Remove core closes the board and retrieves the core. Reset tiles restores the board's initial orientation.

Each tile has connectors on some of its north, east, south and west edges. Power begins only if the leftmost source tile faces the incoming supply. It crosses to an adjacent tile only if both touching edges have connectors. Straight pieces carry, elbows turn and T pieces split power. Unused open ends do not cause failure. The board succeeds only when every marked contact is powered and every marked right-edge outlet receives power with its east connector facing out.

- Moulding: 3 by 3 board, one outlet and one required contact, introducing rotation and visible connectivity.
- Ribbon: 3 by 3 board, two outlets and one contact, applying a T junction to split one input.
- Engraving: 4 by 4 board, one outlet and two required contacts, requiring a route through intermediate points rather than only the shortest connection.
- Press: 4 by 4 board, two outlets and two contacts, combining turning, branching and intermediate routing.

All four are authored and have validated solutions. Tile rotations persist when power is removed. The machine cannot start while the board is disconnected or a contact is missing. No hint claims a unique solution. Automated tests use known authored solutions and do not measure real puzzle-solving time or enjoyment.

## Inspector

Hush is a friendly safety inspector. The archive is a safe demonstration throughout: he patrols and investigates the chime but cannot pursue, interrupt or send the player back, even after the chime lesson or drawing reveal. The archive sign, introductory dialogue and compact safe-practice label explain this. No wait for his rest or patrol is a progression condition. Ringing the Chime calls him to its corner for a 16-game-second visit, which remains active if the core is removed immediately. The player can move the crate straight away. Bell power still demonstrates reusing the same physical core.

Only Final Assembly activates the safety challenge, after a paused introduction explains its consequence. Detection uses a 7-unit range and 110-degree horizontal field of view with physical line of sight. Its player region is z = 47–53.3. Patrol waypoints are (4,52), (-1,50), (-1,52), (4,50). Patrol speed is 1.45 units/s; sustained visibility fills the warning over 1.6 seconds, then Approach moves at 2.8 units/s. Hiding behind a shelf or a placed crate breaks sight, leading to Search for four seconds and then Return. A valid production-machine sound attracts an unalerted inspector to a point in front of the machine; it cannot override an active warning or approach.

Full warning followed by contact within 1.1 units causes a local safety retry to the assembly entrance (z = 47.2). Finished jobs, the drawing and crates already holding access remain safe. Other assembly crates return home; the held item is released and a held core is recovered beside the local checkpoint. The inspector resets with five game seconds of grace. The retry dialogue pauses simulation and explains the recovery. No damage, jump scare or global restart occurs. The archive explicitly rejects this reset path and does not drop held items.

## Story progression and ending

Aunt May's foyer note explains the helpers return at Halloween and the old drawing is in the archive. Completing moulding provides Button's hint about the child who asked who went home last. Reading the framed drawing reveals the player's own note: 'For the last one to leave. So the way home is bright, too.'

The reveal unlocks assembly. The player engraves, powers the press, picks up the finished lantern and interacts with Pip at dispatch. Delivery ends the night. The exit opens and the camera follows the helpers walking with their lantern into the illuminated village. Ending text remains visible beside the scene. Reading again does not repeat progression rewards.

## State machines

Player: Active.EmptyHands / Active.Carrying. Pickup enters Carrying; placement, insertion or delivery leaves it. Movement may coexist with either state. Reading, Journal, Routing, Map, Screenshot, DesktopHold and Paused retain carrying state while freezing time. Safety reset returns to Active.EmptyHands. Delivery enters Ending.

Inspector: Patrol -> Alert -> Approach -> Search -> Return -> Patrol. A machine sound may replace Patrol/Search/Return with Investigate; seeing the player replaces investigation with Alert. The return route and sound priority are specified above.

## Rules at three levels

Operational: the controls, interaction prompts, visible socket/plate feedback and actions described above.
Constitutive: one core cannot occupy two machines, one item can be held, dependency flags gate later jobs, completion persists, and delivery requires the finished lantern.
Implicit: players engage through the supplied controls. No rule relies on an unwritten assumption that players will avoid valid shortcuts; discovered shortcuts must be evaluated in iteration.

## Schemas and evidence

Systems of Information: the circuit’s lit connections and outlet/contact counters expose connectivity without showing the answer. Machine labels, job states, plate state, objectives, inspector alert and dialogue reveal different information needed for planning. See Rules of Play slides 86–95.
Systems of Uncertainty: outcomes depend on the player's incomplete layout knowledge, sequence and traversal timing, without random inspector detection. See slides 68–84.
Emergence remains a design hypothesis. Do not claim emergent depth until different viable player strategies are observed.

## Learning claims and validation limits

Required progression exercises holding a door condition while reusing the sole core, preserving finished work, and completing reciprocal, contact-constrained and branching circuits. Crate occlusion and deliberate patrol prediction are optional strategies: the developer native run escaped some detection through movement. Therefore completion does not prove that a player learned occlusion, used distraction strategically, or predicted the patrol. First-time testing must observe and ask about those actions before the report claims them as demonstrated learning.

A complete native developer run reached the ending with the inspector active and one safety retry; see NATIVE-FULL-REVIEW-v0.5.md. It establishes recoverable progression under those tested actions, while the long interruption invalidates its duration for grading.

## Engine check limits

Engine integration checks exercise serialized scene references and actual components for power transfer, gate conditions, job progression, inspector noise, retries and delivery. They place objects and accelerate time deliberately. They do not prove unaided navigation, time-to-completion, music quality, fun or learning. Record real player observations before claiming any of those.

Crate placement has a 1.35-unit horizontal assist around a pressure plate at the same floor level. Placement still rejects solid obstructions. The opening objective describes the foyer crate before introducing the workshop power problem.

## v0.4 / visible craft and responsive helpers

Manufacturing now has a visible object on each relevant worktop. Moulding forms a plain pumpkin shell as processing advances; its output remains when power is removed. Ribbon displays the shell with a plum bow when finished. Engraving adds a small handwritten promise tag after the drawing is found. The Press displays the engraved unlit shell during processing, then clears its display when the sole portable lantern appears on its pedestal. Worktop displays have no pickup, collider or independent progression flag. The final pickup includes bow, tag, lit eyes/smile and warm point light.

Button moves from the foyer to the Moulding work area after the player enters Packaging. Nearby helpers orient toward the player and wave once; Button celebrates completed Packaging jobs. Talking to Button is optional, pauses the world through the normal dialogue mode, and responds to the current manufacturing/story state. It introduces no gate or required wait. During the ending, helper behaviour yields to the authored departure sequence.

Workshop objectives distinguish initial relay power, bringing back the override crate, retrieving the core and using the Moulding Table. These are instructions about current mechanics, not extra tasks.

The archive now contains a framed childlike line drawing of a pumpkin lantern and small helper, in addition to its readable reverse-side note. It is the same ordinary E interaction and has no extra collectible requirement.

## v0.6 / guidance across the whole shift

English is the fresh-install default. English/Chinese selection is available at title and pause, and persists only after selection. Large keycaps, a screen-space marker and a chapter calendar accompany exploration. The guide chooses one immediate action from actual item, gate and job state. Markers use intermediate doorways/corners where needed; they do not simply point through the nearest wall. World-label sizes are bounded by their signs. Placement previews use the same validation as Q; invalid placement receives a separate feedback card.

The foyer teaches looking, approaching the crate, aiming/E, Q onto the plate, and leaving the crate behind to cross. The first successful placement receives a brief positive response. Packaging then introduces shared gate power and the crate override before core transfer, one-route Moulding and branching Ribbon puzzles. Physical output changes from shell to bow, tag and final light, so progress is visible in the world.

The archive entry introduces Hush and the chime in two paused lines. In v0.13 the entire archive is harmless practice; the actual safety challenge is introduced in Final Assembly.

The assembly entry reconnects the childhood promise with the task. While the Engraver runs, the immediate guide asks the player to prepare the crate gate instead of standing exposed beside the machine. After completion, the core moves to the Press for the combined branching/contact puzzle. The final delivery gives the crafted item to a character and changes the whole scene through the departure animation.

H now repeats the current actionable instruction and reason; it cannot contradict the ongoing job with a future core-transfer instruction. Routing provides visible contact/outlet counters, a gold input arrow and an optional animated connection example. The example does not solve or rotate the actual board. Dialogue and routing pause hazards, with explicit text explaining this; Tab/F1 opens the exit menu from either.

Audio initialization/recovery now restarts stopped sources and rebuilds generated clips after configuration changes. The reported intermittent initial silence has not been reproduced; playback-state checks do not establish audible output or musical quality.

## v0.7 / seven-language interface

Fresh preferences default to English. The title and pause menu open a shared Language selector with English, 简体中文, Español, Français, Deutsch, 日本語 and 한국어. L opens it from those screens; arrows select, Enter confirms, Esc returns. Explicit choices persist in PlayerPrefs. Switching language preserves the current game, carried item, dialogue line and circuit; it is presentation state, not progression. Unsupported preference values fall back to English.

The 263-entry resource catalog covers interface, instructional actions/reasons, formal-level checklists, item/machine prompts, story dialogue and recovery messages. Factory signage and the fictional brand/title remain English. Input letters and circuit IN/OUT remain stable across languages, with explanations translated. Japanese and Korean use bundled Noto fonts subsetted to the needed glyphs; licenses are included.

Long dialogue is split using actual font measurements. Continue/Space/Enter first advances the visible page, then advances the underlying story line after its final page. The page counter is visible when needed. Opening a new conversation resets pagination; changing language restarts the current line at its first page. This avoids clipping translations or shrinking narrative text to an unreadable size.

Translations are authored implementation drafts, not a claim of professional native-speaker proofreading. Visual and coverage checks do not establish culturally natural dialogue or new-player comprehension in every language.

## v0.8 / historical operating-system revision (mouse default superseded by v0.10)

Based on direct player feedback, active play no longer locks or confines the hardware cursor. Hold the right mouse button and drag to look; release it for a free pointer. Arrow-key looking remains available. Ordinary pointer motion alone cannot rotate the view or satisfy the initial look lesson. System modifiers suspend game input, so system shortcuts do not also place/use objects. The first v0.8 launch restores a normal 1280×800 resizable window; fullscreen switching is enabled for the native window controls.

Esc opens the menu consistently during dialogue, circuits and notebook; Backspace returns from circuits, and J/Backspace closes the notebook. The menu retains the underlying mode. The top-left Menu and Screenshot controls are clickable. F2 freezes the world and hides the main HUD for system screenshots; F2/Esc returns to the previous mode. Losing focus during active play freezes without covering the scene with the pause menu. Returning requires Enter or the visible Resume button. Screenshot/desktop hold time is excluded from active play timing.

The destination striped plate has a pulsing gold border. A world-space crate outline uses the same validated landing position as Q: green when it is on the target plate, pale blue for ordinary floor placement, red for an invalid candidate. Q appears only as a keycap/fixed instruction, never as a spatial location marker. The carried crate is smaller (26% visual scale) so the plate remains visible. Within 3 units and in front of the player, a clear target plate can be selected directly; collision, player separation and line-of-sight validation still apply. The final placement cannot pass through a wall. The physical output remains full size.

Reference decisions: Minecraft's official control guide ties placement to the surface being pointed at (https://www.minecraft.net/article/minecraft-controls). Unity documents that a locked cursor blocks UI interaction and recommends capture only after user action (https://docs.unity.com/en-us/engine/6000.0/script-reference/unityengine/cursor/lockstate). This game uses an unconfined drag gesture and visible world feedback to suit the player's desktop/screenshot requirements. Apple documents Shift-Command-4 region selection at https://support.apple.com/en-gb/102646 . These references informed the redesign; they are not claims of identical controls across all games.

## v0.9 / interaction and navigation revisions from direct play feedback

The player reported that a visible nearby core still required exact aiming, the console marker felt vague, star prerequisites needed clearer teaching, and doorway hints led to circles near the archive. Nearby visible cores now receive a forgiving interaction cone within 3.8 units; line of sight still blocks through-wall pickup. Socketed cores are directly retrievable when the machine is finished or running, despite their disabled pickup colliders. An unfinished console opens its circuit instead. Core insertion opens the production circuit in the same E action; both body and worktop remain valid console targets. Current focus is shown in the interaction strip.

Star contacts remain visibly star-shaped after activation and turn green. The circuit instructions explicitly require every star and every outlet, with live counters. Stars are not optional collectibles and all tiles need not be powered.

M or the clickable upper-left mini-map opens a paused floor plan. It identifies Entrance, Packaging, Archive and Assembly, numbered machines, drawing and dispatch, the player/facing, physical core, C crates, P plates, current goal and route. The floor trail and map use the same physics-checked grid route, avoiding walls, shelves, conveyors, crates and closed gates. Guidance uses the next reachable route segment and full route distance. The old coordinate-based doorway guessing is removed. Archive guidance requests the reusable core before sending the player to the next room. Map navigation does not rotate circuits, move objects or avoid the inspector on the player's behalf. Map time is recorded as paused time.

The course brief requires teaching and later novel application; it does not prescribe a separate tutorial duration. The 5-10 minute criterion applies to the complete game without noticeable filler. The foyer remains the short interactive introduction to looking, walking, carrying and plate placement; Packaging is already the first production challenge. A 45-75 second foyer is a design target to observe in first-time play, not a measured result. Required later transfer includes crate-held access while reusing power, turning/splitting through contacts, and safe traversal with the inspector. Search time caused by unclear navigation is not treated as learning or substantial content.


## v0.10 / discoverable first-person looking and comfort

The user's friend reported being unable to discover how to turn, only moving sideways, and feeling 3D nausea. This is user-reported feedback from a voice message, not a developer-observed session or a measured completion time. The right-drag-only default from v0.8 is superseded.

After dialogue, an explicit first-use panel teaches moving the mouse in all directions without holding a button. Clicking the visible entry button or pressing Enter captures relative mouse input. Yaw is unrestricted, pitch clamps to ±72 degrees, and there is no automatic turn, recentering, camera bob or motion blur. A short repeat-entry panel appears after later interruptions. The first look instruction is preserved even when the crate is far away; distance navigation must not override that lesson. Arrows remain a fallback, never a requirement for ordinary mouse looking.

Esc opens pause and releases the pointer immediately. Dialogue, routing, map, notebook, screenshot view, a system modifier, or lost application focus release capture too. Modal screens reject capture. Closing a modal restores a safe, frozen look-entry state until the player explicitly resumes mouse look. Advancing the final dialogue with Return cannot also consume that same press as a capture request. Pointer/right-drag and arrow-only modes instead retain a free pointer during active exploration. No mode uses cursor confinement.

Buttons visibly brighten and gain a light outline on hover. Enabled buttons, circuit tiles and the clickable mini-map request a code-authored pointing-hand cursor; disabled controls do not. The map heading is text, and its return control is simply ← Back. Esc and M remain supported shortcuts.

Controls & comfort [C] can open from title, pause or look entry. It saves explicit choices locally when closed. Horizontal and vertical sensitivity range .3–4 independently; FOV is vertical, 55–90 degrees, default 72. Default walking speed is 3.3 units/s and hand bob is off. There is no claim that these options eliminate motion sickness. Awaiting look entry and comfort-screen time are excluded from active-play counters and logged as separate paused states.

The design follows the basic mouse-look/WASD/Esc pattern described in Minecraft's official controls guide (https://www.minecraft.net/en-us/article/minecraft-controls). Adjustable FOV/sensitivity and optional secondary motion are informed by Xbox Accessibility Guideline 117 (https://learn.microsoft.com/en-us/xbox/accessibility/xbox-accessibility-guidelines/117). Physical mouse feel and the friend's comfort must be retested by a human; automated camera math and GUI captures do not establish them.


## v0.11 / visible route and obvious insertion vs pause

A user screenshot showed a carried gold core facing the moulding worktop while DesktopHold was active. This supports two separate findings: inputs were intentionally suspended by focus/shortcut protection, and generic Q placement rejected an elevated worktop. It does not prove that every earlier failed action occurred during pause. Desktop hold now has a dedicated paused screen explaining that E/Q do nothing until Enter/resume. It hides the normal task/action HUD, while retaining menu/map/screenshot access. The user still deliberately enters mouse look after resuming.

Gold floor dashes now have full opacity and .38-unit width (previously .14), with a .055-unit visual elevation offset above the original route. Stationary diamond-shaped route lights rise to about 1.2 units at intervals along the same collision-checked route. They are depth tested against walls, do not collide with the player and do not steer around Hush. The map and route planner remain unchanged. The compass uses horizontal signed heading only; target elevation and camera pitch cannot request looking down. R explicitly returns pitch to zero and preserves yaw; no automatic camera motion is introduced.

A carried core prioritizes a nearby machine/relay over decorative interactables. Machine reach uses the visible socket or nearest console surface, checks horizontal facing, accepts very close peripheral targets and still rejects solid-wall occlusion. E remains insertion; Q near a socket now invokes that same machine action, including its real prerequisite checks and circuit opening. It does not drop the core onto a worktop. Q away from a machine still uses validated floor placement. All crate placement, overlap, wall and pressure-plate rules remain unchanged. The footer shows Insert when that is the actual nearby action. The large guide switches to E/Q insertion only if the nearby console matches the current task, so walking past a completed machine cannot replace archive navigation with an irrelevant instruction.

These changes address the reported visibility and intent mismatch. Staged level-view renders and engine checks are developer evidence, not a claim of independent human usability, comfort, completion time or learning.

## v0.12 game review changes

Formal-game guidance defaults to one short next action. J opens the actual chapter checklist, core whereabouts and the remembered promise; H requests the detailed explanation. During dialogue the exploration HUD is hidden. Gold outlines mark reachable focused objects. Direct aimed objects take priority over nearby core assistance; story boards support the same visible-object reach aid as pickups. Crafting does not request needless walking.

Safety inspection builds detection over 1.6 seconds and grants five seconds of grace on room-duty transition/retry. Retries preserve crates already holding access, completed production jobs and the drawing; other local crates recover home. The sole held core is recovered locally. Ribbon completion now includes Button's two-line response connecting its crooked bow to the archived drawing. The last lantern uses a smaller, lower carry pose. Full problem/change/retest evidence is in FINAL-GAME-PLAYTEST.md.

## v0.13 / clear lesson boundary and harmless archive practice

Direct player feedback reported confusion about Hush being friendly while sending the player back during the first crate task. The opening teaches looking, walking, picking up and placing a crate, then announces the formal night shift without another blocking dialogue. Packaging and Archive are formal chapters that introduce tools in context. Archive now demonstrates the chime safely without retries; Assembly combines crate access, power reuse and the announced safety check. The short opening targets roughly one minute, but independent first-play timing is not validated. Added perimeter pumpkin lamps, paper bats, hanging paper ghosts and treat buckets with no colliders or interactions. No extra collection task or forced wait was added. See HUSH-REVIEW-v0.13.md for checks and limits.
