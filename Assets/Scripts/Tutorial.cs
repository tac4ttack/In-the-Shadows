using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using TMPro;

public class Tutorial : MonoBehaviour
{
    public StateMachine TutorialStateMachine = new StateMachine();
    public IState[] TutorialSteps;
    public int CurrentStep = 0;
    public PuzzlePiece[] PuzzlePieces;
    public PuzzlePiece DummyPuzzlePiece;
    public TutorialCard[] Cards;
    public CanvasGroup Tutorial_CG;
    public TextMeshProUGUI Tutorial_Title_TXT;
    public TextMeshProUGUI Tutorial_Content_TXT;
    public Image Tutorial_Pic_IMG;
    public Button Tutorial_Prev_BTN;
    public Button Tutorial_Next_BTN;
    public Material GuideWall_MAT;

    [ColorUsageAttribute(true, true)] public Color GuideAlbedoHue = new Color(0.34f, 0.69f, 0.7f, 1f);
    [ColorUsageAttribute(true, true)] public Color GuideEmissionHue = new Color(0.45f, 1.96f, 2f, 1f);
    [ColorUsageAttribute(true, true)] public Color GuideEmissionValidHue = new Color(0.053f, 1f, 0f, 1f);

    void Awake()
    {
        if (Tutorial_CG == null)
            Tutorial_CG = GameObject.FindGameObjectWithTag("Tutorial_Panel").GetComponent<CanvasGroup>();
        Assert.IsNotNull(Tutorial_CG, "Tutorial panel canvas group not found in scene!");

        if (Tutorial_Title_TXT == null)
            Tutorial_Title_TXT = GameObject.FindGameObjectWithTag("Tutorial_Title").GetComponent<TextMeshProUGUI>();
        Assert.IsNotNull(Tutorial_Title_TXT, "Tutorial panel title text not found in scene!");

        if (Tutorial_Content_TXT == null)
            Tutorial_Content_TXT = GameObject.FindGameObjectWithTag("Tutorial_Content").GetComponent<TextMeshProUGUI>();
        Assert.IsNotNull(Tutorial_Content_TXT, "Tutorial panel content text not found in scene!");

        if (Tutorial_Pic_IMG == null)
            Tutorial_Pic_IMG = GameObject.FindGameObjectWithTag("Tutorial_Picture").GetComponent<Image>();
        Assert.IsNotNull(Tutorial_Pic_IMG, "Tutorial panel image not found in scene!");

        if (Tutorial_Prev_BTN == null)
            Tutorial_Prev_BTN = GameObject.FindGameObjectWithTag("Tutorial_Prev").GetComponent<Button>();
        Assert.IsNotNull(Tutorial_Prev_BTN, "Tutorial panel previous button not found in scene!");
        Tutorial_Prev_BTN.interactable = false;
        
        if (Tutorial_Next_BTN == null)
            Tutorial_Next_BTN = GameObject.FindGameObjectWithTag("Tutorial_Next").GetComponent<Button>();
        Assert.IsNotNull(Tutorial_Next_BTN, "Tutorial panel next button not found in scene!");
        Tutorial_Next_BTN.interactable = false;

        if (GuideWall_MAT == null)
            GuideWall_MAT = GameObject.FindGameObjectWithTag("Tutorial_GuideWall").GetComponent<MeshRenderer>().materials[1];

        TutorialSteps = new IState[5] { new Tutorial_Step_0(this),
                                        new Tutorial_Step_1(this),
                                        new Tutorial_Step_2(this),
                                        new Tutorial_Step_3(this),
                                        new Tutorial_Step_4(this)};
    }

    void Start()
    {
        /*
            Tutorial puzzle pieces fetching
            DUMMY piece MUST BE in the last place of the Puzzle script's puzzle pieces array
        */
        GameObject[] tmp = GameObject.FindGameObjectsWithTag("InGame_PuzzlePiece");
        PuzzlePieces = new PuzzlePiece[tmp.Length - 1];
        for (int i = 0;i < tmp.Length;i++)
        {
                if (i == 2)
                    DummyPuzzlePiece = tmp[i].GetComponent<PuzzlePiece>();
                else
                    PuzzlePieces[i] = tmp[i].GetComponent<PuzzlePiece>();
        }
        Assert.IsNotNull(PuzzlePieces, "No Puzzle pieces found in scene!");

        TutorialStateMachine.ChangeState(TutorialSteps[CurrentStep]);
        Tutorial_Prev_BTN.onClick.AddListener(delegate { TutorialPrevButtonPress();});
        Tutorial_Next_BTN.onClick.AddListener(delegate { TutorialNextButtonPress();});
    }

