using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotController : MonoBehaviour
{
    /* Speed is how fast this shot moves, I keep it at 1 so it does not change
     * 
     * distance is used to see if we have gone the max distance
     * 
     * maxDistance is the maximum distance that this shot can travel
     */
    private float speed = 1,
        distance = 0,
        maxDistance = 100;

    // Update is called once per frame
    void Update()
    {
        //move the shot
        transform.localPosition += Vector3.forward * speed;

        //increase the distance moved
        distance++;

        if (distance > maxDistance)
        {
            //reset distance
            distance = 0;

            //turn off parent
            transform.parent.gameObject.SetActive(false);
        }
    }
}
