using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Player;



namespace RPG.Combat
{
    public class EnemyTarget : MonoBehaviour
    {
        [SerializeField] float autoAttacktime;
        [SerializeField] int CanAttack = 1;
        [SerializeField] int basisAttackdmg;
        AiController aiController;
        // Start is called before the first frame update
        void Start()
        {
            aiController = GetComponent<AiController>();
        }

        // Update is called once per frame
        void Update()
        {

        }
       public void Attack()
        {
            if (CanAttack == 1)
            {
                StartCoroutine(basisAttack());
            }
            
        }
        IEnumerator basisAttack()
        {
            CanAttack = 0;
            GetComponent<Animator>().SetTrigger("BasisAttack1");
            yield return new WaitForSeconds(autoAttacktime);
            CanAttack = 1; 
            Attack();
        }
        public void EnemyHit()
        {
            if(aiController.Target == null) return;
            HealthPlayer healthCom = aiController.Target.GetComponent<HealthPlayer>();
            healthCom.TakeDamage(basisAttackdmg);
        }
    }
}
