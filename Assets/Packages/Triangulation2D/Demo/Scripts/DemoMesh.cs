// using UnityEngine;
// using Random = UnityEngine.Random;

// using System.Collections;
// using System.Collections.Generic;

// namespace mattatz.Triangulation2DSystem.Example {

// 	[RequireComponent (typeof(MeshFilter))]
// 	[RequireComponent (typeof(Rigidbody))]
// 	public class DemoMesh : MonoBehaviour {

// 		[SerializeField] Material lineMat;

// 		Triangle2D[] triangles;

// 		void Start () {
// 			//var body = GetComponent<Rigidbody>();
// 			//body.AddForce(Vector3.forward * Random.Range(150f, 160f));
// 			//body.AddTorque(Random.insideUnitSphere * Random.Range(10f, 20f));
// 		}

// 		void Update () {}

// 		public void SetTriangulation (Triangulation2D triangulation) {
// 			var mesh = triangulation.Build();
// 			GetComponent<MeshFilter>().sharedMesh = mesh;
// 			this.triangles = triangulation.Triangles;
// 		}

// 		void OnRenderObject () {
// 			if(triangles == null) return;

// 			GL.PushMatrix();
// 			GL.MultMatrix (transform.localToWorldMatrix);

// 			lineMat.SetColor("_Color", Color.red);
// 			lineMat.SetPass(0);
// 			GL.Begin(GL.LINES);
// 			for(int i = 0, n = triangles.Length; i < n; i++) {
// 				var t = triangles[i];
// 				GL.Vertex(t.s0.a.Coordinate); GL.Vertex(t.s0.b.Coordinate);
// 				GL.Vertex(t.s1.a.Coordinate); GL.Vertex(t.s1.b.Coordinate);
// 				GL.Vertex(t.s2.a.Coordinate); GL.Vertex(t.s2.b.Coordinate);
// 			}
// 			GL.End();
// 			GL.PopMatrix();
// 		}

// 	}

// }


using UnityEngine;
using Random = UnityEngine.Random;

using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace mattatz.Triangulation2DSystem.Example {

	[RequireComponent (typeof(MeshFilter))]
	[RequireComponent (typeof(Rigidbody))]
	public class DemoMesh : MonoBehaviour {

		[SerializeField] Material lineMat;

		Triangle2D[] triangles;

		Segment2D skeletonLine;

		Mesh mesh;

		void Start () {	
			skeletonLine = new Segment2D(new Vertex2D(new Vector2(-5,3.75f)),new Vertex2D(new Vector2(2,2)));
		}

		void Update () {}

		public void SetTriangulation (Triangulation2D triangulation) {
			mesh = triangulation.Build();
			GetComponent<MeshFilter>().sharedMesh = mesh;
			this.triangles = triangulation.Triangles;
		}

		void OnRenderObject () {
			if(triangles == null) return;

			GL.PushMatrix();
				GL.MultMatrix (transform.localToWorldMatrix);

				lineMat.SetColor("_Color", Color.red);
				lineMat.SetPass(0);
				
				int[] trianglesMomo = mesh.triangles;

				for (int i = 0; i < trianglesMomo.Length; i += 3)
				{
					if (
						Utils2D.Intersect(
								skeletonLine.a.Coordinate,
								skeletonLine.b.Coordinate,
								mesh.vertices[trianglesMomo[i + 0]],
								mesh.vertices[trianglesMomo[i + 1]]
							)
						||
						Utils2D.Intersect(
							skeletonLine.a.Coordinate,
							skeletonLine.b.Coordinate,
							mesh.vertices[trianglesMomo[i + 1]],
							mesh.vertices[trianglesMomo[i + 2]]
							)
						||
						Utils2D.Intersect(
							skeletonLine.a.Coordinate,
							skeletonLine.b.Coordinate,
							mesh.vertices[trianglesMomo[i + 0]],
							mesh.vertices[trianglesMomo[i + 2]]
							)
						){
							GL.Begin(GL.TRIANGLES);
							GL.Color(Color.green);
							GL.Vertex(mesh.vertices[trianglesMomo[i + 0]]);
							GL.Vertex(mesh.vertices[trianglesMomo[i + 1]]);
							GL.Vertex(mesh.vertices[trianglesMomo[i + 2]]);
							GL.End();
					}else{
							GL.Begin(GL.TRIANGLES);
							GL.Color(Color.cyan);
							GL.Vertex(mesh.vertices[trianglesMomo[i + 0]]);
							GL.Vertex(mesh.vertices[trianglesMomo[i + 1]]);
							GL.Vertex(mesh.vertices[trianglesMomo[i + 2]]);
							GL.End();
					}
				}
				GL.Begin(GL.LINES);
					GL.Color(Color.green);
					for(int i = 0, n = triangles.Length; i < n; i++) {
						var t = triangles[i];
						GL.Vertex(t.s0.a.Coordinate); GL.Vertex(t.s0.b.Coordinate);
						GL.Vertex(t.s1.a.Coordinate); GL.Vertex(t.s1.b.Coordinate);
						GL.Vertex(t.s2.a.Coordinate); GL.Vertex(t.s2.b.Coordinate);
					}
				GL.End();
				GL.Begin(GL.LINES);
					GL.Color(Color.blue);
					GL.Vertex(skeletonLine.a.Coordinate); GL.Vertex(skeletonLine.b.Coordinate);
				GL.End();
			GL.PopMatrix();
		}

	}
		
}
