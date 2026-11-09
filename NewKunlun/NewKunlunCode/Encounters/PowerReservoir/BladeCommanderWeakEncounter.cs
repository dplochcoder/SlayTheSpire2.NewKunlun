using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;
using NewKunlun.NewKunlunCode.Acts;
using NewKunlun.NewKunlunCode.Monsters.PowerReservoir;

namespace NewKunlun.NewKunlunCode.Encounters.PowerReservoir;

public class BladeCommanderWeakEncounter(bool autoAdd = true)
    : SingleMonsterEncounter<PowerReservoirAct, BladeCommanderMonster>(RoomType.Monster, autoAdd)
{
    public override bool IsWeak => true;
}
