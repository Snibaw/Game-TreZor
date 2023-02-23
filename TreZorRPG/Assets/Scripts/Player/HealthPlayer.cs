using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;
using Mirror;

namespace RPG.Player
{
    public class HealthPlayer : NetworkBehaviour
    {
        [SerializeField] private Image health_image;
        [SerializeField] public TextMeshProUGUI health_text = null;
        private float currentFill;
        [SyncVar(hook = nameof(OnUIChange))]
        [SerializeField] float currentHealthPoint;
        [SerializeField] float maxHealthPoint = 100f;
        [SerializeField] Animator animator;
        bool isDead = false;

        [SerializeField] TakeDamageEvent takeDamage;
        [System.Serializable]
        public class TakeDamageEvent : UnityEvent<float>
        {
            
        }
        private void Start() {
            currentHealthPoint = maxHealthPoint;
            if(health_text!=null)
            {
                health_text.text = currentHealthPoint + "/" + maxHealthPoint;
            }
        }
        public bool IsDead()
        {
            return isDead;
        }

        [Server]
        public void TakeDamage(float damage)
        {
            currentHealthPoint = Mathf.Max(currentHealthPoint - damage, 0); // so the damage cannot go below 0
            
            if (currentHealthPoint == 0)
            {
                Death();
            }
            else
            {
                takeDamage.Invoke(damage);
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
            if (health_text != null)
            {
                health_text.text = currentHealthPoint + "/" + maxHealthPoint;
            }
            currentFill = _new / maxHealthPoint;
            health_image.fillAmount = currentFill;
        }



    }

}
