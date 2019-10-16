[System.Serializable]
public class PlayersData
{
    public string[] PlayersName;
    public bool[] IsEmpty;
    public string[] LastPlayed;
    public float[] ProgressionPercentage;
    public int[][] Progression; // 0 -> locked | 1 -> unlocked | 2 -> completed


    public int[,]Testiboule;

    // CANT SERIALIZE MULTIDIM ARRAY?????????????
    //Create the 1-dimensional array that will represent the 2d array
    // public BlockColors [] board = new BlockColors [collumns * rows];
    //Translate the index [x, y] to the index in the 1D array
    //Where x and y represent indexes, and width, the width of the matrix
    // board[x + y * width]

    public int _puzzlesAmount = 3;

    public PlayersData()
    {
        PlayersName = new string[3];
        IsEmpty = new bool[3];
        LastPlayed = new string[3];
        ProgressionPercentage = new float[3];
        
        Progression = new int[3][];
        for (int i = 0; i < 3; i++)
        {
            PlayersName[i] = "John Doe";
            IsEmpty[i] = true;
            LastPlayed[i] = "never";
            ProgressionPercentage[i] = 0f;
            Progression[i] = new int[_puzzlesAmount];
            for (int j = 0; j < _puzzlesAmount; j++)
                Progression[i][j] = 0;
        }
    }

    public PlayersData(PlayersData iData)
    {
        for (int i = 0; i < 3; i++)
        {
            PlayersName[i] = iData.PlayersName[i];
            IsEmpty[i] = iData.IsEmpty[i];
            LastPlayed[i] = iData.LastPlayed[i];
            ProgressionPercentage[i] = iData.ProgressionPercentage[i];
            for (int j = 0; j < _puzzlesAmount; j++)
                Progression[i][j] = iData.Progression[i][j];
        }
    }
}
