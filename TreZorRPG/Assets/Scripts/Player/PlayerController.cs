using RIPTIDE.CameraController;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Combat;
using System;
using Mirror;

namespace RPG.Player
{
    public class PlayerController : NetworkBehaviour
    {
        // inputs
        public Controls controls;
        [SerializeField] private Vector2 inputs;
        [HideInInspector]
        public float rotation;
        [HideInInspector] public Vector2 inputNormalized;
        [SerializeField] bool run = true;
        [SerializeField] bool jump = false;
        [HideInInspector] public bool steer, AutoRun;

        public GameObject camCam;
        [SerializeField] public GameObject UiCam;
        public Animator anim;

        //Combat
        public bool standinStill;


        //Selecting 
        public float clickDistance = 50;
        public GameObject selectedEnemy;


        //velocity
        [HideInInspector] Vector3 velocity;
        [SerializeField] float gravity = -9, velocityY, terminalVelocity = -25;
        [HideInInspector] float fallMault;

        //Running
       public float currentSpeed;
        public float baseSpeed = 1f, runSpeed = 4, rotationSpeed = 1.0f;

        //ground
        Vector3 forwardDirection, CollisionPoint;
        [HideInInspector] public float slopeAngle;
        [HideInInspector] public float directionAngle;
        [HideInInspector] public float strafeAngle;
        [HideInInspector] public float forwardAngle;
        [HideInInspector] float forwardMult, strafeMault;
        Ray groundRay;
        RaycastHit groundhit;

        //DebugGround
        [SerializeField] bool showFallNormal, showMoveDirection, showForwardDirection, showStrafeDirection, showGroundRay;

        //jump
        bool jumping, canJump = true;
        public float jumpSpeed, jumpHeight = 2;
        Vector3 jumpDirection;

        // reference
        CharacterController controller;
        public Transform groundDirection, falldirection, moveDirection;
        public cameraController maincam;
        HealthPlayer health;

       
        private void Start()
        {
          if(!isLocalPlayer)
            {
                camCam.gameObject.SetActive(false);
                UiCam.gameObject.SetActive(false);
            }

            controller = GetComponent<CharacterController>();
           health = GetComponent<HealthPlayer>();
        }
        private void Update()
        {
            if(!isLocalPlayer)
            {
                return;
            }
            if (health.IsDead()) return;

            GetInputs();
            Locomotion();
        }
        void Locomotion()
        {
            GroundDirection();
            //rotation
            Vector3 characterRotation = transform.eulerAngles + new Vector3(0, rotation * rotationSpeed, 0);
            transform.eulerAngles = characterRotation;


            //Press space to jump 
            if (jump && controller.isGrounded && slopeAngle <= controller.slopeLimit && canJump)

            {
                Jump();
            }

            //running and walking 
            if (controller.isGrounded && slopeAngle <= controller.slopeLimit)
            {


                currentSpeed = baseSpeed;

                if (run)
                {
                    
                    currentSpeed *= runSpeed;
                    if (inputNormalized.y < 0)
                    {
                        currentSpeed = currentSpeed / 2;
                    }

                }
            }
            else if (!controller.isGrounded || slopeAngle > controller.slopeLimit)
            {
                inputNormalized = Vector2.Lerp(inputNormalized, Vector2.zero, 0.025f);
                currentSpeed = Mathf.Lerp(currentSpeed, 0, 0.025f);
            }
           

            //Apply Gravity if not grounded
            if (!controller.isGrounded && velocityY > terminalVelocity)
            {
                velocityY += gravity * Time.deltaTime;
            }
            else if (controller.isGrounded && slopeAngle > controller.slopeLimit)
            {
                velocityY = Mathf.Lerp(velocityY, terminalVelocity, 0.25f);
            }






            // apply inputs
            if (!jumping)
            {
                //velocity = (groundDirection.forward * inputNormalized.magnitude) * (currentSpeed * forwardMult) + falldirection.up * (velocityY * fallMault);
                velocity = (groundDirection.forward * inputNormalized.y * forwardMult + groundDirection.right * inputNormalized.x * strafeMault); // appling movement direction inputs
                velocity *= currentSpeed; // applying current movespeed
                velocity += falldirection.up * (velocityY * fallMault); // gravity 
            }

            else
            {
                velocity = jumpDirection * jumpSpeed + Vector3.up * velocityY;
            }


            //moving controller
            controller.Move(velocity * Time.deltaTime);

            if (controller.isGrounded)
            {
                if (jumping)
                    jumping = false;
                 this.anim.SetBool("Jump", false);
               
                

                if (!jump && !canJump)
                {
                    canJump = true;
                }

                velocityY = 0;
            }
        }

