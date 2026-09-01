using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using NewKunlun.NewKunlunCode.Cards;
using NewKunlun.NewKunlunCode.Localization;

namespace NewKunlun.NewKunlunCode.Powers;

// TODO: Formatter.
[PowerLocalization(
    title: "Talisman Detonate",
    description: "Next turn, add one [gold]Talisman Detonate[/gold] to your hand.",
    smartDescription: "Next turn, add one [gold]Talisman Detonate{UpgradeCount:cond:>0:(+)|}[/gold] to your hand."
)]
public partial class TalismanDetonatePower : NewKunlunPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;
    public override PowerInstanceType InstanceType => PowerInstanceType.Instanced;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DynamicVar(nameof(UpgradeCount), 0M)];

    public bool Upgraded
    {
        get => UpgradeCount.BaseValue > 0;
        set => UpgradeCount.BaseValue = value ? 1 : 0;
    }

    public override async Task BeforeHandDraw(
        Player player,
        PlayerChoiceContext choiceContext,
        ICombatState combatState
    )
    {
        if (player != Owner.Player)
            return;

        var card = combatState.CreateCard<TalismanDetonateCard>(player);
        if (Upgraded)
            CardCmd.Upgrade(card, CardPreviewStyle.None);
        await CardPileCmd.AddGeneratedCardToCombat(card, PileType.Hand, player);
        await PowerCmd.Remove(this);
        Flash();
    }
}
