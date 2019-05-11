using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeSceneWhenTouched : MonoBehaviour
{
    // Start is called before the first frame update
    Collider myCollider;
    void Start()
    {
        myCollider = gameObject.GetComponent<Collider>();
        if (myCollider == null)
            Debug.LogError("Portal needs a collider");
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 currentHeadPosition = Camera.main.transform.position;
        if (myCollider.bounds.Contains(currentHeadPosition))
        {
            int nextLevel = (SceneManager.GetActiveScene().buildIndex + 1) % SceneManager.sceneCount;
            Debug.Log("Loading scene " + SceneManager.GetSceneAt(nextLevel).name);
            SceneManager.LoadScene(nextLevel);
        }
    }
}
