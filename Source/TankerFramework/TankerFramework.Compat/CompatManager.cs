using System;
using Verse;

namespace TankerFramework.Compat;

public static class CompatManager
{
    public static bool AnyActive => BadHygieneCompat.IsActive || RimefellerCompat.IsActive;

    public static bool IsActive(TankType tankType)
    {
        switch (tankType)
        {
            case TankType.Water:
                return BadHygieneCompat.IsActive;
            case TankType.Fuel:
            case TankType.Oil:
                return RimefellerCompat.IsActive;
            default:
                return true;
        }
    }

    public static TaggedString GetTranslatedTankName(TankType tankType)
    {
        return (tankType switch
        {
            TankType.Fuel => "TankerFrameworkFuelStorage",
            TankType.Oil => "TankerFrameworkOilStorage",
            TankType.Water => "TankerFrameworkWaterStorage",
            TankType.Helixien => "TankerFrameworkHelixienStorage",
            _ => throw new ArgumentOutOfRangeException(nameof(tankType), tankType, "Invalid tanker contents")
        }).Translate();
    }
}