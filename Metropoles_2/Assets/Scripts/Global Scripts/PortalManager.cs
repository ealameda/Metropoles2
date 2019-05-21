using System.Collections;
using System.Collections.Generic;
using UnityEngine;

struct PortalSet 
{
    Portal origin;
    Portal destination;
    Portalable subject;
}

public class PortalManager : MonoBehaviour
{
    // What does a Portal look like? 
    public GameObject PortalPrefab;
    // What does a Portalable look like?
    public GameObject PortalablePrefab;

    // The minimum x, y, z for the portals to appear.
    public Vector3 minPosition;
    // The maximal x, y, z for the portals to appear.
    public Vector3 maxPosition;
    // Minimum height that objects fly through. 
    public float minHeight;

    // Slowest that the portalable object should go. 
    public float minSpeed;
    // Fastest that the portalable obejct should go. 
    public float maxSpeed;

    // How long to wait between generations? Will pick a random number between .5 and 1.5 this.
    public float timeBetweenGenerations;
    // How many portal/portalable sets can be active at once? Once a portalable is grabbed, it stops counting.
    public int maxNumberPortals;

    // When should the next portal set be generated?
    float timeStampofNextGeneration = 1.0f;

    // List of currently active portal sets. Does not include any grabbed objects. 
    List<PortalSet> portalSets;
    // Start is called before the first frame update
    void Start()
    {   
        portalSets = new List<PortalSet>();
        if (minHeight > minPosition.y)
            Debug.LogAssertion("minHeight must be lower than portals generate.");
        //Physics.IgnoreLayerCollision(17, 17, true);
    }

    // Update is called once per frame
    void Update()
    {
        TimerUpdate();
        
    }

    void TimerUpdate()
    {
        if (Time.time > timeStampofNextGeneration)
        {
            if (portalSets.Count < maxNumberPortals)
                GenerateNewPortalSet();
            timeStampofNextGeneration = Time.time + Random.Range(0.5f, 1.5f) * timeBetweenGenerations; 
        }
    }

    void GenerateNewPortalSet()
    {

        GameObject newSource = GameObject.Instantiate(PortalPrefab);
        GameObject newDestination = GameObject.Instantiate(PortalPrefab);
        GameObject newPortalable = GameObject.Instantiate(PortalablePrefab);
        
        Vector3[] trajectory = GenerateNewTrajectory();

        newSource.transform.position = trajectory[0];
        newSource.transform.LookAt(trajectory[1]);
        Portal src = newSource.AddComponent<Portal>();
        src.isSource = true;
        src.lifespan = trajectory[3].x;

        newDestination.transform.position = trajectory[1];
        newDestination.transform.LookAt(trajectory[0]);
        Portal dst = newDestination.AddComponent<Portal>();
        dst.isSource = false;
        dst.lifespan = trajectory[3].x;

        newPortalable.transform.position = trajectory[0];
        Portalable prt = newPortalable.AddComponent<Portalable>();
        
        Rigidbody body = newPortalable.AddComponent<Rigidbody>(); 
        body.velocity = trajectory[2];
        body.drag = 0;
        body.isKinematic = false;
        body.angularDrag = 0;
        body.mass = 1;
        body.useGravity = false;
        
    }

    Vector3[] GenerateNewTrajectory(int tries = 0)
    {
        if (tries > 100)
        {
            Debug.LogAssertion("Can't get a good trajectory, increasing speed");
            maxSpeed *= 2;
        }
        Vector3 origin = RandomInGenerationBox();
        Vector3 destination = RandomInGenerationBox();
        Vector3 difference = destination - origin;
        float velocity = Random.Range(minSpeed, maxSpeed);
        float time = difference.magnitude / velocity;

        Vector3 startTrajectory = difference.normalized * velocity;

        Vector3[] ret = {origin, destination, startTrajectory, new Vector3(time, time, time)};
        return ret;
    }

    Vector3 RandomInGenerationBox()
    {
        return new Vector3(Random.Range(minPosition.x, maxPosition.x), 
                           Random.Range(minPosition.y, maxPosition.y),
                           Random.Range(minPosition.z, maxPosition.z));
    }
}
