using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.Events;
using RPG.Player;
using RPG.Combat;

namespace RPG.Combat
{
    public class Health : NetworkBehaviour
    {
        [SyncVar(hook = nameof(OnUIChange))]
        public float currentHealthPoint;
        public float maxHealthPoint = 100f;
        public float enemyCurrentFill;
        
        public float currentManaPoint;
        public float maxManaPoint = 100f;
        public float enemyCurrentFillMana;
        
        
        //Enemy Stats
        [SerializeField] public string enemyName;
        [SerializeField] public int enemyLevel;

        public AiController aicontroller;
        public EnemyTarget enemyTarget;
        public AiMover aiMover;
       
        public GameObject[] objectsToHide;

        [SyncVar(hook = nameof(OnIsDeadChange))]
        bool isDead = false;


        public TextMesh Hptext = null;
        [SerializeField] Animator animator;
        
        [SerializeField] TakeDamageEvent takeDamage;
        [System.Serializable]
        public class TakeDamageEvent : UnityEvent<float>
        {
            
        }
       

       public bool IsDead()
        {
            return isDead;
        }
        private void Start()
        {
            currentManaPoint = maxManaPoint;
            currentHealthPoint = maxHealthPoint;
            if (Hptext != null)
            {
                Hptext.text = "hp" + currentHealthPoint;
            }
        }
        private void Update()
        {
            if (!isServer) return;
            UseMana();
        }


        [Command(requiresAuthority = false)]
        public void TakeDamage(float damage)
        {
            currentHealthPoint = Mathf.Max(currentHealthPoint - damage, 0); // so the damage can not go below 0
            
            if (currentHealthPoint == 0)
            {
                Death();
            }
            else
            {
                takeDamage.Invoke(damage);
            }
        }
        
        public void UseMana()
        {
            if(maxManaPoint ==0)
            {
                enemyCurrentFillMana = 0;
            }
            else
            {
                enemyCurrentFillMana = currentManaPoint / maxManaPoint;
            }
            
        }
        void Death()
        {
            if (isDead) return;

            isDead = true;
            animator.SetTrigger("Die");
        }
       void OnUIChange(float _old, float _new)
        {
            if (Hptext != null)
            {
                Hptext.text = currentHealthPoint + " hp";
            }
            enemyCurrentFill = _new / maxHealthPoint;
        }
       void OnIsDeadChange(bool _old, bool _new)
        {
            if (isDead == false)
            {
                aicontroller.enabled = true;
                enemyTarget.enabled = true;
                aiMover.enabled = true;
               

                foreach (var obj in objectsToHide)
                {
                    obj.SetActive(true);
                }
            }else if (isDead == true)
            {
                aicontroller.enabled = false;
                enemyTarget.enabled = false;
                aiMover.enabled = false;
               

                animator.SetTrigger("Die");
            }
               

        }

    }
}
