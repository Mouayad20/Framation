using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;
using FreeDraw;

public class Triangle  : MonoBehaviour {

    public int id ;
    public Vector3 a,b,c;
    public List<int> linesId;
    public Color color;
    public Transform triangleTransform;


    public Triangle(){
        linesId = new List<int>();
    }

    // void Start(){
    //     triangleTransform = transform;
    //     print("id  : " + id + " pos : " + triangleTransform.position);
    // }


    // void Update () {
    //     print("id  : " + id + " pos : " + triangleTransform.position);
    // }


}