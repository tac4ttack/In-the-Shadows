using UnityEngine;
using UnityEngine.Assertions;

public class MoonRotate : MonoBehaviour
{
    public GameObject _Earth;
    public float RotationSpeed = 2.0f;
    private Vector3 _RotationAxis = new Vector3(0, 1, 0);

    void Awake()
    {
        // DEBUG
        #if UNITY_EDITOR || DEVELOPMENT_BUILD
        Debug.Log($"MOON ROTATE - {this.name} - Awake()");
        #endif
        
        if (_Earth == null)
            _Earth = GameObject.FindGameObjectWithTag("Main Menu/Earth");
        Assert.IsNotNull(_Earth, "Earth GameObject not set or found!");
    }

    void Update()
    {
        this.transform.RotateAround(_Earth.transform.position, Vector3.up, -1 * RotationSpeed * Time.deltaTime);
    }
}
