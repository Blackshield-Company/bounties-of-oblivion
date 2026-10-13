# Daedric Ledger: Bounties of Oblivion

A Skyrim Special Edition mod. The Daedric Princes have work for you. Take radiant tasks from eight princes, earn favor with each one, and play them against each other.

**Status: v0.1.2, early beta.** It passes every automated check in the build (Papyrus compile, plugin validation, byte-exact round trip, FOMOD schema) but has not been playtested in-game yet. Expect rough edges and please report what you find. See [docs/TESTING.md](docs/TESTING.md).

- Game: Skyrim Special Edition / Anniversary Edition 1.6.x (also fine on 1.5.97). Works on Windows and on Linux through Proton.
- Requirements: none beyond the base game. No SKSE, no SkyUI, no DLC, no Creation Club content.
- Plugin: `BountiesOfOblivion.esp`, ESL-flagged, so it doesn't use a full load-order slot. Skyrim.esm is the only master.
- Compatible with [Blackshield Company](https://github.com/synthalorian/blackshield-company): separate records, globals and prefixes (`BOO_` vs `BSC_`), and both use the vanilla radiant system so they never send you to the same reserved location at once.

## What it does

| Feature | How it works |
|---|---|
| The Ledger | Once you're level 5 and past the Helgen intro, a black book appears beside you after you sleep (or after a day of play if you never sleep). Read it and sign to get the lesser power **Consult the Daedric Ledger**. You can also cast the book away for good. |
| Tasks | Pick a prince in the Ledger and accept their task. One task at a time. Locations are picked by the game's radiant system from vanilla locations inside Skyrim's holds. |
| Mehrunes Dagon | Kill the leader of a bandit or Forsworn camp. |
| Boethiah | Defeat a named champion waiting near a dungeon. |
| Namira | Clear a draugr crypt. |
| Molag Bal | Destroy the master of a vampire lair. |
| Clavicus Vile | Recover the Gilded Wager from the boss chest of a Dwemer ruin. |
| Hircine | Hunt a named bear or sabre cat near an animal den or giant camp. |
| Sanguine | Drink three bottles of Sanguine's Revel Wine (he sends four) inside a chosen inn. |
| Vaermina | Retrieve the Shard of Nightmare from the boss chest of a warlock lair or hagraven nest. |
| Favor | 0 to 100 per prince, stored in globals. +20 per task, plus `150 + 10 × level` gold. Abandoning a task costs 10. |
| Rivalries | Dagon/Boethiah, Namira/Clavicus Vile, Molag Bal/Hircine, Sanguine/Vaermina. Each task costs 10 favor with the rival. |
| Blessings | At 30 favor a prince grants an ability (for example *Blessing of Boethiah*: Health +20). It's removed if favor falls below 30. |
| Gifts | At 80 favor a prince sends one enchanted ring or amulet (for example *Ember Torc of Dagon*: Destruction +12%). Once per prince. |

Everything reuses vanilla assets: jewelry, gem, gyro and wine-bottle meshes, vanilla magic effects, and leveled bandit, bear and sabre cat templates. There is no new voice acting. The princes speak through the Ledger and message boxes.

## Install

Grab `BountiesOfOblivion-<version>.zip` from [Releases](https://github.com/synthalorian/bounties-of-oblivion/releases) (it's a FOMOD installer).

- **Mod Organizer 2**: *Install a new mod from an archive*, pick the zip, then enable `BountiesOfOblivion.esp` in the right pane.
- **Vortex**: drag the zip into the Mods page, install and enable it.
- **Manual (Linux/Proton, no mod manager)**: copy everything inside `00 Core/` from the zip into `.../steamapps/common/Skyrim Special Edition/Data/`. Then add the line `*BountiesOfOblivion.esp` to
  `.../steamapps/compatdata/489830/pfx/drive_c/users/steamuser/AppData/Local/Skyrim Special Edition/Plugins.txt`.

It's safe to add to an existing save. Uninstalling mid-playthrough is not recommended, as with any scripted mod.

## Console tunables

```
set BOO_IntroMinLevel to 5    (level before the Ledger can arrive)
set BOO_PayMult to 1.0        (multiplier on task gold)
set BOO_FavorGain to 20       (favor per completed task)
set BOO_RivalLoss to 10       (favor the rival loses per task)
```

## Build from source

The release zip is built entirely from this repo by `./build.sh` on Linux:

1. **Papyrus**: `src/papyrus/*.psc` are compiled to `.pex` with the open-source [papyrus-compiler](https://github.com/russo-2025/papyrus-compiler) (MIT, native Linux). The build downloads it and verifies its checksum. It type-checks against clean-room API declarations in `tools/modkit/papyrus-stubs`.
2. **Plugin**: `src/plugin/BountiesOfOblivion.Generator` is a C# program that uses [Mutagen](https://github.com/Mutagen-Modding/Mutagen) to write `BountiesOfOblivion.esp` from code. It then re-reads the plugin and validates it: ESL range, masters, dangling links, every script property against the `.psc` sources, quest text tags against aliases and text globals, and a byte-exact round trip.
3. **FOMOD**: `tools/modkit/fomod/pack.py` validates `fomod/ModuleConfig.xml` against the FOMOD schema and writes a reproducible zip to `dist/`.

Requirements: .NET SDK 9 or newer, python3, curl, and optionally `xmllint` (libxml2) for the schema check. No Creation Kit or Wine needed.

```bash
./build.sh
# Optional: type-check the scripts against your game's own sources
# (unpack Data/Scripts.zip from your install first):
./build.sh -H "/path/to/Skyrim Special Edition/Data/Source/Scripts"
```

`src/plugin/formids.json` pins every record's FormID. Never edit or reuse entries, or existing saves will break.

## Layout

```
src/papyrus/                 Papyrus sources (.psc)
src/plugin/                  Mutagen plugin generator + pinned FormIDs
fomod/                       FOMOD installer config (info.xml, ModuleConfig.xml)
docs/                        Testing checklist, design notes, Nexus page and upload fields
tools/modkit/                Shared tooling for the mod series (copied from Blackshield Company, extended)
build.sh                     One-command build
```

## Support

Donations are optional. Nothing in the mod is gated on them.

- GitHub Sponsors: https://github.com/sponsors/synthalorian
- Ko-fi: https://ko-fi.com/synthalorian

## Credits and licenses

- Code in this repo: MIT (see `LICENSE`).
- [Mutagen](https://github.com/Mutagen-Modding/Mutagen) by Noggog and contributors (GPL-3.0). It's used as a build tool and isn't redistributed.
- [papyrus-compiler](https://github.com/russo-2025/papyrus-compiler) by russo-2025 (MIT). It's downloaded at build time.
- FOMOD schema from [GandaG/fomod-schema](https://github.com/GandaG/fomod-schema) (MIT).
- Skyrim and its assets belong to Bethesda Softworks. This repo contains no Bethesda assets or script sources. The plugin only references vanilla records by FormID and vanilla meshes by path.
