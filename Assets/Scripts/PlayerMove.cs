using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

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

        //Used to make player Jump
        [SerializeField]
        public float jumpSpeed = 5.0f;

        public float gravity = 20.0f;
        private Vector3 moveDirection = Vector3.zero;
        public Camera Camera;

        [SerializeField]
        private float walkSpeed = 3f;

        [SerializeField]
        private float walkBackSpeed = 1.0f;

        [SerializeField]
        private float strafeSpeed = 2.75f;

        [SerializeField]
        private float rotateSpeed = 10.0f;

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

        GameObject playerCustomizeChildOBJ;

        public bool getPauseMenu = true;
        public string otherID = "";
        public int idInt = 0;

        public GameObject currrentSceneChageOBJ = null;
        public bool changeScenes = false;
        public bool enterCover = false;

        public Transform bestTarget = null;
        public string bestTargetName = "";
        public bool inCover = false;
        public string currentCover = "";
        public Vector3 closestPoint;
        public bool transitioning = false;
        public GameObject coverRegion = null;
        public bool stopLerp = false;

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

            //If youre dead don't allow movement
            if(animator.GetBool("IsDead") == true)
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


            if (other.tag == "CoverRegion")
            {
                inCover = true;
                enterCover = false;
                transitioning = false;
                return;
            }

            //If we collide with Player
            if (parentTag == "Player")
            {
                //If the obj we collided with is ourselves, return, else we collided with something important
                if (other.transform.GetComponentInParent<NetworkPlayer>().gameObject.name == PFC.myID)
                {
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

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, 0.5f);
            //Gizmos.DrawLine(NWP.local_chest.position, -transform.up);
            //Gizmos.DrawLine(NWP.local_head.transform.position, midpoint);
            Gizmos.DrawLine(NWP.local_head.transform.position, midpoint);


        }

        // Update is called once per frame
        private void Update()
        {
            midpoint = (NWP.local_head.transform.position + NWP.target.transform.position) / 2;

            if (health.hitPoints <= 0)
            {
                transform.GetChild(0).GetComponent<Animator>().SetBool("IsDead", true);
            }
            else
            {
                transform.GetChild(0).GetComponent<Animator>().SetBool("IsDead", false);
            }

            //This sends the player to the "bestTarget" IF were less than 0.5 away
            if (enterCover)
            {
                /*
                //If the distance between the player and Target are < 0.5 stop moving!
                if (transform.position == coverRegion.transform.position)//bestTarget != null && (closestPoint - transform.position).sqrMagnitude < 0.5
                {
                    inCover = true;
                    enterCover = false;
                    transitioning = false;
                }
                else
                {
                    transform.position = Vector3.MoveTowards(transform.position, closestPoint, 0.01f);
                    transitioning = true;
                }
                */

                transform.position = Vector3.MoveTowards(transform.position, closestPoint, 0.01f);
                transitioning = true;
            }

            //If were moving to the CoverRegion keep updating the regions location so it doesn't move with our player
            if (transitioning)
            {
                  coverRegion.transform.position = closestPoint;
            }

            if (inCover)
            {
                if ((closestPoint - transform.position).sqrMagnitude < 0.8)
                {
                    Debug.Log("Were In Cover");
                    
                }
                else//If we run out of cover to transition with "E"
                {
                    Debug.Log("Were out of Cover");
                    inCover = false;
                    currentCover = "";
                    
                    //If were currently running towards cover just return, we dont want to wipe cover target (bestTarget)
                    if (transitioning)
                    {
                        Debug.Log("Running to other cover!");
                        currentCover = "";
                        return;
                    }

                    //bestTarget could be used during a transition because the bestTarget is the current target the player is running towards
                    bestTarget = null;
                    bestTargetName = "";
                }
            }

            //Press E for INTERACT
            if (Input.GetKeyDown(KeyCode.E))
            {

                //Don't check for LOCAL cover if were in it.
                if (inCover == false)
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

                                //Dont let the player float upward to closest point of cover
                                closestPoint = bestTarget.transform.GetComponent<BoxCollider>().ClosestPoint((midpoint + bestTarget.transform.position)/2);
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
                        enterCover = true;
                    }
                    else
                    {
                        Debug.Log("There is no close Cover!");
                        currentCover = null;
                        enterCover = false;
                        inCover = false;
                    }
                }

                RaycastHit localhit;
                Vector3 hitPoint;
                Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
                Transform hitTransform;

                Physics.Raycast(ray, out localhit, midpoint.sqrMagnitude);
                


                hitTransform = playerShooting.FindClosestHitObject(ray, out hitPoint, "Cover", currentCover);

                if (hitTransform.transform != null)
                {
                    Debug.Log("Using: " + hitTransform.name + " for cover!");

                    closestPoint = hitTransform.transform.GetComponent<BoxCollider>().ClosestPoint(localhit.point);
                    closestPoint.y = 0;

                    coverRegion.transform.position = closestPoint;
                    enterCover = true;
                    currentCover = bestTargetName;
                }

                    /*
                    //Run to cover were looking at even if were in cover it doesnt matter!
                    //Casting ray from head to middle of target
                    RaycastHit localhit;

                    if (Physics.Raycast(NWP.local_head.transform.position, midpoint, out localhit, midpoint.sqrMagnitude) == true)
                    {  

                        Debug.Log("Cover Ray is hitting something!");

                        Ray ray = new Ray(NWP.local_head.transform.position, midpoint);

                        Vector3 hitPoint;

                        bestTarget = playerShooting.FindClosestHitObject(ray, out hitPoint, "Cover", currentCover);

                        //Dont let the player float upward to closest point of cover
                        closestPoint = bestTarget.transform.GetComponent<BoxCollider>().ClosestPoint(midpoint);
                        closestPoint.y = 0;

                        //Place cover region on ground to avoid getting too close to cover
                        coverRegion.transform.position = closestPoint;

                        bestTargetName = bestTarget.name;

                        enterCover = true;
                        currentCover = bestTargetName;
                    }
                    */




                    //If were interacting within a "SceneChange" taged Trigger
                    if (changeScenes && currrentSceneChageOBJ != null)
                {
                    //Access script of SceneChange OBJ and Send in int for scene selection. CHANGE TO GAME MODE 1
                    currrentSceneChageOBJ.GetComponentInChildren<ChangeScene>().ChangeSceneController(1);
                }
            }

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

        
            if (Physics.Raycast(NWP.local_chest.position, -transform.up, 0.05f) == true)
            {
                isGrounded = true;
            }
            else
            {
                isGrounded = false;
            }

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