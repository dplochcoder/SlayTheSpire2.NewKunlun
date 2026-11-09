using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;
using NewKunlun.NewKunlunCode.Extensions;
using NewKunlun.NewKunlunCode.Localization;

namespace NewKunlun.NewKunlunCode.Cards.Ancients;

[Pool(typeof(NecrobinderCardPool))]
[CardLocalization(
    title: "Growth",
    description: "[gold]Summon[/gold] {Summon:diff()}.\nIncrease [gold]Summon[/gold] amount by {Increase:diff()} this combat."
)]
public partial class GrowthCard()
    : NewKunlunCard(1, CardType.Skill, CardRarity.Ancient, TargetType.None)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new SummonVar(8), new(nameof(Increase), 2)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.Static(StaticHoverTip.SummonStatic)];

    protected override void OnUpgrade()
    {
        Summon.UpgradeValueBy(2);
        Increase.UpgradeValueTo(3);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await OstyCmd.Summon(choiceContext, Owner, Summon.IntValue, this);
        Summon.BaseValue += Increase.IntValue;
    }
}
