namespace NewKunlun.NewKunlunCode.Monsters;

[AttributeUsage(AttributeTargets.Method, Inherited = false)]
public sealed class InitialMoveAttribute(double weight = 1D) : Attribute
{
    public double Weight { get; } = weight;
}
