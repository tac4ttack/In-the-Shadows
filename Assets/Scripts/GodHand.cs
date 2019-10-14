using UnityEngine;
using UnityEngine.Assertions;

public class GodHand : MonoBehaviour
{
    private Rigidbody _body;

    void Awake()
    {
        _body = GetComponent<Rigidbody>();
        Assert.IsNotNull(_body, "Rigid body not found for " + this.name + " GameObject!");
    }

    void OnMouseDrag()
    {
        _body.AddTorque(new Vector3(0, -1 * Input.GetAxis("Mouse X"), 0), ForceMode.Impulse);
    }
}