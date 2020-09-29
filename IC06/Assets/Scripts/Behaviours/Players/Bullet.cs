using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    Color bulletColor = new Color(0,0,0,1);
    SpriteRenderer rend;

    public float bulletLifespan = 0.4f;
    public float BulletLifespan 
    {
        get { return bulletLifespan; }
        set { bulletLifespan = value; }
    }

    private bool isBouncy = false;
    public bool IsBouncy {
        get { return isBouncy; }
        set { isBouncy = value; }
	}


    void Start()
    {
        rend = GetComponent<SpriteRenderer>();
        rend.color = bulletColor;
        Destroy(gameObject, bulletLifespan);
	}

    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "wall")
            if (!isBouncy)
                Destroy(gameObject);
        
        if (other.gameObject.tag == "Player")
        {
            if (bulletColor != other.gameObject.GetComponent<PlayerCharacteristics>().getPlayerColor())
            {
                Destroy(gameObject);
                other.gameObject.GetComponent<PlayerCharacteristics>().takeDamage(1);
			}
             
		}
	}

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Tile")
        {
            other.gameObject.GetComponent<ChangeColor>().setColor(bulletColor);
		}
	}

    public void setColor(Color color)
    {
        bulletColor = color;
	}
}
