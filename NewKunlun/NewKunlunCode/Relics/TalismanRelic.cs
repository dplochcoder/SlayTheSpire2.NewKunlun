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
using NewKunlun.NewKunlunCode.Cards;
using NewKunlun.NewKunlunCode.Character;
using NewKunlun.NewKunlunCode.Commands;
using NewKunlun.NewKunlunCode.Localization;
using NewKunlun.NewKunlunCode.Tips;

namespace NewKunlun.NewKunlunCode.Relics;

[Pool(typeof(YiRelicPool))]
[RelicLocalization(
    title: "Talisman",
    description: "At the start of combat, shuffle one [gold]Talisman Dash[/gold] into your deck and gain 1 [gold]Qi Charge[/gold].",
    flavor: ""
)]
public partial class TalismanRelic : NewKunlunRelic
{
    public override RelicRarity Rarity => RelicRarity.Starter;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [Tip.TalismanDashCard(null), Tip.QiCharge()];

    public override async Task BeforeSideTurnStart(
        PlayerChoiceContext choiceContext,
        CombatSide side,
        IReadOnlyList<Creature> participants,
        ICombatState combatState
    )
    {
        if (!participants.Contains(Owner.Creature) || Owner.PlayerCombatState!.TurnNumber > 1)
            return;
        await QiChargeCmd.GainQiCharges(choiceContext, Owner.Creature, 1M, Owner.Creature, null);
    }

    public override async Task BeforeHandDraw(
        Player player,
        PlayerChoiceContext choiceContext,
        ICombatState combatState
    )
    {
        if (player != Owner || Owner.PlayerCombatState!.TurnNumber != 1)
            return;

        await CardPileCmd.AddGeneratedCardToCombat(
            Owner.Creature.CombatState!.CreateCard<TalismanDashCard>(Owner),
            PileType.Draw,
            Owner,
            CardPilePosition.Random
        );
    }

    public override RelicModel? GetUpgradeReplacement() => ModelDb.Get<EnhancedTalismanRelic>();
}
