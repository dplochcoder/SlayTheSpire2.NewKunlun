namespace NewKunlun.NewKunlunCode.Monsters.Intents;

public class MultiCrimsonAttackIntent : CrimsonAttackIntent
{
    private readonly Func<int> _repeatCalc;

    public override int Repeats => _repeatCalc();

    public MultiCrimsonAttackIntent(Func<decimal> damage, Func<int> repeat)
    {
        DamageCalc = damage;
        _repeatCalc = repeat;
    }

    public MultiCrimsonAttackIntent(decimal damage, Func<int> repeat)
        : this(() => damage, repeat) { }

    public MultiCrimsonAttackIntent(Func<decimal> damage, int repeat)
        : this(damage, () => repeat) { }

    public MultiCrimsonAttackIntent(decimal damage, int repeat)
        : this(() => damage, () => repeat) { }
}
