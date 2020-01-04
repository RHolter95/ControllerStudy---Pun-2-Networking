using Photon.Pun;
using UnityEngine;

namespace UnderdogCity
{
    public class PlayerMove : MonoBehaviourPun, IPunObservable
    {
        public PlayerShooting playerShooting = null;
        public InventoryObject inventory;
        public PlayFabsController PFC = null;
        public NetworkPlayer NWP = null;
        public Health health = null;
        public CharacterController charController = null;

        public int clothesInt = 0;
        public FixedJoystick joystickOne;
        public FixedJoystick joystickTwo;
        public Texture2D crosshair;
        private bool showMessage = false;

        [SerializeField]
        public Animator animator;

        [SerializeField]
        private Animation anim;

        private Vector3 playerPosition;

        public float gravity = 20.0f;
        private Vector3 moveDirection = Vector3.zero;
        public Camera Camera;

        [SerializeField]
        private float jumpSpeed = 0;

        [SerializeField]
        private float walkSpeed = 0;

        [SerializeField]
        private float walkBackSpeed = 0;

        [SerializeField]
        private float strafeSpeed = 0;

        [SerializeField]
        private float rotateSpeed = 0;

        [SerializeField]
        private bool isGrounded = true;

        private float verticalVelocity = 0;
        protected GameObject temp;
        public Transform chest;
        public GameObject target;
        public Vector3 offset;
        public GameObject CameraPivot;
        public float upTime;
        public float downTime;
        public GameObject canvas;
        public PhotonView itemPV;

        public GameObject pauseMenu = null;
        public GameObject pausePanel = null;
        public GameObject friendButton = null;
        public GameObject leaderBoardButton = null;

        private GameObject playerCustomizeChildOBJ;

        public bool getPauseMenu = true;
        public string otherID = "";
        public int idInt = 0;

        public GameObject currrentSceneChageOBJ = null;
        public bool changeScenes = false;
        public bool enterCover = false;

        //COVER SYSTEM VARS
        public Transform bestTarget = null;

        public string bestTargetName = "";
        public bool inCover = false;
        public string currentCover = "";
        public GameObject currentCoverOBJ = null;
        public Vector3 closestPoint;
        public bool transitioning = false;
        public GameObject coverRegion = null;
        public bool stopLerp = false;
        public Vector3 coverMin;
        public Vector3 coverMax;
        public Vector3[] verts = new Vector3[8];        // Array that will contain the BOX Collider Vertices

        //Midpoint between player and target GO. This is to run to closeby cover!
        public Vector3 midpoint;

        // Start is called before the first frame update
        private void Start()
        {
        }

        private void Awake()
        {
            //Set aim up and aim down limits to 0 for aiming at center
            downTime = 0f;
            upTime = 0f;

            //Actual player animatin controller
            animator = GetComponentInChildren<Animator>();
            if (animator == null)
            {
                Debug.Log("Couldn't find animator Component && playerCustomizeChildOBJ && chest");
            }
            else
            {
                animator.SetBool("IsGrounded", true);
                playerCustomizeChildOBJ = animator.gameObject;
                chest = animator.GetBoneTransform(HumanBodyBones.Chest);
                chest = chest.transform.GetChild(0).transform;
            }

            CameraPivot = GameObject.Find("CameraPivot");
            if (CameraPivot == null)
            {
                Debug.Log("Couldn't find CameraPivot GameObject");
            }

            temp = GameObject.Find("Character@Idle");
            if (temp == null)
            {
                Debug.Log("Couldn't find Character@Idle GameObject");
            }

            coverRegion = GameObject.Find("CoverRegion");
            if (coverRegion == null)
            {
                Debug.Log("Couldn't find coverRegion GameObject");
            }

            playerShooting = transform.GetComponent<PlayerShooting>();
            if (playerShooting == null)
            {
                Debug.Log("Couldn't find playerShooting Component");
            }

            charController = transform.GetComponent<CharacterController>();
            if (charController == null)
            {
                Debug.Log("Couldn't find charController Component");
            }

            NWP = transform.GetComponent<NetworkPlayer>();
            if (NWP == null)
            {
                Debug.Log("Couldn't find NetworkPlayer Component");
            }

            health = transform.GetComponent<Health>();
            if (health == null)
            {
                Debug.Log("Couldn't find Health Component");
            }

            PFC = GameObject.Find("NetworkController").GetComponent<PlayFabsController>();
            if (PFC == null)
            {
                Debug.Log("Couldn't find NetworkController/PlayFabsController");
            }

            //Find the joystick objects
            GameObject tempJoystickOne = canvas.gameObject.transform.GetChild(0).gameObject;
            GameObject tempJoystickOTwo = canvas.gameObject.transform.GetChild(1).gameObject;

            //Assign Joysticks to script variables [Left,Right]
            if (tempJoystickOne)
            {
                joystickOne = tempJoystickOne.GetComponent<FixedJoystick>();
            }
            else
            {
                Debug.Log("No Left JoyStick");
            }

            if (tempJoystickOTwo)
            {
                joystickTwo = tempJoystickOTwo.GetComponent<FixedJoystick>();
            }
            else
            {
                Debug.Log("No Right JoyStick");
            }
        }

