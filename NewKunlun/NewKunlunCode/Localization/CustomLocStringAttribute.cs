namespace NewKunlun.NewKunlunCode.Localization;

public enum LocTable
{
    Achievements,
    Acts,
    Afflictions,
    Ancients,
    Ascension,
    Badges,
    Bestiary,
    CardKeywords,
    CardLibrary,
    CardRewardUi,
    CardSelection,
    Cards,
    Characters,
    CombatMessages,
    Credits,
    Enchantments,
    Encounters,
    Epochs,
    Eras,
    Events,
    Extensions,
    Ftues,
    GameModes,
    GameOverScreen,
    GameplayUi,
    InspectRelicScreen,
    Intents,
    MainMenuUi,
    Map,
    MerchantRoom,
    Modifiers,
    Monsters,
    Orbs,
    PotionLab,
    Potions,
    Powers,
    RelicCollection,
    Relics,
    RestSiteUi,
    RichPresence,
    RunHistory,
    SettingsUi,
    StaticHoverTips,
    StatsScreen,
    Timeline,
    Vfx,
}

[AttributeUsage(AttributeTargets.Property)]
public sealed class CustomLocStringAttribute(LocTable table, string description) : Attribute
{
    public LocTable Table { get; } = table;
    public string Description { get; } = description;
}
