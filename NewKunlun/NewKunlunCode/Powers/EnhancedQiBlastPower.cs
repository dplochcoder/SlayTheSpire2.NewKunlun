using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using NewKunlun.NewKunlunCode.Cards;
using NewKunlun.NewKunlunCode.Extensions;
using NewKunlun.NewKunlunCode.Hooks;
using NewKunlun.NewKunlunCode.Localization;

namespace NewKunlun.NewKunlunCode.Powers;

[PowerLocalization(
    title: "Enhanced Qi Blast",
    description: "",
    smartDescription: "Whenever you spend 3 or more [gold]Qi Charges[/gold] on [gold]Talisman Detonate[/gold], place 1 {Upgraded:cond:>0?[green]Azure Sand+[/green]|[gold]Azure Sand[/gold]} on top of your draw pile."
)]
public partial class EnhancedQiBlastPower : NewKunlunPower, ITalismanDetonateListener
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;
    public override PowerInstanceType InstanceType => PowerInstanceType.Instanced;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DynamicVar(nameof(Upgraded), 0M)];

    async Task ITalismanDetonateListener.OnTalismanDetonated(
        PlayerChoiceContext choiceContext,
        int qiCharges,
        decimal totalDamage,
        Creature? dealer
    )
    {
        if (Owner.Player == null || dealer != Owner || qiCharges < 3)
            return;

        List<CardModel> cards = [];
        for (var i = 0; i < Amount; i++)
            cards.Add(
                CombatState.CreateCard<AzureSandCard>(Owner.Player, upgrade: Upgraded.BaseValue > 0)
            );

        CardCmd.PreviewCardPileAdd(
            await CardPileCmd.AddGeneratedCardsToCombat(
                cards,
                PileType.Draw,
                Owner.Player,
                CardPilePosition.Top
            )
        );
    }
}
