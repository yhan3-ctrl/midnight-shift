# Finite State Machines / implemented baseline

These diagrams describe current states and transitions. They are design documentation, not engine-generated verification. Parameters and exceptions are listed in GameScript.md.

## Player modes

```mermaid
stateDiagram-v2
    [*] --> Title
    Title --> Active: Begin
    state Active {
        [*] --> EmptyHands
        EmptyHands --> Carrying: valid pickup
        Carrying --> EmptyHands: valid place / insert / deliver
    }
    Active --> Routing: powered production machine
    Routing --> Active: close / remove core
    Routing --> Active: connected route accepted, machine starts
    Active --> Reading: story interaction or job dialogue
    Reading --> Active: final line advanced
    Active --> Journal: J
    Journal --> Active: J / Esc / Close
    Active --> Paused: Esc
    Paused --> Active: Esc / Resume
    Active --> SafetyReset: inspector contact
    SafetyReset --> Active: restore safe position and recover held item
    Active --> Ending: deliver completed lantern to Pip
    Paused --> Title: restart scene
    Ending --> Title: restart scene
```

Movement can coexist with both carrying states. Reading, Journal, Routing and Paused retain the previous carrying state and freeze world simulation. A safety reset returns held items to known home positions and preserves completed jobs. There is no player death state.

## Inspector

```mermaid
stateDiagram-v2
    [*] --> Patrol
    Patrol --> Investigate: valid machine-start sound
    Return --> Investigate: valid machine-start sound
    Search --> Investigate: valid machine-start sound
    Investigate --> Return: arrive and wait 5 seconds
    Patrol --> Alert: player visible
    Investigate --> Alert: player visible
    Return --> Alert: player visible
    Search --> Alert: player visible
    Alert --> Approach: visibility fills alert over 0.95 seconds
    Alert --> Search: lose line of sight
    Approach --> Search: lose line of sight
    Search --> Return: 4 seconds elapsed
    Return --> Patrol: reach patrol waypoint
    Approach --> Patrol: contact causes safety reset
```

A new machine sound cannot override Alert or Approach. Visibility requires range, field of view, region bounds and an unblocked ray. A crate is occlusion only when placed, since held objects have colliders disabled.

## Machine

```mermaid
stateDiagram-v2
    [*] --> Unpowered
    Unpowered --> Ready: insert core
    Ready --> Running: E and prerequisite satisfied
    Running --> Unpowered: E removes core
    Running --> Complete: work target reached
    Complete --> Complete: output persists
    Complete --> UnpoweredComplete: remove core
    UnpoweredComplete --> Complete: reinsert core
```

Partial work survives power removal. The workshop relay is a continuous power source for its gate, not a completed-work flag. A manual crate plate can take over while its core is reused elsewhere.

## Helper presentation (v0.4)

Button and Pip: Idle → WelcomeWave on first nearby active encounter; Idle/WelcomeWave → Celebrate on manufacturing success or optional Button interaction; animations return to Idle after their timers. Body bob, sleeve gestures and gaze use the same paused world clock. When Ended becomes true, helper relocation and gaze stop and EndOfShift controls the group departure. This helper presentation does not replace Hush's patrol FSM.
