using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.RelicPools;
using NewKunlun.NewKunlunCode.Enchantments;
using NewKunlun.NewKunlunCode.Localization;

namespace NewKunlun.NewKunlunCode.Relics.Ancients.Abacus;

[Pool(typeof(SharedRelicPool))]
[RelicLocalization(
    title: "Rhizomatic Substrate",
    description: "Lose {MaxHpLoss} max hp.\nEnchant a card with [gold]Amortized[/gold].",
    flavor: ""
)]
public partial class RhizomaticSubstrateRelic : EnchantCardsRelic<AmortizedEnchantment>
{
    public override RelicRarity Rarity => RelicRarity.Ancient;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new(nameof(MaxHpLoss), 11)];

    protected override int NumCardsMin => 1;

    protected override int NumCardsMax => 1;

    protected override Task AfterObtainedInternal() =>
        CreatureCmd.LoseMaxHp(
            new ThrowingPlayerChoiceContext(),
            Owner.Creature,
            MaxHpLoss.IntValue,
            false
        );
}