    void Update()
    {   
        TutorialStateMachine.ExecuteState();
    }

    public void FillInCard(TutorialCard iCard)
    {
        if (iCard)
        {
            Tutorial_Title_TXT.text = iCard.Title;
            Tutorial_Content_TXT.text = iCard.Content;
            if (iCard.Picture)
            {
                Tutorial_Pic_IMG.enabled = true;    
                Tutorial_Pic_IMG.sprite = iCard.Picture;
            }
            else
            {
                Tutorial_Pic_IMG.enabled = false;    
            }
        }
    }

    #region Buttons Navigation

    public void TutorialPrevButtonPress()
    {
        GameManager.GM.SM.SfxSrc.PlayOneShot(GameManager.GM.SM.Sfx[1]);
        switch (CurrentStep)
        {
            case 1:
                TutorialStateMachine.ChangeState(TutorialSteps[0]);
                break;

            case 2:
                TutorialStateMachine.ChangeState(TutorialSteps[1]);
                break;

            case 3:
                TutorialStateMachine.ChangeState(TutorialSteps[2]);
                break;
            
            case 4:
                TutorialStateMachine.ChangeState(TutorialSteps[3]);
                break;

            default:
                break;
        }
    }

    public void TutorialNextButtonPress()
    {
        GameManager.GM.SM.SfxSrc.PlayOneShot(GameManager.GM.SM.Sfx[1]);
        switch (CurrentStep)
        {
            case 0:
                TutorialStateMachine.ChangeState(TutorialSteps[1]);
                break;

            case 1:
                TutorialStateMachine.ChangeState(TutorialSteps[2]);
                break;

            case 2:
                TutorialStateMachine.ChangeState(TutorialSteps[3]);
                break;

            case 3:
                TutorialStateMachine.ChangeState(TutorialSteps[4]);
                break;
            
            case 4:
                DummyPuzzlePiece.transform.localRotation = new Quaternion(0.5f, 0.5f, 0.5f, 0.5f);
                GameManager.GM.StartCoroutine(Utility.PopOutCanvasGroup(Tutorial_CG, 1f, Utility.TransitionSpeed));
                break;

            default:
                break;
        }
    }

    #endregion
}

#region Tutorial Steps
public class Tutorial_Step_0 : IState
{
    private Tutorial _TutorialScript;

    public Tutorial_Step_0(Tutorial iTutorialScript) => _TutorialScript = iTutorialScript;

    public void Enter()
    {
        _TutorialScript.CurrentStep = 0;
        _TutorialScript.FillInCard(_TutorialScript.Cards[_TutorialScript.CurrentStep]);

        _TutorialScript.Tutorial_Prev_BTN.gameObject.SetActive(false);
        _TutorialScript.Tutorial_Prev_BTN.interactable = false;
        _TutorialScript.Tutorial_Next_BTN.gameObject.SetActive(true);
        _TutorialScript.Tutorial_Next_BTN.interactable = true;

        _TutorialScript.PuzzlePieces[0].gameObject.SetActive(false);
        _TutorialScript.PuzzlePieces[1].gameObject.SetActive(false);

        _TutorialScript.GuideWall_MAT.SetColor(Shader.PropertyToID("_Color"), new Color(1, 1, 1, 0));
    }

    public void Execute() {}

    public void Exit() {}
}

public class Tutorial_Step_1 : IState
{
    private Tutorial _TutorialScript;
    private bool _Unlocked = false;

    public Tutorial_Step_1(Tutorial iTutorialScript) => _TutorialScript = iTutorialScript;

