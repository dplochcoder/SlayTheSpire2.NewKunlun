using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using NewKunlun.NewKunlunCode.Character;
using NewKunlun.NewKunlunCode.Extensions;
using NewKunlun.NewKunlunCode.Localization;
using NewKunlun.NewKunlunCode.Powers;

namespace NewKunlun.NewKunlunCode.Cards;

[Pool(typeof(YiCardPool))]
[CardLocalization(
    title: "Parrying Strike",
    description: "Deal {Damage:diff()} damage. Gain {Block:diff()} block. The next [gold]Parry Card[/gold] you play is free."
)]
public partial class ParryingStrike()
    : NewKunlunCard(2, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
{
    public override bool GainsBlock => true;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DamageVar(11M, ValueProp.Move), new BlockVar(3M, ValueProp.Move)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [Tips.ParryCardKeyword()];

    protected override void OnUpgrade()
    {
        Damage.UpgradeValueTo(15M);
        Block.UpgradeValueTo(5M);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await DamageCmd
            .Attack(Damage.BaseValue)
            .FromCard(this, cardPlay)
            .WithSlashVfx()
            .Targeting(cardPlay.Target!)
            .Execute(choiceContext);
        await CreatureCmd.GainBlock(Owner.Creature, Block, cardPlay);
        await PowerCmd.Apply<ParryingStrikePower>(
            choiceContext,
            Owner.Creature,
            1M,
            Owner.Creature,
            this
        );
    }
}
