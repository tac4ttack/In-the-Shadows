using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using TMPro;

public class LevelSelection : MonoBehaviour
{
    [Header("Camera orbiting settings")]
    [SerializeField] private float _CamAltitude = 1.5f;
    [HideInInspector] public float CamAltitude { get => _CamAltitude; }
    [SerializeField] private float _OrbitSpeed = 1f;

    [Header("Elements of Level Selection UI")]
    public Button NavLeft_BTN;
    public Button NavRight_BTN;
    public TextMeshProUGUI LevelTitle_TXT;
    public TextMeshProUGUI LevelCount_TXT;
    public TextMeshProUGUI LevelDescriptionTitle_TXT;
    public TextMeshProUGUI LevelDescriptionContent_TXT;
    public TextMeshProUGUI LevelDescriptionBestTime_TXT;
    public Button LevelDescriptionPlay_BTN;
    [Space]
    public LevelMarker[] Levels;

    private int _CurrentSelection = 0;
    private int _PreviousSelection = 0;
    
    private GameObject _Earth_GO;
    public GameObject Earth_GO { get => _Earth_GO; }

    private IEnumerator _OrbitCamCoroutine;
    private bool _IsOrbiting = false;

    private PauseMenu _PauseMenuUI;

    void Awake()
    {
        if (NavLeft_BTN == null)
            NavLeft_BTN = GameObject.FindGameObjectWithTag("LevelSelection_NavLeftButton").GetComponent<Button>();
        Assert.IsNotNull(NavLeft_BTN, "Left Navigation button not found!");

        if (NavRight_BTN == null)
            NavRight_BTN = GameObject.FindGameObjectWithTag("LevelSelection_NavRightButton").GetComponent<Button>();
        Assert.IsNotNull(NavRight_BTN, "Right Navigation button not found!");

        if (LevelTitle_TXT == null)
            LevelTitle_TXT = GameObject.FindGameObjectWithTag("LevelSelection_LevelTitleText").GetComponent<TextMeshProUGUI>();
        Assert.IsNotNull(LevelTitle_TXT, "Level Title text placeholder not found!");

        if (LevelCount_TXT == null)
            LevelCount_TXT = GameObject.FindGameObjectWithTag("LevelSelection_LevelCountText").GetComponent<TextMeshProUGUI>();
        Assert.IsNotNull(LevelCount_TXT, "Level Count text placeholder not found!");

        if (LevelDescriptionTitle_TXT == null)
            LevelDescriptionTitle_TXT = GameObject.FindGameObjectWithTag("LevelSelection_Description_TitleText").GetComponent<TextMeshProUGUI>();
        Assert.IsNotNull(LevelDescriptionTitle_TXT, "Level Description Title text placeholder not found!");
        
        if (LevelDescriptionContent_TXT == null)
            LevelDescriptionContent_TXT = GameObject.FindGameObjectWithTag("LevelSelection_Description_ContentText").GetComponent<TextMeshProUGUI>();;
        Assert.IsNotNull(LevelDescriptionContent_TXT, "Level Description Content text placeholder not found!");
        
        if (LevelDescriptionBestTime_TXT == null)
            LevelDescriptionBestTime_TXT = GameObject.FindGameObjectWithTag("LevelSelection_Description_BestTimeText").GetComponent<TextMeshProUGUI>();;
        Assert.IsNotNull(LevelDescriptionBestTime_TXT, "Level Description Best Time text placeholder not found!");
        
        if (LevelDescriptionPlay_BTN == null)
            LevelDescriptionPlay_BTN = GameObject.FindGameObjectWithTag("LevelSelection_Description_PlayButton").GetComponent<Button>();;
        Assert.IsNotNull(LevelDescriptionPlay_BTN, "Level Description Play button not found!");

        if (_Earth_GO == null)
            _Earth_GO = GameObject.FindGameObjectWithTag("LevelSelection_Earth");
        Assert.IsNotNull(_Earth_GO, "Earth GameObject is missing from the scene!");

        Levels = GameObject.FindGameObjectWithTag("LevelSelection_Map").GetComponentsInChildren<LevelMarker>();
        if (Levels.Length <= 0)
            Debug.LogError("Level list can't be empty!");

        if (_PauseMenuUI == null)
            _PauseMenuUI = GameObject.FindGameObjectWithTag("PauseMenu_UI").GetComponent<PauseMenu>();
        Assert.IsNotNull(_PauseMenuUI, "Pause Menu UI not found in scene!");
    }

