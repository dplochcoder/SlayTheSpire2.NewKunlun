using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using NewKunlun.NewKunlunCode.Character;
using NewKunlun.NewKunlunCode.Extensions;
using NewKunlun.NewKunlunCode.Localization;
using NewKunlun.NewKunlunCode.Powers;
using ParryPower = NewKunlun.NewKunlunCode.Powers.ParryPower;

namespace NewKunlun.NewKunlunCode.Cards;

[Pool(typeof(YiCardPool))]
[CardLocalization(
    title: "Unbounded Counter",
    description: "At the end of your next turn, gain {Block:diff()} block and {Parry:diff()} [gold]Parry[/gold]."
)]
public partial class UnboundedCounterCard()
    : NewKunlunCard(2, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    public override bool GainsBlock => true;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new BlockVar(28M, ValueProp.Move), new DynamicVar(nameof(Parry), 2M)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [Tips.Power<ParryPower>()];

    protected override void OnUpgrade() => Block.UpgradeValueTo(42M);

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<BlockNextTurnPower>(
            choiceContext,
            Owner.Creature,
            await Owner.Creature.ComputeBlockGain(Block, cardPlay),
            Owner.Creature,
            this
        );
        await PowerCmd.Apply<ParryNextTurnPower>(
            choiceContext,
            Owner.Creature,
            Parry.BaseValue,
            Owner.Creature,
            this
        );
    }
}
