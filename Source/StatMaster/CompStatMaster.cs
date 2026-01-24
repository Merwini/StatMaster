using RimWorld;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using UnityEngine;

namespace StatMaster
{
    public class CompStatMaster : ThingComp
    {
        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            foreach (var gizmo in base.CompGetGizmosExtra())
            {
                yield return gizmo;
            }

            if (parent is Pawn pawn && pawn.IsColonist)
            {
                yield return new Command_Action
                {
                    defaultLabel = "StatMaster_GizmoLabel".Translate(),
                    defaultDesc = "StatMaster_GizmoDesc".Translate(pawn.LabelShortCap),
                    icon = PortraitsCache.Get(pawn, new Vector2(64, 64), Rot4.South), // I don't actually know what the Vector2 does?
                    action = () =>
                    {
                        Find.WindowStack.Add(new Window_CustomizeStats(GetOrMakeHediff()));
                    }
                };
            }
        }

        private Hediff_StatMaster GetOrMakeHediff()
        {
            Hediff_StatMaster hediff = null;
            Pawn pawn = parent as Pawn;

            foreach (Hediff h in pawn.health?.hediffSet?.hediffs)
            {
                if (h is Hediff_StatMaster hsm)
                {
                    hediff = hsm;
                    break;
                }
            }

            if (hediff == null)
            {
                hediff = (Hediff_StatMaster)HediffMaker.MakeHediff(HediffDef.Named("StatMaster_Hediff"), pawn);
                pawn.health.hediffSet.AddDirect(hediff);
            }

            return hediff;
        }
    }
}
