using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UserPortal : MonoBehaviour
{
    // Configs
    float minimumLineLength = 1.0f;
    float maximumLineGap = 0.1f;

    // Track the other scripts on this object
    LineRenderer lineRenderer;
    Material lineMaterial;
    Mesh mesh;
    MeshRenderer meshRenderer;
    MeshFilter meshFilter;
    MeshCollider meshCollider;
    Material meshMaterial;

    // Public for visibility - can't render portal until there are material
    public bool rendering = false;


    // Positions that have been added to this portal
    public List<Vector3> positions;

    // Variables to keep track the current portal drawing. 
    // The Average of all the positions will be the center of the portal
    Vector3 averagePosition;
    // LineLength will be used to determine if the portal is big enough to coagulate.
    float lineLength;

    GameObject debugCenter;

    // Morphing variables
    // Public for visibility - is this morphing into a circle?
    public bool morphing;
    // Store the old positions before we start morphing;
    Vector3[] startMorphPositions;
    // Store target positions to morph to.
    Vector3[] endMorphPositions;
    // Time to morph
    float morphDuration = 3.0f;
    // Morph percentage
    float morphT = 0;
    // How close to the portal can you get before being portaled?
    float portalTriggerDistance = 0.01f;


    // Add a point to the portal. 
    public void AddPoint(Vector3 newPoint)
    {
        // Sometimes, AddPoint gets called before we're done instanstiating
        // all our variables. If so, just skip it. 
        if (positions == null)
            return;
        if (positions.Count == 0)
        {
            averagePosition = newPoint;
            lineLength = 0;
        }
        else
        {
            averagePosition *= positions.Count;
            averagePosition += newPoint;
            averagePosition /= (positions.Count +1);
            //debugCenter.transform.position = averagePosition;

            lineLength += (positions[positions.Count -1] - newPoint).magnitude;
        }
        positions.Add(newPoint);
    }

    public void FinishDrawing()
    {
        // If the line isn't long enough, destroy this object
        if (lineLength < minimumLineLength)
        {
            Debug.Log("Line is too short at " + lineLength);
            Destroy(gameObject);
            return;
        }

        // If the Gap is too large, destroy object
        float gap = (positions[0] - positions[positions.Count -1]).magnitude; 
        if (gap  > maximumLineGap)
        {
            Debug.Log("Gap is too large at "+ gap);
            Destroy(gameObject);
            return;
        }

        // This is a good portal!
        Debug.Log("This is a good portal!");
        StartMorph();

    }

    public void StartMorph()
    {
        morphing = true;
        startMorphPositions = positions.ToArray();
        endMorphPositions = positions.ToArray();
        float radius = 0;
        for (int i = 0; i < endMorphPositions.Length; ++i)
        {
            float distanceToCenter = ( endMorphPositions[i] - averagePosition).magnitude; 
            if (distanceToCenter > radius)
            {
                radius = distanceToCenter;
            }
        }
        // Set end Morph positions
        for (int i = 0; i < endMorphPositions.Length; ++i)
        {
            Vector3 direction = (endMorphPositions[i] - averagePosition).normalized;
            endMorphPositions[i] = averagePosition + direction * radius;
        }
    }

    public void StopMorph() 
    {
        meshCollider = gameObject.AddComponent<MeshCollider>();
        meshCollider.convex = true;
        debugCenter = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        debugCenter.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
    }

    public void SetMaterials(Material newLineMat, Material newMeshMat)
    {
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.startWidth = 0.01f;
        lineRenderer.endWidth = 0.01f;
        lineRenderer.startColor = Color.red;
        lineRenderer.endColor = Color.yellow;
        lineRenderer.material = newLineMat;


        meshRenderer = gameObject.AddComponent<MeshRenderer>();
        meshRenderer.material = newMeshMat;
        rendering = true;
    }
    // Start is called before the first frame update
    void Start()
    {
        mesh = new Mesh();
        meshFilter = gameObject.AddComponent<MeshFilter>();
        meshFilter.mesh = mesh;
        positions = new List<Vector3>();

        // debugCenter = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        // debugCenter.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
    }

    // Update is called once per frame
    void Update()
    {
        UpdateLine();
        UpdateMesh();
        UpdateMorph(); // No op unless morph has been started. 
        UpdateCollider(); // No op unless motph has finished. 

    }

    void UpdateLine()
    {
        lineRenderer.positionCount = positions.Count;
        lineRenderer.SetPositions(positions.ToArray());
    }

    void UpdateMesh() 
    {
        mesh.Clear();
        int numTriangles = positions.Count; 
        Vector3[] newVertices = new Vector3[numTriangles +1];
        Color[] newColors = new Color[numTriangles + 1];
        int[] newTriangles = new int[numTriangles *3];
        newVertices[numTriangles] = averagePosition;
        newColors[numTriangles] = Color.blue;
        for (int i = 0; i < numTriangles; ++i)
        {
            newVertices[i] = positions[i];
            newColors[i] = Color.black;
            newTriangles[i*3] = numTriangles; // center
            newTriangles[i*3+1] = i; // this vertex
            newTriangles[i*3+2] = (i-1 + numTriangles) % numTriangles; // previous vertex
        }
        mesh.vertices = newVertices;
        mesh.triangles = newTriangles;
        mesh.colors = newColors;
    }

    void UpdateMorph()
    {
        if (!morphing)
            return;
        if (morphT > 1) 
        {
            StopMorph();
            morphing = false;
        }
        morphT += Time.deltaTime / morphDuration;
        //Debug.Log("Morhing percentage " + morphT);
        for (int i = 0; i < positions.Count; ++i)
        {
            positions[i] = Vector3.Lerp(startMorphPositions[i], endMorphPositions[i], morphT);
        }
    }

    void UpdateCollider()
    {
        if (meshCollider == null)
            return;
        GameObject camera = GameObject.FindGameObjectWithTag("MainCamera");
        Vector3 closestPoint = meshCollider.ClosestPoint(camera.transform.position);
        debugCenter.transform.position = closestPoint;
        Debug.Log("Distance " + (closestPoint - camera.transform.position).magnitude);
        if ((closestPoint - camera.transform.position).magnitude < portalTriggerDistance)
        {
            SceneChanger sc = GameObject.FindGameObjectWithTag("PersistentScripts").GetComponentInChildren<SceneChanger>();
            if (sc == null)
            {
                Debug.LogError("No scene changer could be found");
                return;
            }
            sc.StartChange();
        }
    }

    
}
