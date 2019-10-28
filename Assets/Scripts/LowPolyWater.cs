using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LowPolyWater : MonoBehaviour
{
    public float Scale = 1.0f;
    public float SinSpeedX = 1.0f;
    public float SinSpeedZ = 1.0f;
    public float PerlinSpeedX = 1.0f;
    public float PerlinSpeedZ = 1.0f;
    public bool RecalculateNormals = true;
    public bool UseSin = false;
    public bool UsePerlin = true;
    
    private Mesh _Mesh;
    private Vector3[] _Vertices;
    private Vector3[] _BaseVertices;

    void Start()
    {
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
                vertex.y += Mathf.Sin(vertex.x + Time.time * SinSpeedX) *
                            Mathf.Sin(vertex.z + Time.time * SinSpeedZ) * Scale;
            }

            if (UsePerlin == true && UseSin == false)
            {
                vertex.y += Mathf.PerlinNoise(vertex.x + Time.time * PerlinSpeedX,
                                              vertex.z + Time.time * PerlinSpeedZ) * Scale;
            }
            
            if (UsePerlin == true && UseSin == true)
            {
                vertex.y += Mathf.PerlinNoise(vertex.x + Time.time * PerlinSpeedX,
                                              vertex.z + Time.time * PerlinSpeedZ) *
                            Mathf.Sin(vertex.x + Time.time * SinSpeedX) *
                            Mathf.Sin(vertex.z + Time.time * SinSpeedZ) * Scale;
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