using MegaCrit.Sts2.Core.Rooms;
using NewKunlun.NewKunlunCode.Acts;
using NewKunlun.NewKunlunCode.Monsters.PowerReservoir;

namespace NewKunlun.NewKunlunCode.Encounters.PowerReservoir;

public class BladeTrooperWeakEncounter(bool autoAdd = true)
    : SingleMonsterEncounter<PowerReservoirAct, BladeTrooperMonster>(RoomType.Monster, autoAdd)
{
    public override bool IsWeak => true;
}
