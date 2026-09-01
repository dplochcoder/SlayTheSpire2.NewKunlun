using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using NewKunlun.NewKunlunCode.Character;
using NewKunlun.NewKunlunCode.Commands;
using NewKunlun.NewKunlunCode.Extensions;
using NewKunlun.NewKunlunCode.Localization;
using NewKunlun.NewKunlunCode.Powers;
using NewKunlun.NewKunlunCode.Variables;

namespace NewKunlun.NewKunlunCode.Cards;

[Pool(typeof(YiCardPool))]
[CardLocalization(
    title: "Patch Up",
    description: "Gain {Block:diff()} block. Heal {HealHP:diff()} HP. Heal {HealInternalDamage:diff()} [gold]Internal Damage[/gold]."
)]
public partial class PatchUpCard()
    : NewKunlunCard(0, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    public override bool GainsBlock => true;

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new BlockVar(2M, ValueProp.Move),
            new DynamicVar(nameof(HealHP), 2M),
            new InternalDamageVar(nameof(HealInternalDamage), 2M),
        ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [Tips.Power<InternalDamagePower>()];

    protected override void OnUpgrade()
    {
        Block.UpgradeValueTo(5M);
        HealHP.UpgradeValueTo(3M);
        HealInternalDamage.UpgradeValueTo(3M);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.GainBlock(Owner.Creature, Block, cardPlay);
        await CreatureCmd.Heal(Owner.Creature, HealHP.BaseValue);
        await InternalDamageCmd.Heal(
            choiceContext,
            Owner.Creature,
            HealInternalDamage.BaseValue,
            Owner.Creature,
            this
        );
    }
}
