using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Assertions;
using TMPro;

public class FramerateCounter : MonoBehaviour
{
    // private TextMeshProUGUI _Counter_TXT;
    public TextMeshProUGUI _Counter_TXT;    
    private const float _UpdateFrequency = 0.5f;
    private float _ElapsedTime = 0f;
    private int _Current = 0;
    private int _TickAmount = 0;

    void Awake()
    {
        // DEBUG
        #if UNITY_EDITOR || DEVELOPMENT_BUILD
        Debug.Log($"FRAMERATE COUNTER - {this.name} - Awake()");
        #endif


        if (_Counter_TXT == null)
            _Counter_TXT = GameObject.FindGameObjectWithTag("FPS Counter").GetComponent<TextMeshProUGUI>();
        Assert.IsNotNull(_Counter_TXT, "FPS counter not found in scene!");
        DontDestroyOnLoad(_Counter_TXT.gameObject);

        _ElapsedTime = Time.realtimeSinceStartup + _UpdateFrequency;

    }

    void Start()
    {
        // DEBUG
        #if UNITY_EDITOR || DEVELOPMENT_BUILD
        Debug.Log($"FRAMERATE COUNTER - {this.name} - Start()");
        #endif

        // if (_Counter_TXT == null)
        // _Counter_TXT = GameObject.FindGameObjectWithTag("FPS Counter").GetComponent<TextMeshProUGUI>();
        // Assert.IsNotNull(_Counter_TXT, "FPS counter not found in scene!");

        // _ElapsedTime = Time.realtimeSinceStartup + _UpdateFrequency;
    }
    
    void Update()
    {
        if (_Counter_TXT != null)
        {
            if (GameManager.GM.Settings.FPSCounter)
            {
                _TickAmount++;
                if (Time.realtimeSinceStartup > _ElapsedTime)
                {
                    _Current = (int)(_TickAmount / _UpdateFrequency);
                    _TickAmount = 0;
                    _ElapsedTime += _UpdateFrequency;
                    _Counter_TXT.text = _Current.ToString();
                }
                if (_Current < 30)
                    _Counter_TXT.color = Color.red;
                else if (_Current > 30 && _Current < 60)
                    _Counter_TXT.color = Color.white;
                else
                    _Counter_TXT.color = Color.green;
            }
            else
                _Counter_TXT.text = "";
        }
    }

    void InitFPsCounter()
    {
        // DEBUG
        #if UNITY_EDITOR || DEVELOPMENT_BUILD
        Debug.Log($"FRAMERATE COUNTER - {this.name} - OnLevelWasLoaded()");
        #endif

        _Counter_TXT = GameObject.FindGameObjectWithTag("FPS Counter").GetComponent<TextMeshProUGUI>();
        Assert.IsNotNull(_Counter_TXT, "FPS counter not found in scene!");
        _ElapsedTime = Time.realtimeSinceStartup + _UpdateFrequency;
    }
}