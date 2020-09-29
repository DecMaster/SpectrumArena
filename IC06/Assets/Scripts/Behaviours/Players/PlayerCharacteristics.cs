using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCharacteristics : MonoBehaviour
{
    // PLAYER CARACTERISTICS
    public SpriteRenderer playerSprites;
    public Color playerColor = new Color(1,1,0,1);
    public string playerName = "CountryName";
    public int maxHealthPoints = 3;
    public int healthPoints;
    public int shieldPoints = 0;
    public int maxShieldPoints = 3;
    public bool isAlive = true;
    public float respawnTime = 3.0f;
    public Shooting shootScript;
    public FootstepsScript footstepsScript;
    public GameObject shieldVisual;
    Vector3 playerStartingPosition;

    // HEALTH BAR
    public GameObject HealthBar;
    public Image Fill;
    public Image Frame;
    public Image Back;

    // Start is called before the first frame update
    void Start()
    {
        if(shootScript == null)
        {
            shootScript = GetComponent<Shooting>();
        }
        if (footstepsScript == null)
        {
            footstepsScript = transform.Find("FootCollider").GetComponent<FootstepsScript>();
        }
        if(shieldVisual == null)
        {
            shieldVisual = transform.Find("Shield").gameObject;
            shieldVisual.GetComponent<SpriteRenderer>().color = new Color(playerColor.r, playerColor.g, playerColor.b, 0.2f);
        }
        if (playerSprites == null)
        {
            playerSprites = GetComponent<SpriteRenderer>();
        }
        if (HealthBar == null)
        {
            HealthBar = transform.Find("HealthBar").gameObject;
        }
        if (Fill == null)
        {
            Fill = HealthBar.transform.Find("Fill").GetComponent<Image>();
        }
        if (Frame == null)
        {
            Frame = HealthBar.transform.Find("Frame").GetComponent<Image>();
        }
        if (Back == null)
        {
            Back = HealthBar.transform.Find("Back").GetComponent<Image>();
        }
        HealthBar.SetActive(false);

        GameManagerScript.instance.addColorToDict(playerColor);
        GameManagerScript.instance.addPlayerToDict(playerColor, playerName);
        GameManagerScript.instance.addSpawnPointToDict(this.gameObject, this.gameObject.transform.position);

        playerStartingPosition = this.gameObject.transform.position;

        healthPoints = maxHealthPoints;

        Debug.Log(playerName + " : " 
            + " maxHP : " + maxHealthPoints + " HP"
            + " Respawn time : " + respawnTime + "s"
            );
    }

    public void Respawn() {
        this.gameObject.transform.position = playerStartingPosition;
        shootScript.canShoot = true;
    }

    public void Die(){

        // ARRET TIMER SI POWER UP
        if(GameManagerScript.instance.powerTimerScript.playerName == this.transform.parent.name)
        {
            GameManagerScript.instance.powerTimerScript.StopAllCoroutines();
            GameManagerScript.instance.powerTimerScript.gameObject.SetActive(false);
        }

        // LANCEMENT TIMER RESPAWN
        //Vector2 screenPosition = GameManagerScript.instance.camera.WorldToViewportPoint(playerStartingPosition);
        if (GameManagerScript.instance.P1RespawnTimerScript.gameObject.activeInHierarchy == false)
        {
            // DETERMINATION POSITION
            GameManagerScript.instance.P1RespawnTimerScript.rectTransform.position = playerStartingPosition;
            GameManagerScript.instance.P1RespawnTimerScript.LaunchTimer(respawnTime);
        }
        else if (GameManagerScript.instance.P1RespawnTimerScript.gameObject.activeInHierarchy == false)
        {
            GameManagerScript.instance.P2RespawnTimerScript.rectTransform.position = playerStartingPosition;
            GameManagerScript.instance.P2RespawnTimerScript.LaunchTimer(respawnTime);
        }

        // LANCEMENT RESPAWN
        this.gameObject.transform.position = playerStartingPosition + new Vector3(1, 1, 1)*100000;
        shootScript.canShoot = false;
        Invoke("Respawn", respawnTime);
	}

    public string getName(){return playerName;}
    public Color getPlayerColor(){return playerColor;}

    public void takeDamage(int damage = 1){

        // LOGIQUE
        if (shieldPoints > 0) {
            shieldPoints -= damage;
            // SI BOUCLIER : Enlever bouclier
            shieldVisual.SetActive(false);
            return;
		}
        healthPoints -= damage;

        if(healthPoints > 0)
        {
            // SON : Un aléatoire parmi les 3 son de hit
            System.Random random = new System.Random();
            int n = random.Next(0, 2);
            switch (n)
            {
                case 0:
                    GameManagerScript.instance.JouerSonJeu(Sons.InGame.Hit2);
                    break;
                case 1:
                    GameManagerScript.instance.JouerSonJeu(Sons.InGame.Hit2);
                    break;
                case 2:
                    GameManagerScript.instance.JouerSonJeu(Sons.InGame.Hit2);
                    break;
                default:
                    break;
            }
        }
        else
        {
            // SON MORT : A modifier si on en trouve un
            GameManagerScript.instance.JouerSonJeu(Sons.InGame.Hit2);

            Die();
            healthPoints = maxHealthPoints;
        }

        // ANIMATION
        StartCoroutine(HitAnimation(3, 0.05f));
    }

    public int getShieldPoints() {return shieldPoints;}

    public void setShieldPoints(int sp = -1) {
        if (sp < 0 || sp >= maxShieldPoints) {
            shieldPoints = maxShieldPoints;
            return;
		}
        shieldPoints = sp;
    }

    public void addShieldPoints(int sp = 1) {
        if (sp <= 0) return;
        shieldPoints += sp;
        if (shieldPoints > maxShieldPoints) shieldPoints = maxShieldPoints;

        // AJOUT BOUCLIER VISUEL
        shieldVisual.SetActive(true);
    }

    public void addHP(int quantity = 1){
        if (quantity <= 0) return;
        healthPoints += quantity;
        if (healthPoints > maxHealthPoints) healthPoints = maxHealthPoints;
    }
    public void die(){isAlive = false;}

    public void setRespawnTime(float t){
        if (t < 0) {
            Debug.Log("Respawn time must be positive");
            return;
		}
        respawnTime = t;
    }

    public void setMaxHealthPoints(int hp){
        if (hp <= 0) {
            Debug.Log("MaxHealthPoints has to be at least 1");
            return;
		}
        maxHealthPoints = hp;
    }


    public void activateFootStepsPowerUp(float effectDuration) {
        Debug.Log("OK");
        footstepsScript.ok = true;
        footstepsScript.color = playerColor;
        Invoke("deactivateFootStepsPowerUp", effectDuration);
	}
    public void deactivateFootStepsPowerUp() {
        Debug.Log("PAS OK");
        footstepsScript.ok = false;
	}

    // On fait clignoter le personnage lorsqu'il se fait toucher
    private IEnumerator HitAnimation(int nbRepetition, float tempsAttente)
    {
        // AFFICHAGE BARRE VIE
        Fill.fillAmount = ((float) healthPoints / (float) maxHealthPoints);
        HealthBar.SetActive(true);

        // CLIGNOTEMENT
        do
        {
            // Blanc
            playerSprites.color = Color.black;
            yield return new WaitForSeconds(tempsAttente);
            // Normal
            playerSprites.color = playerColor;
            yield return new WaitForSeconds(tempsAttente);
            nbRepetition--;
        }
        while (nbRepetition > 0);

        // RETIRE BARRE DE VIE
        yield return new WaitForSeconds(tempsAttente * 5);
        HealthBar.SetActive(false);

        yield return null;
    }
}
