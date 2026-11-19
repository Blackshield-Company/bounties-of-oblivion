Scriptname BOO_LedgerScript extends Quest
{Daedric Ledger: Bounties of Oblivion - main controller. Delivers the Ledger,
runs its menus, dispatches contracts, pays out, and tracks favor, rivalries,
blessings and boons for the eight princes. Since 0.2.0 it also runs the King of
Worms: an antagonist (not a prince) whose menace grows with every task done.}

; ---------------------------------------------------------------------------
; Properties (filled by the plugin generator, see src/plugin)
; ---------------------------------------------------------------------------

GlobalVariable Property BOO_Bound Auto
{1 once the player has signed the Ledger, -1 if they cast it away.}
GlobalVariable Property BOO_ContractActive Auto
GlobalVariable Property BOO_ContractsDone Auto
GlobalVariable Property BOO_IntroMinLevel Auto
GlobalVariable Property BOO_PayMult Auto
GlobalVariable Property BOO_FavorGain Auto
GlobalVariable Property BOO_RivalLoss Auto

GlobalVariable[] Property Favor Auto
{Favor per prince, 0 to 100. Index order below.}
Quest[] Property Contracts Auto
Spell[] Property Blessings Auto
Armor[] Property Boons Auto
Message[] Property OfferMessages Auto

Quest Property MQ101 Auto
Book Property BOO_Ledger Auto
Spell Property BOO_PowerLedger Auto
MiscObject Property Gold001 Auto

Message Property BOO_MsgArrival Auto
Message Property BOO_MsgBindOffer Auto
Message Property BOO_MsgBound Auto
Message Property BOO_MsgRefused Auto
Message Property BOO_MsgLedgerMenu Auto
Message Property BOO_MsgPrinceMenu Auto
Message Property BOO_MsgNoContract Auto
Message Property BOO_MsgContractStatus Auto

; 0.2.0: the King of Worms (Mannimarco). Not a prince: no favor, blessing or boon
; tracks, and never part of the 8-long arrays above. All of his state lives in
; globals, because OnInit does not run again on a save that predates 0.2.0.
GlobalVariable Property BOO_WormMenace Auto
{0 to 100. Grows with every prince task, reset by slaying a thrall.}
GlobalVariable Property BOO_WormSlain Auto
GlobalVariable Property BOO_WormGiftGiven Auto
GlobalVariable Property BOO_WormGain Auto
GlobalVariable Property BOO_WormDeathGain Auto
{Menace gained instead of BOO_WormGain for Namira and Molag Bal tasks.}
GlobalVariable Property BOO_WormThreshold Auto
Quest Property WormHunt Auto
{The thrall hunt (BOO_WormHunt). Runs BOO_ContractScript with Prince == WORM.}
Armor Property BOO_WormBane Auto
Message Property BOO_MsgWormOffer Auto
Message Property BOO_MsgWormStatus Auto

; A single real line-feed character (0x0A), filled by the plugin generator.
; Papyrus string literals cannot hold one, and the build compiler does not expand
; backslash escapes (the text would print a literal backslash and letter), so the
; only escape-free way to break a line in a message box is to carry the character
; in this property. Empty (an old save that predates 0.1.3): Br() falls back to a
; plain space. Never write a backslash in a string literal; the build rejects it.
String Property LineBreak Auto

; Prince indices: 0 Mehrunes Dagon, 1 Boethiah, 2 Namira, 3 Molag Bal,
; 4 Clavicus Vile, 5 Hircine, 6 Sanguine, 7 Vaermina.
Int Property PRINCE_COUNT = 8 AutoReadOnly
Int Property BLESSING_FAVOR = 30 AutoReadOnly
Int Property BOON_FAVOR = 80 AutoReadOnly
Int Property MAX_FAVOR = 100 AutoReadOnly
Int Property AbandonPenalty = 10 AutoReadOnly
; Prince value of the worm hunt. Never an index into Favor/Contracts/Blessings/Boons/OfferMessages.
Int Property WORM = 8 AutoReadOnly
Int Property MAX_MENACE = 100 AutoReadOnly
Int Property WORM_GIFT_SLAIN = 3 AutoReadOnly

