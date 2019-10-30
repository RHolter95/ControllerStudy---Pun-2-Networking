using Photon.Pun;
using UnityEngine;

    public class NetworkPlayer : MonoBehaviourPun, IPunObservable
    {

        protected Animator animator;
        protected Vector3 realPosition = Vector3.zero;
        protected Quaternion realRotation = Quaternion.identity;
        protected float realSpeed;
        private void Awake()
        {
        }

        void Start()
        {
            animator = GetComponentInChildren<Animator>();
            if(animator == null){
                Debug.Log("Animator Empty");
            }
        }

        public void Update()
        {
            if (this.photonView.IsMine)
            {
                //Do Nothing -- Everything is already enabled
            }
            else
            {
                transform.position = Vector3.Lerp(transform.position, realPosition, 0.1f);
                transform.rotation = Quaternion.Lerp(transform.rotation, realRotation, 0.1f);
                Mathf.Lerp(animator.GetFloat("Speed"), realSpeed, 0.1f);
            }
            
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                stream.SendNext(transform.position);
                stream.SendNext(transform.rotation);
                stream.SendNext(animator.GetFloat("Speed"));                
                stream.SendNext(animator.GetBool("IsGrounded"));
                stream.SendNext(animator.GetFloat("JoyStickX"));
                stream.SendNext(animator.GetFloat("JoyStickY"));


                
            }
            else
            {
                //This is someone else' player. we need to get their positions
                //as of a few milliseconds ago and update or version of that player
                realPosition = (Vector3)stream.ReceiveNext();
                realRotation = (Quaternion)stream.ReceiveNext();
                animator.SetFloat("Speed",(float)stream.ReceiveNext());
                animator.SetBool("IsGrounded",(bool)stream.ReceiveNext());
                animator.SetFloat("JoyStickX",(float)stream.ReceiveNext());
                animator.SetFloat("JoyStickY",(float)stream.ReceiveNext());


            }
        }
    }
