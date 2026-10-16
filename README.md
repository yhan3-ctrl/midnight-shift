# Midnight Shift: The Last Lantern

A warm Halloween puzzle game set inside Willow Works. Finish a forgotten pumpkin lantern, share the factory's last power core and discover who the gift was meant for.

Unity 6000.6.5f1, 3D, Built-in Render Pipeline. Current gameplay revision: **v0.12**. English is the default; Chinese, Spanish, French, German, Japanese and Korean are optional.

## Play on macOS

Open `开始试玩.app` in the project folder, or the installed `Midnight Shift v0.12.app`. The distributable archive is `Builds/MidnightShift-Mac-v0.12.zip`.

## Controls

| Control | Action |
|---|---|
| WASD | Walk |
| Mouse | Look after explicitly clicking/pressing Enter to begin mouse look |
| Arrow keys | Alternative camera turning; select circuit tiles while routing |
| E | Pick up, use, read or deliver |
| Q | Place; near a visible console, insert a held power core |
| R | Level the view while retaining your heading |
| H | Explain the current step |
| J | Full chapter checklist, core whereabouts and notebook |
| M | Expandable factory map |
| Space / Enter | Continue dialogue; Space turns a selected circuit tile, Enter starts a solved circuit |
| Esc / Tab | Pause and free the pointer |
| Backspace | Return from a circuit, map or notebook |
| F2 | Pause in a clean screenshot view; F2/Esc returns |
| C | Controls and comfort from the title, pause or look-entry screen |

Mouse look never resumes automatically after a menu, dialogue or desktop interruption. Click/Enter explicitly to continue. Switching windows or holding desktop shortcut modifiers releases the pointer and pauses hazards. Use Enter to resume desktop pause. Arrow-only and right-drag alternatives are available in Controls & comfort; sensitivity, field of view, walk speed and optional hand motion can be adjusted.

## Playing the night shift

Follow the gold route and the next-action card. A striped pressure plate is a physical target; the green crate outline shows a valid placement. Leave the crate there to keep the door open.

There is one reusable gold core. E or Q near a visible console inserts it and opens the circuit board. Rotate tiles to connect IN to every OUT **through every star contact**, then choose Start machine. Completed work remains finished after retrieving the core. Hush checks safety: watch his route, use the chime or hide behind shelves and placed crates. Retries preserve completed jobs, the drawing and crates already holding access.

Dialogue hides the exploration HUD. The compact calendar opens the complete checklist with J. The story ends with physically delivering the finished lantern to Pip.

## Development and evidence

Open this directory through Unity Hub, then open `Assets/Scenes/WillowWorks.unity`. The scene generator and build commands are in Unity's Willow Works menu. `Tools/build-mac.zsh` regenerates the authored scene, builds, signs, smoke-checks and packages the Mac app. Save any manual scene changes before running the generator.

- [Playtesting findings and revisions](Documentation/FINAL-GAME-PLAYTEST.md)
- [Reported player feedback](Documentation/PLAYTEST.md)
- [Iteration history](Documentation/ITERATIONS.md)
- [Technical validation](Documentation/VALIDATION.md)
- [High concept](Documentation/HighConcept.md), [Game script](Documentation/GameScript.md), [Design schemas](Documentation/Schemas.md), [State machines](Documentation/FSM.md)
- [Asset and font credits](Documentation/CREDITS.md)

Local session records distinguish automatic development checks from unattributed ordinary runs. They contain progression, pauses, reading, hints and retries; they are stored locally with no network transport. Tool-controlled or solution-informed session times do not establish first-play duration. Independent completion timing, player comfort and audio listening remain separate acceptance checks.

Windows/Linux release execution and course distribution/submission are tracked separately from this Mac gameplay review. Passing development checks is not a predicted grade.
