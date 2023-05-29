using UnityEngine;
using Random = UnityEngine.Random;

using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using FreeDraw;

namespace mattatz.Triangulation2DSystem.Example {

	[RequireComponent (typeof(MeshFilter))]
	[RequireComponent (typeof(Rigidbody))]
	public class DemoMesh : MonoBehaviour {

		[SerializeField] Material lineMat;

		public Triangle2D[] triangles;

		Color[] colors = new Color[6];
		
		public Mesh mesh;
		
		void Update () {}

		public void SetTriangulation (Triangulation2D triangulation,Texture2D texture) {
			colors[0] = Color.red;
			colors[1] = Color.green;
			colors[2] = Color.cyan;
			colors[3] = Color.blue;
			colors[4] = Color.magenta;
			colors[5] = Color.yellow;
			mesh = triangulation.Build();
			MeshRenderer rend = gameObject.AddComponent<MeshRenderer>();
			rend.sharedMaterial = new Material(Shader.Find("Standard"));
			MeshFilter fil = gameObject.GetComponent<MeshFilter>();

			Vector2[] uv = new Vector2[mesh.vertices.Length];
			// print(">>>>>  " + mesh.vertices.Length);
			for (int i = 0; i < mesh.vertices.Length; i ++){
				uv[i] = new Vector2(mesh.vertices[i].x/10,mesh.vertices[i].y/7.5f);
				// print(uv[i]);
			}
			mesh.uv = uv;
			// print(">>>>>  " + mesh.uv.Length);
			fil.mesh = mesh; 
			fil.GetComponent<Renderer>().material.mainTexture = texture;
			this.triangles = triangulation.Triangles;
		}

		void OnRenderObject () {
			if(triangles == null) return;
			
			if (Drawable.DrawTriangulation){
				GL.PushMatrix();
				lineMat.SetColor("_Color", Color.red);
				lineMat.SetPass(0);
				for (int t = 0; t < Drawable.link.triangles.Count; t++){
					for (int o = 0; o < Drawable.output.Count; o++){
						for (int i = 0; i < Drawable.output[o].triangles.Count; i++){
								GL.Begin(GL.TRIANGLES);
								if (Drawable.output[o].triangles[i].linesId.Count > 1 )
									GL.Color(Color.black);
								else
									GL.Color(colors[Drawable.output[o].line.id]);

									GL.Vertex(Drawable.output[o].triangles[i].a);
									GL.Vertex(Drawable.output[o].triangles[i].b);
									GL.Vertex(Drawable.output[o].triangles[i].c);
								GL.End();

								GL.Begin(GL.LINES);
								GL.Color(Color.yellow);
									GL.Vertex(Drawable.output[o].triangles[i].a);
									GL.Vertex(Drawable.output[o].triangles[i].b);
									GL.Vertex(Drawable.output[o].triangles[i].b);
									GL.Vertex(Drawable.output[o].triangles[i].c);
									GL.Vertex(Drawable.output[o].triangles[i].c);
									GL.Vertex(Drawable.output[o].triangles[i].a);
								GL.End();
						}
					}
					GL.Begin(GL.LINES);
					GL.Color(Color.magenta);
						GL.Vertex(Drawable.link.triangles[t].a);
						GL.Vertex(Drawable.link.triangles[t].b);
						GL.Vertex(Drawable.link.triangles[t].b);
						GL.Vertex(Drawable.link.triangles[t].c);
						GL.Vertex(Drawable.link.triangles[t].c);
						GL.Vertex(Drawable.link.triangles[t].a);
					GL.End();
				}
				GL.PopMatrix();	
			}	
		}
	}
}