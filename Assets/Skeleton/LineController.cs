using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineController : MonoBehaviour
{
    public int id;
    private LineRenderer lr;
    public DotController start;
    public DotController end;

    private void Awake(){
        lr = GetComponent <LineRenderer>();
        lr.positionCount = 0;
        id = 0 ;
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
        }
    }
}
