using System;
using System.Collections.Generic;
using System.Text;
using Multiplayer.API;
using TankerFramework.Compat;
using UnityEngine;
using Verse;

namespace TankerFramework;

[HotSwappable]
public class CompTanker : CompTankerBase
{
    private Command_Action gizmoDebugEmpty;
    private Command_Action gizmoDebugFill;

    private Command_Toggle gizmoToggleDrain;

    private Command_Toggle gizmoToggleFill;

    public bool isDraining;

    public bool isFilling;

    public double storedAmount;

    protected override float CapPercent => (float)(storedAmount / Props.storageCap);

    public new CompProperties_Tanker Props => (CompProperties_Tanker)props;

    private Command_Action GizmoDebugFill
    {
        get
        {
            var command_Action = gizmoDebugFill;
            if (command_Action != null)
            {
                return command_Action;
            }

            var obj = new Command_Action
            {
                action = DebugFill,
                defaultLabel = "Dev: Fill"
            };
            gizmoDebugFill = obj;
            command_Action = obj;

            return command_Action;
        }
    }

    private Command_Action GizmoDebugEmpty
    {
        get
        {
            var command_Action = gizmoDebugEmpty;
            if (command_Action != null)
            {
                return command_Action;
            }

            var obj = new Command_Action
            {
                action = DebugEmpty,
                defaultLabel = "Dev: Empty"
            };
            gizmoDebugEmpty = obj;
            command_Action = obj;

            return command_Action;
        }
    }

    private Command_Toggle GizmoToggleDrain
    {
        get
        {
            var command_Toggle = gizmoToggleDrain;
            if (command_Toggle != null)
            {
                return command_Toggle;
            }

            var obj = new Command_Toggle
            {
                isActive = () => isDraining,
                toggleAction = ToggleDrain,
                defaultLabel = "TankerFrameworkToggleDrain".Translate(),
                defaultDesc = "TankerFrameworkToggleDrainDesc".Translate(),
                icon = ContentFinder<Texture2D>.Get(Props.drainGizmoPath)
            };
            gizmoToggleDrain = obj;
            command_Toggle = obj;

            return command_Toggle;
        }
    }

    private Command_Toggle GizmoToggleFill
    {
        get
        {
            var command_Toggle = gizmoToggleFill;
            if (command_Toggle != null)
            {
                return command_Toggle;
            }

            var obj = new Command_Toggle
            {
                isActive = () => isFilling,
                toggleAction = ToggleFill,
                defaultLabel = "TankerFrameworkToggleFill".Translate(),
                defaultDesc = "TankerFrameworkToggleFillDesc".Translate(),
                icon = ContentFinder<Texture2D>.Get(Props.fillGizmoPath)
            };
            gizmoToggleFill = obj;
            command_Toggle = obj;

            return command_Toggle;
        }
    }

    public override bool? IsDraining(TankType type)
    {
        if (!parent.Spawned)
        {
            return null;
        }

        return isDraining;
    }

    public override void SetDraining(TankType type, bool value)
    {
        if (parent.Spawned)
        {
            isDraining = value;
        }
    }

    public override bool? IsFilling(TankType type)
    {
        if (!parent.Spawned)
        {
            return null;
        }

        return isFilling;
    }

    public override void SetFilling(TankType type, bool value)
    {
        if (parent.Spawned)
        {
            isFilling = value;
        }
    }

    public override double GetStoredAmount(TankType type)
    {
        return !parent.Spawned ? 0.0 : storedAmount;
    }

    public override void SetStoredAmount(TankType type, double count)
    {
        if (parent.Spawned)
        {
            storedAmount = count;
        }
    }

    public override bool TransferFrom(CompTankerBase other)
    {
        if (other is not CompTanker compTanker)
        {
            return false;
        }

        if (Props.contents != compTanker.Props.contents)
        {
            return false;
        }

        storedAmount = compTanker.storedAmount;
        return true;
    }

    [SyncMethod]
    internal void ToggleFill()
    {
        isDraining = false;
        isFilling = !isFilling;
    }

    [SyncMethod]
    internal void ToggleDrain()
    {
        isFilling = false;
        isDraining = !isDraining;
    }

    [SyncMethod(debugOnly = true)]
    internal void DebugFill()
    {
        storedAmount = Props.storageCap;
    }

    [SyncMethod(debugOnly = true)]
    internal void DebugEmpty()
    {
        storedAmount = 0.0;
    }

    public override void PostDrawExtraSelectionOverlays()
    {
        base.PostDrawExtraSelectionOverlays();
        switch (Props.contents)
        {
            case TankType.Fuel:
            case TankType.Oil:
                RimefellerCompat.MarkForDrawing(parent.Map);
                break;
            case TankType.Water:
                BadHygieneCompat.MarkForDrawing(parent.Map);
                break;
            default:
                throw new ArgumentOutOfRangeException("contents", Props.contents, "Invalid tanker contents");
            case TankType.Helixien:
                break;
        }
    }

    public override void CompTick()
    {
        base.CompTick();
        switch (Props.contents)
        {
            case TankType.Fuel:
            case TankType.Oil:
                RimefellerCompat.HandleTick(this, Props.contents);
                break;
            case TankType.Water:
                BadHygieneCompat.HandleTick(this, Props.contents);
                break;
            default:
                throw new ArgumentOutOfRangeException("contents", Props.contents, "Invalid tanker contents");
        }
    }

    public override IEnumerable<Gizmo> CompGetGizmosExtra()
    {
        foreach (var item in base.CompGetGizmosExtra())
        {
            yield return item;
        }

        yield return GizmoToggleDrain;
        yield return GizmoToggleFill;
        if (!Prefs.DevMode)
        {
            yield break;
        }

        yield return GizmoDebugEmpty;
        yield return GizmoDebugFill;
    }

    public override void PostExposeData()
    {
        base.PostExposeData();
        Scribe_Values.Look(ref storedAmount, "storedAmount");
        Scribe_Values.Look(ref isDraining, "isDraining");
        Scribe_Values.Look(ref isFilling, "isFilling");
    }

    public override string CompInspectStringExtra()
    {
        if (!parent.Spawned)
        {
            return string.Empty;
        }

        var stringBuilder = new StringBuilder();
        stringBuilder.Append(CompatManager.GetTranslatedTankName(Props.contents));
        stringBuilder.Append(' ');
        stringBuilder.Append(storedAmount.ToString("0.0"));
        stringBuilder.Append('/');
        stringBuilder.Append(Props.storageCap);
        stringBuilder.AppendLine();
        if (isFilling)
        {
            stringBuilder.AppendLine("TankerFrameworkFillingInspect".Translate());
        }
        else if (isDraining)
        {
            stringBuilder.AppendLine("TankerFrameworkDrainingInspect".Translate());
        }

        return stringBuilder.ToString().TrimEndNewlines();
    }
}