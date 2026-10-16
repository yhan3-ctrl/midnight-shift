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

## October 16 / Hush and teaching scope

The user reported both desktop pause and an unexpected archive safety retry, confusion about waiting at the chime and Hush's friendly role, and too much apparent teaching. Archive practice now cannot cause a reset; chime retrieval does not cancel its distraction, and the first-door lesson has an explicit ending. The real safety challenge starts in Assembly with its own explanation. The user also requested more Halloween decorations. See HUSH-REVIEW-v0.13.md for implementation and executed checks. No post-fix independent player response has yet been recorded.

## October 16 / route motion, task purpose, craft feedback and blocked core

The user reported the gold route jumping sideways while walking, unclear reasons for moving the crate/core between tasks, insufficient success/start/production feedback, and an insertion instruction conflicting with a hands-full message. The supplied photograph shows a held core, crate in front and a different console goal. v0.14 retains valid routes in world space, shows Why on task cards, adds circuit/start/work feedback and actual progress UI, and replaces the second-pickup conflict with safe console-access guidance. See FEEDBACK-REVIEW-v0.14.md. This is actual reported feedback; no post-fix human comfort or comprehension result is asserted.

## v0.15 / tutorial boundary, character clearance and Halloween interface

Direct user photos identified a helper intersecting the foyer desk, unclear tutorial/formal separation, a date mistaken for progress, weak placement targets and a generic title. Moved helpers clear of desks; choreographed a separated exit with an exterior camera; added an acknowledged tutorial completion card and persistent formal phase; replaced the date with labeled task progress; illuminated the actual target plate before pickup; added a decorative title face and paper/pumpkin/bat UI while retaining readable instructions. Executed checks: 222 engine integration cases, complete deterministic physical traversal, 196 seven-language staged screens, actual animated ending captures, and installed/extracted package validation. See INTERFACE-REVIEW-v0.15.md and PlaytestEvidence/v0.15 for scope and portable evidence. No new independent first-play duration or guaranteed grade is claimed.

## October 16 confirmed completion times

Yu Han reported three completed ordinary play sessions: their first run took approximately 20 minutes; one friend completed a subsequently updated build in approximately 13 minutes; another friend completed the latest v0.15 in approximately 10 minutes. Yu Han explicitly confirmed the last build number. These are participant-reported approximate durations, not stopwatch logs or averages. Exact earlier build numbers, pause/reading breakdowns, prior familiarity and assistance were not supplied. Different people played after different updates, so the 20-to-13-to-10 sequence cannot isolate a causal effect. The latest report supports a completion at the upper edge of the rubric's 5-10-minute range; it does not prove every player's duration or enjoyment.
