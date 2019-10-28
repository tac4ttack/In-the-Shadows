using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Puzzle : MonoBehaviour
{
    // DEBUG
    public Light            DebugSpot;

    public PuzzlePiece[]    PuzzlePieces;

    private bool            _PuzzleValidated;

    void Start()
    {
        _PuzzleValidated = false;
    }

    void Update()
    {
        CheckPuzzlePieces();

        // DEBUG
        if (_PuzzleValidated)
        {
            DebugSpot.color = Color.green;
        }
        else
        {
            DebugSpot.color = Color.white;
        }
    }

    void CheckPuzzlePieces()
    {
        bool   tmp = false;
        for (int i = 0; i < PuzzlePieces.Length; i++)
        {
            PuzzlePiece p = (PuzzlePiece)PuzzlePieces[i];
            tmp = p.isPuzzlePieceValidated;
        }
        _PuzzleValidated = tmp;
    }

}
