using BaseLib.Abstracts;
using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using NewKunlun.NewKunlunCode.Collections;

namespace NewKunlun.NewKunlunCode.Monsters;

public abstract class NewKunlunMonster : CustomMonsterModel
{
    private string DefaultVisualPath =>
        SceneHelper.GetScenePath($"creature_visuals/{Id.Entry.ToLowerInvariant()}");

    private const string FallbackVisualPath =
        "res://scenes/creature_visuals/fuzzy_wurm_crawler.tscn";

    public override int MinInitialHp => MaxInitialHp;

    private static IReadOnlyList<Player> GetPlayers(IReadOnlyList<Creature> creatures)
    {
        OrderedSet<Player> players = [];
        foreach (var creature in creatures)
        {
            var player = creature.Player ?? creature.PetOwner;
            if (player != null)
                players.Add(player);
        }

        return [.. players];
    }

    protected async Task GiveStatusCards<T>(
        IReadOnlyList<Creature> targets,
        IReadOnlyList<StatusCardArgs> args
    )
        where T : CardModel
    {
        foreach (var player in GetPlayers(targets))
        {
            List<CardPileAddResult> results = [];
            foreach (var arg in args)
                results.AddRange(await arg.AddToCombat(CombatState, ModelDb.Card<T>(), player));

            if (!LocalContext.IsMe(player))
                continue;

            CardCmd.PreviewCardPileAdd(results);
            await Cmd.Wait(1f);
        }
    }

    protected Task PowerUp<T>(int amount = 1)
        where T : PowerModel =>
        PowerCmd.Apply<T>(new ThrowingPlayerChoiceContext(), Creature, amount, Creature, null);

    public override string? CustomVisualPath =>
        ResourceLoader.Exists(DefaultVisualPath) ? DefaultVisualPath : FallbackVisualPath;

    protected record StatusCardArgs(
        PileType Type,
        CardPilePosition Position = CardPilePosition.Random,
        int Count = 1
    )
    {
        public async Task<IEnumerable<CardPileAddResult>> AddToCombat(
            ICombatState combatState,
            CardModel canonical,
            Player owner
        )
        {
            List<CardPileAddResult> results = [];
            for (var i = 0; i < Count; i++)
            {
                var card = combatState.CreateCard(canonical, owner);
                results.Add(await CardPileCmd.AddGeneratedCardToCombat(card, Type, null, Position));
            }

            return results;
        }
    }
}
