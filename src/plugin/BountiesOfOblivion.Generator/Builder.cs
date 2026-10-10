using Modkit.Mutagen;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using Noggog;
using static BountiesOfOblivion.Generator.Vanilla;

namespace BountiesOfOblivion.Generator;

internal sealed class Builder
{
    private readonly SkyrimMod _mod;
    private readonly FormIdRegistry _ids;
    private static readonly SkyrimRelease R = SkyrimRelease.SkyrimSE;

    // Contract types (must match BOO_ContractScript)
    // Quest DNAM form version written by the SE Creation Kit (Mutagen defaults to 255).
    private const byte VanillaQuestFormVersion = 65;

    private const int TypeKill = 0, TypeClear = 1, TypeFetch = 2, TypeRevel = 3;

    // Alias layout shared by every contract quest
    private const uint AliasLocation = 0, AliasMapMarker = 1, AliasTarget = 2, AliasRelic = 3;

    /// <summary>Script properties deliberately left unfilled ("EditorID:Script.Property"), for the validator.</summary>
    public HashSet<string> ExpectedUnfilled { get; } = new();

    public Builder(SkyrimMod mod, FormIdRegistry ids)
    {
        _mod = mod;
        _ids = ids;
    }

    private FormKey Id(string editorId) => _ids.Get(editorId);
    private static FormLink<T> L<T>(FormKey k) where T : class, IMajorRecordGetter => new(k);

    private sealed record PrinceData(
        Texts.Prince Text,
        (FormKey mgef, float mag)[] Blessing,
        (FormKey mgef, float mag) Boon,
        bool BoonIsRing, string BoonMesh, FormKey BoonAA, int BoonValue);

    private static readonly PrinceData[] PrinceSpecs =
    {
        new(Texts.Princes[0], new[] { (AbResistFire, 15f) }, (EnchFortifyDestructionConstantSelf, 12f),
            false, MeshAmuletGoldRuby, GoldAmuletRubyAA, 900),
        new(Texts.Princes[1], new[] { (AbFortifyHealth, 20f) }, (EnchFortifyOneHandedConstantSelf, 15f),
            true, MeshRingGoldDiamond, RingGoldDiamondAA, 900),
        new(Texts.Princes[2], new[] { (AbResistDisease, 50f), (AbResistPoison, 15f) }, (EnchFortifySneakConstantSelf, 15f),
            false, MeshAmuletSilver, SilverAmuletAA, 700), // 0.1.1: necklace, not a ring (vanilla already has the Ring of Namira)
        new(Texts.Princes[3], new[] { (AbResistFrost, 15f) }, (EnchFortifyTwoHandedConstantSelf, 15f),
            false, MeshAmuletSilver, SilverAmuletAA, 800),
        new(Texts.Princes[4], new[] { (AbFortifyCarryWeight, 25f) }, (EnchFortifySpeechcraftConstantSelf, 15f),
            true, MeshRingGoldSapphire, RingGoldSapphireAA, 1000),
        new(Texts.Princes[5], new[] { (AbFortifyStamina, 25f) }, (EnchFortifyArcheryConstantSelf, 15f),
            false, MeshAmuletSilverEmerald, SilverAmuletEmeraldAA, 900),
        new(Texts.Princes[6], new[] { (AbFortifyStaminaRate, 25f) }, (EnchFortifyHealRateConstantSelf, 25f),
            true, MeshRingGoldEmerald, RingGoldEmeraldAA, 900),
        new(Texts.Princes[7], new[] { (AbFortifyMagickaRate, 25f) }, (EnchFortifyIllusionConstantSelf, 15f),
            false, MeshAmuletGoldGems, GoldAmuletGemsAA, 900),
    };

