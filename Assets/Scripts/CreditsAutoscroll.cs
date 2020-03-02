using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using TMPro;

public class CreditsAutoscroll : MonoBehaviour
{
    private Scrollbar _Bar;
    [SerializeField] private float _Speed = 0.006f;
    private bool _Paused = false;
    private float _Delay = 0f;
    private string _Credits_Text;
    private Button _Credits_BTN;
    private TextMeshProUGUI _Credits_TextContainer;

    void Awake()
    {
        // DEBUG
        #if UNITY_EDITOR || DEVELOPMENT_BUILD
        Debug.Log($"CREDITS AUTOSCROLL - {this.name} - Awake()");
        #endif
        
        if (_Bar == null)
            _Bar = GetComponent<UnityEngine.UI.Scrollbar>();
        Assert.IsNotNull(_Bar, "Scroll bar component not found!");
        _Bar.value = 1f;

        _Credits_Text = Resources.Load("Credits").ToString();
        if (_Credits_Text.Length == 0)
            _Credits_Text = "Credits text is missing!";

        if (_Credits_BTN == null)
            _Credits_BTN = GameObject.FindGameObjectWithTag("Main Menu/Credits Button").GetComponent<Button>();
        Assert.IsNotNull(_Credits_BTN, "Credits button not found!");
        _Credits_BTN.onClick.AddListener(delegate { ResetScroll(); });
    
        if (_Credits_TextContainer == null)
            _Credits_TextContainer = GameObject.FindGameObjectWithTag("Main Menu/Credits Container").GetComponent<TextMeshProUGUI>();
        Assert.IsNotNull(_Credits_TextContainer, "Credits text container not found!");
        
        _Credits_TextContainer.text = _Credits_Text;
        _Bar.value = 1f;
        _Delay = 0f;
    }

    void Update()
    {
        _Delay += Time.deltaTime;

        if (!_Paused && (_Delay > 1f))
            _Bar.value = _Bar.value - Time.deltaTime * _Speed;
    }

    public void ScrollPause()
    {
        _Paused = true;
    }

    public void ScrollResume()
    {
        _Paused = false;
    }

    private void ResetScroll()
    {
        _Bar.value = 1f;
        _Delay = 0f;
    }
}
