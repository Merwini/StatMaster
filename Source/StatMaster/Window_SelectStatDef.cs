using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using RimWorld;
using UnityEngine;

namespace StatMaster
{
    public class Window_SelectStatDef : Window
    {
        string searchTerm = "";
        Vector2 leftScrollPosition = new Vector2();
        StatDef selectedDef = null;
        Hediff_StatMaster hediff;

        static readonly HashSet<StatCategoryDef> PawnStatCategories = new HashSet<StatCategoryDef>
        {
            StatCategoryDefOf.BasicsPawn,
            StatCategoryDefOf.PawnCombat,
            StatCategoryDefOf.PawnWork,
            StatCategoryDefOf.PawnSocial,
            StatCategoryDefOf.PawnResistances,
            StatCategoryDefOf.PawnHealth,
            StatCategoryDefOf.PawnFood
        };

        public Window_SelectStatDef(Hediff_StatMaster hediff)
        {
            this.hediff = hediff;

            doCloseX = true;
            closeOnCancel = true;
            absorbInputAroundWindow = true;
        }

        public override Vector2 InitialSize => new Vector2(650f, 520f);

        public override void DoWindowContents(Rect inRect)
        {
            Listing_Standard list = new Listing_Standard();

            list.Begin(inRect);
            Text.Font = GameFont.Medium;
            Widgets.Label(new Rect(0f, 0f, inRect.width - 17f, 35f), "StatMaster_SelectAStat".Translate());
            Text.Font = GameFont.Small;
            list.End();
            list.Gap(45);

            float searchBoxHeight = 25f;
            Rect searchBoxRect = new Rect(inRect.x + 10, inRect.y + 45, inRect.width - 20, searchBoxHeight);
            searchTerm = Widgets.TextField(searchBoxRect, searchTerm);

            float listTop = searchBoxRect.yMax + 10;
            float listBottomPadding = 50f;
            Rect listArea = new Rect(inRect.x + 10, listTop, inRect.width - 20, inRect.height - listTop - listBottomPadding);
            GUI.BeginGroup(listArea, new GUIStyle(GUI.skin.box));

            List<StatDef> tempList = DefDatabase<StatDef>.AllDefsListForReading
                    .Where(item => item.label.ToLower().Contains(searchTerm.ToLower()) && 
                        !hediff.statDictionary.ContainsKey(item) &&
                        (PawnStatCategories.Contains(item.category)))
                    .OrderBy(def => def.label)
                    .ToList();

            float num = 3f;
            Rect viewRect = new Rect(0f, 0f, listArea.width - 16f, tempList.Count * 32f);
            Widgets.BeginScrollView(listArea.AtZero(), ref leftScrollPosition, viewRect);

            if (!tempList.NullOrEmpty())
            {
                foreach (StatDef def in tempList)
                {
                    Rect rowRect = new Rect(x: 5, y: num, width: listArea.width - 6, height: 30);
                    Widgets.DrawHighlightIfMouseover(rowRect);
                    if (def == selectedDef)
                    {
                        Widgets.DrawHighlightSelected(rowRect);
                    }

                    Widgets.Label(rowRect, $"{def.label} - ({def.defName})");
                    if (Widgets.ButtonInvisible(rowRect))
                    {
                        selectedDef = def;
                    }

                    num += 32f;
                }
            }

            Widgets.EndScrollView();
            GUI.EndGroup();

            float buttonWidth = (inRect.width - 30) / 2;
            Rect acceptButtonRect = new Rect(inRect.x + 10, inRect.yMax - 40, buttonWidth, 30);
            Rect cancelButtonRect = new Rect(inRect.x + 20 + buttonWidth, inRect.yMax - 40, buttonWidth, 30);

            if (Widgets.ButtonText(acceptButtonRect, "StatMaster_Accept".Translate(), true, false, Color.green) && selectedDef != null)
            {
                hediff.statDictionary[selectedDef] = 0;
                Close();
            }

            if (Widgets.ButtonText(cancelButtonRect, "StatMaster_Cancel".Translate(), true, false, Color.red))
            {
                Close();
            }
        }
    }
}