    // ----------------------------------------------------------------------------------------
    public void Build()
    {
        // FormIDs are reserved in a fixed order on first generation; afterwards formids.json rules.
        var questLedger = Id("BOO_LedgerQuest");
        var contractQuests = PrinceSpecs.Select(p => Id($"BOO_Contract{p.Text.Key}")).ToArray();

        // --- Globals ---------------------------------------------------------------------------
        var gBound = Global("BOO_Bound", 0);
        var gActive = Global("BOO_ContractActive", 0);
        var gDone = Global("BOO_ContractsDone", 0);
        var gMinLevel = Global("BOO_IntroMinLevel", 5);
        var gPayMult = Global("BOO_PayMult", 1.0f);
        var gFavorGain = Global("BOO_FavorGain", 20);
        var gRivalLoss = Global("BOO_RivalLoss", 10);
        var gRevelCount = Global("BOO_RevelCount", 0);
        var gFavor = PrinceSpecs.Select(p => Global($"BOO_Favor{p.Text.Key}", 0)).ToArray();

        // --- Magic: ledger power, revel wine effect ---------------------------------------------
        var mgefLedger = Add(ScriptEffect("BOO_LedgerEffect", "Consult the Daedric Ledger",
            "Open the Daedric Ledger.", CastType.FireAndForget,
            Vmad.Adapter(Vmad.Script("BOO_LedgerEffectScript", Vmad.Obj("Ledger", questLedger)))));

        var powerLedger = Add(new Spell(Id("BOO_PowerLedger"), R)
        {
            EditorID = "BOO_PowerLedger",
            Name = "Consult the Daedric Ledger",
            Description = "Open the Daedric Ledger: take a task from a Daedric Prince, check your current task, and see where you stand with each prince.",
            Type = SpellType.LesserPower,
            CastType = CastType.FireAndForget,
            TargetType = TargetType.Self,
            Flags = SpellDataFlag.ManualCostCalc | SpellDataFlag.NoAbsorbOrReflect,
            BaseCost = 0,
            EquipmentType = L<IEquipTypeGetter>(EquipVoice).AsNullable(),
            Effects = { Effect(mgefLedger.FormKey, 0, 0) },
        });

        var mgefRevel = Add(ScriptEffect("BOO_RevelWineEffect", "Sanguine's Revel",
            "A toast to the Prince of Debauchery.", CastType.FireAndForget,
            Vmad.Adapter(Vmad.Script("BOO_RevelWineEffectScript", Vmad.Obj("Ledger", questLedger)))));

        // --- Blessings (abilities on vanilla magic effects) -----------------------------------
        var blessings = PrinceSpecs.Select(p => Add(new Spell(Id($"BOO_Blessing{p.Text.Key}"), R)
        {
            EditorID = $"BOO_Blessing{p.Text.Key}",
            Name = p.Text.BlessingName,
            Description = p.Text.BlessingDesc,
            Type = SpellType.Ability,
            CastType = CastType.ConstantEffect,
            TargetType = TargetType.Self,
            Flags = SpellDataFlag.ManualCostCalc | SpellDataFlag.NoAbsorbOrReflect,
            BaseCost = 0,
            EquipmentType = L<IEquipTypeGetter>(EquipEitherHand).AsNullable(),
            Effects = p.Blessing.Select(b => Effect(b.mgef, b.mag, 0)).ToExtendedList(),
        })).ToArray();

        // --- Boons (vanilla jewelry meshes, vanilla enchantment effects) ----------------------
        var boons = PrinceSpecs.Select(p =>
        {
            var ench = Add(new ObjectEffect(Id($"BOO_EnchBoon{p.Text.Key}"), R)
            {
                EditorID = $"BOO_EnchBoon{p.Text.Key}",
                Name = p.Text.BoonName,
                CastType = CastType.ConstantEffect,
                TargetType = TargetType.Self,
                EnchantType = ObjectEffect.EnchantTypeEnum.Enchantment,
                Flags = ObjectEffect.Flag.NoAutoCalc,
                Effects = { Effect(p.Boon.mgef, p.Boon.mag, 0) },
            });
            return Add(new Armor(Id($"BOO_Boon{p.Text.Key}"), R)
            {
                EditorID = $"BOO_Boon{p.Text.Key}",
                Name = p.Text.BoonName,
                ObjectBounds = p.BoonIsRing ? Bounds(-1, -1, 0, 1, 1, 0) : Bounds(-3, -9, 0, 3, 9, 1),
                ObjectEffect = L<IObjectEffectGetter>(ench.FormKey).AsNullable(),
                WorldModel = new GenderedItem<ArmorModel?>(new ArmorModel { Model = new Model { File = p.BoonMesh } }, null),
                BodyTemplate = new BodyTemplate
                {
                    FirstPersonFlags = p.BoonIsRing ? BipedObjectFlag.Ring : BipedObjectFlag.Amulet,
                    ArmorType = ArmorType.Clothing,
                    ActsLike44 = true,
                },
                Race = L<IRaceGetter>(DefaultRace).AsNullable(),
                Keywords = new()
                {
                    L<IKeywordGetter>(ArmorJewelry), L<IKeywordGetter>(VendorItemJewelry), L<IKeywordGetter>(JewelryExpensive),
                    L<IKeywordGetter>(p.BoonIsRing ? ClothingRing : ClothingNecklace), L<IKeywordGetter>(MagicDisallowEnchanting),
                },
                Description = "",
                Armature = { L<IArmorAddonGetter>(p.BoonAA) },
                Value = (uint)p.BoonValue,
                Weight = 0.25f,
            });
        }).ToArray();

        // --- Items: the ledger, relics, revel wine --------------------------------------------
        var ledger = Add(new Book(Id("BOO_Ledger"), R)
        {
            EditorID = "BOO_Ledger",
            Name = Texts.LedgerName,
            ObjectBounds = Bounds(-9, -11, -1, 9, 11, 2),
            Model = new Model { File = MeshLedger },
            BookText = Texts.Ledger,
            Keywords = new() { L<IKeywordGetter>(VendorItemBook) },
            Type = Book.BookType.BookOrTome,
            Teaches = new BookTeachesNothing { RawContent = 0xFFFFFFFF },
            Value = 0,
            Weight = 1,
            InventoryArt = L<IStaticGetter>(InvArtSpellTomeConjuration).AsNullable(),
            Description = "",
            VirtualMachineAdapter = Vmad.Adapter(Vmad.Script("BOO_LedgerBookScript", Vmad.Obj("Ledger", questLedger))),
        });

        MiscItem Relic(string edid, string name, string mesh, ObjectBounds bounds, float weight) => Add(new MiscItem(Id(edid), R)
        {
            EditorID = edid,
            Name = name,
            ObjectBounds = bounds,
            Model = new Model { File = mesh },
            PickUpSound = L<ISoundDescriptorGetter>(ITMGenericUpSD).AsNullable(),
            PutDownSound = L<ISoundDescriptorGetter>(ITMGenericDownSD).AsNullable(),
            Value = 0,
            Weight = weight,
        });
        var wager = Relic("BOO_GildedWager", "Gilded Wager", MeshGyro, Bounds(-11, -11, -12, 11, 12, 11), 2);
        var shard = Relic("BOO_NightmareShard", "Shard of Nightmare", MeshAmethyst, Bounds(-2, -2, 0, 3, 2, 1), 0.1f);

        var wine = Add(new Ingestible(Id("BOO_RevelWine"), R)
        {
            EditorID = "BOO_RevelWine",
            Name = "Sanguine's Revel Wine",
            ObjectBounds = Bounds(-7, -6, 0, 7, 6, 29),
            Model = new Model { File = MeshWine },
            Keywords = new() { L<IKeywordGetter>(VendorItemFood) },
            PickUpSound = L<ISoundDescriptorGetter>(ITMPotionUpSD).AsNullable(),
            PutDownSound = L<ISoundDescriptorGetter>(ITMPotionDownSD).AsNullable(),
            ConsumeSound = L<ISoundDescriptorGetter>(ITMPotionUse).AsNullable(),
            Weight = 0.5f,
            Value = 25,
            Flags = Ingestible.Flag.NoAutoCalc | Ingestible.Flag.FoodItem,
            Description = "",
            Effects = { Effect(AlchRestoreHealth, 15, 0), Effect(mgefRevel.FormKey, 0, 0) },
        });

        // --- Actors: Boethiah's champions, Hircine's quarry -----------------------------------
        var allButName = NpcConfiguration.TemplateFlag.Traits | NpcConfiguration.TemplateFlag.Stats | NpcConfiguration.TemplateFlag.Factions
            | NpcConfiguration.TemplateFlag.SpellList | NpcConfiguration.TemplateFlag.AIData | NpcConfiguration.TemplateFlag.AIPackages
            | NpcConfiguration.TemplateFlag.ModelAnimation | NpcConfiguration.TemplateFlag.Inventory | NpcConfiguration.TemplateFlag.Script
            | NpcConfiguration.TemplateFlag.DefPackList | NpcConfiguration.TemplateFlag.AttackData | NpcConfiguration.TemplateFlag.Keywords;

        var champions = new (string edid, string name, string shortName)[]
        {
            ("BOO_Champion01", "Brannoc Redhand", "Redhand"),
            ("BOO_Champion02", "Ulfgar Twice-Sworn", "Ulfgar"),
            ("BOO_Champion03", "Sarith the Unbowed", "Sarith"),
        }.Select(t => Add(TemplatedNpc(t.edid, t.name, t.shortName, LCharBanditBoss, NordRace, allButName))).ToList();
        var lvlChampion = Add(LeveledList("BOO_LCharChampion", champions));

        var quarry = new (string edid, string name, string shortName, FormKey template, FormKey race)[]
        {
            ("BOO_Quarry01", "Old Scar-Muzzle", "Scar-Muzzle", LCharBearAll, BearBrownRace),
            ("BOO_Quarry02", "The White Sow", "White Sow", LCharBearAll, BearBrownRace),
            ("BOO_Quarry03", "Greyfang", "Greyfang", LCharSabrecat, SabreCatRace),
            ("BOO_Quarry04", "The Silent Stalker", "Stalker", LCharSabrecat, SabreCatRace),
        }.Select(t => Add(TemplatedNpc(t.edid, t.name, t.shortName, t.template, t.race, allButName))).ToList();
        var lvlQuarry = Add(LeveledList("BOO_LCharQuarry", quarry));

        // --- Messages --------------------------------------------------------------------------
        var msgArrival = Msg("BOO_MsgArrival", Texts.Arrival);
        var msgBind = Msg("BOO_MsgBindOffer", Texts.BindOffer, Btn("Sign in blood"), Btn("Not now"), Btn("Cast the Ledger away for good"));
        var msgBound = Msg("BOO_MsgBound", Texts.Bound);
        var msgRefused = Msg("BOO_MsgRefused", Texts.Refused);
        var msgMain = Msg("BOO_MsgLedgerMenu", Texts.LedgerMenu,
            Btn("Seek a task", Cond.Global(gActive, CompareOperator.EqualTo, 0)),
            Btn("Current task", Cond.Global(gActive, CompareOperator.EqualTo, 1)),
            Btn("Favor of the princes"),
            Btn("Close the Ledger"));
        var msgPrinces = Msg("BOO_MsgPrinceMenu", Texts.PrinceMenu,
            PrinceSpecs.Select(p => Btn(p.Text.Name)).Append(Btn("Back")).ToArray());
        var msgOffers = PrinceSpecs.Select(p => Msg($"BOO_MsgOffer{p.Text.Key}", p.Text.Offer, Btn("Accept"), Btn("Decline"))).ToArray();
        var msgNoContract = Msg("BOO_MsgNoContract", Texts.NoContract);
        var msgStatus = Msg("BOO_MsgContractStatus", Texts.Status, Btn("Close the Ledger"), Btn("Abandon the task (-10 favor)"));

        // --- Controller quest -----------------------------------------------------------------
        var ledgerQuest = Add(new Quest(questLedger, R)
        {
            EditorID = "BOO_LedgerQuest",
            Name = "The Daedric Ledger",
            Flags = Quest.Flag.StartGameEnabled,
            Priority = 50,
            Type = Quest.TypeEnum.None,
            QuestFormVersion = VanillaQuestFormVersion,
        });
        ledgerQuest.VirtualMachineAdapter = Vmad.QuestAdapter(Vmad.Script("BOO_LedgerScript",
            Vmad.Obj("BOO_Bound", gBound), Vmad.Obj("BOO_ContractActive", gActive), Vmad.Obj("BOO_ContractsDone", gDone),
            Vmad.Obj("BOO_IntroMinLevel", gMinLevel), Vmad.Obj("BOO_PayMult", gPayMult),
            Vmad.Obj("BOO_FavorGain", gFavorGain), Vmad.Obj("BOO_RivalLoss", gRivalLoss),
            Vmad.ObjList("Favor", gFavor),
            Vmad.ObjList("Contracts", contractQuests),
            Vmad.ObjList("Blessings", blessings.Select(b => b.FormKey).ToArray()),
            Vmad.ObjList("Boons", boons.Select(b => b.FormKey).ToArray()),
            Vmad.ObjList("OfferMessages", msgOffers),
            Vmad.Obj("MQ101", MQ101),
            Vmad.Obj("BOO_Ledger", ledger.FormKey), Vmad.Obj("BOO_PowerLedger", powerLedger.FormKey),
            Vmad.Obj("Gold001", Gold001),
            Vmad.Obj("BOO_MsgArrival", msgArrival), Vmad.Obj("BOO_MsgBindOffer", msgBind),
            Vmad.Obj("BOO_MsgBound", msgBound), Vmad.Obj("BOO_MsgRefused", msgRefused),
            Vmad.Obj("BOO_MsgLedgerMenu", msgMain), Vmad.Obj("BOO_MsgPrinceMenu", msgPrinces),
            Vmad.Obj("BOO_MsgNoContract", msgNoContract), Vmad.Obj("BOO_MsgContractStatus", msgStatus)));

        // --- Contract quests ------------------------------------------------------------------
        for (int i = 0; i < PrinceSpecs.Length; i++)
        {
            var p = PrinceSpecs[i].Text;
            var q = ContractQuest(contractQuests[i], $"BOO_Contract{p.Key}", p.QuestName, p.StartLog);
            int type;
            uint objectiveTarget;
            switch (p.Key)
            {
                case "Dagon":
                    type = TypeKill;
                    q.Aliases.Add(LocationAlias(new[] { LocTypeBanditCamp, LocTypeForswornCamp }, RefTypeBoss, allowCleared: false));
                    q.Aliases.Add(MapMarkerAlias(optional: true));
                    q.Aliases.Add(BossAlias(RefTypeBoss));
                    objectiveTarget = AliasTarget;
                    break;
                case "Boethiah":
                    type = TypeKill;
                    q.Aliases.Add(LocationAlias(new[] { LocTypeDungeon }, RefTypeMapMarker, allowCleared: true));
                    q.Aliases.Add(MapMarkerAlias(optional: false));
                    q.Aliases.Add(CreatedAlias(lvlChampion.FormKey));
                    objectiveTarget = AliasTarget;
                    break;
                case "Namira":
                    type = TypeClear;
                    q.Aliases.Add(LocationAlias(new[] { LocTypeDraugrCrypt }, RefTypeBoss, allowCleared: false));
                    q.Aliases.Add(MapMarkerAlias(optional: true));
                    q.Aliases.Add(BossAlias(RefTypeBoss));
                    objectiveTarget = AliasTarget;
                    break;
                case "MolagBal":
                    type = TypeKill;
                    q.Aliases.Add(LocationAlias(new[] { LocTypeVampireLair }, RefTypeBoss, allowCleared: false));
                    q.Aliases.Add(MapMarkerAlias(optional: true));
                    q.Aliases.Add(BossAlias(RefTypeBoss));
                    objectiveTarget = AliasTarget;
                    break;
                case "Clavicus":
                    type = TypeFetch;
                    q.Aliases.Add(LocationAlias(new[] { LocTypeDwarvenAutomatons }, RefTypeBossContainer, allowCleared: true));
                    q.Aliases.Add(MapMarkerAlias(optional: true));
                    q.Aliases.Add(BossAlias(RefTypeBossContainer, name: "RelicContainer"));
                    q.Aliases.Add(RelicAlias(wager.FormKey));
                    objectiveTarget = AliasRelic;
                    break;
                case "Hircine":
                    type = TypeKill;
                    q.Aliases.Add(LocationAlias(new[] { LocTypeAnimalDen, LocTypeGiantCamp }, RefTypeMapMarker, allowCleared: true));
                    q.Aliases.Add(MapMarkerAlias(optional: false));
                    q.Aliases.Add(CreatedAlias(lvlQuarry.FormKey));
                    objectiveTarget = AliasTarget;
                    break;
                case "Sanguine":
                    type = TypeRevel;
                    q.Aliases.Add(LocationAlias(new[] { LocTypeInn }, null, allowCleared: true));
                    q.Aliases.Add(new QuestAlias
                    {
                        ID = AliasMapMarker, Type = QuestAlias.TypeEnum.Reference, Name = "MapMarker",
                        Flags = QuestAlias.Flag.Optional,
                        Location = new LocationAliasReference { AliasID = (int)AliasLocation, RefType = L<ILocationReferenceTypeGetter>(RefTypeLocationCenter).AsNullable() },
                    });
                    q.TextDisplayGlobals.Add(L<IGlobalGetter>(gRevelCount));
                    objectiveTarget = AliasMapMarker;
                    break;
                case "Vaermina":
                    type = TypeFetch;
                    q.Aliases.Add(LocationAlias(new[] { LocTypeWarlockLair, LocTypeHagravenNest }, RefTypeBossContainer, allowCleared: true));
                    q.Aliases.Add(MapMarkerAlias(optional: true));
                    q.Aliases.Add(BossAlias(RefTypeBossContainer, name: "RelicContainer"));
                    q.Aliases.Add(RelicAlias(shard.FormKey));
                    objectiveTarget = AliasRelic;
                    break;
                default:
                    throw new InvalidOperationException(p.Key);
            }
            q.NextAliasID = (uint)q.Aliases.Count;
            q.Objectives.Add(Objective(10, p.Objective, (int)objectiveTarget));

            var props = new List<ScriptProperty>
            {
                Vmad.Obj("Ledger", questLedger),
                Vmad.Int("Prince", i),
                Vmad.Int("ContractType", type),
                Vmad.Alias("ContractLocation", contractQuests[i], (int)AliasLocation),
                Vmad.Alias("MapMarker", contractQuests[i], (int)AliasMapMarker),
            };
            if (type is TypeKill or TypeClear) props.Add(Vmad.Alias("Target", contractQuests[i], (int)AliasTarget));
            if (type == TypeFetch)
            {
                props.Add(Vmad.Alias("Relic", contractQuests[i], (int)AliasRelic));
                props.Add(Vmad.Obj("RelicItem", p.Key == "Clavicus" ? wager.FormKey : shard.FormKey));
            }
            if (type == TypeRevel)
            {
                props.Add(Vmad.Obj("BOO_RevelCount", gRevelCount));
                props.Add(Vmad.Obj("BOO_RevelWine", wine.FormKey));
            }
            q.VirtualMachineAdapter = Vmad.QuestAdapter(Vmad.Script("BOO_ContractScript", props.ToArray()));
            // Each contract type uses only some of the shared script's properties.
            foreach (var optional in new[] { "Target", "Relic", "RelicItem", "BOO_RevelCount", "BOO_RevelWine" })
                if (!props.Any(x => x.Name == optional))
                    ExpectedUnfilled.Add($"BOO_Contract{p.Key}:BOO_ContractScript.{optional}");
        }
    }

