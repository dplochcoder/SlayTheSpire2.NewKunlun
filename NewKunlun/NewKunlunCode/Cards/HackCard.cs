using BaseLib.Cards;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Events;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Saves.Runs;
using NewKunlun.NewKunlunCode.Character;
using NewKunlun.NewKunlunCode.Extensions;
using NewKunlun.NewKunlunCode.Keywords;
using NewKunlun.NewKunlunCode.Localization;
using NewKunlun.NewKunlunCode.Relics;
using NewKunlun.NewKunlunCode.Tips;

namespace NewKunlun.NewKunlunCode.Cards;

[Pool(typeof(QuestCardPool))]
[CardLocalization(
    title: "Hack",
    description: "Play {PlaysRemaining} more {PlaysRemaining:plural:time|times} to open the [gold]Locked Chest[/gold].\nOn open, gain {Relics:diff()} Relics and {Gold:diff()} Gold."
)]
public partial class HackCard()
    : NewKunlunCard(2, CardType.Quest, CardRarity.Quest, TargetType.None)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords =>
        [CardKeyword.Exhaust, NewKunlunCardKeywords.Quantum];

    private const int TotalPlays = 4;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new(nameof(PlaysRemaining), TotalPlays),
            new(nameof(Relics), 3),
            new GoldVar(nameof(Gold), 75),
        ];

    protected override bool IsPlayable =>
        FirstChestOfState(LockedChestRelic.State.Closed) is not null;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromKeyword(BaseLibKeywords.Purge)];

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

    private LockedChestRelic? FirstChestOfState(LockedChestRelic.State state) =>
        Owner.Relics.OfType<LockedChestRelic>()?.FirstOrDefault(r => r.CurrentState == state);

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var deck = this.Permanently(c => --c.PlaysRemaining.BaseValue);
        if (deck == null)
            return;

        if (deck.PlaysRemaining.BaseValue <= 0)
        {
            FinishedThisCombat = true;
            await CardPileCmd.RemoveFromDeck(deck);

            var chest = FirstChestOfState(LockedChestRelic.State.Closed);
            if (chest != null)
            {
                chest.Flash();
                chest.CurrentState = LockedChestRelic.State.Opened;
            }
        }
    }

    public override Task AfterCombatEnd(CombatRoom room)
    {
        if (!FinishedThisCombat)
            return Task.CompletedTask;

        var chest = FirstChestOfState(LockedChestRelic.State.Opened);
        if (chest == null)
            return Task.CompletedTask;

        chest.Flash();
        chest.CurrentState = LockedChestRelic.State.Looted;

        for (var i = 0; i < Relics.IntValue; i++)
            room.AddExtraReward(Owner, new RelicReward(Owner));
        room.AddExtraReward(Owner, new GoldReward(Gold.IntValue, Owner));
        return Task.CompletedTask;
    }
}
