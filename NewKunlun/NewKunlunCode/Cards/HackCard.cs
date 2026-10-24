using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Saves.Runs;
using NewKunlun.NewKunlunCode.Character;
using NewKunlun.NewKunlunCode.Commands;
using NewKunlun.NewKunlunCode.Extensions;
using NewKunlun.NewKunlunCode.Localization;
using NewKunlun.NewKunlunCode.Powers;
using NewKunlun.NewKunlunCode.Tips;
using NewKunlun.NewKunlunCode.Variables;

namespace NewKunlun.NewKunlunCode.Cards;

[Pool(typeof(YiCardPool))]
[CardLocalization(
    title: "Hack",
    description: "After {PlaysRemaining} more {PlaysRemaining:plural:play|plays}, [gold]Purge[/gold] this card and obtain {Relics:diff()} relics and {Gold:diff()} gold at the end of combat."
)]
public partial class HackCard()
    : NewKunlunCard(2, CardType.Quest, CardRarity.Quest, TargetType.None)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords =>
        [CardKeyword.Ethereal, CardKeyword.Exhaust];

    private const int TotalPlays = 4;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new DynamicVar(nameof(PlaysRemaining), TotalPlays),
            new DynamicVar(nameof(Relics), 3),
            new GoldVar(nameof(Gold), 75),
        ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [Tip.Purge()];

    protected override void OnUpgrade()
    {
        Relics.UpgradeValueTo(4);
        Gold.UpgradeValueTo(150);
    }

    protected override void AfterCloned()
    {
        PlaysRemaining.BaseValue = TotalPlays;
        FinishedThisCombat = false;
    }

    [SavedProperty]
    public bool FinishedThisCombat
    {
        get;
        set
        {
            AssertMutable();
            field = value;
        }
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var deck = this.Permanently(c => --c.PlaysRemaining.BaseValue);
        if (deck == null)
            return;

        if (deck.PlaysRemaining.BaseValue <= 0)
        {
            FinishedThisCombat = true;
            await CardPileCmd.RemoveFromDeck(deck);
        }
    }

    public override Task AfterCombatEnd(CombatRoom room)
    {
        if (!FinishedThisCombat)
            return Task.CompletedTask;

        for (var i = 0; i < Relics.IntValue; i++)
            room.AddExtraReward(Owner, new RelicReward(Owner));
        room.AddExtraReward(Owner, new GoldReward(Gold.IntValue, Owner));
        return Task.CompletedTask;
    }
}
