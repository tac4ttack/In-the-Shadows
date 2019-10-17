using UnityEngine;

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

    public int _puzzlesAmount = 3;

    public PlayersData()
    {
        PlayersName = new string[3];
        Progression = new ProgressionArray[3];
        ProgressionPercentage = new float[3];
        LastPlayed = new string[3];
        IsEmpty = new bool[3];

        for (int i = 0; i < 3; i++)
        {
            PlayersName[i] = "John Doe";
            Progression[i].Level = new int[3];
            for (int j = 0; j < _puzzlesAmount; j++)
                Progression[i].Level[j] = i + j;
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
        for (int i = 0; i < 3; i++)
        {
            PlayersName[i] = iData.PlayersName[i];
            Progression[i].Level = new int[3];
            for (int j = 0; j < _puzzlesAmount; j++)
                Progression[i].Level[j] = iData.Progression[i].Level[j];
            ProgressionPercentage[i] = iData.ProgressionPercentage[i];
            LastPlayed[i] = iData.LastPlayed[i];
            IsEmpty[i] = iData.IsEmpty[i];
        }
    }
}
