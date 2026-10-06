namespace BountiesOfOblivion.Generator;

/// <summary>All player-facing text in one place.</summary>
internal static class Texts
{
    public const string LedgerName = "The Daedric Ledger";
    public const string Ledger =
@"<p align='center'><font size='40'>THE DAEDRIC LEDGER</font></p>
<p align='center'>Being a Register of Debts Owed to the Princes of Oblivion</p>

[pagebreak]

<p align='center'><font size='28'>Terms</font></p>

Eight seals are pressed into the first page. Mehrunes Dagon. Boethiah. Namira. Molag Bal. Clavicus Vile. Hircine. Sanguine. Vaermina.

Sign your name, and each of them may write a task beneath it. Finish the task and the page burns clean. The prince pays in gold, and in favor.

Favor is remembered. At thirty, a prince lends you a blessing. At eighty, a prince sends you a gift of their own.

[pagebreak]

<p align='center'><font size='28'>Rivalries</font></p>

The princes do not love one another. Serve one, and their rival takes note.

Dagon and Boethiah: two hungers for ruin.
Namira and Clavicus Vile: the hoarder of filth and the hoarder of wishes.
Molag Bal and Hircine: the one who enslaves and the one who hunts.
Sanguine and Vaermina: the revel and the nightmare.

Each task done for one costs favor with the other. A blessing is lost if favor falls below thirty.

[pagebreak]

<p align='center'><font size='28'>Use</font></p>

Open the Ledger, or call on it with the power it grants, and choose a seal. One task at a time. Break your word, and the prince remembers that as well.";

    public const string Arrival =
        "You wake with ash on your fingertips. A heavy black book lies beside you, bound in something that is not quite leather. It was not there when you lay down.\n\nThe Daedric Ledger has found you.";

    public const string BindOffer =
        "Eight seals mark the first page of the Daedric Ledger. Beneath them is an empty line, waiting for a name.\n\nSign it, and the Princes of Oblivion will offer you their work. Their gold and their favor come with it. So does their memory.";

    public const string Bound =
        "Your name sinks into the page like blood into snow. Somewhere beyond Mundus, eight princes turn their heads.\n\nYou can now call on the Ledger with a lesser power: Consult the Daedric Ledger.";

    public const string Refused =
        "You throw the Ledger into the nearest shadow. It does not come back.";

    public const string LedgerMenu =
        "THE DAEDRIC LEDGER\n\nEight princes. Eight seals. One name, signed in blood.\n\nContracts fulfilled: %.0f";

    public const string PrinceMenu = "Whose seal will you break?";

    public const string NoContract =
        "The page stays blank. No place in Skyrim answers this prince's call right now.\n\nTry another prince, or come back later.";

    public const string Status =
        "A task is already written in the Ledger. Its details are in your journal.\n\nThe princes do not share. Finish it, or break your word: abandoning a task costs 10 favor with that prince.";

    public sealed record Prince(
        string Key,          // EditorID fragment
        string Name,         // full name
        string Short,        // short name used in record names
        string QuestName,
        string StartLog,
        string Objective,
        string Offer,
        string BlessingName,
        string BlessingDesc,
        string BoonName);

