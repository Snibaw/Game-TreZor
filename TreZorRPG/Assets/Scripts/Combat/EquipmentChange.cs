using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using RPG.Combat;


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

    void OnChangeEquipment (EquipptedItem oldEquippeditem, EquipptedItem newEquippeditem)
    {
        StartCoroutine(ChangeEquipment(newEquippeditem));
    }

    IEnumerator ChangeEquipment(EquipptedItem newEquippeditem)
    {
        while (Lefthandtransform.transform.childCount > 0)
        {
            Destroy(Lefthandtransform.transform.GetChild(0).gameObject);
            yield return null;
        }
        while (Righthandtransform.transform.childCount > 0)
        {
            Destroy(Righthandtransform.transform.GetChild(0).gameObject);
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

        if (Input.GetKeyDown(KeyCode.W) && equippedItem != EquipptedItem.Unarmed)
            CmDChangeEquippedItem(EquipptedItem.Unarmed);
        if (Input.GetKeyDown(KeyCode.X) && equippedItem != EquipptedItem.twohandedWep)
            CmDChangeEquippedItem(EquipptedItem.twohandedWep);
        if (Input.GetKeyDown(KeyCode.C) && equippedItem != EquipptedItem.bow)
            CmDChangeEquippedItem(EquipptedItem.bow);

       if (equippedItem == EquipptedItem.bow)
        {
            haveRangedWep = true;
        }else
        {
            haveRangedWep = false;
        }


    }

    [Command]
    void CmDChangeEquippedItem(EquipptedItem selectedItem)
    {
        equippedItem = selectedItem;
    }
    public void EquipWeapon(Weapon weapon)
    {
        currentWeapon = weapon;
        Animator animator = GetComponent<Animator>();
        weapon.Spawn(Righthandtransform,Lefthandtransform, My_animator);
        
    }
}
