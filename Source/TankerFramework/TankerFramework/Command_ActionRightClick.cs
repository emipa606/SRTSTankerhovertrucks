using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace TankerFramework;

public class Command_ActionRightClick : Command_Action
{
    public bool openOnLeftClick;

    public List<FloatMenuOption> rightClickFloatMenuOptions;

    public override IEnumerable<FloatMenuOption> RightClickFloatMenuOptions => rightClickFloatMenuOptions;

    protected void OpenMenu()
    {
        if (rightClickFloatMenuOptions == null)
        {
            return;
        }

        var window = new FloatMenu(rightClickFloatMenuOptions);
        Find.WindowStack.Add(window);
    }

    public override void ProcessInput(Event ev)
    {
        if (!openOnLeftClick)
        {
            base.ProcessInput(ev);
            return;
        }

        if (rightClickFloatMenuOptions.Count == 1)
        {
            rightClickFloatMenuOptions[0].action();
            CurActivateSound?.PlayOneShotOnCamera();
            return;
        }

        OpenMenu();
        CurActivateSound?.PlayOneShotOnCamera();
    }
}