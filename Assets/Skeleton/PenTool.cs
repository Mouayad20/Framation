using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FreeDraw;

public class PenTool : MonoBehaviour
{
    [Header("Pen Canvas")]
    [SerializeField] private PenCanvas penCanvas;

    [Header("Dots")]
    [SerializeField] private GameObject dotPrefab;
    [SerializeField] Transform dotParent;
    
    [Header("Lines")]
    [SerializeField] GameObject lineprefab;
    [SerializeField] Transform  lineParent;

    private List<LineController> lines;
    private List<LineController> linesTemp;
    public static List<List<LineController>> skeletons;
    private LineController currentLine;
    DotController dot ;
    DotController prevDot ;
    bool selectDot = false;
    int  counter;
    int pupId;
    int lineCounter ;


    Vector3 prevCenter ;
    Vector3 center;
    float prevX ;
    float prevY ;
    float dX ;
    float dY ;

    float k = 0.0f ; 
    bool move ;

    private void Start(){
        penCanvas.OnPenCanvasLeftClickEvent += AddDot;
        lines     = new List <LineController>();
        linesTemp = new List <LineController>();
        skeletons = new List<List<LineController>>();
        counter     = 0 ;
        pupId       = 0 ;
        lineCounter = 0 ;
        dX = 0 ; 
        dY = 0 ; 
        prevX = 0 ; 
        prevY = 0 ; 
        move = false;
        prevCenter = new Vector3(0,0,0);
        
    }

    private void Update(){
        if(selectDot){
            if(Input.GetKeyDown(KeyCode.D)){
                RemoveDot(dot);
                selectDot = false;
            }
        } 
        if(Input.GetKeyDown(KeyCode.W)){
            // print("lines size : " + lines.Count);
            // print("temps size : " + linesTemp.Count);
            // print("newws size : " + linesNew.Count);
            // for(int i = 0 ; i< lines.Count ; i++) {
            //     print("line ( " + lines[i].id + " )  :  {  ( " +lines[i].start.id + " , " + +lines[i].end.id + " )  } ");  
            // }
        }
        if(Input.GetKeyDown(KeyCode.M)){
            move = true;           
        }
        if(move){
            if(k >= 1.0f){
                k = 0.0f;
            }
            k = k + 0.0001f ;
            List<LineController> sk1 = skeletons[0]; 
            List<LineController> sk2 = skeletons[1]; 
            for(int i = 0 ; i< sk1.Count ; i++) {
                center = new Vector3(
                    (sk1[i].start.transform.position.x + sk1[i].end.transform.position.x) / 2 ,
                    (sk1[i].start.transform.position.y + sk1[i].end.transform.position.y) / 2 ,
                    (sk1[i].start.transform.position.z + sk1[i].end.transform.position.z) / 2  
                );
                if (prevX != 0 && prevY != 0){
                    foreach(Triangle triangle in Drawable.output[sk1[i]]){
                        if (triangle.linesId.Count == 1){
                            triangle.a = new Vector3(triangle.a.x + (center.x - prevX) , triangle.a.y + (center.y - prevY)  , 0);
                            triangle.b = new Vector3(triangle.b.x + (center.x - prevX) , triangle.b.y + (center.y - prevY)  , 0);
                            triangle.c = new Vector3(triangle.c.x + (center.x - prevX) , triangle.c.y + (center.y - prevY)  , 0);
                        }
                    }
                }
                prevX = center.x;
                prevY = center.y;

                if ( i == 0 ){
                    sk1[i].start.transform.position = Vector3.Lerp(sk1[i].start.transform.position, sk2[i].start.transform.position, k);
                    sk1[i].end.transform.position   = Vector3.Lerp(sk1[i].end.transform.position  , sk2[i].end.transform.position  , k);                    
                }else{
                    sk1[i].end.transform.position   = Vector3.Lerp(sk1[i].end.transform.position  , sk2[i].end.transform.position  , k);
                }                   
            }  
        }

        if(Input.GetKeyDown(KeyCode.N)){
            move = false;
        }

        if(Input.GetKeyDown(KeyCode.K)){
            print("KKKKKKKKKKKKKKKKKKKKKKKKKKKKK");
            linesTemp = new List<LineController>(lines);
            skeletons.Add(linesTemp);
            lines.Clear();
            counter = 0 ;
            pupId = 0 ;
            lineCounter = 0 ;
            prevDot = null;
            dot = null;
        }
    }

