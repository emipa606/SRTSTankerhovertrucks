using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using JetBrains.Annotations;
using TankerFramework;
using Verse;

namespace CompTankerCompat.HarmonyPatches;

internal static class Harmony_Rimefeller_DrawOverlay
{
    private static AccessTools.FieldRef<object, bool> markTowersForDrawField;

    private static AccessTools.FieldRef<object, IList> pipeNetsField;

    private static AccessTools.FieldRef<object, List<ThingWithComps>> pipedThingsField;

    [UsedImplicitly]
    public static MethodInfo TargetMethod()
    {
        var type = AccessTools.TypeByName("Rimefeller.MapComponent_Rimefeller");
        if (type == null)
        {
            return null;
        }

        markTowersForDrawField = AccessTools.FieldRefAccess<bool>(type, "MarkTowersForDraw");
        pipeNetsField = AccessTools.FieldRefAccess<IList>(type, "PipeNets");
        pipedThingsField =
            AccessTools.FieldRefAccess<List<ThingWithComps>>(AccessTools.TypeByName("Rimefeller.PipelineNet"),
                "PipedThings");
        return AccessTools.Method(type, "MapComponentUpdate");
    }

    [UsedImplicitly]
    public static void Prefix(MapComponent __instance)
    {
        if (!markTowersForDrawField(__instance))
        {
            return;
        }

        foreach (var item in pipeNetsField(__instance))
        {
            foreach (var item2 in pipedThingsField(item))
            {
                var comp = item2.GetComp<CompTankerBase>();
                if (comp != null)
                {
                    comp.drawOverlay = true;
                }
            }
        }
    }
}