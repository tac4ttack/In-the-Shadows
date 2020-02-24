using UnityEngine;

public class AxisHints : MonoBehaviour
{
    private MeshRenderer[] _Axis;

    void Awake()
    {
        _Axis = this.transform.GetComponentsInChildren<MeshRenderer>();
        if (_Axis.Length != 3 || _Axis == null)
        {
            // DEBUG
            #if UNITY_EDITOR
            Debug.LogError("AxisHint gameObject is corrupted");
            #endif
            
            return;
        }
    }

    public void Enable(bool iAction)
    {
        for (int i = 0; i < _Axis.Length; i++)
            _Axis[i].enabled = iAction;
    }
}
