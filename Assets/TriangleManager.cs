
using UnityEngine;
 public class TriangleManager : MonoBehaviour
{
    public Rigidbody2D object1;
    public Rigidbody2D object2;
    public float springForce = 100f;
    public float dampingRatio = 0.5f;
    private Joint2D joint1;
    private Joint2D joint2;
    private SpringJoint2D springJoint;
     private bool isDraggingJoint1;
    private bool isDraggingJoint2;
     private void Start()
    {
        // Create Joint2D components
        joint1 = object1.gameObject.AddComponent<HingeJoint2D>();
        joint2 = object2.gameObject.AddComponent<HingeJoint2D>();
         // Set properties for the joints
        //joint1.anchor = new Vector2(0.5f, 0.5f);
        //joint2.anchor = new Vector2(-0.5f, 0.5f);
         // Create SpringJoint2D component
        springJoint = object1.gameObject.AddComponent<SpringJoint2D>();
         // Set the connected body for the SpringJoint2D
        springJoint.connectedBody = object2;
         // Set the spring and damping properties
        springJoint.frequency = springForce;
        springJoint.dampingRatio = dampingRatio;
    }
     private void Update()
    {
        // Check for mouse input
        if (Input.GetMouseButtonDown(0))
        {
            // Get the mouse position in world space
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
             // Check if the mouse is over joint1
            if (joint1 != null && joint1.attachedRigidbody != null &&
                joint1.attachedRigidbody.position == mousePosition)
            {
                isDraggingJoint1 = true;
            }
            // Check if the mouse is over joint2
            else if (joint2 != null && joint2.attachedRigidbody != null &&
                     joint2.attachedRigidbody.position == mousePosition)
            {
                isDraggingJoint2 = true;
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            // Stop dragging
            isDraggingJoint1 = false;
            isDraggingJoint2 = false;
        }
         // Move the joints while dragging
        if (isDraggingJoint1)
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            joint1.attachedRigidbody.MovePosition(mousePosition);
        }
        else if (isDraggingJoint2)
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            joint2.attachedRigidbody.MovePosition(mousePosition);
        }
    }
}



// using UnityEngine;
//  public class TriangleManager : MonoBehaviour
// {

//     public GameObject jointPrefab;

//     public int numJoints = 3;
//     public float jointSpacing    = 0.5f;
//     public float springStrength  = 0.5f;
//     public float springDamping   = 0.5f;

//     private GameObject[] joints;
//     private SpringJoint[] springs;
//     private LineRenderer lineRenderer;

//     GameObject joint1;
//     GameObject joint2;
//     SpringJoint spring1;
//     Vector3 joint1Pos;
//     Vector3 joint2Pos;
//     bool soso ;

//     private void Start()
//     {
//         // CreateJoints();
//         // CreateSprings();
//         joint1Pos = new Vector3(0f, 0f, 0f);
//         joint2Pos = new Vector3(2f, 0f, 0f);
        
//         lineRenderer = gameObject.AddComponent<LineRenderer>();
//         lineRenderer.positionCount = 2;

//         // Set the line renderer properties (material, width, color, etc.)
//         // Adjust these properties to match your desired visual style

//         // Example properties:
//         lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
//         lineRenderer.startWidth = 0.1f;
//         lineRenderer.endWidth = 0.1f;
//         lineRenderer.startColor = Color.red;
//         lineRenderer.endColor = Color.red;

//         joint1 = Instantiate(jointPrefab);
//         joint2 = Instantiate(jointPrefab);
        
//         soso = true;
//     }

//     public void Update()
//     {
//         if (soso)
//         {
//             soso = false;
//             spring1 = joint1.AddComponent<SpringJoint>();
//         }

//          if (Input.GetMouseButton(0)){ 
//             print(">>  " + joint1.transform.position);
//             Vector3 v = GetMousePosition();
//             v.z = 0 ;
//             joint1Pos = v;
//          }

//         joint1.transform.position = joint1Pos ;
//         joint2.transform.position = joint2Pos ;
//         lineRenderer.SetPosition(0, joint1.transform.position);
//         lineRenderer.SetPosition(1, joint2.transform.position);

//         spring1.autoConfigureConnectedAnchor = false;
//         spring1.connectedBody = joint2.GetComponent<Rigidbody>();
//         spring1.spring = springStrength;
//         spring1.damper = springDamping;
            
//     }

//     private Vector3 GetMousePosition(){
//         Vector3 worldMousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
//         worldMousePosition.z = 0;
//         return worldMousePosition;
//     }




//     private void CreateJoints()
//     {
//         joints = new GameObject[numJoints];

//         joints[0] = Instantiate(jointPrefab, new Vector3(0f, 0f, 0f), Quaternion.identity);;
//         joints[1] = Instantiate(jointPrefab, new Vector3(2f, 0f, 0f), Quaternion.identity);;
//         // joints[2] = Instantiate(jointPrefab, new Vector3(1f, 2f, 0f), Quaternion.identity);
//     }

//     private void CreateSprings()
//     {
//         springs = new SpringJoint[numJoints];

//         SpringJoint spring1 = joints[0].AddComponent<SpringJoint>();
//         spring1.autoConfigureConnectedAnchor = false;
//         spring1.connectedBody = joints[1].GetComponent<Rigidbody>();
//         spring1.spring = springStrength;
//         spring1.damper = springDamping;
//         springs[0] = spring1;


//         ////////////////////////

//         // SpringJoint spring2 = joints[1].AddComponent<SpringJoint>();
//         // spring2.autoConfigureConnectedAnchor = true;
//         // spring2.connectedBody = joints[0].GetComponent<Rigidbody>();
//         // spring2.spring = springStrength;
//         // spring2.damper = springDamping;

//         // springs[1] = spring2;
        
//         ////////////////////////

//         // SpringJoint spring3 = joints[2].AddComponent<SpringJoint>();
//         // spring3.autoConfigureConnectedAnchor = true;
//         // spring3.connectedBody = joints[0].GetComponent<Rigidbody>();
//         // spring3.spring = springStrength;
//         // spring3.damper = springDamping;

//         // springs[2] = spring3;
//     }
// }

// /*
// csharp
// using UnityEngine;
//  public class TriangleSpringSystem : MonoBehaviour
// {
//     public GameObject jointPrefab;
//     public GameObject springPrefab;
//      public int numJoints = 3;
//     public float jointSpacing = 1f;
//     public float springStrength = 1f;
//     public float springDamping = 0.5f;
//      private GameObject[] joints;
//     private SpringJoint[] springs;
//      private void Start()
//     {
//         CreateJoints();
//         CreateSprings();
//     }
//      private void CreateJoints()
//     {
//         joints = new GameObject[numJoints];
//          for (int i = 0; i < numJoints; i++)
//         {
//             Vector3 position = new Vector3(i * jointSpacing, 0f, 0f);
//             GameObject joint = Instantiate(jointPrefab, position, Quaternion.identity);
//             joints[i] = joint;
//         }
//     }
//      private void CreateSprings()
//     {
//         springs = new SpringJoint[numJoints];
//          for (int i = 0; i < numJoints; i++)
//         {
//             GameObject jointA = joints[i];
//             GameObject jointB = joints[(i + 1) % numJoints];
//              SpringJoint spring = jointA.AddComponent<SpringJoint>();
//             spring.autoConfigureConnectedAnchor = false;
//             spring.connectedBody = jointB.GetComponent<Rigidbody>();
//             spring.spring = springStrength;
//             spring.damper = springDamping;
//              springs[i] = spring;
//         }
//     }
// }
// */
