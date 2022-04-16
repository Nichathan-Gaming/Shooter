using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BadGuyController : MonoBehaviour
{
    #region Variables
    //get the player
    private Transform player;

    //get the playerController
    private PlayerController playerController;

    //The distance that we move every update
    private float maxDist = .5f;

    [Header("Controls the speed that the badGuy moves towards the player at")]
    public float speed;
    #endregion Variables

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        playerController = player.GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        #region move the badGuy
        //move this
        transform.position = Vector3.MoveTowards(transform.position, player.position, (maxDist * speed));

        if(Vector3.Distance(transform.position, player.position) < (maxDist * speed))
        {
            //turns this off when touching the player
            transform.parent.gameObject.SetActive(false);

            //turns on Game Over screen
            playerController.GameOver();
        }
        #endregion move the badGuy
    }

    private void OnCollisionEnter(Collision collision)
    {
        //if we touched a shot, turn off bad guy
        if (collision.gameObject.tag.Equals("Shot"))
        {
            //tells the player that we are defeated
            playerController.BadGuyDefeated();

            //turn this off
            transform.parent.gameObject.SetActive(false);

            //turn off the shot, now that it is spent
            collision.transform.parent.gameObject.SetActive(false);
        }

        //if we touch the player
        if (collision.gameObject.tag.Equals("Player"))
        {
            //turns this off when touching the player
            transform.parent.gameObject.SetActive(false);

            //turns on Game Over screen
            playerController.GameOver();
        }
    }
}
