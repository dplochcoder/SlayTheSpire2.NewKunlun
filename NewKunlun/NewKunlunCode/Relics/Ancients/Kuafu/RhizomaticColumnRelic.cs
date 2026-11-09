using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Runs;
using NewKunlun.NewKunlunCode.Enchantments;
using NewKunlun.NewKunlunCode.Localization;
using NewKunlun.NewKunlunCode.Powers;

namespace NewKunlun.NewKunlunCode.Relics.Ancients.Kuafu;

[Pool(typeof(SharedRelicPool))]
[RelicLocalization(
    title: "Rhizomatic Column",
    description: "Card rewards will contain one [gold]Rhizomatic[/gold] card.\nIf you end your turn with unspent energy, lose one energy next turn.",
    flavor: ""
)]
public class RhizomaticColumnRelic : NewKunlunRelic
{
    public override RelicRarity Rarity => RelicRarity.Ancient;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        HoverTipFactory.FromEnchantment<RhizomaticEnchantment>();

    public override bool TryModifyCardRewardOptions(
        Player player,
        List<CardCreationResult> cardRewardOptions,
        CardCreationOptions creationOptions
    )
    {
        var enchantment = ModelDb.Enchantment<RhizomaticEnchantment>();
        List<CardCreationResult> options =
        [
            .. cardRewardOptions.Where(o =>
                o.Card.Enchantment == null && enchantment.CanEnchant(o.Card)
            ),
        ];
        var rng = creationOptions.RngOverride ?? player.PlayerRng.Rewards;
        var option = rng.NextItem(options);
        if (option == null)
            return false;

        var clone = option.Card.CreateClone();
        CardCmd.Enchant<RhizomaticEnchantment>(clone, 1);
        option.ModifyCard(clone);
        return true;
    }

    public override async Task AfterSideTurnEnd(
        PlayerChoiceContext choiceContext,
        CombatSide side,
        IEnumerable<Creature> participants
    )
    {
        if (side != Owner.Creature.Side || Owner.PlayerCombatState?.Energy != 0)
            return;

        await PowerCmd.Apply<RhizomaticColumnPower>(
            choiceContext,
            Owner.Creature,
            1,
            Owner.Creature,
            null
        );
    }
}
