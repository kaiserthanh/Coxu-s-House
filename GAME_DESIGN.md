# Game Design (working cut of GDD v0.2)

## Locked

| Area | Decision |
| --- | --- |
| Players | Usually 4 to 6 friends |
| Setting | A fictionalized recreation of Coxu's real house |
| Tone | Comedy horror: real tension, ridiculous situations |
| Mode | Structured co-op story with objectives. No sandbox, no PvP minigames |
| Structure | Scripted story beats, free exploration between them, progress driven by player actions |
| Episode 1 | Friends arrive for Coxu's birthday, the house comes alive, they escape a monstrous version of it |
| Signature reference | Coxu's dad's "radioactive" pizza. Its exact role is open |
| Presentation | Standalone 3D first-person, closer to R.E.P.O. / Lethal Company than Roblox |
| Engine and network | Unity 6 (currently 6.6), URP, Netcode for GameObjects 2.x, Unity Transport. LAN first, Unity Relay after M0 |
| Authority | Host owns story phase, objectives, doors, items, enemies. Each player owns their own movement |
| Incident trigger | A player interacting with the pizza; the house triggers it anyway after about 6 minutes |

## Recommended, not locked yet

- Challenge mix: objectives first, chases second, stun-only melee third. No guns.
- The pizza is the first real threat; the final boss is open.
- Downed, not dead: teammates revive. Everyone down restarts the current phase.
- One story path for Episode 1.
- Private friends-only builds.

## Lore rule

Only two real references are confirmed: Coxu's house and the pizza. Everything else is placeholder until the developer adds it to a Lore Bible. Never invent "real" group history.

## M0: The Blackout

Graybox, two players, about 5 minutes. Four placeholder spaces: living room (spawn, pizza), hallway, utility room (breaker), front door.

1. **Party**: explore, open doors. Objective "Hang out."
2. **Blackout**: a player presses E on the pizza. All lights go out for everyone, the front door loops back into the hallway. Objective "Find the fuse."
3. **RestorePower**: carry the fuse (one of two spawn spots) to the breaker. Two switches about 4 m apart must be held together for 2 seconds.
4. **Escape**: lights return in a wrong color; the front door now leads outside. Objective "Get out."
5. **End**: all players in the exit zone see "You escaped" and the run time. Host can restart to Party.

Out of scope for M0: pizza AI, combat, downed state, Relay, art, menus beyond Host and Join.
