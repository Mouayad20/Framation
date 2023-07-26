using UnityEngine;
using System.Collections.Generic;

public class MakeMesh : MonoBehaviour
{
    private Mesh mesh = null;
    public List<Vector3> vertices = new List<Vector3>();
    public List<int> triangles = new List<int>();
    public List<Vector2> uv = new List<Vector2>();

    private void Start()
    {
        mesh = GetMesh();
        MakeTriangle(new Vector3(0,0,0));
        SetMesh();
    }
    public Mesh GetMesh()
    {
        MeshFilter meshfilter = this.GetComponent<MeshFilter>();
        Mesh mesh = null;
        if (Application.isEditor == true)
        {
            mesh = meshfilter.sharedMesh;
            if (mesh == null)
            {
                meshfilter.sharedMesh = new Mesh();
                mesh = meshfilter.sharedMesh;
            }
        }
        else
        {
            mesh = meshfilter.mesh;
            if (mesh == null)
            {
                meshfilter.mesh = new Mesh();
                mesh = meshfilter.mesh;
            }
        }
        return mesh;
    }
    public void Reset()
    {
        vertices.Clear();
        triangles.Clear();
        uv.Clear();
        SetMesh();
    }
    public void MakeTriangle(Vector3 position)
    {
        vertices.Add(new Vector3(-1, 0, 0) + position);
        vertices.Add(new Vector3(0, 0 , 1) + position);
        vertices.Add(new Vector3(1, 0 , 0) + position);

        // vertices.Add(new Vector3(1, 0, 0) + position);
        // vertices.Add(new Vector3(0, 0, 1) + position);
        // vertices.Add(new Vector3(1, 0, 1) + position);

        int firstVertex = vertices.Count - 3;

        triangles.Add(0);
        triangles.Add(1);
        triangles.Add(2);
        // triangles.Add(3);
        // triangles.Add(4);
        // triangles.Add(5);
        float a1 = 256f/512f;
        float a2 = 0f  /512f;
        float a3 = 128f/512f;
        float a4 = 512f/512f;

        uv.Add(new Vector2(a2, a1));

        uv.Add(new Vector2(a1, a3));
        
        uv.Add(new Vector2(a4, a1));

        print("0 : " + uv[0]);
        print("1 : " + uv[1]);
        print("2 : " + uv[2]);



        // uv.Add(new Vector2(1, 0));
        // uv.Add(new Vector2(0, 1));
        // uv.Add(new Vector2(0, 0));
    }
    public void SetMesh()
    {
        if (mesh == null)
        {
            mesh = GetMesh();
        }

        mesh.Clear();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uv.ToArray();
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        mesh.RecalculateTangents();
    }
}