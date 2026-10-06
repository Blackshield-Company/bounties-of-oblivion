using Mutagen.Bethesda.Plugins;

namespace BountiesOfOblivion.Generator;

/// <summary>
/// Vanilla Skyrim.esm records this mod links to. Only FormIDs are referenced;
/// no Bethesda data or assets are copied into the plugin or the repo.
/// FormIDs were looked up with the Mutagen.Bethesda.FormKeys.SkyrimSE package.
/// </summary>
internal static class Vanilla
{
    public static readonly ModKey Skyrim = ModKey.FromFileName("Skyrim.esm");
    private static FormKey K(uint id) => new(Skyrim, id);

    // Actors / races / classes
    public static readonly FormKey NordRace = K(0x013746);
    public static readonly FormKey BearBrownRace = K(0x0131E7);
    public static readonly FormKey SabreCatRace = K(0x013200);
    public static readonly FormKey DefaultRace = K(0x000019);
    public static readonly FormKey ClassWarrior1H = K(0x013176);       // CombatWarrior1H
    public static readonly FormKey CsHumanMelee = K(0x068849);         // csHumanMeleeLvl3
    public static readonly FormKey LCharBanditMeleeAny = K(0x03DECD);
    public static readonly FormKey LCharBanditBoss = K(0x03DF16);
    public static readonly FormKey LCharBearAll = K(0x042266);
    public static readonly FormKey LCharSabrecat = K(0x0FE2D5);

    // Quests
    public static readonly FormKey MQ101 = K(0x03372B);

    // Location data
    public static readonly FormKey LocTypeBanditCamp = K(0x0130DF);
    public static readonly FormKey LocTypeForswornCamp = K(0x0130EE);
    public static readonly FormKey LocTypeDungeon = K(0x0130DB);
    public static readonly FormKey LocTypeDraugrCrypt = K(0x0130E2);
    public static readonly FormKey LocTypeVampireLair = K(0x0130EB);
    public static readonly FormKey LocTypeDwarvenAutomatons = K(0x0130E3);
    public static readonly FormKey LocTypeWarlockLair = K(0x0130EC);
    public static readonly FormKey LocTypeHagravenNest = K(0x0130E6);
    public static readonly FormKey LocTypeAnimalDen = K(0x0130DE);
    public static readonly FormKey LocTypeGiantCamp = K(0x0130E5);
    public static readonly FormKey LocTypeInn = K(0x01CB87);
    public static readonly FormKey RefTypeBoss = K(0x0130F7);
    public static readonly FormKey RefTypeBossContainer = K(0x0130F8);
    public static readonly FormKey RefTypeMapMarker = K(0x10F63C);
    public static readonly FormKey RefTypeLocationCenter = K(0x01BDF1);  // LocationCenterMarker
    public static readonly FormKey CompanionsRadiantAllowedHolds = K(0x0F9073); // keeps contracts inside Skyrim's holds

    // Items
    public static readonly FormKey Gold001 = K(0x00000F);

    // Keywords
    public static readonly FormKey ArmorJewelry = K(0x06BBE9);
    public static readonly FormKey VendorItemJewelry = K(0x08F95A);
    public static readonly FormKey JewelryExpensive = K(0x0A8664);
    public static readonly FormKey ClothingNecklace = K(0x10CD0A);
    public static readonly FormKey ClothingRing = K(0x10CD09);
    public static readonly FormKey VendorItemBook = K(0x0937A2);
    public static readonly FormKey VendorItemFood = K(0x08CDEA);
    public static readonly FormKey MagicDisallowEnchanting = K(0x0C27BD);

    // Armor addons (worn jewelry meshes) and equip types
    public static readonly FormKey GoldAmuletRubyAA = K(0x091720);
    public static readonly FormKey GoldAmuletGemsAA = K(0x09171F);
    public static readonly FormKey SilverAmuletEmeraldAA = K(0x09171E);
    public static readonly FormKey SilverAmuletAA = K(0x09171A);
    public static readonly FormKey RingSilverAmethystAA = K(0x08E840);
    public static readonly FormKey RingGoldSapphireAA = K(0x08E83E);
    public static readonly FormKey RingGoldEmeraldAA = K(0x08E83D);
    public static readonly FormKey RingGoldDiamondAA = K(0x08E83C);
    public static readonly FormKey EquipVoice = K(0x025BEE);
    public static readonly FormKey EquipEitherHand = K(0x013F44);

