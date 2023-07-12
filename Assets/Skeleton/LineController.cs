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
    public Vector3 position;
    public float rotation;
    public Vector3 scale;
    public Vector3 previousPosition;
    public Vector3 positionChange;
    public float previousRotation;
    public float previousDistance;
    public float previousDistanceX;
    public float previousDistanceY;
    public float rotationChange;
    public Vector3 scallionChange;
    public Vector3 previousScallion;

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
        previousPosition = transform.position;
        previousRotation = 0;
        previousScallion = new Vector3(1f,1f,1f);
        previousDistanceX = end.transform.position.x - start.transform.position.x;
        previousDistanceY = end.transform.position.y - start.transform.position.y;
        previousDistance = 1;
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
            lr.SetPosition( 0 , start.transform.position);
            lr.SetPosition( 1 , end.transform.position  );
            CalculateLineChanges();
        }
    }

    public void CalculateLineChanges() {


        position = (start.transform.position + end.transform.position) / 2f;
        positionChange = position - previousPosition;

        Vector2 direction = end.transform.position - start.transform.position;
        rotation = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        rotationChange = rotation - previousRotation;

        float distance = Vector3.Distance(start.transform.position, end.transform.position);

        float currentDistanceX = end.transform.position.x - start.transform.position.x;
        float currentDistanceY = end.transform.position.y - start.transform.position.y;

     /*    
        if(currentDistanceX < 0 ) {
            currentDistanceX = -1 * currentDistanceX;
        }
        if(currentDistanceY < 0 ) {
            currentDistanceY = -1 * currentDistanceY;
        }
        float scaleX = currentDistanceX / previousDistanceX ;
        float scaleY = currentDistanceY / previousDistanceY ;
        if(scaleY < 0 ) {
            scaleY = -1 * scaleY ;
        }
        if(scaleX < 0 ) {
            scaleX = -1 * scaleX ;
        }

        scaleX = 1 ;
     */
        // if(currentDistanceX > previousDistanceX){
        //     scale = new Vector3(1f , distance/previousDistance , 1f);
        // }
        // if(currentDistanceY > previousDistanceY){
        //     scale = new Vector3(distance/previousDistance , 1f , 1f);
        // }
        scale = new Vector3(distance/previousDistance , distance/previousDistance , 1f);
        
        previousDistance = distance;
        
        previousPosition  = position;
        previousRotation  = rotation;
        previousScallion  = scale;
        previousDistanceX = currentDistanceX;
        previousDistanceY = currentDistanceY;
    }
}