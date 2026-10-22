using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using NewKunlun.NewKunlunCode.Character;
using NewKunlun.NewKunlunCode.Localization;
using NewKunlun.NewKunlunCode.Powers;
using NewKunlun.NewKunlunCode.Tips;

namespace NewKunlun.NewKunlunCode.Relics;

[Pool(typeof(YiRelicPool))]
[RelicLocalization(
    title: "Seal of Discipline",
    description: "Whenever you play an attack, gain {Strength} [gold]Strength[/gold].\nWhenever you play a non-attack, lose all strength obtained this way.",
    flavor: ""
)]
public partial class SealOfDisciplineRelic : NewKunlunRelic
{
    public override RelicRarity Rarity => RelicRarity.Event;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DynamicVar(nameof(Strength), 2)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [Tip.Strength()];

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner == Owner && cardPlay.Card.Type == CardType.Attack)
        {
            Flash();
            await PowerCmd.Apply<DisciplinePower>(
                choiceContext,
                Owner.Creature,
                Strength.IntValue,
                Owner.Creature,
                cardSource: null,
                silent: true
            );
        }
    }

    public override async Task BeforeCardPlayed(CardPlay cardPlay)
    {
        if (
            cardPlay.Card.Owner == Owner
            && cardPlay.Card.Type != CardType.Attack
            && Owner.HasPower<DisciplinePower>()
        )
        {
            Flash();
            await PowerCmd.Remove<DisciplinePower>(Owner.Creature);
        }
    }
}
