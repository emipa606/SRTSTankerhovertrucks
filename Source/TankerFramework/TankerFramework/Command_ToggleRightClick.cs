using System;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace TankerFramework;

public class Command_ToggleRightClick : Command_ActionRightClick
{
    public Func<bool?> isActive;

    public Action toggleAction;

    public override SoundDef CurActivateSound =>
        isActive() == true ? SoundDefOf.Checkbox_TurnedOff : SoundDefOf.Checkbox_TurnedOn;

    public override void ProcessInput(Event ev)
    {
        if (!openOnLeftClick || rightClickFloatMenuOptions.Count <= 1)
        {
            toggleAction();
        }
        else
        {
            OpenMenu();
        }

        CurActivateSound?.PlayOneShotOnCamera();
    }

    public override GizmoResult GizmoOnGUI(Vector2 loc, float maxWidth, GizmoRenderParms parms)
    {
        var result = base.GizmoOnGUI(loc, maxWidth, parms);
        var rect = new Rect(loc.x, loc.y, GetWidth(maxWidth), 75f);
        var position = new Rect(rect.x + rect.width - 24f, rect.y, 24f, 24f);
        var texture2D = !isActive().HasValue ? Widgets.CheckboxPartialTex :
            isActive() != true ? Widgets.CheckboxOffTex : Widgets.CheckboxOnTex;
        GUI.DrawTexture(position, texture2D);
        return result;
    }

    public override bool InheritInteractionsFrom(Gizmo other)
    {
        return false;
    }
}