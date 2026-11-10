using BaseLib.Abstracts;
using Godot;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Acts;
using MegaCrit.Sts2.Core.Random;
using MegaCrit.Sts2.Core.Rooms;

namespace NewKunlun.NewKunlunCode.Encounters;

public abstract class NewKunlunEncounter<TAct>(RoomType roomType, bool autoAdd = true)
    : CustomEncounterModel(roomType, autoAdd)
    where TAct : ActModel
{
    private const string DefaultBossNodePath = "res://images/map/placeholder/aeonglass_boss_icon";

    public override bool IsValidForAct(ActModel act) => act is TAct;

    public override BackgroundAssets CustomEncounterBackground(ActModel parentAct, Rng rng)
    {
        var backgroundName = Id.Entry.ToLowerInvariant();
        var layersPath = $"res://scenes/backgrounds/{backgroundName}/layers";
        var scenePath =
            $"res://scenes/backgrounds/{backgroundName}/{backgroundName}_background.tscn";
        return DirAccess.DirExistsAbsolute(layersPath) && ResourceLoader.Exists(scenePath)
            ? new BackgroundAssets(backgroundName, rng)
            : ModelDb.Act<Overgrowth>().GenerateBackgroundAssets(rng);
    }

    public override string BossNodePath =>
        RoomType == RoomType.Boss && !HasBossNodeImages() ? DefaultBossNodePath : base.BossNodePath;

    private bool HasBossNodeImages()
    {
        var path = base.BossNodePath;
        return ResourceLoader.Exists($"{path}.png") && ResourceLoader.Exists($"{path}_outline.png");
    }
}
