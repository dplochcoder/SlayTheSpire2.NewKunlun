namespace NewKunlun.NewKunlunCode.Monsters;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
public sealed class InitialStateAttribute(double weight = 1D) : Attribute
{
    public double Weight { get; } = weight;
}
