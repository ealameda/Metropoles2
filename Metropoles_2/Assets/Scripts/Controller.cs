using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class Controller : MonoBehaviour
{
    public float speed;
    GameObject target;
    public GameObject leftHand;
    public GameObject rightHand;

    Vector2? mouseStartPosition;
    Quaternion startRotation;
    // Start is called before the first frame update
    void Start()
    {
        target = gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        // Should these be world space?
        if (Input.GetKey(KeyCode.A))
            Move(target.transform.right * -1);
        if (Input.GetKey(KeyCode.D))
            Move(target.transform.right);
        if (Input.GetKey(KeyCode.W))
            Move(target.transform.forward * -1);
        if (Input.GetKey(KeyCode.S))
            Move(target.transform.forward);
        if (Input.GetKey(KeyCode.Q))
            Move(target.transform.up);
        if (Input.GetKey(KeyCode.E))
            Move(target.transform.up * -1);

        if (Input.GetKey(KeyCode.Alpha1))
            target = gameObject;
        if (Input.GetKey(KeyCode.Alpha2))
            target = leftHand;
        if (Input.GetKey(KeyCode.Alpha3))
            target = rightHand;
                

        // Rotate code
        if (Input.GetMouseButton(0))
        {
            if (mouseStartPosition == null)
            {
                //Debug.Log("Started dragging.");
                mouseStartPosition = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
                startRotation = target.transform.rotation;
            }
            else
            {
                Vector2 offset = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
                offset = offset - mouseStartPosition.Value;
                float scale = Mathf.Min(Screen.width, Screen.height);
                //offset /= scale;
                //transform.rotation = startRotation;
                target.transform.Rotate(offset.y, -offset.x, 0);
                //Debug.Log("Rotating " + offset);
            }
        }
        else
        {
            //Debug.Log("Stopped dragging.");
            mouseStartPosition = null;
        }

        if (Input.GetMouseButtonDown(1))
        {
            Debug.Log("Felt a right click on " + target.gameObject.name);
            Manipulator hand = target.GetComponent<Manipulator>();
            if (hand != null)
                hand.GrabOrRelease();
        }

        if(Input.GetKeyDown(KeyCode.P))
        {
            Debug.Log("Got User Portal input");
            UserPortalDrawer hand = target.GetComponent<UserPortalDrawer>();
            if (hand != null)
                hand.StartOrStop();
        }
    }

    void Move(Vector3 offset)
    {
        float moveSpeed = speed;
        if (target == gameObject) {
            moveSpeed *= 3;
            // Hack to make WASD work as normal on the player, but on a 90 degree turn for wands
            if (Mathf.Abs(Vector3.Dot(offset, target.transform.forward)) > 0.5f)
                offset *= -1;
        }   
        target.transform.position += offset * moveSpeed;
    }
}
