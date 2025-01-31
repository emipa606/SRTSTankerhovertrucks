using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Multiplayer.API;
using TankerFramework.Compat;
using UnityEngine;
using Verse;

namespace TankerFramework;

[HotSwappable]
public class CompTankerMulti : CompTankerBase
{
    private Command_ActionRightClick gizmoDebugEmpty;

    private Command_ActionRightClick gizmoDebugFill;

    private Command_ToggleRightClick gizmoToggleDrain;

    private Command_ToggleRightClick gizmoToggleFill;

    public Dictionary<TankType, bool> isDraining;

    public Dictionary<TankType, bool> isFilling;
    public Dictionary<TankType, double> storedAmount;

    protected override float CapPercent => (float)(storedAmount.Sum(x => x.Value) / Props.storageCap);

    public new CompProperties_TankerMulti Props => (CompProperties_TankerMulti)props;

    private Command_ActionRightClick GizmoDebugFill
    {
        get
        {
            var command_ActionRightClick = gizmoDebugFill;
            if (command_ActionRightClick != null)
            {
                return command_ActionRightClick;
            }

            var obj = new Command_ActionRightClick
            {
                openOnLeftClick = true,
                rightClickFloatMenuOptions = storedAmount.Keys
                    .Select(x => new FloatMenuOption($"Fill {x}", delegate { DebugFill(x); })).ToList(),
                defaultLabel = "Dev: Fill"
            };
            gizmoDebugFill = obj;
            command_ActionRightClick = obj;

            return command_ActionRightClick;
        }
    }

    private Command_ActionRightClick GizmoDebugEmpty
    {
        get
        {
            var command_ActionRightClick = gizmoDebugEmpty;
            if (command_ActionRightClick != null)
            {
                return command_ActionRightClick;
            }

            var obj = new Command_ActionRightClick
            {
                action = delegate { DebugEmpty(TankType.All); },
                rightClickFloatMenuOptions = storedAmount.Keys
                    .Select(x => new FloatMenuOption($"Empty {x}", delegate { DebugEmpty(x); })).ToList(),
                defaultLabel = "Dev: Empty"
            };
            gizmoDebugEmpty = obj;
            command_ActionRightClick = obj;

            return command_ActionRightClick;
        }
    }

    private Command_ToggleRightClick GizmoToggleDrain
    {
        get
        {
            var command_ToggleRightClick = gizmoToggleDrain;
            if (command_ToggleRightClick != null)
            {
                return command_ToggleRightClick;
            }

            var obj = new Command_ToggleRightClick
            {
                isActive = delegate
                {
                    var num = isDraining.Count(x => x.Value);
                    if (num == 0)
                    {
                        return false;
                    }

                    return num == isDraining.Count ? true : null;
                },
                toggleAction = delegate { ToggleDrain(TankType.All); },
                rightClickFloatMenuOptions = Props.tankTypes.Select(x =>
                    new FloatMenuOption(
                        "TankerFrameworkToggleSpecificDrain".Translate($"TankerFramework{x}".Translate()),
                        delegate { ToggleDrain(x); })).ToList(),
                defaultLabel = "TankerFrameworkToggleDrain".Translate(),
                defaultDesc = "TankerFrameworkToggleDrainDesc".Translate(),
                icon = ContentFinder<Texture2D>.Get(Props.drainGizmoPath)
            };
            gizmoToggleDrain = obj;
            command_ToggleRightClick = obj;

            return command_ToggleRightClick;
        }
    }