Bool ledgerDelivered = False
Float eligibleSince = -1.0
Bool[] boonGiven

; ---------------------------------------------------------------------------
; Text helper
; ---------------------------------------------------------------------------

; aiCount line breaks, for Debug.MessageBox text. Falls back to one space if the
; LineBreak property is empty, so text stays readable and never shows a backslash.
String Function Br(Int aiCount = 1)
	String sep = LineBreak
	If sep == ""
		Return " "
	EndIf
	String result = ""
	Int i = 0
	While i < aiCount
		result += sep
		i += 1
	EndWhile
	Return result
EndFunction

; ---------------------------------------------------------------------------
; Delivery: the Ledger appears after the player sleeps (or after a day
; of being eligible, for players who never sleep).
; ---------------------------------------------------------------------------

Event OnInit()
	boonGiven = new Bool[8]
	RegisterForSleep()
	RegisterForSingleUpdate(30.0)
EndEvent

Event OnUpdate()
	If BOO_Bound.GetValueInt() != 0 || ledgerDelivered
		Return
	EndIf
	If CanDeliver()
		Float now = Utility.GetCurrentGameTime()
		If eligibleSince < 0.0
			eligibleSince = now
		ElseIf now - eligibleSince >= 1.0
			DeliverLedger()
			Return
		EndIf
	EndIf
	RegisterForSingleUpdate(60.0)
EndEvent

Event OnSleepStop(Bool abInterrupted)
	If BOO_Bound.GetValueInt() != 0
		Return
	EndIf
	; Also re-delivers a Ledger the player lost or sold before signing it.
	If (!ledgerDelivered || Game.GetPlayer().GetItemCount(BOO_Ledger) == 0) && CanDeliver()
		DeliverLedger()
	EndIf
EndEvent

Bool Function CanDeliver()
	Actor player = Game.GetPlayer()
	If player.GetLevel() < BOO_IntroMinLevel.GetValueInt()
		Return False
	EndIf
	; Still inside the vanilla Helgen intro? (Alternate-start mods never run it.)
	If MQ101 && MQ101.IsRunning() && !MQ101.IsCompleted()
		Return False
	EndIf
	If player.IsInCombat()
		Return False
	EndIf
	If !Game.IsMovementControlsEnabled() || !Game.IsFightingControlsEnabled()
		Return False
	EndIf
	Return True
EndFunction

Function DeliverLedger()
	ledgerDelivered = True
	BOO_MsgArrival.Show()
	Game.GetPlayer().AddItem(BOO_Ledger, 1)
EndFunction

; Called by BOO_LedgerBookScript when the player reads the Ledger.
Function OnLedgerRead()
	ledgerDelivered = True
	Int bound = BOO_Bound.GetValueInt()
	If bound > 0
		OpenLedger()
		Return
	EndIf
	Int choice = BOO_MsgBindOffer.Show()
	If choice == 0
		Bind()
	ElseIf choice == 2
		BOO_Bound.SetValueInt(-1)
		UnregisterForSleep()
		Game.GetPlayer().RemoveItem(BOO_Ledger, 99, True)
		BOO_MsgRefused.Show()
	EndIf
EndFunction

Function Bind()
	BOO_Bound.SetValueInt(1)
	UnregisterForSleep()
	Game.GetPlayer().AddSpell(BOO_PowerLedger)
	BOO_MsgBound.Show()
EndFunction

; ---------------------------------------------------------------------------
; Ledger menus (lesser power + ledger book)
; ---------------------------------------------------------------------------

Function OpenLedger()
	If BOO_Bound.GetValueInt() <= 0
		Return
	EndIf
	SyncContractActive()
	Bool keepOpen = True
	While keepOpen
		Int choice = BOO_MsgLedgerMenu.Show(BOO_ContractsDone.GetValue())
		If choice == 0
			keepOpen = !PrinceMenu()
		ElseIf choice == 1
			keepOpen = ContractStatusMenu()
		ElseIf choice == 2
			Debug.MessageBox(StandingsText())
			keepOpen = False
		ElseIf choice == 3
			keepOpen = !WormMenu()
		Else
			keepOpen = False
		EndIf
	EndWhile