    // ----------------------------------------------------------------------------------------
    // Record helpers
    // ----------------------------------------------------------------------------------------

    private T Add<T>(T record) where T : SkyrimMajorRecord
    {
        _mod.GetTopLevelGroup<T>().Add(record);
        return record;
    }

    private FormKey Global(string edid, float value)
    {
        var g = new GlobalFloat(Id(edid), R) { EditorID = edid, Data = value };
        _mod.Globals.Add(g);
        return g.FormKey;
    }

    private MagicEffect ScriptEffect(string edid, string name, string description, CastType castType, VirtualMachineAdapter vmad) => new(Id(edid), R)
    {
        EditorID = edid,
        Name = name,
        Flags = MagicEffect.Flag.NoDuration | MagicEffect.Flag.NoMagnitude | MagicEffect.Flag.NoArea
                | MagicEffect.Flag.Painless | MagicEffect.Flag.NoHitEvent | MagicEffect.Flag.HideInUI,
        BaseCost = 0,
        MagicSkill = ActorValue.None,
        ResistValue = ActorValue.None,
        SecondActorValue = ActorValue.None,
        Archetype = new MagicEffectArchetype { Type = MagicEffectArchetype.TypeEnum.Script, ActorValue = ActorValue.None },
        CastType = castType,
        TargetType = TargetType.Self,
        CastingSoundLevel = SoundLevel.Normal,
        Description = description,
        VirtualMachineAdapter = vmad,
    };

