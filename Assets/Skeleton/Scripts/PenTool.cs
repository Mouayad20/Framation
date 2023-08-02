using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Framation;
using OperationNamespace;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

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

    public static PenTool penTool;

    public  Vector3[] textureVertices;
    public  List<UpdateAll> newVertices;
    public  List<Skeleton> skeletons;
    private Skeleton basicSkeleton;
    private Skeleton copySkeleton;
    private DotController prevDot ;
    private DotController maxDot ;
    private DotController dot ;
    private Color color;
    private float k = 0.0f ; 
    private float prevX ;
    private float prevY ;
    private float dX ;
    private float dY ;
    private bool  selectDot = false;
    private bool  moveSkeleton2 ;
    private bool  move ;
    private int   lineCounter ;
    private int   counter;
    private int   dotId;

    private void Start(){
        penCanvas.OnPenCanvasLeftClickEvent += AddDot;
        skeletons     = new List <Skeleton>();
        maxDot        = new DotController();
        basicSkeleton = new Skeleton();
        counter       = 0 ;
        dotId         = 0 ;
        lineCounter   = 0 ;
        dX = 0 ; 
        dY = 0 ; 
        moveSkeleton2 = false;
        move    = false;
        penTool = this;
    }

    private void Update(){
        if(selectDot){
            if(Input.GetKeyDown(KeyCode.D)){
                RemoveDot(dot);
                selectDot = false;
            }
        } 
        if(move){
            if(k >= 1.0f){
                k = 0.0f;
            }
            k = k + 0.0001f ;

            Skeleton skeleton1 = skeletons[0]; 
            Skeleton skeleton2 = skeletons[1]; 
            
            float distance = Vector3.Distance(skeleton1.lines[0].start.transform.position, skeleton2.lines[0].start.transform.position);
            
            if (!((distance >= 0.00) && (distance <= 0.01))){

                Dictionary<UpdateAll, List<LineController>> pointLines = new Dictionary<UpdateAll, List<LineController>>();
                
                for(int i = 0 ; i< skeleton1.lines.Count ; i++) {
                    
                    if ( i == 0 ){
                        skeleton1.lines[i].start.transform.position = Vector3.Lerp(skeleton1.lines[i].start.transform.position, skeleton2.lines[i].start.transform.position, k);
                        skeleton1.lines[i].end.transform.position   = Vector3.Lerp(skeleton1.lines[i].end.transform.position  , skeleton2.lines[i].end.transform.position  , k);                    
                    }else{
                        skeleton1.lines[i].end.transform.position   = Vector3.Lerp(skeleton1.lines[i].end.transform.position  , skeleton2.lines[i].end.transform.position  , k);
                    }

                    HashSet<UpdateAll> uniqueUpdateAll = new HashSet<UpdateAll>();
                    foreach(Triangle triangle in Drawable.drawable.output[skeleton1.lines[i]]){
                        if(!uniqueUpdateAll.Contains(triangle.a))
                            uniqueUpdateAll.Add(triangle.a);
                        if(!uniqueUpdateAll.Contains(triangle.b))
                            uniqueUpdateAll.Add(triangle.b);
                        if(!uniqueUpdateAll.Contains(triangle.c))
                            uniqueUpdateAll.Add(triangle.c);
                    }

                    foreach(UpdateAll updateAll in uniqueUpdateAll){
                        if(!pointLines.ContainsKey(updateAll))
                            pointLines[updateAll] = new List<LineController>();
                        pointLines[updateAll].Add(skeleton1.lines[i]);
                    }
                
                }  
                foreach(KeyValuePair<UpdateAll, List<LineController>> kvp in pointLines){
                    UpdateAll result = new UpdateAll(99999999,new Vector3(0, 0, 0));

                    foreach(LineController line in kvp.Value){
                        Vector3 localCenter = new Vector3(
                            (line.start.transform.position.x + line.end.transform.position.x) / 2 ,
                            (line.start.transform.position.y + line.end.transform.position.y) / 2 ,
                            (line.start.transform.position.z + line.end.transform.position.z) / 2  
                        );

                        UpdateAll cur = new UpdateAll(kvp.Key.id,kvp.Key.vector);

                        cur.vector = cur.vector - localCenter ; 

                        cur.vector = Vector3.Scale(cur.vector , line.scale);
                        cur.vector = cur.vector + line.positionChange ;
                        cur.vector = Quaternion.Euler(0f, 0f,  line.rotationChange) * cur.vector ;
                        
                        cur.vector = cur.vector + localCenter ; 

                        result.vector += cur.vector;
                    }
                    result.vector /= kvp.Value.Count;

                    kvp.Key.vector = result.vector;
                    for (int i = 0; i < newVertices.Count; i++){
                        if (newVertices[i].id == kvp.Key.id) {
                            textureVertices[kvp.Key.id] = kvp.Key.vector;
                        }
                    }
                }

            }
        }
        if(moveSkeleton2){
            maxDot.onDragMoveEvent += MoveMaxDot;
        }
        if(Input.GetKeyDown(KeyCode.R)){
            print("RRRRRRRRRRRRRRRRRRRRRRRRRRRRRRR");
            moveSkeleton2 = !moveSkeleton2 ;
            if(moveSkeleton2 ==  false){
                maxDot.GetComponent<Image>().color = color;
                maxDot.onDragMoveEvent = null;
            }else {
                maxDot.GetComponent<Image>().color = Color.blue;
                prevX = maxDot.transform.position.x;
                prevY = maxDot.transform.position.y;
            }
        }
        if(Input.GetKeyDown(KeyCode.M)){
            print("MMMMMMMMMMMMMMMMMMMMMMMMMMMMMM");
            move = !move;           
            // for(int i = 0 ; i < copySkeleton.lines.Count ; i++){
            //     copySkeleton.lines[i].lr.enabled = false;
            //     copySkeleton.lines[i].start.GetComponent<Image>().enabled = false;
            //     copySkeleton.lines[i].end.GetComponent<Image>().enabled = false;
            // }
        }
        if(Input.GetKeyDown(KeyCode.F)){
            print("FFFFFFFFFFFFFFFFFFFFFFFFFFFFFF");
            skeletons.Add(copySkeleton);
        }
        if(Input.GetKeyDown(KeyCode.K)){
            print("KKKKKKKKKKKKKKKKKKKKKKKKKKKKK");
            copySkeleton = new Skeleton();
            Dictionary<int, DotController> dotsDictionary = new Dictionary<int, DotController>();
            foreach(LineController line in basicSkeleton.lines){
                if(!dotsDictionary.ContainsKey(line.start.id)){
                    DotController dot = Instantiate(dotPrefab , line.start.transform.position, Quaternion.identity, dotParent).GetComponent<DotController>();
                    dot.id = line.start.id;
                    dot.onDragEvent = line.start.onDragEvent;
                    dot.OnRightClickEvent  = line.start.OnRightClickEvent;
                    dot.OnLeftClickEvent   = line.start.OnLeftClickEvent;
                    dotsDictionary[dot.id] = dot;
                }
                if(!dotsDictionary.ContainsKey(line.end.id)){
                    DotController dot = Instantiate(dotPrefab , line.end.transform.position, Quaternion.identity, dotParent).GetComponent<DotController>();
                    dot.id = line.end.id;
                    dot.onDragEvent = line.end.onDragEvent;
                    dot.OnRightClickEvent  = line.end.OnRightClickEvent;
                    dot.OnLeftClickEvent   = line.end.OnLeftClickEvent;
                    dotsDictionary[dot.id] = dot;
                }
            }
            maxDot = Instantiate(dotPrefab , new Vector3(0,-99999999999,0), Quaternion.identity, dotParent).GetComponent<DotController>();
            foreach (LineController line in basicSkeleton.lines){
                if ( ( line.start.transform.position.y >  maxDot.transform.position.y) || ( line.end.transform.position.y > maxDot.transform.position.y ) ) {
                    if(line.start.transform.position.y >= line.end.transform.position.y){
                        maxDot = dotsDictionary[line.start.id];
                    }else{
                        maxDot = dotsDictionary[line.end.id];
                    }
                }
                line.lr.enabled = false;
                line.start.GetComponent<Image>().enabled = false;
                line.end.GetComponent<Image>().enabled = false;

                LineController cloneLine = line.Clone(); 
                cloneLine = Instantiate (lineprefab , Vector3.zero , Quaternion.identity , lineParent).GetComponent<LineController>(); ; 
                cloneLine.id = line.id  ;  

                cloneLine.start    = dotsDictionary[line.start.id];
                cloneLine.start.id = dotsDictionary[line.start.id].id;
                cloneLine.start.onDragEvent = dotsDictionary[line.start.id].onDragEvent;
                cloneLine.start.OnRightClickEvent = dotsDictionary[line.start.id].OnRightClickEvent;
                cloneLine.start.OnLeftClickEvent  = dotsDictionary[line.start.id].OnLeftClickEvent;

                cloneLine.end    = dotsDictionary[line.end.id];
                cloneLine.end.id = dotsDictionary[line.end.id].id;
                cloneLine.end.onDragEvent =  dotsDictionary[line.end.id].onDragEvent;
                cloneLine.end.OnRightClickEvent =  dotsDictionary[line.end.id].OnRightClickEvent;
                cloneLine.end.OnLeftClickEvent  =  dotsDictionary[line.end.id].OnLeftClickEvent;

                cloneLine.SetStart(cloneLine.start , cloneLine.start.id) ;
                cloneLine.SetEnd(  cloneLine.end   , cloneLine.end.id  ) ;
                
                copySkeleton.lines.Add(cloneLine);   
            }

            skeletons.Add(basicSkeleton);

            color = maxDot.GetComponent<Image>().color;
            maxDot.GetComponent<Image>().color = Color.blue;
            prevX = maxDot.transform.position.x;
            prevY = maxDot.transform.position.y;

            counter = 0 ;
            dotId = 0 ;
            lineCounter = 0 ;
            prevDot = null;
            dot = null;
        }
    }

    private void AddDot() {
        if (!Drawable.drawable.isDrawing){
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

                LineController line =  Instantiate(lineprefab , Vector3.zero , Quaternion.identity , lineParent).GetComponent<LineController>(); 
                line.id = lineCounter  ;  
                line.SetStart(prevDot,prevDot.id) ;
                dotId = dotId + 1 ;
                line.SetEnd(newDot , dotId) ;

                basicSkeleton.lines.Add(line);

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

                LineController line  = Instantiate(lineprefab , Vector3.zero , Quaternion.identity , lineParent).GetComponent<LineController>(); 
                line.id = lineCounter   ;
                line.SetStart(prevDot,dotId);
                dotId = dotId + 1 ;
                line.SetEnd(newDot , dotId) ;

                basicSkeleton.lines.Add(line);

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

    private void MoveMaxDot(DotController dot) {
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

        dX    = dot.transform.position.x - prevX;
        dY    = dot.transform.position.y - prevY;
        prevX = dot.transform.position.x; 
        prevY = dot.transform.position.y;

        for(int i = 0 ; i < copySkeleton.lines.Count ; i++){
            if ( i == 0 ){
                copySkeleton.lines[i].start.transform.position = new Vector3(copySkeleton.lines[i].start.transform.position.x+ dX , copySkeleton.lines[i].start.transform.position.y + dY , 0 ); 
                copySkeleton.lines[i].end.transform.position   = new Vector3(copySkeleton.lines[i].end.transform.position.x  + dX , copySkeleton.lines[i].end.transform.position.y   + dY , 0 );                   
            }else{
                copySkeleton.lines[i].end.transform.position   = new Vector3(copySkeleton.lines[i].end.transform.position.x  + dX , copySkeleton.lines[i].end.transform.position.y   + dY , 0 );                   
            }
        }
    }

    private void SelectDot(DotController selectedDot) {
        prevDot   = selectedDot;
        dot       = selectedDot;
        selectDot = true;
    }

    private void UnSelectDot(DotController selectedDot) {
        dot  = null;
        selectDot = false;
    }

    private void RemoveDot(DotController dotRemove){
        LineController line = CheckIfLeaf(dotRemove); 
        if ( line != null ){    
            print("it is a leaf");
            Destroy(line.gameObject);
            Destroy(dotRemove.gameObject);
            for(int i = 0; i < basicSkeleton.lines.Count; i++) {
                if (basicSkeleton.lines[i].id == line.id ) {
                    prevDot = null;
                    basicSkeleton.lines.RemoveAt(i);
                }
            }
        } else {
            print("it is not a leaf");
        }
    }

    private LineController CheckIfLeaf(DotController  dot){
        int freq = 0 ; 
        LineController line = null ;
        foreach(LineController line2 in  basicSkeleton.lines) {
            if(line2.start.id == dot.id || line2.end.id == dot.id){
                line  = line2;
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

/*
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.S))
            {
                string screenshotFilename = "screenshot.png";
                StartCoroutine(TakeScreenshot(screenshotFilename));
            }
        }

        private System.Collections.IEnumerator TakeScreenshot(string filename)
        {
            yield return new WaitForEndOfFrame();

            Texture2D screenshotTexture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
            screenshotTexture.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
            screenshotTexture.Apply();

            byte[] screenshotBytes = screenshotTexture.EncodeToPNG();
            System.IO.File.WriteAllBytes(filename, screenshotBytes);

            Debug.Log("Screenshot captured and saved as " + filename);
        }
*/