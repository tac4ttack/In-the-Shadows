/*
    This multidimensional array for which level we have to launch the
    unlock cutscene. 0 -> unqueued | 1 -> queued
    You can access values like this:
        UnlockQueue[player_slot].Sub[level_index]
*/
using UnlockQueue = PlayersDataNew.SerializableDoubleArray<int>;

/*
    This multidimensional array for which level we have to launch the
    complete cutscene. 0 -> unqueued | 1 -> queued
    You can access values like this:
        CompleteQueue[player_slot].Sub[level_index]
*/
using CompleteQueue = PlayersDataNew.SerializableDoubleArray<int>;

/*
    This multidimensional array is used to store the players progression
    0 -> locked | 1 -> unlocked | 2 -> completed
    You can access values like this:
        ProgressionArray[player_slot].Sub[level_index]
*/
using ProgressionArray = PlayersDataNew.SerializableDoubleArray<int>;

[System.Serializable]
public class PlayersDataNew
{

    [System.Serializable]
    public struct SerializableDoubleArray<T>
    {
        public T[] Sub;
    }

    public string[] PlayersName;
    public ProgressionArray[] Progression;
    public float[] ProgressionPercentage;
    public string[] LastPlayed;
    public bool[] IsEmpty;
    public UnlockQueue[] ToUnlock;
    public CompleteQueue[] ToComplete;

    public int _PuzzlesAmount = 4;

    public PlayersDataNew()
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
            Progression[i].Sub = new int[_PuzzlesAmount];
            ToUnlock[i].Sub = new int[_PuzzlesAmount];
            ToComplete[i].Sub = new int[_PuzzlesAmount];

            for (int j = 0; j < _PuzzlesAmount - 1; j++)
            {
                if (j == 0)
                    Progression[i].Sub[j] = 1;
                else
                    Progression[i].Sub[j] = 0;

                ToUnlock[i].Sub[j] = 0;
                ToComplete[i].Sub[j] = 0;
            }
            ProgressionPercentage[i] = 0f;
            LastPlayed[i] = "never";
            IsEmpty[i] = true;
        }
    }

    public PlayersDataNew(PlayersDataNew iData)
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
            Progression[i].Sub = new int[_PuzzlesAmount];
            ToUnlock[i].Sub = new int[_PuzzlesAmount];
            ToComplete[i].Sub = new int[_PuzzlesAmount];
            for (int j = 0; j < _PuzzlesAmount - 1; j++)
            {
                Progression[i].Sub[j] = iData.Progression[i].Sub[j];
                ToUnlock[i].Sub[j] = iData.ToUnlock[i].Sub[j];
                ToComplete[i].Sub[j] = iData.ToComplete[i].Sub[j];
            }
            ProgressionPercentage[i] = iData.ProgressionPercentage[i];
            LastPlayed[i] = iData.LastPlayed[i];
            IsEmpty[i] = iData.IsEmpty[i];
        }
    }

    public void ResetTargetPlayer(int iSlot)
    {
        if (iSlot >= 0 && iSlot < 3)
        {
            PlayersName[iSlot] = "Player #" + (iSlot + 1);
            for (int i = 0; i < _PuzzlesAmount - 1; i++)
            {
                if (i == 0)
                    Progression[iSlot].Sub[i] = 1;
                else
                    Progression[iSlot].Sub[i] = 0;

                ToUnlock[iSlot].Sub[i] = 0;
                ToComplete[iSlot].Sub[i] = 0;
            }
            ProgressionPercentage[iSlot] = 0f;
            LastPlayed[iSlot] = "never";
            IsEmpty[iSlot] = true;
        }
    }
}