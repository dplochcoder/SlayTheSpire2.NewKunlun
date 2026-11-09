using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using NewKunlun.NewKunlunCode.Enchantments;
using NewKunlun.NewKunlunCode.Extensions;
using NewKunlun.NewKunlunCode.Localization;

namespace NewKunlun.NewKunlunCode.Cards.Ancients;

[Pool(typeof(RegentCardPool))]
[CardLocalization(
    title: "Convert",
    description: "Gain {Stars:starIcons()}.\nAll cards in your hand gain {Reverence:diff()} [gold]Reverent[/gold]."
)]
public partial class ConvertCard()
    : NewKunlunCard(1, CardType.Skill, CardRarity.Ancient, TargetType.None)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new StarsVar(2), new(nameof(Reverence), 1)];

    protected override void OnUpgrade()
    {
        Stars.UpgradeValueTo(3);
        Reverence.UpgradeValueTo(2);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PlayerCmd.GainStars(Stars.IntValue, Owner);

        foreach (
            var card in PileType
                .Hand.GetPile(Owner)
                .Cards.Where(ModelDb.Enchantment<ReverentEnchantment>().CanEnchant)
        )
            CardCmd.Enchant<ReverentEnchantment>(card, Reverence.IntValue);
    }
}
