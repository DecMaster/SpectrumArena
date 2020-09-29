using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooting : MonoBehaviour
{
    public enum NumPlayer
    {
        P1,
        P2
    }

    public AudioSource sfxPlayer = null;
    public SpriteRenderer rend;
    private Color playerColor;

    public NumPlayer numPlayer;
    public Transform firePoint;
    public GameObject bulletPrefab;
    public float bulletForce = 5f;

    public string weapon = "standard";
    
    public bool canShoot = true;

    Transform playerTransform;

    void Start()
    {
        playerColor = GetComponent<PlayerCharacteristics>().playerColor;
        rend = GetComponent<SpriteRenderer>();
        rend.color = playerColor;

        playerTransform = gameObject.GetComponent<Transform>();

        if(sfxPlayer == null)
        {
            sfxPlayer = this.GetComponent<AudioSource>();
        }
        sfxPlayer.volume = GameParameters.SfxVolume;

    }

    // Update is called once per frame
    void Update()
    {
        // SI TIR POSSIBLE
        if(!GameManagerScript.instance.pause && canShoot == true)
        {
            // JOUEUR 1
            if(numPlayer == NumPlayer.P1)
            {
                // TIR SIMPLE
                if (Input.GetButton("P1 - FireButton"))
                {
                    
                    float shootingDirection = gameObject.GetComponent<PlayerMovement>().getAngle();
                    Shoot(shootingDirection);
                }

                // TIR DIRECTIONNEL
                if (Input.GetAxisRaw("P1 - FireX") != 0 || Input.GetAxisRaw("P1 - FireY") != 0)
                {
                    float shootingDirection = Mathf.Atan2(Input.GetAxisRaw("P1 - FireY"), Input.GetAxisRaw("P1 - FireX")) * Mathf.Rad2Deg;
                    shootingDirection = reduceToFourDirections(shootingDirection);
                    Shoot(shootingDirection);
                }
            }

            // JOUEUR 2
            else
            {
                // TIR SIMPLE
                if (Input.GetButton("P2 - FireButton"))
                {
                    float shootingDirection = gameObject.GetComponent<PlayerMovement>().getAngle();
                    Shoot(shootingDirection);
                }

                // TIR DIRECTIONNEL
                if (Input.GetAxisRaw("P2 - FireX") != 0 || Input.GetAxisRaw("P2 - FireY") != 0)
                {
                    float shootingDirection = Mathf.Atan2(Input.GetAxisRaw("P2 - FireY"), Input.GetAxisRaw("P2 - FireX")) * Mathf.Rad2Deg;
                    shootingDirection = reduceToFourDirections(shootingDirection);
                    Shoot(shootingDirection);
                }
            }
        }
    }

    public Vector3 getFirePointPosition(Vector3 playerPosition, float shootingAngle, float offset=0) {
        Vector3 firePoint = playerPosition + new Vector3(
            offset*Mathf.Cos(shootingAngle * Mathf.Deg2Rad),
            offset*Mathf.Sin(shootingAngle * Mathf.Deg2Rad),
            0
		);
        return firePoint;
	}

    float reduceToFourDirections(float angle) { //takes an angle in degrees in a [0, 360[ interval and set it to only 4 possibles directions : {0, 90, -90, 180}
        if (angle <= 45 && angle > -45) return 0;
        if (angle <= 135 && angle > 45) return 90;
        if (angle <= -45 && angle > -135) return -90;
        return 180;
	}

    void SetCanShootToTrue()
    {
        canShoot = true;
	}

    void SingleFire(float shootingDirection) {
        GameObject bullet = Instantiate(bulletPrefab, getFirePointPosition(playerTransform.position, shootingDirection, 0.2f), Quaternion.Euler(0, 0, shootingDirection));
        bullet.GetComponent<Bullet>().setColor(playerColor);
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        rb.rotation = shootingDirection;
        rb.AddForce(rb.transform.right * bulletForce, ForceMode2D.Impulse);

        JouerSon(Sons.InGame.Shoot1);

        Invoke("SetCanShootToTrue", 0.33f);
	}

    void Shotgun(float shootingDirection) {
        GameObject bullet_middle = Instantiate(bulletPrefab, getFirePointPosition(playerTransform.position, shootingDirection, 0.4f), Quaternion.Euler(0, 0, shootingDirection));
        GameObject bullet_left = Instantiate(bulletPrefab, getFirePointPosition(playerTransform.position, shootingDirection, 0.4f), Quaternion.Euler(0, 0, shootingDirection + 10));
        GameObject bullet_right = Instantiate(bulletPrefab, getFirePointPosition(playerTransform.position, shootingDirection, 0.4f), Quaternion.Euler(0, 0, shootingDirection - 10));

        bullet_middle.GetComponent<Bullet>().setColor(playerColor);
        bullet_left.GetComponent<Bullet>().setColor(playerColor);
        bullet_right.GetComponent<Bullet>().setColor(playerColor);

        Rigidbody2D rb_middle = bullet_middle.GetComponent<Rigidbody2D>();
        Rigidbody2D rb_left = bullet_left.GetComponent<Rigidbody2D>();
        Rigidbody2D rb_right = bullet_right.GetComponent<Rigidbody2D>();

        rb_middle.rotation = shootingDirection;
        rb_left.rotation = shootingDirection;
        rb_right.rotation = shootingDirection;

        rb_middle.AddForce(rb_middle.transform.right * bulletForce, ForceMode2D.Impulse);
        rb_left.AddForce(rb_left.transform.right * bulletForce, ForceMode2D.Impulse);
        rb_right.AddForce(rb_right.transform.right * bulletForce, ForceMode2D.Impulse);

        JouerSon(Sons.InGame.Shoot1);

        Invoke("SetCanShootToTrue", 0.5f);
	}

    void Burst(float shootingDirection) {
        GameObject bullet_middle = Instantiate(bulletPrefab, getFirePointPosition(playerTransform.position, shootingDirection, 0.5f), Quaternion.Euler(0, 0, shootingDirection));
        GameObject bullet_left = Instantiate(bulletPrefab, getFirePointPosition(playerTransform.position, shootingDirection, 0.7f), Quaternion.Euler(0, 0, shootingDirection));
        GameObject bullet_right = Instantiate(bulletPrefab, getFirePointPosition(playerTransform.position, shootingDirection, 0.9f), Quaternion.Euler(0, 0, shootingDirection));

        bullet_middle.GetComponent<Bullet>().setColor(playerColor);
        bullet_left.GetComponent<Bullet>().setColor(playerColor);
        bullet_right.GetComponent<Bullet>().setColor(playerColor);

        Rigidbody2D rb_middle = bullet_middle.GetComponent<Rigidbody2D>();
        Rigidbody2D rb_left = bullet_left.GetComponent<Rigidbody2D>();
        Rigidbody2D rb_right = bullet_right.GetComponent<Rigidbody2D>();

        rb_middle.rotation = shootingDirection;
        rb_left.rotation = shootingDirection;
        rb_right.rotation = shootingDirection;

        rb_middle.AddForce(rb_middle.transform.right * bulletForce, ForceMode2D.Impulse);
        rb_left.AddForce(rb_left.transform.right * bulletForce, ForceMode2D.Impulse);
        rb_right.AddForce(rb_right.transform.right * bulletForce, ForceMode2D.Impulse);

        JouerSon(Sons.InGame.Shoot1);

        Invoke("SetCanShootToTrue", 0.33f);
	}

    void Shoot(float shootingDirection)
    {
        canShoot = false;
        switch (weapon)
        {
            case "shotgun":
                Shotgun(shootingDirection);
                break;
            case "burst":
                Burst(shootingDirection);
                break;
            default:
                SingleFire(shootingDirection);
                break;
		}
	}

    public void setWeapon(string w, float timeToWeaponReset=0){
        weapon = w;
        if (timeToWeaponReset == 0) return;
        Invoke("setWeaponToStandard", timeToWeaponReset);
    }
    public void setWeaponToStandard(){weapon = "standard";}

    public void JouerSon(Sons.InGame SonAJouer)
    {
        string chemin = "Sounds/InGame/" + SonAJouer.ToString();
        sfxPlayer.clip = Resources.Load(chemin, typeof(AudioClip)) as AudioClip;
        sfxPlayer.Play();
    }
}
