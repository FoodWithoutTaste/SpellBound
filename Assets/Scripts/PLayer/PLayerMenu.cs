using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class PLayerMenu : MonoBehaviour
{
    [SerializeField] bool selection = true;
    [SerializeField] List<GameObject> Menus;
    [SerializeField] List<Animator> MenuButtons;
    [SerializeField]  float selectedMenu = 0;

    [SerializeField] bool itemInspectBool = false;
    [SerializeField] GameObject itemInspect;
    public void Start()
    {
        ChangeToMenu(0);
    }

    public void ChangeToMenu(int a)
    {
        if(itemInspectBool == false)
        {
            if (a == 0)
            {
                itemInspect.SetActive(true);
            }
            else
            {
                itemInspect.SetActive(false);

            }
        }
       
        selectedMenu = a;
        for (int i = 0; i < Menus.Count; i++)
        {
           
            if (i == selectedMenu)
            {
                if(selection)
                    MenuButtons[i].SetBool("SelectedD", true);
                Menus[i].SetActive(true);
            }
            else
            {
                if (selection)
                    MenuButtons[i].SetBool("SelectedD", false);
                Menus[i].SetActive(false);
            }

         
                
        }
    }

    private void OnDisable()
    {
        if (itemInspectBool == false)
            itemInspect.SetActive(false);

    }
    private void OnEnable()
    {
        if (itemInspectBool == false)
            itemInspect.SetActive(true);
    }

}
