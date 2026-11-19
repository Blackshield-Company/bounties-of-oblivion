# Changelog

All notable changes to this project are documented here. Versions follow [Semantic Versioning](https://semver.org/).

## [0.2.0] - Unreleased (local review build)

Not yet played in game. Built and checked by the automated build only.

### Added
- **The King of Worms.** Mannimarco, the lich who would steal Arkay's dominion over death, is a rival and not a prince. He has no favor, blessing or gift track and never offers tasks.
  - **Menace** (`BOO_WormMenace`, 0 to 100): every finished prince task adds `BOO_WormGain` (15), or `BOO_WormDeathGain` (25) for Namira and Molag Bal. When it reaches `BOO_WormThreshold` (50), a message says a worm-eaten page has appeared and he has sent a thrall. The contract-satisfied box notes when the menace crosses into a new stage.
  - **The thrall hunt** (`BOO_WormHunt`): at 50+ menace, with no task running, the Ledger menu shows *The King of Worms*. The hunt is a radiant kill task near a dungeon against a necromancer boss (Thrall of the King of Worms, Worm Acolyte or Hand of Mannimarco). It counts as the one active task. The reward is the usual task gold, menace reset to 0 and one more thrall slain, with no favor changes. Abandoning costs no favor and leaves the menace unchanged.
  - **Gnawed pages**: while menace is at 100, prince tasks give half favor (rounded down) and the payout box says so.
  - **Amulet of the Worm's Bane**: a one-time gift for the third thrall slain (Restoration +15%, vanilla Amulet of Arkay worn model).
  - *Favor of the princes* ends with the menace, its stage (Unnoticed, Watching, Hunting, Gnawing) and the number of thralls slain.
- Console tunables `BOO_WormGain`, `BOO_WormDeathGain` and `BOO_WormThreshold`.

### Notes
- Namira was already a full prince before 0.2.0. She was checked end to end and is unchanged, except that her tasks now feed the King of Worms.
- Save-compatible with 0.1.3: existing FormIDs and the 8-long prince arrays are unchanged. All new state is in new globals, so nothing depends on `OnInit` rerunning.

## [0.1.3] - 2026-10-25

Still early beta. Built and checked, but not yet played in game (no Skyrim install is available to the build).

### Fixed
- The "Favor of the princes" box, the contract-satisfied box and the blessing box printed a literal backslash-n instead of starting a new line. Papyrus text now takes its line breaks from a real line-feed character carried by a `LineBreak` property that the plugin fills, and the build fails if any Papyrus string literal or compiled string constant contains a backslash escape. Save-compatible: no scripts, properties or records were renamed, and FormIDs are unchanged.

## [0.1.2] - 2026-10-10

Still early beta, not yet playtested in game.

### Fixed
- Wrye Bash could not load the plugin (IndexError "list index out of range"). The first stage's quest log entry was written without its QSDT (log entry flags) subrecord, which every log entry must start with. Every log entry now has one. No gameplay changes.

## [0.1.1] - 2026-10-10

Still early beta, not yet playtested in game.

### Changed
- Namira's boon is now the **Necklace of the Hollow** (amulet slot, vanilla silver necklace model) instead of the Ring of the Hollow, so it no longer clashes with the vanilla Ring of Namira. Same effect (Sneak +15%), same item record, so a copy you already own just becomes a necklace. If it was equipped as a ring, unequip and re-equip it.

## [0.1.0] - 2026-10-06

Early beta. First public release. It passes every automated check in the build (Papyrus compile, plugin validation, round trip, FOMOD schema) but has not been playtested in-game yet. Please report problems on the Nexus page or on GitHub.

### Added
- The Daedric Ledger: arrives after you sleep at level 5 or higher (or after a day of play if you never sleep). Sign it to gain the lesser power "Consult the Daedric Ledger".
- Eight repeatable radiant tasks, one per prince, on vanilla locations:
  - Mehrunes Dagon: kill the leader of a bandit or Forsworn camp.
  - Boethiah: defeat a named champion near a dungeon.
  - Namira: clear a draugr crypt.
  - Molag Bal: destroy the master of a vampire lair.
  - Clavicus Vile: recover the Gilded Wager from a Dwemer ruin.
  - Hircine: hunt a named beast near an animal den or giant camp.
  - Sanguine: drink three bottles of Revel Wine at a chosen inn.
  - Vaermina: retrieve the Shard of Nightmare from a warlock lair or hagraven nest.
- Favor per prince (0 to 100), tracked in globals. +20 per task and gold scaled to your level.
- Rivalries: Dagon/Boethiah, Namira/Clavicus Vile, Molag Bal/Hircine, Sanguine/Vaermina. Every task costs 10 favor with the rival.
- Blessings at 30 favor (lost below 30) and a one-time enchanted jewelry gift at 80 favor, for each prince.
- FOMOD installer with optional Papyrus sources.
