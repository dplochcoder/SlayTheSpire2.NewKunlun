using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using NewKunlun.NewKunlunCode.Character;
using NewKunlun.NewKunlunCode.Commands;
using NewKunlun.NewKunlunCode.Extensions;
using NewKunlun.NewKunlunCode.Localization;
using NewKunlun.NewKunlunCode.Tips;
using NewKunlun.NewKunlunCode.Variables;

namespace NewKunlun.NewKunlunCode.Cards;

[Pool(typeof(YiCardPool))]
[CardLocalization(
    title: "Invigorate",
    description: "Gain {Strength:diff()} [gold]Strength[/gold]. Take {InternalDamageSelfInflict:inverseDiff()} [gold]Internal Damage[/gold]."
)]
public partial class InvigorateCard()
    : NewKunlunCard(1, CardType.Power, CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DynamicVar(nameof(Strength), 5M), new InternalDamageSelfInflictVar(16M)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [Tip.Strength(), Tip.InternalDamage()];

    protected override void OnUpgrade() => Strength.UpgradeValueTo(7M);

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<StrengthPower>(
            choiceContext,
            Owner.Creature,
            Strength.BaseValue,
            Owner.Creature,
            this
        );
        await InternalDamageCmd.Inflict(
            choiceContext,
            Owner.Creature,
            InternalDamageSelfInflict,
            Owner.Creature,
            this
        );
    }
}
