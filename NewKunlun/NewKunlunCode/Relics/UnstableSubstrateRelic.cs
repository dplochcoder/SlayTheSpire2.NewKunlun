using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using NewKunlun.NewKunlunCode.Character;
using NewKunlun.NewKunlunCode.Commands;
using NewKunlun.NewKunlunCode.Enchantments;
using NewKunlun.NewKunlunCode.Localization;

namespace NewKunlun.NewKunlunCode.Relics;

[Pool(typeof(YiRelicPool))]
[RelicLocalization(
    title: "Unstable Substrate",
    description: "Lose {HpLoss} hp.\nEnchant a card with [gold]Amortized[/gold].",
    flavor: ""
)]
public partial class UnstableSubstrateRelic : NewKunlunRelic
{
    public override RelicRarity Rarity => RelicRarity.Ancient;

    public override bool HasUponPickupEffect => true;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DynamicVar(nameof(HpLoss), 14)];

    public override async Task AfterObtained()
    {
        await CreatureCmd.LoseMaxHp(
            new ThrowingPlayerChoiceContext(),
            Owner.Creature,
            HpLoss.IntValue,
            false
        );
        await CardEnchantCmd.EnchantCardsFromDeck<AmortizedEnchantment>(
            Owner,
            amount: 1,
            min: 0,
            max: 1
        );
    }
}
