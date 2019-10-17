using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using TMPro;

public class PlayerSlot : MonoBehaviour
{
    private GameObject _CurrentSlot;
    
    public int SlotID = -1;
    public TextMeshProUGUI PlayerNameText;
    public Image CompletionRadial;
    public TextMeshProUGUI CompletionPercentageText;
    public TextMeshProUGUI LastPlayedText;
    public Button ClearSlotButton;
    
    public GameObject EmptyTextPlaceholder;
    public GameObject PlayerSlotInfoPlaceholder;

    [HideInInspector] public bool Empty;

    void Awake()
    {
        _CurrentSlot = this.gameObject;
        
        if (SlotID < 0 || SlotID > 3)
        {
            Debug.LogError("Invalid SlotID for \"" + _CurrentSlot.name + "\" player slot GameObject.\nDesactivating player slot Gameobject.");
            _CurrentSlot.SetActive(false);
        }

        if (!EmptyTextPlaceholder)
            EmptyTextPlaceholder = _CurrentSlot.transform.Find("Empty_Text").gameObject;

        if (!PlayerSlotInfoPlaceholder)
            PlayerSlotInfoPlaceholder = _CurrentSlot.transform.Find("PlayerSlotInfo").gameObject;

        if (!PlayerNameText)
            PlayerNameText = PlayerSlotInfoPlaceholder.transform.Find("PlayerName_Text").GetComponent<TextMeshProUGUI>();

        if (!CompletionRadial)
            CompletionRadial = PlayerSlotInfoPlaceholder.transform.Find("Progression_Radial").GetComponent<Image>();

        if (!CompletionPercentageText)
            CompletionPercentageText = PlayerSlotInfoPlaceholder.transform.Find("Progression_Radial").Find("Progression_Value").GetComponent<TextMeshProUGUI>();

        if (!LastPlayedText)
            LastPlayedText = PlayerSlotInfoPlaceholder.transform.Find("LastPlayed_Text").GetComponent<TextMeshProUGUI>();

        if (!ClearSlotButton)
            ClearSlotButton = PlayerSlotInfoPlaceholder.transform.Find("ClearSlot_Button").GetComponent<Button>();


        Assert.IsNotNull(_CurrentSlot, "Slot GameObject not found!");
        Assert.IsNotNull(PlayerNameText, "Player name text not found in slot \"" + _CurrentSlot.name + "\"");
        Assert.IsNotNull(CompletionRadial, "Completion radial sprite not found in slot \"" + _CurrentSlot.name + "\"");
        Assert.IsNotNull(CompletionPercentageText, "Completion percentage text not found in slot \"" + _CurrentSlot.name + "\"");
        Assert.IsNotNull(LastPlayedText, "Last played text not found in slot \"" + _CurrentSlot.name + "\"");
        Assert.IsNotNull(ClearSlotButton, "Clear slot button not found in slot \"" + _CurrentSlot.name + "\"");
        Assert.IsNotNull(EmptyTextPlaceholder, "Empty placeholder text not found in slot \"" + _CurrentSlot.name + "\"");
    }

    void Start()
    {
        Empty = GameManager.gm.Players.IsEmpty[SlotID];

        FetchSlotInfo();
    }

    void Update()
    {
        Empty = GameManager.gm.Players.IsEmpty[SlotID];
        FetchSlotInfo();
    }

    private void FetchSlotInfo()
    {
        if (Empty)
        {
            EmptyTextPlaceholder.SetActive(true);
            PlayerSlotInfoPlaceholder.SetActive(false);
        }
        else
        {
            EmptyTextPlaceholder.SetActive(false);
            PlayerSlotInfoPlaceholder.SetActive(true);
            PlayerNameText.text = GameManager.gm.Players.PlayersName[SlotID];
            CompletionRadial.fillAmount = GameManager.gm.Players.ProgressionPercentage[SlotID] / 100f;
            CompletionPercentageText.text = Mathf.RoundToInt(GameManager.gm.Players.ProgressionPercentage[SlotID]) + "%";
            LastPlayedText.text = "Last played: " + GameManager.gm.Players.LastPlayed[SlotID];
        }
    }

    public void ClearSlotButtonPress()
    {
        GameManager.gm.Players.ResetTargetPlayer(SlotID);
        SaveSystem.SavePlayers(GameManager.gm.Players);
    }
}
