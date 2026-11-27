Scriptname BOO_WintersunBridge Hidden
{Daedric Ledger - reads the deity the player follows in Wintersun - Faiths of
Skyrim. Kept in its own script so BOO_LedgerScript never names Wintersun's
script type. Only called after Wintersun's tracker quest was found with
Game.GetFormFromFile, so without Wintersun this script never runs.}

Int Function WorshipID(Quest akTracker) Global
	wsn_trackerquest_quest tracker = akTracker as wsn_trackerquest_quest
	If !tracker
		Return -1
	EndIf
	Return tracker.worshipID
EndFunction
