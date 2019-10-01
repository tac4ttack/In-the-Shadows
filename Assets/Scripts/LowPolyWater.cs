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
    private Vector3[] _baseVertices;
    public bool RecalculateNormals = true;

    public bool isSin = false;
    public bool isPerlin = true;

    Mesh Mesh;
    Vector3[] Vertices;

    void Start()
    {
        Mesh = GetComponent<MeshFilter>().mesh;

        // Fetch the plane vertices
        if (_baseVertices == null)
            _baseVertices = Mesh.vertices;

        Vertices = new Vector3[_baseVertices.Length];
    }

    void Update()
    {
        for (int i = 0; i < Vertices.Length; i++)
        {
            Vector3 vertex = _baseVertices[i];

            if (isSin == true && isPerlin == false)
            {
                vertex.y += Mathf.Sin(vertex.x + Time.time * SinSpeedX) *
                            Mathf.Sin(vertex.z + Time.time * SinSpeedZ) * Scale;
            }

            if (isPerlin == true && isSin == false)
            {
                vertex.y += Mathf.PerlinNoise(vertex.x + Time.time * PerlinSpeedX,
                                              vertex.z + Time.time * PerlinSpeedZ) * Scale;
            }
            
            if (isPerlin == true && isSin == true)
            {
                vertex.y += Mathf.PerlinNoise(vertex.x + Time.time * PerlinSpeedX,
                                              vertex.z + Time.time * PerlinSpeedZ) *
                            Mathf.Sin(vertex.x + Time.time * SinSpeedX) *
                            Mathf.Sin(vertex.z + Time.time * SinSpeedZ) * Scale;
            }

            Vertices[i] = vertex;
        }

        Mesh.MarkDynamic();
        Mesh.vertices = Vertices;
        Mesh.RecalculateBounds();

        if (RecalculateNormals)
            Mesh.RecalculateNormals();
    }
}