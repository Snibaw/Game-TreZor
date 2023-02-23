using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.UI.DamageText
{
    public class CombatTextSpawner : MonoBehaviour
    {
        [SerializeField] CombatText combatTextPrefab = null;
        public void spawn(float damageAmount)
        {
            CombatText instance = Instantiate<CombatText>(combatTextPrefab, transform);
            instance.SetValue(damageAmount);
        }
    }
}

