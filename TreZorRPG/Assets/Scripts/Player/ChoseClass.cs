using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Combat;
using RPG.Player;

namespace RPG.Player
{
    public class ChoseClass : MonoBehaviour
    {
        public int id_actual_class; 
        private bool isClassMenuOpen = false;
        [SerializeField] GameObject classMenu;
        private PlayerController playerControl;
        private EquipmentChange equipmentChange;
        
        void Start()
        {
            playerControl = GetComponent<PlayerController>();
            equipmentChange = GetComponent<EquipmentChange>();
            
        }

        // Update is called once per frame
        void Update()
        {
            if(playerControl.controls.ChoseClass.GetControlBindingDown()) // If the player presses the button to open the class menu
            {
                if(isClassMenuOpen)
                {
                    CloseClassMenu();
                }
                else
                {
                    OpenClassMenu();
                }  
            }
        }
        void OpenClassMenu() // Open the class menu
        {
            isClassMenuOpen = true;
            classMenu.SetActive(true);
        }
        void CloseClassMenu() // Close the class menu
        {
            isClassMenuOpen = false;
            classMenu.SetActive(false);
        }
        public void ClickOnClassButton(int id_class) // When the player clicks on a class button
        {
            if(id_class != id_actual_class)
            {
                id_actual_class = id_class;
                equipmentChange.EquipHand();
            }
            Debug.Log("Class id: " + id_actual_class);
            CloseClassMenu();
        }
        public int GetIdActualClass()
        {
            return id_actual_class;
        }
    }

}
