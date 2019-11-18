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
    private bool _InCutscene = false;

    private readonly Queue<IEnumerator> _CutsceneQueue = new Queue<IEnumerator>();

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
        if (Levels.Length <= 0)
            Debug.LogError("Level list can't be empty!");

        if (_PauseMenuUI == null)
            _PauseMenuUI = GameObject.FindGameObjectWithTag("PauseMenu_UI").GetComponent<PauseMenu>();
        Assert.IsNotNull(_PauseMenuUI, "Pause Menu UI not found in scene!");
    }

    void Start()
    {
        LevelDescriptionPlay_BTN.onClick.AddListener(delegate { PlayButtonPress(); });
        NavLeft_BTN.onClick.AddListener(delegate { NavButtonPress(-1); });
        NavRight_BTN.onClick.AddListener(delegate { NavButtonPress(1); });

        if (CutsceneQueued())
            LaunchCutscene();
        else
        {
            _OrbitCamCoroutine = OrbitCamera(Levels[GameManager.GM.LastPlayedLevel].Position * _CamAltitude, 1f);
            StartCoroutine(_OrbitCamCoroutine);
        }
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

    private bool CutsceneQueued()
    {
        if (GameManager.GM.Players.ToUnlock.Length > 0 || GameManager.GM.Players.ToComplete.Length > 0)
        {
            // Cleaning lists, sorting and remove duplicates
            GameManager.GM.Players.ToUnlock[GameManager.GM.CurrentPlayerSlot].q.Sort();
            GameManager.GM.Players.ToUnlock[GameManager.GM.CurrentPlayerSlot].q = GameManager.GM.Players.ToUnlock[GameManager.GM.CurrentPlayerSlot].q.Distinct().ToList();
            GameManager.GM.Players.ToComplete[GameManager.GM.CurrentPlayerSlot].q.Sort();
            GameManager.GM.Players.ToComplete[GameManager.GM.CurrentPlayerSlot].q = GameManager.GM.Players.ToComplete[GameManager.GM.CurrentPlayerSlot].q.Distinct().ToList();

            // If level is in complete queue, search and remove it from the unlock queue.
            // In the end, the unlock queue should only have one element max.
            foreach (int id in GameManager.GM.Players.ToComplete[GameManager.GM.CurrentPlayerSlot].q)
            {
                while (GameManager.GM.Players.ToUnlock[GameManager.GM.CurrentPlayerSlot].q.Contains(id))
                    GameManager.GM.Players.ToUnlock[GameManager.GM.CurrentPlayerSlot].q.Remove(id);
            }

            if (GameManager.GM.Players.ToUnlock.Length > 0 || GameManager.GM.Players.ToComplete.Length > 0)
                return true;
            else
                return false;
        }
        else
            return false;
    }

    private void LaunchCutscene()
    {
        int previousSelection = _CurrentSelection;

        foreach (int id in GameManager.GM.Players.ToComplete[GameManager.GM.CurrentPlayerSlot].q)
        {
            _CutsceneQueue.Enqueue(CompleteCutscene(Levels[id].Position * _CamAltitude, id, 0.5f));
        }
        foreach (int id in GameManager.GM.Players.ToUnlock[GameManager.GM.CurrentPlayerSlot].q)
        {
            _CutsceneQueue.Enqueue(UnlockCutscene(Levels[id].Position * _CamAltitude, id, 0.5f));
        }

        // To Go back to the previously selected level
        _CurrentSelection = previousSelection;
        _CutsceneQueue.Enqueue(OrbitCamera(Levels[_CurrentSelection].Position * _CamAltitude, 0.5f));

        GameManager.GM.Players.ToComplete[GameManager.GM.CurrentPlayerSlot].q.Clear();
        GameManager.GM.Players.ToUnlock[GameManager.GM.CurrentPlayerSlot].q.Clear();

        StartCoroutine(ExecuteCutsceneQueue());
    }

    private void UpdateLevelSelectionUI(int iNextSelection)
    {
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
        if (!_InCutscene)
            GameManager.GM.GameStateMachine.ChangeState(new InGame_GameState(_CurrentSelection + Utility.LevelSceneIndexOffset));
    }

    public void NavButtonPress(int iDirection)
    {
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
        _IsOrbiting = false;
    }

    private IEnumerator ExecuteCutsceneQueue()
    {
        while (_CutsceneQueue.Count > 0)
        {
            yield return _CutsceneQueue.Dequeue();
        }
    }

    private IEnumerator UnlockCutscene(Vector3 iTarget, int iLevel, float iTime)
    {
        // DEBUG
        Debug.Log($"Unlock cutscene for level #{iLevel}");

        Vector3 startPosition = Camera.main.transform.position;

        _CurrentSelection = iLevel;
        _IsOrbiting = true;
        _InCutscene = true;
        UpdateLevelSelectionUI(_CurrentSelection);

        for (float t = 0f; t < iTime; t += Time.deltaTime * _OrbitSpeed)
        {
            Camera.main.transform.position = Vector3.Slerp(startPosition, iTarget, t / iTime);
            Camera.main.transform.LookAt(_Earth_GO.transform.position);
            yield return null;
        }
        Levels[iLevel].AnimationController.SetTrigger("ToUnlock");
        Levels[iLevel].AnimationController.SetInteger("Status", 1);
        GameManager.GM.Players.Progression[GameManager.GM.CurrentPlayerSlot].Level[iLevel] = 1;
        yield return new WaitForSeconds(2f);
        _IsOrbiting = false;
        _InCutscene = false;
    }

    private IEnumerator CompleteCutscene(Vector3 iTarget, int iLevel, float iTime)
    {
        // DEBUG
        Debug.Log($"Complete cutscene for level #{iLevel}");

        Vector3 startPosition = Camera.main.transform.position;

        _CurrentSelection = iLevel;
        _IsOrbiting = true;
        _InCutscene = true;
        UpdateLevelSelectionUI(_CurrentSelection);

        for (float t = 0f; t < iTime; t += Time.deltaTime * _OrbitSpeed)
        {
            Camera.main.transform.position = Vector3.Slerp(startPosition, iTarget, t / iTime);
            Camera.main.transform.LookAt(_Earth_GO.transform.position);
            yield return null;
        }
        Levels[iLevel].AnimationController.SetTrigger("ToComplete");
        Levels[iLevel].AnimationController.SetInteger("Status", 2);
        GameManager.GM.Players.Progression[GameManager.GM.CurrentPlayerSlot].Level[iLevel] = 2;
        yield return new WaitForSeconds(2f);
        _IsOrbiting = false;
        _InCutscene = false;
    }
}