    private Command_ToggleRightClick GizmoToggleFill
    {
        get
        {
            var command_ToggleRightClick = gizmoToggleFill;
            if (command_ToggleRightClick != null)
            {
                return command_ToggleRightClick;
            }

            var obj = new Command_ToggleRightClick
            {
                isActive = delegate
                {
                    var num = isFilling.Count(x => x.Value);
                    if (num == 0)
                    {
                        return false;
                    }

                    return num == isFilling.Count ? true : null;
                },
                toggleAction = delegate { ToggleFill(TankType.All); },
                rightClickFloatMenuOptions = Props.tankTypes.Select(x =>
                    new FloatMenuOption(
                        "TankerFrameworkToggleSpecificFill".Translate($"TankerFramework{x}".Translate()),
                        delegate { ToggleFill(x); })).ToList(),
                defaultLabel = "TankerFrameworkToggleFill".Translate(),
                defaultDesc = "TankerFrameworkToggleFillDesc".Translate(),
                icon = ContentFinder<Texture2D>.Get(Props.fillGizmoPath)
            };
            gizmoToggleFill = obj;
            command_ToggleRightClick = obj;

            return command_ToggleRightClick;
        }
    }

    public override bool? IsDraining(TankType type)
    {
        if (!parent.Spawned)
        {
            return null;
        }

        if (type != TankType.All)
        {
            return isDraining[type];
        }

        var num = isDraining.Values.Count(x => x);
        if (num == 0)
        {
            return false;
        }

        if (num == isDraining.Count)
        {
            return true;
        }

        return null;
    }

    public override void SetDraining(TankType type, bool value)
    {
        if (!parent.Spawned)
        {
            return;
        }

        if (type != TankType.All)
        {
            isDraining[type] = value;
            return;
        }

        foreach (var tankType in Props.tankTypes)
        {
            isDraining[tankType] = value;
        }
    }

    public override bool? IsFilling(TankType type)
    {
        if (!parent.Spawned)
        {
            return null;
        }

        if (type != TankType.All)
        {
            return isFilling[type];
        }

        var num = isFilling.Values.Count(x => x);
        if (num == 0)
        {
            return false;
        }

        if (num == isFilling.Count)
        {
            return true;
        }

        return null;
    }

    public override void SetFilling(TankType type, bool value)
    {
        if (!parent.Spawned)
        {
            return;
        }

        if (type != TankType.All)
        {
            isFilling[type] = value;
            return;
        }

        foreach (var tankType in Props.tankTypes)
        {
            isFilling[tankType] = value;
        }
    }

    public override double GetStoredAmount(TankType type)
    {
        if (!parent.Spawned)
        {
            return 0.0;
        }

        return type != TankType.All ? storedAmount[type] : storedAmount.Values.Sum();
    }

    public override void SetStoredAmount(TankType type, double count)
    {
        if (parent.Spawned)
        {
            storedAmount[type] = count;
        }
    }

    public override bool TransferFrom(CompTankerBase other)
    {
        if (other is not CompTankerMulti compTankerMulti)
        {
            return false;
        }

        if (Props.tankTypes.Count != compTankerMulti.Props.tankTypes.Count)
        {
            return false;
        }

        for (var i = 0; i < Props.tankTypes.Count; i++)
        {
            if (Props.tankTypes[i] != compTankerMulti.Props.tankTypes[i])
            {
                return false;
            }
        }

        storedAmount = compTankerMulti.storedAmount;
        return true;
    }

    public override void PostSpawnSetup(bool respawningAfterLoad)
    {
        if (!CompatManager.AnyActive || Props.tankTypes == null || !Enumerable.Any(Props.tankTypes))
        {
            parent.AllComps.Remove(this);
            return;
        }

        if (storedAmount.NullOrEmpty())
        {
            storedAmount = Props.tankTypes.ToDictionary(x => x, _ => 0.0);
        }

        if (isDraining.NullOrEmpty())
        {
            isDraining = Props.tankTypes.ToDictionary(x => x, _ => false);
        }

        if (isFilling.NullOrEmpty())
        {
            isFilling = Props.tankTypes.ToDictionary(x => x, _ => false);
        }
    }

    public override void PostDrawExtraSelectionOverlays()
    {
        base.PostDrawExtraSelectionOverlays();
        foreach (var tankType in Props.tankTypes)
        {
            switch (tankType)
            {
                case TankType.Fuel:
                case TankType.Oil:
                    RimefellerCompat.MarkForDrawing(parent.Map);
                    break;
                case TankType.Water:
                    BadHygieneCompat.MarkForDrawing(parent.Map);
                    break;
                default:
                    throw new ArgumentOutOfRangeException("content", tankType, "Invalid tanker contents");
                case TankType.Helixien:
                    break;
            }
        }
    }

