using UnityEngine;
using RPG.Player;

namespace RPG.Combat
{
[CreateAssetMenu(fileName = "Weapon", menuName = "Weapon / Make New Weapon", order = 0)]

public class Weapon : ScriptableObject

    {
        [SerializeField] float weaponRange;
        [SerializeField] float weaponDamage;
        [SerializeField] float weaponSpeed;
        [SerializeField] bool isRightHanded = true;
        [SerializeField] Projectiles projectile = null;

        PlayerController playercontrol;
        public AnimatorOverrideController My_WeaponOveride = null;
        public GameObject EquipPrefab = null; 


        public void Spawn(Transform Righthand,Transform Lefthand, Animator animator)
        {
            if (EquipPrefab != null)
            {
                Transform handTransform = GetTransform(Righthand, Lefthand);
                Instantiate(EquipPrefab, handTransform);
            }
            if (My_WeaponOveride != null)
            {
                animator.runtimeAnimatorController = My_WeaponOveride;
            }
           
            

        }
        public bool Hasprojectile()
        { return projectile != null; }

        public void rangedAttack(Transform righthand, Transform lefthand)
        {
            Projectiles projectilesinstance = Instantiate(projectile,GetTransform(righthand,lefthand).position, Quaternion.identity); 
            playercontrol = FindObjectOfType<RPG.Player.PlayerController>();
            projectilesinstance.target = playercontrol.selectedEnemy.transform;
        }

        private Transform GetTransform(Transform Righthand, Transform Lefthand)
        {
            Transform handTransforms;
            if (isRightHanded) handTransforms = Righthand;
            else handTransforms = Lefthand;
            return handTransforms;
        }
        public float GetDamage()
        {
            return weaponDamage;
        }
        public float GetAttackSpeed()
        {
            return weaponSpeed;
        }
        public float GetAttackRange()
        {
            return weaponRange;
        }
    }

}