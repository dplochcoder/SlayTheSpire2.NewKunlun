using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.CardRewardAlternatives;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Entities.Rewards;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Rewards;
using NewKunlun.NewKunlunCode.Localization;

namespace NewKunlun.NewKunlunCode.Relics;

[Pool(typeof(SharedRelicPool))]
[RelicLocalization(
    title: "Recycler",
    description: "You can recycle card rewards for {MinGold} to {MaxGold} gold.",
    flavor: ""
)]
public partial class RecyclerRelic : NewKunlunRelic
{
    public override RelicRarity Rarity => RelicRarity.Ancient;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new GoldVar(nameof(MinGold), 30), new GoldVar(nameof(MaxGold), 40)];

    public override bool TryModifyCardRewardAlternatives(
        Player player,
        CardReward cardReward,
        List<CardRewardAlternative> alternatives
    )
    {
        if (player != Owner)
            return false;

        alternatives.Add(
            new CardRewardAlternative(
                "NEW_KUNLUN-Recycle",
                Recycle,
                PostAlternateCardRewardAction.EndSelectionAndCompleteReward
            )
        );
        return true;
    }

    private async Task Recycle()
    {
        Flash();

        var gold = Owner.PlayerRng.Rewards.NextInt(MinGold.IntValue, MaxGold.IntValue + 1);
        await PlayerCmd.GainGold(gold, Owner);
    }
}
