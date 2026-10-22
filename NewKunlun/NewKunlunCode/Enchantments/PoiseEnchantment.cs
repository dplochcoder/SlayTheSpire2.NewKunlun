using System.Buffers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models.Powers;
using NewKunlun.NewKunlunCode.Localization;
using NewKunlun.NewKunlunCode.Tips;

namespace NewKunlun.NewKunlunCode.Enchantments;

[EnchantmentLocalization(
    title: "Poise",
    description: "When played, grant 1 [gold]Strength[/gold].",
    extraCardText: "Gain {Amount} [gold]Strength[/gold]."
)]
public class PoiseEnchantment : NewKunlunEnchantment
{
    public override bool CanEnchantCardType(CardType cardType) => cardType == CardType.Skill;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [Tip.Strength()];

    public override bool HasExtraCardText => true;

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card != Card)
            return;

        await PowerCmd.Apply<StrengthPower>(
            choiceContext,
            Card.Owner.Creature,
            Amount,
            Card.Owner.Creature,
            Card
        );
    }
}
