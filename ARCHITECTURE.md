# Architecture

Rule: **the host decides, clients ask, and every client plays its own effects when shared state changes.**

## Authority

| Thing | Owner | Sync |
| --- | --- | --- |
| Player position and look | That player | Owner-authoritative network transform |
| Story phase | Host | `NetworkVariable<StoryPhase>` on `StoryDirector` (late joiners get it automatically) |
| Doors, breaker switches, pizza | Host | Client sends an interact RPC; host validates distance and phase, then sets a `NetworkVariable` |
| Carried item (fuse) | Host | Host sets the holder's client id; clients render it in that player's hand |
| Lights, sounds, screen shake | Each client | Local reaction to phase changes. Never networked individually |
| Front-door loop teleport | Each client, for its own player | Players own their movement, so they move themselves |

Use the NGO 2.x universal RPC style: `[Rpc(SendTo.Server)]` and `[Rpc(SendTo.ClientsAndHost)]`. Do not use the older `[ServerRpc]` / `[ClientRpc]` attributes.

## Story phases (M0)

`Party -> Blackout -> RestorePower -> Escape -> End`, and `End -> Party` on host restart.

Transition rules live in a plain C# class (`StoryRules`) with no Unity or network dependencies, so they are covered by EditMode tests. `StoryDirector` (a NetworkBehaviour) only asks `StoryRules` and writes the result to the NetworkVariable.

## M0 systems

| Script | Folder | Job |
| --- | --- | --- |
| `NetworkBootstrap` | Net | Host / Join buttons, IP field, player spawning |
| `PlayerController` | Player | CharacterController movement and camera; enabled only for the owner |
| `PlayerInteractor` | Interaction | Camera raycast, prompt, sends interact RPC |
| `Interactable` | Interaction | Base NetworkBehaviour: `CanInteract(player)`, `OnInteractServer(player)` |
| `StoryDirector` + `StoryRules` | Story | Host-only phase machine |
| `HouseState` | World | Reacts to phase: swaps Normal/Other room roots, light sets, door destinations |
| `ObjectiveUI` | UI | Shows objective text for the current phase |

New props are subclasses of `Interactable`, not new systems. No dependency injection, event buses or assembly definitions until something needs them.