    // Sounds / statics
    public static readonly FormKey ITMGenericUpSD = K(0x03C7BA);
    public static readonly FormKey ITMGenericDownSD = K(0x03C7B9);
    public static readonly FormKey ITMPotionUpSD = K(0x03EDBD);
    public static readonly FormKey ITMPotionDownSD = K(0x03EDC0);
    public static readonly FormKey ITMPotionUse = K(0x0B6435);
    public static readonly FormKey InvArtSpellTomeConjuration = K(0x02FBB6);

    // Magic effects reused for blessings (abilities) ...
    public static readonly FormKey AbResistFire = K(0x024314);
    public static readonly FormKey AbResistFrost = K(0x024315);
    public static readonly FormKey AbFortifyHealth = K(0x0A1A3F);
    public static readonly FormKey AbFortifyStamina = K(0x091F77);
    public static readonly FormKey AbResistDisease = K(0x0E40D3);
    public static readonly FormKey AbResistPoison = K(0x0AA024);
    public static readonly FormKey AbFortifyCarryWeight = K(0x10CED9);
    public static readonly FormKey AbFortifyStaminaRate = K(0x105F21);
    public static readonly FormKey AbFortifyMagickaRate = K(0x05312B);
    // ... and boon enchantments (constant effect, self)
    public static readonly FormKey EnchFortifyDestructionConstantSelf = K(0x07A0F6);
    public static readonly FormKey EnchFortifyOneHandedConstantSelf = K(0x07A0FF);
    public static readonly FormKey EnchFortifySneakConstantSelf = K(0x07A103);
    public static readonly FormKey EnchFortifyTwoHandedConstantSelf = K(0x07A106);
    public static readonly FormKey EnchFortifySpeechcraftConstantSelf = K(0x07A104);
    public static readonly FormKey EnchFortifyArcheryConstantSelf = K(0x07A0FE);
    public static readonly FormKey EnchFortifyHealRateConstantSelf = K(0x07A0F8);
    public static readonly FormKey EnchFortifyIllusionConstantSelf = K(0x07A0FA);
    // ... and the revel wine
    public static readonly FormKey AlchRestoreHealth = K(0x03EB15);

    // Vanilla mesh paths (referenced by path only; the meshes ship with the game)
    public const string MeshLedger = @"Clutter\Books\SpellTomeConjurationLowPoly.nif";
    public const string MeshWine = @"Clutter\Wine\WineBottle02B.nif";
    public const string MeshGyro = @"Clutter\Dwemer\DwarvenGyro.nif";
    public const string MeshAmethyst = @"Clutter\Gemstones\Amethyst.nif";
    public const string MeshAmuletGoldRuby = @"Armor\AmuletsandRings\GoldAmuletRubyGO.nif";
    public const string MeshAmuletGoldGems = @"Armor\AmuletsandRings\GoldAmuletGemsGO.nif";
    public const string MeshAmuletSilverEmerald = @"Armor\AmuletsandRings\SilverAmuletEmeraldGO.nif";
    public const string MeshAmuletSilver = @"Armor\AmuletsandRings\SilverAmuletGO.nif";
    public const string MeshRingSilverAmethyst = @"Armor\AmuletsandRings\SilverRingAmethystGO.nif";
    public const string MeshRingGoldSapphire = @"Armor\AmuletsandRings\GoldRingSapphireGO.nif";
    public const string MeshRingGoldEmerald = @"Armor\AmuletsandRings\GoldRingEmeraldGO.nif";
    public const string MeshRingGoldDiamond = @"Armor\AmuletsandRings\GoldRingDiamondGO.nif";
}
