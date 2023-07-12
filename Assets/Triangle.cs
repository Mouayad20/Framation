using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;
using FreeDraw;
using OperationNamespace;

public class Triangle  : MonoBehaviour {

    public int id ;
    public Vector3 a,b,c;
    public List<int> linesId;
    public Color color;
    public Transform triangleTransform;

    public Triangle(){
        linesId = new List<int>();
    }

    public void Shift(Vector3 center,Operation operation){
        if(operation == Operation.Minus) {
            this.a = this.a - center ; 
            this.b = this.b - center ; 
            this.c = this.c - center ;
        }
        if(operation == Operation.Add) {
            this.a = this.a + center ; 
            this.b = this.b + center ; 
            this.c = this.c + center ;
        }
    }

    public void TransformationSTR( Vector3 scale , Vector3 translate , float rotation){
        this.a = Vector3.Scale(this.a , scale);
        this.b = Vector3.Scale(this.b , scale);
        this.c = Vector3.Scale(this.c , scale);

        this.a = this.a + translate ;
        this.b = this.b + translate ;
        this.c = this.c + translate ;
        
        this.a = Quaternion.Euler(0f, 0f,  rotation) * this.a ;
        this.b = Quaternion.Euler(0f, 0f,  rotation) * this.b ;
        this.c = Quaternion.Euler(0f, 0f,  rotation) * this.c ;
    }
    
}