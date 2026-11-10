using MegaCrit.Sts2.Core.Rooms;
using NewKunlun.NewKunlunCode.Acts;
using NewKunlun.NewKunlunCode.Monsters.PowerReservoir;

namespace NewKunlun.NewKunlunCode.Encounters.PowerReservoir;

public class BladeCommanderWeakEncounter()
    : SingleMonsterEncounter<PowerReservoirAct, BladeCommanderMonster>(RoomType.Monster)
{
    public override bool IsWeak => true;
}
