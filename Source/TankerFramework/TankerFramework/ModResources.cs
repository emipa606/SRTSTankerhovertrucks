using UnityEngine;
using Verse;

namespace TankerFramework;

[StaticConstructorOnStartup]
public static class ModResources
{
    public static readonly Vector2 BarSize = new Vector2(1.7f, 0.2f);

    public static readonly Material BarFilledMat = SolidColorMaterials.SimpleSolidColorMaterial(Color.cyan);

    public static readonly Material BarUnfilledMat =
        SolidColorMaterials.SimpleSolidColorMaterial(new Color(0.1f, 0.1f, 0.1f));
}