    private static Effect Effect(FormKey mgef, float magnitude, int duration) => new()
    {
        BaseEffect = new FormLinkNullable<IMagicEffectGetter>(mgef),
        Data = new EffectData { Magnitude = magnitude, Area = 0, Duration = duration },
    };

    private static ObjectBounds Bounds(short x1, short y1, short z1, short x2, short y2, short z2)
        => new() { First = new P3Int16(x1, y1, z1), Second = new P3Int16(x2, y2, z2) };

    private static MessageButton Btn(string text, params Condition[] conditions)
    {
        var b = new MessageButton { Text = text };
        b.Conditions.AddRange(conditions);
        return b;
    }

    private FormKey Msg(string edid, string text, params MessageButton[] buttons)
    {
        if (buttons.Length > 10) throw new InvalidOperationException($"{edid}: a message box holds at most 10 buttons");
        var m = new Message(Id(edid), R)
        {
            EditorID = edid,
            Description = text,
            Flags = Message.Flag.MessageBox,
            INAM = new byte[4],
        };
        m.MenuButtons.AddRange(buttons);
        _mod.Messages.Add(m);
        return m.FormKey;
    }

    private Npc TemplatedNpc(string edid, string name, string shortName, FormKey template, FormKey race, NpcConfiguration.TemplateFlag flags) => new(Id(edid), R)
    {
        EditorID = edid,
        Name = name,
        ShortName = shortName,
        ObjectBounds = Bounds(-22, -14, 0, 22, 14, 128),
        Template = new FormLinkNullable<INpcSpawnGetter>(template),
        Race = L<IRaceGetter>(race),
        Class = L<IClassGetter>(ClassWarrior1H),
        CombatStyle = L<ICombatStyleGetter>(CsHumanMelee).AsNullable(),
        Configuration = new NpcConfiguration
        {
            Flags = NpcConfiguration.Flag.AutoCalcStats,
            TemplateFlags = flags,
            Level = new PcLevelMult { LevelMult = 1 },
            CalcMinLevel = 1,
            CalcMaxLevel = 0,
            SpeedMultiplier = 100,
            DispositionBase = 35,
        },
        AIData = new AIData { Aggression = Aggression.Aggressive, Confidence = Confidence.Average, Assistance = Assistance.HelpsAllies, EnergyLevel = 50 },
        PlayerSkills = new PlayerSkills(),
        Height = 1,
        Weight = 50,
    };

