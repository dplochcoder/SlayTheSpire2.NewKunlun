using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models.RelicPools;
using NewKunlun.NewKunlunCode.Localization;
using NewKunlun.NewKunlunCode.Modifiers;

namespace NewKunlun.NewKunlunCode.Relics.Ancients.Shuanshuan;

[Pool(typeof(SharedRelicPool))]
[RelicLocalization(
    title: "Tiandao Academy Periodical",
    description: "At the start of your turn, a random card in your hand gains [gold]Replay[/gold] 1 for this turn.",
    flavor: ""
)]
public class TiandaoAcademyPeriodicalRelic : NewKunlunRelic
{
    public override RelicRarity Rarity => RelicRarity.Ancient;

    public override Task AfterSideTurnStart(
        CombatSide side,
        IReadOnlyList<Creature> participants,
        ICombatState combatState
    )
    {
        if (side != Owner.Creature.Side)
            return Task.CompletedTask;

        var card = combatState.RunState.Rng.CombatCardSelection.NextItem(
            PileType
                .Hand.GetPile(Owner)
                .Cards.Where(c => !c.Keywords.Contains(CardKeyword.Unplayable))
        );
        if (card == null)
            return Task.CompletedTask;

        Flash();
        CardModifier.AddModifier<AcademicModifier>(card);
        CardCmd.Preview(card);
        return Task.CompletedTask;
    }
}
