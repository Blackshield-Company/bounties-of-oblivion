# In-game test checklist (v0.2.0, early beta)

v0.2.0 adds the King of Worms (section 9). v0.1.3 fixed line breaks in the favor, contract-satisfied and blessing boxes. Like v0.1.0, it passed every automated build check but hasn't been played in-game yet. This checklist is for players and testers who want to confirm everything works, and for the next round of fixes.

Open the console with `~`. Commands that take a quest or global accept the EditorID directly. `player.additem` needs a full FormID, which depends on your load order, so look it up first:

```
help "Daedric Ledger" 4         -> e.g. BOOK: (FE013834) 'The Daedric Ledger'
```

ESL-flagged plugins show up as `FE` + a 3-digit slot (`xxx`) + the record's own 3 digits. The record IDs are:

| Record | Local ID | Full FormID |
|---|---|---|
| The Daedric Ledger (book) | 834 | `FExxx834` |
| Consult the Daedric Ledger (power) | 81A | `FExxx81A` |
| Sanguine's Revel Wine | 837 | `FExxx837` |
| Gilded Wager | 835 | `FExxx835` |
| Shard of Nightmare | 836 | `FExxx836` |
| Gifts: Dagon, Boethiah, Namira, Molag Bal | 825, 827, 829, 82B | `FExxx825` … |
| Gifts: Clavicus Vile, Hircine, Sanguine, Vaermina | 82D, 82F, 831, 833 | `FExxx82D` … |
| Blessings (abilities), Dagon … Vaermina | 81C to 823 | `FExxx81C` … |
| Amulet of the Worm's Bane (0.2.0) | 859 | `FExxx859` |

Favor globals: `BOO_FavorDagon`, `BOO_FavorBoethiah`, `BOO_FavorNamira`, `BOO_FavorMolagBal`, `BOO_FavorClavicus`, `BOO_FavorHircine`, `BOO_FavorSanguine`, `BOO_FavorVaermina`.

**Turn on Papyrus logging before testing.** Add this to `Skyrim.ini` (under Proton: `.../compatdata/489830/pfx/drive_c/users/steamuser/Documents/My Games/Skyrim Special Edition/Skyrim.ini`):

```ini
[Papyrus]
bEnableLogging=1
bEnableTrace=1
bLoadDebugInformation=1
```

The log is written to `.../My Games/Skyrim Special Edition/Logs/Script/Papyrus.0.log`. After each session, run `grep -n "BOO_" Papyrus.0.log` and report anything it finds.

---

## 0. Plugin loads
- [ ] `BountiesOfOblivion.esp` is enabled and the game reaches the main menu.
- [ ] Load a save. `show BOO_Bound` prints `0.00`. `sqv BOO_LedgerQuest` shows the quest running.
- [ ] With Blackshield Company also installed, both plugins load and `show BSC_Joined` still works.

## 1. Ledger arrival
- [ ] Level 5+ character, past Helgen. Sleep for an hour. On waking, the arrival message appears and **The Daedric Ledger** is in your inventory.
  - Faster: `set BOO_IntroMinLevel to 1`.
  - Never sleeping: wait about one in-game day (`set timescale to 2000` for a minute, then back to `20`).
  - Bypass: `player.additem FExxx834 1`.
- [ ] The book shows the Terms, Rivalries and Use pages, and the conjuration-tome mesh in the inventory preview.

## 2. The pact
- [ ] Read the Ledger and close the inventory. *Sign in blood / Not now / Cast the Ledger away for good* appears.
- [ ] *Not now* closes it. Reading again offers it again.
- [ ] Drop or sell the unsigned Ledger, then sleep. It comes back.
- [ ] *Sign in blood*: the bound message appears and **Consult the Daedric Ledger** is in Magic > Powers. `show BOO_Bound` is `1.00`.
- [ ] (Separate save) *Cast the Ledger away*: the book is gone, `show BOO_Bound` is `-1.00`, and it never returns after sleeping.

