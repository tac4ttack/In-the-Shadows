using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
public class Starfield : MonoBehaviour
{
    public int      StarAmount = 100;
    public float    StarSize = 0.5f;
    public float    StarSizeRange = 0.5f;
    public float    StarFieldWidth = 64.0f;
    public float    StarFieldHeight = 36.0f;
    public bool     AddColor = false;

    private float                       _xOffset;
    private float                       _yOffset;
    private ParticleSystem              _Emitter;
    private ParticleSystem.Particle[]   _Stars;

    public float GreenOffset = 0f;
    public float BlueOffset = 0f;

    void Awake()
    {
        //DEBUG
        // Debug.Log("Awakening starfield!");

        float randomSize;
        float color;

        _Emitter = this.GetComponent<ParticleSystem>();
        Assert.IsNotNull(_Emitter, "Particle System component is missing from GameObject!");
        
        _xOffset = StarFieldWidth * 0.5f;
        _yOffset = StarFieldHeight * 0.5f;

        _Stars = new ParticleSystem.Particle[StarAmount];
        _StarsStartSize = new float[StarAmount];
        _StarsBlink = new bool[StarAmount];

        for (int i = 0; i < StarAmount; i++)
        {
            randomSize = Random.Range(StarSizeRange, StarSizeRange + 1.0f);
            if (AddColor)
            {
                color = randomSize - StarSizeRange;
            }
            else
            {
                color = 1f;
            }
            _Stars[i].position = new Vector3(Random.Range(0, StarFieldWidth) - _xOffset,
                                            Random.Range(0, StarFieldHeight) - _yOffset,
                                            0);
            _Stars[i].startSize = StarSize * randomSize;
            
            // WIP
            _StarsBlink[i] = Random.Range(0f, 1f) > 0.75f ? true : false ;
            _StarsStartSize[i] = _Stars[i].startSize;

            _Stars[i].startColor = new Color(1.0f, color + GreenOffset, color + BlueOffset, 1.0f);
        }
        _Emitter.SetParticles(_Stars, _Stars.Length);
    }

    public float ShineSpeed = 0.5f;
    public float MaxExpand = 0.5f;
    private float[] _StarsStartSize;
    private bool[] _StarsBlink;

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.J))
            Awake();
        for (int i = 0; i < _Stars.Length; i++)
        {
            if (_StarsBlink[i])
                _Stars[i].startSize = (Mathf.Sin(Time.time * ShineSpeed) + 1.0f) / 2.0f * MaxExpand + _StarsStartSize[i];;
            // _Stars[i].startColor = new Color(1.0f, Random.Range(0, 1f), Random.Range(0, 1f), 1.0f);
            // _Stars[i].startSize = _Stars[i].GetCurrentSize(_Emitter) * Mathf.Sin(Time.deltaTime) * 10f;
            // _Stars[i].startSize = Mathf.PingPong(Time.time * ShineSpeed, MaxExpand);
        }
        _Emitter.SetParticles(_Stars, _Stars.Length);
    }

    
    
    // var range = maxSize - minSize;
    // transform.localScale.y = minSize + Mathf.PingPong(Time.time * speed, range);
    // transform.localScale.y = (Mathf.Sin(Time.time * speed) + 1.0) / 2.0 * range + minSize;
}
