using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Combat;
using RPG.Stats;
using RPG.Player;

public class AnimationPass : MonoBehaviour
{
    public PlayerController playerControl;
    public PlayerStats pStats;
    public EquipmentChange equipmentChange;
   
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void hit()
    {
       
        if(playerControl.isLocalPlayer == false){
            return;
        }
        Health healthCom = playerControl.selectedEnemy.GetComponent<Health>();
        healthCom.TakeDamage(pStats.My_Dmg);
        
    }
    public void rangedHit()
    {
       
        Health healthCom = playerControl.selectedEnemy.GetComponent<Health>();
        equipmentChange.currentWeapon.rangedAttack(equipmentChange.Righthandtransform, equipmentChange.Lefthandtransform);
        healthCom.TakeDamage(pStats.My_Dmg);
    }
  
}