## 3. Menus
- [ ] Cast the power. The menu shows contracts fulfilled 0 and *Seek a task / Favor of the princes / Close the Ledger*. *Current task* is hidden.
- [ ] *Seek a task* lists the 8 princes and *Back*. Each prince shows their own offer text and current favor. *Decline* returns to the prince list.
- [ ] *Favor of the princes* lists all 8 at 0 (Unknown).
- [ ] Reading the book after signing opens the same menu.

## 4. Each task
For each prince: accept, check the journal (Daedric section) names a real location, check the map marker, complete, check the payout box. `sqv BOO_Contract<Prince>` shows alias fills (`Target`, `MapMarker`, `RelicContainer`, `Relic`).

- [ ] **Dagon**: the leader of a bandit/Forsworn camp. Fast: `sqv BOO_ContractDagon`, `prid <Target id>`, `moveto player`, `kill`.
- [ ] **Boethiah**: a named champion (Brannoc Redhand, Ulfgar Twice-Sworn or Sarith the Unbowed) at a dungeon's map marker. Hostile, name on the crosshair.
- [ ] **Namira**: a draugr crypt. Killing the boss or clearing the location completes it.
- [ ] **Molag Bal**: the boss of a vampire lair.
- [ ] **Clavicus Vile**: the **Gilded Wager** is in the boss chest of a Dwemer ruin. The quest marker points at the chest. Taking it completes the task and the Wager vanishes from your inventory.
- [ ] **Hircine**: a named bear or sabre cat (Old Scar-Muzzle, The White Sow, Greyfang, The Silent Stalker) at an animal den or giant camp map marker.
- [ ] **Sanguine**: you get 4 bottles of **Sanguine's Revel Wine**. Drinking outside the chosen inn shows "Sanguine sulks…" and doesn't count. Inside the inn, each drink shows (1/3), (2/3) and the objective counter updates. The third completes it.
- [ ] **Vaermina**: the **Shard of Nightmare** is in the boss chest of a warlock lair or hagraven nest.
- [ ] If no location fits (rare), "The page stays blank…" appears and nothing starts.

## 5. Favor, rivalry, blessings, gifts
- [ ] Finishing a Dagon task shows gold, Dagon favor 0 -> 20, and "Boethiah has nothing left to take from you" (Boethiah was at 0).
- [ ] `set BOO_FavorBoethiah to 50`, finish a Dagon task. Boethiah drops to 40.
- [ ] `set BOO_FavorDagon to 15`, finish a Dagon task. Favor 35, *Blessing of Dagon* message, and the blessing shows in Active Effects (Resist Fire).
- [ ] With the Dagon blessing active, `set BOO_FavorDagon to 35` and finish a Boethiah task. Dagon drops to 25, "The blessing of Mehrunes Dagon fades" appears and the effect is gone.
- [ ] `set BOO_FavorHircine to 65`, finish a Hircine task. Favor 85, blessing (if not already) and the **Fang of the Hunt** gift. A second task at 100 doesn't give another.
- [ ] Equip every gift on a male and a female character. Rings and amulets show the vanilla meshes. Enchantment names match the item names. They can't be disenchanted.

## 6. Abandon
- [ ] Accept any task, then *Ledger > Current task > Abandon*. The journal shows it failed, favor with that prince drops by 10 (not below 0), and *Seek a task* is back.

## 7. Persistence
- [ ] Save during an active task, quit to desktop, reload. The task still completes.
- [ ] Save after earning a blessing and a gift, reload. Both are kept and the gift isn't handed out again.

## 8. v0.1.3 checks (not yet run in game)
- [ ] Open the ledger, pick *Favor of the princes*: a title, then each of the 8 princes on their own line, then the closing line with a blank line before it. No `\n` or backslash text anywhere.
- [ ] Finish any task: the contract-satisfied box shows "<Prince> is satisfied.", a blank line, then gold and each favor change on separate lines. No `\n`.
- [ ] Earn a blessing: the blessing box has a blank line between the title line, the effect and the closing sentence.
- [ ] If any of these show one long line, the `LineBreak` property is empty (a save that kept the pre-0.1.3 script data). The text is still readable; please report it.