        //Controls bone animation after anim is playing.
        private void LateUpdate()
        {
            //Handles late physics chest rotation
            Camera.main.transform.LookAt(target.transform.position);
            chest.LookAt(target.transform.position);
            chest.rotation = chest.rotation * Quaternion.Euler(offset);

            //If our core movement speed values are wrong then LateUpdate() them, Something is updaing these values somewhere else
            if (jumpSpeed != 5 || walkSpeed != 2 || walkBackSpeed != 3 || strafeSpeed != 2 || rotateSpeed != 80)
            {
                Debug.Log("Updating movment speeds");
                jumpSpeed = 5;
                walkSpeed = 2;
                walkBackSpeed = 3;
                strafeSpeed = 2;
                rotateSpeed = 80;
            }

            //If were running to cover lets play the animation
            if (transitioning)
            {
                animator.Play("CoverTransition", 0);
            }

            //If youre dead don't allow movement but allow camera rotate
            if (animator.GetBool("IsDead") == true)
            {
                transform.root.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z);
                transform.root.position = new Vector3(transform.root.position.x, transform.root.position.y, transform.root.position.z);
            }
        }

        private void AdjustAimAngle(float verticalTwo)
        {
            Camera myCamera = this.GetComponentInChildren<Camera>();

            if (myCamera == null)
            {
                Debug.Log("No Main Camera!");
                return;
            }
            float aimAngle = 0;

            if (verticalTwo > 0.05f && upTime < 1.0f)
            {
                upTime += Time.deltaTime;
                downTime -= Time.deltaTime;
                CameraPivot.transform.Rotate(Vector3.left * Time.deltaTime * 50f);
            }
            if (verticalTwo < -0.05f && downTime < 1.0f)
            {
                downTime += Time.deltaTime;
                upTime -= Time.deltaTime;
                CameraPivot.transform.Rotate(Vector3.right * Time.deltaTime * 50f);
            }

            if (myCamera.transform.rotation.eulerAngles.x <= 90f)
            {
                //We are looking down
                aimAngle = -myCamera.transform.rotation.eulerAngles.x;
            }
            else
            {
                aimAngle = 360 - myCamera.transform.rotation.eulerAngles.x;
            }

            animator.SetFloat("AimAngle", aimAngle);
        }

        public void OnTriggerExit(Collider other)
        {
            var parentTag = other.transform.root.tag;

            if (other.tag == "CoverRegion")
            {
                inCover = false;
                return;
            }

            if (parentTag == "ChangeScene")
            {
                if (other.transform.GetComponent<ChangeScene>())
                {
                    currrentSceneChageOBJ = null;
                    changeScenes = false;
                    return;
                }
            }
        }

