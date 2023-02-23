using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Stats
{
    public class PlayerStats : MonoBehaviour
    {
        [SerializeField] EquipmentChange My_Equipment;
        public float My_Attackspeed;
        public float My_Dmg = 5f;
        public float My_Attackrange;

        public void Update()
        {
            CalcStats();
        }

        private void CalcStats()
        {
            My_Dmg = My_Equipment.currentWeapon.GetDamage();
            My_Attackspeed = My_Equipment.currentWeapon.GetAttackSpeed();
            My_Attackrange = My_Equipment.currentWeapon.GetAttackRange();
        }
    }

   
}
