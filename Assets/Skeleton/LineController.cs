using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineController : MonoBehaviour
{
    public int id;
    public LineRenderer lr;
    public  DotController start;
    public  DotController end;
    public float prevX;
    public float prevY;
    public float angle;
    public Transform vector1;
    public Vector2 position;
    public float rotation;
    public Vector2 scale;

    private void Awake(){
        lr = GetComponent <LineRenderer>();
        lr.positionCount = 0;
        id = 0 ;
        prevX = 0 ;
        prevY = 0 ;
        angle = 0 ;
    }

    private void Start(){
        vector1 = transform;
    }

    public void SetStart(DotController dot, int dotId){
        start = dot;
        start.id = dotId;
        lr.positionCount++;
    }

    public void SetEnd(DotController dot, int dotId){
        end = dot;
        end.id = dotId;
        lr.positionCount++;
    }
 
    private void LateUpdate(){
        if (start != null && end != null){
            // print("start pos  : " + start.transform.position );
            // print("end   pos  : " + end.transform.position );
            lr.SetPosition( 0 , start.transform.position);
            lr.SetPosition( 1 , end.transform.position  );
            CalculateLineChanges();
            // print("position   : " + lr.transform.position );
            // print("localScale : " + lr.transform.localScale );
            // print("rotation   : " + lr.transform.rotation );
            // print("translate  : " + lr.transform.TransformPoint(Vector3.zero) );
        }
    }

    private void CalculateLineChanges() {
        // Calculate position
        position = (start.transform.position + end.transform.position) / 2f;

        // Calculate rotation
        Vector2 direction = end.transform.position - start.transform.position;
        rotation = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Calculate scale
        float distance = Vector2.Distance(start.transform.position, end.transform.position);
        scale = new Vector2(distance, 1f);

        // Apply changes to the line
        lr.transform.position   = position;
        lr.transform.rotation   = Quaternion.Euler(0f, 0f, rotation);
        lr.transform.localScale = scale;
    }




}
