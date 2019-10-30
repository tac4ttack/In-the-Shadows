using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using TMPro;

public class PlayerSlot : MonoBehaviour
{
    private GameObject _CurrentSlot;
    private MainMenu _MainMenuScript;
    
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
            CompletionRadial = PlayerSlotInfoPlaceholder.transform.Find("Progression_Radial_Back").transform.Find("Progression_Radial").GetComponent<Image>();
        if (!CompletionPercentageText)
            CompletionPercentageText = CompletionRadial.transform.Find("Progression_Value").GetComponent<TextMeshProUGUI>();
        if (!LastPlayedText)
            LastPlayedText = PlayerSlotInfoPlaceholder.transform.Find("Bottom").transform.Find("Bottom_left").transform.Find("LastPlayed_Text").GetComponent<TextMeshProUGUI>();
        if (!ClearSlotButton)
            ClearSlotButton = PlayerSlotInfoPlaceholder.transform.Find("Bottom").transform.Find("Bottom_right").transform.Find("ClearSlot_Button").GetComponent<Button>();
        if (!_MainMenuScript)
            _MainMenuScript = GameObject.FindGameObjectWithTag("MainMenuUI").GetComponent<MainMenu>();

        Assert.IsNotNull(_CurrentSlot, "Slot GameObject not found!");
        Assert.IsNotNull(PlayerNameText, "Player name text not found in slot \"" + _CurrentSlot.name + "\"");
        Assert.IsNotNull(CompletionRadial, "Completion radial sprite not found in slot \"" + _CurrentSlot.name + "\"");
        Assert.IsNotNull(CompletionPercentageText, "Completion percentage text not found in slot \"" + _CurrentSlot.name + "\"");
        Assert.IsNotNull(LastPlayedText, "Last played text not found in slot \"" + _CurrentSlot.name + "\"");
        Assert.IsNotNull(ClearSlotButton, "Clear slot button not found in slot \"" + _CurrentSlot.name + "\"");
        Assert.IsNotNull(EmptyTextPlaceholder, "Empty placeholder text not found in slot \"" + _CurrentSlot.name + "\"");
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
            EmptyTextPlaceholder.SetActive(true);
            PlayerSlotInfoPlaceholder.SetActive(false);
        }
        else
        {
            EmptyTextPlaceholder.SetActive(false);
            PlayerSlotInfoPlaceholder.SetActive(true);
            PlayerNameText.text = GameManager.GM.Players.PlayersName[SlotID];
            CompletionRadial.fillAmount = GameManager.GM.Players.ProgressionPercentage[SlotID] / 100f;
            CompletionPercentageText.text = Mathf.RoundToInt(GameManager.GM.Players.ProgressionPercentage[SlotID]) + "%";
            LastPlayedText.text = "Last played: " + GameManager.GM.Players.LastPlayed[SlotID];
        }
    }

    public void ClearSlotButtonPress()
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
            Debug.Log("Should launch game for player slot #" + SlotID);
            // Use the GameManager State Machine!
            // GameManager.GM.DebugMode = ToggleDebug value!!!
            GameManager.GM.CurrentPlayerSlot = SlotID;
            GameManager.GM.GameStateMachine.ChangeState(new LevelSelection_GameState());
        }
    }
}
