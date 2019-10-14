using UnityEngine;
using UnityEngine.Assertions;

public class MoonRotate : MonoBehaviour
{
    public GameObject Earth;
    public  float   rotationSpeed = 2.0f;
    private Vector3 _rotationAxis = new Vector3(0, 1, 0);

    void Awake()
    {
        Assert.IsNotNull(Earth, "Earth GameObject not set!");
    }
    
    void Update()
    {
        this.transform.RotateAround(Earth.transform.position, Vector3.up, -1 * rotationSpeed * Time.deltaTime);
    }
}
