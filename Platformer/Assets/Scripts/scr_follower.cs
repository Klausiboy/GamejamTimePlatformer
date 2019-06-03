using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr_follower : MonoBehaviour
{
    public Transform objToFollow;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
        transform.position = new Vector3(objToFollow.position.x, objToFollow.position.y, transform.position.z);
    }
}
