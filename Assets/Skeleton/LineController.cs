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

    private void CalculateLineChanges() {


        position = (start.transform.position + end.transform.position) / 2f;
        positionChange = position - previousPosition;

        Vector2 direction = end.transform.position - start.transform.position;
        rotation = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        rotationChange = rotation - previousRotation;

        float distance = Vector2.Distance(start.transform.position, end.transform.position);
        float currentDistanceX = start.transform.position.x - end.transform.position.x;
        float currentDistanceY = start.transform.position.y - end.transform.position.y;
        float scaleX = currentDistanceX / previousDistanceX;
        float scaleY = currentDistanceY / previousDistanceY;

        scale = new Vector3(scaleX,scaleY, 1f);
        scallionChange = new Vector3(scale.x / previousScallion.x,scale.y / previousScallion.y,1f);

        lr.transform.position   = position;
        lr.transform.rotation   = Quaternion.Euler(0f, 0f, rotation);
        lr.transform.localScale = scale;
        
        previousPosition = position;
        previousRotation = rotation;
        previousScallion = scale;
        previousDistanceX = currentDistanceX;
        previousDistanceY = currentDistanceY;
    }
}