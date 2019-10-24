using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;

public class PauseMenu : MonoBehaviour
{
    public CanvasGroup Background_CG;
    public CanvasGroup PauseMenu_CG;
    public CanvasGroup Settings_CG;

    private float _transitionSpeed = 0.1f;
    private bool _paused = false;

    void Awake()
    {
        if (!Background_CG)
            Background_CG = this.gameObject.transform.Find("PauseMenu_Background").GetComponent<CanvasGroup>();;
        Assert.IsNotNull(Background_CG, "Pause menu background not found in scene!");

        if (!PauseMenu_CG)
            PauseMenu_CG = this.gameObject.transform.Find("PauseMenu_Panel").GetComponent<CanvasGroup>();
        Assert.IsNotNull(PauseMenu_CG, "Pause menu panel not found in scene!");

        if (!Settings_CG)
            Settings_CG = this.gameObject.transform.Find("Settings_Panel").GetComponent<CanvasGroup>();
        Assert.IsNotNull(Settings_CG, "Settings panel not found in scene!");

        Background_CG.gameObject.SetActive(true);
        Background_CG.alpha = 0f;
        Background_CG.blocksRaycasts = false;
        
        PauseMenu_CG.gameObject.SetActive(true);
        PauseMenu_CG.alpha = 0f;
        PauseMenu_CG.blocksRaycasts = false;

        Settings_CG.gameObject.SetActive(true);
        Settings_CG.alpha = 0f;
        Settings_CG.blocksRaycasts = false;

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("toto");
            if (_paused)
                StartCoroutine(PopOut(1f));
            else
                StartCoroutine(PopUpPauseMenu(1f));

        }
    }

    IEnumerator PopUpPauseMenu(float iTime)
    {
        _paused = true;
        for (float t = 0f; t < iTime; t += _transitionSpeed)
        {
            Background_CG.alpha += 0.1f;
            Background_CG.blocksRaycasts = true;
            PauseMenu_CG.alpha += 0.1f;
            PauseMenu_CG.blocksRaycasts = true;
            yield return 0;
        }
    }

    IEnumerator PopOut(float iTime)
    {
        _paused = false;
        for (float t = 0f; t < iTime; t += _transitionSpeed)
        {
            Background_CG.alpha -= 0.1f;
            Background_CG.blocksRaycasts = false;
            PauseMenu_CG.alpha -= 0.1f;
            PauseMenu_CG.blocksRaycasts = false;
            yield return 0;
        }
    }
}
