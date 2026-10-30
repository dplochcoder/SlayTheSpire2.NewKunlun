using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.ValueProps;
using NewKunlun.NewKunlunCode.Extensions;
using NewKunlun.NewKunlunCode.Localization;
using NewKunlun.NewKunlunCode.Powers;

namespace NewKunlun.NewKunlunCode.Cards;

[Pool(typeof(TokenCardPool))]
[CardLocalization(
    title: "Yingzhao's Lance",
    description: "Remove the enemy's [gold]Block[/gold].\nDeal {Damage:diff()} damage.\nApply {Electrostatic:diff()} [gold]Electrostatic[/gold]."
)]
public partial class YingzhaosLanceCard()
    : NewKunlunCard(1, CardType.Attack, CardRarity.Ancient, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new(nameof(Electrostatic), 1), new DamageVar(19, ValueProp.Move)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [
            HoverTipFactory.Static(StaticHoverTip.Block),
            HoverTipFactory.FromPower<ElectrostaticPower>(),
        ];

    protected override void OnUpgrade()
    {
        Electrostatic.UpgradeValueTo(2);
        Damage.UpgradeValueTo(25);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Target == null)
            return;

        await CreatureCmd.LoseBlock(
            choiceContext,
            cardPlay.Target,
            cardPlay.Target.Block,
            Owner.Creature
        );
        await DamageCmd
            .Attack(Damage.BaseValue)
            .FromCard(this, cardPlay)
            .Targeting(cardPlay.Target)
            .WithSlashVfx()
            .Execute(choiceContext);
        await PowerCmd.Apply<ElectrostaticPower>(
            choiceContext,
            cardPlay.Target,
            Electrostatic.IntValue,
            Owner.Creature,
            this
        );
    }
}
