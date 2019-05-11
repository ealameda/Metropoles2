using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserPortalDrawer : MonoBehaviour
{
    // Are we drawing the portal
    public bool isDrawing;
    // 
    public List<Vector3> positions;
    // The portal we are actually drawing
    public GameObject portal;
    // The renderer on the portal.
    LineRenderer portalRenderer;

    public float minPortalLength;
    public float maxPortalGap;

    // If we draw a portal, instantiate this prefab. 
    public GameObject UserPortalPrefab;
    
    // Start is called before the first frame update
    void Start()
    {
        positions = new List<Vector3>();
    }

    // Update is called once per frame
    void Update()
    {
        if (portal != null)
        {
            positions.Add(transform.position); 
            portalRenderer.positionCount = positions.Count;
            portalRenderer.SetPositions(positions.ToArray());
        }    
               
    }

    void StartDrawing()
    {
        portal = new GameObject("UserPortal");
        portalRenderer = portal.AddComponent<LineRenderer>();
        portalRenderer.material = gameObject.GetComponent<Pointer>().lineMaterial;  
        portalRenderer.startWidth = 0.01f;
        portalRenderer.endWidth = 0.01f;
        portalRenderer.startColor = Color.red;
        portalRenderer.endColor = Color.yellow;
    }

    void StopDrawing()
    {
        Destroy(portal);

        // If portal is long enough
        float totalLength = 0;
        for (int i = 0 ; i < positions.Count - 1; i++)
        {
            totalLength += (positions[i] - positions[i+1]).magnitude;
        }
        if (totalLength < minPortalLength)
        {
            Debug.Log("Portal only " + totalLength + " long");
            positions.Clear();
            return;
        }
        
        float portalGap = (positions[0] - positions[positions.Count-1]).magnitude;
        if (portalGap > maxPortalGap)
        {
            Debug.Log("Portal gap is too large at " + portalGap);
            positions.Clear();
            return;
        }

        Vector3 averagePosition = new Vector3();
        for (int i = 0 ; i < positions.Count; i++)
        {
            averagePosition += positions[i];
        }
        averagePosition /= positions.Count;

        GameObject newUserPortal = GameObject.Instantiate(UserPortalPrefab);
        newUserPortal.transform.position = averagePosition;
        newUserPortal.transform.LookAt(gameObject.transform.parent.position);

        positions.Clear();
    }

    public void StartOrStop()
    {
        Debug.Log("Got activated");
        if (portal == null)
            StartDrawing();
        else
            StopDrawing();
    }
}
