// Decompiled with JetBrains decompiler
// Type: KompressionMod.Crate
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
  internal class Crate : ThingWithComps
  {
    private int StoredAmount = 0;
    private string StoredThingName;
    private static Texture2D UnLockIco;

    public override void SpawnSetup()
    {
      base.SpawnSetup();
      this.ReadFormXML();
      Crate.UnLockIco = ContentFinder<Texture2D>.Get("Things/Building/Ui/Ui_unPack", true);
    }

    private void ReadFormXML()
    {
      KompressionModThingDefs def = (KompressionModThingDefs) this.def;
      if (def == null)
        return;
      this.StoredThingName = def.StoredStuff;
      this.StoredAmount = def.StoredStuffAmmount;
    }

    public override IEnumerable<Gizmo> GetGizmos()
    {
      IList<Gizmo> source = (IList<Gizmo>) new List<Gizmo>();
      if (!string.IsNullOrEmpty(this.StoredThingName) && this.StoredAmount > 0)
      {
        IList<Gizmo> gizmoList = source;
        Command_Action commandAction1 = new Command_Action();
        commandAction1.icon = Crate.UnLockIco;
        commandAction1.defaultDesc = "Force Unpack.  Warning: may damage some merchandise.";
        commandAction1.hotKey = KeyBindingDefOf.Misc4;
        commandAction1.activateSound = SoundDef.Named("Click");
        commandAction1.action = new Action(this.ForceUnpack);
        commandAction1.groupKey = 887729001;
        Command_Action commandAction2 = commandAction1;
        gizmoList.Add((Gizmo) commandAction2);
      }
      IEnumerable<Gizmo> gizmos = base.GetGizmos();
      return gizmos == null ? source.AsEnumerable<Gizmo>() : source.AsEnumerable<Gizmo>().Concat<Gizmo>(gizmos);
    }

    private void ForceUnpack()
    {
      Thing newThing = ThingMaker.MakeThing(ThingDef.Named(this.StoredThingName), (ThingDef) null);
      newThing.stackCount = UnityEngine.Random.Range(1, 100) <= 75 ? this.StoredAmount : UnityEngine.Random.Range(this.StoredAmount / 2, this.StoredAmount);
      GenSpawn.Spawn(newThing, this.Position);
      this.Destroy(DestroyMode.Vanish);
    }
  }
}
