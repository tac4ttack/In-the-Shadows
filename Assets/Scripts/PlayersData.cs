[System.Serializable]
public class PlayersData
{
    /*
        This multidimensional array is used to store the players progression
        0 -> locked | 1 -> unlocked | 2 -> completed
        You can access values like this:
            ProgressionArray[player_slot].Level[level_index]
    */
    [System.Serializable]
    public struct ProgressionArray
    {
        public int[] Level;
    }

    public string[] PlayersName;
    public ProgressionArray[] Progression;
    public float[] ProgressionPercentage;
    public string[] LastPlayed;
    public bool[] IsEmpty;
    public bool[] DoTutorial;

    public int _puzzlesAmount = 3;

    public PlayersData()
    {
        PlayersName = new string[3];
        Progression = new ProgressionArray[3];
        ProgressionPercentage = new float[3];
        LastPlayed = new string[3];
        IsEmpty = new bool[3];
        DoTutorial = new bool[3];

        for (int i = 0; i < 3; i++)
        {
            PlayersName[i] = "Player #" + (i + 1);
            Progression[i].Level = new int[3];
            for (int j = 0; j < _puzzlesAmount; j++)
            {
                if (j == 0)
                    Progression[i].Level[j] = 1;
                else
                    Progression[i].Level[j] = 0;
            }
            ProgressionPercentage[i] = 0f;
            LastPlayed[i] = "never";
            IsEmpty[i] = true;
            DoTutorial[i] = true;
        }
    }

    public PlayersData(PlayersData iData)
    {
        PlayersName = new string[3];
        Progression = new ProgressionArray[3];
        ProgressionPercentage = new float[3];
        LastPlayed = new string[3];
        IsEmpty = new bool[3];
        DoTutorial = new bool[3];

        for (int i = 0; i < 3; i++)
        {
            PlayersName[i] = iData.PlayersName[i];
            Progression[i].Level = new int[3];
            for (int j = 0; j < _puzzlesAmount; j++)
                Progression[i].Level[j] = iData.Progression[i].Level[j];
            ProgressionPercentage[i] = iData.ProgressionPercentage[i];
            LastPlayed[i] = iData.LastPlayed[i];
            IsEmpty[i] = iData.IsEmpty[i];
            DoTutorial[i] = iData.DoTutorial[i];
        }
    }

    public void ResetTargetPlayer(int iSlot)
    {
        if (iSlot >= 0 && iSlot < 3)
        {
            PlayersName[iSlot] = "Player #" + (iSlot + 1);
            for (int i = 0; i < _puzzlesAmount; i++)
            {
                if (i == 0)
                    Progression[iSlot].Level[i] = 1;
                else
                    Progression[iSlot].Level[i] = 0;
            }
            ProgressionPercentage[iSlot] = 0f;
            LastPlayed[iSlot] = "never";
            IsEmpty[iSlot] = true;
            DoTutorial[iSlot] = true;
        }
    }
}
