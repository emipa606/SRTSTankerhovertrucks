using System.Collections.Generic;
using Verse;

namespace TankerFramework;

public class CompProperties_Tanker : CompProperties_TankerBase
{
    public TankType contents;

    public CompProperties_Tanker()
    {
        compClass = typeof(CompTanker);
    }

    public override IEnumerable<string> ConfigErrors(ThingDef parentDef)
    {
        foreach (var item in base.ConfigErrors(parentDef))
        {
            yield return item;
        }

        var tankType = contents;
        if (tankType <= TankType.Invalid || tankType >= TankType.All)
        {
            yield return $"{contents} is of illegal type: {contents}";
        }
    }
}