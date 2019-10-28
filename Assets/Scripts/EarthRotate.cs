using UnityEngine;

public class EarthRotate : MonoBehaviour
{
    public  float   RotationSpeed = 1.0f;
    private Vector3 _RotationAxis = new Vector3(0, 1, 0);

    void Update()
    {
        this.transform.Rotate(_RotationAxis, -1 * RotationSpeed * Time.deltaTime);
    }
}
