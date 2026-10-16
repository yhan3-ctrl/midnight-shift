# Hush clarity and Halloween presentation / v0.13

Review date: October 16, 2026 (America/Chicago).

## Reported problem

The user reported being told to wait for Hush to finish resting and observe his route while trying the chime, then being sent back while carrying the archive crate. The user experienced both desktop pause and an inspector retry, and asked whether Hush was a villain and whether too much content was being presented as teaching. The supplied screenshot shows desktop pause; it does not disprove the separately reported retry.

## Cause and revision

Previously the first chime activation enabled archive detection after only a short grace period. This contradicted the friendly introduction and punished the player's first attempt at the crate task. The revised archive is safe practice throughout: Hush can patrol and investigate, but cannot pursue, teleport the player, drop the held item or interrupt with a retry dialogue. The chime invites him away for 16 game seconds; removing its core immediately does not cancel that invitation. Waiting for him to rest or finish a route is never required. The E prompt explicitly says Ring chime rather than Open.

The opening lesson covers look, walk, pick up, place and the first door. A non-blocking message announces the formal shift upon leaving the entrance. Packaging and Archive are formal chapters that introduce tools when reached, rather than a separate mandatory tutorial covering every later rule. An approximately one-minute opening is a design target, not an independently measured result. The required overall 5–10-minute experience remains a separate first-play acceptance check.

Final Assembly retains the combined safety challenge, explained in a paused introduction before activation. Only there can sustained visibility fill a warning and subsequent contact cause a local retry. Completed jobs and gate-holding crates survive. Hush remains a friendly inspector in the story; the consequence is a safety check, with no injury, combat or death.

Halloween decorations now include perimeter pumpkin lamps, paper bats, hanging paper ghosts, treat buckets and a Happy Halloween banner. They have no colliders, interactions or collection requirements, use the existing palette and leave the central signs, pressure plates and worktops clear.

## Verification

170 engine integration checks passed on the final source, including a forced stale Approach state and the real archive Update leaving the player safe, the archive rejecting a direct reset call, core retrieval preserving Hush's visit, renewed ringing, explicit Ring chime text, retained assembly retries and collision-free decorations. A full deterministic newcomer-profile route reached actual delivery (20 interactions, four validated placements, four authored circuits), with live geometry and inspector locomotion disabled to isolate navigation. It preceded the final prompt/repeated-ring refinements; those refinements received the final engine checks. The final Mac application rendered 175 staged screens across seven languages, including the safe archive and immediate core retrieval, with seven pagination continuations and no layout-overflow or standalone runtime-exception entries. Installed and ZIP-extracted builds passed signature and startup checks; ZIP integrity passed.

Portable evidence: [PlaytestEvidence/v0.13](PlaytestEvidence/v0.13/README.md). Archive: `Builds/MidnightShift-Mac-v0.13.zip`. SHA-256: `04103d1864cde14dda5d6cd75c1d9ac8bf4053bf8c938b774172a0cccde0078e`. These checks use authored solutions and staged states where indicated; they do not constitute independent human comprehension, comfort or timing evidence. The v0.12 complete native session and five scenario lenses remain documented separately in FINAL-GAME-PLAYTEST.md, not relabeled as v0.13 runs.
