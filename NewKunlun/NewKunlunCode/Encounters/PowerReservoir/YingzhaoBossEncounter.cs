using MegaCrit.Sts2.Core.Rooms;
using NewKunlun.NewKunlunCode.Acts;
using NewKunlun.NewKunlunCode.Monsters.PowerReservoir;

namespace NewKunlun.NewKunlunCode.Encounters.PowerReservoir;

public class YingzhaoBossEncounter()
    : SingleMonsterEncounter<PowerReservoirAct, YingzhaoMonster>(RoomType.Boss) { }
