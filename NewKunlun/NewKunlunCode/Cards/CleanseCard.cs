using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using NewKunlun.NewKunlunCode.Character;
using NewKunlun.NewKunlunCode.Localization;
using NewKunlun.NewKunlunCode.Powers;
using NewKunlun.NewKunlunCode.Tips;
using NewKunlun.NewKunlunCode.Variables;

namespace NewKunlun.NewKunlunCode.Cards;

[Pool(typeof(YiCardPool))]
[CardLocalization(
    title: "Cleanse",
    description: "At the end of your next {Turns:diff()} turns, heal {InternalDamageHeal:diff()} [gold]Internal Damage[/gold] and [gold]Exhaust[/gold] 1 card from your hand."
)]
public partial class CleanseCard()
    : NewKunlunCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DynamicVar(nameof(Turns), 2M), new InternalDamageHealVar(4M)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [Tip.InternalDamage(), Tip.Exhaust()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var power = await PowerCmd.Apply<CleansePower>(
            choiceContext,
            Owner.Creature,
            Turns.BaseValue,
            Owner.Creature,
            this
        );
        power?.InternalDamageHeal.BaseValue = InternalDamageHeal.BaseValue;
    }
}
