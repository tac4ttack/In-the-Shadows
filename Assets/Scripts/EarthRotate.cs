using UnityEngine;

public class EarthRotate : MonoBehaviour
{
    [SerializeField] private float _RotationSpeed = 1.0f;
    [SerializeField] private Vector3 _RotationAxis = new Vector3(0, 1, 0);

    void Awake()
    {
        #if UNITY_EDITOR || DEVELOPMENT_BUILD
        // DEBUG
        Debug.Log($"EARTH ROTATE - {this.name} - Awake()");
        #endif

        this.transform.localRotation = Quaternion.Euler(0f, Random.Range(0f, 359f), 0f);
    }
    
    void Update()
    {
        this.transform.Rotate(_RotationAxis, -1 * _RotationSpeed * Time.deltaTime);
    }
}
