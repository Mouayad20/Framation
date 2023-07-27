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
			// print("@@@@@@@@@@ : " + transform.position);
			mesh = triangulation.Build();
			GetComponent<MeshFilter>().sharedMesh = mesh;
			this.triangles = triangulation.Triangles;
		}

		void OnRenderObject () {
			if(triangles == null) return;
			
			if (Drawable.DrawTriangulation){
				
				Drawable.globalMesh.vertices = PenTool.verticesMimo;
				Drawable.globalMesh.uv = Drawable.uv.ToArray();
				Drawable.globalMesh.RecalculateNormals();
				Drawable.globalMesh.RecalculateBounds();
				Drawable.globalMesh.RecalculateTangents();


			// 	GL.PushMatrix();
			// 	lineMat.SetColor("_Color", Color.red);
			// 	lineMat.SetPass(0);
			// 	for (int t = 0; t < Drawable.link.triangles.Count; t++) {
			// 		// for (int o = 0; o < Drawable.output.Count; o++) {
			// 		// 	for (int i = 0; i < Drawable.output[o].triangles.Count; i++) {
			// 		GL.Begin(GL.TRIANGLES);
			// 			if (Drawable.link.triangles[t].lines.Count > 1 )
			// 				GL.Color(Color.black);
			// 			else
			// 				GL.Color(Drawable.link.triangles[t].color);

			// 			GL.Vertex(Drawable.link.triangles[t].a.vector);
			// 			GL.Vertex(Drawable.link.triangles[t].b.vector);
			// 			GL.Vertex(Drawable.link.triangles[t].c.vector);
			// 		GL.End();

			// 		GL.Begin(GL.LINES);
			// 		GL.Color(Color.yellow);
			// 			GL.Vertex(Drawable.link.triangles[t].a.vector);
			// 			GL.Vertex(Drawable.link.triangles[t].b.vector);
			// 			GL.Vertex(Drawable.link.triangles[t].b.vector);
			// 			GL.Vertex(Drawable.link.triangles[t].c.vector);
			// 			GL.Vertex(Drawable.link.triangles[t].c.vector);
			// 			GL.Vertex(Drawable.link.triangles[t].a.vector);
			// 		GL.End();
			// 		// 	}
			// 		// }
			// 		// GL.Begin(GL.LINES);
			// 		// GL.Color(Color.magenta);
			// 		// 	GL.Vertex(Drawable.link.triangles[t].a);
			// 		// 	GL.Vertex(Drawable.link.triangles[t].b);
			// 		// 	GL.Vertex(Drawable.link.triangles[t].b);
			// 		// 	GL.Vertex(Drawable.link.triangles[t].c);
			// 		// 	GL.Vertex(Drawable.link.triangles[t].c);
			// 		// 	GL.Vertex(Drawable.link.triangles[t].a);
			// 		// GL.End();
			// 	}
			// 	GL.PopMatrix();	
			}	
		}
	}
}