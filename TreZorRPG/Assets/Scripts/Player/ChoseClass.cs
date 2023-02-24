using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Player;

public class ChoseClass : MonoBehaviour
{
    [SerializeField] private GameObject[] classModels;
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
        if(id_class > classModels.Length)
        {
            Debug.Log("The class id is higher than the number of classes");
            return;
        }
        for (int i = 0; i < classModels.Length; i++)
        {
            if(i == id_class)
            {
                classModels[i].SetActive(true);
            }
            else
            {
                classModels[i].SetActive(false);
            }
        }
        CloseClassMenu();
    }
}
