using UnityEngine;

public class PlayMenu : MonoBehaviour
{
    public void ClearDataButton()
    {
        GameManager.gm.ClearAllPlayersData();
    }

    public void ClearSlotButton(int iSlot)
    {
        GameManager.gm.ClearTargetPlayerData(iSlot);
    }
}
