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
    Vector2 prevVector;
    float dX ;
    float dY ;

    int index ;

    float k = 0.0f ; 
    bool move ;
    bool soso ;

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
            float distance = Vector3.Distance(sk1[0].start.transform.position, sk2[0].start.transform.position);
            
            for(int i = 0 ; i< sk1.Count ; i++) {
                if (!((distance >= 0.00) && (distance <= 0.01))){
                    center = new Vector3(
                        (sk1[i].start.transform.position.x + sk1[i].end.transform.position.x) / 2 ,
                        (sk1[i].start.transform.position.y + sk1[i].end.transform.position.y) / 2 ,
                        (sk1[i].start.transform.position.z + sk1[i].end.transform.position.z) / 2  
                    );
                    Vector2 line1Direction  = sk1[i].end.transform.position - sk1[i].start.transform.position;
                    if ( i == 0 ){
                        sk1[i].start.transform.position = Vector3.Lerp(sk1[i].start.transform.position, sk2[i].start.transform.position, k);
                        sk1[i].end.transform.position   = Vector3.Lerp(sk1[i].end.transform.position  , sk2[i].end.transform.position  , k);                    
                    }else{
                        sk1[i].end.transform.position   = Vector3.Lerp(sk1[i].end.transform.position  , sk2[i].end.transform.position  , k);
                    } 
                    Vector2 line2Direction  = sk1[i].end.transform.position - sk1[i].start.transform.position;
                    sk1[i].angle = Vector3.Angle(line1Direction, line2Direction);


                    if (sk1[i].prevX != 0 && sk1[i].prevY != 0){
                        Vector3    linePosition    = sk1[i].transform.position;
                        Quaternion lineRotation = sk1[i].transform.rotation;
                        Vector3    lineScale       = sk1[i].transform.localScale;

                        
                            foreach(Triangle triangle in Drawable.output[sk1[i]]){
                                // if (triangle.linesId.Count == 1){ 
                                    // print("__________________");
                                    // print("sk1[i].scale                >>>>>>>  " +  sk1[i].scale.normalized );
                                    print("sk1[i].scale      >>>>>>>  " +  sk1[i].scale );
                                    // print("scallionChange    >>>>>>>  " +  sk1[i].scallionChange );
                                    // print("sk1[i].transform.localScale >>>>>>>  " +  sk1[i].transform.localScale );
                                    // print("sk1[i].normalized.localScale >>>>>>>  " +  sk1[i].transform.localScale.normalized  );
                                    // print(">>>>>Euler>>  " + Quaternion.Euler(0f, 0f,  sk1[i].angle) * triangle.a );

                                    


                                    
                                    // Matrix4x4 matrix = Matrix4x4.TRS(
                                    //     Vector3.zero,
                                    //     Quaternion.Euler(0f, 0f, 0f),
                                    //     sk1[i].scale
                                    // );
                                    // triangle.a = matrix.MultiplyPoint3x4(triangle.a);
                                    // triangle.b = matrix.MultiplyPoint3x4(triangle.b);
                                    // triangle.c = matrix.MultiplyPoint3x4(triangle.c);

                                    triangle.a = triangle.a - center ; 
                                    triangle.b = triangle.b - center ; 
                                    triangle.c = triangle.c - center ;

                                    triangle.a = new Vector3(
                                        triangle.a.x * sk1[i].scale.x,
                                        triangle.a.y * sk1[i].scale.y,
                                        0
                                    );
                                    triangle.b = new Vector3(
                                        triangle.b.x * sk1[i].scale.x,
                                        triangle.b.y * sk1[i].scale.y,
                                        0
                                    );
                                    triangle.c = new Vector3(
                                        triangle.c.x * sk1[i].scale.x,
                                        triangle.c.y * sk1[i].scale.y,
                                        0
                                    );

                                    triangle.a = triangle.a + sk1[i].positionChange ;
                                    triangle.b = triangle.b + sk1[i].positionChange ;
                                    triangle.c = triangle.c + sk1[i].positionChange ;
                                    
                                    // triangle.a = Quaternion.Euler(0f, 0f,  sk1[i].rotationChange) * triangle.a ;
                                    // triangle.b = Quaternion.Euler(0f, 0f,  sk1[i].rotationChange) * triangle.b ;
                                    // triangle.c = Quaternion.Euler(0f, 0f,  sk1[i].rotationChange) * triangle.c ;
        
                                    triangle.a = triangle.a + center ; 
                                    triangle.b = triangle.b + center ; 
                                    triangle.c = triangle.c + center ;




                                    // triangle.a = triangle.a + Vector3.Scale(center, sk1[i].scale) ;
                                    // triangle.b = triangle.b + Vector3.Scale(center, sk1[i].scale) ;
                                    // triangle.c = triangle.c + Vector3.Scale(center, sk1[i].scale) ;


                                    // print("before :  " + triangle.a) ;
                                    // triangle.a = new Vector3(
                                    //     triangle.a.x * 1.1f,
                                    //     triangle.a.y * 1.1f,
                                    //     0
                                    // );
                                    // print("after  :  " + triangle.a) ;
                                    // triangle.b = new Vector3(
                                    //     triangle.b.x * 1.1f,
                                    //     triangle.b.y * 1.1f,
                                    //     0
                                    // );
                                    // triangle.c = new Vector3(
                                    //     triangle.c.x * 1.1f,
                                    //     triangle.c.y * 1.1f,
                                    //     0
                                    // );
                                    
                                    // GameObject triangleObject = new GameObject("Triangle");
                                    // print("________________");
                                    // print("id         :  " + triangle.id);
                                    // print("position   :  " + triangle.triangleTransform.position);
                                    // print("rotation   :  " + triangle.triangleTransform.rotation);
                                    // print("localScale :  " + triangle.triangleTransform.localScale);
                                    // triangle.triangleTransform.position   = linePosition ;
                                    // triangle.triangleTransform.rotation   = lineRotation ;
                                    // triangle.triangleTransform.localScale = lineScale    ;

                                    // Vector3 transformedPosition = linePosition + (lineRotation * Vector3.Scale(triangle.triangleTransform.localPosition , lineScale));
                                    // Quaternion transformedRotation = lineRotation * triangle.triangleTransform.localRotation;
                                    // Vector3 transformedScale = Vector3.Scale(lineScale, triangle.triangleTransform.localScale);
                                    // triangle.triangleTransform.position = transformedPosition;
                                    // triangle.triangleTransform.rotation = transformedRotation;
                                    // triangle.triangleTransform.localScale = transformedScale;

                                    // triangle.a = triangle.a + sk1[i].transform.TransformPoint(Vector3.zero);
                                    // triangle.b = triangle.b + sk1[i].transform.TransformPoint(Vector3.zero);
                                    // triangle.c = triangle.c + sk1[i].transform.TransformPoint(Vector3.zero);

                                    // triangle.triangleTransform.SetParent(sk1[i].transform);

                                    // triangle.a   = sk1[i].transform.position   ;
                                    // triangle.b   = sk1[i].transform.position   ;
                                    // triangle.c   = sk1[i].transform.position   ;

                                    // triangle.triangleTransform.rotation   = sk1[i].transform.rotation   ;
                                    // triangle.triangleTransform.localScale = sk1[i].transform.localScale ;
                                    // print("before :  " + triangle.a);
                                    // Vector3 triangleLocalPosition = triangle.triangleTransform.TransformPoint(triangle.a);
                                    // Vector3 newTrianglePosition = linePosition + (lineRotation * Vector3.Scale(triangleLocalPosition , lineScale));
                                    // triangle.a = triangle.triangleTransform.InverseTransformPoint(newTrianglePosition);
                                    // print("after :  " + triangle.a);
                                    
                                    // Vector3 triangleLocalPosition = triangle.triangleTransform.localPosition;
                                    // Quaternion triangleLocalRotation = triangle.triangleTransform.localRotation;
                                    // Vector3 triangleLocalScale = triangle.triangleTransform.localScale;

                                    // Vector3 eulerRotation = lineRotation.eulerAngles;
                                    // Vector3 scaledRotation = Vector3.Scale(eulerRotation, triangleLocalPosition);
                                    // Vector3 scaledPosition = Vector3.Scale(scaledRotation, lineScale);

                                    // Vector3 newTrianglePosition = linePosition + scaledPosition;
                                    // Quaternion newTriangleRotation = lineRotation * triangleLocalRotation;
                                    // Vector3 newTriangleScale = Vector3.Scale(lineScale, triangleLocalScale);

                                    // triangle.triangleTransform.position = newTrianglePosition;
                                    // triangle.triangleTransform.rotation = newTriangleRotation;
                                    // triangle.triangleTransform.localScale = newTriangleScale;

                                    // Matrix4x4 transformationMatrix = Matrix4x4.identity;
                                    // Vector3 translation = new Vector3(0,06, 0f);
                                    // Quaternion rotation = Quaternion.Euler(0f, 0f,sk1[i].rotation );
                                    // Vector3 scale = new Vector3(1f, 1f, 0);
                                    // transformationMatrix.CalculateTransformedVector(linePosition,rotation , lineScale);
                                    // triangle.a = CalculateTransformedVector(triangle.a , linePosition , lineRotation , lineScale);
                                    // triangle.b = CalculateTransformedVector(triangle.b , linePosition , lineRotation , lineScale);
                                    // triangle.c = CalculateTransformedVector(triangle.c , linePosition , lineRotation , lineScale);
                                // }
                            }
                        
                    }
                    sk1[i].prevX = center.x;
                    sk1[i].prevY = center.y;
                }                  
            }  
        }
        // if(move){
        //     List<LineController> sk1 = skeletons[0]; 
        //     List<LineController> sk2 = skeletons[1]; 
        //     while(index != sk1.Count){
        //         // if(k >= 1.0f){
        //         //     k = 0.0f;
        //         //     move = false;
        //         //     print("ssssssssssssssssssssssssssssssssssssssssssssssss");
        //         // }
                
        //         print("index  :  " + index );   
        //         float distance = Vector3.Distance(sk1[0].start.transform.position, sk2[0].start.transform.position);
        //         print("Distance  : " + distance);
        //         while((distance >= 0.00) && (distance <= 0.01) ){
        //             foreach(Triangle triangle in Drawable.output[sk1[index]]){
        //             k = k + 0.0001f ;
        //             print("tid : " + triangle.id);
        //             if((distance >= 0.00) && (distance <= 0.01) ){
        //                 print("soossosososoossosoosossoosososososo");
        //                 move = false;
        //             }else{
        //                 center = new Vector3(
        //                     (sk1[index].start.transform.position.x + sk1[index].end.transform.position.x) / 2 ,
        //                     (sk1[index].start.transform.position.y + sk1[index].end.transform.position.y) / 2 ,
        //                     (sk1[index].start.transform.position.z + sk1[index].end.transform.position.z) / 2  
        //                 );
        //                 if (prevX != 0 && prevY != 0){
        //                     triangle.a = new Vector3(triangle.a.x + (center.x - prevX) , triangle.a.y + (center.y - prevY)  , 0);
        //                     triangle.b = new Vector3(triangle.b.x + (center.x - prevX) , triangle.b.y + (center.y - prevY)  , 0);
        //                     triangle.c = new Vector3(triangle.c.x + (center.x - prevX) , triangle.c.y + (center.y - prevY)  , 0);
        //                 }
        //                 prevX = center.x;
        //                 prevY = center.y;
        //                 if ( index == 0 ){
        //                     sk1[index].start.transform.position = Vector3.Lerp(sk1[index].start.transform.position, sk2[index].start.transform.position, k);
        //                     sk1[index].end.transform.position   = Vector3.Lerp(sk1[index].end.transform.position  , sk2[index].end.transform.position  , k);                    
        //                 }else{
        //                     sk1[index].end.transform.position   = Vector3.Lerp(sk1[index].end.transform.position  , sk2[index].end.transform.position  , k);
        //                 }  
        //             }
        //         }
                
        //         }
        //         index = index + 1 ;
        //     }
        //     // for(int i = 0 ; i< sk1.Count ; i++) {
        //     //     float distance = Vector3.Distance(sk1[0].start.transform.position, sk2[0].start.transform.position);
        //     //     print("i      : "+i);
        //     //     // if((distance >= 0.00) && (distance <= 0.01) ){
        //     //     //     print("soossosososoossosoosossoosososososo");
        //     //     //     move = false;
        //     //     // }else{
        //     //     //     // prevX = 0 ; 
        //     //     //     // prevY = 0 ;
        //     //     //     print("Distance  : " + distance);
        //     //     //     center = new Vector3(
        //     //     //         (sk1[i].start.transform.position.x + sk1[i].end.transform.position.x) / 2 ,
        //     //     //         (sk1[i].start.transform.position.y + sk1[i].end.transform.position.y) / 2 ,
        //     //     //         (sk1[i].start.transform.position.z + sk1[i].end.transform.position.z) / 2  
        //     //     //     );
        //     //     //     print("i      : "+i);
        //     //     //     print("prevX  : "+prevX);
        //     //     //     print("prevY  : "+prevY);
        //     //     //     if (prevX != 0 && prevY != 0){
        //     //     //         foreach(Triangle triangle in Drawable.output[sk1[i]]){
        //     //     //             // if (triangle.linesId.Count == 1){
        //     //     //                 triangle.a = new Vector3(triangle.a.x + (center.x - prevX) , triangle.a.y + (center.y - prevY)  , 0);
        //     //     //                 triangle.b = new Vector3(triangle.b.x + (center.x - prevX) , triangle.b.y + (center.y - prevY)  , 0);
        //     //     //                 triangle.c = new Vector3(triangle.c.x + (center.x - prevX) , triangle.c.y + (center.y - prevY)  , 0);
        //     //     //             // }
        //     //     //         }
        //     //     //     }
        //     //     //     prevX = center.x;
        //     //     //     prevY = center.y;
        //     //     //     if ( i == 0 ){
        //     //     //         sk1[i].start.transform.position = Vector3.Lerp(sk1[i].start.transform.position, sk2[i].start.transform.position, k);
        //     //     //         sk1[i].end.transform.position   = Vector3.Lerp(sk1[i].end.transform.position  , sk2[i].end.transform.position  , k);                    
        //     //     //     }else{
        //     //     //         sk1[i].end.transform.position   = Vector3.Lerp(sk1[i].end.transform.position  , sk2[i].end.transform.position  , k);
        //     //     //     }  
        //     //     // }
        //     // }  
        // }

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
    
    private Vector3 RotateVectorV11(Vector3 vector,float angle){
        // Convert the rotation axis and angle into a Quaternion
        // Quaternion rotation = Quaternion.AngleAxis(angle, new Vector3(0f, 0f, 1f).normalized);
        Quaternion rotation = Quaternion.Euler(0f, 0f, angle);

        // Rotate the vector using the Quaternion
        Vector3 rotatedVector = rotation * vector;

        return rotatedVector;
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

    public static Vector3 TransformVectorTransRota(Vector3 vector, Vector3 position, Vector3 rotation){
        // rotate vector around its local space origin
        vector = Quaternion.Euler(rotation) * vector;

        // then add a translation amount
        vector += position;

        return vector;
    }

    public static Vector3 CalculateTransformedVector(Vector3 originalVector, Vector3 translation, Vector3 rotation, Vector3 scale){
        // Apply translation
        Vector3 translatedVector = originalVector + translation;

        // Apply rotation
        Quaternion rotationQuaternion = Quaternion.Euler(rotation);
        Vector3 rotatedVector = rotationQuaternion * translatedVector;

        // Apply scale
        Vector3 scaledVector = new Vector3(
            rotatedVector.x * scale.x,
            rotatedVector.y * scale.y,
            rotatedVector.z * scale.z
        );

        return scaledVector;
    }

    public float CalculateAngle(Vector2 vectorA, Vector2 vectorB,Vector2 vectorC, Vector2 vectorD){
        float angleAB = CalculateAngle2V(vectorA, vectorB);
        float angleCD = CalculateAngle2V(vectorC, vectorD);
        float angleBetweenABandCD = Mathf.Abs(angleAB - angleCD);
        return angleBetweenABandCD;
    }

    public float CalculateAngle2V(Vector2 vector1, Vector2 vector2){
        float dotProduct = Vector2.Dot(vector1, vector2);
        float vectorLength1 = vector1.magnitude;
        float vectorLength2 = vector2.magnitude;

        float angle = Mathf.Acos(dotProduct / (vectorLength1 * vectorLength2));

        return angle;
    }

    public static Vector3 GeometricTransformation(Vector3 input, float size, Vector3 shift, float rotation){
        // Calculate the angle of rotation in radians
        float radians = rotation * Mathf.Deg2Rad;
        
        // Transform the vector by scaling, rotation, and shifting
        Vector3 output = Vector3.Scale(Vector3.Scale(input, new Vector3(size, size, 1)), new Vector3(Mathf.Cos(radians), Mathf.Sin(radians), 1));
        output = output + shift;
        
        // Return the result 
        return output;
    }

    public Vector3 RotateVector3(Vector3 originalVector, float degrees){
        //convert angle to radians
        var angle = degrees * Mathf.Deg2Rad;

        //calculate the sin and cos of the angle
        var sinAngle = Mathf.Sin(angle);
        var cosAngle = Mathf.Cos(angle);

        //calculate the x, y, and z of the rotated vector
        var x = originalVector.x * cosAngle - originalVector.y * sinAngle;
        var y = originalVector.x * sinAngle + originalVector.y * cosAngle;
        var z = originalVector.z;

        //create a new vector
        var rotatedVector = new Vector3(x, y, z);

        //return the rotated vector
        return rotatedVector;
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
