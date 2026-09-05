using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using NewKunlun.NewKunlunCode.Cards;
using NewKunlun.NewKunlunCode.Character;
using NewKunlun.NewKunlunCode.Commands;
using NewKunlun.NewKunlunCode.Extensions;
using NewKunlun.NewKunlunCode.Localization;
using NewKunlun.NewKunlunCode.Tips;

namespace NewKunlun.NewKunlunCode.Relics;

[Pool(typeof(YiRelicPool))]
[RelicLocalization(
    title: "Enhanced Talisman",
    description: "At the start of combat, add one [green]Talisman Dash+[/green] into your hand and gain 3 [gold]Qi Charges[/gold].",
    flavor: ""
)]
public partial class EnhancedTalismanRelic : NewKunlunRelic
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

        await QiChargeCmd.GainQiCharges(choiceContext, Owner.Creature, 3M, Owner.Creature, null);
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
            Owner.Creature.CombatState!.CreateUpgradedCard<TalismanDashCard>(Owner, upgrade: true),
            PileType.Hand,
            Owner,
            CardPilePosition.Top
        );
    }
}
