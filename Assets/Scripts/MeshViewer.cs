using UnityEngine;
using UnityEngine.Assertions;

public class MeshViewer : MonoBehaviour
{
    [SerializeField] private  float  _RotationSpeed = 1.0f;
    [SerializeField] private Vector3 _RotationAxis = new Vector3(0, 1, 0);
    [SerializeField] private Vector3 _ScaleFactor = new Vector3(1f, 1f, 1f);
    [SerializeField] private Mesh _Mesh = null;
    [SerializeField] private Material _Material = null;

    private MeshFilter _MeshFilter;
    private MeshRenderer _MeshRenderer;

    void Awake()
    {
        if (_MeshFilter == null)
            _MeshFilter = this.GetComponent<MeshFilter>();
        Assert.IsNotNull(_MeshFilter, "No mesh filter component found on mesh viewer!");

        if (_MeshRenderer == null)
            _MeshRenderer = this.GetComponent<MeshRenderer>();
        Assert.IsNotNull(_MeshRenderer, "No mesh renderer component found on mesh viewer!");

        Assert.IsNotNull(_Mesh, "No mesh set for win screen!");
        Assert.IsNotNull(_Material, "No material set for win screen!");

        _MeshFilter.mesh = _Mesh;
        _MeshRenderer.material = _Material;
        this.transform.localScale = _ScaleFactor;
    }
    
    void Update()
    {
        this.transform.Rotate(_RotationAxis, -1 * _RotationSpeed * Time.deltaTime);
    }
}
