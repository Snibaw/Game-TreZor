using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.UI.DamageText
{
    public class CombatTextSpawnerPro : MonoBehaviour
    {
        [SerializeField] CombatTextPro combatTextPrefab = null;
        public void spawn(float damageAmount)
        {
            CombatTextPro instance = Instantiate<CombatTextPro>(combatTextPrefab, transform);
            instance.SetValue(damageAmount);
        }
    }

}