    public void Enter()
    {      
        _TutorialScript.CurrentStep = 1;
        _TutorialScript.FillInCard(_TutorialScript.Cards[_TutorialScript.CurrentStep]);
        
        _TutorialScript.Tutorial_Prev_BTN.interactable = true;
        _TutorialScript.Tutorial_Prev_BTN.gameObject.SetActive(true);
        _TutorialScript.Tutorial_Next_BTN.gameObject.SetActive(true);
        _TutorialScript.Tutorial_Next_BTN.interactable = false;

        _TutorialScript.PuzzlePieces[0].transform.localPosition = new Vector3(8.62f, -1.599f, -1.915f);
        _TutorialScript.PuzzlePieces[0].transform.localRotation = Quaternion.Euler(98f, 16f, 0f);
        _TutorialScript.PuzzlePieces[0].SetRotationConstraint(new bool[] {false, true, true});
        _TutorialScript.PuzzlePieces[0].SetTranslationConstraint(new bool[] {true, true, true});
        _TutorialScript.PuzzlePieces[0].SetDirectionSolution(new Vector3(2.2f, 0.2f, 0f));
        _TutorialScript.PuzzlePieces[0].SetRotationSolutions(new Quaternion[]{new Quaternion(0f, 0.1f, 0f, 1f)});
        _TutorialScript.PuzzlePieces[0].SetDistanceSolution(2.25f);
        _TutorialScript.PuzzlePieces[0].gameObject.SetActive(true);
        
        _TutorialScript.PuzzlePieces[1].gameObject.SetActive(false);
        _TutorialScript.PuzzlePieces[1].transform.localPosition = new Vector3(1.99f, -0.99f, -0.55f);
        _TutorialScript.PuzzlePieces[1].transform.localRotation = Quaternion.Euler(0f, 24f, 0f);
        _TutorialScript.PuzzlePieces[1].SetRotationConstraint(new bool[] {true, true, true});
        _TutorialScript.PuzzlePieces[1].SetTranslationConstraint(new bool[] {true, true, true});
        _TutorialScript.PuzzlePieces[1].SetDirectionSolution(new Vector3(-2.2f, -0.2f, 0f));
        _TutorialScript.PuzzlePieces[1].SetRotationSolutions(new Quaternion[]{new Quaternion(0f, 0.2f, 0f, 1f)});
        _TutorialScript.PuzzlePieces[1].SetDistanceSolution(2.25f);

        _TutorialScript.GuideWall_MAT.SetColor(Shader.PropertyToID("_Color"), _TutorialScript.GuideAlbedoHue);
        Texture tmp = Resources.Load<Texture>("Tutorial Guide/TXT_Guide_4");
        if (tmp == null)
                throw new FileNotFoundException("Target asset not found in Resources folder!");
        _TutorialScript.GuideWall_MAT.SetTexture(Shader.PropertyToID("_MainTex"), tmp);
        _TutorialScript.GuideWall_MAT.SetColor(Shader.PropertyToID("_EmissionColor"), _TutorialScript.GuideEmissionHue);
    }

    public void Execute()
    {
        if (_TutorialScript.PuzzlePieces[0].isPuzzlePieceValidated)
        {
            if (_Unlocked == false)
            {
                GameManager.GM.SM.SfxSrc.PlayOneShot(GameManager.GM.SM.Sfx[6]);
                _Unlocked = true;
            }
            _TutorialScript.Tutorial_Next_BTN.interactable = true;
            _TutorialScript.GuideWall_MAT.SetColor(Shader.PropertyToID("_EmissionColor"), _TutorialScript.GuideEmissionValidHue);
        }
        else
        {
            _Unlocked = false;
            _TutorialScript.Tutorial_Next_BTN.interactable = false;
            _TutorialScript.GuideWall_MAT.SetColor(Shader.PropertyToID("_EmissionColor"), _TutorialScript.GuideEmissionHue);    
        }
    }

    public void Exit() {}
}

public class Tutorial_Step_2 : IState
{
    private Tutorial _TutorialScript;
    private bool _Unlocked = false;

    public Tutorial_Step_2(Tutorial iTutorialScript) => _TutorialScript = iTutorialScript;