    void Start()
    {
        LevelDescriptionPlay_BTN.onClick.AddListener(delegate{PlayButtonPress();});
        NavLeft_BTN.onClick.AddListener(delegate{NavButtonPress(-1);});
        NavRight_BTN.onClick.AddListener(delegate{NavButtonPress(1);});

        _OrbitCamCoroutine = OrbitCamera(Camera.main.transform.position, Levels[_CurrentSelection].Position * _CamAltitude, 1f);
        StartCoroutine(_OrbitCamCoroutine);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            switch (_PauseMenuUI.CurrentState)
            {
                case PauseMenu.PauseMenuStates.Inactive:
                    _PauseMenuUI.PauseMenuStateMachine.ChangeState((new Active_PauseMenuState(_PauseMenuUI)));
                    break;
                case PauseMenu.PauseMenuStates.Active:
                    _PauseMenuUI.PauseMenuStateMachine.ChangeState((new Inactive_PauseMenuState(_PauseMenuUI)));
                    break;
                case PauseMenu.PauseMenuStates.Settings:
                    _PauseMenuUI.PauseMenuStateMachine.GoBackToPreviousState();
                    break;
            }
        }
    }
    private void UpdateLevelSelectionUI(int iNextSelection)
    {
        Debug.Log($"new selection is {iNextSelection}");

        if (iNextSelection == 0)
        {
            NavLeft_BTN.animator.SetTrigger(NavLeft_BTN.animationTriggers.disabledTrigger);
            NavRight_BTN.animator.SetTrigger(NavRight_BTN.animationTriggers.normalTrigger);
        }
        else if (iNextSelection + 1 == Levels.Length)
        {
            NavLeft_BTN.animator.SetTrigger(NavLeft_BTN.animationTriggers.normalTrigger);
            NavRight_BTN.animator.SetTrigger(NavRight_BTN.animationTriggers.disabledTrigger);
        }
        else
        {
            NavLeft_BTN.animator.SetTrigger(NavLeft_BTN.animationTriggers.normalTrigger);
            NavRight_BTN.animator.SetTrigger(NavRight_BTN.animationTriggers.normalTrigger);
        }
        
        LevelTitle_TXT.text = Levels[_CurrentSelection].Title;
        LevelCount_TXT.text = (_CurrentSelection + 1).ToString("D2") + "/" + Levels.Length.ToString("D2");
        LevelDescriptionTitle_TXT.text = Levels[_CurrentSelection].Reference;
        LevelDescriptionContent_TXT.text = Levels[_CurrentSelection].Description;
        LevelDescriptionBestTime_TXT.text = Levels[_CurrentSelection].BestTime;
        Levels[_PreviousSelection].AnimationController.SetBool("Selected", false);
        Levels[_CurrentSelection].AnimationController.SetBool("Selected", true);
    }

    public void PlayButtonPress()
    {
        GameManager.GM.GameStateMachine.ChangeState(new InGame_GameState(_CurrentSelection + Utility.LevelSceneIndexOffset));
    }

    public void NavButtonPress(int iDirection)
    {
        int nextSelection = _CurrentSelection + iDirection;

        if (nextSelection >= 0 && nextSelection < Levels.Length)
        {
            _PreviousSelection = _CurrentSelection;
            _CurrentSelection = nextSelection;

            if (_IsOrbiting)
                StopCoroutine(_OrbitCamCoroutine);

            _OrbitCamCoroutine = OrbitCamera(Camera.main.transform.position, Levels[_CurrentSelection].Position * _CamAltitude, 1f);
            StartCoroutine(_OrbitCamCoroutine);
        }
    }

    public void OnMarkerClick(int iId)
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit))
        {
            if (!Utility.IsPointerOverUIObject())
            {
                if (_IsOrbiting)
                    StopCoroutine(_OrbitCamCoroutine);
                _PreviousSelection = _CurrentSelection;
                _CurrentSelection = iId;
                _OrbitCamCoroutine = OrbitCamera(Camera.main.transform.position, Levels[_CurrentSelection].Position * _CamAltitude, 1f);
                StartCoroutine(_OrbitCamCoroutine);
            }
        }
    }

    IEnumerator OrbitCamera(Vector3 iStart, Vector3 iTarget, float iTime)
    {
        _IsOrbiting = true;
        UpdateLevelSelectionUI(_CurrentSelection);
        for (float t = 0f; t < iTime; t += Time.deltaTime * _OrbitSpeed)
        {
            Camera.main.transform.position = Vector3.Slerp(iStart, iTarget, t / iTime);
            Camera.main.transform.LookAt(_Earth_GO.transform.position);
            yield return null;
        }
        _IsOrbiting = false;
    }

    IEnumerator UnlockCutscene(Vector3 iStart, Vector3 iTarget, int iLevel, float iTime)
    {
        _IsOrbiting = true;
        // _InCutscene = true;
        UpdateLevelSelectionUI(_CurrentSelection);
        for (float t = 0f; t < iTime; t += Time.deltaTime * _OrbitSpeed * 2f)
        {
            Camera.main.transform.position = Vector3.Slerp(iStart, iTarget, t / iTime);
            Camera.main.transform.LookAt(_Earth_GO.transform.position);
            yield return null;
        }
        // Levels[iLevel].AnimationController.SetTrigger("Unlock");
        // _InCutscene = false;
        _IsOrbiting = false;
    }

    IEnumerator CompleteCutscene(Vector3 iStart, Vector3 iTarget, int iLevel, float iTime)
    {
        _IsOrbiting = true;
        // _InCutscene = true;
        UpdateLevelSelectionUI(_CurrentSelection);
        for (float t = 0f; t < iTime; t += Time.deltaTime * _OrbitSpeed * 2f)
        {
            Camera.main.transform.position = Vector3.Slerp(iStart, iTarget, t / iTime);
            Camera.main.transform.LookAt(_Earth_GO.transform.position);
            yield return null;
        }
        // Levels[iLevel].AnimationController.SetTrigger("Complete");
        // _InCutscene = false;
        _IsOrbiting = false;
    }
}
