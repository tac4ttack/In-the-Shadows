using UnityEngine;

public class EarthRotate : MonoBehaviour
{
    [SerializeField] private float _RotationSpeed = 1.0f;
    [SerializeField] private Vector3 _RotationAxis = new Vector3(0, 1, 0);

    void Update()
    {
        this.transform.Rotate(_RotationAxis, -1 * _RotationSpeed * Time.deltaTime);
    }
}
