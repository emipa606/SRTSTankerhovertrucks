using System.Collections.Generic;
using System.Linq;
using TankerFramework.Compat;
using Verse;

namespace TankerFramework;

public class CompProperties_TankerMulti : CompProperties_TankerBase
{
    public List<TankType> tankTypes;

    public CompProperties_TankerMulti()
    {
        compClass = typeof(CompTankerMulti);
    }

    public override IEnumerable<string> ConfigErrors(ThingDef parentDef)
    {
        foreach (var item in base.ConfigErrors(parentDef))
        {
            yield return item;
        }

        if (tankTypes == null || !tankTypes.Any())
        {
            tankTypes = [TankType.All];
            yield break;
        }

        if (tankTypes.Count == 1 && tankTypes[0] == TankType.All)
        {
            tankTypes.Clear();
            tankTypes.AddRange(from x in Enumerable.Range(1, 4)
                select (TankType)x);
        }

        tankTypes.RemoveAll(x => !CompatManager.IsActive(x));
        foreach (var tankType in tankTypes)
        {
            if (tankType <= TankType.Invalid || tankType >= TankType.All)
            {
                yield return $"{tankTypes} contains contents of illegal type: {tankType}";
            }
        }

        tankTypes.RemoveAll(contents => contents <= TankType.Invalid || contents >= TankType.All);
    }
}