## 9. v0.2.0: the King of Worms (not yet run in game)
King of Worms globals: `BOO_WormMenace` (`FExxx851`), `BOO_WormSlain` (`FExxx852`), `BOO_WormGiftGiven` (`FExxx853`), tunables `BOO_WormGain` (`FExxx854`), `BOO_WormDeathGain` (`FExxx855`), `BOO_WormThreshold` (`FExxx856`). The hunt quest is `BOO_WormHunt` (`FExxx857`).

Menace and the payout box:
- [ ] On a 0.1.3 save that already has favor and gifts, load with 0.2.0. `show BOO_WormMenace` prints `0.00`. Favor and gifts are unchanged. *Favor of the princes* ends with "The King of Worms: menace 0/100 (Unnoticed). Thralls slain: 0."
- [ ] Finish a Dagon task. `show BOO_WormMenace` is `15.00`, and the satisfied box ends with "The King of Worms: Watching (menace 15/100)".
- [ ] Finish a Namira (or Molag Bal) task. Menace goes up by 25, not 15.
- [ ] `set BOO_WormMenace to 40`, finish a Namira task. Menace 65. The box notes "Hunting", then a second box says a page of the Ledger has been eaten through and a thrall was sent.
- [ ] `set BOO_WormMenace to 100`, finish any prince task. Favor rises by 10 (half of 20), the box says "The King of Worms gnawed the page", and menace stays 100. The rival still loses 10.
- [ ] `set BOO_WormMenace to 99`, finish a task: menace 100 and the stage note says "Gnawing". The favor gain for this task is still full.

The hunt:
- [ ] `set BOO_WormMenace to 49`: the Ledger menu has no *The King of Worms* button. `set BOO_WormMenace to 50`: the button appears between *Favor of the princes* and *Close the Ledger*.
- [ ] `set BOO_WormThreshold to 70` with menace 50: the button disappears. Set it back to 50.
- [ ] With a prince task running, the button is hidden (only *Current task* shows).
- [ ] Choose *The King of Worms*: the offer names Mannimarco, Arkay and the current menace. *Decline* returns to the menu. *Hunt the Worm Thrall* starts **Daedric Ledger: The King of Worms**. The journal names the thrall (Thrall of the King of Worms, Worm Acolyte or Hand of Mannimarco) and a dungeon, and the map marker is placed.
- [ ] `sqv BOO_WormHunt` shows `ContractLocation`, `MapMarker` and `Target` filled. The thrall is a hostile necromancer with its name on the crosshair. Fast: `prid <Target id>`, `moveto player`, `kill`.
- [ ] While the hunt runs, *Seek a task* is hidden and *Current task* opens the hunt box (*Close the Ledger / Abandon the hunt*).
- [ ] Kill the thrall: the box shows gold, "Menace: 0/100", thralls slain 1 and "Arkay's priests would thank you, if they knew." `show BOO_WormMenace` is `0.00`, `show BOO_WormSlain` is `1.00`, and no prince's favor changed. *Seek a task* is back.
- [ ] Abandon a hunt instead: the journal shows it failed, "The King of Worms laughs from the dark." appears, menace is unchanged, and no favor is lost.

Amulet:
- [ ] `set BOO_WormSlain to 2`, `set BOO_WormMenace to 50`, finish a hunt. A second box gives the **Amulet of the Worm's Bane**. Equip it: Restoration +15% in Active Effects, Amulet of Arkay worn model, and it can't be disenchanted. (Its ground and inventory model is the plain silver necklace. Please report how it looks.)
- [ ] Finish a fourth hunt: no second amulet. `show BOO_WormGiftGiven` is `1.00`.
- [ ] Bypass: `player.additem FExxx859 1`.

## Stuck? Reset commands
```
stopquest BOO_ContractDagon      (or any BOO_Contract<Prince>)
set BOO_ContractActive to 0
stopquest BOO_WormHunt           (then set BOO_ContractActive to 0)
set BOO_WormMenace to 0
set BOO_Bound to 1               (then player.addspell FExxx81A to get the power)
```