    public override void CompTick()
    {
        base.CompTick();
        foreach (var tankType in Props.tankTypes)
        {
            switch (tankType)
            {
                case TankType.Fuel:
                case TankType.Oil:
                    RimefellerCompat.HandleTick(this, tankType);
                    break;
                case TankType.Water:
                    BadHygieneCompat.HandleTick(this, tankType);
                    break;
                default:
                    throw new ArgumentOutOfRangeException("content", tankType, "Invalid tanker contents");
            }
        }
    }

    public override void PostExposeData()
    {
        Scribe_Collections.Look(ref storedAmount, "storedAmount", LookMode.Value, LookMode.Value);
        Scribe_Collections.Look(ref isDraining, "isDraining", LookMode.Value, LookMode.Value);
        Scribe_Collections.Look(ref isFilling, "isFilling", LookMode.Value, LookMode.Value);
        if (storedAmount.NullOrEmpty())
        {
            storedAmount = Props.tankTypes.ToDictionary(x => x, _ => 0.0);
        }

        if (isDraining.NullOrEmpty())
        {
            isDraining = Props.tankTypes.ToDictionary(x => x, _ => false);
        }

        if (isFilling.NullOrEmpty())
        {
            isFilling = Props.tankTypes.ToDictionary(x => x, _ => false);
        }
    }

    public override string CompInspectStringExtra()
    {
        if (!parent.Spawned || Props.tankTypes.NullOrEmpty())
        {
            return string.Empty;
        }

        var stringBuilder = new StringBuilder();
        stringBuilder.AppendLine(
            "TankerFrameworkTotalStorage".Translate(storedAmount.Values.Sum().ToString("0.0"), Props.storageCap));
        foreach (var tankType in Props.tankTypes)
        {
            stringBuilder.Append(CompatManager.GetTranslatedTankName(tankType));
            stringBuilder.Append(' ');
            stringBuilder.Append(storedAmount[tankType].ToString("0.0"));
            if (isFilling[tankType])
            {
                stringBuilder.Append(" (");
                stringBuilder.Append("TankerFrameworkFillingInspect".Translate());
                stringBuilder.Append(')');
            }
            else if (isDraining[tankType])
            {
                stringBuilder.Append(" (");
                stringBuilder.Append("TankerFrameworkDrainingInspect".Translate());
                stringBuilder.Append(')');
            }

            stringBuilder.AppendLine();
        }

        return stringBuilder.ToString().TrimEndNewlines();
    }

    [SyncMethod(debugOnly = true)]
    protected void DebugFill(TankType type)
    {
        var num = storedAmount.Where(x => x.Key != type).Sum(x => x.Value);
        storedAmount[type] = Props.storageCap - num;
    }

    [SyncMethod(debugOnly = true)]
    protected void DebugEmpty(TankType type)
    {
        if (type == TankType.All)
        {
            foreach (var tankType in Props.tankTypes)
            {
                storedAmount[tankType] = 0.0;
            }

            return;
        }

        storedAmount[type] = 0.0;
    }

    [SyncMethod]
    protected void ToggleDrain(TankType type)
    {
        if (type == TankType.All)
        {
            var value = !isDraining.Any(x => x.Value);
            {
                foreach (var tankType in Props.tankTypes)
                {
                    isDraining[tankType] = value;
                    isFilling[tankType] = false;
                }

                return;
            }
        }

        var dictionary = isDraining;
        dictionary[type] = !dictionary[type];
        isFilling[type] = false;
    }

    [SyncMethod]
    protected void ToggleFill(TankType type)
    {
        if (type == TankType.All)
        {
            var value = !isFilling.Any(x => x.Value);
            {
                foreach (var tankType in Props.tankTypes)
                {
                    isFilling[tankType] = value;
                    isDraining[tankType] = false;
                }

                return;
            }
        }

        var dictionary = isFilling;
        dictionary[type] = !dictionary[type];
        isDraining[type] = false;
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
}