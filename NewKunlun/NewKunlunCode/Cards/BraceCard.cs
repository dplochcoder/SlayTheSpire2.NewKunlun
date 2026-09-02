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
    title: "Brace",
    description: "Gain {Block:diff()} block. Take {Imperfect:diff()} [gold]Imperfect[/gold]."
)]
public partial class BraceCard()
    : NewKunlunCard(2, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    public override bool GainsBlock => true;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new BlockVar(26M, ValueProp.Move), new DynamicVar(nameof(Imperfect), 12M)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [Tips.Imperfect()];

    protected override void OnUpgrade() => Block.UpgradeValueTo(34M);

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.GainBlock(Owner.Creature, Block, cardPlay);
        await PowerCmd.Apply<ImperfectPower>(
            choiceContext,
            Owner.Creature,
            Imperfect.BaseValue,
            Owner.Creature,
            this
        );
    }
}
