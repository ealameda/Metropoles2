using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This class encapsulated all highlighting (but not grabbing) behavior.
public class Pointer : MonoBehaviour
{
    // How far should the pointer be able to point? Cannot be infinite.
    public float maxDistance;

    // How many points do we use to approximate the path. Higher costs performance.
    public int quality;
    
    // Material to put on line. Probably default line material.
    public Material lineMaterial;

    // Should the path be visible?
    public bool isPathVisible;
    // If isPathVisible, this is a LineRenderer for the path. 
    // Will detect existing one, or generate a new one.  
    LineRenderer pathRenderer;

    //TODO: Cache Approximate path.
    // Stores last approximation of path. 
    List<Vector3> approximatePath;
    // Stores t-values used in approximation. 
    List<float> tSteps;

    // What Grabable are we currently pointing at, if any
    public Grabable currentPointee;
    public Vector3? currentPointeeHandle;
    
    // Boolean for turning off searching for things to point at. 
    public bool shouldPoint;
    // Wand need to point "up" instead of "forward"
    public bool pointUp;


    // This defines the path that the Pointer points along. 
    // It is a parametric path, taking t in [0,1]
    Vector3 ParametricPath(float t)
    {
        Vector3 straight;
        if (pointUp)
            straight = transform.position + transform.up * t * maxDistance;
        else
            straight = transform.position + transform.forward * t * maxDistance;
        return (straight + transform.up * -1 *t*t);
    }

    // Calculates new approximation to the path, and returns a list. 
    List<Vector3> ApproximatePath()
    {
        for (int i = 0 ; i <= quality ; i++)
        {
            approximatePath[i] = ParametricPath(tSteps[i]);
        }
        return approximatePath;
    }

    // Given a Collider, tests to see whether my line intersects it. 
    // Returns the point of intersection, or null if not intersecting.
    Vector3? IntersectsPath(Collider collider) 
    {
        for (int i = 0 ; i < quality ; i++)
        {
            Vector3 direction = approximatePath[i+1] - approximatePath[i];
            Ray segment = new Ray (approximatePath[i], direction);
            // Debug.Log("Testing ray " + segment + " with collider " + collider.name);
            float intersectionDistance;
            if (collider.bounds.IntersectRay(segment, out intersectionDistance) && intersectionDistance <= direction.magnitude)
            {
                // Debug.Log("Intersects at " + intersectionDistance);
                return segment.GetPoint(intersectionDistance);
            }
        }
        return null;
    }

    float DistanceToPoint(Vector3 point)
    {
        return (approximatePath[0] - point).magnitude;
    }

    // Find all Grabable objects and Intersect them
    // Uses "Grabable" tag for selection purposes. This is probably
    // a good place to start profiling for performance improvements. 
    void TestAllGrabablesForIntersection()
    {
        GameObject[] grabableObjects = GameObject.FindGameObjectsWithTag("Grabable");
        Vector3? intersectionPoint = null;
        
        Grabable closestGrabable = null;
        float closestGrabDistance = float.MaxValue;
        foreach (GameObject grabableObject in grabableObjects)
        {
            Grabable grabable = grabableObject.GetComponent<Grabable>();
            if (grabable == null)
            {
                Debug.Log("Object with tag 'grabable' " + grabableObject.name + " does not have a Grabable component");
                continue;
            }
            intersectionPoint = IntersectsPath(grabableObject.GetComponent<Collider>());
            if (intersectionPoint != null && DistanceToPoint(intersectionPoint.Value) < closestGrabDistance)
            {
                closestGrabDistance = DistanceToPoint(intersectionPoint.Value);
                closestGrabable = grabable;
            }
        }

        // Short circuit if nothing is changing.
        if (currentPointee == closestGrabable)
            return;
        
        // If we're no longer pointing to our currentPointee, stop highlighting.
        if (currentPointee != null)
        {
            currentPointee.Unhighlight(this);
            currentPointee = null;
        }

        // If we found anything, highlight it. 
        if (closestGrabable != null)
        {
            closestGrabable.Highlight(this);
            currentPointee = closestGrabable;
            currentPointeeHandle = intersectionPoint.Value;
        }
    }

    // Called in Start, initializes internal variables
    void InitializeVariables()
    {
        // Path variables
        Debug.Log("Initializing");
        approximatePath = new List<Vector3> (new Vector3[quality+1]) ;
        tSteps = new List<float>(new float[quality+1]);
        float tStep = 1.0f / quality;
        for (int i = 0 ; i <= quality; i++)
        {
            tSteps[i] = tStep * i;
        }

        // LineRenderer variables
        if (isPathVisible)
        {
            pathRenderer = GetComponent<LineRenderer>();
            if (pathRenderer == null) {
                pathRenderer = gameObject.AddComponent<LineRenderer>();
                pathRenderer.startWidth = 0.02f;
                pathRenderer.endWidth = 0.02f;
                pathRenderer.startColor = Color.green;
                pathRenderer.endColor = Color.blue;

                pathRenderer.material = lineMaterial; 
            }
        }
    }

    void StartParticleSystem()
    {
        
    }

    // Start is called before the first frame update
    protected void Start()
    {
        InitializeVariables();

    }

    // Update is called once per frame
    protected void Update()
    {
        ApproximatePath();
        if (isPathVisible) {
            pathRenderer.positionCount = approximatePath.Count;
            pathRenderer.SetPositions(approximatePath.ToArray());
        }
        if (shouldPoint)
            TestAllGrabablesForIntersection();
    }
}
