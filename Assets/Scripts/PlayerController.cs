using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    #region variables
    [Header("Where we instantiate the shots")]
    public Transform barrelEnd;

    [Header("The shot that we shoot")]
    public Transform shotPrefab;

    //All of the shots that we instantiate so we can reuse them when they are innactive and save memory.
    private List<Transform> shots = new List<Transform>();

    [Header("The bad guy that we instantiate")]
    public GameObject badGuyPrefab;

    //All of the bad guys that we attack the player with
    private List<Transform> badGuys = new List<Transform>();

    [Header("The screen that displays when we loose.")]
    public GameObject gameOverScreen;

    [Header("The place where we display the score")]
    public Text scoreText;

    /*Score is to track the player score as they eliminate badGuys
     * 
     * nextBadGuyBoost will add 1 to the maxBadGuys when score is greater
     * 
     * delay will track time before we try to play another badGuy
     * 
     * maxDelay is the time between delays
     * 
     * maxBadGuys is the maximum number of bad guys that can be active at once
     */
    private int score = 0,
        nextBadGuyBoost = 5,
        delay = 0,
        maxDelay = 50,
        maxBadGuys = 1;

    //tells us if the game is active or not
    private bool isGameActive = true;

    //controls the players rotation as they look around.
    private float rotation = 0,
        movementSpeed = 10,
        maxRotation = 45;
    #endregion variables

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        //if game is active
        if (isGameActive)
        {
            //if player clicks left mouse button or taps on screen, shoot shot
            if (Input.GetMouseButtonDown(0))
            {
                //shoot a shot
                Shoot();
            }

            #region rotate the player
            //tells us if the mouse was moved sideways
            float mouseX = Input.GetAxisRaw("Mouse X");

            //the left-right rotation of this object
            float eulerY = transform.eulerAngles.y;

            //Get rotation as it is shown in the editor 180-0-(-180)-180
            float maxEulerY = (eulerY > 180) ? (eulerY - 360) : eulerY;

            //mouse moves right || mouse moves left
            if((mouseX>0 && maxEulerY < maxRotation) || (mouseX < 0 && maxEulerY > -maxRotation))
            {
                rotation = eulerY + (movementSpeed * mouseX);
                transform.localRotation = Quaternion.Euler(0, rotation, 0);
            }
            #endregion rotate the player

            //check if we need more badGuys
            delay++;
            if(delay > maxDelay)
            {
                //Check bad guys
                CheckBadGuys();
            }
        }
    }

    #region handle badGuys
    /**
     * Checks the list of shots for an innactive shot
     *  if found
     *      activate shot and shoot it
     *  else, 
     *      instantiate a new shot, add it to the list, and shoot it.
     */
    private void Shoot()
    {
        //make sure that nothing is null
        if (shots != null)
        {
            //look through all shots
            foreach(Transform shot in shots)
            {
                //if the shot is not active 
                if (!shot.gameObject.activeInHierarchy)
                {
                    //move shot to barrelEnd, rotate and activate
                    
                    //activate
                    shot.gameObject.SetActive(true);

                    //rotate
                    shot.localRotation = Quaternion.Euler(0, rotation, 0);

                    //move to barrelEnd
                    shot.GetChild(0).localPosition = Vector3.zero;

                    //stop looking, do not create one
                    return;
                }
            }
        }

        //a shot was not found, create one.
        CreateShot();

        //the method that creates shots
        void CreateShot()
        {
            //the created shot
            Transform shot = Instantiate(shotPrefab);

            //move into position
            shot.position = barrelEnd.position;

            //rotate
            shot.localRotation = Quaternion.Euler(0, rotation, 0);

            //add to shots
            shots.Add(shot);
        }
    }

    /**
     * If we need to make a new bad guy
     *  look through the badGuys list for an innactive bad guy
     *      if found
     *          activate, movem and randomize variables
     *      else
     *          create a new badGuy
     */
    private void CheckBadGuys()
    {
        //if we need more badGuys
        if(badGuys.Count < maxBadGuys)
        {
            //Create a bad guy
            CreateBadGuy();
        }

        //see if we should turn on a defeated bad guy
        for(int i = 0; i < badGuys.Count; i++)
        {
            //used to protect us on game reset
            if (i > maxBadGuys) break;

            //if we can find an innactive badGuy
            if (!badGuys[i].gameObject.activeInHierarchy)
            {
                //turn on the bad guy
                badGuys[i].gameObject.SetActive(true);

                //set his variables
                AssignBadGuy(badGuys[i]);
            }
        }

        //create a new bad guy
        void CreateBadGuy()
        {
            //create a new bad guy
            Transform badGuy = Instantiate(badGuyPrefab).transform;

            //add to the list
            badGuys.Add(badGuy);

            //move into place and set variables
            AssignBadGuy(badGuy);
        }

        /**
         * Moves the bad guy to a random x and z location with a random speed 
         */
        void AssignBadGuy(Transform badGuy)
        {
            //create a random x position (left-right)
            float randX = Random.Range(0, 100);

            //create a random z position (distance from player)
            float randZ = Random.Range(75, 150);

            //create a random speed for the badGuy
            float randSpeed = Random.Range(0.5f, 1);

            //set the position
            badGuy.position = new Vector3(randX, 0, randZ);

            //return the child to its parent
            badGuy.GetChild(0).localPosition = Vector3.zero;

            //set the speed
            badGuy.GetChild(0).GetComponent<BadGuyController>().speed = randSpeed;
        }
    }

    /**
     * calls when a shot touches a badGuy
     * 
     * increases score, updates score text and may boost badGuyCount
     */
    public void BadGuyDefeated()
    {
        score++;

        if (score > nextBadGuyBoost)
        {
            maxBadGuys++;
            nextBadGuyBoost *= 2;
        }

        scoreText.text = "" + score;
    }
    #endregion handle badGuys

    #region control methods
    /**
     * Closes the application
     * 
     * --Does not work in testing
     */
    public void Quit()
    {
        //closes program in production (or minimizes screen on android)
        Application.Quit();
    }

    /**
     * Resets all of the variables and starts the game again.
     * 
     * Does not clear lists to save on processing power
     */
    public void Restart()
    {
        //hide the cursor
        Cursor.lockState = CursorLockMode.Locked;

        //turn on the game
        isGameActive = true;

        //turn off game over screen
        gameOverScreen.SetActive(false);

        //reset score
        score = 0;

        //reset score text
        scoreText.text = "" + score;

        //reset badGuyBoost
        nextBadGuyBoost = 5;

        //reset the maximum number of bad guys
        maxBadGuys = 1;
    }

    /**
     * Called when the game is over
     */
    public void GameOver()
    {
        //release the cursor
        Cursor.lockState = CursorLockMode.None;

        //turn off the game
        isGameActive = false;

        //turn on game over screen
        gameOverScreen.SetActive(true);
    }
    #endregion control methods
}
