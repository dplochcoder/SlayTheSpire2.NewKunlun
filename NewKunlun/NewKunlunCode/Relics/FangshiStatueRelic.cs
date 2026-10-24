using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using NewKunlun.NewKunlunCode.Character;
using NewKunlun.NewKunlunCode.Extensions;
using NewKunlun.NewKunlunCode.Localization;
using NewKunlun.NewKunlunCode.Relics;
using NewKunlun.NewKunlunCode.Tips;

[Pool(typeof(YiRelicPool))]
[RelicLocalization(
    title: "Fangshi Statue",
    description: "On pickup, gain an extra [gold]Strike[/gold] or [gold]Defend[/gold].\nAt start of combat, gain 1 [gold]Strength[/gold] and 1 [gold]Dexterity[/gold].",
    flavor: ""
)]
public class FangshiStatueRelic : NewKunlunRelic
{
    public override RelicRarity Rarity => RelicRarity.Ancient;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [Tip.Strength(), Tip.Dexterity()];

    public override bool HasUponPickupEffect => true;

    public override async Task AfterObtained()
    {
        List<CardModel> cardPool = [.. Owner.Character.CardPool.AllCards];
        var strike = cardPool.First(c => c.Tags.Contains(CardTag.Strike));
        var defend = cardPool.First(c => c.Tags.Contains(CardTag.Defend));
        await this.ChooseCardToAddToDeck([strike, defend]);
    }

    public override async Task AfterSideTurnStart(
        CombatSide side,
        IReadOnlyList<Creature> participants,
        ICombatState combatState
    )
    {
        if (side != Owner.Creature.Side || combatState.RoundNumber != 1)
            return;

        Flash();
        await PowerCmd.Apply<StrengthPower>(
            new ThrowingPlayerChoiceContext(),
            Owner.Creature,
            1,
            Owner.Creature,
            null
        );
        await PowerCmd.Apply<DexterityPower>(
            new ThrowingPlayerChoiceContext(),
            Owner.Creature,
            1,
            Owner.Creature,
            null
        );
    }
}
