using UnityEngine;

public class AxisHints : MonoBehaviour
{
    private MeshRenderer[] _Axis;

    void Awake()
    {
        #if UNITY_EDITOR || DEVELOPMENT_BUILD
        // DEBUG
        Debug.Log($"AXIS HINT - {this.name} - Awake()");
        #endif
        
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
