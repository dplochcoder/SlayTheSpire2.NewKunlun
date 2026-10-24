using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;

namespace NewKunlun.NewKunlunCode.Encounters;

public abstract class NewKunlunEncounter(RoomType roomType, bool autoAdd = true)
    : CustomEncounterModel(roomType, autoAdd) { }

public abstract class NewKunlunEncounter<T>(RoomType roomType, bool autoAdd = true)
    : NewKunlunEncounter(roomType, autoAdd)
    where T : ActModel
{
    public override bool IsValidForAct(ActModel act) => act is T;
}
