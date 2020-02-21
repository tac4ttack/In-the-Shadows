using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public static class Utility
{
    public const float TransitionSpeed = 0.1f;
    public const int LevelSceneIndexOffset = 2;
    public static Material Puzzle_Material = Resources.Load("Assets/Materials/PuzzlePiece.mat") as Material;

    public static bool IsPointerOverUIObject()
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        List<RaycastResult> raycastResults = new List<RaycastResult>();

        eventData.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        EventSystem.current.RaycastAll(eventData, raycastResults);

        return (raycastResults.Count > 0);
    }

    public static int NoRepeatRandom(int iMin, int iMax, int iPrev)
    {
        int i = Random.Range(iMin, iMax);
        while (i == iPrev)
            i = Random.Range(iMin, iMax);
        return i;
    }

    public static float NoRepeatRandom(float iMin, float iMax, float iPrev)
    {
        float i = Random.Range(iMin, iMax);
        while (i == iPrev)
            i = Random.Range(iMin, iMax);
        return i;
    }

    public static IEnumerator PopInCanvasGroup(CanvasGroup iCanvasGroup, float iTime, float iSpeed, float iTargetAlpha = 1f)
    {
        if (iCanvasGroup != null)
        {
            iCanvasGroup.interactable = true;
            iCanvasGroup.blocksRaycasts = true;
            for (float t = 0f; t < iTime; t += iSpeed)
            {
                iCanvasGroup.alpha += 0.1f;
                yield return null;
            }
        }
    }

    public static IEnumerator PopOutCanvasGroup(CanvasGroup iCanvasGroup, float iTime, float iSpeed, float iTargetAlpha = 0f)
    {
        if (iCanvasGroup != null)
        {
            iCanvasGroup.interactable = false;
            iCanvasGroup.blocksRaycasts = false;
            for (float t = 0f; t < iTime; t += iSpeed)
            {
                iCanvasGroup.alpha -= 0.1f;
                yield return null;
            }
        }
    }

    public static bool CheckPuzzlePieces(PuzzlePiece[] iPuzzlePiecesArray)
    {
        bool[] tmp = new bool[iPuzzlePiecesArray.Length];
        bool result = true;

        for (int i = 0; i < iPuzzlePiecesArray.Length; i++)
        {
            PuzzlePiece p = (PuzzlePiece)iPuzzlePiecesArray[i];
            tmp[i] = p.isPuzzlePieceValidated;
        }

        for (int i = 0; i < tmp.Length; i++)
        {
            if (tmp[i] == false)
                result = false;
        }

        return result;
    }

    public static int CurrentLevelIndex => SceneManager.GetActiveScene().buildIndex - LevelSceneIndexOffset;
    public static int CurrentPlayer => GameManager.GM.CurrentPlayerSlot;
    public static int PuzzleAmount => GameManager.GM.Players.PuzzlesAmount;
}
