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
using NewKunlun.NewKunlunCode.Tips;

namespace NewKunlun.NewKunlunCode.Cards;

[Pool(typeof(YiCardPool))]
[CardLocalization(
    title: "Perfect Parry",
    description: "Gain {Block:diff()} [gold]Block[/gold]. Draw 1 card. Gain 1 [gold]Parry[/gold]."
)]
public partial class PerfectParryCard()
    : NewKunlunCard(0, CardType.Skill, CardRarity.Ancient, TargetType.Self)
{
    public override bool GainsBlock => true;

    public override bool IsParryCard => true;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(18M, ValueProp.Move)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [Tip.Parry()];

    protected override void OnUpgrade() => AddKeyword(CardKeyword.Retain);

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.GainBlock(Owner.Creature, Block, cardPlay);
        await CardPileCmd.Draw(choiceContext, Owner);
        await PowerCmd.Apply<ParryPower>(choiceContext, Owner.Creature, 1M, Owner.Creature, this);
    }
}
