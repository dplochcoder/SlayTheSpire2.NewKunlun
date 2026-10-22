using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models.Powers;
using NewKunlun.NewKunlunCode.Character;
using NewKunlun.NewKunlunCode.Extensions;
using NewKunlun.NewKunlunCode.Localization;
using NewKunlun.NewKunlunCode.Tips;
using Void = MegaCrit.Sts2.Core.Models.Cards.Void;

namespace NewKunlun.NewKunlunCode.Relics;

[Pool(typeof(YiRelicPool))]
[RelicLocalization(
    title: "Rhizomatic Battery",
    description: "At start of combat, shuffle one [gold]Void[/gold] into your draw pile.\nWhenever you end your turn with X unspent energy, draw X additional cards next turn.",
    flavor: ""
)]
public class RhizomaticBatteryRelic : NewKunlunRelic
{
    public override RelicRarity Rarity => RelicRarity.Ancient;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [Tip.Void()];

    public override async Task BeforeHandDraw(
        Player player,
        PlayerChoiceContext choiceContext,
        ICombatState combatState
    )
    {
        if (player != Owner || Owner.PlayerCombatState!.TurnNumber != 1)
            return;

        Flash();
        await Owner.AddGeneratedCardToPile<Void>(PileType.Draw, position: CardPilePosition.Random);
    }

    public override async Task AfterSideTurnEnd(
        PlayerChoiceContext choiceContext,
        CombatSide side,
        IEnumerable<Creature> participants
    )
    {
        if (!participants.Contains(Owner.Creature))
            return;

        var energy = Owner.PlayerCombatState?.Energy ?? 0;
        if (energy == 0)
            return;

        Flash();
        await PowerCmd.Apply<DrawCardsNextTurnPower>(
            choiceContext,
            Owner.Creature,
            energy,
            Owner.Creature,
            null
        );
    }
}
