# Tasks: M0 The Blackout

Build in order. Each ticket ends with a commit and a quick test; networked tickets are tested with two players (Multiplayer Play Mode, then a real build plus the editor).

- [ ] **T1 Project setup**: Unity 6.3 LTS project, URP, packages, Git LFS. *Done when* a fresh clone opens and plays the empty `Graybox_House` scene. (Steps in `README.md`.)
- [ ] **T2 Local movement**: first-person walk, look, sprint. *Done when* you can cross every room without snagging.
- [ ] **T3 Local interaction**: raycast prompt, a door that opens and closes. *Done when* the prompt shows only within 2 m.
- [ ] **T4 Host and Join**: bootstrap UI, player spawning. *Done when* a build and the editor connect on one PC and see each other move.
- [ ] **T5 Networked door**: `Interactable` base class, host-checked door. *Done when* either player opens it and both see the same state, including a late joiner.
- [ ] **T6 Story Director**: phase machine plus EditMode tests for every transition. *Done when* tests pass and the pizza moves Party to Blackout on both screens.
- [ ] **T7 House State**: lights, door destinations and the looping front door react to the phase. *Done when* both players see the blackout within the same second and both loop back through the front door.
- [ ] **T8 Fuse and breaker**: carryable fuse, two-switch breaker. *Done when* one player alone cannot finish it and two players can.
- [ ] **T9 Escape and restart**: exit zone, end screen, host restart. *Done when* three full runs in a row work without restarting the app.

## M0 acceptance test (two separate PCs)

1. Host starts on PC A, client joins from PC B by IP.
2. Each sees the other move and turn smoothly.
3. Client opens a door; host sees it. Host closes it; client sees it.
4. Client touches the pizza. Both black out together and show "Find the fuse."
5. Both walk out the front door and arrive back in the hallway.
6. Host picks up the fuse; client sees it in the host's hand. Host inserts it.
7. One player alone on a switch: nothing. Both hold: lights return in the wrong color.
8. Both reach the exit and see "You escaped" with the same time.
9. Host restarts; both are back at Party with the pizza untouched.
10. Client disconnects mid-run: the host keeps running without errors.
