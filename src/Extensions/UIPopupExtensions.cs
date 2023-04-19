// ReSharper disable MemberCanBePrivate.Global
using System.Linq;

namespace ColliderEditor.Extensions
{
    static class UIPopupExtensions
    {
        public const int MAX_VISIBLE_COUNT = 400;

        public static void SelectPrevious(this UIPopup uiPopup)
        {
            if(uiPopup.currentValue == uiPopup.popupValues.First())
            {
                uiPopup.currentValue = uiPopup.LastVisibleValue();
            }
            else
            {
                uiPopup.SetPreviousValue();
            }
        }

        public static void SelectNext(this UIPopup uiPopup)
        {
            if(uiPopup.currentValue == uiPopup.LastVisibleValue())
            {
                uiPopup.currentValue = uiPopup.popupValues.First();
            }
            else
            {
                uiPopup.SetNextValue();
            }
        }

        public static string LastVisibleValue(this UIPopup uiPopup)
        {
            return uiPopup.popupValues.Length > MAX_VISIBLE_COUNT
                ? uiPopup.popupValues[MAX_VISIBLE_COUNT - 1]
                : uiPopup.popupValues.Last();
        }
    }
}
