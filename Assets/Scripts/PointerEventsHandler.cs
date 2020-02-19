using UnityEngine;
using UnityEngine.EventSystems;

public class PointerEventsHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        GameManager.GM.SM.SfxSrc.PlayOneShot(GameManager.GM.SM.Sfx[8]);
    }

    public void OnPointerExit(PointerEventData pointerEventData) {}
}
