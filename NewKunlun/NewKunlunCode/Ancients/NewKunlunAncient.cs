using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Models;
using NewKunlun.NewKunlunCode.Relics;

namespace NewKunlun.NewKunlunCode.Ancients;

public abstract class NewKunlunAncient : CustomAncientModel
{
    protected override OptionPools MakeOptionPools =>
        throw new NotSupportedException(nameof(MakeOptionPools));

    public static IEnumerable<AncientEventModel> ActOneAncients =>
        [ModelDb.AncientEvent<AbacusAncient>()];

    public static IEnumerable<AncientEventModel> ActTwoAncients =>
        [ModelDb.AncientEvent<KuafuAncient>(), ModelDb.AncientEvent<ShennongAncient>()];

    public static IEnumerable<AncientEventModel> ActThreeAncients =>
        [ModelDb.AncientEvent<HengAncient>(), ModelDb.AncientEvent<LearAncient>()];

    protected abstract List<List<EventOption>> PositiveOptions { get; }

    protected abstract List<EventOption> CurseOptions { get; }

    private void Filter(List<EventOption> options) =>
        options.RemoveAll(o =>
            !o.Relic!.IsAllowed(Owner!.RunState)
            || (o.Relic is NewKunlunRelic r && !r.IsAllowedForPlayer(Owner!))
        );

    private void Filter(List<List<EventOption>> options)
    {
        foreach (var list in options)
            Filter(list);
        options.RemoveAll(o => o.Count == 0);
    }

    protected virtual bool IsCurseAllowed(
        RelicModel curse,
        IReadOnlyCollection<RelicModel> positive
    ) => true;

    protected override IReadOnlyList<EventOption> GenerateInitialOptions()
    {
        var positive = PositiveOptions;
        Filter(positive);
        Rng.Shuffle(positive);
        List<EventOption> options = [Rng.NextItem(positive[0])!, Rng.NextItem(positive[1])!];
        HashSet<RelicModel> chosen = [.. options.Select(o => o.Relic!)];

        var curse = CurseOptions;
        Filter(curse);
        curse.RemoveAll(o => !IsCurseAllowed(o.Relic!, chosen));
        options.Add(Rng.NextItem(curse)!);
        return options;
    }
}
