# In-game test checklist (v0.1.0, early beta)

v0.1.0 ships as an early beta. It passed every automated build check but hasn't been played in-game yet. This checklist is for players and testers who want to confirm everything works, and for the next round of fixes.

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

## Stuck? Reset commands
```
stopquest BOO_ContractDagon      (or any BOO_Contract<Prince>)
set BOO_ContractActive to 0
set BOO_Bound to 1               (then player.addspell FExxx81A to get the power)
```
