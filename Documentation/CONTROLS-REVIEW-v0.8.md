# v0.8 / cursor access and crate placement review

Date: 2026-10-15. Trigger: direct user feedback that region screenshots and native window controls were obstructed, and the floating Q was mistaken for the destination.

## Changes

The game never locks or confines the hardware cursor. Looking uses right-button dragging or arrow keys; ordinary pointer movement does not rotate the camera. Releasing the button leaves a free pointer. Command/Control/Option modifiers suspend gameplay input. The opening lesson and seven-language controls explain this gesture.

Esc consistently opens the menu from gameplay, dialogue, circuits and notebook; Backspace returns from a circuit. Top-left Menu and Screenshot buttons remain visible during these modes. F2 freezes the world and hides the main HUD, and F2/Esc restores the prior mode. Active play losing focus freezes without a full-screen pause overlay; returning requires Enter or Resume. These interruptions do not count as active game time.

The real striped wooden plate receives a gold outline. The full-size crate landing outline uses exactly the same validated position as Q. Green means aligned with the target plate, pale blue means ordinary clear floor, red means blocked. Q appears only as a fixed keycap/instruction. Near a clear plate, alignment assistance still respects walls, overlap and player separation. When already aligned, the generic turn-camera hint is suppressed. The held crate is visually smaller; the placed crate retains its full physical size.

## Executed checks

- 112 engine integration checks passed, including cursor state, photo freeze/resume, desktop hold/resume, actual guide/landing agreement, through-wall placement rejection, seven-language glyph coverage and the animated ending. Full deterministic traversal reached the ending with 24 camera-ray interactions, four Q placements and four circuits. Inspector locomotion was disabled and time accelerated for that route; these are not human difficulty or duration results.
- Final seven-language layout pass produced 105 staged screens with no overflow/exception log entries and advanced seven long-dialogue pagination cases. Source coverage passed for 283 entries × seven languages. A later one-line change only suppresses an already-aligned navigation hint; no translations changed after that pass.
- Native standalone session used the normal opening, keyboard look/movement, E pickup, the new green plate outline, Q placement and passage into Packaging. The crate visibly occupied its preview and the checklist advanced. No teleports or progression shortcuts were used in this opening session. F2 displayed the scene without the main HUD; Esc returned. Losing focus visibly exposed the desktop Resume state, which Return cleared.
- Native macOS fullscreen button entered fullscreen; the native Window menu returned to a normal window. The red close button ended the standalone process with exit code 0. This establishes these accessible window actions, not every physical mouse gesture.
- Attempted Command-Shift-4 plus region dragging through CUA twice, including screenshot view. No resulting Desktop PNG could be verified. Therefore OS region screenshot capture is **not marked passed**. Tool event routing or a configured screenshot destination may be involved; no system screenshot preference was changed. The no-lock implementation and free pointer are verified separately.
- Physical right-button dragging cannot be fully simulated by the currently documented CUA drag API. Arrow look, modifier suppression and idle-pointer behaviour are covered; real hand-operated drag feel and the user's screenshot workflow remain a human acceptance check.
- Final native title review restored English through L/Left/Enter and verified the English controls on screen. The muted review session was then closed with the native red button (exit code 0). Automated/agent launches were muted. No global audio device or volume preferences were changed.

## Package

Unity 6000.6.5f1 successfully built v0.8. Installed and extracted app strict deep signatures, packaged startup smoke and ZIP integrity passed. Archive: Builds/MidnightShift-Mac-v0.8.zip. SHA-256: 4be43152616258564da7d26f4a280b89af86929b8b39433f85cb681e41988e6f.

## Reference decisions

[Minecraft's official controls guide](https://www.minecraft.net/article/minecraft-controls) makes placement refer to a pointed-at physical surface. [Unity cursor documentation](https://docs.unity.com/en-us/engine/6000.0/script-reference/unityengine/cursor/lockstate) explains how a locked cursor prevents UI interaction. [Apple's screenshot instructions](https://support.apple.com/en-gb/102646) describe Command-Shift-4 followed by region dragging. The revised game uses a free pointer plus explicit physical landing feedback to address this player's needs; identical controls across games are not claimed.

Independent first-time comprehension, enjoyable full-speed patrol difficulty, audible mixing and valid 5–10 minute human timing still require human observation.
