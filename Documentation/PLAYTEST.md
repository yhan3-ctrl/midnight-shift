# Reported player feedback and playtesting record

## Evidence categories

The user supplied direct observations and screenshots from their own play, and a transcript of a friend's first-person-game experience. These are real reported experiences, with identities left unspecified. Exact build numbers, complete timings and independent full-session recordings were not provided; they are not inferred from the comments. No feedback below is presented as a test of the latest build unless explicitly stated.

The separate [final game review](FINAL-GAME-PLAYTEST.md) records developer-operated window sessions and five deterministic scenario lenses. Those are not five recruited human participants.

## Feedback supplied by the user

| Report | Observation supplied | Change motivated by it | Evidence / current limit |
|---|---|---|---|
| Initial first use | Intermittent silent startup; small text; obscured world instructions; no clear next step; difficulty exiting. | Audio recovery, readable UI fonts/keycaps, stepwise instruction, calendar tasks, pause and quit access. | FIRST-USE-REVIEW-v0.6.md. The exact original silent-start incident was not reproduced; listening remains required. |
| Desktop access | Mouse capture prevented normal region screenshots and access to window controls. Q was mistaken for a location instead of a key. | Explicit pointer release, safe desktop pause, screenshot view, a real highlighted plate and validated crate outline. | CONTROLS-REVIEW-v0.8.md and MOUSE-LOOK-REVIEW-v0.10.md. Physical OS screenshot selection remains unverified by development tooling. |
| Navigation and interaction | E failed when near the core; circuit rules were unclear; archive instructions led to circling. | Forgiving visible-object interaction, persistent star contacts and examples, expandable labeled map and a collision-checked route. | WAYFINDING-REVIEW-v0.9.md. |
| Route visibility / placement | Route needed excessive looking down; core would not place at a machine worktop. Screenshot also displayed desktop pause. | Wider opaque route, raised markers, horizontal compass, R to level the view, E/Q socket insertion and clear pause instructions. | ROUTE-PLACEMENT-REVIEW-v0.11.md. Pause is established for that screenshot, not every reported failed input. |

## Friend's experience, reported through the user

The friend described limited experience with first-person computer games, difficulty discovering how to turn freely, moving mainly forward/back/sideways, and feeling 3D nausea. The friend also requested visible hover feedback for buttons and a simple map Back control rather than an awkward exit instruction.

The response was explicit-entry mouse look with a clear first turning lesson, visible hover/hand-cursor feedback, a map Back arrow, safe Esc/focus release and adjustable comfort controls. These changes are recorded in MOUSE-LOOK-REVIEW-v0.10.md. The report does not establish a diagnosis or show that the later revision resolves the friend's discomfort. No successful retest, complete duration or final story reaction was supplied.

## Next independent acceptance observation

For an actual first-time player, record the build, prior first-person experience, whether answers were known, start-to-delivery active/reading time, completion or abandonment, external help, first unclear step and retries. Ask the player to explain the shared-core rule and who the lantern was for in their own words. Record actual use of patrol prediction, distraction or crate cover rather than assuming mastery from completion. Stop a session if the player feels unwell.

Record each finding as observation -> revision -> retest. Engine tests demonstrate component behavior, not enjoyment, independent understanding or a 5-10-minute duration.
