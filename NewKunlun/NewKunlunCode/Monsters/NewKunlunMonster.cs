using BaseLib.Abstracts;

namespace NewKunlun.NewKunlunCode.Monsters;

public abstract class NewKunlunMonster : CustomMonsterModel
{
    public override int MinInitialHp => MaxInitialHp;
}
