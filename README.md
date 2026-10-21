# Coxu's House

A small first-person co-op comedy-horror game for a group of 4 to 6 friends. Episode 1 is *The Birthday Incident*. The current target is milestone **M0: The Blackout**, a two-player networked graybox (see `TASKS.md`).

Private project. Do not share builds or house details publicly without Coxu's permission.

## Stack

- Unity 6.3 LTS (editor `6000.3.x`), Universal Render Pipeline
- Netcode for GameObjects 2.13.3 (pulls in Unity Transport)
- Input System 1.20.1
- Multiplayer Play Mode 2.0.2 for testing several players in one editor

Package versions live in `Packages/manifest.json`. Check them against the Package Manager when you first open the project.

## First-time setup (T1)

This repo starts with the packages, folders and docs but no generated Unity settings. Do this once:

1. Install Git LFS (`git lfs install`), then clone the repo.
2. In Unity Hub, install the current **Unity 6.3 LTS** editor if you don't have it.
3. Unity Hub > **Add** > **Add project from disk** > pick the cloned folder. Open it with 6.3 LTS. Unity creates `ProjectSettings/` and downloads packages. If the Package Manager reports a version it can't find, accept the version it suggests.
4. If Unity asks to enable the new Input System backend and restart, say **Yes**.
5. Set up URP:
   - Project window: right-click `Assets/_Project` > Create > Folder `Settings`.
   - Inside it: right-click > Create > Rendering > **URP Asset (with Universal Renderer)**.
   - Edit > Project Settings > **Graphics**: set Default Render Pipeline to that asset.
   - Edit > Project Settings > **Quality**: set Render Pipeline to the same asset for every quality level.
6. Edit > Project Settings > **Editor**: check Asset Serialization is **Force Text** (Unity 6 default).
7. Create two empty scenes in `Assets/_Project/Scenes`: `Boot` and `Graybox_House`. Add both to File > Build Profiles (Boot first).
8. Commit everything Unity generated (`Assets/`, `Packages/`, `ProjectSettings/`), including `ProjectSettings/ProjectVersion.txt` so everyone uses the same editor.

**T1 is done when** a fresh clone opens in 6.3 LTS without errors and plays the empty `Graybox_House` scene.

## Docs

- `GAME_DESIGN.md`: locked design decisions and the M0 scope
- `ARCHITECTURE.md`: who owns what on the network, and the M0 systems
- `TASKS.md`: M0 tickets in build order
- `CONVENTIONS.md`: code and folder rules, for people and coding agents

The full design brief (v0.1) and the GDD v0.2 doc are the source of truth; these files are the working cut.
