using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserPortalDrawer : MonoBehaviour
{
    // Are we drawing the portal
    public bool isDrawing;
    // The portal we are actually drawing
    public GameObject portal;
    // The UserPortal script on the portal.
    UserPortal portalScript;

    public float minPortalLength;
    public float maxPortalGap;

    // If we draw a portal, instantiate this prefab. 
    public GameObject UserPortalPrefab;
    
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (portal != null)
        {
            portalScript.AddPoint(transform.position);
        }                   
    }

    void StartDrawing()
    {
        portal = new GameObject("UserPortal");
        portalScript = portal.AddComponent<UserPortal>();
        Debug.Log("Setting mats");
        portalScript.SetMaterials(gameObject.GetComponent<Pointer>().lineMaterial,
                                  gameObject.GetComponent<Pointer>().lineMaterial);
    }

    void StopDrawing()
    {
        portalScript.FinishDrawing();
        // The portal now lives its own life.
        portal = null;
        portalScript = null;
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
