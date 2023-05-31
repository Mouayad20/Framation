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
		
		public Mesh mesh;
		
		void Update () {}

		public void SetTriangulation (Triangulation2D triangulation) {
			mesh = triangulation.Build();
			GetComponent<MeshFilter>().sharedMesh = mesh;
			this.triangles = triangulation.Triangles;
		}

		void OnRenderObject () {
			if(triangles == null) return;
			
			if (Drawable.DrawTriangulation){
				GL.PushMatrix();
				lineMat.SetColor("_Color", Color.red);
				lineMat.SetPass(0);
				for (int t = 0; t < Drawable.link.triangles.Count; t++) {
					// for (int o = 0; o < Drawable.output.Count; o++) {
					// 	for (int i = 0; i < Drawable.output[o].triangles.Count; i++) {
					GL.Begin(GL.TRIANGLES);
						if (Drawable.link.triangles[t].linesId.Count > 1 )
							GL.Color(Color.black);
						else
							GL.Color(Color.red);

						GL.Vertex(Drawable.link.triangles[t].a);
						GL.Vertex(Drawable.link.triangles[t].b);
						GL.Vertex(Drawable.link.triangles[t].c);
					GL.End();

					GL.Begin(GL.LINES);
					GL.Color(Color.yellow);
						GL.Vertex(Drawable.link.triangles[t].a);
						GL.Vertex(Drawable.link.triangles[t].b);
						GL.Vertex(Drawable.link.triangles[t].b);
						GL.Vertex(Drawable.link.triangles[t].c);
						GL.Vertex(Drawable.link.triangles[t].c);
						GL.Vertex(Drawable.link.triangles[t].a);
					GL.End();
					// 	}
					// }
					// GL.Begin(GL.LINES);
					// GL.Color(Color.magenta);
					// 	GL.Vertex(Drawable.link.triangles[t].a);
					// 	GL.Vertex(Drawable.link.triangles[t].b);
					// 	GL.Vertex(Drawable.link.triangles[t].b);
					// 	GL.Vertex(Drawable.link.triangles[t].c);
					// 	GL.Vertex(Drawable.link.triangles[t].c);
					// 	GL.Vertex(Drawable.link.triangles[t].a);
					// GL.End();
				}
				GL.PopMatrix();	
			}	
		}
	}
}