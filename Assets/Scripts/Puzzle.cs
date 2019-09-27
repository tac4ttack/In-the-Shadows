using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Puzzle : MonoBehaviour
{
    // DEBUG
    public Light            debugSpot;

    public PuzzlePiece[]    PuzzlePieces;

    private bool            _validated;

    void Start()
    {
        _validated = false;
    }

    // Update is called once per frame
    void Update()
    {
        CheckPuzzlePieces();
        
        // DEBUG
        if (_validated)
            debugSpot.color = Color.green;
        else
            debugSpot.color = Color.white;
    }

    void CheckPuzzlePieces()
    {
        bool   tmp = false;
        for (int i = 0; i < PuzzlePieces.Length; i++)
        {
            PuzzlePiece p = (PuzzlePiece)PuzzlePieces[i];
            tmp = p.validated;
        }
        _validated = tmp;
    }
}
