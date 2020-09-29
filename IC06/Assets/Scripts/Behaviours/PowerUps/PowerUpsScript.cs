using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PowerUpsScript : MonoBehaviour
{
    // REFERENCES
    public TextMeshPro texte;
    public GameObject powerObject;

    // PROPRIETES PUBLIQUES
    public PowerSpawnScript.TypePowerUps type;
    public float duration;

    // METHODES
    void Start()
    {
        // REFERENCES MANQUANTES
        if (texte == null)
        {
            texte = transform.Find("Text").GetComponent<TextMeshPro>();
        } 
        if(powerObject == null)
        {
            powerObject = transform.Find("Object").gameObject;
        }
    }

    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        // Si un joueur touche le PowerUp
        if (col.tag == "Player")
        {
            Take(col.transform.parent.name);
            applyEffectToPlayer(col.gameObject);
        }
    }

    private void Take(string playerName)
    {
        // SON
        GameManagerScript.instance.JouerSonJeu(Sons.InGame.PowerUp);

        // VISUEL OBJET
        GetComponent<BoxCollider2D>().enabled = false;
        powerObject.SetActive(false);

        // TIMER
        if(type != PowerSpawnScript.TypePowerUps.shield && type != PowerSpawnScript.TypePowerUps.swap)
        {
            GameManagerScript.instance.powerTimerScript.LaunchTimer(duration);
            GameManagerScript.instance.powerTimerScript.playerName = playerName;
        }

        // ANIMATION TEXTE
        texte.rectTransform.localPosition = powerObject.transform.localPosition;
        texte.text = type.ToString();
        StartCoroutine(TextAnimation());

        // MAJ SPAWN
        GameManagerScript.instance.powerScript.apparitionPossible = true;
    }

    private IEnumerator TextAnimation()
    {
        // INIT
        float delta = 0f;

        // ANIMATION
        while (delta < 1)
        {
            // INCREMENTATION
            delta += Time.deltaTime;

            // POSITION
            texte.transform.Translate(new Vector3(0, delta / 100, 0));

            yield return new WaitForEndOfFrame();
        }

        // DESTRUCTION OBJET
        Destroy(this.gameObject);
        yield return null;
    }

    private void applyEffectToPlayer(GameObject player) {
        PlayerCharacteristics playerCharacteristics = player.GetComponent<PlayerCharacteristics>();
        Shooting playerShootingScript = player.GetComponent<Shooting>();

        switch (type) {
        case PowerSpawnScript.TypePowerUps.shield:
            playerCharacteristics.addShieldPoints(); // optionnal parameter : quantity of shield points to add (by default : 1).
            break;

        case PowerSpawnScript.TypePowerUps.footsteps:
            playerCharacteristics.activateFootStepsPowerUp(duration);
            break;
           
        case PowerSpawnScript.TypePowerUps.shotgun:
            playerShootingScript.setWeapon("shotgun", 10.0f);
            break;

        case PowerSpawnScript.TypePowerUps.burst:
            playerShootingScript.setWeapon("burst", 10.0f);
            break;

        case PowerSpawnScript.TypePowerUps.swap:
            Debug.Log(GameManagerScript.instance.listeJoueurs);
            foreach(GameObject p in GameManagerScript.instance.listeJoueurs){
                if (player != p) {
                    Vector3 previousPlayerPosition = player.transform.position;
                    player.transform.position = p.transform.position;
                    p.transform.position = previousPlayerPosition;
				}
			}
            break;

        default:
            Debug.Log("Failed to apply power up : unknown power up type");
            break;
		}
	}
}
