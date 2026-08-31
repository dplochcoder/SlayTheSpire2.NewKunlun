using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using NewKunlun.NewKunlunCode.Character;
using NewKunlun.NewKunlunCode.Extensions;
using NewKunlun.NewKunlunCode.Localization;

namespace NewKunlun.NewKunlunCode.Cards;

[Pool(typeof(YiCardPool))]
[CardLocalization(
    title: "Swift Dash",
    description: "Gain {Block:diff()} block {Times:diff()} times."
)]
public partial class SwiftDashCard()
    : NewKunlunCard(1, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    public override bool GainsBlock => true;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new BlockVar(2M, ValueProp.Move), new DynamicVar(nameof(Times), 3)];

    protected override void OnUpgrade() => Times.UpgradeValueTo(4M);

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        for (var i = 0; i < Times.BaseValue; i++)
            await CreatureCmd.GainBlock(Owner.Creature, Block, cardPlay, fast: true);
    }
}
