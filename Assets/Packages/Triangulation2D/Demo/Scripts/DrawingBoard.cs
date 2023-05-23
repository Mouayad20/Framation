using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawingBoard : MonoBehaviour
{

    List<Vector2> points;
    float depth = 1000f;
    bool dragging;
    Camera cam;


    void Start(){
        cam = Camera.main;
        depth = Mathf.Abs(cam.transform.position.z - transform.position.z);
        points = new List<Vector2>();

    }

    void Update(){
        
        if(Input.GetMouseButtonDown(0)) {
            dragging = true;
            // Clear();
        } else if(Input.GetMouseButtonUp(0)) {
            dragging = false;
            points.Add(new Vector2(999999999, 999999999));
            // Build();
        }

        if(dragging) {
            var screen = Input.mousePosition;
            screen.z = depth;
            var p = cam.ScreenToWorldPoint(screen);
            var p2D = new Vector2(p.x, p.y);
            if(points.Count <= 0 || Vector2.Distance(p2D, points[^1]) > 0.12) {
                points.Add(p2D);
            }
        }

    }

    void OnRenderObject () {
		if(points != null) {
			GL.PushMatrix();
			GL.MultMatrix (transform.localToWorldMatrix);
			GL.Begin(GL.LINES);
            GL.Color(Color.cyan);
			for(int i = 0, n = points.Count - 1; i < n; i++) {
				if(points[i] == new Vector2(999999999, 999999999) || points[i+1] == new Vector2(999999999, 999999999)){
					continue ;
				}else{
					GL.Vertex(points[i]);
					GL.Vertex(points[i + 1]);
				}
			}
			GL.End();
			GL.PopMatrix();
		}
	}

    void Clear () {
		points.Clear();
	}
}