    public void Enter()
    {
        _TutorialScript.CurrentStep = 2;
        _TutorialScript.FillInCard(_TutorialScript.Cards[_TutorialScript.CurrentStep]);

        _TutorialScript.Tutorial_Prev_BTN.gameObject.SetActive(true);
        _TutorialScript.Tutorial_Prev_BTN.interactable = true;
        _TutorialScript.Tutorial_Next_BTN.gameObject.SetActive(true);
        _TutorialScript.Tutorial_Next_BTN.interactable = false;

        _TutorialScript.PuzzlePieces[0].gameObject.SetActive(false);
        _TutorialScript.PuzzlePieces[0].transform.localPosition = new Vector3(8.62f, -1.599f, -1.915f);
        _TutorialScript.PuzzlePieces[0].transform.localRotation = Quaternion.Euler(-0.315f, 16f, 0f);
        _TutorialScript.PuzzlePieces[0].SetRotationConstraint(new bool[] {true, true, true});
        _TutorialScript.PuzzlePieces[0].SetTranslationConstraint(new bool[] {true,true, true});
        _TutorialScript.PuzzlePieces[0].SetDirectionSolution(new Vector3(2f, 0.2f, 0f));
        _TutorialScript.PuzzlePieces[0].SetRotationSolutions(new Quaternion[]{new Quaternion(0f, 0.1f, 0f, 1f)});
        _TutorialScript.PuzzlePieces[0].SetDistanceSolution(2.25f);

        _TutorialScript.PuzzlePieces[1].transform.localPosition = new Vector3(1.99f, -0.99f, -0.55f);
        _TutorialScript.PuzzlePieces[1].transform.localRotation = Quaternion.Euler(0f, -75f, 0f);
        _TutorialScript.PuzzlePieces[1].SetRotationConstraint(new bool[] {true, false, true});
        _TutorialScript.PuzzlePieces[1].SetTranslationConstraint(new bool[] {true, true, true});
        _TutorialScript.PuzzlePieces[1].SetDirectionSolution(new Vector3(-2f, -0.2f, 0f));
        _TutorialScript.PuzzlePieces[1].SetRotationSolutions(new Quaternion[]{new Quaternion(0f, 0.2f, 0f, 1f)});
        _TutorialScript.PuzzlePieces[1].SetDistanceSolution(2.25f);
        _TutorialScript.PuzzlePieces[1].gameObject.SetActive(true);
        
        _TutorialScript.GuideWall_MAT.SetColor(Shader.PropertyToID("_Color"), _TutorialScript.GuideAlbedoHue);
        Texture tmp = Resources.Load<Texture>("Tutorial Guide/TXT_Guide_2");
        if (tmp == null)
                throw new FileNotFoundException("Target asset not found in Resources folder!");
        _TutorialScript.GuideWall_MAT.SetTexture(Shader.PropertyToID("_MainTex"), tmp);
        _TutorialScript.GuideWall_MAT.SetColor(Shader.PropertyToID("_EmissionColor"), _TutorialScript.GuideEmissionHue);
    }

    public void Execute()
    {
        if (_TutorialScript.PuzzlePieces[1].isPuzzlePieceValidated)
        {
            if (_Unlocked == false)
            {
                GameManager.GM.SM.SfxSrc.PlayOneShot(GameManager.GM.SM.Sfx[6]);
                _Unlocked = true;
            }
            _TutorialScript.Tutorial_Next_BTN.interactable = true;
            _TutorialScript.GuideWall_MAT.SetColor(Shader.PropertyToID("_EmissionColor"), _TutorialScript.GuideEmissionValidHue);
        }
        else
        {
            _Unlocked = false;
            _TutorialScript.Tutorial_Next_BTN.interactable = false;
            _TutorialScript.GuideWall_MAT.SetColor(Shader.PropertyToID("_EmissionColor"), _TutorialScript.GuideEmissionHue);    
        }
    }

    public void Exit() {}
}

public class Tutorial_Step_3 : IState
{
    private Tutorial _TutorialScript;
    private bool _Unlocked = false;

    public Tutorial_Step_3(Tutorial iTutorialScript) => _TutorialScript = iTutorialScript;

