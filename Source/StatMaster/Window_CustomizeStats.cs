using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace StatMaster
{
    public class Window_CustomizeStats : Window
    {
        private Hediff_StatMaster hediff;

        private Vector2 scrollPos;
        private float viewHeight;
        private readonly Dictionary<StatDef, string> buffers = new Dictionary<StatDef, string>();

        private const float RowHeight = 32f;
        private const float ButtonHeight = 35f;
        private const float Gap = 6f;
        private const float RemoveButtonWidth = 30f;
        private const float AddButtonWidth = 32f;
        private const float LabelWidth = 260f;

        public Window_CustomizeStats(Hediff_StatMaster hediff)
        {
            this.hediff = hediff ?? throw new ArgumentNullException(nameof(hediff));

            doCloseX = true;
            closeOnCancel = true;
            absorbInputAroundWindow = true;

            draggable = true;
        }

        public override Vector2 InitialSize => new Vector2(650f, 520f);

        public override void DoWindowContents(Rect inRect)
        {
            Text.Font = GameFont.Medium;
            string header = $"StatMaster_StatModsFor".Translate(hediff.pawn.LabelShortCap);
            float headerHeight = Text.CalcHeight(header, inRect.width);
            Rect headerRect = new Rect(inRect.x, inRect.y, inRect.width, headerHeight);
            Widgets.Label(headerRect, header);

            Text.Font = GameFont.Small;

            float curY = headerRect.yMax + Gap;

            Rect bottomButtonsRect = new Rect(inRect.x, inRect.yMax - ButtonHeight, inRect.width, ButtonHeight);

            Rect scrollOuter = new Rect(inRect.x, curY, inRect.width, bottomButtonsRect.yMin - curY - Gap);
            Rect scrollView = new Rect(0f, 0f, scrollOuter.width - 16f, viewHeight);

            List<StatDef> statKeys = hediff.statDictionary != null
                ? hediff.statDictionary.Keys.Where(k => k != null).ToList()
                : new List<StatDef>();

            var stale = buffers.Keys.Where(k => !statKeys.Contains(k)).ToList();
            for (int i = 0; i < stale.Count; i++)
                buffers.Remove(stale[i]);

            float innerY = 0f;
            StatDef removeStat = null;

            Widgets.BeginScrollView(scrollOuter, ref scrollPos, scrollView);

            for (int i = 0; i < statKeys.Count; i++)
            {
                StatDef stat = statKeys[i];
                Rect rowRect = new Rect(0f, innerY, scrollView.width, RowHeight);

                DrawStatRow(rowRect, stat, ref removeStat);

                innerY += RowHeight + 2f;
            }

            Rect addRowRect = new Rect(0f, innerY, scrollView.width, RowHeight);
            DrawAddRow(addRowRect);
            innerY += RowHeight;

            Widgets.EndScrollView();

            viewHeight = innerY + 4f;

            if (removeStat != null && hediff.statDictionary != null)
            {
                hediff.statDictionary.Remove(removeStat);
                buffers.Remove(removeStat);
                SoundDefOf.Tick_Low.PlayOneShotOnCamera();
            }

            DrawBottomButtons(bottomButtonsRect);
        }
        private void DrawStatRow(Rect rowRect, StatDef stat, ref StatDef removeStat)
        {
            if (Mouse.IsOver(rowRect))
                Widgets.DrawHighlight(rowRect);

            Rect labelRect = new Rect(rowRect.x, rowRect.y + 4f, LabelWidth, rowRect.height - 8f);
            Rect valueRect = new Rect(labelRect.xMax + Gap, rowRect.y + 4f, 120f, rowRect.height - 8f);
            Rect removeRect = new Rect(valueRect.xMax + Gap, rowRect.y + 4f, RemoveButtonWidth, rowRect.height - 8f);

            Widgets.Label(labelRect, stat.LabelCap);

            if (hediff.statDictionary == null)
                return;

            if (!buffers.TryGetValue(stat, out string buffer) || buffer == null)
            {
                float existing = hediff.statDictionary.TryGetValue(stat, out float v) ? v : 0f;
                buffer = existing.ToString("0.###");
                buffers[stat] = buffer;
            }

            float val = hediff.statDictionary.TryGetValue(stat, out float cur) ? cur : 0f;
            string buf = buffers[stat];

            Widgets.TextFieldNumeric(valueRect, ref val, ref buf, min: -999f, max: 999f);

            buffers[stat] = buf;
            hediff.statDictionary[stat] = val;

            Color old = GUI.color;
            GUI.color = Color.red;
            if (Widgets.ButtonText(removeRect, "X"))
                removeStat = stat;
            GUI.color = old;
        }

        private void DrawAddRow(Rect rowRect)
        {
            if (Mouse.IsOver(rowRect))
                Widgets.DrawHighlight(rowRect);

            Rect plusRect = new Rect(rowRect.x, rowRect.y + 4f, AddButtonWidth, rowRect.height - 8f);
            Rect labelRect = new Rect(plusRect.xMax + Gap, rowRect.y + 4f, rowRect.width - plusRect.width - Gap, rowRect.height - 8f);

            if (Widgets.ButtonText(plusRect, "+"))
            {
                Find.WindowStack.Add(new Window_SelectStatDef(hediff));
                SoundDefOf.Tick_High.PlayOneShotOnCamera();
            }

            Widgets.Label(labelRect, "StatMaster_AddNewStatModifier".Translate());
        }

        private void DrawBottomButtons(Rect rect)
        {
            float half = (rect.width - Gap) / 2f;

            Rect closeRect = new Rect(rect.x, rect.y, half, rect.height);
            Rect removeRect = new Rect(closeRect.xMax + Gap, rect.y, half, rect.height);

            if (Widgets.ButtonText(closeRect, "StatMaster_Done".Translate()))
            {
                Close();
                SoundDefOf.Tick_High.PlayOneShotOnCamera();
            }

            Color old = GUI.color;
            GUI.color = new Color(1f, 0.6f, 0.6f);
            if (Widgets.ButtonText(removeRect, "StatMaster_RemoveFrom".Translate(hediff.pawn.LabelShortCap)))
            {
                hediff.MarkForRemoval();
                SoundDefOf.ExecuteTrade.PlayOneShotOnCamera();
                Close();
            }
            GUI.color = old;
        }


        public override void PreClose()
        {
            base.PreClose();
            hediff.ClearCachedStage();
        }
    }
}
