using System.Collections.Generic;

[System.Serializable]
public class PlayersData
{
    /*
        This multidimensional array is used to store the players progression
        0 -> locked | 1 -> unlocked | 2 -> completed
        You can access values like this:
            ProgressionArray[player_slot].Level[level_index]
    */
    [System.Serializable] public struct ProgressionArray { public int[] Level; }
    
    /*
        List storing for which level we have to launch the unlock cutscene
    */
    [System.Serializable] public struct UnlockQueue { public List<int> q; }

    /*
        List storing for which level we have to launch the complete cutscene
    */
    [System.Serializable] public struct CompleteQueue { public List<int> q; }



    public string[] PlayersName;
    public ProgressionArray[] Progression;
    public float[] ProgressionPercentage;
    public string[] LastPlayed;
    public bool[] IsEmpty;
    
    public UnlockQueue[] ToUnlock;
    public CompleteQueue[] ToComplete;

    public int PuzzlesAmount = 4;

    public PlayersData()
    {
        PlayersName = new string[3];
        Progression = new ProgressionArray[3];
        ProgressionPercentage = new float[3];
        LastPlayed = new string[3];
        IsEmpty = new bool[3];
        ToUnlock = new UnlockQueue[3];
        ToComplete = new CompleteQueue[3];

        for (int i = 0; i < 3; i++)
        {
            PlayersName[i] = "Player #" + (i + 1);
            Progression[i].Level = new int[PuzzlesAmount];
            ToUnlock[i].q = new List<int>();
            ToComplete[i].q = new List<int>();
            for (int j = 0; j < PuzzlesAmount - 1; j++)
            {
                if (j == 0)
                    Progression[i].Level[j] = 1;
                else
                    Progression[i].Level[j] = 0;
            }
            ProgressionPercentage[i] = 0f;
            LastPlayed[i] = "never";
            IsEmpty[i] = true;
        }
    }

    public PlayersData(PlayersData iData)
    {
        PlayersName = new string[3];
        Progression = new ProgressionArray[3];
        ProgressionPercentage = new float[3];
        LastPlayed = new string[3];
        IsEmpty = new bool[3];
        ToUnlock = new UnlockQueue[3];
        ToComplete = new CompleteQueue[3];

        for (int i = 0; i < 3; i++)
        {
            PlayersName[i] = iData.PlayersName[i];
            Progression[i].Level = new int[PuzzlesAmount];
            for (int j = 0; j < PuzzlesAmount - 1; j++)
            {
                Progression[i].Level[j] = iData.Progression[i].Level[j];
            }
            ProgressionPercentage[i] = iData.ProgressionPercentage[i];
            LastPlayed[i] = iData.LastPlayed[i];
            IsEmpty[i] = iData.IsEmpty[i];
            ToUnlock[i].q = new List<int>(iData.ToUnlock[i].q);
            ToComplete[i].q = new List<int>(iData.ToComplete[i].q);
        }
    }

    public void ResetTargetPlayer(int iSlot)
    {
        if (iSlot >= 0 && iSlot < 3)
        {
            PlayersName[iSlot] = "Player #" + (iSlot + 1);
            for (int i = 0; i < PuzzlesAmount - 1; i++)
            {
                if (i == 0)
                    Progression[iSlot].Level[i] = 1;
                else
                    Progression[iSlot].Level[i] = 0;                
                ToUnlock[iSlot].q.Clear();
                ToComplete[iSlot].q.Clear();
            }
            ProgressionPercentage[iSlot] = 0f;
            LastPlayed[iSlot] = "never";
            IsEmpty[iSlot] = true;
        }
    }
}
