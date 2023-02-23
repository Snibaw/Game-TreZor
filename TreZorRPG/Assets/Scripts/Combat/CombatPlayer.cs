using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Player;
using RPG.Stats;
using Mirror;
using UnityEngine.UI;
using TMPro;



namespace RPG.Combat
{
    public class CombatPlayer : NetworkBehaviour
    {
        
        [SerializeField] bool canAutoAttackAngle, attacking = false, inRange = false;
        [SerializeField] PlayerController playerControl;
        [SerializeField] PlayerStats pStats;         
        [SyncVar] public int canAttack = 1;
        [SerializeField] EquipmentChange equipmentChange;
        [SerializeField] Animator animator;
        public NetworkAnimator netAnimator;
        
        // targetFrameEnemy
        [SerializeField] private GameObject targetFrame;
        // Health
        [SerializeField] private Image targetHealth_Image;
        [SerializeField] private TextMeshProUGUI targetHealth_Text = null;
        [SerializeField] private TextMeshProUGUI targetName_Text = null;
        [SerializeField] private TextMeshProUGUI targetLevel_Text = null;
        // Mana
        [SerializeField] private Image targetMana_Image;
        [SerializeField] private TextMeshProUGUI targetMana_Text = null;
        [SerializeField] private GameObject targetMana_Frame;

        public bool Attacking()
        {
            return attacking;
        }
        // Start is called before the first frame update
        void Start()
        {
            playerControl = GetComponent<PlayerController>();
            pStats = GetComponent<PlayerStats>();
          
        }

        

      

        // Update is called once per frame
        void Update()
        {
            
            if(playerControl.controls.ActionBar1.GetControlBindingDown())
            {
                StartAutoAttack();
            }
            //if (equipmentChange.haveRangedWep == true)
            //{


            //    playerControl.anim.SetFloat("AttackSpeed", 5 * Mathf.Pow(pStats.My_Attackspeed, -1f));
            //}else
            //{
            //    playerControl.anim.SetFloat("AttackSpeed", 1);
            //}


            if (playerControl.selectedEnemy != null)
            {
                UpdateTargetFrame();
                Health target = playerControl.selectedEnemy.GetComponent<Health>();
                if (playerControl.selectedEnemy != null &&  !target.IsDead())
                {
                    autoAttackAngle();
                    checkRange();
                    if (attacking == true)
                    {
                        StartAutoAttack();
                    }
                }else if (playerControl.selectedEnemy != null && target.IsDead())
                {
                    attacking = false;
                    playerControl.anim.SetBool("InCombat", false); 
                }

               
            }
            if (playerControl.selectedEnemy == null)
            {
                attacking = false;
            }
        }


        public void selectEnemy(EnemyTarget target) //this is left click on enemy 
        {
            showEnemyTargetFrame(target);
            UpdateTargetFrame();
        }
        private void UpdateTargetFrame()
        {
            Health target_Health = playerControl.selectedEnemy.GetComponent<Health>();
            targetHealth_Image.fillAmount = target_Health.enemyCurrentFill;
            targetHealth_Text.text = target_Health.currentHealthPoint + "/" + target_Health.maxHealthPoint;
            targetLevel_Text.text = target_Health.enemyLevel.ToString();
            targetName_Text.text = target_Health.enemyName;
            if(target_Health.maxManaPoint == 0)
            {
                targetMana_Frame.SetActive(false);
            }
            else
            {
                targetMana_Frame.SetActive(true);
                targetMana_Text.text = target_Health.currentManaPoint + "/" + target_Health.maxManaPoint;
                targetMana_Image.fillAmount = target_Health.enemyCurrentFillMana;
            }
            
        }

        public void autoAttack(EnemyTarget target)
        {
            print("Starting Autoattacking if we are within range");
            StartAutoAttack();

            


            if (playerControl.selectedEnemy != null && !inRange)
            {
                print("not within range to autoattack");
                StartAutoAttack();

                // Make animation for autoattack here. (Combat stance) 
            }
            else if (playerControl.selectedEnemy != null && inRange && canAutoAttackAngle)
            {
                print("Within range to Auto attack + angle");
                StartAutoAttack();
              

                // Make animation for autoattack 
                // combat dmg 

            }
            else if (playerControl.selectedEnemy != null && inRange && !canAutoAttackAngle)
            {
                print("Im facing the wrong way");
                StartAutoAttack();

                // make sound 
            }


        }

        void autoAttackAngle()
        {
            Vector3 targetDir = playerControl.selectedEnemy.transform.position - transform.position;
            Vector3 forward = transform.forward;
            float angle = Vector3.Angle(targetDir, forward);

            if (angle > 60)
            {
                canAutoAttackAngle = false;

            } else
            {
                canAutoAttackAngle = true;
            }
        }
        public void StartAutoAttack()
        {
            if(playerControl.selectedEnemy == null)
            {
                print("No target selected");
            }
            else
            {
                UpdateTargetFrame();
                Health target = playerControl.selectedEnemy.GetComponent<Health>();
                if (!target.IsDead())
                {
                    attacking = true;

                    if (playerControl.standinStill == true)
                    {
                        playerControl.anim.SetBool("InCombat", true);
                    }
                    else if (playerControl.standinStill == false)
                    {
                        playerControl.anim.SetBool("InCombat", false);
                    }

                    if (equipmentChange.currentWeapon.Hasprojectile() ==false)
                    {
                        if (playerControl.selectedEnemy != null && canAutoAttackAngle && inRange && canAttack == 1)
                        {
                            StartCoroutine(basisAttack());
                        }
                    }else if (equipmentChange.currentWeapon.Hasprojectile() == true)
                    {
                        if (playerControl.selectedEnemy != null && canAutoAttackAngle && inRange && canAttack == 1)
                        {
                            StartCoroutine(rangedAttack());
                        }
                    }
                
                }
            }
        }
        IEnumerator rangedAttack()
        {
            canAttack = 0;
            netAnimator.SetTrigger("AttackNow");
            //equipmentChange.currentWeapon.rangedAttack(equipmentChange.Righthandtransform, equipmentChange.Lefthandtransform);
            yield return new WaitForSeconds(pStats.My_Attackspeed);
            canAttack = 1;
            StartAutoAttack();


        }
       
        IEnumerator basisAttack()
        {
            canAttack = 0;
            netAnimator.SetTrigger("AttackNow");           
            yield return new WaitForSeconds(pStats.My_Attackspeed);
            canAttack = 1;
            StartAutoAttack();
        }
        void checkRange()
        {
            bool IsInRange = Vector3.Distance(this.transform.position, playerControl.selectedEnemy.transform.position) < pStats.My_Attackrange;

            if (IsInRange)
            {
                inRange = true;
            }else
            {
                inRange = false;
            }
        }
        public void showEnemyTargetFrame(EnemyTarget target)
        {
            targetFrame.SetActive(true);
        }
        public void unShowEnemyTargetFrame()
        {
            targetFrame.SetActive(false);
        }
     
      
     
      

       
}
}