        #region //Groundstuff
        void GroundDirection()
        {
            //Setting forwarddirection
            //setting forward Direction to Controller position
            forwardDirection = transform.position;

            //Setting forward direction based on inputs
            if (inputNormalized.magnitude > 0)
                forwardDirection += transform.forward * inputNormalized.y + transform.right * inputNormalized.x;

            else
                forwardDirection += transform.forward;
            //setting ground direction to look in the forward direction normal. 
            moveDirection.LookAt(forwardDirection);
            falldirection.rotation = transform.rotation;
            groundDirection.rotation = transform.rotation;


            // setting ground ray 
            groundRay.origin = transform.position + CollisionPoint + Vector3.up * 0.05f;
            groundRay.direction = Vector3.down;

            if (showGroundRay)
                Debug.DrawLine(groundRay.origin, groundRay.origin + Vector3.down * 0.3f, Color.red);

            forwardMult = 1;
            fallMault = 1;
            strafeMault = 1;

            if (Physics.Raycast(groundRay, out groundhit, 0.3f))
            {
                slopeAngle = Vector3.Angle(transform.up, groundhit.normal);
                directionAngle = Vector3.Angle(moveDirection.forward, groundhit.normal) - 90;

                if (directionAngle < 0 && slopeAngle <= controller.slopeLimit)
                {
                    forwardAngle = Vector3.Angle(transform.forward, groundhit.normal) - 90; // checking forwardAngle to the slope
                    forwardMult = 1 / Mathf.Cos(forwardAngle * Mathf.Deg2Rad); // applying the forward movement multiplier based on the wardangle
                    groundDirection.eulerAngles += new Vector3(-forwardAngle, 0, 0); // rotation groundDirection X 

                    strafeAngle = Vector3.Angle(groundDirection.right, groundhit.normal) - 90;
                    strafeMault = 1 / Mathf.Cos(strafeAngle * Mathf.Deg2Rad); // applying strafe movement multiplayer based on strafe angle
                    groundDirection.eulerAngles += new Vector3(0, 0, strafeAngle);
                }
                else if (slopeAngle > controller.slopeLimit)
                {
                    float groundDistance = Vector3.Distance(groundRay.origin, groundhit.point);

                    if (groundDistance <= 0.1f)
                    {
                        fallMault = 1 / Mathf.Cos((90 - slopeAngle) * Mathf.Deg2Rad);

                        Vector3 groundCross = Vector3.Cross(groundhit.normal, Vector3.up);
                        falldirection.rotation = Quaternion.FromToRotation(transform.up, Vector3.Cross(groundCross, groundhit.normal));
                    }


                }
            }
            DebugGroundNormals();

        }
        #endregion
        void Jump()
        {
            //set jumping to true
            if (!jumping)
            {
                this.anim.SetBool("Jump", true);
                jumping = true;                
                canJump = false;
                standinStill = false;

                //if(canJump == false)
                //{
                //    this.anim.SetBool("Jump", false);
                //}
            }

            // set jump direction and speed
            jumpDirection = (transform.forward * inputs.y + transform.right * inputs.x).normalized;
            jumpSpeed = currentSpeed;

            // set velocity Y
            velocityY = Mathf.Sqrt(-gravity * jumpHeight);
        }

        void interactWithCombat()
        {
            RaycastHit[] hits = Physics.RaycastAll(GetMouseRay());
            foreach (RaycastHit hit in hits)
            {

                RaycastHit hitenemy;
                if (Physics.Raycast(GetMouseRay(), out hitenemy, clickDistance))
                {
                    EnemyTarget target = hit.transform.GetComponent<EnemyTarget>();
                    if (target == null) continue;

                    if (Input.GetMouseButtonDown(0))
                    {
                        selectedEnemy = hit.transform.gameObject;
                        GetComponent<CombatPlayer>().selectEnemy(target);

                    }
                    if (Input.GetMouseButtonDown(1))
                    {
                        selectedEnemy = hit.transform.gameObject;
                        GetComponent<CombatPlayer>().autoAttack(target);
                        GetComponent<CombatPlayer>().selectEnemy(target);
                    }


                }



            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                deselectenemy();
            }


        }
        void deselectenemy()
        {
            selectedEnemy = null;
            anim.SetBool("InCombat", false);
            GetComponent<CombatPlayer>().unShowEnemyTargetFrame();
        }
       







