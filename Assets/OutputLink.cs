using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Random = UnityEngine.Random;
using UnityEngine;
using FreeDraw;
  
public class OutputLink : MonoBehaviour {

    public LineController line;
    public List<Triangle> triangles ;
    public Color color;

    public OutputLink(){
        line = new LineController();
        triangles = new List<Triangle>();
        color = new Color(Random.value, Random.value, Random.value, 1.0f);
    }

}