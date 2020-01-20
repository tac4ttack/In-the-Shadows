using UnityEngine;
using UnityEngine.Assertions;

public class Lantern : MonoBehaviour
{
    private Light _LanternLight;
    private float NewLanternIntensity = 0f;
    private float OldLanternIntensity = 0f;
    public bool ModulateLanternLightIntensity = true;
    public Vector2 LanternLightMinMaxIntensity = new Vector2(1.25f, 3.5f);
    [Range(0f, 9.99f)] public float LanternLightMinimumIntensity = 1.25f;
    [Range(0.01f, 10f)] public float LanternLightMaximumIntensity = 3.5f;


    /*
        TO FINISH:
        replace floats by vector2 !
        tweak the settings to have a good look
        try to get a smoother effect!
     */

    private GameObject _PuzzleLightGO;
    private Light _PuzzleLight;
    private float NewPuzzleIntensity = 0f;
    private float OldPuzzleIntensity = 0f;
    public bool ModulatePuzzleLightIntensity = false;
    public bool ModulatePuzzleLightPosition = true;
    public Vector2 PuzzleLightMinMax = new Vector2(0.1f, 1.2f);
    [Range(0f, 9.99f)] public float PuzzleLightMinimumIntensity = 0.1f;
    [Range(0.01f, 10f)] public float PuzzleLightMaximumIntensity = 1.2f;    
    public Vector3 PuzzleLightPosOffset = new Vector3(2f, 0f, 2f);
    [Range(-10f, 10f)] public float PuzzleLightPositionAmplifier = 2f;

    void Awake()
    {
        if (_LanternLight == null)
            _LanternLight = this.GetComponent<Light>();
        Assert.IsNotNull(_LanternLight, "No light component found on current GameObject!");

        if (LanternLightMaximumIntensity < LanternLightMinimumIntensity)
        {
            Debug.LogWarning($"Maximum lantern intensity ({LanternLightMaximumIntensity}) is smaller than minimum lantern intensity ({LanternLightMinimumIntensity})!");
            float lanternSwap = LanternLightMaximumIntensity;
            LanternLightMaximumIntensity = LanternLightMinimumIntensity;
            LanternLightMinimumIntensity = lanternSwap;
        }
        OldLanternIntensity = _LanternLight.intensity;

        if (_PuzzleLightGO == null)
            _PuzzleLightGO = GameObject.FindGameObjectWithTag("InGame_PuzzleLight");
        Assert.IsNotNull(_PuzzleLightGO, "No puzzle light GameObject found in scene!");
        if (_PuzzleLight == null)
            _PuzzleLight = GameObject.FindGameObjectWithTag("InGame_PuzzleLight").GetComponent<Light>();
        Assert.IsNotNull(_PuzzleLight, "No puzzle light found in scene!");
        if (PuzzleLightMaximumIntensity < PuzzleLightMinimumIntensity)
        {
            Debug.LogWarning($"Maximum puzzle intensity ({PuzzleLightMaximumIntensity}) is smaller than minimum puzzle intensity ({PuzzleLightMinimumIntensity})!");
            float puzzleSwap = PuzzleLightMaximumIntensity;
            PuzzleLightMaximumIntensity = PuzzleLightMinimumIntensity;
            PuzzleLightMinimumIntensity = puzzleSwap;
        }
        OldLanternIntensity = _LanternLight.intensity;
    }

    void Update()
    {
        if (ModulateLanternLightIntensity)
        {
            NewLanternIntensity = Random.Range(LanternLightMinimumIntensity, LanternLightMaximumIntensity);
            OldLanternIntensity = _LanternLight.intensity;
            _LanternLight.intensity = Mathf.Lerp(OldLanternIntensity, NewLanternIntensity, Time.deltaTime);
        }

        if (ModulatePuzzleLightIntensity)
        {
            NewPuzzleIntensity = Random.Range(LanternLightMinimumIntensity, LanternLightMaximumIntensity);
            OldPuzzleIntensity = _PuzzleLight.intensity;
            _PuzzleLight.intensity = Mathf.Lerp(OldPuzzleIntensity, NewPuzzleIntensity, Time.deltaTime);
        }

        if (ModulatePuzzleLightPosition)
        {
            Vector3 newPos = new Vector3(Random.Range(-PuzzleLightPosOffset.x, PuzzleLightPosOffset.x),
                                        Random.Range(-PuzzleLightPosOffset.y, PuzzleLightPosOffset.y),
                                        Random.Range(-PuzzleLightPosOffset.z, PuzzleLightPosOffset.z));
            _PuzzleLightGO.transform.localPosition = Vector3.Slerp(_PuzzleLightGO.transform.localPosition, _PuzzleLightGO.transform.localPosition + newPos, Time.deltaTime);

            // _PuzzleLightGO.transform.localPosition = new Vector3(Mathf.Llerp(_PuzzleLightGO.transform.localPosition.x, _PuzzleLightGO.transform.localPosition.x + newX, Time.deltaTime * testFactor),
            //                                                      Mathf.Lerp(_PuzzleLightGO.transform.localPosition.y, _PuzzleLightGO.transform.localPosition.y + newY, Time.deltaTime * testFactor),
            //                                                      Mathf.Lerp(_PuzzleLightGO.transform.localPosition.z, _PuzzleLightGO.transform.localPosition.z + newZ, Time.deltaTime * testFactor));
        }
    }
}
