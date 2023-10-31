using System;
using UnityEngine;

namespace ElectorchStrauss.ProceduralTerrainSystem.Scripts
{
    public class PlayerController : MonoBehaviour
    {
        private Animator animator;
        private GameController _gameController;
        private Camera m_Camera;
        public float camRotationSpeed = 4f;
        Quaternion cameraRotation,playerRotation;
        private Vector2 angleX, angleY;
        private Vector3 hitPoint, xAxis, zAxis;
        private Canvas menuCanvas;

        Rigidbody body;
        Vector3 velocity, desiredVelocity;
        [SerializeField, Range(0f, 100f)] float maxSpeed = 10f, maxClimbSpeed = 6f, maxShiftSpeed = 15f, maxClimbShiftSpeed = 9f;

        [SerializeField, Range(0f, 100f)] float maxAcceleration = 20f,
            maxAirAcceleration = 25f,
            maxClimbAcceleration = 20f;


        [SerializeField, Range(0f, 90f)] float maxGroundAngle = 25f, maxStairsAngle = 75f;
        [SerializeField, Range(90f, 180f)] float maxClimbAngle = 140f;

        float minGroundDotProduct, minStairsDotProduct, minClimbDotProduct;
        int groundContactCount, steepContactCount, climbContactCount;

        Vector3 contactNormal, steepNormal, climbNormal;
        int stepsSinceLastGrounded;
        private Vector3 gravity;
        bool OnGround => groundContactCount > 0;
        bool OnSteep => steepContactCount > 0;
        bool Climbing => climbContactCount > 0;
        public Vector3 upAxis, rightAxis, forwardAxis;

        [SerializeField] LayerMask groundMask = -1,steepMask = -1, climbMask = -1;

        [SerializeField, Range(0f, 10f)] float jumpHeight = 2f;
        int jumpPhase;
        [SerializeField, Range(0, 5)] int maxAirJumps = 1;
        bool desiredJump;
        int stepsSinceLastJump;

        private float verticalRaw,horizontalRaw;
        private float vertical, horizontal;
        public float rotspeed = 2f;

        private Vector3 targetRotation;

        public GameObject moveAroundButton, leftPanel, infoPanel;

        private CameraMove _cameraMove;

        private OrbitCamera _orbitCamera;

        private int counter;

        public Transform playerSpawner;
        //Check for steep contact
        bool CheckSteepContacts()
        {
            //return true if contact
            if (steepContactCount > 1)
            {
                //normalize steep contact
                steepNormal.Normalize();
                //local var calculate upDot
                float upDot = Vector3.Dot(upAxis, steepNormal);
                //if steep contact return true
                if (upDot >= minGroundDotProduct)
                {
                    groundContactCount = 1;
                    contactNormal = steepNormal;
                    return true;
                }
            }
            //no steep contact
            return false;
        }
        //Check for climbing
        bool CheckClimbing()
        {
            //climbContactCount > 0 return true
            if (Climbing)
            {
                groundContactCount = climbContactCount;
                contactNormal = climbNormal;
                return true;
            }
            //not climbing
            return false;
        }
        //Calculate dot
        void OnValidate()
        {
            minGroundDotProduct = Mathf.Cos(maxGroundAngle * Mathf.Deg2Rad);
            minStairsDotProduct = Mathf.Cos(maxStairsAngle * Mathf.Deg2Rad);
            minClimbDotProduct = Mathf.Cos(maxClimbAngle * Mathf.Deg2Rad);
        }
        //Set local var 
        void LocalAxis()
        {
            upAxis = transform.up;
            rightAxis = transform.right;
            forwardAxis = transform.forward;
        }
        //Awake before start
        void Awake()
        {
            //set rigidbody
            body = GetComponent<Rigidbody>();
            //set gravity
            gravity = Physics.gravity;
            //set local var
            LocalAxis();
            //calculate dot
            OnValidate();
        }

