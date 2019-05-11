using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portalable : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Collider col = gameObject.GetComponent<Collider>();
        if (col == null)
            col = gameObject.AddComponent<BoxCollider>();
        col.isTrigger = true;
        gameObject.layer = 17;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
