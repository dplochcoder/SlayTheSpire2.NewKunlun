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
    description: "Take {ExhaustDamage} damage. If this is in your hand at the end of your turn, lose {EndTurnDamage:inverseDiff()} and add 1 [gold]Smolder[/gold] to your discard pile."
)]
public partial class SmolderCard()
    : NewKunlunCard(1, CardType.Status, CardRarity.Status, TargetType.None)
{
    public override int MaxUpgradeLevel => 0;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new DamageVar(nameof(EndTurnDamage), 1, ValueProp.Unblockable | ValueProp.Unpowered),
            new DamageVar(nameof(ExhaustDamage), 1, ValueProp.Unpowered | ValueProp.Move),
        ];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromCard<SmolderCard>()];

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
        await CreatureCmd.Damage(choiceContext, Owner.Creature, EndTurnDamage, this, null);
        await this.AddGeneratedStatusToPile<SmolderCard>();
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        SpawnFire();
        await CardCmd.Exhaust(choiceContext, this);
        await CreatureCmd.Damage(choiceContext, Owner.Creature, ExhaustDamage, this, cardPlay);
    }
}
