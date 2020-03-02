using UnityEngine;

public class Water : MonoBehaviour
{
    public float Scale = 1.0f;
    public Vector2 SinSpeed = new Vector2(1.0f, 1.0f);
    public Vector2 PerlinSpeed = new Vector2(1.0f, 1.0f);
    public bool RecalculateNormals = true;
    public bool UseSin = false;
    public bool UsePerlin = true;

    private Mesh _Mesh;
    private Vector3[] _Vertices;
    private Vector3[] _BaseVertices;

    void Awake()
    {
        // DEBUG
        #if UNITY_EDITOR || DEVELOPMENT_BUILD
        Debug.Log($"WATER - {this.name} - Awake()");
        #endif

        _Mesh = GetComponent<MeshFilter>().mesh;

        // Fetch the plane vertices
        if (_BaseVertices == null)
            _BaseVertices = _Mesh.vertices;

        _Vertices = new Vector3[_BaseVertices.Length];
    }

    void Update()
    {
        for (int i = 0; i < _Vertices.Length; i++)
        {
            Vector3 vertex = _BaseVertices[i];

            if (UseSin == true && UsePerlin == false)
            {
                vertex.y += Mathf.Sin(vertex.x + Time.time * SinSpeed.x) *
                            Mathf.Sin(vertex.z + Time.time * SinSpeed.y) * Scale;
            }

            if (UsePerlin == true && UseSin == false)
            {
                vertex.y += Mathf.PerlinNoise(vertex.x + Time.time * PerlinSpeed.x,
                                              vertex.z + Time.time * PerlinSpeed.y) * Scale;
            }

            if (UsePerlin == true && UseSin == true)
            {
                vertex.y += Mathf.PerlinNoise(vertex.x + Time.time * PerlinSpeed.x,
                                              vertex.z + Time.time * PerlinSpeed.y) *
                            Mathf.Sin(vertex.x + Time.time * SinSpeed.x) *
                            Mathf.Sin(vertex.z + Time.time * SinSpeed.y) * Scale;
            }

            _Vertices[i] = vertex;
        }

        _Mesh.MarkDynamic();
        _Mesh.vertices = _Vertices;
        _Mesh.RecalculateBounds();

        if (RecalculateNormals)
            _Mesh.RecalculateNormals();
    }
}