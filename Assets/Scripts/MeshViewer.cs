using UnityEngine;
using UnityEngine.Assertions;

namespace ITS.MeshViewer
{
    public class MeshViewer : MonoBehaviour
    {
        [SerializeField] private float _RotationSpeed = 1.0f;
        [SerializeField] private Vector3 _RotationAxis = new Vector3(0, 1, 0);
        [SerializeField] private Vector3 _ScaleFactor = new Vector3(1f, 1f, 1f);
        [SerializeField] private Mesh _Mesh = null;
        [SerializeField] private Material[] _Materials = new Material[1];

        private MeshFilter _MeshFilter;
        private MeshRenderer _MeshRenderer;

        void Awake()
        {
            // DEBUG
            #if UNITY_EDITOR || DEVELOPMENT_BUILD
            Debug.Log($"MESH VIEWER - {this.name} - Awake()");
            #endif
            
            if (_MeshFilter == null)
                _MeshFilter = this.GetComponent<MeshFilter>();
            Assert.IsNotNull(_MeshFilter, "No mesh filter component found on mesh viewer!");

            if (_MeshRenderer == null)
                _MeshRenderer = this.GetComponent<MeshRenderer>();
            Assert.IsNotNull(_MeshRenderer, "No mesh renderer component found on mesh viewer!");

            Assert.IsNotNull(_Mesh, "No mesh set for win screen!");
            Assert.IsTrue(_Materials.Length > 0, "Materials list is empty!");

            _MeshFilter.mesh = _Mesh;   
            this.transform.localScale = _ScaleFactor;
            _MeshRenderer.materials = new Material[_Materials.Length];
            Material[] tmp = _MeshRenderer.materials;
            for (int i = 0; i < _Materials.Length; i++)
                tmp[i] = _Materials[i];
            _MeshRenderer.materials = tmp;
        }

        void FixedUpdate()
        {
            this.transform.Rotate(_RotationAxis, -1 * _RotationSpeed * Time.deltaTime);
        }
    }
}