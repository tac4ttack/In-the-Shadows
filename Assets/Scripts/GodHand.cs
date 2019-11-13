using UnityEngine;
using UnityEngine.Assertions;

public class GodHand : MonoBehaviour
{
    private Rigidbody _Body;

    void Awake()
    {
        _Body = GetComponent<Rigidbody>();
        Assert.IsNotNull(_Body, "Rigid body not found for " + this.name + " GameObject!");
    }

    void OnMouseDrag()
    {
        if (!Utility.IsPointerOverUIObject())
            _Body.AddTorque(new Vector3(0.5f * Input.GetAxis("Mouse Y"), -0.5f * Input.GetAxis("Mouse X"), 0), ForceMode.Impulse);
    }
}