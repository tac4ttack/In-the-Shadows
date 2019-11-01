using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using TMPro;

public class PlayerSlot : MonoBehaviour
{
    private GameObject _CurrentSlot;
    private MainMenu _MainMenuScript;

    public int SlotID = -1;
    public TextMeshProUGUI PlayerName_TXT;
    public Image CompletionRadial_IMG;
    public TextMeshProUGUI CompletionPercentage_TXT;
    public TextMeshProUGUI LastPlayed_TXT;
    public Button ClearSlot_BTN;

    public GameObject EmptyText_GO;
    public GameObject PlayerSlotInfo_GO;

    [HideInInspector] public bool Empty;

    void Awake()
    {
        _CurrentSlot = this.gameObject;
        Assert.IsNotNull(_CurrentSlot, "Slot GameObject not found!");

        if (SlotID < 0 || SlotID > 3)
        {
            Debug.LogError("Invalid SlotID for \"" + _CurrentSlot.name + "\" player slot GameObject.\nDesactivating player slot Gameobject.");
            _CurrentSlot.SetActive(false);
        }

        if (!EmptyText_GO)
            EmptyText_GO = _CurrentSlot.transform.Find("Empty_Text").gameObject;
        Assert.IsNotNull(EmptyText_GO, "Empty placeholder text not found in slot \"" + _CurrentSlot.name + "\"");

        if (!PlayerSlotInfo_GO)
            PlayerSlotInfo_GO = _CurrentSlot.transform.Find("PlayerSlotInfo").gameObject;

        if (!PlayerName_TXT)
            PlayerName_TXT = PlayerSlotInfo_GO.transform.Find("PlayerName_Text").GetComponent<TextMeshProUGUI>();
        Assert.IsNotNull(PlayerName_TXT, "Player name text not found in slot \"" + _CurrentSlot.name + "\"");

        if (!CompletionRadial_IMG)
            CompletionRadial_IMG = PlayerSlotInfo_GO.transform.Find("Progression_Radial_Back").Find("Progression_Radial").GetComponent<Image>();
        Assert.IsNotNull(CompletionRadial_IMG, "Completion radial sprite not found in slot \"" + _CurrentSlot.name + "\"");

        if (!CompletionPercentage_TXT)
            CompletionPercentage_TXT = CompletionRadial_IMG.transform.Find("Progression_Value").GetComponent<TextMeshProUGUI>();
        Assert.IsNotNull(CompletionPercentage_TXT, "Completion percentage text not found in slot \"" + _CurrentSlot.name + "\"");

        if (!LastPlayed_TXT)
            LastPlayed_TXT = PlayerSlotInfo_GO.transform.Find("Bottom").Find("Bottom_left").Find("LastPlayed_Text").GetComponent<TextMeshProUGUI>();
        Assert.IsNotNull(LastPlayed_TXT, "Last played text not found in slot \"" + _CurrentSlot.name + "\"");

        if (!ClearSlot_BTN)
            ClearSlot_BTN = PlayerSlotInfo_GO.transform.Find("Bottom").Find("Bottom_right").Find("ClearSlot_Button").GetComponent<Button>();
        Assert.IsNotNull(ClearSlot_BTN, "Clear slot button not found in slot \"" + _CurrentSlot.name + "\"");

        if (!_MainMenuScript)
            _MainMenuScript = GameObject.FindGameObjectWithTag("MainMenu_UI").GetComponent<MainMenu>();
        Assert.IsNotNull(_MainMenuScript, "Main Menu Script instance not found!");
    }

    void Start()
    {
        Empty = GameManager.GM.Players.IsEmpty[SlotID];
        FetchSlotInfo();
    }

    void Update()
    {
        Empty = GameManager.GM.Players.IsEmpty[SlotID];
        FetchSlotInfo();
    }

    private void FetchSlotInfo()
    {
        if (Empty)
        {
            EmptyText_GO.SetActive(true);
            PlayerSlotInfo_GO.SetActive(false);
        }
        else
        {
            EmptyText_GO.SetActive(false);
            PlayerSlotInfo_GO.SetActive(true);
            PlayerName_TXT.text = GameManager.GM.Players.PlayersName[SlotID];
            CompletionRadial_IMG.fillAmount = GameManager.GM.Players.ProgressionPercentage[SlotID] / 100f;
            CompletionPercentage_TXT.text = Mathf.RoundToInt(GameManager.GM.Players.ProgressionPercentage[SlotID]) + "%";
            LastPlayed_TXT.text = "Last played: " + GameManager.GM.Players.LastPlayed[SlotID];
        }
    }

    public void ClearSlot_BTNPress()
    {
        GameManager.GM.Players.ResetTargetPlayer(SlotID);
        SaveSystem.SavePlayers(GameManager.GM.Players);
    }

    public void SlotPress()
    {
        if (Empty)
        {
            _MainMenuScript.MainMenuStateMachine.ChangeState(new NewPlayerPrompt_MainMenuState(_MainMenuScript, SlotID));
        }
        else
        {
            GameManager.GM.CurrentPlayerSlot = SlotID;
            GameManager.GM.GameStateMachine.ChangeState(new LevelSelection_GameState());
        }
    }
}
