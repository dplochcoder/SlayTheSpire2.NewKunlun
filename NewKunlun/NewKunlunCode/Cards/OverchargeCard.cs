using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using NewKunlun.NewKunlunCode.Character;
using NewKunlun.NewKunlunCode.Commands;
using NewKunlun.NewKunlunCode.Extensions;
using NewKunlun.NewKunlunCode.Localization;
using NewKunlun.NewKunlunCode.Variables;

namespace NewKunlun.NewKunlunCode.Cards;

[Pool(typeof(YiCardPool))]
[CardLocalization(
    title: "Overcharge",
    description: "Take {InternalDamageSelfInflict} [gold]Internal Damage[/gold]. Gain {Energy:energyIcons()}."
)]
public partial class OverchargeCard()
    : NewKunlunCard(0, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new InternalDamageSelfInflictVar(9M), new EnergyVar(1)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [Tips.InternalDamage()];

    protected override void OnUpgrade() => Energy.UpgradeValueTo(2);

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await InternalDamageCmd.Inflict(
            choiceContext,
            Owner.Creature,
            InternalDamageSelfInflict,
            Owner.Creature,
            this
        );
        await PlayerCmd.GainEnergy(Energy.BaseValue, Owner);
    }
}
