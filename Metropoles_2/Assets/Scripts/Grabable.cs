using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Interacts with two other clasess: Pointer and Manipulator
public class Grabable : MonoBehaviour
{
    // Is this grabable highlighted by a Pointer?
    // Is basically whether the highlighters list is empty.
    public bool isHighlighted = false;
    // List of all Pointers currently highlighting this object.
    List<Pointer> highlighters;

    // EventManager is used for interface to Kent. 
    EventManager eventManager;

    // The Manipulator currently grabbing this object.
    public Manipulator grabber;

    // This Joint will be used to connect the Grabable to the Manipulator.
    // In the case of a building, it will also pull the building to the zone.
    public SpringJoint joint;
    // We deactivate the joint by setting its spring to 0, so remember what it should be. 
    float springFactor;

    public GameObject marker;


    // Start is called before the first frame update
    void Start()
    {
        eventManager = GameObject.FindObjectOfType<EventManager>();
        highlighters = new List<Pointer>();
        InitializeScripts();
    }

    void InitializeScripts()
    {
        joint = gameObject.GetComponent<SpringJoint>();
        if (joint == null)
        {
            joint = gameObject.AddComponent<SpringJoint>();
        }
        springFactor = joint.spring;        
    }

    // Update is called once per frame
    void Update()
    {
        ResolveHighlights();

        // Rotate towards manipulator if grabbed
        if (grabber != null)
        {
            RotateTowards(grabber.transform.position);
        }

    }

    // Torques us to that our forward vector points towards this position
    void RotateTowards (Vector3 targetToFace)
    {
        Rigidbody rigid = GetComponent<Rigidbody>(); 
        Vector3 towardsTarget = (targetToFace - transform.position).normalized;
        Vector3 axis = Vector3.Cross(transform.forward, towardsTarget);

        Quaternion q = transform.rotation * rigid.inertiaTensorRotation;
        Vector3 Torque = q * Vector3.Scale(rigid.inertiaTensor, (Quaternion.Inverse(q)*axis ));
        
        rigid.AddTorque(Torque);
    }

    // Reports highlights or unhighlights and triggers events. j   
    void ResolveHighlights()
    {
        // Is there a new highlight?
        if (!isHighlighted && highlighters.Count > 0)
        {
            isHighlighted = true;
            eventManager.SendMessage("buildingHighlightEvent", gameObject);
        }

        // Do we unhighlight?
        if (isHighlighted && highlighters.Count == 0 )
        {
            isHighlighted = false;
            eventManager.SendMessage("buildingUnhighlightEvent", gameObject);
        }
    }

    // Highlight this grabable
    public void Highlight (Pointer pointer)
    {
        // Check if this is already highlighted 
        if (highlighters.Contains(pointer))
            return;
        highlighters.Add(pointer);
    }

    // Unhighlight this grabable
    public void Unhighlight(Pointer pointer)
    {
        if (highlighters.Contains(pointer))
            highlighters.Remove(pointer);
    }

    public void Grab(Manipulator manipulator)
    {
        if (grabber != null)
            Debug.LogError("Trying to grab an already grabbed item");
        grabber = manipulator;
        eventManager.SendMessage("buildingSelectEvent", gameObject);

        // Configure joint
        joint.connectedBody = manipulator.gameObject.GetComponent<Rigidbody>();
        joint.spring = springFactor;
        // Place joint anchor at intersection point. UPDATE: Doesn't feel great. 
        // if (manipulator.currentPointeeHandle.HasValue)
        //     joint.anchor = transform.InverseTransformPoint(manipulator.currentPointeeHandle.Value);
        // else
        //     Debug.LogWarning("Connect a manipulator that didn't have a pointer handle.");

        // DEBUG CODE: Place a marker at the joint anchor.  
        // marker = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        // marker.transform.position = transform.TransformPoint(joint.anchor);
        // marker.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
        // marker.transform.SetParent(manipulator.transform);
    }

    public void Release(Manipulator manipulator)
    {
        if (grabber != manipulator)
            Debug.LogError("A grabbed item is released by not its holder");
        grabber = null;
        
        eventManager.SendMessage("buildingUnselectEvent", gameObject);
        joint.connectedBody = null;
        joint.spring = 0;
    }
}
