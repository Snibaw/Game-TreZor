using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

namespace RPG.UI.DamageText
{
    public class CombatTextPro : MonoBehaviour
    {
        [SerializeField] TextMeshPro combatTextPrefab = null;
        public void DestroyText()
        {
            Destroy(gameObject);
        }
        public void SetValue(float amount)
        {
            combatTextPrefab.text = String.Format("{0:0}", amount);
        }
    }
}

