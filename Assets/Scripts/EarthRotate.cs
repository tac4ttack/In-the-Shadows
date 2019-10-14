using UnityEngine;

public class EarthRotate : MonoBehaviour
{
    public  float   rotationSpeed = 1.0f;
    private Vector3 _rotationAxis = new Vector3(0, 1, 0);

    void Update()
    {
        this.transform.Rotate(_rotationAxis, -1 * rotationSpeed * Time.deltaTime);
    }
}