    private void AddDot() {   
        if (!Drawable.isDrawing){
            if(counter == 0 ) {
                DotController dot = Instantiate(dotPrefab , GetMousePosition(), Quaternion.identity, dotParent).GetComponent<DotController>();
                dot.onDragEvent += MoveDot;
               dot.OnLeftClickEvent += SelectDot;
                dot.OnRightClickEvent += UnSelectDot;
                prevDot = dot;
                counter = counter + 1;
            } 
            else if (selectDot && prevDot != null ) {
                DotController newDot = Instantiate(dotPrefab , GetMousePosition(), Quaternion.identity, dotParent).GetComponent<DotController>();
                newDot.onDragEvent += MoveDot;
                newDot.OnLeftClickEvent += SelectDot;
                newDot.OnRightClickEvent += UnSelectDot;
                LineController line =  Instantiate (lineprefab , Vector3.zero , Quaternion.identity , lineParent).GetComponent<LineController>(); 
                line.id = lineCounter  ;  
                line.SetStart(prevDot,prevDot.id) ;
                pupId = pupId + 1 ;
                line.SetEnd(newDot , pupId) ;
                lines.Add(line);
                prevDot = newDot;
                counter = counter + 1;
                lineCounter = lineCounter + 1;
                selectDot = false;
            }
            else if ( counter > 0 && prevDot != null ) {
                DotController newDot = Instantiate(dotPrefab  , GetMousePosition(), Quaternion.identity, dotParent).GetComponent<DotController>();
                newDot.onDragEvent += MoveDot;
                newDot.OnLeftClickEvent += SelectDot;
                newDot.OnRightClickEvent += UnSelectDot;
                LineController line =  Instantiate(lineprefab , Vector3.zero , Quaternion.identity , lineParent).GetComponent<LineController>(); 
                line.id = lineCounter   ;
                line.SetStart(prevDot,pupId);
                pupId = pupId + 1 ;
                line.SetEnd(newDot , pupId) ;
                lines.Add(line);
                prevDot = newDot;
                counter = counter + 1;
                lineCounter = lineCounter + 1;
            } 
            else {
                print("please select dot before!! ");
            }
        }
    }

    private void MoveDot(DotController dot) {
        print("MoveDot");
        Vector3 mousePos = GetMousePosition();
        if (
            mousePos.x <=  0.1f  || 
            mousePos.x >=  9.9f  || 
            mousePos.y >= -0.1f  || 
            mousePos.y <= -7.4 
        ){ 
            dot.transform.position = dot.transform.position; 
        }else{
            dot.transform.position = mousePos; 
        }
        
    }

    private void SelectDot(DotController selectedDot) {
        print("Selected dot id  : " +  selectedDot.id);
        prevDot   = selectedDot;
        dot       = selectedDot;
        selectDot = true;
    }

    private void UnSelectDot(DotController selectedDot) {
        print("UnSelect");
        dot  = null;
        selectDot = false;
    }

    private void RemoveDot(DotController dotRemove){
        print("dotRemove id  : "+  dotRemove.id);
        LineController line = CheckIfLeaf(dotRemove); 
        if ( line != null ){    
            print("it is a leaf");
            Destroy(line.gameObject);
            Destroy(dotRemove.gameObject);
            for(int i = 0; i < lines.Count; i++) {
                if (lines[i].id == line.id ) {
                    prevDot = null;
                    lines.RemoveAt(i);
                }
            }
        } else {
            print("it is not a leaf");
        }
    }

    private LineController CheckIfLeaf(DotController  dot){
        int freq = 0 ; 
        LineController line = null ;
        for(int i = 0; i < lines.Count; i++) {
            if(lines[i].start.id == dot.id || lines[i].end.id == dot.id){
                line  = lines[i];
                freq++;
            }
        }
        if(freq == 1 ){
            return line;
        }
        else
            return null;
    }

    private Vector3 GetMousePosition(){
        Vector3 worldMousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        worldMousePosition.z = 0;
        return worldMousePosition;
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


    // Vector3 barycentric = Barycentric(triangle.a,triangle.b,triangle.c,center);
    // float b1New = Vector3.Cross(triangle.b - triangle.a, center - triangle.a).magnitude /
    //         Vector3.Cross(triangle.b - triangle.a, triangle.c - triangle.a).magnitude;
    // float b2New = Vector3.Cross(triangle.c - triangle.b, center - triangle.b).magnitude /
    //         Vector3.Cross(triangle.c - triangle.b, triangle.a - triangle.b).magnitude;
    // float b3New = Vector3.Cross(triangle.a - triangle.c, center - triangle.c).magnitude /
    //         Vector3.Cross(triangle.a - triangle.c, triangle.b - triangle.c).magnitude;
    // triangle.a  = triangle.a + (b1New * (center - triangle.a));
    // triangle.b  = triangle.b + (b2New * (center - triangle.b));
    // triangle.c  = triangle.c + (b3New * (center - triangle.c));