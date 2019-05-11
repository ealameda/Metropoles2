using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manipulator : Pointer
{

    // Are we grabbing anything currently? What is it?
    public Grabable grabbed;

    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    new void Update()
    {
        base.Update();
    }

    public void GrabOrRelease ()
    {
        Debug.Log(gameObject.name + " got a GrabOrRelease command");
        if (grabbed != null)
            Release();
        else
            Grab();
    }

    void Release()
    {
        if (grabbed == null)
            return;
        else 
        {
            grabbed.Release(this);
            grabbed = null;
            shouldPoint = true;
        }
    }

    void Grab()
    {
        Debug.Log("Grabbing");
        if (grabbed != null || currentPointee == null)
            return;
        else
        {
            grabbed = currentPointee;
            grabbed.Grab(this);
            shouldPoint = false;
        }
    }

}
