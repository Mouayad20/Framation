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

    // public List<OutputLink> Linking(){
    //     List<OutputLink> output  = new List<OutputLink>();

    //     for (int i = 0; i < skeletons[0].Count; i ++){
    //         OutputLink o = new OutputLink();
    //         o.line  = skeletons[0][i];
    //         for (int j = 0; j < triangles.Count; j ++){
    //             if (
    //                     Utils2D.Intersect(
    //                             skeletons[0][i].start.transform.position,skeletons[0][i].end.transform.position,
    //                             triangles[j].a,triangles[j].b
    //                         )
    //                     ||
    //                     Utils2D.Intersect(
    //                         skeletons[0][i].start.transform.position,skeletons[0][i].end.transform.position,
    //                         triangles[j].b,triangles[j].c
    //                         )
    //                     ||
    //                     Utils2D.Intersect(
    //                         skeletons[0][i].start.transform.position,skeletons[0][i].end.transform.position,
    //                         triangles[j].a,triangles[j].c
    //                         )
    //                 ) {
    //                     o.triangles.Add(triangles[j]);
    //                 }
    //         }
    //         output.Add(o);
    //     }

    //     return output; 
    // }
    public List<OutputLink> Linking(){

        List<OutputLink> output = new List<OutputLink>();

        List<LineController> lines = skeletons[0];

        // This dictionary will relate each point with its triangles
        Dictionary<Vector3, List<Triangle>> pointTriangles = new Dictionary<Vector3, List<Triangle>>();

        // We will fill this dictionary for all the points and their triangles
        // by adding each triangle to its 3 points (a, b, c)
        foreach (Triangle triangle in triangles)
        {
            // Initialize the lists for the points if they don't exist
            if (!pointTriangles.ContainsKey(triangle.a))
                pointTriangles[triangle.a] = new List<Triangle>();
            if (!pointTriangles.ContainsKey(triangle.b))
                pointTriangles[triangle.b] = new List<Triangle>();
            if (!pointTriangles.ContainsKey(triangle.c))
                pointTriangles[triangle.c] = new List<Triangle>();

            // Add the triangle to the corresponding points
            pointTriangles[triangle.a].Add(triangle);
            pointTriangles[triangle.b].Add(triangle);
            pointTriangles[triangle.c].Add(triangle);
        }

        // Now we know all triangles that touch a specific point
        // and now we can know the neighbors of a triangle by knowing the neighbors of all its points
        // so we will calculate them and put them in the neighbors dictionary

        // Define the neighbors dictionary and initialize it as empty
        Dictionary<Triangle, List<Triangle>> neighbors = new Dictionary<Triangle, List<Triangle>>();

        // Fill the neighbors dictionary by iterating over all the triangles
        // We will add each neighbor for any point of this triangle as a neighbor for it
        foreach (Triangle triangle in triangles)
        {
            neighbors[triangle] = new List<Triangle>();

            // Add the neighbors of point a
            foreach (Triangle neighbor in pointTriangles[triangle.a])
                neighbors[triangle].Add(neighbor);

            // Add the neighbors of point b
            foreach (Triangle neighbor in pointTriangles[triangle.b])
                neighbors[triangle].Add(neighbor);

            // Add the neighbors of point c
            foreach (Triangle neighbor in pointTriangles[triangle.c])
                neighbors[triangle].Add(neighbor);
        }

        // Now we have the neighbors dictionary filled correctly
        // and by it, we can access all the neighbors for a specific triangle
        // neighbors[triangle] -> List<Triangle>

        // This dictionary will give us in the end the distance of each triangle for each line
        Dictionary<KeyValuePair<LineController, Triangle>, int> distance = new Dictionary<KeyValuePair<LineController, Triangle>, int>();

        // We will fill this distance with -1 (-1 means that this value is not calculated yet)
        foreach (LineController line in lines)
        {
            foreach (Triangle triangle in triangles)
            {
                distance[new KeyValuePair<LineController, Triangle>(line, triangle)] = -1;
            }
        }

        Dictionary<LineController, List<Triangle>> directConnectedTriangles = new Dictionary<LineController, List<Triangle>>();


        for (int i = 0; i < lines.Count; i ++){
            directConnectedTriangles[lines[i]] = new List<Triangle>();
            for (int j = 0; j < triangles.Count; j ++){
                if (
                        Utils2D.Intersect(
                                lines[i].start.transform.position,lines[i].end.transform.position,
                                triangles[j].a,triangles[j].b
                            )
                        ||
                        Utils2D.Intersect(
                            lines[i].start.transform.position,lines[i].end.transform.position,
                            triangles[j].b,triangles[j].c
                            )
                        ||
                        Utils2D.Intersect(
                            lines[i].start.transform.position,lines[i].end.transform.position,
                            triangles[j].a,triangles[j].c
                            )
                    ) {
                        directConnectedTriangles[lines[i]].Add(triangles[j]);
                    }
            }
        }

        // Now we have to calculate the direct triangle for each line
        // Then we will do BFS for each line
        // In every BFS, we will calculate the distance of all the triangles for a specific line

        // Our BFS will be multi-source (that means we will start with a queue that has many nodes)
        // When we start this BFS, we will fill the queue with the direct triangles with 0 distance
        foreach (LineController line in lines)
        {
            Queue<KeyValuePair<Triangle, int>> queue = new Queue<KeyValuePair<Triangle, int>>();

            // Iterate over all the triangles that are directly connected with the temporary line
            // Set their distance as 0 and add them to the queue
            foreach (Triangle triangle in directConnectedTriangles[line])
            {
                distance[new KeyValuePair<LineController, Triangle>(line, triangle)] = 0;
                queue.Enqueue(new KeyValuePair<Triangle, int>(triangle, 0));
            }

            while (queue.Count > 0)
            {
                // Get the first element of the queue
                KeyValuePair<Triangle, int> current = queue.Dequeue();

                foreach (Triangle neighbor in neighbors[current.Key])
                {
                    // Check if we calculated this neighbor before or not
                    // If it is not calculated, we have to calculate it
                    // If it is already calculated, we will skip it
                    if (distance[new KeyValuePair<LineController, Triangle>(line, neighbor)] == -1)
                    {
                        // Calculate the distance for this neighbor
                        distance[new KeyValuePair<LineController, Triangle>(line, neighbor)] = current.Value + 1;
                        // Add it to the queue to calculate its neighbors as well
                        queue.Enqueue(new KeyValuePair<Triangle, int>(neighbor, current.Value + 1));
                    }
                }
            }
        }

        // Now, after the BFS is finished, we will have the distance dictionary filled with the correct values
        // We will be able to know the distance between each line and triangle (0 means direct touch)

        // Now we can relate the triangles to their lines in the output
        // Define the epsilon or take it as an argument of the function or access it by any way
        int epsilon = 2;

        // When epsilon == 0, that means we will add each triangle to the closest line
        // and we will add it to many lines if they are with the same closest distance
        // Ex: distances=[2, 2, 2, 4, 5]
        // In this example, we will add the triangle to the first 3 lines with a distance of 2

        // Define a dictionary that gives us all the triangles for a line
        Dictionary<LineController, List<Triangle>> lineTriangles = new Dictionary<LineController, List<Triangle>>();

        foreach (Triangle triangle in triangles)
        {
            int minimumDistance = int.MaxValue;

            // Calculate the minimum distance to compare with other distances
            foreach (LineController line in lines)
            {
                minimumDistance = Math.Min(minimumDistance, distance[new KeyValuePair<LineController, Triangle>(line, triangle)]);
            }

            // Compare the distances with the minimumDistance
            // If the minimumDistance + epsilon >= the distance for this line,
            // then we will consider it as connected to this triangle
            foreach (LineController line in lines)
            {
                if (distance[new KeyValuePair<LineController, Triangle>(line, triangle)] - minimumDistance <= epsilon)
                {
                    if (!lineTriangles.ContainsKey(line))
                        lineTriangles[line] = new List<Triangle>();
                    lineTriangles[line].Add(triangle);
                }
            }
        }

        // Now we have the lineTriangles dictionary filled
        // and we can access the triangles for a specific line directly through this dictionary
        // You can fill the output as needed

        /// ---------- here you have to fill the output from the lineTriangles dictionary --------- ///

        foreach (KeyValuePair<LineController, List<Triangle>> kvp in lineTriangles){
            OutputLink outputLink = new OutputLink();
            outputLink.line = kvp.Key;
            outputLink.triangles = kvp.Value;
            output.Add(outputLink);
        }
        print("____________***********__________");
        print(output[0].line.id);
        print(output[0].triangles.Count);
        print("__________________________________");
        print(output[1].line.id);
        print(output[1].triangles.Count);
        print("____________***********__________");
        return output;
    }

}
/*

        for (int i = 0; i < skeletons[0].Count; i ++){
            OutputLink o = new OutputLink();
            o.line  = skeletons[0][i];
            for (int j = 0; j < triangles.Count; j ++){
                if (
                        Utils2D.Intersect(
                                skeletons[0][i].start.transform.position,skeletons[0][i].end.transform.position,
                                triangles[j].a,triangles[j].b
                            )
                        ||
                        Utils2D.Intersect(
                            skeletons[0][i].start.transform.position,skeletons[0][i].end.transform.position,
                            triangles[j].b,triangles[j].c
                            )
                        ||
                        Utils2D.Intersect(
                            skeletons[0][i].start.transform.position,skeletons[0][i].end.transform.position,
                            triangles[j].a,triangles[j].c
                            )
                    ) {
                        o.triangles.Add(triangles[j]);
                    }
            }
            output.Add(o);
        }

        
        // while ( true ){
        //     for (int o = 0; o < output.Count; o++) {
        //         int sizeO = output[o].triangles.Count;
        //         for(int k = 0; k < sizeO ; k++) {
        //             for (int t = 0; t < triangles.Count; t ++) {
        //                 if (
        //                     output[o].triangles[k].id != triangles[t].id &&
        //                     (
        //                         output[o].triangles[k].a == triangles[t].a ||
        //                         output[o].triangles[k].a == triangles[t].b ||
        //                         output[o].triangles[k].a == triangles[t].c ||

        //                         output[o].triangles[k].b == triangles[t].a ||
        //                         output[o].triangles[k].b == triangles[t].b ||
        //                         output[o].triangles[k].b == triangles[t].c ||

        //                         output[o].triangles[k].c == triangles[t].a ||
        //                         output[o].triangles[k].c == triangles[t].b ||
        //                         output[o].triangles[k].c == triangles[t].c   
        //                     )
        //                 ){
        //                     output[o].triangles.Add(triangles[t]);
        //                     size = size - 1 ;
        //                     print("2size  :: " + size);
        //                     if (size <= 0 ) break;
        //                 }else continue;
        //             }
        //         }
        //     }

        // }        

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
          



*/



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