    private LeveledNpc LeveledList(string edid, IEnumerable<Npc> npcs) => new(Id(edid), R)
    {
        EditorID = edid,
        Flags = LeveledNpc.Flag.CalculateFromAllLevelsLessThanOrEqualPlayer,
        Entries = npcs.Select(n => new LeveledNpcEntry
        {
            Data = new LeveledNpcEntryData { Level = 1, Count = 1, Reference = L<INpcSpawnGetter>(n.FormKey) },
        }).ToExtendedList(),
    };

    private Quest ContractQuest(FormKey fk, string edid, string name, string startLog)
    {
        var q = Add(new Quest(fk, R)
        {
            EditorID = edid,
            Name = name,
            Priority = 60,
            Type = Quest.TypeEnum.Daedric,
            QuestFormVersion = VanillaQuestFormVersion,
        });
        q.Stages.Add(new QuestStage { Index = 0, Flags = QuestStage.Flag.StartUpStage });
        q.Stages.Add(new QuestStage { Index = 10, LogEntries = { new QuestLogEntry { Flags = 0, Entry = startLog } } });
        q.Stages.Add(new QuestStage
        {
            Index = 100,
            LogEntries = { new QuestLogEntry { Flags = QuestLogEntry.Flag.CompleteQuest, Entry = Texts.StageDone } },
        });
        q.Stages.Add(new QuestStage
        {
            Index = 200,
            Flags = QuestStage.Flag.ShutDownStage,
            LogEntries = { new QuestLogEntry { Flags = QuestLogEntry.Flag.FailQuest, Entry = Texts.StageAbandoned } },
        });
        return q;
    }

