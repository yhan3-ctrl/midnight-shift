# v0.10 / first-person mouse look and comfort review

## Trigger and design decision

The user supplied a friend's voice-message feedback: turning was not discoverable, the player mostly strafed, felt 3D nausea, could not recognize clickable controls, and disliked the map's verbose Back shortcut label. This is attributed user-reported human feedback. No completion duration or independent observation was supplied. The v0.8 right-drag default is superseded.

Default exploration now uses relative mouse look after an explicit click or Enter. The first entry explains turning in every direction without holding a mouse button; later returns use a shorter entry panel. WASD moves and arrows remain a fallback. Esc releases the pointer and opens pause. Map, circuits, dialogue, notebook, photo view, system modifiers and lost focus release capture. Nothing silently captures on resume. Look entry freezes hazards and is excluded from active-play counters. Advancing a final dialogue with Enter cannot also capture on that same key press.

Buttons brighten and gain a light border on hover; enabled buttons, mini-map and circuit tiles request a code-authored pointing-hand cursor. Map has a plain ← Back control. The map heading remains text. C opens Controls & comfort from title, pause or look entry. Settings offer separate X/Y sensitivity (.3–4), vertical FOV (55–90°, default 72°), walking speed (2–4.5, default 3.3 units/s), three input modes and optional hand motion, off by default. There is no camera bob, automatic turning/recentering, or motion blur. Explicit settings are stored locally. These options are not a claim to eliminate motion sickness.

The ordinary controls pattern follows [Minecraft's official controls guide](https://www.minecraft.net/en-us/article/minecraft-controls). Adjustable FOV, sensitivity and optional secondary movement follow the relevant design considerations in [Xbox Accessibility Guideline 117](https://learn.microsoft.com/en-us/xbox/accessibility/xbox-accessibility-guidelines/117). User-action-only capture and release on UI/focus changes also reflect [Unity's cursor guidance](https://docs.unity3d.com/6000.0/Documentation/ScriptReference/Cursor-lockState.html).

## Executed verification

| Evidence | Result and scope |
|---|---|
| Work/playcheck-v010-final.log and Work/playcheck.txt | **145 engine integration checks pass**, including animated ending. Added real camera-transform X/Y rotation, negative movement, pitch clamp, stationary-camera behavior, capture rejection by modals, safe return states, static-hands defaults and alternative modes. A regression check preserves the first look lesson despite distant navigation. These camera deltas are programmatically supplied; they do not prove physical mouse behavior. |
| Work/navigation-v010.log and Work/navigation.txt | Complete deterministic physical traversal to ending: **20 interactions, four validated placements, four authored circuits**. CharacterController collision remains active; inspector locomotion is disabled, solutions known, time accelerated. No human timing/fun/learning claim. Later changes were UI text/layout and first-look guidance only. |
| Tools/check-localization.py | **331 complete entries × seven languages**, literal guide/UI coverage. JP/KR subsets refreshed; per-language glyph coverage is included in engine checks. |
| Work/languages-v010-release.log | **147 staged screens**, seven long-dialogue continuations, no UI_LAYOUT_OVERFLOW or game-runtime exception entries. Includes look entry, comfort and FPS HUD in all languages. HUD capture sets the private capture state by reflection solely for rendering with player movement disabled; it is not physical capture/input evidence. Preferences are not saved by this harness. |
| Visual inspection | English title, first look entry, default FPS HUD, map/Back and comfort screen; French comfort after shortening its overflowing pointer-mode button. Native review found the two-line FPS footer needed more height; final rendering corrects it and moves the focus strip clear of the action coach/footer. |
| Work/native-v010.log | Silent developer keyboard review confirmed explicit entry, real arrow-key camera rotation, Esc menu/release, deliberate re-entry, focus hold and map/Back keyboard flow. An observed footer overflow was fixed afterwards. |
| Work/native-v010-release.log | Final silent app: C opens comfort from title, values display correctly, Esc returns, intro advances, explicit Enter enters exploration, and the MOUSE coach remains visible rather than being replaced by a walking instruction. No runtime exception or layout-overflow entries. Cmd-Q exits 0. |
| Work/packaged-smoke.log and Work/extracted-v010.log | Installed and ZIP-extracted startup smoke pass: scene references, title action, dialogue and journal. ZIP integrity and strict deep signature verification pass. |

Native CUA mouse events again moved the displayed pointer but did not reliably activate Unity GUI controls or produce relative mouse-axis motion. A failed mouse attempt is **not** counted as a pass. Physical mouse look, actual hand-cursor appearance, hover/slider/button operation, OS region screenshot completion and the friend's comfort remain for human retesting. Keyboard entry/release and programmatic camera rotation are separately evidenced above.

One editor run emitted an internal UnityEditor.Search startup indexing exception before gameplay checks; the integration runner subsequently passed. Packaged runtime logs were checked separately. A stale review instance was reselected by the UI tool after quitting; it was closed, the final build identified by its visible [C] label, and all v0.10 review processes closed. The user's original v0.9 process was preserved.

## Local delivery

- App: `/Users/unnh/Applications/Midnight Shift v0.10.app` (separate from the still-running older app).
- ZIP: `Builds/MidnightShift-Mac-v0.10.zip`.
- SHA-256: `d43cfefe9f50d8eeb18d597be2c351d23d72c1b34127ebd8fd3e49a1dfaa9489`.
- Local and desktop project play shortcuts point to v0.10. Close the older game before launching the new shortcut, since macOS may activate an already-running app with the same bundle ID.
- English remains the default; seven explicit language choices remain supported. Agent/automated flags mute audio, normal launches keep sound. No headset or global audio setting was changed.
- Build invocation used: `zsh Tools/build-mac.zsh '/Users/unnh/Applications/Midnight Shift v0.10.app'`.

## Next human check

From a fresh run, the player should be able to explain and demonstrate mouse turning before walking to the crate, open/close the map without help, identify buttons by hover, pause/release to use window controls, and adjust comfortable view/speed settings. Record the actual player's report and any failures. Do not push a nauseated tester to continue. This revision is not an independent first-run playtest, a validated 5–10-minute duration, a cure for motion sickness, or a final course-submission claim. Windows/Linux execution and external publication/submission remain unperformed.