    public void Enter()
    {
        _TutorialScript.CurrentStep = 3;
        _TutorialScript.FillInCard(_TutorialScript.Cards[_TutorialScript.CurrentStep]);

        _TutorialScript.Tutorial_Prev_BTN.gameObject.SetActive(true);        
        _TutorialScript.Tutorial_Prev_BTN.interactable = true;
        _TutorialScript.Tutorial_Next_BTN.gameObject.SetActive(true);
        _TutorialScript.Tutorial_Next_BTN.interactable = false;

        _TutorialScript.PuzzlePieces[0].transform.localPosition = new Vector3(0f, -1.599f, -1.915f);
        _TutorialScript.PuzzlePieces[0].transform.localRotation = Quaternion.Euler(-0.45f, 16f, 0f);
        _TutorialScript.PuzzlePieces[0].SetRotationConstraint(new bool[] {true, true, true});
        _TutorialScript.PuzzlePieces[0].SetTranslationConstraint(new bool[] {false, false, true});
        _TutorialScript.PuzzlePieces[0].SetDirectionSolution(new Vector3(2.2f, 0.2f, -0.5f));
        _TutorialScript.PuzzlePieces[0].SetRotationSolutions(new Quaternion[]{new Quaternion(0f, 0.1f, 0f, 1f)});
        _TutorialScript.PuzzlePieces[0].SetDistanceSolution(2.25f);
        _TutorialScript.PuzzlePieces[0].gameObject.SetActive(true);

        _TutorialScript.PuzzlePieces[1].gameObject.SetActive(false);
        _TutorialScript.PuzzlePieces[1].transform.localPosition = new Vector3(1.99f, -0.99f, -0.55f);
        _TutorialScript.PuzzlePieces[1].transform.localRotation = Quaternion.Euler(0f, 18.25f, 0f);
        _TutorialScript.PuzzlePieces[1].SetRotationConstraint(new bool[] {true, true, true});
        _TutorialScript.PuzzlePieces[1].SetTranslationConstraint(new bool[] {true, true, true});
        _TutorialScript.PuzzlePieces[1].SetDirectionSolution(new Vector3(-2.2f, -0.2f, 0f));
        _TutorialScript.PuzzlePieces[1].SetRotationSolutions(new Quaternion[]{new Quaternion(0f, 0.2f, 0f, 1f)});
        _TutorialScript.PuzzlePieces[1].SetDistanceSolution(2.25f);

        _TutorialScript.GuideWall_MAT.SetColor(Shader.PropertyToID("_Color"), _TutorialScript.GuideAlbedoHue);
        Texture tmp = Resources.Load<Texture>("Tutorial Guide/TXT_Guide_4");
        if (tmp == null)
                throw new FileNotFoundException("Target asset not found in Resources folder!");
        _TutorialScript.GuideWall_MAT.SetTexture(Shader.PropertyToID("_MainTex"), tmp);
        _TutorialScript.GuideWall_MAT.SetColor(Shader.PropertyToID("_EmissionColor"), _TutorialScript.GuideEmissionHue);    
    }

    public void Execute()
    {
        if (_TutorialScript.PuzzlePieces[0].isPuzzlePieceValidated)
        {
            if (_Unlocked == false)
            {
                GameManager.GM.SM.SfxSrc.PlayOneShot(GameManager.GM.SM.Sfx[6]);
                _Unlocked = true;
            }
            _TutorialScript.Tutorial_Next_BTN.interactable = true;
            _TutorialScript.GuideWall_MAT.SetColor(Shader.PropertyToID("_EmissionColor"), _TutorialScript.GuideEmissionValidHue);
        }
        else
        {
            _Unlocked = false;
            _TutorialScript.Tutorial_Next_BTN.interactable = false;
            _TutorialScript.GuideWall_MAT.SetColor(Shader.PropertyToID("_EmissionColor"), _TutorialScript.GuideEmissionHue);    
        }
    }

    public void Exit() {}
}

public class Tutorial_Step_4 : IState
{
    private Tutorial _TutorialScript;
    private bool _Unlocked = false;

    public Tutorial_Step_4(Tutorial iTutorialScript) => _TutorialScript = iTutorialScript;

