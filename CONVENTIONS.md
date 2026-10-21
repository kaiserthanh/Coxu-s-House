# Conventions

For people and coding agents working in this repo.

## Folders

- Everything of ours lives under `Assets/_Project/`. Third-party assets go in their own top-level `Assets/` folder.
- Scripts by area: `Net`, `Player`, `Interaction`, `Story`, `World`, `UI`. Tests in `Tests/EditMode`.

## C#

- One public class per file, file name = class name. Namespace `CoxusHouse.<Area>` (for example `CoxusHouse.Story`).
- `PascalCase` for types, methods and properties; `_camelCase` for private fields; `[SerializeField] private` instead of public fields.
- Keep gameplay rules in plain C# classes where possible so they can be unit tested; MonoBehaviours and NetworkBehaviours wire them up.
- Networked state changes only on the host. Clients call a `[Rpc(SendTo.Server)]` method and let the host decide.
- Methods that run only on the host end in `Server` (for example `OnInteractServer`).

## Working with AI

- One ticket at a time, with its "done when" test from `TASKS.md`.
- Read `ARCHITECTURE.md` before adding a system. Prefer extending an existing one.
- No large generated files pasted in blind: review, run, test with two players, then commit.

## Git

- Small commits that each leave the project opening and playing.
- Commit `.meta` files with their assets. Never commit `Library/`, `Temp/`, `Logs/` or `UserSettings/`.
- Binary assets go through Git LFS (see `.gitattributes`).
