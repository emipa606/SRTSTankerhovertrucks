using System.Linq;
using HarmonyLib;
using JetBrains.Annotations;
using Multiplayer.API;
using TankerFramework.Compat;
using Verse;

namespace TankerFramework;

[UsedImplicitly]
public class TankerFrameworkMod : Mod
{
    public TankerFrameworkMod(ModContentPack content)
        : base(content)
    {
        if (MP.enabled)
        {
            MP.RegisterAll();
        }

        if (IsModLoaded("dubwise.dubsbadhygiene"))
        {
            BadHygieneCompat.Init();
        }

        if (IsModLoaded("dubwise.rimefeller"))
        {
            RimefellerCompat.Init();
        }

        LongEventHandler.ExecuteWhenFinished(delegate
        {
            var harmony = new Harmony("Dra.CompTankerMod");
            if (!harmony.GetPatchedMethods().Any())
            {
                harmony.PatchAll();
            }
        });
    }

    private static bool IsModLoaded(string s)
    {
        return LoadedModManager.RunningMods.Any(x => x.PackageId.ToLower().NoModIdSuffix() == s);
    }
}