using MegaCrit.Sts2.Core.Rooms;
using NewKunlun.NewKunlunCode.Acts;
using NewKunlun.NewKunlunCode.Monsters.PowerReservoir;

namespace NewKunlun.NewKunlunCode.Encounters.PowerReservoir;

public class BladeTrooperWeakEncounter()
    : SingleMonsterEncounter<PowerReservoirAct, BladeTrooperMonster>(RoomType.Monster)
{
    public override bool IsWeak => true;
}
