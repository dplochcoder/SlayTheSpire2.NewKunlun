using System.Reflection.Emit;
using BaseLib.Abstracts;
using BaseLib.Extensions;
using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Random;
using MegaCrit.Sts2.Core.Runs;
using NewKunlun.NewKunlunCode.Extensions;
using NewKunlun.NewKunlunCode.Keywords;

namespace NewKunlun.NewKunlunCode.Cards;

/// <summary>
/// This is the base class for your mod's cards, which is set up to load the card's images from your mod's resources.
/// When creating a card, right click the Cards folder and create a new file with the Custom Card template.
/// This will generate a class that extends this one.
/// You can also just create the class manually; just make sure to inherit from this class.
/// </summary>
public abstract class NewKunlunCard(int cost, CardType type, CardRarity rarity, TargetType target)
    : CustomCardModel(cost, type, rarity, target)
{
    public virtual bool IsParryCard => false;

    public bool Endemic => Keywords.Contains(ExtraCardKeywords.Endemic);

    public bool Quantum => Keywords.Contains(ExtraCardKeywords.Quantum);

    //Image size:
    //Normal art: 1000x760 (Using 500x380 should also work, it will simply be scaled.)
    //Full art: 606x852
    public override string CustomPortraitPath =>
        $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigCardImagePath();

    //Smaller variants of card images for efficiency:
    //Smaller variant of fullart: 250x350
    //Smaller variant of normalart: 250x190

    //Uses card_portraits/card_name.png as image path. These should be smaller images.
    public override string PortraitPath =>
        $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".CardImagePath();
    public override string BetaPortraitPath =>
        $"beta/{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".CardImagePath();

    public bool AddedToHand
    {
        get;
        set
        {
            AssertMutable();
            field = value;
        }
    }

    public bool QuantumBroken
    {
        get;
        set
        {
            AssertMutable();
            field = value;
        }
    }

    protected override bool ShouldGlowGoldInternal => Quantum && AddedToHand && !QuantumBroken;

    public override async Task AfterCardExhausted(
        PlayerChoiceContext choiceContext,
        CardModel card,
        bool causedByEthereal
    )
    {
        if (card != this || !Endemic)
            return;

        await CardPileCmd.Add(CreateClone(), PileType.Discard.GetPile(Owner), clonedBy: this);
    }

    public virtual async Task AfterTransformedFromAsync()
    {
        if (Endemic)
            CardCmd.PreviewCardPileAdd(
                await CardPileCmd.Add(
                    CreateClone(),
                    PileType.Discard.GetPile(Owner),
                    clonedBy: this
                )
            );
    }

    public sealed override Task AfterCardChangedPiles(
        CardModel card,
        PileType oldPileType,
        AbstractModel? clonedBy
    )
    {
        if (card == this && oldPileType != PileType.Hand && card.Pile?.Type == PileType.Hand)
            AddedToHand = true;

        return Task.CompletedTask;
    }

    public sealed override bool TryModifyKeywordsInCombat(
        CardModel card,
        ISet<CardKeyword> keywords
    )
    {
        if (card != this || !QuantumBroken)
            return false;

        return keywords.Add(CardKeyword.Unplayable);
    }

    public sealed override Task AfterCardPlayed(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay
    )
    {
        if (
            Quantum
            && !QuantumBroken
            && AddedToHand
            && cardPlay.Card.Owner == Owner
            && cardPlay.Card != this
        )
            QuantumBroken = true;

        return Task.CompletedTask;
    }

    [HarmonyPatch]
    private static class Patches
    {
        // Modifying an async state machine with more awaits is extremely difficult.
        // Instead, we hijack Hook.AfterCardChangedPiles, replacing it and the following statements with our own Task.
        [HarmonyPatch(
            typeof(CardCmd),
            MethodType.Async,
            typeof(IEnumerable<CardTransformation>),
            typeof(Rng),
            typeof(CardPreviewStyle)
        )]
        [HarmonyTranspiler]
        private static IEnumerable<CodeInstruction> Transpile(IEnumerable<CodeInstruction> src)
        {
            List<CodeInstruction> orig = [.. src];
            var methodInfo = typeof(Hook).GetMethod(nameof(Hook.AfterCardChangedPiles));
            var idx = orig.FindIndex(i => i.Calls(methodInfo));

            var invokeCardAddFinished = typeof(CardPile).GetMethod(
                nameof(CardPile.InvokeCardAddFinished)
            );
            var afterTransformedFrom = typeof(CardModel).GetMethod(nameof(AfterTransformedFrom));
            var afterTransformedTo = typeof(CardModel).GetMethod(nameof(AfterTransformedTo));
            var replacements = 0;
            CodeInstruction? getOriginal = null;
            for (var i = idx; i < orig.Count && replacements <= 3; i++)
            {
                if (orig[i].Calls(invokeCardAddFinished))
                {
                    orig[i] = new CodeInstruction(OpCodes.Pop);
                    replacements++;
                }
                else if (orig[i].Calls(afterTransformedFrom))
                {
                    getOriginal = orig[i - 1];
                    orig[i] = new CodeInstruction(OpCodes.Pop);
                    replacements++;
                }
                else if (orig[i].Calls(afterTransformedTo))
                {
                    orig[i] = new CodeInstruction(OpCodes.Pop);
                    replacements++;
                }
            }

            if (getOriginal == null || replacements < 3)
                throw new ArgumentException(
                    $"Failed to update {nameof(CardCmd)}.{nameof(CardCmd.Transform)}."
                );

            for (var i = 0; i < orig.Count; i++)
            {
                if (i == idx)
                {
                    yield return getOriginal;
                    yield return CodeInstruction.Call(
                        (
                            IRunState runState,
                            ICombatState? combatState,
                            CardModel card,
                            PileType oldPile,
                            AbstractModel? clonedBy,
                            CardPile cardPile,
                            CardModel originalCard
                        ) =>
                            // ReSharper disable once ConvertClosureToMethodGroup
                            CallAsyncTransformCallbacks(
                                runState,
                                combatState,
                                card,
                                oldPile,
                                clonedBy,
                                originalCard
                            )
                    );
                }
                else
                    yield return orig[i];
            }
        }

        private static async Task CallAsyncTransformCallbacks(
            IRunState runState,
            ICombatState? combatState,
            CardModel card,
            PileType oldPile,
            AbstractModel? clonedBy,
            CardModel originalCard
        )
        {
            await Hook.AfterCardChangedPiles(runState, combatState, card, oldPile, clonedBy);

            card.Pile?.InvokeCardAddFinished();
            originalCard.AfterTransformedFrom();
            if (originalCard is NewKunlunCard nkCard)
                await nkCard.AfterTransformedFromAsync();
            card.AfterTransformedTo();
        }
    }
}
