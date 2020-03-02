using UnityEngine;
using UnityEngine.Assertions;
public class Starfield : MonoBehaviour
{
    public int StarAmount = 100;
    public float StarSize = 0.5f;
    public float StarSizeRange = 0.5f;
    public float StarFieldWidth = 64.0f;
    public float StarFieldHeight = 36.0f;
    public bool AddColor = false;

    private float _OffsetX;
    private float _OffsetY;
    private ParticleSystem _Emitter;
    private ParticleSystem.Particle[] _Stars;

    public float ShineSpeed = 0.5f;
    public float MaxExpand = 0.5f;
    private float[] _StarsStartSize;
    private bool[] _StarsBlink;

    public float GreenOffset = 0f;
    public float BlueOffset = 0f;

    void Awake()
    {
        // DEBUG
        #if UNITY_EDITOR || DEVELOPMENT_BUILD
        Debug.Log($"STAR FIELD - {this.name} - Awake()");
        #endif
        
        float randomSize;
        float color;

        _Emitter = this.GetComponent<ParticleSystem>();
        Assert.IsNotNull(_Emitter, "Particle System component is missing from GameObject!");

        _OffsetX = StarFieldWidth * 0.5f;
        _OffsetY = StarFieldHeight * 0.5f;

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
            _Stars[i].position = new Vector3(Random.Range(0, StarFieldWidth) - _OffsetX,
                                            Random.Range(0, StarFieldHeight) - _OffsetY,
                                            0);
            _Stars[i].startSize = StarSize * randomSize;

            _StarsBlink[i] = Random.Range(0f, 1f) > 0.75f ? true : false;
            _StarsStartSize[i] = _Stars[i].startSize;

            _Stars[i].startColor = new Color(1.0f, color + GreenOffset, color + BlueOffset, 1.0f);
        }
        _Emitter.SetParticles(_Stars, _Stars.Length);
    }

    void Update()
    {
        for (int i = 0; i < _Stars.Length; i++)
        {
            if (_StarsBlink[i])
                _Stars[i].startSize = (Mathf.Sin(Time.time * ShineSpeed) + 1.0f) / 2.0f * MaxExpand + _StarsStartSize[i]; ;
        }
        _Emitter.SetParticles(_Stars, _Stars.Length);
    }
}
