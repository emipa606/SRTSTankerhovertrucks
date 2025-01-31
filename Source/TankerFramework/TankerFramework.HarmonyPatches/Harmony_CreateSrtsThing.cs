using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using Verse;

namespace TankerFramework.HarmonyPatches;

[HarmonyPatch]
public static class Harmony_CreateSrtsThing
{
    [UsedImplicitly]
    private static IEnumerable<MethodInfo> TargetMethods()
    {
        var type = AccessTools.TypeByName("SRTS.CompLaunchableSRTS");
        yield return AccessTools.Method(type, "TryLaunch");
        type = AccessTools.TypeByName("SRTS.CompBombFlyer");
        yield return AccessTools.Method(type, "TryLaunchBombRun");
    }

    [UsedImplicitly]
    private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> codeInstructions,
        MethodBase parentMethod)
    {
        var target = AccessTools.Method(typeof(Harmony_CreateSrtsThing), "SetupThing");
        var targetIndex = parentMethod.Name == "TryLaunch" ? 10 : 7;
        foreach (var ci in codeInstructions)
        {
            yield return ci;
            if (ci.opcode != OpCodes.Stloc_S || ci.operand is not LocalBuilder localBuilder ||
                localBuilder.LocalIndex != targetIndex)
            {
                continue;
            }

            yield return new CodeInstruction(OpCodes.Ldarg_0);
            yield return new CodeInstruction(OpCodes.Ldloc_S, ci.operand);
            yield return new CodeInstruction(OpCodes.Call, target);
        }
    }

    private static void SetupThing(ThingComp parent, Thing thing)
    {
        var array = parent.parent.GetComps<CompTankerBase>()?.ToArray();
        if (array == null || array.Length == 0 || thing is not ThingWithComps thingWithComps)
        {
            return;
        }

        var array2 = thingWithComps.GetComps<CompTankerBase>().ToArray();
        foreach (var other in array)
        {
            for (var j = 0; j < array2.Length && !array2[j].TransferFrom(other); j++)
            {
            }
        }
    }
}