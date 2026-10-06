# Changelog

All notable changes to this project are documented here. Versions follow [Semantic Versioning](https://semver.org/).

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
