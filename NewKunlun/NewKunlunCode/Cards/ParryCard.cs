using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using NewKunlun.NewKunlunCode.Character;
using NewKunlun.NewKunlunCode.Localization;
using NewKunlun.NewKunlunCode.Powers;
using NewKunlun.NewKunlunCode.Variables;

namespace NewKunlun.NewKunlunCode.Cards;

[Pool(typeof(YiCardPool))]
[CardLocalization(
    "Parry",
    "Block {Block:diff()}. If you are hit by the enemy this turn, take {InternalDamage} [gold]Internal Damage[/gold] and gain {QiCharge:plural:[gold]Qi Charge[/gold]|[gold]Qi Charges[/gold]}."
)]
public partial class ParryCard()
    : NewKunlunCard(1, CardType.Skill, CardRarity.Basic, TargetType.Self)
{
    public override bool GainsBlock => true;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new BlockVar(10M, ValueProp.Move), new InternalDamageVar(3M), new QiChargeVar(1M)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [
            HoverTipFactory.FromPower<InternalDamagePower>(),
            HoverTipFactory.FromPower<QiChargePower>(),
        ];

    protected override void OnUpgrade()
    {
        Block.UpgradeValueBy(6M);
        QiCharge.UpgradeValueBy(1M);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.GainBlock(Owner.Creature, Block, cardPlay);

        var power = await PowerCmd.Apply<ParryPower>(
            choiceContext,
            Owner.Creature,
            QiCharge.BaseValue,
            Owner.Creature,
            cardPlay.Card
        );
        power?.InternalDamage.BaseValue += InternalDamage.BaseValue;
    }
}
