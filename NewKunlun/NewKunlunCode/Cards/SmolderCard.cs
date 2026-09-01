using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.ValueProps;
using NewKunlun.NewKunlunCode.Extensions;
using NewKunlun.NewKunlunCode.Localization;

namespace NewKunlun.NewKunlunCode.Cards;

[Pool(typeof(StatusCardPool))]
[CardLocalization(
    title: "Smolder",
    description: "Take {OnExhaustDamage} damage. If this is in your hand at the end of your turn, lose {EndOfTurnDamage:inverseDiff()} and add 1 [gold]Smolder[/gold] to your discard pile."
)]
public partial class SmolderCard()
    : NewKunlunCard(1, CardType.Status, CardRarity.Status, TargetType.None)
{
    public override int MaxUpgradeLevel => 0;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new DamageVar(nameof(EndOfTurnDamage), 1, ValueProp.Unblockable | ValueProp.Unpowered),
            new DamageVar(nameof(OnExhaustDamage), 1, ValueProp.Unpowered | ValueProp.Move),
        ];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [Tips.Card<SmolderCard>()];

    public override bool HasTurnEndInHandEffect => true;

    protected override IEnumerable<string> ExtraRunAssetPaths => NGroundFireVfx.AssetPaths;

    private void SpawnFire()
    {
        var instance = NCombatRoom.Instance;
        instance?.CombatVfxContainer.AddChildSafely(NGroundFireVfx.Create(Owner.Creature));
        SfxCmd.Play("event:/sfx/characters/attack_fire");
    }

    protected override async Task OnTurnEndInHand(PlayerChoiceContext choiceContext)
    {
        SpawnFire();
        await CreatureCmd.Damage(choiceContext, Owner.Creature, EndOfTurnDamage, this, null);
        await this.AddGeneratedStatusToPile<SmolderCard>(PileType.Discard);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        SpawnFire();
        await CreatureCmd.Damage(choiceContext, Owner.Creature, OnExhaustDamage, this, cardPlay);
    }
}
