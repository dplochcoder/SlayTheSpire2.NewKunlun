using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Saves.Runs;
using MegaCrit.Sts2.Core.ValueProps;
using NewKunlun.NewKunlunCode.Extensions;
using NewKunlun.NewKunlunCode.Localization;

namespace NewKunlun.NewKunlunCode.Cards.Ancients;

[Pool(typeof(EventCardPool))]
[CardLocalization(
    title: "Ancestral Sword",
    description: "Gain {Block} [gold]Block[/gold].\nDeal {Damage} damage.\nOnce per turn, if you have no [gold]Block[/gold] cards in your hand, put this card into your hand."
)]
public partial class AncestralSwordCard()
    : NewKunlunCard(1, CardType.Attack, CardRarity.Ancient, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DamageVar(21, ValueProp.Move), new BlockVar(12, ValueProp.Move)];

    public override bool GainsBlock => true;

    protected override void OnUpgrade()
    {
        Damage.UpgradeValueTo(28);
        Block.UpgradeValueTo(16);
    }

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.Static(StaticHoverTip.Block)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Target == null)
            return;

        await CreatureCmd.GainBlock(Owner.Creature, Block, cardPlay);
        await DamageCmd
            .Attack(Damage.BaseValue)
            .FromCard(this, cardPlay)
            .WithSlashVfx()
            .Targeting(cardPlay.Target)
            .Execute(choiceContext);
    }

    public enum YoinkState
    {
        WaitingForHandDraw,
        NotYoinkedThisTurn,
        YoinkedThisTurn,
    }

    [SavedProperty]
    public YoinkState State
    {
        get;
        set
        {
            AssertMutable();
            field = value;
        }
    } = YoinkState.WaitingForHandDraw;

    public override Task BeforeSideTurnStart(
        PlayerChoiceContext choiceContext,
        CombatSide side,
        IReadOnlyList<Creature> participants,
        ICombatState combatState
    )
    {
        if (side != Owner.Creature.Side)
            return Task.CompletedTask;

        State = YoinkState.WaitingForHandDraw;
        return Task.CompletedTask;
    }

    public override async Task AfterPlayerTurnStart(
        PlayerChoiceContext choiceContext,
        Player player
    )
    {
        if (player != Owner)
            return;

        State = YoinkState.NotYoinkedThisTurn;
        await CheckYoink();
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (
            choiceContext.ModelStack == null
            || choiceContext.ModelStack.OfType<CardModel>().Count() != 1
        )
            return;

        if (State == YoinkState.NotYoinkedThisTurn)
            await CheckYoink();
    }

    private async Task CheckYoink()
    {
        var hand = PileType.Hand.GetPile(Owner);
        if (
            Pile?.Type != PileType.Hand
            && hand.Cards.All(c => !c.GainsBlock)
            && hand.Cards.Count < Owner.MaxHandSize
        )
        {
            State = YoinkState.YoinkedThisTurn;
            await CardPileCmd.Add(this, PileType.Hand.GetPile(Owner));
        }
    }
}
