using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Puzzle : MonoBehaviour
{
    public Light            debugSpot;
    public PuzzlePiece[]    pieces;

    private bool            validated;

    void Start()
    {
        validated = false;
    }

    // Update is called once per frame
    void Update()
    {
        CheckPieces();
        if (validated)
            debugSpot.color = Color.green;
        else
            debugSpot.color = Color.white;
    }

    void CheckPieces()
    {
        bool   tmp = false;
        for (int i = 0; i < pieces.Length; i++)
        {
            PuzzlePiece p = (PuzzlePiece)pieces[i];
            tmp = p.validated;
        }
        validated = tmp;
    }
}
