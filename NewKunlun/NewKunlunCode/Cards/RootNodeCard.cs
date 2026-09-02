using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using NewKunlun.NewKunlunCode.Character;
using NewKunlun.NewKunlunCode.Commands;
using NewKunlun.NewKunlunCode.Extensions;
using NewKunlun.NewKunlunCode.Localization;
using NewKunlun.NewKunlunCode.Powers;
using NewKunlun.NewKunlunCode.Variables;

namespace NewKunlun.NewKunlunCode.Cards;

[Pool(typeof(YiCardPool))]
[CardLocalization(
    title: "Root Node",
    description: "Heal {HealHP:diff()}. Gain {GainMaxHP:diff()} max HP. Heal all [gold]Internal Damage[/gold]. Gain {Strength:diff()} [gold]Strength[/gold] and {Dexterity:diff()} [gold]Dexterity[/gold]. Choose {TopDeckCards:plural:card|cards} from your deck and place {TopDeckCards:cond:>1?them|it} on top. {UpgradesRemaining:cond:>0?Can be upgraded {UpgradesRemaining:plural:more time|more times}|}.",
    selectionScreenPrompt: "Choose up to {TopDeckCards:plural:card|cards} to place at the top of your deck."
)]
public partial class RootNodeCard()
    : NewKunlunCard(3, CardType.Power, CardRarity.Rare, TargetType.Self)
{
    private const int MaxUpgrades = 3;

    public override int MaxUpgradeLevel => MaxUpgrades;

    public override IEnumerable<CardKeyword> CanonicalKeywords =>
        IsUpgradable ? [] : [CardKeyword.Retain];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new DynamicVar(nameof(HealHP), 6M),
            new DynamicVar(nameof(GainMaxHP), 1M),
            new InternalDamageHealVar(20M),
            new DynamicVar(nameof(Strength), 2M),
            new DynamicVar(nameof(Dexterity), 2M),
            new DynamicVar(nameof(TopDeckCards), 1M),
            new DynamicVar(nameof(UpgradesRemaining), MaxUpgrades),
        ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [
            Tips.Power<InternalDamagePower>(),
            Tips.Power<StrengthPower>(),
            Tips.Power<DexterityPower>(),
        ];

    protected override void OnUpgrade()
    {
        switch (CurrentUpgradeLevel)
        {
            case 0:
                HealHP.UpgradeValueTo(8M);
                InternalDamageHeal.UpgradeValueTo(24M);
                TopDeckCards.UpgradeValueTo(2M);
                break;
            case 1:
                HealHP.UpgradeValueTo(10M);
                Strength.UpgradeValueTo(3M);
                Dexterity.UpgradeValueTo(3M);
                break;
            case 2:
                GainMaxHP.UpgradeValueTo(2M);
                InternalDamageHeal.UpgradeValueTo(30M);
                Strength.UpgradeValueTo(4M);
                Dexterity.UpgradeValueTo(4M);
                TopDeckCards.UpgradeValueTo(3M);
                break;
        }

        UpgradesRemaining.BaseValue--;
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.Heal(Owner.Creature, HealHP.BaseValue);
        await CreatureCmd.GainMaxHp(Owner.Creature, GainMaxHP.BaseValue);
        await InternalDamageCmd.Heal(
            choiceContext,
            Owner.Creature,
            InternalDamageHeal,
            Owner.Creature,
            this
        );
        await PowerCmd.Apply<StrengthPower>(
            choiceContext,
            Owner.Creature,
            Strength.BaseValue,
            Owner.Creature,
            this
        );
        await PowerCmd.Apply<DexterityPower>(
            choiceContext,
            Owner.Creature,
            Dexterity.BaseValue,
            Owner.Creature,
            this
        );

        var cards = await CardSelectCmd.FromCombatPile(
            choiceContext,
            PileType.Draw.GetPile(Owner),
            cardPlay.Player,
            new CardSelectorPrefs(SelectionScreenPrompt, 0, (int)TopDeckCards.BaseValue)
        );
        foreach (var card in cards)
            await CardPileCmd.Add(card, PileType.Draw, CardPilePosition.Top);
    }
}
