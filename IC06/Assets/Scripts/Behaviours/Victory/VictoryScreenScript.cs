using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class VictoryScreenScript : MonoBehaviour
{

    // REFERENCES
    SpriteRenderer VictoryText;
    SpriteRenderer WinnerSprites;
    SpriteRenderer LooserSprites;

    // PROPRIETES
    private bool skipAllowed = false;

    // METHODES

    void Start()
    {
        // REFERENCES MANQUANTES
        if(VictoryText == null)
        {
            VictoryText = transform.Find("VictoryText").GetComponent<SpriteRenderer>();
        }
        if (WinnerSprites == null)
        {
            WinnerSprites = transform.Find("Winner").GetComponent<SpriteRenderer>();
        }
        if (LooserSprites == null)
        {
            LooserSprites = transform.Find("Looser").GetComponent<SpriteRenderer>();
        }

        // SETUP COULEUR
        VictoryText.color = VictoryClass.VictoryColor;
        WinnerSprites.color = VictoryClass.VictoryColor;
        LooserSprites.color = VictoryClass.LooserColor;

        Invoke("EnableSkip", 2f);

    }

    private void EnableSkip()
    {
        skipAllowed = true;
    }


    void Update()
    {
        if (skipAllowed)
        {
            for (int i = 0; i < 20; i++)
            {
                if (Input.GetKeyDown("joystick button " + i) || Input.anyKeyDown)
                {
                    SceneManager.LoadScene("Menu");
                    skipAllowed = false;
                }
            }
        }
    }
}
