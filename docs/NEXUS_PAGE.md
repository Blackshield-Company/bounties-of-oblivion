# Nexus Mods page: Daedric Ledger: Bounties of Oblivion

Early beta: v0.1.1 passed every automated build check but hasn't been playtested in-game. The page says so up front. Exact upload-form values are in [NEXUS_FIELDS.md](NEXUS_FIELDS.md).

## Page settings

- **Game:** Skyrim Special Edition
- **Mod name:** Daedric Ledger - Bounties of Oblivion
- **Version:** 0.1.1 (early beta)
- **Category:** Quests and Adventures
- **Suggested tags:** Quests, Lore-Friendly, Daedra, Gameplay, Radiant, ESL, No SKSE, Scripted
- **Requirements:** none.
- **Permissions (suggested):** source on GitHub under MIT, so allow modification and conversions with credit. Don't allow re-uploads of the archive itself.
- **File to upload:** `BountiesOfOblivion-0.1.1.zip` (FOMOD), main file, named "Bounties of Oblivion".

## Description (paste into the BBCode editor)

```bbcode
[center][size=6][b]DAEDRIC LEDGER: BOUNTIES OF OBLIVION[/b][/size]
[size=3][i]Eight seals. One empty line. Sign it, and the Princes of Oblivion will give you work.[/i][/size]

[size=3][color=#e0a030][b]EARLY BETA (v0.1.1)[/b]: this version passes every automated build check but hasn't been playtested in-game yet. Please report bugs in the Bugs tab or on GitHub, ideally with your Papyrus log.[/color][/size][/center]

[line]

[size=4][b]Summary[/b][/size]
A black book appears beside you as you wake: [b]the Daedric Ledger[/b]. Sign it, and eight Daedric Princes start writing tasks for you, sending you to vanilla locations all over Skyrim. Every task earns gold and [b]favor[/b] with that prince, and costs favor with their [b]rival[/b]. Earn enough, and a prince lends you a [b]blessing[/b], then sends you a [b]gift[/b] of their own.

[size=4][b]Features[/b][/size]
[list]
[*][b]Non-invasive delivery.[/b] No new NPCs in towns, no cell edits. The Ledger arrives after you sleep at level 5+ (alternate-start friendly). Not interested? Cast it away for good.
[*][b]A lesser power[/b], Consult the Daedric Ledger, opens the menu anywhere.
[*][b]Eight repeatable radiant tasks[/b], one per prince:
[list]
[*][b]Mehrunes Dagon[/b]: kill the leader of a bandit or Forsworn camp.
[*][b]Boethiah[/b]: defeat a named champion waiting near a ruin or cave.
[*][b]Namira[/b]: clear a draugr crypt.
[*][b]Molag Bal[/b]: destroy the master of a vampire lair.
[*][b]Clavicus Vile[/b]: recover his lost wager from a Dwemer ruin.
[*][b]Hircine[/b]: hunt a named beast near an animal den or giant camp.
[*][b]Sanguine[/b]: drink three bottles of his wine at the inn he picks.
[*][b]Vaermina[/b]: take back a shard of nightmare from a witch's lair.
[/list]
[*][b]Favor per prince[/b], 0 to 100. +20 per task, plus gold that scales with your level.
[*][b]Rivalries[/b]: Dagon and Boethiah, Namira and Clavicus Vile, Molag Bal and Hircine, Sanguine and Vaermina. Every task costs 10 favor with the rival.
[*][b]Blessings at 30 favor[/b]: one ability per prince, lost if favor drops below 30.
[*][b]Gifts at 80 favor[/b]: an enchanted ring or amulet from each prince.
[*]All vanilla assets. No new meshes or textures, so retextures apply.
[/list]

[size=4][b]Blessings and gifts[/b][/size]
[list]
[*]Mehrunes Dagon: Resist Fire +15% / [i]Ember Torc of Dagon[/i] (Destruction +12%)
[*]Boethiah: Health +20 / [i]Oathbreaker's Band[/i] (One-Handed +15%)
[*]Namira: Resist Disease +50%, Resist Poison +15% / [i]Necklace of the Hollow[/i] (Sneak +15%)
[*]Molag Bal: Resist Frost +15% / [i]Chain of Coldharbour[/i] (Two-Handed +15%)
[*]Clavicus Vile: Carry Weight +25 / [i]Band of the Wager[/i] (Speech +15%)
[*]Hircine: Stamina +25 / [i]Fang of the Hunt[/i] (Archery +15%)
[*]Sanguine: Stamina regenerates 25% faster / [i]Sanguine's Merry Band[/i] (Health regenerates 25% faster)
[*]Vaermina: Magicka regenerates 25% faster / [i]Dreamstone of Vaermina[/i] (Illusion +15%)
[/list]

[size=4][b]Requirements[/b][/size]
[list]
[*]Skyrim Special Edition or Anniversary Edition (1.5.97 or 1.6.x). Works on Steam under Proton on Linux.
[*]Nothing else. No SKSE, no SkyUI, no DLC, no Creation Club content.
[/list]

[size=4][b]Installation[/b][/size]
Install with [b]Mod Organizer 2[/b] or [b]Vortex[/b] (FOMOD installer) and enable [b]BountiesOfOblivion.esp[/b]. It's ESL-flagged, so it doesn't take a full load-order slot. Safe to add mid-playthrough.

[size=4][b]Compatibility[/b][/size]
[list]
[*]No edits to vanilla records, cells or navmesh. Everything is new records. Skyrim.esm is the only master.
[*]Uses the vanilla radiant system to pick locations, so other radiant quests won't send you to a location a task has reserved, and the reverse.
[*]Fully compatible with [b]Blackshield Company[/b] (same author). Both can run at the same time.
[*]Vanilla Daedric quests are untouched. The Ledger's tasks are separate and repeatable.
[*]Alternate-start mods: supported.
[/list]

[size=4][b]Uninstalling[/b][/size]
As with any scripted mod, don't remove it mid-playthrough. If you must, abandon any active task first.

[size=4][b]Known issues / early beta limits[/b][/size]
[list]
[*]Not playtested in-game yet. Found something? Please post it in the Bugs tab.
[*]No voice acting. The princes speak through the Ledger and message boxes.
[*]If every eligible location for a prince is reserved or cleared, the page stays blank. Try another prince or come back later.
[*]Settings are console globals for now: [code]set BOO_PayMult to 1.0 / set BOO_FavorGain to 20 / set BOO_RivalLoss to 10 / set BOO_IntroMinLevel to 5[/code]
[/list]

[size=4][b]Roadmap[/b][/size]
More task types per prince, the other Daedric Princes, prince-specific penalties at zero favor, and an MCM.

[size=4][b]Source[/b][/size]
Open source (MIT): [url=https://github.com/synthalorian/bounties-of-oblivion]github.com/synthalorian/bounties-of-oblivion[/url]

[size=4][b]Credits[/b][/size]
[list]
[*][url=https://github.com/Mutagen-Modding/Mutagen]Mutagen[/url] by Noggog and contributors, which generates the plugin from code.
[*][url=https://github.com/russo-2025/papyrus-compiler]papyrus-compiler[/url] by russo-2025, the open-source Papyrus compiler.
[*][url=https://github.com/GandaG/fomod-schema]FOMOD schema[/url] by GandaG.
[*]Bethesda Game Studios for Skyrim.
[/list]
```

