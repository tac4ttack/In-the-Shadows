using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using TMPro;

public class LevelSelection : MonoBehaviour
{
    [Header("Elements of Level Selection UI")]
    public Button NavLeft_BTN;
    public Button NavRight_BTN;
    public TextMeshProUGUI LevelTitle_TXT;
    public TextMeshProUGUI LevelCount_TXT;

    public TextMeshProUGUI LevelDescriptionTitle_TXT;
    public TextMeshProUGUI LevelDescriptionContent_TXT;
    public TextMeshProUGUI LevelDescriptionBestTime_TXT;
    public Button LevelDescriptionPlay_BTN;

    public LevelMarker[] Levels;

    void Awake()
    {

        if (NavLeft_BTN == null)
            NavLeft_BTN = GameObject.FindGameObjectWithTag("LevelSelection_NavLeftButton").GetComponent<Button>();
        Assert.IsNotNull(NavLeft_BTN, "Left Navigation button not found!");

        if (NavRight_BTN == null)
            NavRight_BTN = GameObject.FindGameObjectWithTag("LevelSelection_NavRightButton").GetComponent<Button>();
        Assert.IsNotNull(NavRight_BTN, "Right Navigation button not found!");

        if (LevelTitle_TXT == null)
            LevelTitle_TXT = GameObject.FindGameObjectWithTag("LevelSelection_LevelTitleText").GetComponent<TextMeshProUGUI>();
        Assert.IsNotNull(LevelTitle_TXT, "Level Title text placeholder not found!");

        if (LevelCount_TXT == null)
            LevelCount_TXT = GameObject.FindGameObjectWithTag("LevelSelection_LevelCountText").GetComponent<TextMeshProUGUI>();
        Assert.IsNotNull(LevelCount_TXT, "Level Count text placeholder not found!");

        Levels = GameObject.FindGameObjectWithTag("LevelSelection_Map").GetComponentsInChildren<LevelMarker>();
        if (Levels.Length <= 0)
            Debug.LogError("Level list can't be empty!");
    }

    void Start()
    {
        // DEBUG
        // Load here the information to fill the texts!
        LevelTitle_TXT.text = "Foobar!";
        LevelCount_TXT.text = "66/66";
    }
}
