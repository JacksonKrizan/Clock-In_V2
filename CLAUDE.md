# CLAUDE.md — Clock-In_V2

## Who's who
- **Jackson** — the owner (human). Files tickets, makes decisions.
- **Simon** — the working agent. **The assistant's name in this project is Simon** — sign all ticket annotations and attributions as `Simon`.
- **Timmy** — background subagents Simon deploys (parallel searches, review passes, queue maintenance). Attribute their ticket work as `Timmy`.

## Project
Unity 3D multiplayer educational game ("JobSim" / Clock-In): players experience real-world jobs through mini-games. Multiplayer via **Photon PUN 2**.

- `Assets/_Scripts/_GameImportant/` — player controller, networked player manager, menus, room/lobby
- `Assets/_Scripts/GamingMap/` — networking-education mini-game (packets, tiles, goals, timer) — **current focus** (branch `Jackson's_GameDevMap`)
- `Assets/_Scripts/Kitchen/`, `Assets/_Scripts/AutoMap/` — chef & auto-shop job mini-games
- `Assets/_Scenes/` — `_Menu`, `GameDev`, `AutoShop`, `Chef`, `TestField`

## Issue tracking — ADIOS_Tickets/ (Obsidian vault)
All work is tracked in `ADIOS_Tickets/` — plain-markdown, one file per issue, browsed by Jackson in Obsidian. **Read `ADIOS_Tickets/agents.md` before touching tickets**; `ADIOS_Tickets/README.md` is the full spec.

Quick rules:
- Subsystems: `app` (A-) game code · `data` (D-) assets/prefabs · `infra` (I-) editor/build/tooling · `other` (O-) catch-all · `security` (S-) Photon/auth/secrets.
- New ticket = copy `_TICKET_TEMPLATE.md`, next sequential ID, fill **every** frontmatter field, first annotation `- <date> (Simon): Created.`
- Annotations are append-only. `focus: true` on the 1–3 tickets actively being worked.
- **🚫 NEVER delete a ticket.** When done: `status: archived` + `resolution: resolved` + `archived_at: <date>` + `focus: false`, file stays in place. (Jackson's standing rule, 2026-06-03.)
- Jackson hands files over via `ADIOS_Tickets/_PNG/` (images) and `ADIOS_Tickets/_ETC/` (logs, docs).

## Unity logs — tail these to help with console errors
- **Editor log (live):** `/mnt/c/Users/jacks/AppData/Local/Unity/Editor/Editor.log` (previous session: `Editor-prev.log` next to it)
- Project `Logs/` dir holds shader-compiler and package logs; Unity recreates `Editor.log` files on restart, so use `tail -F` (capital F) to survive rotation.
- Compile errors look like `Assets\...\File.cs(line,col): error CSxxxx: ...` — when one appears, file/update a ticket in `ADIOS_Tickets/`.
- At session start (or when Jackson asks for log help), start a background tail:
  `tail -n 0 -F /mnt/c/Users/jacks/AppData/Local/Unity/Editor/Editor.log`

## Environment notes
- WSL2 on Windows; repo lives under OneDrive (`/mnt/c/Users/jacks/OneDrive/...`) — files can be touched by OneDrive sync and the Unity Editor while you work.
- Unity solution: `Clock-In_V2.sln`; scripts compile via the Unity Editor (no standalone build command in WSL).