## Screenshots to capture in-game

Take these at 16:9, with the HUD hidden for scenery shots (`tm` in the console toggles menus).

1. **Hero shot.** Your character wearing a couple of the gifts, at a Daedric shrine or at night. This goes on the mod tile.
2. **Arrival.** The "You wake with ash on your fingertips…" message beside a bed.
3. **The Ledger.** The open book on the Terms or Rivalries page.
4. **The pact.** The *Sign in blood* box.
5. **Main menu.** The Ledger menu with *Seek a task*.
6. **Prince list.** The eight princes.
7. **An offer.** One prince's offer box showing favor and rival (Sanguine or Clavicus Vile read well).
8. **Journal.** An active task under the Daedric tab, naming a real location.
9. **Boethiah's champion.** The named champion with the name on the crosshair.
10. **Hircine's quarry.** The named bear or sabre cat in the wild.
11. **Sanguine's revel.** Drinking at a crowded inn with the (2/3) notification.
12. **Relic.** The Gilded Wager or Shard of Nightmare in a boss chest.
13. **Payout.** The "<Prince> is satisfied" box showing favor and rival change.
14. **Standings.** The "Favor of the princes" box with mixed tiers.
15. **Blessing and gift.** A blessing message, and the gift's inventory preview with its enchantment.
