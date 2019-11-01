using UnityEngine;

public class LevelMarker : MonoBehaviour
{
    public enum LevelStatus {Locked = 0, Unlocked, Completed};

    [SerializeField] private int _Id = -1;
    [SerializeField] private string _Title = "unset!";
    [SerializeField] private string _Reference = "unset!";
    [SerializeField] private Vector3 _Position = new Vector3(0f, 0f, 0f);
    [SerializeField] private LevelStatus  _Status = LevelStatus.Locked;
    [SerializeField][Multiline] private string _Description = "empty!";
    [SerializeField] private string _BestTime = "00:00:00"; // -> find a better type for this?
    
    public int Id { get => _Id; }
    public string Title { get => _Title; }
    public string Reference { get => _Reference; }
    public Vector3 Position { get => _Position; }
    public LevelStatus  Status { get => _Status; }
    public string Description { get => _Description; }
    public string BestTime { get => _BestTime; }

    void Awake()
    {
        if (Id == -1)
            Debug.LogError("ERROR in [" + this.gameObject.name + "]: LevelMarker ID is invalid!");
        if (Title == "unset!")
            Debug.LogError("ERROR in [" + this.gameObject.name + "]: LevelMarker TITLE seems unset!");
        if (Reference == "unset!")
            Debug.LogError("ERROR in [" + this.gameObject.name + "]: LevelMarker REFERENCE seems unset!");
        if (Description == "empty!")
            Debug.LogError("ERROR in [" + this.gameObject.name + "]: LevelMarker DESCRIPTION seems empty!");
        if (Id >= 0)
        {
            _Position = new Vector3(Random.Range(0f, 2f), Random.Range(0f, 2f), Random.Range(0f, 2f));
            
            // Fetch the level status
            // _Status = LevelStatus[GameManager.GM.Players[CurrentPlayer].Levels[LevelId]];

            // Best time fetch from player data!
            // _BestTime = GameManager.GM.Players[CurrentPlayer].BestTimes[LevelId];
            _BestTime = (Random.Range(0, 24)).ToString("D2")
                + ":" + (Random.Range(0, 60)).ToString("D2")
                + ":" + (Random.Range(0, 60)).ToString("D2");
            
            _Position = this.gameObject.transform.position;
        }
    }
}
