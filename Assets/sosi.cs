using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sosi : MonoBehaviour
{
    [SerializeField]  public Texture2D texture;
    [SerializeField] Material lineMat;

    
    void Update()
    {
        // set the material's main texture to the provided texture
        Material material = new Material(Shader.Find("Standard"));
        lineMat.SetTexture("_MainTex", texture);
        lineMat.color = Color.white;
        lineMat.SetPass(0);

        // define the triangle's vertices
        Vector3[] vertices = new Vector3[3];
        vertices[0] = new Vector3(0, 0, 0);
        vertices[1] = new Vector3(1, 0, 0);
        vertices[2] = new Vector3(0, 1, 0);

        // define the triangle's texture coordinates
        Vector2[] texCoords = new Vector2[3];
        texCoords[0] = new Vector2(0, 0);
        texCoords[1] = new Vector2(1, 0);
        texCoords[2] = new Vector2(0, 1);

        // draw the triangle
        GL.Begin(GL.TRIANGLES);
        for (int i = 0; i < 3; i++)
        {
            GL.TexCoord2(texCoords[i].x, texCoords[i].y);
            GL.Vertex(vertices[i]);
        }
        GL.End();
    }
}