        // Start is called before the first frame update
        private void Start()
        {
            //animator
            animator = GetComponent<Animator>();
            //set Camera
            m_Camera = Camera.main;
            if(m_Camera)
            {
                _cameraMove = m_Camera.transform.GetComponent<CameraMove>();
                _orbitCamera = m_Camera.transform.GetComponent<OrbitCamera>();
            }
            
            //set GameController
            _gameController = GameObject.FindWithTag("GameController").GetComponent<GameController>();
        }
        private void Update()
        {
            //Activate menu
            if (_gameController.escpAction.triggered)
            {
                //return to terrain edition
                _cameraMove.enabled = true;
                _orbitCamera.enabled = false;
                moveAroundButton.SetActive(true);
                leftPanel.SetActive(true);
                infoPanel.SetActive(true);
            }

            if (_gameController.escp > 0)
            {
                transform.position = new Vector3(0, playerSpawner.position.y+1, 0);
            }

            if (_gameController.move.y != 0 || _gameController.move.x != 0)
            {
                animator.SetBool("Walk",true);
            }
            else
            {
                animator.SetBool("Walk",false);
            }

            if (OnGround)
            {
                animator.SetBool("Grounded", true);
            }
            else
            {
                animator.SetBool("Grounded", false);
            }
            
            if(_gameController.jumpAction.triggered && maxAirJumps > 0 && jumpPhase <= maxAirJumps)
            {
                animator.SetBool("Jump", true);
            }
            else
            {
                animator.SetBool("Jump", false);
            }

            //move in camera direction
            if (m_Camera.transform)
            {
                Vector3 forward = m_Camera.transform.forward;
                forward.y = 0f;
                forward.Normalize();
                Vector3 right = m_Camera.transform.right;
                right.y = 0f;
                right.Normalize();
                //accelerate
                desiredVelocity =
                    (forward * _gameController.move.y + right * _gameController.move.x);
            }
            //jump
            desiredJump |= _gameController.jumpAction.triggered;

            RotatePlayer();
            transform.rotation = cameraRotation * playerRotation;
        }

        
        void FixedUpdate()
        {
            UpdateState();
            AdjustVelocity();
            
            if (desiredJump)
            {
                desiredJump = false;
                Jump();
            }
            //gravity
            if (Climbing)
            {
                velocity -= contactNormal * (maxClimbAcceleration * 0.7f * Time.deltaTime);
            }
            else
            {
                velocity += gravity * Time.deltaTime;
            }

            RotateCamera();

            //displace
            body.velocity = velocity;
            ClearState();
        }

        void UpdateState()
        {
            stepsSinceLastGrounded += 1;
            stepsSinceLastJump += 1;
            velocity = body.velocity;
            if (CheckClimbing() || OnGround || CheckSteepContacts())
            {
                stepsSinceLastGrounded = 0;
                if (stepsSinceLastJump > 1)
                {
                    jumpPhase = 0;
                }
                if (groundContactCount > 1)
                {
                    contactNormal.Normalize();
                }
                transform.up = contactNormal.normalized;
            }
            else
            {
                contactNormal = Vector3.up;
            }
        }

        void AdjustVelocity()
        {
            //local var acc & speed
            float acceleration, speed;
            //local var axis for climb direction
            Vector3 xAxis=default, zAxis=default;
            if (Climbing)
            {
                //set acc
                acceleration = maxClimbAcceleration;
                
                //set speed
                speed = _gameController.moveShift > 0 ? maxClimbShiftSpeed : maxClimbSpeed;
                //set axis
                if (transform.rotation.eulerAngles.y >= 135 && transform.rotation.eulerAngles.y <= 225)//looking backward
                {
                    zAxis = -upAxis;
                    xAxis = -Vector3.Cross(contactNormal, upAxis);
                }
                if (transform.rotation.eulerAngles.y <= 45 || transform.rotation.eulerAngles.y >= 315)//looking forward
                {
                    zAxis = upAxis;
                    xAxis = Vector3.Cross(contactNormal, upAxis);
                }
                if (transform.rotation.eulerAngles.y > 225 && transform.rotation.eulerAngles.y < 315)//looking left
                {
                    zAxis = Vector3.Cross(contactNormal, upAxis);
                    xAxis = -upAxis;
                }
                if (transform.rotation.eulerAngles.y > 45 && transform.rotation.eulerAngles.y < 135)//looking right
                {
                    zAxis = -Vector3.Cross(contactNormal, upAxis);
                    xAxis = upAxis;
                }
            }
            else
            {
                //set acc
                acceleration = OnGround ? maxAcceleration : maxAirAcceleration;
                //set speed
                speed = _gameController.moveShift>0 ? maxShiftSpeed : maxSpeed;
                //set axis
                xAxis = rightAxis;
                zAxis = forwardAxis;
            }
            if (_gameController.moveShift > 0)
            {
                animator.SetBool("Run",true);
            }
            else
            {
                animator.SetBool("Run",false);
            }
            //calculate normal axis
            xAxis = ProjectDirectionOnPlane(xAxis, contactNormal);
            zAxis = ProjectDirectionOnPlane(zAxis, contactNormal);
            
            //calculate local var dot
            float currentX = Vector3.Dot(velocity, xAxis);
            float currentZ = Vector3.Dot(velocity, zAxis);
            
            //calculate acc along time
            float maxSpeedChange = acceleration * Time.deltaTime;
            //calculate desiredMovement along each axis
            float newX =
                Mathf.MoveTowards(currentX, desiredVelocity.x * speed, maxSpeedChange);
            float newZ =
                Mathf.MoveTowards(currentZ, desiredVelocity.z * speed, maxSpeedChange);
            //apply movement to velocity
            velocity += xAxis * (newX - currentX) + zAxis * (newZ - currentZ);
        }
        void Jump()
        {
            //local var jump normal
            Vector3 jumpDirection;
            
            //set jumpDot on ground
            if (OnGround)
            {
                jumpDirection = contactNormal;
            }
            else if (OnSteep)//on steep
            {
                jumpDirection = steepNormal;
                jumpPhase = 0;
            }
            else if (maxAirJumps > 0 && jumpPhase <= maxAirJumps)//in thin air
            {
                
                if (jumpPhase == 0)
                {
                    jumpPhase = 1;
                }

                jumpDirection = contactNormal;
            }
            else
            {
                return;
            }
            
            stepsSinceLastJump = 0;
            //++ jump
            jumpPhase += 1;
            //calculate jump speed according to gravity 
            float jumpSpeed = Mathf.Sqrt(-2f * gravity.y * jumpHeight);
            //normalize jumpDir with upAxis 
            jumpDirection = (jumpDirection + upAxis).normalized;
            //calculate speed of jump along jumpDir
            float alignedSpeed = Vector3.Dot(velocity, jumpDirection);
            //clamp smooth jump speed 
            if (alignedSpeed > 0f)
            {
                jumpSpeed = Mathf.Max(jumpSpeed - alignedSpeed, 0f);
            }
            //apply jump speed to velocity
            velocity += jumpDirection * jumpSpeed;
        }

