using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using NewKunlun.NewKunlunCode.Character;
using NewKunlun.NewKunlunCode.Localization;
using NewKunlun.NewKunlunCode.Powers;
using NewKunlun.NewKunlunCode.Tips;
using NewKunlun.NewKunlunCode.Variables;

namespace NewKunlun.NewKunlunCode.Cards;

[Pool(typeof(YiCardPool))]
[CardLocalization(
    title: "Crouch",
    description: "Gain {Block:diff()} [gold]Block[/gold].\nNext turn, gain {Energy:energyIcons()} and pull {TalismanDash:cardName()} into your hand."
)]
public partial class CrouchCard()
    : NewKunlunCard(2, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    public override bool GainsBlock => true;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new BlockVar(10M, ValueProp.Move),
            new EnergyVar(1),
            new TalismanDashVar<CrouchCard>(card =>
                TalismanDashCard.IsUpgradedAnywhere(card.Owner)
            ),
        ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [Tip.TalismanDashCard(Owner)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.GainBlock(Owner.Creature, Block, cardPlay);
        await PowerCmd.Apply<EnergyNextTurnPower>(
            choiceContext,
            Owner.Creature,
            1M,
            Owner.Creature,
            this
        );
        await PowerCmd.Apply<CrouchPower>(choiceContext, Owner.Creature, 1M, Owner.Creature, this);
    }
}
