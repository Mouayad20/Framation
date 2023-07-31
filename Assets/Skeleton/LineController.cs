using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

[Serializable]
public class LineController : MonoBehaviour
{
    public int id;
    public LineRenderer lr;
    public DotController start;
    public DotController end;
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
        previousDistanceX = start.transform.position.x - end.transform.position.x;
        previousDistanceY = start.transform.position.y - end.transform.position.y;
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
        float currentDistanceX = start.transform.position.x - end.transform.position.x;
        float currentDistanceY = start.transform.position.y - end.transform.position.y;
        float scaleX = 1 ;
        float scaleY = 1 ;
        float distance = Vector3.Distance(start.transform.position, end.transform.position);

        if((Mathf.Abs(currentDistanceX) < 0.1)&&(Mathf.Abs(currentDistanceX) > 0)){
            scaleX = 1 ;
        }else{
            scaleY = distance / previousDistance ;
        }
        if((Mathf.Abs(currentDistanceY) < 0.1)&&(Mathf.Abs(currentDistanceY) > 0)){
            scaleY = 1 ;
        }else{
            scaleX = distance / previousDistance ;
        }
        scale = new Vector3(scaleX,scaleY, 1f);
        
        
        previousPosition  = position;
        previousRotation  = rotation;
        previousScallion  = scale;
        previousDistance  = distance;
        previousDistanceX  = currentDistanceX;
        previousDistanceY  = currentDistanceY;
    }

    public LineController Clone(){
        LineController line = new LineController();
        line.id = this.id ;
        line.prevX = this.prevX ;
        line.prevY = this.prevY ;
        line.angle = this.angle ;
        line.vector1 = this.vector1 ;
        line.position = this.position ;
        line.rotation = this.rotation ;
        line.scale = this.scale ;
        line.previousPosition = this.previousPosition ;
        line.positionChange = this.positionChange ;
        line.previousRotation = this.previousRotation ;
        line.previousDistance = this.previousDistance ;
        line.previousDistanceX = this.previousDistanceX;
        line.previousDistanceY = this.previousDistanceY;
        line.rotationChange = this.rotationChange ;
        line.scallionChange = this.scallionChange ;
        line.previousScallion = this.previousScallion ;
        return line;
    }

}