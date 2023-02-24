using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using RPG.Combat;
using RPG.Player;


public enum EquipptedItem : byte
{
    nothing,
    twohandedWep,
    Unarmed,
    oneHandwep,
    dagger, 
    bow
}
public class EquipmentChange : NetworkBehaviour
{
   
    public Transform Righthandtransform = null;
    public Transform Lefthandtransform = null;    
    [SerializeField] Animator My_animator;
    [SerializeField] Weapon weaponSword;
    [SerializeField] Weapon Unarmed;
    [SerializeField] Weapon bow; 

    public Weapon currentWeapon = null;

    public bool haveRangedWep = false; 
    

    [SyncVar(hook = nameof(OnChangeEquipment))]
    public EquipptedItem equippedItem;
    public ChoseClass choseClass;
    private void Start(){
        choseClass = GetComponent<ChoseClass>();
    }

    void OnChangeEquipment (EquipptedItem oldEquippeditem, EquipptedItem newEquippeditem)
    {
        StartCoroutine(ChangeEquipment(newEquippeditem));
    }

    IEnumerator ChangeEquipment(EquipptedItem newEquippeditem)
    {
        while (Lefthandtransform.transform.childCount > 0) // if there is a child in the left hand
        {
            Destroy(Lefthandtransform.transform.GetChild(0).gameObject); // destroy it
            yield return null;
        }
        while (Righthandtransform.transform.childCount > 0) // if there is a child in the right hand
        {
            Destroy(Righthandtransform.transform.GetChild(0).gameObject); // destroy it
            yield return null;
        }
        switch (newEquippeditem)
        {
            case EquipptedItem.twohandedWep:

                EquipWeapon(weaponSword); 
                break;
            case EquipptedItem.Unarmed:
                EquipWeapon(Unarmed);
                break;
            case EquipptedItem.bow:
                EquipWeapon(bow);
                break;
        }
    }
    private void Update()
    {
        if (!isLocalPlayer) return;
        // If the player presses X.
        if(Input.GetKeyDown(KeyCode.X))
           {
               if(equippedItem != EquipptedItem.Unarmed) // if the player is armed
               {
                   EquipHand(); // We give him his hands
               }
               else // If the player is unarmed, we give him a weapon according to his class
               {
                    int id = choseClass.GetIdActualClass(); 
                    switch(id)
                    {
                        case 0:
                            CmDChangeEquippedItem(EquipptedItem.twohandedWep);
                            Debug.Log(id);
                            break;
                        case 1:
                            CmDChangeEquippedItem(EquipptedItem.bow);
                            break;
                        default:
                            EquipHand();
                            break;
                    }
               }
           }

       if (equippedItem == EquipptedItem.bow)
        {
            haveRangedWep = true;
        }else
        {
            haveRangedWep = false;
        }


    }
    public void EquipHand() // When we change class, we equip hands
    {
        CmDChangeEquippedItem(EquipptedItem.Unarmed);
    }

    [Command]
    void CmDChangeEquippedItem(EquipptedItem selectedItem) // We change the item equipped by the player
    {
        equippedItem = selectedItem;
    }

    public void EquipWeapon(Weapon weapon) // We equip the weapon
    {
        currentWeapon = weapon;
        Animator animator = GetComponent<Animator>();
        weapon.Spawn(Righthandtransform,Lefthandtransform, My_animator);
        
    }
}
