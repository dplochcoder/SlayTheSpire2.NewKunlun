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
using NewKunlun.NewKunlunCode.Tips;

namespace NewKunlun.NewKunlunCode.Cards;

[Pool(typeof(YiCardPool))]
[CardLocalization(
    title: "Imperfect Parry",
    description: "Gain {Block:diff()} block. Gain {Parry:diff()} [gold]Parry[/gold]. Take {Imperfect:diff()} [gold]Imperfect[/gold]."
)]
public partial class ImperfectParryCard()
    : NewKunlunCard(1, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    public override bool IsParryCard => true;

    public override bool GainsBlock => true;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new BlockVar(13M, ValueProp.Move),
            new DynamicVar(nameof(Parry), 1M),
            new DynamicVar(nameof(Imperfect), 6M),
        ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [Tip.Parry(), Tip.Imperfect()];

    protected override void OnUpgrade()
    {
        Block.UpgradeValueTo(16M);
        Imperfect.UpgradeValueTo(4M);
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
        await PowerCmd.Apply<ImperfectPower>(
            choiceContext,
            Owner.Creature,
            Imperfect.BaseValue,
            Owner.Creature,
            this
        );
    }
}
