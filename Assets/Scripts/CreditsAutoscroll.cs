using UnityEngine;
using UnityEngine.Assertions;

public class CreditsAutoscroll : MonoBehaviour
{
    public float Speed = 0.01f;
    
    private UnityEngine.UI.Scrollbar _bar;
    private bool _paused = false;
    private float _delay = 0f;
    
    void Awake()
    {
        _bar = GetComponent<UnityEngine.UI.Scrollbar>();
        Assert.IsNotNull(_bar, "Scroll bar component not found!");
    }
    void OnEnable()
    {
        _bar.value = 1f;
        _delay = 0f;
    }

    void Update()
    {
        _delay += 0.01f;

        if (!_paused && (_delay > 1f))
            _bar.value = _bar.value - Time.deltaTime * Speed;
    }

    
    public void ScrollPause()
    {
        _paused = true;
    }

    public void ScrollResume()
    {
        _paused = false;
    }
}
