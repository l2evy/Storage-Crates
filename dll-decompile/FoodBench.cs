// Decompiled with JetBrains decompiler
// Type: KompressionMod.FoodBench
// Assembly: CompressionMod, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 6BEC81DF-9DDE-492D-9D6D-A1D159C3C5C3
// Assembly location: X:\-Shade\RimWorld1249Win\Mods\Storage Crates\Assemblies\CompressionMod.dll

using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace KompressionMod
{
  internal class FoodBench : Building_WorkTable
  {
    private bool Change = false;
    private int timer = 0;
    public bool stateOpen = true;
    private CompPowerTrader powerComp;
    private static Texture2D Ui_Pmode1;
    private static Texture2D Ui_Pmode2;
    private Graphic TexMain;
    private static Graphic TexOpen;
    private static Graphic TexClosed;
    private static Graphic[] TexResFrames;
    private Graphic OutputGraphic;
    private Faction factionthing;

    public override Graphic Graphic
    {
      get
      {
        return this.OutputGraphic == null ? base.Graphic : this.OutputGraphic;
      }
    }

    public override void SpawnSetup()
    {
      base.SpawnSetup();
      this.powerComp = this.GetComp<CompPowerTrader>();
      FoodBench.Ui_Pmode1 = ContentFinder<Texture2D>.Get("Things/Building/Ui/Ui_Pack", true);
      FoodBench.Ui_Pmode2 = ContentFinder<Texture2D>.Get("Things/Building/Ui/Ui_unPack", true);
      FoodBench.TexOpen = GraphicDatabase.Get<Graphic_Single>("Things/Building/Frames/FoodBench_Packing");
      FoodBench.TexClosed = GraphicDatabase.Get<Graphic_Single>("Things/Building/Frames/FoodBench_UnPacking");
      FoodBench.TexResFrames = (Graphic[]) new Graphic_Single[12];
      for (int index = 0; index < 12; ++index)
      {
        FoodBench.TexResFrames[index] = GraphicDatabase.Get<Graphic_Single>("Things/Building/Frames/FoodBench_Frame" + (object) (index + 1));
        FoodBench.TexResFrames[index].drawSize = this.Graphic.drawSize;
        FoodBench.TexResFrames[index].color = this.Graphic.color;
        FoodBench.TexResFrames[index].colorTwo = this.Graphic.colorTwo;
        FoodBench.TexResFrames[index].MatSingle.color = this.Graphic.MatSingle.color;
      }
      this.factionthing = this.factionInt;
    }

    public override void ExposeData()
    {
      base.ExposeData();
      Scribe_Values.LookValue<bool>(ref this.Change, "ChangeActivator", false, false);
      Scribe_Values.LookValue<int>(ref this.timer, "Timer", 0, false);
      Scribe_Values.LookValue<bool>(ref this.stateOpen, "BenchState", false, false);
    }

    public override IEnumerable<Gizmo> GetGizmos()
    {
      IList<Gizmo> source = (IList<Gizmo>) new List<Gizmo>();
      Command_Action commandAction = new Command_Action();
      if (!this.Change && this.powerComp.PowerOn)
      {
        if (this.stateOpen)
        {
          commandAction.icon = FoodBench.Ui_Pmode2;
          commandAction.defaultDesc = "Change to Un-Packing Mode";
        }
        else
        {
          commandAction.icon = FoodBench.Ui_Pmode1;
          commandAction.defaultDesc = "Change to Packing Mode";
        }
        commandAction.activateSound = SoundDef.Named("Click");
        commandAction.action = new Action(this.Open);
        commandAction.groupKey = 78142894;
        commandAction.hotKey = KeyBindingDefOf.Misc1;
        source.Add((Gizmo) commandAction);
      }
      IEnumerable<Gizmo> gizmos = base.GetGizmos();
      return gizmos == null ? source.AsEnumerable<Gizmo>() : source.AsEnumerable<Gizmo>().Concat<Gizmo>(gizmos);
    }

    private void Open()
    {
      this.Change = !this.Change;
    }

    public override void Tick()
    {
      base.Tick();
      if (!this.Change || !this.powerComp.PowerOn)
        return;
      if (this.stateOpen)
        this.handleAnimation(false);
      else
        this.handleAnimation(true);
      ++this.timer;
      Find.MapDrawer.MapMeshDirty(this.Position, MapMeshFlag.Things, false, false);
      if (this.timer >= 60)
      {
        Thing newThing;
        if (this.stateOpen)
        {
          this.TexMain = FoodBench.TexClosed;
          newThing = ThingMaker.MakeThing(ThingDef.Named("FoodUnPackingBench"), this.Stuff);
        }
        else
        {
          this.TexMain = FoodBench.TexOpen;
          newThing = ThingMaker.MakeThing(ThingDef.Named("FoodPackingBench"), this.Stuff);
        }
        newThing.HitPoints = this.HitPoints;
        ((FoodBench) newThing).stateOpen = !this.stateOpen;
        newThing.SetFactionDirect(this.Faction);
        this.Destroy(DestroyMode.Vanish);
        GenSpawn.Spawn(newThing, this.Position, this.Rotation);
        this.timer = 0;
        this.Change = false;
      }
    }

    private void handleAnimation(bool open)
    {
      if (this.timer >= 60)
        return;
      int index = this.timer / 5;
      if (!open)
        index = 11 - index;
      this.TexMain = FoodBench.TexResFrames[index];
      this.UpdateOutputGraphic();
    }

    private void UpdateOutputGraphic()
    {
      this.OutputGraphic = this.TexMain.GetColoredVersion(this.def.graphicData.Graphic.Shader, this.Stuff.stuffProps.color, this.Stuff.stuffProps.color);
    }
  }
}
