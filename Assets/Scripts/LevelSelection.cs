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
    }

    void Start()
    {
        NavLeft_BTN.onClick.AddListener(delegate{NavButtonPress(-1);});
        NavRight_BTN.onClick.AddListener(delegate{NavButtonPress(1);});

        UpdateLevelSelectionUI();
        _OrbitCamCoroutine = OrbitCamera(Camera.main.transform.position, Levels[_CurrentSelection].Position * _CamAltitude, 1f);
        StartCoroutine(_OrbitCamCoroutine);
    }

    void FixedUpdate()
    {
        if (_CurrentSelection == 0)
        {
            NavLeft_BTN.interactable = false;
        }
        if (_CurrentSelection == Levels.Length - 1)
        {
            NavRight_BTN.interactable = false;
        }
        if (_CurrentSelection > 0 || _CurrentSelection < (Levels.Length - 1))
        {
            NavLeft_BTN.interactable = true;            
            NavRight_BTN.interactable = true;
        }
    }

    void UpdateLevelSelectionUI()
    {
        LevelTitle_TXT.text = Levels[_CurrentSelection].Title;
        LevelCount_TXT.text = (_CurrentSelection + 1).ToString("D2") + "/" +Levels.Length.ToString("D2");
        LevelDescriptionTitle_TXT.text = Levels[_CurrentSelection].Reference;
        LevelDescriptionContent_TXT.text = Levels[_CurrentSelection].Description;
        LevelDescriptionBestTime_TXT.text = Levels[_CurrentSelection].BestTime;
        Levels[_PreviousSelection].AnimationController.SetBool("Selected", false);
        Levels[_CurrentSelection].AnimationController.SetBool("Selected", true);
    }

    public void PlayButtonTest()
    {
        Debug.Log("coucoucoucoucoucouc");
        // scenemanager.launchscen(levelindex + sceneindexoffset)
    }

    public void NavButtonPress(int iDirection)
    {
        Debug.Log($"Foobar {iDirection}!");
    }

    public void NavLeftButtonPress()
    {
        if (_IsOrbiting)
            StopCoroutine(_OrbitCamCoroutine);

        if (_CurrentSelection - 1 >= 0)
        {
            _PreviousSelection = _CurrentSelection;
            _CurrentSelection--;
            UpdateLevelSelectionUI();
            _OrbitCamCoroutine = OrbitCamera(Camera.main.transform.position, Levels[_CurrentSelection].Position * _CamAltitude, 1f);
            StartCoroutine(_OrbitCamCoroutine);
        }
    }

    public void NavRightButtonPress()
    {
        if (_IsOrbiting)
            StopCoroutine(_OrbitCamCoroutine);
        
        if (_CurrentSelection + 1 <= Levels.Length)
        {
            _PreviousSelection = _CurrentSelection;
            _CurrentSelection++;
            UpdateLevelSelectionUI();
            _OrbitCamCoroutine = OrbitCamera(Camera.main.transform.position, Levels[_CurrentSelection].Position * _CamAltitude, 1f);
            StartCoroutine(_OrbitCamCoroutine);
        }
    }

    public void OnMarkerClick(int iId)
    {
        if (_IsOrbiting)
            StopCoroutine(_OrbitCamCoroutine);
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit))
        {
            if (!Utility.IsPointerOverUIObject())
            {
                _PreviousSelection = _CurrentSelection;
                _CurrentSelection = iId;
                UpdateLevelSelectionUI();
                _OrbitCamCoroutine = OrbitCamera(Camera.main.transform.position, Levels[_CurrentSelection].Position * _CamAltitude, 1f);
                StartCoroutine(_OrbitCamCoroutine);
            }
        }
    }

    IEnumerator OrbitCamera(Vector3 iStart, Vector3 iTarget, float iTime)
    {
        _IsOrbiting = true;
        for (float t = 0f; t < iTime; t += Time.deltaTime * _OrbitSpeed)
        {
            Camera.main.transform.position = Vector3.Slerp(iStart, iTarget, t / iTime);
            Camera.main.transform.LookAt(_Earth_GO.transform.position);
            yield return null;
        }
        _IsOrbiting = false;
    }
}