        public void OnTriggerEnter(Collider other)
        {
            var parentTag = other.transform.root.tag;

            /*
            if (other.tag == "CoverRegion")
            {
                inCover = true;
                enterCover = false;
                transitioning = false;
                animator.SetBool("IsTransitioning", transitioning);
                return;
            }
            */

            //If we collide with Player
            if (parentTag == "Player")
            {
                //If the obj we collided with is ourselves, return, else we collided with something important
                if (other.transform.GetComponentInParent<NetworkPlayer>().gameObject.name == PFC.myID)
                {
                    if (transitioning)
                    {
                        inCover = true;
                        enterCover = false;
                        transitioning = false;
                        animator.SetBool("IsTransitioning", transitioning);
                        return;
                    }

                    Debug.Log("Collided with self");
                    return;
                }
                else
                {
                    var player = other.GetComponentInParent<NetworkPlayer>().gameObject;
                    Debug.Log("Collided with another player with ID: " + player.name + ", set bool value here for social menu");
                    //Setup Pause Menu inside player model prefabs instead of GO inside hirearchy and re-assign links and store ID of player were infront of in our panel for easy Add Req
                    //PFC.friendListPanel.transform.GetChild(3).GetComponent<Text>().text = player.name;
                    return;
                }
            }

            //If we collide with ChangeScene OBJ
            if (parentTag == "ChangeScene")
            {
                if (other.transform.GetComponent<ChangeScene>())
                {
                    Debug.Log("Press E To Change Scene");
                    currrentSceneChageOBJ = other.gameObject;
                    changeScenes = true;
                    return;
                }
            }

            if (parentTag == "Item")
            {
                var item = other.GetComponent<Item>();

                itemPV = item.GetComponent<PhotonView>();
                if (itemPV == null)
                {
                    Debug.Log("PhotonView of item is missing!");
                    return;
                }
                else
                {
                    inventory.AddItem(item.item, 1, item.item.image);
                    if (itemPV.InstantiationId != 0)
                    {
                        //This deletes INSTANTIATED items on the Network
                        PhotonNetwork.Destroy(other.gameObject);
                        return;
                    }
                    if (itemPV.InstantiationId == 0)
                    {
                        Destroy(other.gameObject);
                        return;
                    }
                }
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            Debug.Log("Collided");
            foreach (ContactPoint contact in collision.contacts)
            {
                //If we collide with our current bestTarget, stop moving
                if (contact.thisCollider.gameObject.name == bestTargetName)
                {
                    Debug.Log("Collided with cover!");
                    inCover = true;
                    enterCover = false;
                    transitioning = false;
                    animator.SetBool("IsTransitioning", transitioning);
                    return;
                }
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, 0.5f);
            Gizmos.DrawLine(NWP.local_head.transform.position, midpoint);
            Gizmos.DrawSphere(closestPoint, 0.25f);
            Gizmos.DrawWireSphere(midpoint, 0.25f);

            Gizmos.DrawSphere(verts[0], 0.25f);
            Gizmos.DrawSphere(verts[1], 0.25f);
            Gizmos.DrawSphere(verts[2], 0.25f);
            Gizmos.DrawSphere(verts[3], 0.25f);
            Gizmos.DrawSphere(verts[4], 0.25f);
            Gizmos.DrawSphere(verts[5], 0.25f);
            Gizmos.DrawSphere(verts[6], 0.25f);
            Gizmos.DrawSphere(verts[7], 0.25f);
        }

        // Update is called once per frame
        private void Update()
        {
            Debug.Log("Current Cover: " + currentCover);

            playerPosition = transform.position;

            #region HealthZeroOnDeath()

            if (health.hitPoints <= 0)
            {
                transform.GetChild(0).GetComponent<Animator>().SetBool("IsDead", true);
            }
            else
            {
                transform.GetChild(0).GetComponent<Animator>().SetBool("IsDead", false);
            }

            #endregion HealthZeroOnDeath()

            #region CoverSystem

            midpoint = (NWP.local_head.transform.position + NWP.target.transform.position) / 2;

            //This sends the player to the "bestTarget" IF were less than 0.5 away
            if (enterCover)
            {
                transform.position = Vector3.MoveTowards(transform.position, new Vector3(closestPoint.x, 0, closestPoint.z), 3f * Time.deltaTime);
                transitioning = true;
            }

            //If were running to the CoverRegion keep updating the regions location so it doesn't move with our player
            if (transitioning)
            {
                Debug.Log("Running to cover!");
                //If you try to run somewhere else while transitioning stop running to cover
                if (animator.GetFloat("JoyStickX") > 0.1 || animator.GetFloat("JoyStickX") < -0.1 || animator.GetFloat("JoyStickY") > 0.1 || animator.GetFloat("JoyStickY") < -0.1)
                {
                    enterCover = false;
                    transitioning = false;
                    currentCover = "";
                    currentCoverOBJ = null;
                }

                animator.SetBool("IsTransitioning", transitioning);
                coverRegion.transform.position = closestPoint;
            }

            //If were in cover great! else, were out of cover and possibly transitioning to more cover
            if (inCover)
            {
                if ((closestPoint - transform.position).sqrMagnitude < 0.8)
                {
                    Debug.Log("Were In Cover");
                    inCover = true;
                    bestTarget = null;
                    bestTargetName = "";
                    animator.SetBool("InCover", inCover);
                }
                else//If we run out of cover in general
                {
                    Debug.Log("Were out of Cover");
                    inCover = false;
                    animator.SetBool("InCover", inCover);
                    currentCover = "";
                    currentCoverOBJ = null;

                    // to transition with "E" to another wall
                    //If were currently running towards cover just return, we dont want to wipe cover target (bestTarget)
                    if (transitioning)
                    {
                        Debug.Log("Running to other cover: " + bestTarget.name);
                        currentCover = "";
                        currentCoverOBJ = null;
                        return;
                    }

                    //If were out of cover and not running to any other cover just reset the the rest of the system of cover
                    bestTarget = null;
                    bestTargetName = "";
                }
            }

            #endregion CoverSystem

            #region Interact

            //Press E for INTERACT
            if (Input.GetKeyDown(KeyCode.E))
            {
                //Wipe old cover corners
                verts[0] = verts[1] = verts[2] = verts[3] = new Vector3(0, 0, 0);

                /*
                //Don't check for LOCAL cover if were in it.
                if(inCover == false)
                {
                    float closestDistanceSqr = Mathf.Infinity;
                    Vector3 currentPosition = transform.position;

                    //Get colliders around player with tag "Cover", then store player -> OBJ dist. If tempDistance < distance replace it.
                    //The radius of 0.5f requires us to be ALONG A WALL
                    Collider[] hitColliders = Physics.OverlapSphere(transform.position, 0.5f);

                    foreach (var potentialTarget in hitColliders)
                    {
                        if (potentialTarget.tag == "Cover")
                        {
                            Vector3 directionToTarget = potentialTarget.transform.position - currentPosition;
                            float dSqrToTarget = directionToTarget.sqrMagnitude;
                            if (dSqrToTarget < closestDistanceSqr)
                            {
                                closestDistanceSqr = dSqrToTarget;
                                bestTarget = potentialTarget.transform;

                                //Gets the four corners of the collider
                                BoxCollider b = bestTarget.GetComponent<BoxCollider>();

                                verts[0] = b.center + new Vector3(b.size.x, -b.size.y, b.size.z) * 0.5f;
                                verts[1] = b.center + new Vector3(-b.size.x, -b.size.y, b.size.z) * 0.5f;
                                verts[2] = b.center + new Vector3(-b.size.x, -b.size.y, -b.size.z) * 0.5f;
                                verts[3] = b.center + new Vector3(b.size.x, -b.size.y, -b.size.z) * 0.5f;

                                //Dont let the player float upward to closest point of cover
                                closestPoint = bestTarget.transform.GetComponent<BoxCollider>().ClosestPoint((midpoint + bestTarget.transform.position) / 2);
                                closestPoint.y = 0;

                                //Place cover region on ground to avoid getting too close to cover
                                coverRegion.transform.position = closestPoint;

                                Debug.Log("Best Target: " + bestTarget.name + " at distance: " + closestPoint.sqrMagnitude);
                            }
                        }
                    }
                    //If one of the colliders in the overlapsphere had the tag "cover" we get its name and set enter cover true to start moveTowards()
                    if (bestTarget != null)
                    {
                        //Get name of current cover so we wont try to get this cover at same position when ray casting out for transition to another cover
                        currentCover = bestTarget.name;
                        currentCoverOBJ = bestTarget.gameObject;
                        enterCover = true;
                        castRay = false;
                    }
                    else
                    {
                        Debug.Log("There is no close Cover!");
                        currentCover = null;
                        enterCover = false;
                        inCover = false;
                        //Since out local check returned null & were not inCover we fire off a ray
                        castRay = true;
                    }
                }
                */

                    RaycastHit localhit;
                    Vector3 hitPoint;//Camera.main.transform.position
                    Ray ray = new Ray(NWP.local_head.position, midpoint);
                
                    Transform hitTransform;

                    Physics.Raycast(ray, out localhit, midpoint.sqrMagnitude);
                
                    hitTransform = playerShooting.FindClosestHitObject(ray, out hitPoint, "Cover", currentCover);

                    if (localhit.transform != null)
                    {
                        Debug.Log("Using: " + localhit.transform.name + " for cover!");

                        bestTarget = localhit.transform;
                        bestTargetName = localhit.transform.name;

                        //BoxCollider b = hitTransform.transform.GetComponent<BoxCollider>();
                        /*
                        verts[0] = b.center + new Vector3(-b.size.x, -b.size.y, -b.size.z) * 0.5f;
                        verts[1] = b.center + new Vector3(b.size.x, -b.size.y, -b.size.z) * 0.5f;
                        verts[2] = b.center + new Vector3(b.size.x, -b.size.y, b.size.z) * 0.5f;
                        verts[3] = b.center + new Vector3(-b.size.x, -b.size.y, b.size.z) * 0.5f;
                        verts[4] = b.center + new Vector3(-b.size.x, -b.size.y, -b.size.z) * 0.5f;
                        verts[5] = b.center + new Vector3(b.size.x, -b.size.y, -b.size.z) * 0.5f;
                        verts[6] = b.center + new Vector3(b.size.x, -b.size.y, b.size.z) * 0.5f;
                        verts[7] = b.center + new Vector3(-b.size.x, -b.size.y, b.size.z) * 0.5f;
                        */
                        //closestPoint = localhit.point;

                        closestPoint = bestTarget.transform.GetComponent<BoxCollider>().ClosestPoint(midpoint);
                        closestPoint.y = 0;

                        if ((transform.position - closestPoint).sqrMagnitude > 2)
                        {
                            Debug.Log("Too far away");
                            return;
                        }

                        coverRegion.transform.position = closestPoint;
                        enterCover = true;
                        currentCover = bestTargetName;
                        currentCoverOBJ = bestTarget.gameObject;
                    }
                    else
                    {
                        return;
                    }
                

                //If were interacting within a "SceneChange" taged Trigger
                if (changeScenes && currrentSceneChageOBJ != null)
                {
                    //Access script of SceneChange OBJ and Send in int for scene selection. CHANGE TO GAME MODE 1
                    currrentSceneChageOBJ.GetComponentInChildren<ChangeScene>().ChangeSceneController(1);
                }
            }

            #endregion Interact

            #region PauseMenu()

            //Press Esc for PAUSE MENU
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (pauseMenu.activeInHierarchy)
                {
                    pauseMenu.SetActive(false);
                }
                else
                {
                    pauseMenu.SetActive(true);
                }
            }

            #endregion PauseMenu()

            #region IsGroundedCheck()

            if (Physics.Raycast(NWP.local_chest.position, -transform.up, 0.05f) == true)
            {
                isGrounded = true;
            }
            else
            {
                isGrounded = false;
            }

            #endregion IsGroundedCheck()

            //Assign joysticks to variables on canvas
            var horizontalOne = joystickOne.Horizontal;
            var verticalOne = joystickOne.Vertical;
            var horizontalTwo = joystickTwo.Horizontal;
            var verticalTwo = joystickTwo.Vertical;

            //Constantly calculates the
            AdjustAimAngle(verticalTwo);

            var moveDirection = new Vector3(horizontalOne, 0f, verticalOne);
            var lookDirection = new Vector3(0f, verticalTwo, 0f);

            if (moveDirection.magnitude > 0.1f)
            {
                moveDirection = moveDirection.normalized;
            }

            //This translates location from joystick Left
            transform.Rotate(0, moveDirection.y, 0, Space.Self);
            transform.Rotate(joystickTwo.Horizontal * Vector3.up * Time.deltaTime * rotateSpeed);

            //Sets Speed and sets anim joystick vars for mixed animations
            animator.SetFloat("Speed", moveDirection.magnitude);
            animator.SetFloat("JoyStickX", joystickOne.Horizontal);
            animator.SetFloat("JoyStickY", joystickOne.Vertical);

            if (isGrounded && Input.GetButtonDown("Jump"))
            {
                verticalVelocity = jumpSpeed;
            }

            //If were alive we can move
            if (animator.GetBool("IsDead") == false)
            {
                transform.Translate(joystickOne.Horizontal * walkSpeed * Time.deltaTime, 0, joystickOne.Vertical * walkSpeed * Time.deltaTime);

                //If were walking backwards
                if (joystickOne.Direction.y < -.1f)
                {
                    charController.SimpleMove(moveDirection * walkBackSpeed * Time.deltaTime);
                    //rigidBody.MovePosition(joystickOne.Horizontal * walkBackSpeed * Time.deltaTime, 0, joystickOne.Vertical * walkBackSpeed * Time.deltaTime);
                    //transform.Translate(joystickOne.Horizontal * walkBackSpeed * Time.deltaTime, 0, joystickOne.Vertical * walkBackSpeed * Time.deltaTime);
                }
                //Limit side to side strafe speed
                else if (joystickOne.Direction.x < -.1f && joystickOne.Direction.y < -.1f || joystickOne.Direction.x > .1f && joystickOne.Direction.y < -.1f)
                {
                    charController.SimpleMove(moveDirection * strafeSpeed * Time.deltaTime);
                    //transform.Translate(joystickOne.Horizontal * strafeSpeed * Time.deltaTime, 0, joystickOne.Vertical * walkBackSpeed * Time.deltaTime);
                }
                else
                {
                    //Just move if not strafing or walking backwards
                    charController.SimpleMove(moveDirection * walkSpeed * Time.deltaTime);
                    //transform.Translate(joystickOne.Horizontal * walkSpeed * Time.deltaTime, 0, joystickOne.Vertical * walkSpeed * Time.deltaTime);
                }
            }
        }

