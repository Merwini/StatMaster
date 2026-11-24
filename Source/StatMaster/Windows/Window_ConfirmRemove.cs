using Verse;
using UnityEngine;

namespace StatMaster;

public class Window_ConfirmRemove : Window
{
    public override Vector2 InitialSize => new Vector2(650f, 200);

    Hediff_StatMaster hediff;

    public Window_ConfirmRemove(Hediff_StatMaster hediff)
    {
        this.hediff = hediff;
    }

    public override void DoWindowContents(Rect inRect)
    {
        Text.Font = GameFont.Medium;
        Widgets.Label(new Rect(0f, 0f, inRect.width - 17f, 35f), "StatMaster_ConfirmRemove1".Translate());
        Widgets.Label(new Rect(0f, 35f, inRect.width - 17f, 35f), "StatMaster_ConfirmRemove2".Translate());
        Widgets.Label(new Rect(0f, 70f, inRect.width - 17f, 35f), "StatMaster_ConfirmRemove3".Translate());

        float buttonWidth = (inRect.width - 30) / 2;
        Rect acceptButtonRect = new Rect(inRect.x + 10, inRect.yMax - 40, buttonWidth, 30);
        Rect cancelButtonRect = new Rect(inRect.x + 20 + buttonWidth, inRect.yMax - 40, buttonWidth, 30);

        if (Widgets.ButtonText(acceptButtonRect, "StatMaster_Remove".Translate(), true, false, Color.red))
        {
            hediff.MarkForRemoval();
            Close();
        }

        if (Widgets.ButtonText(cancelButtonRect, "StatMaster_Cancel".Translate(), true, false, Color.white))
        {
            Close();
        }
    }
}
