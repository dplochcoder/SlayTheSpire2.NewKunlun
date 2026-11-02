using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Enchantments;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Runs;
using NewKunlun.NewKunlunCode.Commands;
using NewKunlun.NewKunlunCode.Enchantments;
using NewKunlun.NewKunlunCode.Extensions;
using NewKunlun.NewKunlunCode.Localization;

namespace NewKunlun.NewKunlunCode.Relics.Ancients.Abacus;

[Pool(typeof(SharedRelicPool))]
[RelicLocalization(
    title: "Centrifuge",
    description: "Receive [gold]Poor Sleep[/gold].\nYour next {Uses} card {Uses:plural:reward gains|rewards gain} random enchantments.",
    flavor: ""
)]
public partial class CentrifugeRelic : NewKunlunRelic
{
    public override RelicRarity Rarity => RelicRarity.Ancient;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new(nameof(Uses), 3)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromCard<PoorSleep>()];

    private int UsesRemaining
    {
        get => Uses.IntValue;
        set
        {
            AssertMutable();
            Uses.BaseValue = value;
            Status = IsUsedUp ? RelicStatus.Disabled : RelicStatus.Normal;
            InvokeDisplayAmountChanged();
        }
    }

    public override bool ShowCounter => UsesRemaining > 0;

    public override int DisplayAmount => UsesRemaining;

    public override bool IsUsedUp => UsesRemaining == 0;

    public override bool TryModifyCardRewardOptionsLate(
        Player player,
        List<CardCreationResult> cardRewards,
        CardCreationOptions options
    )
    {
        if (
            player != Owner
            || !options.Flags.HasFlag(CardCreationFlags.IsCardReward)
            || !this.IsNextUnused
        )
            return false;

        foreach (var cardReward in cardRewards)
        {
            var clone = Owner.RunState.CloneCard(cardReward.Card);
            if (!RandomEnchantCmd.ApplyEnchantment(Owner.PlayerRng.Rewards, clone))
                continue;

            cardReward.ModifyCard(clone, this);
        }
        return true;
    }

    public override Task AfterModifyingCardRewardOptions()
    {
        --UsesRemaining;
        return Task.CompletedTask;
    }
}
