using System;
using HarmonyLib;
using Verse;

namespace TankerFramework.Compat;

[HotSwappable]
public static class RimefellerCompat
{
    private static Type compPipeType;

    private static FastInvokeHandler pipeNetGetter;

    private static FastInvokeHandler pushFuelMethod;

    private static FastInvokeHandler pullFuelMethod;

    private static FastInvokeHandler pushOilMethod;

    private static FastInvokeHandler pullOilMethod;

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
                AccessTools.PropertyGetter(compPipeType = AccessTools.TypeByName("Rimefeller.CompPipe"),
                    "pipeNet"));
        var type = AccessTools.TypeByName("Rimefeller.PipelineNet");
        pushFuelMethod = MethodInvoker.GetHandler(AccessTools.Method(type, "PushFuel"));
        pullFuelMethod = MethodInvoker.GetHandler(AccessTools.Method(type, "PullFuel"));
        pushOilMethod = MethodInvoker.GetHandler(AccessTools.Method(type, "PushCrude"));
        pullOilMethod = MethodInvoker.GetHandler(AccessTools.Method(type, "PullOil"));
        markTowerForDrawField = AccessTools.FieldRefAccess<bool>(
            mapComponentType = AccessTools.TypeByName("Rimefeller.MapComponent_Rimefeller"), "MarkTowersForDraw");
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
            var num2 = storedAmount;
            storedAmount = num2 + type switch
            {
                TankType.Fuel => (float)pushFuelMethod(pipeNetGetter(comp), (float)num),
                TankType.Oil => (double)pushOilMethod(pipeNetGetter(comp), num),
                _ => num
            };
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
            if (type switch
                {
                    TankType.Fuel => (bool)pullFuelMethod(pipeNetGetter(comp), val),
                    TankType.Oil => (bool)pullOilMethod(pipeNetGetter(comp), val),
                    _ => false
                })
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