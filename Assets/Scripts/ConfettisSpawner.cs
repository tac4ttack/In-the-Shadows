using System.IO;
using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;

public class ConfettisSpawner : MonoBehaviour
{
    public enum SpawnModes { OneShot = 0, Loop, RandomFixed };
    public GameObject TargetToSpawn;
    public SpawnModes SpawnMode = SpawnModes.OneShot;
    public int Amount = 0;
    public float Delay = 0.5f;
    public Vector3[] SpawnPositions;
    public Vector3[] SpawnOffsets;

    private int _LoopCounter = 0;
    private int _PreviousRandom = 0;

    void Awake()
    {
        if (TargetToSpawn == null)
        {
            Debug.LogWarning("No spawnable GameObject set up, fetching default one!");
            TargetToSpawn = Resources.Load<GameObject>("Confettis_ParticleSystem");
            if (TargetToSpawn == null)
                throw new FileNotFoundException("Spawn target asset not found in Resources folder!");
        }
        Assert.IsNotNull(TargetToSpawn, "No GameObject to spawn found!");
        Assert.IsTrue((SpawnPositions.Length > 0), "Now spawn positions found! Please set them up in inspector!");
        Assert.IsFalse((SpawnPositions.Length != SpawnOffsets.Length), "Spawn position offsets amount is different from amount of spawn Spawnpositions!");
    }

    void Start()
    {
        if (SpawnMode == SpawnModes.OneShot)
        {
            for (int i = 0; i < SpawnPositions.Length; i++)
                StartCoroutine(SpawnInstance(i * Delay, i));
        }
        else if (SpawnMode == SpawnModes.RandomFixed)
        {
            for (int j = 0; j < Amount; j++)
            {
                _PreviousRandom = Utility.NoRepeatRandom(0, SpawnPositions.Length, _PreviousRandom);
                StartCoroutine(SpawnInstance(j * Delay, _PreviousRandom));
            }
        }
    }

    void Update()
    {
        if (SpawnMode == SpawnModes.Loop)
        {
                _PreviousRandom = Utility.NoRepeatRandom(0, SpawnPositions.Length, _PreviousRandom);
                StartCoroutine(SpawnInstance(_LoopCounter * Delay, _PreviousRandom));
                _LoopCounter++;
        }
    }

    private IEnumerator SpawnInstance(float iDelay, int iTarget)
    {
        Vector3 randomPosition = new Vector3(
                Random.Range(SpawnPositions[iTarget].x - SpawnOffsets[iTarget].x, SpawnPositions[iTarget].x + SpawnOffsets[iTarget].x),
                Random.Range(SpawnPositions[iTarget].y - SpawnOffsets[iTarget].y, SpawnPositions[iTarget].y + SpawnOffsets[iTarget].y),
                Random.Range(SpawnPositions[iTarget].z - SpawnOffsets[iTarget].z, SpawnPositions[iTarget].z + SpawnOffsets[iTarget].z));
                
        yield return new WaitForSeconds(iDelay);

        GameObject spawn = GameObject.Instantiate<GameObject>(TargetToSpawn, this.transform, true);
        spawn.transform.localPosition = randomPosition;
    }
}
