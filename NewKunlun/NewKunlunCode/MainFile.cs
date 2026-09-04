using System.Reflection;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Modding;
using NewKunlun.NewKunlunCode.Localization;
using SmartFormat;
using SmartFormat.Core.Extensions;
using Logger = MegaCrit.Sts2.Core.Logging.Logger;

namespace NewKunlun.NewKunlunCode;

//You're recommended but not required to keep all your code in this package and all your assets in the NewKunlun folder.
[ModInitializer(nameof(Initialize))]
public partial class MainFile : Node
{
    public const string ModId = "NewKunlun"; //Used for resource filepath
    public const string ResPath = $"res://{ModId}";

    public static Logger Logger { get; } = new(ModId, LogType.Generic);

    public static void Initialize()
    {
        var assembly = Assembly.GetExecutingAssembly();

        Smart.Default.AddExtensions([
            .. assembly
                .GetTypes()
                .Where(t => t.GetCustomAttribute<CustomFormatterAttribute>() != null)
                .Select(Activator.CreateInstance)
                .OfType<IFormatter>(),
        ]);

        //If you want to use scripts defined in your mod for Godot scenes, uncomment the following line.
        //Godot.Bridge.ScriptManagerBridge.LookupScriptsInAssembly(assembly);

        Harmony harmony = new(ModId);

        harmony.PatchAll(assembly);
    }
}
