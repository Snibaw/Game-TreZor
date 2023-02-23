using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;


namespace RPG.UI.DamageText
{
    public class CombatText : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI combatTextPrefab = null;
        public void DestroyObject()
        {
            Destroy(gameObject);
        }
        
        public void SetValue(float amount)
        {
            combatTextPrefab.text = String.Format("{0:0}", amount);
        }
    }
}

