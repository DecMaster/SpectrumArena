using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharactersScript : MonoBehaviour
{
    // PROPRIETES
    private List<Button> listeBoutons = new List<Button>();

    // METHODES
    void Start()
    {
        // REMPLISSAGE LISTE BOUTONS
        foreach(Transform t in transform.Find("CharactersButtons"))
        {
            // AJOUT LISTE
            Button b = t.GetComponent<Button>();
            listeBoutons.Add(b);
        }
    }


    void Update()
    {
        // RETOUR AU MENU
        if (Input.GetButtonDown("Cancel"))
        {
            MenuScript.instance.LeaveCharacters();
        }
    }


}