        private static Ray GetMouseRay()
        {
            return Camera.main.ScreenPointToRay(Input.mousePosition);
        }

        void GetInputs()
        {
            interactWithCombat();

            if (controls.AutoRun.GetControlBindingDown())
            {
                AutoRun = !AutoRun;
                this.anim.SetFloat("Horizontal", 0);
               
            }
            //forward backwards controls

            inputs.y = Axis(controls.Forwards.GetControlBinding(), controls.Backwards.GetControlBinding());

            if (inputs.y != 0 && !maincam.autoRunReset)
            {
                AutoRun = false;
                if (inputs.y == 1)
                {
                    this.anim.SetFloat("Horizontal", 1);
                    standinStill = false;

                }
                else if (inputs.y == -1)
                {
                    this.anim.SetFloat("Horizontal", -1);
                    standinStill = false;

                }
               

            }else if (inputs.y ==0)
            {
                this.anim.SetFloat("Horizontal", 0);
                standinStill = true;

            }

            if (AutoRun)
            {
                inputs.y += Axis(true, false);
                this.anim.SetFloat("Horizontal", 1);
                inputs.y = Mathf.Clamp(inputs.y, -1, 1);
                standinStill = false;

            }

                    

            //STRAFELEFT AND RIGHT 

            inputs.x = Axis(controls.straferight.GetControlBinding(), controls.strafeleft.GetControlBinding());
            {
                this.anim.SetFloat("Vertical", 0);
                

            }

            if (steer)
            {
                inputs.x += rotation = Axis(controls.RotateRight.GetControlBinding(), controls.RotateLeft.GetControlBinding());

                inputs.x = Mathf.Clamp(inputs.x, -1, 1);
               

            }

            if (steer)
            {
                rotation = Input.GetAxis("Mouse X") * maincam.CameraSpeed*2; 

            }
            else
            {
                rotation = Axis(controls.RotateRight.GetControlBinding(), controls.RotateLeft.GetControlBinding());

            }



            if (!controls.straferight.GetControlBinding() && !controls.strafeleft.GetControlBinding())
            {
                inputs.x = 0;
                this.anim.SetFloat("Vertical", 0);
                

            }

            if (inputs.x > 0)
            {
                this.anim.SetFloat("Vertical", 1);
                standinStill = false;

            }
            else if (inputs.x < 0)
            {
                this.anim.SetFloat("Vertical", -1);
                standinStill = false;

            }

            ////Rotation
            //if (controls.RotateRight.GetControlBinding())
            //    rotation= 1;

            ////strafeRight
            //if (controls.RotateLeft.GetControlBinding())
            //{
            //    if (controls.RotateRight.GetControlBinding())
            //        rotation = 0;
            //    else
            //        rotation = -1;
            //}
            ////Strafeleft+rigth = nothing
            //if (!controls.RotateRight.GetControlBinding() && !controls.RotateLeft.GetControlBinding())
            //{
            //    rotation = 0;

            //}
            // togglerun 
            if (controls.walkRun.GetControlBindingDown())
            {
                run = !run;
                
            }

            // jumping 
            jump = controls.Jump.GetControlBinding();

            inputNormalized = inputs.normalized;


        }
        public float Axis(bool pos, bool neg)
        {
            float axis = 0;

            if (pos)
                axis += 1;

            if (neg)
                axis -= 1;
            return axis;
        }

        void DebugGroundNormals()
        {
            Vector3 lineStart = transform.position + Vector3.up * 0.05f;
            if (showMoveDirection)
            {
                Debug.DrawLine(lineStart, lineStart + moveDirection.forward * 0.5f, Color.cyan);

            }

            if (showForwardDirection)
            {
                Debug.DrawLine(lineStart - groundDirection.forward * 0.5f, lineStart + groundDirection.forward * 0.5f, Color.blue);

            }

            if (showStrafeDirection)
            {
                Debug.DrawLine(lineStart - groundDirection.right * 0.5f, lineStart + groundDirection.right * 0.5f, Color.red);

            }
            if (showFallNormal)
            {
                Debug.DrawLine(lineStart, lineStart + falldirection.forward * 0.5f, Color.green);

            }
        }
        private void OnControllerColliderHit(ControllerColliderHit hit)
        {
            CollisionPoint = hit.point;
            CollisionPoint = CollisionPoint - transform.position;
        }





    }

}












