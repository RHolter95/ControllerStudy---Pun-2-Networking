using Photon.Pun;
using UnityEngine;

    public class NetworkPlayer : MonoBehaviourPun, IPunObservable
    {

        public Animator animator;
        public float aimAngle = 0;
        protected Vector3 realPosition = Vector3.zero;
        protected Quaternion realRotation = Quaternion.identity;
        public Quaternion chestRotation = Quaternion.identity;
        public Vector3 targetPosition = Vector3.zero;
        public Vector3 offset = new Vector3(10,56,12);
        public Transform local_head, local_neck, local_spine, local_chest = null;
        public Quaternion server_head, server_neck, server_spine, server_chest = Quaternion.identity;
        private int currentBoneRate = 0;
        public Quaternion local_chestQuaternion;
        public Transform target;
        Quaternion rot;

        private void Awake()
        {
            animator = GetComponentInChildren<Animator>();
            target = transform.Find("CameraPivot/Target");
            rot = Quaternion.Euler(offset.x, offset.y, offset.z);
        }

        void Start()
        {
            if(target == null){
                Debug.Log("Network Player Couldn't find Target in child ");
            }

            if(animator == null){
                Debug.Log("Animator Empty in child");
            }

            local_chest = animator.GetBoneTransform(HumanBodyBones.Chest);
            local_head = animator.GetBoneTransform(HumanBodyBones.Head);
            local_neck = animator.GetBoneTransform(HumanBodyBones.Neck);
            local_spine = animator.GetBoneTransform(HumanBodyBones.Spine);

            if(local_chest == null){
                    Debug.Log("Network Player Couldn't find Chest");
            }
            if(local_head == null){
                    Debug.Log("Network Player Couldn't find Head");
            }
            if(local_neck == null){
                    Debug.Log("Network Player Couldn't find Neck");
            }
            if(local_spine == null){
                    Debug.Log("Network Player Couldn't find Spine");
            }
        }

        public void Update()
        {            
            if (this.photonView.IsMine)
            {
                Debug.Log("Local_Chest: " + local_chest.rotation);
                //Do Nothing -- Everything is already enabled
            }
            else
            {
                transform.position = Vector3.Lerp(transform.position, realPosition, 0.1f);
                transform.rotation = Quaternion.Lerp(transform.rotation, realRotation, 0.1f);
            }
            
        }
       public void LateUpdate()
        {            
            if (this.photonView.IsMine)
            {
                //local_chest.rotation = Quaternion.Lerp(local_chest.rotation, chestRotation, 0.1f);
                //Do Nothing -- Everything is already enabled
            }
            else
            {
                local_chest.LookAt(target);
                target.position = Vector3.Lerp(target.position, targetPosition, 0.1f);
                local_chest.rotation = local_chest.rotation * Quaternion.Euler(offset);
                local_chest.rotation = Quaternion.Lerp(local_chest.rotation, chestRotation, 0.1f);
            }
            
        }


        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                stream.SendNext(target.transform.position);
                stream.SendNext(local_chest.transform.rotation);
                stream.SendNext(transform.position);
                stream.SendNext(transform.rotation);
                stream.SendNext(animator.GetFloat("Speed"));                
                stream.SendNext(animator.GetBool("IsGrounded"));
                stream.SendNext(animator.GetFloat("JoyStickX"));
                stream.SendNext(animator.GetFloat("JoyStickY"));
                stream.SendNext(animator.GetFloat("AimAngle"));
                 
            }
            else
            {
                //This is someone else' player. we need to get their positions
                //as of a few milliseconds ago and update or version of that player
                targetPosition = (Vector3)stream.ReceiveNext();
                chestRotation = (Quaternion)stream.ReceiveNext();
                realPosition = (Vector3)stream.ReceiveNext();
                realRotation = (Quaternion)stream.ReceiveNext();
                animator.SetFloat("Speed",(float)stream.ReceiveNext());
                animator.SetBool("IsGrounded",(bool)stream.ReceiveNext());
                animator.SetFloat("JoyStickX",(float)stream.ReceiveNext());
                animator.SetFloat("JoyStickY",(float)stream.ReceiveNext());
                animator.SetFloat("AimAngle",(float)stream.ReceiveNext());
            }    
        }
    }
