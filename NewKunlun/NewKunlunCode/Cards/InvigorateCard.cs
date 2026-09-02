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
using NewKunlun.NewKunlunCode.Powers;
using NewKunlun.NewKunlunCode.Variables;

namespace NewKunlun.NewKunlunCode.Cards;

[Pool(typeof(YiCardPool))]
[CardLocalization(
    title: "Invigorate",
    description: "Gain {Strength:diff()} [gold]Strength[/gold]. Take {InternalDamageInflict:diff()} [gold]Internal Damage[/gold]."
)]
public partial class InvigorateCard()
    : NewKunlunCard(1, CardType.Power, CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DynamicVar(nameof(Strength), 4M), new InternalDamageInflictVar(16M)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [Tips.Power<StrengthPower>(), Tips.Power<InternalDamagePower>()];

    protected override void OnUpgrade() => Strength.UpgradeValueTo(6M);

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
            InternalDamageInflict,
            Owner.Creature,
            this
        );
    }
}
