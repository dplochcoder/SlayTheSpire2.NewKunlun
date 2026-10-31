using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using NewKunlun.NewKunlunCode.Character;
using NewKunlun.NewKunlunCode.Commands;
using NewKunlun.NewKunlunCode.Extensions;
using NewKunlun.NewKunlunCode.Localization;
using NewKunlun.NewKunlunCode.Variables;

namespace NewKunlun.NewKunlunCode.Cards.Yi;

[Pool(typeof(YiCardPool))]
[CardLocalization(
    title: "Patience",
    description: "Gain {Block:diff()} [gold]Block[/gold].\nHeal {InternalDamageHeal:diff()} [gold]Internal Damage[/gold]."
)]
public partial class PatienceCard()
    : NewKunlunCard(1, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    public override bool GainsBlock => true;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new BlockVar(6M, ValueProp.Move), new InternalDamageHealVar(5M)];

    protected override void OnUpgrade()
    {
        Block.UpgradeValueTo(9M);
        InternalDamageHeal.UpgradeValueTo(7M);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.GainBlock(Owner.Creature, Block, cardPlay);
        await InternalDamageCmd.Heal(
            choiceContext,
            Owner.Creature,
            InternalDamageHeal,
            Owner.Creature,
            this
        );
    }
}
