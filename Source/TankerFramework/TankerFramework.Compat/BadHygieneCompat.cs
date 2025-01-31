using System;
using HarmonyLib;
using Verse;

namespace TankerFramework.Compat;

[HotSwappable]
public static class BadHygieneCompat
{
    private static Type compPipeType;

    private static FastInvokeHandler pipeNetGetter;

    private static FastInvokeHandler pushWaterMethod;

    private static FastInvokeHandler pullWaterMethod;

    private static Type mapComponentType;

    private static AccessTools.FieldRef<object, bool> markTowerForDrawField;

    public static bool IsActive { get; private set; }

    public static void Init()
    {
        if (IsActive)
        {
            return;
        }

        pipeNetGetter =
            MethodInvoker.GetHandler(
                AccessTools.PropertyGetter(compPipeType = AccessTools.TypeByName("DubsBadHygiene.CompPipe"),
                    "pipeNet"));
        var type = AccessTools.TypeByName("DubsBadHygiene.PlumbingNet");
        pushWaterMethod = MethodInvoker.GetHandler(AccessTools.Method(type, "PushWater"));
        pullWaterMethod = MethodInvoker.GetHandler(AccessTools.Method(type, "PullWater"));
        markTowerForDrawField = AccessTools.FieldRefAccess<bool>(
            mapComponentType = AccessTools.TypeByName("DubsBadHygiene.MapComponent_Hygiene"), "MarkTowersForDraw");
        IsActive = true;
    }

    public static void HandleTick(CompTankerBase tanker, TankType type)
    {
        if (IsActive)
        {
            HandleTickPrivate(tanker, type);
        }
    }

    private static void HandleTickPrivate(CompTankerBase tanker, TankType type)
    {
        var comp = tanker.parent.GetComp(compPipeType);
        if (comp == null)
        {
            return;
        }

        if (tanker.IsDraining(type) == true)
        {
            var storedAmount = tanker.GetStoredAmount(type);
            if (storedAmount <= 0.0)
            {
                tanker.SetDraining(type, false);
                return;
            }

            var num = Math.Min(storedAmount, tanker.Props.drainAmount);
            if (!(num > 0.0))
            {
                return;
            }

            storedAmount -= num;
            storedAmount += (float)pushWaterMethod(pipeNetGetter(comp), (float)num);
            tanker.SetStoredAmount(type, storedAmount);
        }
        else
        {
            if (tanker.IsFilling(type) != true)
            {
                return;
            }

            var storedAmount2 = tanker.GetStoredAmount(TankType.All);
            if (storedAmount2 >= tanker.Props.storageCap)
            {
                tanker.SetFilling(type, false);
                return;
            }

            var val = Math.Min(tanker.Props.storageCap - storedAmount2, tanker.Props.fillAmount);
            val = Math.Max(val, 0.0);
            if ((bool)pullWaterMethod(pipeNetGetter(comp), (float)val, 0))
            {
                tanker.SetStoredAmount(type, tanker.GetStoredAmount(type) + val);
            }
        }
    }

    public static void MarkForDrawing(Map map)
    {
        var comp = map.GetComp(mapComponentType);
        if (comp != null)
        {
            markTowerForDrawField(comp) = true;
        }
    }
}