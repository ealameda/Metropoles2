using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    public float birthday;
    public float lifespan;
    public bool isSource;

    float shrinkTime = 1.0f;
    // Start is called before the first frame update
    void Start()
    {
        birthday = Time.time;
        Destroy(gameObject, lifespan + shrinkTime);
        
        Rigidbody body = gameObject.GetComponent<Rigidbody>();
        if (body == null)
        {
            body = gameObject.AddComponent<Rigidbody>();
            gameObject.AddComponent<BoxCollider>();
        }
        //body.isKinematic = true;
        body.useGravity = false;
        gameObject.layer = 17;
    }

    // Update is called once per frame
    void Update()
    {
        // Shrink a portal as it approaches its deathday.
        float timeShrinking = Time.time - birthday - lifespan;
        if ( timeShrinking > 0)
        {
            float t = (shrinkTime - timeShrinking) / shrinkTime;
            transform.localScale = new Vector3(t,t,t);
        }
    }
    
    private void OnTriggerEnter(Collider other) 
    {
        //Debug.Log("Collided with " + other.name);
        if (!isSource && other.GetComponent<Portalable>() != null)
            Destroy(other.gameObject);

    }
}
