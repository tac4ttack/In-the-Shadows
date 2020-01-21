using UnityEngine;
using UnityEngine.Assertions;

public class Lantern : MonoBehaviour
{
    [Header("Lantern light settings")]
    public Light _LanternLight;
    public bool ModulateLanternLightIntensity = true;
    [Range(0f,10f)] public float LanternIntensityOffset = 1.5f;
    [Range(0f,2f)] public float LanternIntensitySpeed = 0.5f;
    private float _StartLanternIntensity = 0f;
    private float _NewLanternIntensity = 0f;
    private float _OldLanternIntensity = 0f;
    [Header("Puzzle light settings")]
    public GameObject _PuzzleLight;
    public bool ModulatePuzzleLightIntensity = false;
    [Range(0f,10f)] public float PuzzleIntensityOffset = 1.5f;
    [Range(0f,2f)] public float PuzzleIntensitySpeed = 0.5f;
    public bool ModulatePuzzleLightPosition = true;
    public Vector3 PuzzlePositionOffset = new Vector3(0f, 0f, 0f);
    [Range(0f,2f)] public float PuzzleMovementSpeed = 0.5f;
    private Light _PuzzleSpotLight;
    private float _StartPuzzleIntensity = 0f;    
    private float _NewPuzzleIntensity = 0f;
    private float _OldPuzzleIntensity = 0f;
    private Vector3 _StartPuzzlePos = new Vector3(0f, 0f, 0f);
    private Vector3 _NewPuzzlePos = new Vector3(0f, 0f, 0f);
    private Vector3 _OldPuzzlePos = new Vector3(0f, 0f, 0f);

    void Awake()
    {
        if (_LanternLight == null)
            _LanternLight = this.GetComponent<Light>();
        Assert.IsNotNull(_LanternLight, "No light component found on current GameObject!");

        if (_PuzzleLight == null)
            _PuzzleLight = GameObject.FindGameObjectWithTag("InGame_PuzzleLight");
        Assert.IsNotNull(_PuzzleLight, "No puzzle light GameObject found in scene!");

        if (_PuzzleSpotLight == null)
            _PuzzleSpotLight = GameObject.FindGameObjectWithTag("InGame_PuzzleLight").GetComponent<Light>();
        Assert.IsNotNull(_PuzzleSpotLight, "No puzzle light found in scene!");

        _StartLanternIntensity = _LanternLight.intensity;
        _OldLanternIntensity = _LanternLight.intensity;

        _StartPuzzleIntensity = _PuzzleSpotLight.intensity;
        _OldPuzzleIntensity = _PuzzleSpotLight.intensity;

        _StartPuzzlePos = _PuzzleLight.transform.position;
        _OldPuzzlePos = _PuzzleLight.transform.position;
    }

    void Update()
    {
        if (ModulateLanternLightIntensity)
            ModulateLanternIntensity();

        if (ModulatePuzzleLightIntensity)
            ModulatePuzzleIntensity();

        if (ModulatePuzzleLightPosition)
            ModulatePuzzlePosition();
    }

    void ModulateLanternIntensity()
    {
        if (ModulateLanternLightIntensity)
        {
            _OldLanternIntensity = _LanternLight.intensity;
            _NewLanternIntensity = Random.Range(_StartLanternIntensity - LanternIntensityOffset, _StartLanternIntensity + LanternIntensityOffset);
            _LanternLight.intensity = Mathf.Lerp(_OldLanternIntensity, _NewLanternIntensity, Time.deltaTime * LanternIntensitySpeed);
        }
    }

    void ModulatePuzzleIntensity()
    {
            _OldPuzzleIntensity = _PuzzleSpotLight.intensity;
            _NewPuzzleIntensity = Random.Range(_StartPuzzleIntensity - PuzzleIntensityOffset, _StartPuzzleIntensity + PuzzleIntensityOffset);
            _PuzzleSpotLight.intensity = Mathf.Lerp(_OldPuzzleIntensity, _NewPuzzleIntensity, Time.deltaTime * PuzzleIntensitySpeed);
    }

    void ModulatePuzzlePosition()
    {
        _OldPuzzlePos = _PuzzleLight.transform.position;

        _NewPuzzlePos = new Vector3(Random.Range(_StartPuzzlePos.x - PuzzlePositionOffset.x, _StartPuzzlePos.x + PuzzlePositionOffset.x),
                                    Random.Range(_StartPuzzlePos.y - PuzzlePositionOffset.y, _StartPuzzlePos.y + PuzzlePositionOffset.y),
                                    Random.Range(_StartPuzzlePos.z - PuzzlePositionOffset.z, _StartPuzzlePos.z + PuzzlePositionOffset.z));
        
        _PuzzleLight.transform.position = Vector3.Lerp(_OldPuzzlePos, _NewPuzzlePos, Time.deltaTime * PuzzleMovementSpeed);
    }
}
