using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.ValueProps;

namespace SlayTheSpire2.NewKunlun.SlayTheSpire2.NewKunlunCode.Cards;

[Pool(typeof(StatusCardPool))]
public class SmolderCard() : NewKunlunCard(1, CardType.Status, CardRarity.Status, TargetType.None)
{
    private const string EndTurnDamage = nameof(EndTurnDamage);
    private const string ExhaustDamage = nameof(ExhaustDamage);

    public override int MaxUpgradeLevel => 0;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new DamageVar(EndTurnDamage, 1, ValueProp.Unblockable | ValueProp.Unpowered),
            new DamageVar(ExhaustDamage, 1, ValueProp.Unpowered | ValueProp.Move),
        ];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

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
        await CreatureCmd.Damage(
            choiceContext,
            Owner.Creature,
            (DamageVar)DynamicVars[EndTurnDamage],
            this,
            null
        );
        CardCmd.PreviewCardPileAdd(
            await CardPileCmd.AddGeneratedCardToCombat(
                CombatState!.CreateCard<SmolderCard>(Owner),
                PileType.Discard,
                Owner
            ),
            1.4f
        );
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        SpawnFire();
        await CardCmd.Exhaust(choiceContext, this);
        await CreatureCmd.Damage(
            choiceContext,
            Owner.Creature,
            (DamageVar)DynamicVars[ExhaustDamage],
            this,
            cardPlay
        );
    }
}