    /// <summary>Radiant location: inside Skyrim's holds, any of <paramref name="locTypes"/>, optionally having a ref type.</summary>
    private static QuestAlias LocationAlias(FormKey[] locTypes, FormKey? requiredRefType, bool allowCleared)
    {
        var a = new QuestAlias
        {
            ID = AliasLocation,
            Type = QuestAlias.TypeEnum.Location,
            Name = "ContractLocation",
            Flags = QuestAlias.Flag.ReservesLocationOrReference | QuestAlias.Flag.StoresText
                    | (allowCleared ? QuestAlias.Flag.AllowCleared : 0),
        };
        a.Conditions.Add(Cond.InCurrentLocFormList(CompanionsRadiantAllowedHolds));
        // Keyword alternatives are OR-ed together (OR binds tighter than AND in Skyrim conditions).
        for (int i = 0; i < locTypes.Length; i++)
            a.Conditions.Add(Cond.LocationHasKeyword(locTypes[i], or: i < locTypes.Length - 1));
        if (requiredRefType is { } rt) a.Conditions.Add(Cond.LocationHasRefType(rt));
        return a;
    }

    private static QuestAlias MapMarkerAlias(bool optional) => new()
    {
        ID = AliasMapMarker, Type = QuestAlias.TypeEnum.Reference, Name = "MapMarker",
        Flags = optional ? QuestAlias.Flag.Optional : 0,
        Location = new LocationAliasReference { AliasID = (int)AliasLocation, RefType = L<ILocationReferenceTypeGetter>(RefTypeMapMarker).AsNullable() },
    };

