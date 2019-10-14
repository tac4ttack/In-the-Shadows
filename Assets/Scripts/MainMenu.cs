using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public void ExitButton()
    {
        Debug.Log("Exiting...");
        Application.Quit();
    }

    public void BackButton()
    {
        Debug.Log("Supposed to go back...");
    }
}
