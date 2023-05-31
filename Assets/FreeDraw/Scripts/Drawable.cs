namespace FreeDraw
{
    using UnityEngine;
    using UnityEngine.EventSystems;
    using UnityEngine;
    using UnityEngine.UI;
    using System.IO;
    using System.Collections;
    using System.Collections.Generic;
    using OpenCvSharp;
    using Newtonsoft.Json;
    using mattatz.Triangulation2DSystem;
    using mattatz.Triangulation2DSystem.Example;

    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(Collider2D))]  // REQUIRES A COLLIDER2D to function
    // 1. Attach this to a read/write enabled sprite image
    // 2. Set the drawing_layers  to use in the raycast
    // 3. Attach a 2D collider (like a Box Collider 2D) to this sprite
    // 4. Hold down left mouse to draw on this texture!
    public class Drawable : MonoBehaviour
    {

        [SerializeField] GameObject prefab;
        List<Vector2> points = new List<Vector2>() ;
        public static bool isDrawing = true ;
        public static bool DrawTriangulation = false ;
        // PEN COLOUR
        public static Color Pen_Colour = Color.red;     // Change these to change the default drawing settings
        // PEN WIDTH (actually, it's a radius, in pixels)
        public static int Pen_Width = 7;

        public delegate void Brush_Function(Vector2 world_position);
        // This is the function called when a left click happens
        // Pass in your own custom one to change the brush type
        // Set the default function in the Awake method
        public Brush_Function current_brush;

        public LayerMask Drawing_Layers;

        public bool Reset_Canvas_On_Play = true;
        // The colour the canvas is reset to each time
        public Color Reset_Colour = new Color(0, 0, 0, 0);  // By default, reset the canvas to be transparent

        // Used to reference THIS specific file without making all methods static
        public static Drawable drawable;
        // MUST HAVE READ/WRITE enabled set in the file editor of Unity
        Sprite drawable_sprite;
        Texture2D drawable_texture;

        Vector2 previous_drag_position;
        Color[] clean_colours_array;
        Color transparent;
        Color32[] cur_colors;
        bool mouse_was_previously_held_down = false;
        bool no_drawing_on_current_drag = false;
        public Mesh globalMesh;
        public static List<Triangle> triangles ;
        public static Link link ;
        public static Dictionary<LineController, List<Triangle>> output ;



//////////////////////////////////////////////////////////////////////////////
// BRUSH TYPES. Implement your own here

        // When you want to make your own type of brush effects,
        // Copy, paste and rename this function.
        // Go through each step
        public void BrushTemplate(Vector2 world_position)
        {
            // 1. Change world position to pixel coordinates
            Vector2 pixel_pos = WorldToPixelCoordinates(world_position);

            // 2. Make sure our variable for pixel array is updated in this frame
            cur_colors = drawable_texture.GetPixels32();

            ////////////////////////////////////////////////////////////////
            // FILL IN CODE BELOW HERE

            // Do we care about the user left clicking and dragging?
            // If you don't, simply set the below if statement to be:
            //if (true)

            // If you do care about dragging, use the below if/else structure
            if (previous_drag_position == Vector2.zero)
            {
                // THIS IS THE FIRST CLICK
                // FILL IN WHATEVER YOU WANT TO DO HERE
                // Maybe mark multiple pixels to colour?
                MarkPixelsToColour(pixel_pos, Pen_Width, Pen_Colour);
            }
            else
            {
                // THE USER IS DRAGGING
                // Should we do stuff between the previous mouse position and the current one?
                ColourBetween(previous_drag_position, pixel_pos, Pen_Width, Pen_Colour);
            }
            ////////////////////////////////////////////////////////////////

            // 3. Actually apply the changes we marked earlier
            // Done here to be more efficient
            ApplyMarkedPixelChanges();
            
            // 4. If dragging, update where we were previously
            previous_drag_position = pixel_pos;
        }

        // Default brush type. Has width and colour.
        // Pass in a point in WORLD coordinates
        // Changes the surrounding pixels of the world_point to the static pen_colour
        public void PenBrush(Vector2 world_point)
        {
            Vector2 pixel_pos = WorldToPixelCoordinates(world_point);

            cur_colors = drawable_texture.GetPixels32();

            if (previous_drag_position == Vector2.zero)
            {
                // If this is the first time we've ever dragged on this image, simply colour the pixels at our mouse position
                MarkPixelsToColour(pixel_pos, Pen_Width, Pen_Colour);
            }
            else
            {
                // Colour in a line from where we were on the last update call
                ColourBetween(previous_drag_position, pixel_pos, Pen_Width, Pen_Colour);
            }
            ApplyMarkedPixelChanges();

            //Debug.Log("Dimensions: " + pixelWidth + "," + pixelHeight + ". Units to pixels: " + unitsToPixels + ". Pixel pos: " + pixel_pos);
            previous_drag_position = pixel_pos;
        }

        // Helper method used by UI to set what brush the user wants
        // Create a new one for any new brushes you implement
        public void SetPenBrush()
        {
            // PenBrush is the NAME of the method we want to set as our current brush
            current_brush = PenBrush;
        }
//////////////////////////////////////////////////////////////////////////////

        // This is where the magic happens.
        // Detects when user is left clicking, which then call the appropriate function
        void Update()
        {

            if(Input.GetKeyDown(KeyCode.G)){
                isDrawing = !isDrawing;
                ConvertSpriteToImage();
            }

            if(Input.GetKeyDown(KeyCode.P)){
                print("PPPPPPPPPPPPPPPPPPPPPPPPPPPPP");
                link = new Link();
				output = link.Linking();
            }

            if(Input.GetKeyDown(KeyCode.T)){
                print("TTTTTTTTTTTTTTTTTTTTTTTTTTTTT");
                DrawTriangulation = !DrawTriangulation;
            }
            
            if(isDrawing){
                   
                // Is the user holding down the left mouse button?
                bool mouse_held_down = Input.GetMouseButton(0);
                if (mouse_held_down && !no_drawing_on_current_drag)
                {
                    // Convert mouse coordinates to world coordinates
                    Vector2 mouse_world_position = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                    // Check if the current mouse position overlaps our image
                    Collider2D hit = Physics2D.OverlapPoint(mouse_world_position, Drawing_Layers.value);
                    if (hit != null && hit.transform != null)
                    {
                        // We're over the texture we're drawing on!
                        // Use whatever function the current brush is
                        current_brush(mouse_world_position);
                    }

                    else
                    {
                        // We're not over our destination texture
                        previous_drag_position = Vector2.zero;
                        if (!mouse_was_previously_held_down)
                        {
                            // This is a new drag where the user is left clicking off the canvas
                            // Ensure no drawing happens until a new drag is started
                            no_drawing_on_current_drag = true;
                        }
                    }
                }
                // Mouse is released
                else if (!mouse_held_down)
                {
                    previous_drag_position = Vector2.zero;
                    no_drawing_on_current_drag = false;
                }
                mouse_was_previously_held_down = mouse_held_down;
            }
        }

        // Set the colour of pixels in a straight line from start_point all the way to end_point, to ensure everything inbetween is coloured
        public void ColourBetween(Vector2 start_point, Vector2 end_point, int width, Color color)
        {
            // Get the distance from start to finish
            float distance = Vector2.Distance(start_point, end_point);
            Vector2 direction = (start_point - end_point).normalized;

            Vector2 cur_position = start_point;

            // Calculate how many times we should interpolate between start_point and end_point based on the amount of time that has passed since the last update
            float lerp_steps = 1 / distance;

            for (float lerp = 0; lerp <= 1; lerp += lerp_steps)
            {
                cur_position = Vector2.Lerp(start_point, end_point, lerp);
                MarkPixelsToColour(cur_position, width, color);
            }
        }

        public void MarkPixelsToColour(Vector2 center_pixel, int pen_thickness, Color color_of_pen)
        {
            // Figure out how many pixels we need to colour in each direction (x and y)
            int center_x = (int)center_pixel.x;
            int center_y = (int)center_pixel.y;
            //int extra_radius = Mathf.Min(0, pen_thickness - 2);

            for (int x = center_x - pen_thickness; x <= center_x + pen_thickness; x++)
            {
                // Check if the X wraps around the image, so we don't draw pixels on the other side of the image
                if (x >= (int)drawable_sprite.rect.width || x < 0)
                    continue;

                for (int y = center_y - pen_thickness; y <= center_y + pen_thickness; y++)
                {
                    MarkPixelToChange(x, y, color_of_pen);
                }
            }
        }

        public void MarkPixelToChange(int x, int y, Color color)
        {
            // Need to transform x and y coordinates to flat coordinates of array
            int array_pos = y * (int)drawable_sprite.rect.width + x;

            // Check if this is a valid position
            if (array_pos > cur_colors.Length || array_pos < 0)
                return;

            cur_colors[array_pos] = color;
        }

        public void ApplyMarkedPixelChanges()
        {
            drawable_texture.SetPixels32(cur_colors);
            drawable_texture.Apply();
        }

        // Directly colours pixels. This method is slower than using MarkPixelsToColour then using ApplyMarkedPixelChanges
        // SetPixels32 is far faster than SetPixel
        // Colours both the center pixel, and a number of pixels around the center pixel based on pen_thickness (pen radius)
        public void ColourPixels(Vector2 center_pixel, int pen_thickness, Color color_of_pen)
        {
            // Figure out how many pixels we need to colour in each direction (x and y)
            int center_x = (int)center_pixel.x;
            int center_y = (int)center_pixel.y;
            //int extra_radius = Mathf.Min(0, pen_thickness - 2);

            for (int x = center_x - pen_thickness; x <= center_x + pen_thickness; x++)
            {
                for (int y = center_y - pen_thickness; y <= center_y + pen_thickness; y++)
                {
                    drawable_texture.SetPixel(x, y, color_of_pen);
                }
            }

            drawable_texture.Apply();
        }

        public Vector2 WorldToPixelCoordinates(Vector2 world_position)
        {
            // Change coordinates to local coordinates of this image
            Vector3 local_pos = transform.InverseTransformPoint(world_position);

            // Change these to coordinates of pixels
            float pixelWidth = drawable_sprite.rect.width;
            float pixelHeight = drawable_sprite.rect.height;
            float unitsToPixels = pixelWidth / drawable_sprite.bounds.size.x * transform.localScale.x;

            // Need to center our coordinates
            float centered_x = local_pos.x * unitsToPixels + pixelWidth / 2;
            float centered_y = local_pos.y * unitsToPixels + pixelHeight / 2;

            // Round current mouse position to nearest pixel
            Vector2 pixel_pos = new Vector2(Mathf.RoundToInt(centered_x), Mathf.RoundToInt(centered_y));

            return pixel_pos;
        }

        // Changes every pixel to be the reset colour
        public void ResetCanvas()
        {
            drawable_texture.SetPixels(clean_colours_array);
            drawable_texture.Apply();
        }

        void Awake()
        {
            drawable = this;
            // DEFAULT BRUSH SET HERE
            current_brush = PenBrush;

            drawable_sprite = this.GetComponent<SpriteRenderer>().sprite;
            drawable_texture = drawable_sprite.texture;

            // Initialize clean pixels to use
            clean_colours_array = new Color[(int)drawable_sprite.rect.width * (int)drawable_sprite.rect.height];
            for (int x = 0; x < clean_colours_array.Length; x++)
                clean_colours_array[x] = Reset_Colour;

            // Should we reset our canvas image when we hit play in the editor?
            if (Reset_Canvas_On_Play)
                ResetCanvas();
        }

        public void ConvertSpriteToImage()
        {
            if (drawable_sprite != null)
            {
                // Create a new Texture2D with the Sprite's dimensions
                Texture2D texture = new Texture2D((int)drawable_sprite.rect.width, (int)drawable_sprite.rect.height);
                
                // Get the pixels from the drawable_sprite and set them to the Texture2D
                texture.SetPixels(drawable_sprite.texture.GetPixels(
                    (int)drawable_sprite.textureRect.x,
                    (int)drawable_sprite.textureRect.y,
                    (int)drawable_sprite.textureRect.width,
                    (int)drawable_sprite.textureRect.height
                    )
                );
                
                // Apply the changes to the Texture2D
                texture.Apply();

                // Convert the Texture2D to bytes (PNG format)
                byte[] bytes = texture.EncodeToPNG();

                //    BEGIN find contours from texture 

                    float width  = 10 ;
                    float height = 7.5f;
                    float center_x = width   / 2 ; 
                    float center_y = height  / 2 ;

                    Mat image = Unity.TextureToMat (texture);

                    //Gray scale image
                    Mat grayMat = new Mat();

                    Cv2.CvtColor (image, grayMat, ColorConversionCodes.BGR2GRAY); 

                    Mat thresh = new Mat ();
                    Cv2.Threshold (grayMat, thresh, 127, 255, ThresholdTypes.BinaryInv);

                    Cv2.Circle(image, new Point(0,0), 5, new Scalar(0,0,255),-1);

                    // Extract Contours
                    Point[][] contours;
                    HierarchyIndex[] hierarchy;
                    Cv2.FindContours (thresh, out contours, out hierarchy, RetrievalModes.Tree, ContourApproximationModes.ApproxNone, null);

                    for (int k = 0 ; k < contours[0].Length  ; k++){
                        points.Add(
                            new Vector2(
                                ((  contours[0][k].X - center_x ) / 100 ) + 0.05f ,
                                (( -contours[0][k].Y - center_y ) / 100 ) + 0.05f 
                                )
                            );
                    }

                    points = Utils2D.Constrain(points, 0.25f );
                    var polygon = Polygon2D.Contour(points.ToArray());
                    var vertices = polygon.Vertices;
                    if(vertices.Length < 3) return; // error
                    var triangulation = new Triangulation2D(polygon, 30);
                    var go = Instantiate(prefab);
                    go.transform.SetParent(transform, false);
                    go.GetComponent<DemoMesh>().SetTriangulation(triangulation);
                    globalMesh = go.GetComponent<DemoMesh>().mesh;
                    int[] trianglesPoints = globalMesh.triangles;
                    int id = 0 ;
                    triangles = new List<Triangle>();
                    for (int i = 0; i < trianglesPoints.Length; i += 3){
                        Triangle triangle = new Triangle();
                        triangle.id = id ;
                        triangle.a = globalMesh.vertices[trianglesPoints[i + 0]];
                        triangle.b = globalMesh.vertices[trianglesPoints[i + 1]];
                        triangle.c = globalMesh.vertices[trianglesPoints[i + 2]];
                        triangles.Add(triangle);
                        id = id + 1 ;
				    }

                    points.Clear(); 

                //    END  find contours from texture 


                // Optionally, you can save the bytes to a file
                // System.IO.File.WriteAllBytes("C:/Users/HP/Downloads/Compressed/trianglation/Assets/OpenCV+Unity/Demo/Identifiy_Contours_by_Shape/someshapes.jpg", bytes);

                // Destroy the temporary Texture2D object
                Destroy(texture);

                // Now you can use the bytes as an image or apply them to a texture
            }
            else
            {
                Debug.LogError("drawable_sprite is not assigned!");
            }
        }
    }
}