    private static QuestAlias BossAlias(FormKey refType, string name = "Target") => new()
    {
        ID = AliasTarget, Type = QuestAlias.TypeEnum.Reference, Name = name,
        Flags = QuestAlias.Flag.ReservesLocationOrReference | QuestAlias.Flag.StoresText,
        Location = new LocationAliasReference { AliasID = (int)AliasLocation, RefType = L<ILocationReferenceTypeGetter>(refType).AsNullable() },
    };

    private static QuestAlias CreatedAlias(FormKey leveledActor) => new()
    {
        ID = AliasTarget, Type = QuestAlias.TypeEnum.Reference, Name = "Target",
        Flags = QuestAlias.Flag.StoresText,
        CreateReferenceToObject = new CreateReferenceToObject
        {
            Object = L<ISkyrimMajorRecordGetter>(leveledActor), AliasID = (int)AliasMapMarker,
            Create = CreateReferenceToObject.CreateEnum.At, Level = Level.Hard,
        },
    };

    private static QuestAlias RelicAlias(FormKey item) => new()
    {
        ID = AliasRelic, Type = QuestAlias.TypeEnum.Reference, Name = "Relic",
        Flags = QuestAlias.Flag.QuestObject | QuestAlias.Flag.StoresText,
        CreateReferenceToObject = new CreateReferenceToObject
        {
            Object = L<ISkyrimMajorRecordGetter>(item), AliasID = (int)AliasTarget,
            Create = CreateReferenceToObject.CreateEnum.In, Level = Level.Easy,
        },
    };

    private static QuestObjective Objective(ushort index, string text, int targetAlias) => new()
    {
        Index = index,
        DisplayText = text,
        Targets = { new QuestObjectiveTarget { AliasID = targetAlias } },
    };
}
