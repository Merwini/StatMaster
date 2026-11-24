using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using RimWorld;

namespace StatMaster;

public class Hediff_StatMaster : Hediff
{
    internal Dictionary<StatDef, float> statDictionary = new();

    internal static Dictionary<StatDef, float> clipboardDictionary = new();

    private HediffStage curStage;

    bool markedToRemove = false;

    public override bool ShouldRemove => markedToRemove;

    public override HediffStage CurStage
    {
        get
        {
            if (curStage == null)
            {
                List<StatModifier> modifiers = BuildStatModifiers();

                curStage = new HediffStage();
                curStage.statOffsets = modifiers;
            }
            return curStage;
        }
    }

    public Hediff_StatMaster()
    {
    }

    private List<StatModifier> BuildStatModifiers()
    {
        List<StatModifier> newMods = new List<StatModifier>();

        foreach (var kvp in statDictionary)
        {
            if (kvp.Value != 0)
            {
                StatModifier sm = new StatModifier();
                sm.stat = kvp.Key;
                sm.value = kvp.Value;
                newMods.Add(sm);
            }
        }

        return newMods;
    }

    public void ClearCachedStage()
    {
        curStage = null;
    }

    public void MarkForRemoval()
    {
        markedToRemove = true;
    }

    public bool CopyToClipboard()
    {
        if (statDictionary.NullOrEmpty())
            return false;

        clipboardDictionary = statDictionary.ToDictionary(
            entry => entry.Key,
            entry => entry.Value
        );

        return true;
    }

    public bool PasteFromClipboard()
    {
        if (clipboardDictionary.NullOrEmpty())
            return false;

        statDictionary = clipboardDictionary.ToDictionary(
        entry => entry.Key,
        entry => entry.Value
        );

        ClearCachedStage();

        return true;
    }

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Collections.Look(ref statDictionary, "statDictionary");
    }
}
