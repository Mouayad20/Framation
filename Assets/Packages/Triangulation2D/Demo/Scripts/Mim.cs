using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace mattatz.Triangulation2DSystem.Ss {

    public class Mim : MonoBehaviour
    {

        [SerializeField] Material lineMat;
        Segment2D skeletonLine;
        Segment2D skeletonLine1;
        Camera cam;
        Vector3 v1 ;
        Vector3 v2 ;
        Vector3 v3 ;
        Vector3 mousePosition ;
        Vector2 mouseVector2 ;
        Vector3 q1 ;
        Vector3 q2 ;
        Vector3 q3 ;
        bool  mimo = false;
        Vector3 vNew1;
        Vector3 vNew2;
        Vector3 vNew3;
        List<Vector3> list;
        List<Vertex2D> listVector2;
        float prevX ;
        float prevY ;
        float dX ;
        float dY ;
        
		void Start () {
			cam = Camera.main;
            v1 = new Vector3(0,1.5f,0 );
            v2 = new Vector3(3.0f,0,0);
            v3 = new Vector3(6.0f,0,0 );
            dX = 0 ; 
            dY = 0 ; 
            mousePosition = new Vector3(2,2,0);
            mouseVector2 = new Vector2(2,2);
            prevX = mousePosition.x ; 
            prevY = mousePosition.y ; 
            q1 = new Vector3(-3.0f,0.5f,0);
            q2 = new Vector3(-1.0f,1.0f,0);
            q3 = new Vector3(2,2,0);
            skeletonLine  = new Segment2D(new Vertex2D(new Vector2(q1.x,q1.y)),new Vertex2D(new Vector2(q2.x,q2.y)));
            skeletonLine1 = new Segment2D(new Vertex2D(new Vector2(q2.x,q2.y)),new Vertex2D(new Vector2(q3.x,q3.y)));
            list = new List<Vector3>(); 
            listVector2 = new List<Vertex2D>(); 
            Vector3 m1 = new Vector3(-0.49f, -0.23f, 0);
            list.Add(m1);
            m1 = new Vector3(-2.46f, -0.74f, 0);
            list.Add(m1);
            m1 = new Vector3(-2.17f,1.45f,0);
            list.Add(m1);

            m1 = new Vector3(-2.46f, -0.74f, 0);
            list.Add(m1);
            m1 = new Vector3(-4.00f, 0.57f, 0);
            list.Add(m1);
            m1 = new Vector3(-2.17f, 1.45f,0);
            list.Add(m1);

            m1 = new Vector3(1.40f, 0.57f, 0);
            list.Add(m1);
            m1 = new Vector3(-0.49f, -0.23f, 0);
            list.Add(m1);
            m1 = new Vector3(-0.20f, 2.10f,0);
            list.Add(m1);

            m1 = new Vector3(-0.49f, -0.23f, 0);
            list.Add(m1);
            m1 = new Vector3(-2.17f, 1.45f, 0);
            list.Add(m1);
            m1 = new Vector3(-0.20f, 2.10f,0);
            list.Add(m1);

            m1 = new Vector3(2.35f, 2.47f, 0);
            // Vertex2D m2 = new Vertex2D(new Vector2(2.35f, 2.47f));
            list.Add(m1);
            // listVector2.Add(m2);
            m1 = new Vector3(1.40f, 0.57f, 0);
            // m2 = new Vertex2D(new Vector2(1.40f, 0.57f));
            list.Add(m1);
            // listVector2.Add(m2);
            m1 = new Vector3(1.08f, 2.29f,0);
            // m2 = new Vertex2D(new Vector2(1.08f, 2.29f));
            list.Add(m1);
            // listVector2.Add(m2);

            m1 = new Vector3(1.40f, 0.57f, 0);
            list.Add(m1);
            m1 = new Vector3(-0.20f, 2.10f, 0);
            list.Add(m1);
            m1 = new Vector3(1.08f, 2.29f, 0);
            list.Add(m1);
        }

        void Update(){

            if (Input.GetKeyDown(KeyCode.D)){
                print(Utils2D.Contains(mouseVector2,listVector2));
            }            
            if (Input.GetMouseButtonDown(0)){
                mimo = true;
            } else if (Input.GetMouseButtonUp(0)){
                mimo = false;
            }
            if (mimo){
                Vector3 screenPosDepth = Input.mousePosition;
                screenPosDepth.z = 5.0f; 
                mousePosition = Camera.main.ScreenToWorldPoint(screenPosDepth);
                dX = mousePosition.x - prevX;
                dY = mousePosition.y - prevY;
                prevX = mousePosition.x;
                prevY = mousePosition.y;
                mousePosition.z = 0 ;
                q3.x = mousePosition.x;
                q3.y = mousePosition.y;
                skeletonLine1 = new Segment2D(new Vertex2D(new Vector2(q2.x,q2.y)),new Vertex2D(new Vector2(q3.x,q3.y)));
            }
        }

         void OnRenderObject(){
            GL.MultMatrix (transform.localToWorldMatrix);
            for(int i = 0, n = list.Count; i < n; i += 3) {
                if ((Utils2D.Intersect(skeletonLine1.a.Coordinate,skeletonLine1.b.Coordinate,
                                       list[i + 0],list[i + 1])
                    ||
                     Utils2D.Intersect(skeletonLine1.a.Coordinate,skeletonLine1.b.Coordinate,
                                       list[i + 1],list[i + 2])
                    ||
                     Utils2D.Intersect(skeletonLine1.a.Coordinate,skeletonLine1.b.Coordinate,
                                       list[i + 0],list[i + 2])
                    ) &&(
                     Utils2D.Intersect(skeletonLine.a.Coordinate,skeletonLine.b.Coordinate,
                                       list[i + 0],list[i + 1])
                    ||
                     Utils2D.Intersect(skeletonLine.a.Coordinate,skeletonLine.b.Coordinate,
                                       list[i + 1],list[i + 2])
                    ||
                     Utils2D.Intersect(skeletonLine.a.Coordinate,skeletonLine.b.Coordinate,
                                       list[i + 0],list[i + 2])
                    )
                    ){
                        GL.Begin(GL.TRIANGLES);
                            GL.Color(Color.cyan);
                            GL.Vertex(list[i + 0]);
                            GL.Vertex(list[i + 1]);
                            GL.Vertex(list[i + 2]);
                        GL.End();
                }
                else if (
                    Utils2D.Intersect(skeletonLine.a.Coordinate,skeletonLine.b.Coordinate,
                                      list[i + 0],list[i + 1])
                    ||
                    Utils2D.Intersect(skeletonLine.a.Coordinate,skeletonLine.b.Coordinate,
                                      list[i + 1],list[i + 2])
                    ||
                    Utils2D.Intersect(skeletonLine.a.Coordinate,skeletonLine.b.Coordinate,
                                      list[i + 0],list[i + 2])
                    ){
                        GL.Begin(GL.TRIANGLES);
                            GL.Color(Color.red);
                            GL.Vertex(list[i + 0]);
                            GL.Vertex(list[i + 1]);
                            GL.Vertex(list[i + 2]);
                        GL.End();
                }
                else if (
                    Utils2D.Intersect(skeletonLine1.a.Coordinate,skeletonLine1.b.Coordinate,
                                      list[i + 0],list[i + 1])
                    ||
                    Utils2D.Intersect(skeletonLine1.a.Coordinate,skeletonLine1.b.Coordinate,
                                      list[i + 1],list[i + 2])
                    ||
                    Utils2D.Intersect(skeletonLine1.a.Coordinate,skeletonLine1.b.Coordinate,
                                      list[i + 0],list[i + 2])
                    ){
                        
                        // Vector3 c1 = list[i + 0];
                        // Vector3 c2 = list[i + 1];
                        // Vector3 c3 = list[i + 2];
                        // Vector3 t1 = new Vector3(list[i + 0].x + dX , list[i + 0].y + dY , list[i + 0].z);
                        // Vector3 t2 = new Vector3(list[i + 1].x + dX , list[i + 1].y + dY , list[i + 1].z);
                        // Vector3 t3 = new Vector3(list[i + 2].x + dX , list[i + 2].y + dY , list[i + 2].z);
                        // for(int j = 0, m = list.Count; j < m; j ++){
                        //     if (list[j] == c1){
                        //         list[j] = t1;
                        //     }
                        //     if (list[j] == c2){
                        //         list[j] = t2;
                        //     }
                        //     if (list[j] == c3){
                        //         list[j] = t3;
                        //     }
                        // }
                        GL.Begin(GL.TRIANGLES);
                        GL.Color(Color.blue);
                            GL.Vertex(list[i + 0]);
                            GL.Vertex(list[i + 1]);
                            GL.Vertex(list[i + 2]);
                        GL.End();
                }
                else{
                        GL.Begin(GL.TRIANGLES);
                            GL.Color(Color.magenta);
                            GL.Vertex(list[i + 0]);
                            GL.Vertex(list[i + 1]);
                            GL.Vertex(list[i + 2]);
                        GL.End();
                }
            }
                
            GL.PushMatrix();
                GL.Begin(GL.QUADS);
                    GL.Color(Color.yellow);
                    GL.Vertex3(q3.x - 0.05f , q3.y         , 0);
                    GL.Vertex3(q3.x         , q3.y + 0.05f , 0);
                    GL.Vertex3(q3.x + 0.05f , q3.y         , 0);
                    GL.Vertex3(q3.x         , q3.y - 0.05f , 0);
                GL.End();
            GL.PopMatrix(); 
            GL.PushMatrix();
                GL.Begin(GL.LINES);
                    GL.Color(Color.blue);
                    GL.Vertex(skeletonLine.a.Coordinate); GL.Vertex(skeletonLine.b.Coordinate);
                    GL.Color(Color.red);
                    GL.Vertex(skeletonLine1.a.Coordinate); GL.Vertex(skeletonLine1.b.Coordinate);
                GL.End(); 
            GL.PopMatrix(); 

            GL.Begin(GL.LINES);
                GL.Color(Color.black);
                for(int i = 0, n = list.Count; i < n; i += 3) {
                    GL.Vertex(list[i+0]); GL.Vertex(list[i+1]);
                    GL.Vertex(list[i+1]); GL.Vertex(list[i+2]);
                    GL.Vertex(list[i+0]); GL.Vertex(list[i+2]);
                }
            GL.End();
            
        }        
        
        public Vector3 Barycentric(Vector3 a, Vector3 b, Vector3 c, Vector3 p) {
            Vector3 v0 = b - a;
            Vector3 v1 = c - a;
            Vector3 v2 = p - a;
            float d00 = Vector3.Dot(v0, v0);
            float d01 = Vector3.Dot(v0, v1);
            float d11 = Vector3.Dot(v1, v1);
            float d20 = Vector3.Dot(v2, v0);
            float d21 = Vector3.Dot(v2, v1);
            float denom = d00 * d11 - d01 * d01;
            float v = (d11 * d20 - d01 * d21) / denom;
            float w = (d00 * d21 - d01 * d20) / denom;
            float u = 1.0f - v - w;
            return new Vector3(u, v, w);
        }
    }
}