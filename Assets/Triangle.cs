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

    public Triangle(){
        linesId = new List<int>();
    }

}