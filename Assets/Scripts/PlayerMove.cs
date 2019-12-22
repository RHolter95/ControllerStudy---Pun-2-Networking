using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

namespace UnderdogCity
{
    public class PlayerMove : MonoBehaviourPun, IPunObservable
    {
        public InventoryObject inventory;
        public PlayFabsController PFC = null;
        public Health health = null;

        public int clothesInt = 0;
        public FixedJoystick joystickOne;
        public FixedJoystick joystickTwo;
        public Texture2D crosshair;
        private bool showMessage = false;
        private CharacterController CharacterController;

        [SerializeField]
        public Animator animator;

        //Used to make player Jump
        [SerializeField]
        public float jumpSpeed = 5.0f;

        public float gravity = 20.0f;
        private Vector3 moveDirection = Vector3.zero;
        public Camera Camera;

        [SerializeField]
        private float walkSpeed = 10.0f;

        [SerializeField]
        private float walkBackSpeed = 0.5f;

        [SerializeField]
        private float strafeSpeed = 2f;

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

        [SerializeField]
        public NetworkPlayer NWplayer;

        public bool changeScenes = false;



        private void Start()
        {
            //Set aim up and aim down limits to 0 for aiming at center
            downTime = 0f;
            upTime = 0f;

            CameraPivot = GameObject.Find("CameraPivot");
            temp = GameObject.Find("Character@Idle");

            animator = temp.GetComponentInChildren<Animator>();

            playerCustomizeChildOBJ = animator.gameObject;

            chest = animator.GetBoneTransform(HumanBodyBones.Chest);
            chest = chest.transform.GetChild(0).transform;

            //Raycast isGrounded returns false @ spawn
            //so manually set it for first frame
            animator.SetBool("IsGrounded", true);

            var collider = GetComponent<Collider>();
        }

        // Start is called before the first frame update
        private void Awake()
        {

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

            pauseMenu = PFC.pauseMenu;

            //Delete this after inventory is setup
            inventory.Awake();

            canvas = transform.GetChild(1).gameObject;

            //This makes the player "look" like he is aiming the gun at the "target"
            //If you edit this you must edit it in NetworkPlayer!
            offset = new Vector3(10, 47.32f, 12);

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

            CharacterController = GetComponent<CharacterController>();
        }

        private void LateUpdate()
        {
            //Handles late physics chest rotation
            Camera.main.transform.LookAt(target.transform.position);
            chest.LookAt(target.transform.position);
            chest.rotation = chest.rotation * Quaternion.Euler(offset);
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
            if (other.tag == "ChangeScene")
            {
                if (other.transform.GetComponent<ChangeScene>())
                {
                    changeScenes = false;
                    return;
                }
            }
        }

        public void OnTriggerEnter(Collider other)
        {
            var parentTag = other.transform.root.tag;

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

        // Update is called once per frame
        private void Update()
        {

            if (health.hitPoints <= 0)
            {
                transform.GetChild(0).GetComponent<Animator>().SetBool("IsDead", true);
            }
            else
            {
                 transform.GetChild(0).GetComponent<Animator>().SetBool("IsDead", false);
            }
            
            //Press E for INTERACT
            if (Input.GetKeyDown(KeyCode.E))
            {
                Debug.Log("Calling Action");
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

            if (Physics.Raycast(transform.position, -Vector3.up, 0.05f) == true)
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

            transform.Translate(joystickOne.Horizontal * walkSpeed * Time.deltaTime, 0, joystickOne.Vertical * walkSpeed * Time.deltaTime);

            //If were walking backwards
            if (joystickOne.Direction.y < -.1f)
            {
                transform.Translate(joystickOne.Horizontal * walkBackSpeed * Time.deltaTime, 0, joystickOne.Vertical * walkBackSpeed * Time.deltaTime);
            }
            //Limit side to side strafe speed
            else if (joystickOne.Direction.x < -.1f && joystickOne.Direction.y < -.1f || joystickOne.Direction.x > .1f && joystickOne.Direction.y < -.1f)
            {
                transform.Translate(joystickOne.Horizontal * strafeSpeed * Time.deltaTime, 0, joystickOne.Vertical * walkBackSpeed * Time.deltaTime);
            }
            else
            {
                //Just move if not strafing or walking backwards
                transform.Translate(joystickOne.Horizontal * walkSpeed * Time.deltaTime, 0, joystickOne.Vertical * walkSpeed * Time.deltaTime);
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
                CharacterController.Move(dist / 2);
            }
        }
    }
}