# v0.9 / interaction and wayfinding review

Date: 2026-10-15. Evidence comes from direct user screenshots and development checks, not an invented independent playtest.

## Reported problems and response

- Visible gold cores at 1.2-1.9 units did not react to E without exact aim. Nearby visible cores now use a 3.8-unit interaction cone, with a clear E prompt. Finished/running socketed cores can be taken directly even though their own colliders are disabled. A solid wall still blocks pickup. An unfinished socket targets its console instead, preserving access to its puzzle. Removing a core clears its real machine/relay reference and stops processing without erasing completed output.
- The vague machine-front marker and repeated E action made the production flow difficult to follow. Markers now use the socket/console and actual machine name. Inserting a core opens the production routing board in that same action. The console body and worktop both remain reachable targets. Irrelevant always-on placement warnings no longer appear while carrying a core or lantern; actual failed Q actions still explain themselves.
- Star requirements were missed. Every required star stays visible and turns green when powered. Three short circuit instructions require every star and every OUT. The optional example includes the same star feedback. Powering all tiles is not required, and the actual board is not solved by the example.
- Old doorway guesses could send the player backward near the archive. Coordinate-based guessing was replaced by a physics-checked grid route. The floor trail and map share its points; capsule-clear shortcuts remove grid zigzags without cutting corners through geometry. Closed doors are obstacles. Guidance asks for the reusable core before directing the player from Packaging into the Archive.
- M or the clickable mini-map opens a paused floor plan with named rooms, eight numbered stations/drawing/dispatch locations, core, player/facing, C crates, P plates, goal and route. M/Esc returns to the retained dialogue/circuit/play mode. Map time is classified as paused time. The map does not complete puzzles or steer around Hush for the player.

## Course brief

Project 2.pdf requires at least one level teaching concepts and later applying them in unique ways. Its rubric gives the highest substantial-content rating for a complete 5-10 minute game without noticeable filler. It sets no separate tutorial duration. The foyer is the brief movement/carry/plate introduction; Packaging is the first formal production challenge. A 45-75 second foyer is a design target requiring first-time human observation. Lost time due to unclear pickup or navigation is not counted as successful learning or substantial content.

## Executed checks

- Final engine suite: 124 checks including animated ending. New checks cover map freeze/resume and dialogue retention, off-centre floor core pickup, disabled-collider socket pickup, one-action circuit opening, through-wall pickup rejection, archive routes around the conveyor and shelves, refusal to cross a closed packaging gate, and retrieving the core before leaving Packaging. Final route smoothing passed the same geometry checks.
- Complete deterministic physical traversal reached the ending with 20 interactions, four validated Q placements and four authored circuit solutions. Traversal uses actual controller/world geometry; inspector locomotion is disabled, time accelerated and solutions known. This is not a human completion-time or difficulty result.
- Catalog audit: 302 complete entries in seven languages. Engine glyph checks cover all translated catalog characters in bundled fonts.
- Staged visual passes exercise 126 screens across seven languages, including enlarged maps, actual archive route views and seven long-dialogue continuations. Observed map-line scaling and English label-height problems were corrected. Final render/native results are recorded below.
- Unity's editor QuickSearch index emitted an internal ArgumentOutOfRangeException before the engine/navigation tests started. Both suites completed successfully; runtime preview/package logs are checked separately for game errors. No change to user system preferences was made to suppress this editor issue.

## References informing the revisions

[Microsoft XAG 109: Objective clarity](https://learn.microsoft.com/en-us/xbox/accessibility/xbox-accessibility-guidelines/109) describes persistent task/progress information, maps, hints and visible ground paths, citing games including Fable III. [Ubisoft's exploration overview](https://www.ubisoft.com/en-us/game/assassins-creed/news/4GKS1MvpHS3f7ebKWfxHmt) describes a pathfinder line on the ground and a guided objective mode. Our changes adapt these ideas to the actual factory geometry and reported player confusion; they are not claims of identical controls across all games.

## Acceptance limits

These checks do not establish first-time comprehension, enjoyable full-speed inspector difficulty, valid 5-10 minute human duration, translated prose quality or audible mixing. The physical route and rendered navigation are developer evidence. Prior v0.8 macOS region screenshot limitations remain separately documented.

## Final packaged and native follow-up

Final packaged seven-language pass: 126 staged screens, seven long-dialogue continuations, no UI_LAYOUT_OVERFLOW or runtime Exception entries. English enlarged archive map, visible smoothed ground trail and circuit instructions were visually inspected. Staged captures are not natural playthroughs.

Native standalone v0.9: opened M during the opening dialogue, closed it and retained the same line. Changed the saved language to English, walked the foyer, picked/placed the crate, crossed into Packaging and picked the core up with the crosshair visibly above it (player approximately x=.34, z=13.36, yaw=0, pitch=-1.1). The E pickup prompt was visible before pickup; carrying feedback appeared afterward. Opening/closing M retained the held core. Command-Q exited with code 0. Native log contained no layout overflow or runtime exceptions. The review was muted and has been closed.

CUA synthetic pointer clicks again did not activate the Unity mini-map button reliably; keyboard M/Esc is verified, whereas hand-operated mouse activation is not claimed as a verified result here. The clickable mini-map and top-row map button are implemented through the same Unity GUI button system as other menus.

Final Unity build, packaged startup smoke, installed/extracted strict deep code signatures and ZIP integrity passed. Archive Builds/MidnightShift-Mac-v0.9.zip SHA-256: 3a42a8fb6eccceee2a246cf6ca89175ad6654f27a6ea1994311b38ebd1d00691. Windows/Linux and independent human timing remain outside these Mac development checks.
