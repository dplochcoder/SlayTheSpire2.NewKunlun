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
using NewKunlun.NewKunlunCode.Variables;

namespace NewKunlun.NewKunlunCode.Powers;

[PowerLocalization(
    title: "Enhanced Qi Blast",
    description: "",
    smartDescription: "Whenever you [gold]Discharge[/gold] 3 or more while [gold]Detonating[/gold], add 1 {AzureSand:cardName()} on top of your draw pile."
)]
public partial class EnhancedQiBlastPower : NewKunlunPower, ITalismanDetonateListener
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override PowerInstanceType InstanceType => PowerInstanceType.Instanced;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new DynamicVar(nameof(UpgradeLevel), 0M),
            new CardNameVar<AzureSandCard>(() => IsUpgraded),
        ];

    public bool IsUpgraded
    {
        get => UpgradeLevel.BaseValue > 0;
        set => UpgradeLevel.BaseValue = value ? 1 : 0;
    }

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
                CombatState.CreateUpgradedCard<AzureSandCard>(Owner.Player, upgrade: IsUpgraded)
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