EndFunction

; Returns True if a contract was taken (closes the ledger).
Bool Function PrinceMenu()
	While True
		Int prince = BOO_MsgPrinceMenu.Show()
		If prince < 0 || prince >= PRINCE_COUNT
			Return False
		EndIf
		If OfferMessages[prince].Show(Favor[prince].GetValue()) == 0
			If TakeContract(prince)
				Return True
			EndIf
		EndIf
	EndWhile
	Return False
EndFunction

Bool Function TakeContract(Int aiPrince)
	Return StartTask(Contracts[aiPrince])
EndFunction

; Starts a task quest (a prince's contract or the worm hunt). One task at a time.
Bool Function StartTask(Quest q)
	If q == None || ActiveContract() != None
		Return False
	EndIf
	If q.IsRunning()
		q.Stop()
	EndIf
	Int tries = 0
	While q.IsStopping() && tries < 50
		Utility.Wait(0.1)
		tries += 1
	EndWhile
	q.Reset()
	If !q.Start()
		BOO_MsgNoContract.Show()
		Return False
	EndIf
	BOO_ContractActive.SetValueInt(1)
	(q as BOO_ContractScript).Begin()
	Return True
EndFunction

; Returns True to keep the ledger open.
Bool Function ContractStatusMenu()
	BOO_ContractScript active = ActiveContract()
	If active == None
		BOO_ContractActive.SetValueInt(0)
		Return True
	EndIf
	If active.Prince == WORM
		Return WormStatusMenu(active)
	EndIf
	If BOO_MsgContractStatus.Show() == 1
		Int prince = active.Prince
		active.Abandon()
		BOO_ContractActive.SetValueInt(0)
		ModFavor(prince, -AbandonPenalty)
		Debug.Notification(PrinceName(prince) + " will remember this.")
		Return True
	EndIf
	Return False
EndFunction

BOO_ContractScript Function ActiveContract()
	Int i = 0
	While i < Contracts.Length
		If Contracts[i].IsRunning()
			Return Contracts[i] as BOO_ContractScript
		EndIf
		i += 1
	EndWhile
	; The worm hunt counts as a task too, but is kept out of the 8-long Contracts array.
	If WormHunt && WormHunt.IsRunning()
		Return WormHunt as BOO_ContractScript
	EndIf
	Return None
EndFunction

Function SyncContractActive()
	If ActiveContract() != None
		BOO_ContractActive.SetValueInt(1)
	Else
		BOO_ContractActive.SetValueInt(0)
	EndIf
EndFunction

; Called by BOO_RevelWineEffectScript whenever the player drinks Revel Wine.
Function OnRevelDrink()
	BOO_ContractScript active = ActiveContract()
	If active && active.ContractType == 3
		active.OnRevelDrink()
	EndIf
EndFunction

; ---------------------------------------------------------------------------
; Payment, favor, rivalries, blessings and boons
; ---------------------------------------------------------------------------

; Called by BOO_ContractScript when a contract is fulfilled.
Function ContractCompleted(Int aiPrince)
	If aiPrince == WORM
		WormSlain()
		Return
	EndIf
	If aiPrince < 0 || aiPrince >= PRINCE_COUNT
		Return
	EndIf
	Actor player = Game.GetPlayer()
	Int pay = TaskPay()
	Int gain = BOO_FavorGain.GetValueInt()
	; The King of Worms at full menace gnaws at every page: favor gains are halved.
	Bool gnawed = Menace() >= MAX_MENACE
	If gnawed
		gain = gain / 2
	EndIf
	Int loss = BOO_RivalLoss.GetValueInt()
	Int rival = RivalOf(aiPrince)

	If pay > 0
		player.AddItem(Gold001, pay, True)
	EndIf
	BOO_ContractsDone.Mod(1)
	BOO_ContractActive.SetValueInt(0)

	Int before = Favor[aiPrince].GetValueInt()
	Int rivalBefore = Favor[rival].GetValueInt()
	SetFavor(aiPrince, before + gain)
	SetFavor(rival, rivalBefore - loss)
	Int after = Favor[aiPrince].GetValueInt()
	Int rivalAfter = Favor[rival].GetValueInt()

	String text = PrinceName(aiPrince) + " is satisfied." + Br(2)
	text += pay + " gold." + Br()
	text += "Favor with " + PrinceName(aiPrince) + ": " + before + " -> " + after + Br()
	If rivalAfter != rivalBefore
		text += "Favor with " + PrinceName(rival) + ": " + rivalBefore + " -> " + rivalAfter
	Else
		text += PrinceName(rival) + " has nothing left to take from you."
	EndIf
	If gnawed
		text += Br(2) + "The King of Worms gnawed the page. Favor gained is halved."
	EndIf

	; Every task feeds the King of Worms; Namira's and Molag Bal's trade in the dead feeds him more.
	Int menaceBefore = Menace()
	Int menaceAfter = AddMenace(MenaceGainFor(aiPrince))
	If WormTier(menaceAfter) != WormTier(menaceBefore)
		text += Br(2) + "The King of Worms: " + WormTierName(menaceAfter) + " (menace " + menaceAfter + "/" + MAX_MENACE + ")"
	EndIf
	Debug.MessageBox(text)

	Int threshold = WormThreshold()
	If menaceBefore < threshold && menaceAfter >= threshold
		Debug.MessageBox("A page of the Ledger has been eaten through. Something wrote in the holes." + Br(2) + "Mannimarco, the King of Worms, has noticed your dealings with the dead, and he has sent a thrall to collect." + Br(2) + "Open the Ledger to hunt it.")
	EndIf

	ApplyTiers(aiPrince)
	ApplyTiers(rival)
EndFunction

; Gold for a finished task (prince contract or worm hunt).
Int Function TaskPay()
	Return Math.Floor((150.0 + 10.0 * Game.GetPlayer().GetLevel()) * BOO_PayMult.GetValue())
EndFunction

Function ModFavor(Int aiPrince, Int aiAmount)
	SetFavor(aiPrince, Favor[aiPrince].GetValueInt() + aiAmount)
	ApplyTiers(aiPrince)
EndFunction

Function SetFavor(Int aiPrince, Int aiValue)
	If aiValue < 0
		aiValue = 0
	ElseIf aiValue > MAX_FAVOR
		aiValue = MAX_FAVOR
	EndIf
	Favor[aiPrince].SetValueInt(aiValue)
EndFunction

; Grants or removes the blessing, and hands out the boon once.
Function ApplyTiers(Int aiPrince)
	Actor player = Game.GetPlayer()
	Int value = Favor[aiPrince].GetValueInt()
	Spell blessing = Blessings[aiPrince]
	If value >= BLESSING_FAVOR
		If !player.HasSpell(blessing)
			player.AddSpell(blessing, False)
			Debug.MessageBox(PrinceName(aiPrince) + " grants you a blessing." + Br(2) + BlessingText(aiPrince) + Br(2) + "It lasts while your favor stays at " + BLESSING_FAVOR + " or more.")
		EndIf
	ElseIf player.HasSpell(blessing)
		player.RemoveSpell(blessing)
		Debug.Notification("The blessing of " + PrinceName(aiPrince) + " fades.")
	EndIf
	If value >= BOON_FAVOR
		If boonGiven.Length != PRINCE_COUNT
			boonGiven = new Bool[8]
		EndIf
		If !boonGiven[aiPrince]
			boonGiven[aiPrince] = True
			player.AddItem(Boons[aiPrince], 1, True)
			Debug.MessageBox(PrinceName(aiPrince) + " counts you among the chosen, and sends a gift: " + BoonText(aiPrince) + ".")
		EndIf
	EndIf
EndFunction

Int Function RivalOf(Int aiPrince)
	If aiPrince == 0
		Return 1
	ElseIf aiPrince == 1
		Return 0
	ElseIf aiPrince == 2
		Return 4
	ElseIf aiPrince == 4
		Return 2
	ElseIf aiPrince == 3
		Return 5
	ElseIf aiPrince == 5
		Return 3
	ElseIf aiPrince == 6
		Return 7
	EndIf
	Return 6
EndFunction

String Function PrinceName(Int aiPrince)
	If aiPrince == 0
		Return "Mehrunes Dagon"
	ElseIf aiPrince == 1
		Return "Boethiah"
	ElseIf aiPrince == 2
		Return "Namira"
	ElseIf aiPrince == 3
		Return "Molag Bal"
	ElseIf aiPrince == 4
		Return "Clavicus Vile"
	ElseIf aiPrince == 5
		Return "Hircine"
	ElseIf aiPrince == 6
		Return "Sanguine"
	EndIf
	Return "Vaermina"
EndFunction

String Function BlessingText(Int aiPrince)
	If aiPrince == 0
		Return "Blessing of Dagon: Resist Fire +15%"
	ElseIf aiPrince == 1
		Return "Blessing of Boethiah: Health +20"
	ElseIf aiPrince == 2
		Return "Blessing of Namira: Resist Disease +50%, Resist Poison +15%"
	ElseIf aiPrince == 3
		Return "Blessing of Molag Bal: Resist Frost +15%"
	ElseIf aiPrince == 4
		Return "Blessing of Clavicus Vile: Carry Weight +25"
	ElseIf aiPrince == 5
		Return "Blessing of Hircine: Stamina +25"
	ElseIf aiPrince == 6
		Return "Blessing of Sanguine: Stamina regenerates 25% faster"
	EndIf
	Return "Blessing of Vaermina: Magicka regenerates 25% faster"
EndFunction

String Function BoonText(Int aiPrince)
	If aiPrince == 0
		Return "the Ember Torc of Dagon (Destruction +12%)"
	ElseIf aiPrince == 1
		Return "Oathbreaker's Band (One-Handed +15%)"
	ElseIf aiPrince == 2
		Return "the Necklace of the Hollow (Sneak +15%)"
	ElseIf aiPrince == 3
		Return "the Chain of Coldharbour (Two-Handed +15%)"
	ElseIf aiPrince == 4
		Return "the Band of the Wager (Speech +15%)"
	ElseIf aiPrince == 5
		Return "the Fang of the Hunt (Archery +15%)"
	ElseIf aiPrince == 6
		Return "Sanguine's Merry Band (Health regenerates 25% faster)"
	EndIf
	Return "the Dreamstone of Vaermina (Illusion +15%)"
EndFunction

String Function TierName(Int aiFavor)
	If aiFavor >= BOON_FAVOR
		Return "Chosen"
	ElseIf aiFavor >= BLESSING_FAVOR
		Return "Blessed"
	ElseIf aiFavor > 0
		Return "Noticed"
	EndIf
	Return "Unknown"
EndFunction

; ---------------------------------------------------------------------------
; The King of Worms (0.2.0)
; ---------------------------------------------------------------------------

Int Function Menace()
	If BOO_WormMenace == None
		Return 0
	EndIf
	Return BOO_WormMenace.GetValueInt()
EndFunction

Int Function WormThreshold()
	If BOO_WormThreshold == None
		Return 50
	EndIf
	Return BOO_WormThreshold.GetValueInt()
EndFunction

Int Function MenaceGainFor(Int aiPrince)
	If aiPrince == 2 || aiPrince == 3
		If BOO_WormDeathGain == None
			Return 25
		EndIf
		Return BOO_WormDeathGain.GetValueInt()
	EndIf
	If BOO_WormGain == None
		Return 15
	EndIf
	Return BOO_WormGain.GetValueInt()
EndFunction

; Adds to the menace, clamped to 0..MAX_MENACE. Returns the new value.
Int Function AddMenace(Int aiAmount)
	If BOO_WormMenace == None
		Return 0
	EndIf
	Int value = BOO_WormMenace.GetValueInt() + aiAmount
	If value < 0
		value = 0
	ElseIf value > MAX_MENACE
		value = MAX_MENACE
	EndIf
	BOO_WormMenace.SetValueInt(value)
	Return value
EndFunction

; 0 Unnoticed, 1 Watching, 2 Hunting, 3 Gnawing.
Int Function WormTier(Int aiMenace)
	If aiMenace >= MAX_MENACE
		Return 3
	ElseIf aiMenace >= WormThreshold()
		Return 2
	ElseIf aiMenace >= 1
		Return 1
	EndIf
	Return 0
EndFunction

String Function WormTierName(Int aiMenace)
	Int tier = WormTier(aiMenace)
	If tier == 3
		Return "Gnawing"
	ElseIf tier == 2
		Return "Hunting"
	ElseIf tier == 1
		Return "Watching"
	EndIf
	Return "Unnoticed"
EndFunction

; Main menu button "The King of Worms". Returns True if the hunt was taken (closes the ledger).
Bool Function WormMenu()
	If BOO_WormMenace == None || WormHunt == None || BOO_MsgWormOffer == None
		Return False
	EndIf
	If Menace() < WormThreshold()
		Return False
	EndIf
	If BOO_MsgWormOffer.Show(BOO_WormMenace.GetValue()) == 0
		Return StartTask(WormHunt)
	EndIf
	Return False
EndFunction

; Returns True to keep the ledger open.
Bool Function WormStatusMenu(BOO_ContractScript akHunt)
	If BOO_MsgWormStatus.Show() == 1
		; No favor penalty and the menace stays where it is.
		akHunt.Abandon()
		BOO_ContractActive.SetValueInt(0)
		Debug.Notification("The King of Worms laughs from the dark.")
		Return True
	EndIf
	Return False
EndFunction

; Called (through ContractCompleted) when the worm hunt's thrall dies. No prince favor changes.
Function WormSlain()
	Actor player = Game.GetPlayer()
	Int pay = TaskPay()
	If pay > 0
		player.AddItem(Gold001, pay, True)
	EndIf
	BOO_ContractActive.SetValueInt(0)
	If BOO_WormMenace
		BOO_WormMenace.SetValueInt(0)
	EndIf
	Int slain = 0
	If BOO_WormSlain
		BOO_WormSlain.Mod(1)
		slain = BOO_WormSlain.GetValueInt()
	EndIf

	String text = "The thrall of the King of Worms is dead." + Br(2)
	text += pay + " gold." + Br()
	text += "The worm-eaten page crumbles to dust. Menace: 0/" + MAX_MENACE + Br()
	text += "Thralls slain: " + slain + Br(2)
	text += "Arkay's priests would thank you, if they knew."
	Debug.MessageBox(text)

	If BOO_WormGiftGiven == None || BOO_WormBane == None
		Return
	EndIf
	If slain >= WORM_GIFT_SLAIN && BOO_WormGiftGiven.GetValueInt() == 0
		BOO_WormGiftGiven.SetValueInt(1)
		player.AddItem(BOO_WormBane, 1, True)
		Debug.MessageBox("Three of his thralls lie dead. Where the last one fell, among the worms, lies an amulet of Arkay that was not there before." + Br(2) + "You receive the Amulet of the Worm's Bane (Restoration +15%).")
	EndIf
EndFunction

String Function StandingsText()
	String text = "FAVOR OF THE PRINCES" + Br()
	Int i = 0
	While i < PRINCE_COUNT
		Int value = Favor[i].GetValueInt()
		text += Br() + PrinceName(i) + ": " + value + " (" + TierName(value) + ")"
		i += 1
	EndWhile
	text += Br(2) + "Blessing at " + BLESSING_FAVOR + ". Gift at " + BOON_FAVOR + ". Contracts fulfilled: " + BOO_ContractsDone.GetValueInt()
	If BOO_WormMenace && BOO_WormSlain
		Int menace = Menace()
		text += Br(2) + "The King of Worms: menace " + menace + "/" + MAX_MENACE + " (" + WormTierName(menace) + "). Thralls slain: " + BOO_WormSlain.GetValueInt() + "."
	EndIf
	Return text
EndFunction
