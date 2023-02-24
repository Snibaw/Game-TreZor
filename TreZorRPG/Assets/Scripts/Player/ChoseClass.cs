using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Player;

public class ChoseClass : MonoBehaviour
{
    private bool isClassMenuOpen = false;
    [SerializeField] GameObject classMenu;
    private PlayerController playerControl;
    
    void Start()
    {
        playerControl = GetComponent<PlayerController>();
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
        Debug.Log("Chose class: " + id_class);
    }
}
