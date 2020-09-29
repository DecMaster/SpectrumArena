using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootstepsScript : MonoBehaviour
{
    // PROPRIETES
    public Color color;
    public bool ok = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (ok && other.gameObject.tag == "Tile") //For the "footsteps" powerUp
        {
            other.gameObject.GetComponent<ChangeColor>().setColor(color);
        }
    }
}
