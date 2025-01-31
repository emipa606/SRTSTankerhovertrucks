using System.Collections.Generic;
using Verse;

namespace TankerFramework;

public abstract class CompProperties_TankerBase : CompProperties
{
    public readonly string drainGizmoPath = "Things/UI/Drain";

    public readonly string fillGizmoPath = "Things/UI/Fill";
    public double drainAmount = 0.5;

    public double fillAmount = 0.5;
    public double storageCap = 10000.0;

    public override IEnumerable<string> ConfigErrors(ThingDef parentDef)
    {
        foreach (var item in base.ConfigErrors(parentDef))
        {
            yield return item;
        }

        switch (storageCap)
        {
            case 0.0:
                yield return $"{storageCap} cannot be 0";
                break;
            case < 0.0:
                storageCap = 0.0 - storageCap;
                break;
        }

        switch (fillAmount)
        {
            case 0.0:
                yield return $"{fillAmount} cannot be 0";
                break;
            case < 0.0:
                fillAmount = 0.0 - fillAmount;
                break;
        }

        switch (drainAmount)
        {
            case 0.0:
                yield return $"{drainAmount} cannot be 0";
                break;
            case < 0.0:
                drainAmount = 0.0 - drainAmount;
                break;
        }

        if (string.IsNullOrWhiteSpace(fillGizmoPath))
        {
            yield return $"{fillGizmoPath} is empty";
        }

        if (string.IsNullOrWhiteSpace(drainGizmoPath))
        {
            yield return $"{drainGizmoPath} is empty";
        }
    }
}