    public void Enter()
    {
        _TutorialScript.CurrentStep = 4;
        _TutorialScript.FillInCard(_TutorialScript.Cards[_TutorialScript.CurrentStep]);

        _TutorialScript.Tutorial_Prev_BTN.gameObject.SetActive(true);
        _TutorialScript.Tutorial_Prev_BTN.interactable = true;
        _TutorialScript.Tutorial_Next_BTN.gameObject.SetActive(true);
        _TutorialScript.Tutorial_Next_BTN.GetComponentInChildren<TextMeshProUGUI>().text = "finish";
        _TutorialScript.Tutorial_Next_BTN.interactable = false;

        _TutorialScript.PuzzlePieces[0].transform.localPosition = new Vector3(2.014084f, -0.191252f, -0.4065721f);
        _TutorialScript.PuzzlePieces[0].transform.localRotation = Quaternion.Euler(220f, 90f, 0f);
        _TutorialScript.PuzzlePieces[0].SetRotationConstraint(new bool[] {false, false, true});
        _TutorialScript.PuzzlePieces[0].SetTranslationConstraint(new bool[] {false,false, true});
        _TutorialScript.PuzzlePieces[0].SetDirectionSolution(new Vector3(2.1f, 0.2f, 0.0f));
        _TutorialScript.PuzzlePieces[0].SetRotationSolutions(new Quaternion[]{
                                                                        new Quaternion(0.0f, 0.1f, 0.0f, 1f),
                                                                        new Quaternion(0.0f, -0.2f, 0.0f, -1f)});
        _TutorialScript.PuzzlePieces[0].SetDistanceSolution(2.15f);
        _TutorialScript.PuzzlePieces[0].gameObject.SetActive(true);

        _TutorialScript.PuzzlePieces[1].transform.localPosition = new Vector3(10.89755f, -0.8364128f, -2.199829f);
        _TutorialScript.PuzzlePieces[1].transform.localRotation = Quaternion.Euler(75f, 90f, 0f);
        _TutorialScript.PuzzlePieces[1].SetRotationConstraint(new bool[] {false, false, true});
        _TutorialScript.PuzzlePieces[1].SetTranslationConstraint(new bool[] {false, false, true});
        _TutorialScript.PuzzlePieces[1].SetDirectionSolution(new Vector3(-2.1f, -0.2f, 0.0f));
        _TutorialScript.PuzzlePieces[1].SetRotationSolutions(new Quaternion[]{
                                                                        new Quaternion(-0.1f, 0.0f, -1.0f, 0f),
                                                                        new Quaternion(-0.2f, 0.0f, -1.0f, 0f)});
        _TutorialScript.PuzzlePieces[1].SetDistanceSolution(2.15f);
        _TutorialScript.PuzzlePieces[1].gameObject.SetActive(true);

        _TutorialScript.GuideWall_MAT.SetColor(Shader.PropertyToID("_Color"), _TutorialScript.GuideAlbedoHue);
        Texture tmp = Resources.Load<Texture>("Tutorial Guide/TXT_Guide_42");
        if (tmp == null)
                throw new FileNotFoundException("Target asset not found in Resources folder!");
        _TutorialScript.GuideWall_MAT.SetTexture(Shader.PropertyToID("_MainTex"), tmp);
        _TutorialScript.GuideWall_MAT.SetColor(Shader.PropertyToID("_EmissionColor"), _TutorialScript.GuideEmissionHue);    
    }

    public void Execute()
    {
        if (Utility.CheckPuzzlePieces(_TutorialScript.PuzzlePieces))
        {
            if (_Unlocked == false)
            {
                GameManager.GM.SM.SfxSrc.PlayOneShot(GameManager.GM.SM.Sfx[6]);
                _Unlocked = true;
            }
            _TutorialScript.Tutorial_Next_BTN.interactable = true;
            _TutorialScript.GuideWall_MAT.SetColor(Shader.PropertyToID("_EmissionColor"), _TutorialScript.GuideEmissionValidHue);
        }
        else
        {
            _Unlocked = false;
            _TutorialScript.Tutorial_Next_BTN.interactable = false;
            _TutorialScript.GuideWall_MAT.SetColor(Shader.PropertyToID("_EmissionColor"), _TutorialScript.GuideEmissionHue);    
        }
    }

    public void Exit()
    {
        _TutorialScript.DummyPuzzlePiece.transform.localRotation = new Quaternion(42f, 42f, 42f, 42f);
        _TutorialScript.Tutorial_Next_BTN.GetComponentInChildren<TextMeshProUGUI>().text = "next";
    }
}
#endregion