using BaseLib.Abstracts;
using BaseLib.Utils.NodeFactories;
using Godot;
using MegaCrit.Sts2.Core.Entities.Characters;
using MegaCrit.Sts2.Core.Models;
using NewKunlun.NewKunlunCode.Cards;
using NewKunlun.NewKunlunCode.Extensions;
using NewKunlun.NewKunlunCode.Relics;

namespace NewKunlun.NewKunlunCode.Character;

public class Yi : PlaceholderCharacterModel
{
    public const string CharacterId = "Yi";

    public static readonly Color Color = new("e7ae33");

    public override Color NameColor => Color;
    public override CharacterGender Gender => CharacterGender.Masculine;
    public override int StartingHp => 63;
    public override int StartingGold => 130;

    public override IEnumerable<CardModel> StartingDeck =>
        [
            ModelDb.Card<StrikeYiCard>(),
            ModelDb.Card<StrikeYiCard>(),
            ModelDb.Card<StrikeYiCard>(),
            ModelDb.Card<StrikeYiCard>(),
            ModelDb.Card<DefendYiCard>(),
            ModelDb.Card<DefendYiCard>(),
            ModelDb.Card<DefendYiCard>(),
            ModelDb.Card<DefendYiCard>(),
            ModelDb.Card<ParryCard>(),
            ModelDb.Card<TalismanDashCard>(),
        ];

    public override IReadOnlyList<RelicModel> StartingRelics => [ModelDb.Relic<JadeSystemRelic>()];

    public override CardPoolModel CardPool => ModelDb.CardPool<YiCardPool>();
    public override RelicPoolModel RelicPool => ModelDb.RelicPool<YiRelicPool>();
    public override PotionPoolModel PotionPool => ModelDb.PotionPool<YiPotionPool>();

    /*  PlaceholderCharacterModel will utilize placeholder basegame assets for most of your character assets until you
    override all the other methods that define those assets.
    These are just some of the simplest assets, given some placeholders to differentiate your character with.
    You don't have to, but you're suggested to rename these images. */
    public override Control CustomIcon
    {
        get
        {
            var icon = NodeFactory<Control>.CreateFromResource(CustomIconTexturePath);
            icon.SetAnchorsAndOffsetsPreset(Control.LayoutPreset.FullRect);
            return icon;
        }
    }
    public override string CustomIconTexturePath =>
        "character_icon_char_name.png".CharacterUiPath();
    public override string CustomCharacterSelectIconPath =>
        "char_select_char_name.png".CharacterUiPath();
    public override string CustomCharacterSelectLockedIconPath =>
        "char_select_char_name_locked.png".CharacterUiPath();
    public override string CustomMapMarkerPath => "map_marker_char_name.png".CharacterUiPath();
}
