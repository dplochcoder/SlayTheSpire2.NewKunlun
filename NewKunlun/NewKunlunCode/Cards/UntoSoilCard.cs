using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Saves.Runs;
using MegaCrit.Sts2.Core.ValueProps;
using NewKunlun.NewKunlunCode.Character;
using NewKunlun.NewKunlunCode.Extensions;
using NewKunlun.NewKunlunCode.Localization;
using NewKunlun.NewKunlunCode.Tips;

namespace NewKunlun.NewKunlunCode.Cards;

// FIXME: Change rarity when reward is implemented.
[Pool(typeof(YiCardPool))]
[CardLocalization(
    title: "UntoSoil",
    description: "Deal {Damage:diff()} damage.\nIf [glow]Fatal[/glow] on an [glow]Elite[/glow] or [glow]Boss[/glow], harvest a [gold]Tao Fruit[/gold] at the end of combat."
)]
public partial class UntoSoilCard()
    : NewKunlunCard(2, CardType.Attack, CardRarity.Token, TargetType.AnyEnemy)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DamageVar(20M, ValueProp.Move)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [Tip.Fatal()];

    protected override void OnUpgrade() => Damage.UpgradeValueTo(30M);

    [SavedProperty]
    public int Successes { get; set; } = 0;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        bool eligible = RunState?.CurrentRoom?.RoomType is RoomType.Elite or RoomType.Boss;
        var fatal = await DamageCmd
            .Attack(Damage.BaseValue)
            .FromCard(this, cardPlay)
            .WithHeavySlashVfx()
            .Targeting(cardPlay.Target!)
            .ExecuteAndCheckFatal(choiceContext);

        if (fatal.Count > 0 && eligible)
            Successes++;
    }

    public override Task AfterCombatEnd(CombatRoom room)
    {
        // FIXME: Implement reward.
        return Task.CompletedTask;
    }
}
