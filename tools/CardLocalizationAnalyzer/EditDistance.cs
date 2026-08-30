namespace CardLocalizationAnalyzer;

public static class EditDistance
{
    public static bool FindClosest(
        string unknown,
        IEnumerable<string> candidates,
        out string bestMatch
    )
    {
        var best = candidates
            .Select(candidate => (candidate, distance: GetDistance(unknown, candidate)))
            .OrderBy(item => item.distance)
            .FirstOrDefault();

        bestMatch = best.candidate ?? "";
        return best.candidate != null && best.distance <= Math.Max(2, unknown.Length / 3);
    }

    private static int GetDistance(string left, string right)
    {
        var previous = Enumerable.Range(0, right.Length + 1).ToArray();
        for (var i = 1; i <= left.Length; i++)
        {
            var current = new int[right.Length + 1];
            current[0] = i;
            for (var j = 1; j <= right.Length; j++)
                current[j] = Math.Min(
                    Math.Min(current[j - 1] + 1, previous[j] + 1),
                    previous[j - 1] + (left[i - 1] == right[j - 1] ? 0 : 1)
                );
            previous = current;
        }
        return previous[right.Length];
    }
}
