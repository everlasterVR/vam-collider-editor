using UnityEngine;
using UnityEngine.UI;

public static partial class MVRScriptExtensions
{
    public static UIDynamicPopup CreatePopupAuto(
        this MVRScript script,
        JSONStorableStringChooser jssc,
        bool rightSide = false,
        float popupPanelHeight = 0f,
        bool upwards = false
    )
    {
        var popup = script.CreateFilterablePopup(jssc, rightSide);
        var uiPopup = popup.popup;

        uiPopup.labelText.alignment = TextAnchor.UpperCenter;
        uiPopup.labelText.GetComponent<RectTransform>().anchorMax = new Vector2(0, 0.89f);

        {
            var btn = Object.Instantiate(script.manager.configurableButtonPrefab, popup.transform, false);
            Object.Destroy(btn.GetComponent<LayoutElement>());
            btn.GetComponent<UIDynamicButton>().label = "<";
            btn.GetComponent<UIDynamicButton>()
                .button.onClick.AddListener(
                    () =>
                    {
                        uiPopup.visible = false;
                        uiPopup.SelectPrevious();
                    }
                );
            var prevBtnRect = btn.GetComponent<RectTransform>();
            prevBtnRect.pivot = new Vector2(0, 0);
            prevBtnRect.anchoredPosition = new Vector2(10f, 0);
            prevBtnRect.sizeDelta = new Vector2(0f, 0f);
            prevBtnRect.offsetMin = new Vector2(5f, 5f);
            prevBtnRect.offsetMax = new Vector2(80f, 70f);
            prevBtnRect.anchorMin = new Vector2(0f, 0f);
            prevBtnRect.anchorMax = new Vector2(0f, 0f);
        }

        {
            var btn = Object.Instantiate(script.manager.configurableButtonPrefab, popup.transform, false);
            Object.Destroy(btn.GetComponent<LayoutElement>());
            btn.GetComponent<UIDynamicButton>().label = ">";
            btn.GetComponent<UIDynamicButton>()
                .button.onClick.AddListener(
                    () =>
                    {
                        uiPopup.visible = false;
                        uiPopup.SelectNext();
                    }
                );
            var prevBtnRect = btn.GetComponent<RectTransform>();
            prevBtnRect.pivot = new Vector2(0, 0);
            prevBtnRect.anchoredPosition = new Vector2(10f, 0);
            prevBtnRect.sizeDelta = new Vector2(0f, 0f);
            prevBtnRect.offsetMin = new Vector2(82f, 5f);
            prevBtnRect.offsetMax = new Vector2(157f, 70f);
            prevBtnRect.anchorMin = new Vector2(0f, 0f);
            prevBtnRect.anchorMax = new Vector2(0f, 0f);
        }

        if(popupPanelHeight > 0f)
        {
            popup.popupPanelHeight = popupPanelHeight;
        }

        if(upwards)
        {
            uiPopup.popupPanel.offsetMin += new Vector2(0, popup.popupPanelHeight + 60);
            uiPopup.popupPanel.offsetMax += new Vector2(0, popup.popupPanelHeight + 60);
        }

        return popup;
    }
}
