using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using FreeDraw;
using mattatz.Triangulation2DSystem;
using mattatz.Triangulation2DSystem.Example;
using Random = UnityEngine.Random;

public class Link : MonoBehaviour {

    public List<Triangle> triangles ;
    public List<List<LineController>> skeletons;

    public Link(){
        triangles = Drawable.triangles;
        skeletons = PenTool.skeletons ;
    }

    public List<OutputLink> Linking(){
        List<OutputLink> output  = new List<OutputLink>();

        int size = triangles.Count;
        
        for (int i = 0; i < skeletons[0].Count; i ++){
            OutputLink o = new OutputLink();
            o.line  = skeletons[0][i];
            for (int j = 0; j < triangles.Count; j ++){
                if (
                        Utils2D.Intersect(
                                skeletons[0][i].start.transform.position,
                                skeletons[0][i].end.transform.position,
                                triangles[j].a,
                                triangles[j].b
                            )
                        ||
                        Utils2D.Intersect(
                            skeletons[0][i].start.transform.position,
                            skeletons[0][i].end.transform.position,
                            triangles[j].b,
                            triangles[j].c
                            )
                        ||
                        Utils2D.Intersect(
                            skeletons[0][i].start.transform.position,
                            skeletons[0][i].end.transform.position,
                            triangles[j].a,
                            triangles[j].c
                            )
                    ) {
                        Triangle tri = triangles[j];
                        o.triangles.Add(tri);
                        size = size - 1 ;
                    } 
                    else {
                        continue;
                    }
            }
            output.Add(o);
        }

        

        // for (int o = 0; o < output.Count; o++) {
        //     for(int k = 0; k < output[o].triangles.Count; k++) {
        //         for (int t = 0; t < triangles.Count; t ++) {
        //             if (
        //                 ( 
        //                     output[o].triangles[k].a == triangles[t].a || output[o].triangles[k].a == triangles[t].b || output[o].triangles[k].a == triangles[t].c ||
        //                     output[o].triangles[k].b == triangles[t].a || output[o].triangles[k].b == triangles[t].b || output[o].triangles[k].b == triangles[t].c ||
        //                     output[o].triangles[k].c == triangles[t].a || output[o].triangles[k].c == triangles[t].b || output[o].triangles[k].c == triangles[t].c   
        //                 ) 
        //                 && 
        //                 !(
        //                     output[o].triangles[k].id == triangles[t].id
        //                 )
        //             ){
        //                 o.triangles.Add(triangles[t]);
        //                 // triangles.RemoveAt(t);
        //                 size = size - 1 ;
        //             }else{
        //                 continue;
        //             }
        //         }
        //     }
        // }
                        
            // while ( triangles.Count > 0  ){
            // }
        return output; 
    }

}

/*
    int size = triangles.Count;

       
        
        for (int i = 0; i < skeletons[0].Count; i ++){
            OutputLink o = new OutputLink();
            o.line  = skeletons[0][i];
            for (int j = 0; j < triangles.Count; j ++){
                if (
                        Utils2D.Intersect(
                                skeletons[0][i].start.transform.position,
                                skeletons[0][i].end.transform.position,
                                triangles[j].a,
                                triangles[j].b
                            )
                        ||
                        Utils2D.Intersect(
                            skeletons[0][i].start.transform.position,
                            skeletons[0][i].end.transform.position,
                            triangles[j].b,
                            triangles[j].c
                            )
                        ||
                        Utils2D.Intersect(
                            skeletons[0][i].start.transform.position,
                            skeletons[0][i].end.transform.position,
                            triangles[j].a,
                            triangles[j].c
                            )
                    ) {
                        Triangle tri = triangles[j];
                        // triangles.RemoveAt(j);
                        o.triangles.Add(tri);
                        size = size - 1 ;
                        
                        
                    } 
                    else {
                        continue;
                    }
            }
            output.Add(o);
        }

        for (int o = 0; o < output.Count; o++) {
            for(int k = 0; k < output[o].triangles.Count; k++) {
                for (int t = 0; t < triangles.Count; t ++) {
                    if (
                        ( 
                            output[o].triangles[k].a == triangles[t].a || output[o].triangles[k].a == triangles[t].b || output[o].triangles[k].a == triangles[t].c ||
                            output[o].triangles[k].b == triangles[t].a || output[o].triangles[k].b == triangles[t].b || output[o].triangles[k].b == triangles[t].c ||
                            output[o].triangles[k].c == triangles[t].a || output[o].triangles[k].c == triangles[t].b || output[o].triangles[k].c == triangles[t].c   
                        ) 
                        && 
                        !(
                            output[o].triangles[k].id == triangles[t].id
                        )
                    ){
                        o.triangles.Add(triangles[t]);
                        // triangles.RemoveAt(t);
                        size = size - 1 ;
                    }else{
                        continue;
                    }
                }
            }
        }
                        
            // while ( triangles.Count > 0  ){
            // }


*/