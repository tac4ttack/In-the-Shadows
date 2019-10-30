using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public static class Utility
{
    public const float TransitionSpeed = 0.1f;

    public static bool IsPointerOverUIObject()
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        List<RaycastResult> raycastResults = new List<RaycastResult>();

        eventData.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        EventSystem.current.RaycastAll(eventData, raycastResults);
        
        return (raycastResults.Count > 0);
    }

    public static IEnumerator PopInCanvasGroup(CanvasGroup iCanvasGroup, float iTime, float iSpeed, float iTargetAlpha = 1f)
    {
        iCanvasGroup.interactable = true;
        iCanvasGroup.blocksRaycasts = true;
        for (float t = 0f; t < iTime; t += iSpeed)
        {
            iCanvasGroup.alpha += 0.1f;
            yield return null;
        }
    }

    public static IEnumerator PopOutCanvasGroup(CanvasGroup iCanvasGroup, float iTime, float iSpeed, float iTargetAlpha = 0f)
    {
        iCanvasGroup.interactable = false;
        iCanvasGroup.blocksRaycasts = false;
        for (float t = 0f; t < iTime; t += iSpeed)
        {
            iCanvasGroup.alpha -= 0.1f;
            yield return null;
        }
    }
}
