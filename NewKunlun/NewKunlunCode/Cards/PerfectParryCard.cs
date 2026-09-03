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
    description: "Gain {Block:diff()} block. Gain {Parry} [gold]Parry[/gold]."
)]
public partial class PerfectParryCard()
    : NewKunlunCard(1, CardType.Skill, CardRarity.Ancient, TargetType.Self)
{
    public override bool GainsBlock => true;

    public override bool IsParryCard => true;

    public override IEnumerable<CardKeyword> CanonicalKeywords => [];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new BlockVar(18M, ValueProp.Move), new DynamicVar(nameof(Parry), 2M)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [Tip.Parry()];

    protected override void OnUpgrade()
    {
        Block.UpgradeValueBy(7M);
        AddKeyword(CardKeyword.Retain);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.GainBlock(Owner.Creature, Block, cardPlay);
        await PowerCmd.Apply<ParryPower>(
            choiceContext,
            Owner.Creature,
            Parry.BaseValue,
            Owner.Creature,
            this
        );
    }
}