        private void OnGUI()
        {
            GUI.DrawTexture(new Rect(Screen.width / 2 - (crosshair.width * 0.5f),
                                       Screen.height / 2 - (crosshair.height * 0.5f),
                                     crosshair.width, crosshair.height), crosshair);

            //Use this for showing weapon Image
            if (showMessage == true)
            {
                //GUI.Label(new Rect(Screen.width/2-100, Screen.height - 375 , 200, 200), "Hello World!");
                //GUI.Label(new Rect(Screen.width/2-100, Screen.height - 375 , 500, 250), textureToDisplay);
            }
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            throw new System.NotImplementedException();
        }

        private void FixedUpdate()
        {
            Vector3 dist = moveDirection * walkSpeed * Time.deltaTime;
            if (isGrounded && verticalVelocity < 0)
            {
                animator.SetBool("IsGrounded", true);
                verticalVelocity = Physics.gravity.y * Time.deltaTime;
            }
            else
            {
                if (Mathf.Abs(verticalVelocity) > jumpSpeed * 100f)
                {
                    animator.SetBool("IsGrounded", false);
                }

                verticalVelocity += Physics.gravity.y * Time.deltaTime;
            }

            dist.y = verticalVelocity * Time.deltaTime;

            if (moveDirection.magnitude > 0.1f)
            {
                charController.Move(dist / 2);
            }
        }
    }
}