        void ClearState()
        {
            groundContactCount = steepContactCount = climbContactCount = 0;
            contactNormal = steepNormal = climbNormal = Vector3.zero;
        }

        Vector3 ProjectDirectionOnPlane(Vector3 direction, Vector3 normal)
        {
            return (direction - normal * Vector3.Dot(direction, normal)).normalized;
        }

        float GetMinDot(int layer)
        {
            return (steepMask & (1 << layer)) == 0 ? minGroundDotProduct : minStairsDotProduct;
        }

        void OnCollisionEnter(Collision collision)
        {
            EvaluateCollision(collision);
        }

        void OnCollisionStay(Collision collision)
        {
            EvaluateCollision(collision);
        }

        void EvaluateCollision(Collision collision)
        {
            int layer = collision.gameObject.layer;
            string objectName = collision.gameObject.name;
            float minDot = GetMinDot(layer);
            for (int i = 0; i < collision.contactCount; i++)
            {
                Vector3 normal = collision.GetContact(i).normal;
                float upDot = Vector3.Dot(upAxis, normal);
                if (upDot >= minDot)
                {
                    groundContactCount += 1;
                    contactNormal += normal;
                }
                else
                {
                    //here stairs
                    if (upDot > -0.01f)
                    {
                        steepContactCount += 1;
                        steepNormal += normal;
                    }
                    if(objectName.Contains("Tree"))
                    {
                        if (upDot >= minClimbDotProduct &&
                            (climbMask & (1 << layer)) != 0)
                        {
                            climbContactCount += 1;
                            climbNormal += normal;
                        }
                    }
                }
            }
        }

        void RotateCamera()
        {
            if (_gameController.alt > 0)
            {
                return;
            }
            //rotate player with camera look direction
            cameraRotation = Quaternion.Slerp(cameraRotation,
                Quaternion.LookRotation(new Vector3(m_Camera.transform.forward.x, 0, m_Camera.transform.forward.z),
                    transform.up), camRotationSpeed * Time.fixedDeltaTime);
        }

        void RotatePlayer()
        {
            Vector3 input = new Vector3(_gameController.move.x,0, _gameController.move.y);

            if (input.sqrMagnitude > 1f)
            {
                input.Normalize();
            }
            if(input != Vector3.zero)
            {
                playerRotation = Quaternion.Slerp(playerRotation,
                    Quaternion.LookRotation(input, upAxis), rotspeed * Time.deltaTime);
            }
        }
        void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawRay(new Vector3(transform.position.x, transform.position.y, transform.position.z + 1),
                transform.up * 3f);

            Gizmos.color = Color.red;
            Gizmos.DrawRay(new Vector3(transform.position.x, transform.position.y, transform.position.z + 1),
                transform.right * 3f);

            Gizmos.color = Color.blue;
            Gizmos.DrawRay(new Vector3(transform.position.x, transform.position.y, transform.position.z + 1),
                transform.forward * 3f);
            Gizmos.color = Color.cyan;
        }
    }
}