    public static readonly Prince[] Princes =
    {
        new("Dagon", "Mehrunes Dagon", "Dagon",
            "Daedric Ledger: Ashes for Dagon",
            "Mehrunes Dagon demands ruin. The leader of the outlaws at <Alias=ContractLocation> is to die, and the Prince of Destruction expects it soon.",
            "Kill the leader of <Alias=ContractLocation>",
            "MEHRUNES DAGON\nPrince of Destruction\n\n\"Find the strongest of the outlaws and burn them out. A camp of bandits or Forsworn. Their leader dies by your hand.\"\n\nFavor with Dagon: %.0f / 100\nRival: Boethiah",
            "Blessing of Dagon", "Resist Fire +15%.", "Ember Torc of Dagon"),
        new("Boethiah", "Boethiah", "Boethiah",
            "Daedric Ledger: Boethiah's Proving",
            "Boethiah has named a champion for me to face: <Alias=Target>, who waits near <Alias=ContractLocation>. Only one of us walks away.",
            "Defeat <Alias=Target> near <Alias=ContractLocation>",
            "BOETHIAH\nPrince of Plots\n\n\"I have chosen a champion who has never lost. Prove that I chose wrong. Face them, and leave nothing standing.\"\n\nFavor with Boethiah: %.0f / 100\nRival: Mehrunes Dagon",
            "Blessing of Boethiah", "Health +20.", "Oathbreaker's Band"),
        new("Namira", "Namira", "Namira",
            "Daedric Ledger: Namira's Feast",
            "Namira wants the restless dead of <Alias=ContractLocation> silenced, so that her children may feed undisturbed.",
            "Clear <Alias=ContractLocation>",
            "NAMIRA\nLady of Decay\n\n\"The draugr guard their crypts too jealously. Put them back in the ground, and let the beetles and the spiders have the tomb.\"\n\nFavor with Namira: %.0f / 100\nRival: Clavicus Vile",
            "Blessing of Namira", "Resist Disease +50%, Resist Poison +15%.", "Ring of the Hollow"),
        new("MolagBal", "Molag Bal", "Molag Bal",
            "Daedric Ledger: Bal's Tithe",
            "Molag Bal will not share his prey. The master of <Alias=ContractLocation>, a vampire who forgot whose blood it drinks, must be destroyed.",
            "Destroy the master of <Alias=ContractLocation>",
            "MOLAG BAL\nKing of Schemes\n\n\"A child of my curse has grown proud in its lair. Remind it who it belongs to. Bring it to its knees, then to its end.\"\n\nFavor with Molag Bal: %.0f / 100\nRival: Hircine",
            "Blessing of Molag Bal", "Resist Frost +15%.", "Chain of Coldharbour"),
        new("Clavicus", "Clavicus Vile", "Clavicus Vile",
            "Daedric Ledger: A Wager with Vile",
            "Clavicus Vile lost a bet and wants his stake back: the Gilded Wager, hidden in the deepest chest of <Alias=ContractLocation>.",
            "Recover the Gilded Wager from <Alias=ContractLocation>",
            "CLAVICUS VILE\nPrince of Bargains\n\n\"I may have wagered a little trinket with a dead Dwemer. Ancient history. Fetch it from their ruin and I will make it worth your while. Probably.\"\n\nFavor with Clavicus Vile: %.0f / 100\nRival: Namira",
            "Blessing of Clavicus Vile", "Carry Weight +25.", "Band of the Wager"),
        new("Hircine", "Hircine", "Hircine",
            "Daedric Ledger: Hircine's Quarry",
            "Hircine has marked <Alias=Target> as his quarry. It roams near <Alias=ContractLocation>. The Huntsman wants it brought down.",
            "Hunt down <Alias=Target> near <Alias=ContractLocation>",
            "HIRCINE\nThe Huntsman\n\n\"I have marked a beast for the hunt. It is old, and it is cunning, and it will not die easily. Show me you are the better predator.\"\n\nFavor with Hircine: %.0f / 100\nRival: Molag Bal",
            "Blessing of Hircine", "Stamina +25.", "Fang of the Hunt"),
        new("Sanguine", "Sanguine", "Sanguine",
            "Daedric Ledger: Sanguine's Revel",
            "Sanguine wants a toast in his name. I am to drink three bottles of Sanguine's Revel Wine inside <Alias=ContractLocation>.",
            "Drink Sanguine's Revel Wine at <Alias=ContractLocation> (<Global=BOO_RevelCount>/3)",
            "SANGUINE\nPrince of Debauchery\n\n\"Work, work, work. Have a drink! Have three, in fact, at the inn I pick. I'll even send the wine.\"\n\nFavor with Sanguine: %.0f / 100\nRival: Vaermina",
            "Blessing of Sanguine", "Stamina regenerates 25% faster.", "Sanguine's Merry Band"),
        new("Vaermina", "Vaermina", "Vaermina",
            "Daedric Ledger: Vaermina's Nightmare",
            "A shard of Vaermina's nightmares has fallen into mortal hands at <Alias=ContractLocation>. She wants it back, and the one who holds it does not want to give it up.",
            "Retrieve the Shard of Nightmare from <Alias=ContractLocation>",
            "VAERMINA\nMother of Nightmares\n\n\"A sliver of my dreaming has been stolen by a witch who thinks she can use it. Take it back from her lair. Do not look into it for too long.\"\n\nFavor with Vaermina: %.0f / 100\nRival: Sanguine",
            "Blessing of Vaermina", "Magicka regenerates 25% faster.", "Dreamstone of Vaermina"),
    };

    public const string StageDone = "The Ledger's page burned clean. The prince has paid.";
    public const string StageAbandoned = "I broke my word to the prince. The Ledger remembers.";
}
