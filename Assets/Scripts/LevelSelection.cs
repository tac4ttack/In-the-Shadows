using System.Linq;
using System.Collections;
using System.Collections.Generic;
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
    public CanvasGroup LevelSelectionUI_CG;
    public TextMeshProUGUI LevelTitle_TXT;
    public TextMeshProUGUI LevelCount_TXT;
    public TextMeshProUGUI LevelDescriptionTitle_TXT;
    public TextMeshProUGUI LevelDescriptionContent_TXT;
    public TextMeshProUGUI LevelDescriptionBestTime_TXT;
    public Button LevelDescriptionPlay_BTN;
    [Space]
    public LevelMarker[] Levels;

    private int _CurrentSelection = 0;
    private int _PreviousSelection = -1;

    private GameObject _Earth_GO;
    public GameObject Earth_GO { get => _Earth_GO; }

    private IEnumerator _OrbitCamCoroutine;
    private bool _IsOrbiting = false;
    private bool _InCutscene = false;
    private readonly Queue<IEnumerator> _CutsceneQueue = new Queue<IEnumerator>();
    private PauseMenu _PauseMenuUI;

    void Awake()
    {
        #if UNITY_EDITOR || DEVELOPMENT_BUILD
        // DEBUG
        Debug.Log($"LEVEL SELECTION - {this.name} - Awake()");
        #endif
        
        if (NavLeft_BTN == null)
            NavLeft_BTN = GameObject.FindGameObjectWithTag("LevelSelection_NavLeftButton").GetComponent<Button>();
        Assert.IsNotNull(NavLeft_BTN, "Left Navigation button not found!");

        if (NavRight_BTN == null)
            NavRight_BTN = GameObject.FindGameObjectWithTag("LevelSelection_NavRightButton").GetComponent<Button>();
        Assert.IsNotNull(NavRight_BTN, "Right Navigation button not found!");

        if (LevelSelectionUI_CG == null)
            LevelSelectionUI_CG = GameObject.FindGameObjectWithTag("LevelSelection_Panel").GetComponent<CanvasGroup>();
        Assert.IsNotNull(LevelSelectionUI_CG, "Level selection UI not found!");

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
            LevelDescriptionContent_TXT = GameObject.FindGameObjectWithTag("LevelSelection_Description_ContentText").GetComponent<TextMeshProUGUI>(); ;
        Assert.IsNotNull(LevelDescriptionContent_TXT, "Level Description Content text placeholder not found!");

        if (LevelDescriptionBestTime_TXT == null)
            LevelDescriptionBestTime_TXT = GameObject.FindGameObjectWithTag("LevelSelection_Description_BestTimeText").GetComponent<TextMeshProUGUI>(); ;
        Assert.IsNotNull(LevelDescriptionBestTime_TXT, "Level Description Best Time text placeholder not found!");

        if (LevelDescriptionPlay_BTN == null)
            LevelDescriptionPlay_BTN = GameObject.FindGameObjectWithTag("LevelSelection_Description_PlayButton").GetComponent<Button>(); ;
        Assert.IsNotNull(LevelDescriptionPlay_BTN, "Level Description Play button not found!");

        if (_Earth_GO == null)
            _Earth_GO = GameObject.FindGameObjectWithTag("LevelSelection_Earth");
        Assert.IsNotNull(_Earth_GO, "Earth GameObject is missing from the scene!");

        Levels = GameObject.FindGameObjectWithTag("LevelSelection_Map").GetComponentsInChildren<LevelMarker>();
        
        // DEBUG
        #if UNITY_EDITOR
        if (Levels.Length <= 0)
            Debug.LogError("Level list can't be empty!");
        #endif

        if (_PauseMenuUI == null)
            _PauseMenuUI = GameObject.FindGameObjectWithTag("PauseMenu_UI").GetComponent<PauseMenu>();
        Assert.IsNotNull(_PauseMenuUI, "Pause Menu UI not found in scene!");
        
        LevelDescriptionPlay_BTN.onClick.AddListener(delegate { PlayButtonPress(); });
        NavLeft_BTN.onClick.AddListener(delegate { NavButtonPress(-1); });
        NavRight_BTN.onClick.AddListener(delegate { NavButtonPress(1); });
    }

    void Start()
    {
        #if UNITY_EDITOR || DEVELOPMENT_BUILD
        // DEBUG
        Debug.Log($"LEVEL SELECTION - {this.name} - Start()");
        #endif

        _CurrentSelection = GameManager.GM.Players.LastPlayedLevel[Utility.CurrentPlayer];
        Camera.main.transform.position = Levels[_CurrentSelection].Position * _CamAltitude;
        Camera.main.transform.LookAt(_Earth_GO.transform.position);

        if (GameManager.GM.DebugMode)
        {
            foreach (LevelMarker level in Levels)
            {
                if (level.Status != LevelMarker.LevelStatus.Completed)
                    level.AnimationController.SetInteger("Status", 1);
            }
        }

        if (CutsceneQueued())
            LaunchCutscene();
        else
            UpdateLevelSelectionUI(_CurrentSelection);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !_InCutscene)
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

    private void UpdateLevelMarkers()
    {
        foreach (LevelMarker level in Levels)
        {
            level.Status = (LevelMarker.LevelStatus)(GameManager.GM.Players.Progression[Utility.CurrentPlayer].Level[level.Id]);
            level.AnimationController.SetInteger("Status", level.Status.GetHashCode());
        }
    }

    private bool CutsceneQueued()
    {
        if (GameManager.GM.Players.ToUnlock[Utility.CurrentPlayer].q.Count > 0 || GameManager.GM.Players.ToComplete[Utility.CurrentPlayer].q.Count > 0)
        {
            /* Cleaning lists, sorting and remove duplicates */
            GameManager.GM.Players.ToUnlock[Utility.CurrentPlayer].q.Sort();
            GameManager.GM.Players.ToUnlock[Utility.CurrentPlayer].q = GameManager.GM.Players.ToUnlock[Utility.CurrentPlayer].q.Distinct().ToList();
            GameManager.GM.Players.ToComplete[Utility.CurrentPlayer].q.Sort();
            GameManager.GM.Players.ToComplete[Utility.CurrentPlayer].q = GameManager.GM.Players.ToComplete[Utility.CurrentPlayer].q.Distinct().ToList();

            /*
                If level is in complete queue, search and remove it from the unlock queue.
                In the end, the unlock queue should only have one element max.
            */
            foreach (int id in GameManager.GM.Players.ToComplete[Utility.CurrentPlayer].q)
            {
                while (GameManager.GM.Players.ToUnlock[Utility.CurrentPlayer].q.Contains(id))
                {
                    Levels[id].UnlockStatus();
                    GameManager.GM.Players.ToUnlock[Utility.CurrentPlayer].q.Remove(id);
                }
            }

            if (GameManager.GM.Players.ToUnlock[Utility.CurrentPlayer].q.Count > 0 || GameManager.GM.Players.ToComplete[Utility.CurrentPlayer].q.Count > 0)
                return true;
            else
                return false;
        }
        else
            return false;
    }

    private void LaunchCutscene()
    {
        foreach (int id in GameManager.GM.Players.ToComplete[Utility.CurrentPlayer].q)
        {
            _CutsceneQueue.Enqueue(CompleteCutscene(Levels[id].Position * _CamAltitude, id, 0.5f));
        }
        foreach (int id in GameManager.GM.Players.ToUnlock[Utility.CurrentPlayer].q)
        {
            _CutsceneQueue.Enqueue(UnlockCutscene(Levels[id].Position * _CamAltitude, id, 0.5f));
        }
        GameManager.GM.Players.ToComplete[Utility.CurrentPlayer].q.Clear();
        GameManager.GM.Players.ToUnlock[Utility.CurrentPlayer].q.Clear();
        StartCoroutine(ExecuteCutsceneQueue());
    }

    private void UpdateLevelSelectionUI(int iSelectedLevel)
    {
        if (iSelectedLevel == 0)
        {
            NavLeft_BTN.animator.SetTrigger(NavLeft_BTN.animationTriggers.disabledTrigger);
            NavRight_BTN.animator.SetTrigger(NavRight_BTN.animationTriggers.normalTrigger);
        }
        else if (iSelectedLevel + 1 == Levels.Length)
        {
            NavLeft_BTN.animator.SetTrigger(NavLeft_BTN.animationTriggers.normalTrigger);
            NavRight_BTN.animator.SetTrigger(NavRight_BTN.animationTriggers.disabledTrigger);
        }
        else
        {
            NavLeft_BTN.animator.SetTrigger(NavLeft_BTN.animationTriggers.normalTrigger);
            NavRight_BTN.animator.SetTrigger(NavRight_BTN.animationTriggers.normalTrigger);
        }

        LevelTitle_TXT.text = Levels[iSelectedLevel].Title;
        LevelCount_TXT.text = (iSelectedLevel + 1).ToString("D2") + "/" + Levels.Length.ToString("D2");
        LevelDescriptionTitle_TXT.text = Levels[iSelectedLevel].Reference;
        LevelDescriptionContent_TXT.text = Levels[iSelectedLevel].Description;
        LevelDescriptionBestTime_TXT.text = Levels[iSelectedLevel].BestTime;

        if (Levels[iSelectedLevel].Status == LevelMarker.LevelStatus.Locked && !GameManager.GM.DebugMode)
            LevelDescriptionPlay_BTN.interactable = false;
        else
            LevelDescriptionPlay_BTN.interactable = true;

        if (_PreviousSelection >= 0)
            Levels[_PreviousSelection].AnimationController.SetBool("Selected", false);
        Levels[iSelectedLevel].AnimationController.SetBool("Selected", true);
    }

    public void PlayButtonPress()
    {
        GameManager.GM.SM.SfxSrc.PlayOneShot(GameManager.GM.SM.Sfx[1]);
        if (!_InCutscene)
            GameManager.GM.GameStateMachine.ChangeState(new InGame_GameState(_CurrentSelection + Utility.LevelSceneIndexOffset));
    }

    public void NavButtonPress(int iDirection)
    {
        GameManager.GM.SM.SfxSrc.PlayOneShot(GameManager.GM.SM.Sfx[1]);
        int nextSelection = _CurrentSelection + iDirection;

        if (!_InCutscene)
        {
            if (nextSelection >= 0 && nextSelection < Levels.Length)
            {
                _PreviousSelection = _CurrentSelection;
                _CurrentSelection = nextSelection;

                if (_IsOrbiting && !_InCutscene && _OrbitCamCoroutine != null)
                    StopCoroutine(_OrbitCamCoroutine);

                _OrbitCamCoroutine = OrbitCamera(Levels[_CurrentSelection].Position * _CamAltitude, 1f);
                StartCoroutine(_OrbitCamCoroutine);
            }
        }
    }

    public void OnMarkerClick(int iId)
    {
        GameManager.GM.SM.SfxSrc.PlayOneShot(GameManager.GM.SM.Sfx[1]);
        if (!_InCutscene)
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                if (!Utility.IsPointerOverUIObject())
                {
                    if (_IsOrbiting && !_InCutscene && _OrbitCamCoroutine != null)
                        StopCoroutine(_OrbitCamCoroutine);
                    _PreviousSelection = _CurrentSelection;
                    _CurrentSelection = iId;
                    _OrbitCamCoroutine = OrbitCamera(Levels[_CurrentSelection].Position * _CamAltitude, 1f);
                    StartCoroutine(_OrbitCamCoroutine);
                }
            }
        }
    }

    private IEnumerator OrbitCamera(Vector3 iTarget, float iTime)
    {
        Vector3 startPosition = Camera.main.transform.position;

        _IsOrbiting = true;
        UpdateLevelSelectionUI(_CurrentSelection);

        for (float t = 0f; t < iTime; t += Time.deltaTime * _OrbitSpeed)
        {
            Camera.main.transform.position = Vector3.Slerp(startPosition, iTarget, t / iTime);
            Camera.main.transform.LookAt(_Earth_GO.transform.position);
            yield return null;
        }
        GameManager.GM.SM.SfxSrc.PlayOneShot(GameManager.GM.SM.Sfx[4]);
        _IsOrbiting = false;
    }

    private IEnumerator ExecuteCutsceneQueue()
    {
        while (_CutsceneQueue.Count > 0)
        {
            yield return _CutsceneQueue.Dequeue();
        }
        UpdateLevelMarkers();
    }

    private IEnumerator UnlockCutscene(Vector3 iTarget, int iLevel, float iTime)
    {
        Vector3 startPosition = Camera.main.transform.position;

        _IsOrbiting = true;
        _InCutscene = true;
        _CurrentSelection = iLevel;
        UpdateLevelSelectionUI(iLevel);
        for (float t = 0f; t < iTime; t += Time.deltaTime * _OrbitSpeed)
        {
            Camera.main.transform.position = Vector3.Slerp(startPosition, iTarget, t / iTime);
            Camera.main.transform.LookAt(_Earth_GO.transform.position);
            yield return null;
        }
        Levels[iLevel].AnimationController.SetTrigger("ToUnlock");
        GameManager.GM.SM.SfxSrc.PlayOneShot(GameManager.GM.SM.Sfx[5]);
        yield return new WaitForSeconds(1.5f);

        if (_PreviousSelection >= 0)
            Levels[_PreviousSelection].AnimationController.SetBool("Selected", false);
        _PreviousSelection = iLevel;
        UpdateLevelSelectionUI(iLevel);
        _IsOrbiting = false;
        _InCutscene = false;
    }

    private IEnumerator CompleteCutscene(Vector3 iTarget, int iLevel, float iTime)
    {
        Vector3 startPosition = Camera.main.transform.position;

        _IsOrbiting = true;
        _InCutscene = true;
        _CurrentSelection = iLevel;
        UpdateLevelSelectionUI(iLevel);
        for (float t = 0f; t < iTime; t += Time.deltaTime * _OrbitSpeed)
        {
            Camera.main.transform.position = Vector3.Slerp(startPosition, iTarget, t / iTime);
            Camera.main.transform.LookAt(_Earth_GO.transform.position);
            yield return null;
        }
        GameManager.GM.SM.SfxSrc.PlayOneShot(GameManager.GM.SM.Sfx[3]);
        Levels[iLevel].AnimationController.SetTrigger("ToComplete");
        yield return new WaitForSeconds(1.5f);

        _PreviousSelection = iLevel;
        _IsOrbiting = false;
        _InCutscene = false;
    }
}
