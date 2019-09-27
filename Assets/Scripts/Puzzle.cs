using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Puzzle : MonoBehaviour
{
    // DEBUG
    public Light            debugSpot;

    public PuzzlePiece[]    PuzzlePieces;

    private bool            _isPuzzleValidated;

    void Start()
    {
        _isPuzzleValidated = false;
    }

    void Update()
    {
        CheckPuzzlePieces();

        // DEBUG
        if (_isPuzzleValidated)
        {
            debugSpot.color = Color.green;
        }
        else
        {
            debugSpot.color = Color.white;
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
        _isPuzzleValidated = tmp;
    }

}
