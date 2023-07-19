using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FreeDraw;
using OperationNamespace;

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
    Vector2 prevVector;
    float dX ;
    float dY ;

    int index ;

    float k = 0.0f ; 
    bool move ;
    bool soso ;

    Vector3 avgTranslate;
    Vector3 avgScale;
    Vector3 sumTranslate;
    Vector3 sumScale;
    float avgRotate;
    float sumRotate;

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
        avgTranslate = new Vector3(0,0,0) ;
        avgScale = new Vector3(0,0,0) ;
        sumScale = new Vector3(0,0,0) ;
        sumTranslate = new Vector3(0,0,0) ;
        sumRotate = 0 ;
        avgRotate = 0 ;
        move = false;
        prevCenter = new Vector3(0,0,0);
        prevVector = new Vector2(0,0);
        index = 0 ;
        soso = true;
    }

    private void Update(){
        if(selectDot){
            if(Input.GetKeyDown(KeyCode.D)){
                RemoveDot(dot);
                selectDot = false;
            }
        } 
        if(Input.GetKeyDown(KeyCode.M)){
            move = !move;           
        }
        if(move){
            if(k >= 1.0f){
                k = 0.0f;
            }
            k = k + 0.0001f ;
            List<LineController> sk1 = skeletons[0]; 
            List<LineController> sk2 = skeletons[1]; 
            float distance = Vector3.Distance(sk1[0].start.transform.position, sk2[0].start.transform.position);
            
            if (!((distance >= 0.00) && (distance <= 0.01))){
                for(int i = 0 ; i< sk1.Count ; i++) {
                    center = new Vector3(
                        (sk1[i].start.transform.position.x + sk1[i].end.transform.position.x) / 2 ,
                        (sk1[i].start.transform.position.y + sk1[i].end.transform.position.y) / 2 ,
                        (sk1[i].start.transform.position.z + sk1[i].end.transform.position.z) / 2  
                    );

                    if ( i == 0 ){
                        sk1[i].start.transform.position = Vector3.Lerp(sk1[i].start.transform.position, sk2[i].start.transform.position, k);
                        sk1[i].end.transform.position   = Vector3.Lerp(sk1[i].end.transform.position  , sk2[i].end.transform.position  , k);                    
                    }else{
                        sk1[i].end.transform.position   = Vector3.Lerp(sk1[i].end.transform.position  , sk2[i].end.transform.position  , k);
                    } 

                    foreach(Triangle triangle in Drawable.output[sk1[i]]){
                        if(triangle.lines.Count  == 1){
                            triangle.Shift(center,Operation.Minus);
                            triangle.TransformationSTR(
                                sk1[i].scale,
                                sk1[i].positionChange,
                                sk1[i].rotationChange
                            );
                            triangle.Shift(center,Operation.Add);
                        }
                    }
                }                  
                foreach(Triangle triangle in Drawable.link.triangles){
                    if(triangle.lines.Count  > 1){
                        avgScale = new Vector3(0,0,0) ;
                        sumScale = new Vector3(0,0,0) ;
                        avgTranslate = new Vector3(0,0,0) ;
                        sumTranslate = new Vector3(0,0,0) ;
                        avgRotate = 0 ;
                        sumRotate = 0 ;
                        for(int l = 0 ; l < triangle.lines.Count ; l++) {
                            sumScale     += triangle.lines[l].scale;
                            sumTranslate += triangle.lines[l].positionChange;
                            sumRotate    += triangle.lines[l].rotationChange;
                        }

                        avgScale     = sumScale     / triangle.lines.Count;
                        avgTranslate = sumTranslate / triangle.lines.Count;
                        avgRotate    = sumRotate    / triangle.lines.Count;

                            triangle.Shift(center,Operation.Minus);
                            triangle.TransformationSTR( avgScale , avgTranslate  , avgRotate );
                            triangle.Shift(center,Operation.Add);
                    }                       
                }
            }  
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
        print("Selected position  : " +  selectedDot.transform.position);
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

}