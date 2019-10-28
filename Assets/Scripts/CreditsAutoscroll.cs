using UnityEngine;
using UnityEngine.Assertions;

public class CreditsAutoscroll : MonoBehaviour
{
    public float Speed = 0.01f;
    
    private UnityEngine.UI.Scrollbar _Bar;
    private bool _Paused = false;
    private float _Delay = 0f;
    
    void Awake()
    {
        _Bar = GetComponent<UnityEngine.UI.Scrollbar>();
        Assert.IsNotNull(_Bar, "Scroll bar component not found!");
    }
    void OnEnable()
    {
        _Bar.value = 1f;
        _Delay = 0f;
    }

    void Update()
    {
        _Delay += 0.01f;

        if (!_Paused && (_Delay > 1f))
            _Bar.value = _Bar.value - Time.deltaTime * Speed;
    }

    
    public void ScrollPause()
    {
        _Paused = true;
    }

    public void ScrollResume()
    {
        _Paused = false;
    }
}
