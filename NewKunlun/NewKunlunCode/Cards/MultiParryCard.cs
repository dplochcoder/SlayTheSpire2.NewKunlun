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
using NewKunlun.NewKunlunCode.Tips;

namespace NewKunlun.NewKunlunCode.Cards;

[Pool(typeof(YiCardPool))]
[CardLocalization(
    title: "Multi Parry",
    description: "Gain {Block:diff()} [gold]Block[/gold]. Gain {Parry:diff()} [gold]Parry[/gold]."
)]
public partial class MultiParryCard()
    : NewKunlunCard(1, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    public override bool GainsBlock => true;

    public override bool IsParryCard => true;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new BlockVar(8M, ValueProp.Move), new DynamicVar(nameof(Parry), 2M)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [Tip.Parry()];

    protected override void OnUpgrade()
    {
        Block.UpgradeValueTo(11M);
        Parry.UpgradeValueTo(3M);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.GainBlock(Owner.Creature, Block, cardPlay);
        await ParryCmd.GainParry(
            choiceContext,
            Owner.Creature,
            Parry.BaseValue,
            Owner.Creature,
            this
        );
    }
}
