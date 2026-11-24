using Verse;
using UnityEngine;

namespace StatMaster;

public class Window_ConfirmOverwrite : Window
{
    public override Vector2 InitialSize => new Vector2(650f, 150);

    Hediff_StatMaster hediff;
    Window_CustomizeStats window;

    public Window_ConfirmOverwrite(Hediff_StatMaster hediff, Window_CustomizeStats window)
    {
        this.hediff = hediff;
        this.window = window;
    }

    public override void DoWindowContents(Rect inRect)
    {
        Text.Font = GameFont.Medium;
        Widgets.Label(new Rect(0f, 0f, inRect.width - 17f, 35f), "StatMaster_ConfirmOverwrite".Translate());
        Widgets.Label(new Rect(0f, 35f, inRect.width - 17f, 35f), "StatMaster_ConfirmRemove2".Translate());

        float buttonWidth = (inRect.width - 30) / 2;
        Rect acceptButtonRect = new Rect(inRect.x + 10, inRect.yMax - 40, buttonWidth, 30);
        Rect cancelButtonRect = new Rect(inRect.x + 20 + buttonWidth, inRect.yMax - 40, buttonWidth, 30);

        if (Widgets.ButtonText(acceptButtonRect, "StatMaster_Overwrite".Translate(), true, false, Color.red))
        {
            hediff.PasteFromClipboard();
            window.ClearBuffers();
            Close();
        }

        if (Widgets.ButtonText(cancelButtonRect, "StatMaster_Cancel".Translate(), true, false, Color.white))
        {
            Close();
        }
    }
}
