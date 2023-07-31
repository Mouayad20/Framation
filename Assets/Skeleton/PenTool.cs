using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using FreeDraw;
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

    private List<LineController> lines;
    private List<LineController> linesTemp;
    List<LineController> copyLines;
    public static List<List<LineController>> skeletons;
    public static List<List<LineController>> skeletons2;
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
    bool moveSkeleton2 ;

    Vector3 avgTranslate;
    Vector3 avgScale;
    Vector3 sumTranslate;
    Vector3 sumScale;
    float avgRotate;
    float sumRotate;
    public static List<PointMimo> newVertices;
    public static Vector3[] verticesMimo;
    public DotController tempDot ;
    public DotController maxDot ;
    public Color color;

    private void Start(){
        penCanvas.OnPenCanvasLeftClickEvent += AddDot;
        tempDot = new DotController();
        maxDot = new DotController();
        lines       = new List <LineController>();
        linesTemp   = new List <LineController>();
        skeletons   = new List <List<LineController>>();
        skeletons2   = new List <List<LineController>>();
        counter     = 0 ;
        pupId       = 0 ;
        lineCounter = 0 ;
        dX = 0 ; 
        dY = 0 ; 
        avgTranslate = new Vector3(0,0,0) ;
        avgScale = new Vector3(0,0,0) ;
        sumScale = new Vector3(0,0,0) ;
        sumTranslate = new Vector3(0,0,0) ;
        sumRotate = 0 ;
        avgRotate = 0 ;
        move = false;
        moveSkeleton2 = false;
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
        if(move){
            if(k >= 1.0f){
                k = 0.0f;
            }
            k = k + 0.0001f ;
            // skeletons.Reverse();
            List<LineController> sk1 = skeletons[0]; 
            List<LineController> sk2 = skeletons[1]; 
            
            float distance = Vector3.Distance(sk1[0].start.transform.position, sk2[0].start.transform.position);
            
            if (!((distance >= 0.00) && (distance <= 0.01))){

                Dictionary<PointMimo, List<LineController>> pointLines = new Dictionary<PointMimo, List<LineController>>();
                
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
                    

                    HashSet<PointMimo> uniquePointMimo = new HashSet<PointMimo>();
                    foreach(Triangle triangle in Drawable.output[sk1[i]]){
                        if(!uniquePointMimo.Contains(triangle.a))
                            uniquePointMimo.Add(triangle.a);
                        if(!uniquePointMimo.Contains(triangle.b))
                            uniquePointMimo.Add(triangle.b);
                        if(!uniquePointMimo.Contains(triangle.c))
                            uniquePointMimo.Add(triangle.c);
                    }

                    foreach(PointMimo pointMimo in uniquePointMimo){
                        if(!pointLines.ContainsKey(pointMimo))
                            pointLines[pointMimo] = new List<LineController>();
                        pointLines[pointMimo].Add(sk1[i]);
                    }
                
                }  
                foreach(KeyValuePair<PointMimo, List<LineController>> kvp in pointLines){
                    PointMimo result = new PointMimo(new Vector3(0, 0, 0),99999999);

                    foreach(LineController line in kvp.Value){
                        Vector3 localCenter = new Vector3(
                            (line.start.transform.position.x + line.end.transform.position.x) / 2 ,
                            (line.start.transform.position.y + line.end.transform.position.y) / 2 ,
                            (line.start.transform.position.z + line.end.transform.position.z) / 2  
                        );

                        PointMimo cur = new PointMimo(kvp.Key.vector,kvp.Key.id);

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
                            verticesMimo[kvp.Key.id] = kvp.Key.vector;
                        }
                    }
                }

            }
        }
        if(moveSkeleton2){
            maxDot.onDragMoveEvent += MoveMaxDot;
        }
        if(Input.GetKeyDown(KeyCode.R)){
            print("RRRRRRRRRRRRRRRRRRR  : " + moveSkeleton2);
            moveSkeleton2 = !moveSkeleton2 ;
            if(moveSkeleton2 ==  false){
                Image dotSpriteRenderer = maxDot.GetComponent<Image>();
                dotSpriteRenderer.color = color;
                maxDot.onDragMoveEvent = null;
            }
        }
        if(Input.GetKeyDown(KeyCode.M)){
            move = !move;           
            for(int i = 0 ; i < copyLines.Count ; i++){
                copyLines[i].lr.enabled = false;
                copyLines[i].start.GetComponent<Image>().enabled = false;
                copyLines[i].end.GetComponent<Image>().enabled = false;
            }
        }
        if(Input.GetKeyDown(KeyCode.F)){
            print("FFFFFFFFFFFFFFFFFFFFFFFFFFFFFF");
            skeletons.Add(copyLines);
        }
        if(Input.GetKeyDown(KeyCode.K)){
            print("KKKKKKKKKKKKKKKKKKKKKKKKKKKKK");
            copyLines = new List<LineController>();
            Dictionary<int, DotController> dotsDictionary = new Dictionary<int, DotController>();
            foreach(LineController line in lines){
                if(!dotsDictionary.ContainsKey(line.start.id)){
                    DotController dodo = Instantiate(dotPrefab , line.start.transform.position, Quaternion.identity, dotParent).GetComponent<DotController>();
                    dodo.id = line.start.id;
                    dodo.onDragEvent = line.start.onDragEvent;
                    dodo.OnRightClickEvent = line.start.OnRightClickEvent;
                    dodo.OnLeftClickEvent =  line.start.OnLeftClickEvent;
                    dotsDictionary[dodo.id]=dodo;
                }
                if(!dotsDictionary.ContainsKey(line.end.id)){
                    DotController dodo = Instantiate(dotPrefab , line.end.transform.position, Quaternion.identity, dotParent).GetComponent<DotController>();
                    dodo.id = line.end.id;
                    dodo.onDragEvent = line.end.onDragEvent;
                    dodo.OnRightClickEvent = line.end.OnRightClickEvent;
                    dodo.OnLeftClickEvent =  line.end.OnLeftClickEvent;
                    dotsDictionary[dodo.id]=dodo;
                }
            }
            maxDot = Instantiate(dotPrefab , new Vector3(0,-99999999999,0), Quaternion.identity, dotParent).GetComponent<DotController>();
            for (int i = 0; i < lines.Count; i++){
                if ( ( lines[i].start.transform.position.y > maxDot.transform.position.y) || ( lines[i].end.transform.position.y > maxDot.transform.position.y ) ) {
                    if(lines[i].start.transform.position.y>=lines[i].end.transform.position.y){
                        maxDot = dotsDictionary[lines[i].start.id];
                    }else{
                        maxDot = dotsDictionary[lines[i].end.id];
                    }
                }
                lines[i].lr.enabled = false;
                lines[i].start.GetComponent<Image>().enabled = false;
                lines[i].end.GetComponent<Image>().enabled = false;
                LineController l = lines[i].Clone(); 
                l = Instantiate (lineprefab , Vector3.zero , Quaternion.identity , lineParent).GetComponent<LineController>(); ; 
                l.id = lines[i].id  ;  

                l.start = dotsDictionary[lines[i].start.id];
                l.start.id = dotsDictionary[lines[i].start.id].id;
                l.start.onDragEvent = dotsDictionary[lines[i].start.id].onDragEvent;
                l.start.OnRightClickEvent = dotsDictionary[lines[i].start.id].OnRightClickEvent;
                l.start.OnLeftClickEvent =  dotsDictionary[lines[i].start.id].OnLeftClickEvent;

                l.end   = dotsDictionary[lines[i].end.id];
                l.end.id =  dotsDictionary[lines[i].end.id].id;
                l.end.onDragEvent =  dotsDictionary[lines[i].end.id].onDragEvent;
                l.end.OnRightClickEvent =  dotsDictionary[lines[i].end.id].OnRightClickEvent;
                l.end.OnLeftClickEvent =  dotsDictionary[lines[i].end.id].OnLeftClickEvent;
                l.SetStart(l.start , l.start.id) ;
                l.SetEnd(l.end , l.end.id) ;
                copyLines.Add(l);   
            }
            skeletons.Add(lines);

            Image dotSpriteRenderer = maxDot.GetComponent<Image>();
            color = dotSpriteRenderer.color;
            dotSpriteRenderer.color = Color.blue;
            if(soso){
                soso = false;
                prevX = maxDot.transform.position.x;
                prevY = maxDot.transform.position.y;
            }
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

    private void MoveMaxDot(DotController dot) {
        print("MoveMaxDot");
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
        for(int i = 0 ; i < copyLines.Count ; i++){
            if ( i == 0 ){
                copyLines[i].start.transform.position = new Vector3(copyLines[i].start.transform.position.x+ dX , copyLines[i].start.transform.position.y + dY , 0 ); 
                copyLines[i].end.transform.position   = new Vector3(copyLines[i].end.transform.position.x  + dX , copyLines[i].end.transform.position.y   + dY , 0 );                   
            }else{
                copyLines[i].end.transform.position   = new Vector3(copyLines[i].end.transform.position.x  + dX , copyLines[i].end.transform.position.y   + dY , 0 );                   
            }
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