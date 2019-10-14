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
        Debug.Log("Awakening starfield!");

        float randomSize;
        float color;

        // if (Screen.currentResolution.width < 1500.0f)
        //     StarFieldWidth = 1500.0f;
        // else
        //     StarFieldWidth = Screen.currentResolution.width;
        
        // if (Screen.currentResolution.height < 1000.0f)
        //     StarFieldHeight = 1000.0f;
        // else
        //     StarFieldHeight = Screen.currentResolution.height;
        

        _Emitter = this.GetComponent<ParticleSystem>();
        Assert.IsNotNull(_Emitter, "Particle System component is missing from GameObject!");
        
        _xOffset = StarFieldWidth * 0.5f;
        _yOffset = StarFieldHeight * 0.5f;

        _Stars = new ParticleSystem.Particle[StarAmount];
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
            _Stars[i].startColor = new Color(1.0f, color + GreenOffset, color + BlueOffset, 1.0f);
        }
        _Emitter.SetParticles(_Stars, _Stars.Length);
    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.J))
            Awake();

        for (int i = 0; i < _Stars.Length; i++)
        {
            // _Stars[i]. = Mathf.Sin(Time.deltaTime) * 1.0f;
        }
    }
}
