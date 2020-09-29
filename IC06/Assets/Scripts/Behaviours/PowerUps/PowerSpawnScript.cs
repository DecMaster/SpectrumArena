using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerSpawnScript : MonoBehaviour
{
    // ENUM
    public enum TypePowerUps
    {
        shield,
        //bouncy,
        footsteps,
        swap,
        shotgun,
        burst
    }

    // PROPRIETES PUBLIQUE
    [Range(5f, 10f)]
    public float dureePowerUps = 8f;
    [Range(10f, 20f)]
    public float tempsMinSpawn = 10f;
    [Range(20f, 40f)]
    public float tempsMaxSpawn = 20f;
    [HideInInspector]
    public bool apparitionPossible = false;

    // PROPRIETES PRIVATE
    private bool powerUpAllowed = true;
    private List<Transform> listeSpawn = new List<Transform>();

    // METHODES

    void Update()
    {
        // Planification de la prochaine apparition
        if (apparitionPossible && powerUpAllowed)
        {
            apparitionPossible = false;
            System.Random random = new System.Random();
            float duree = (float) random.NextDouble() * (tempsMaxSpawn - tempsMinSpawn) + tempsMinSpawn;
            Invoke("Apparition", duree);
        }
    }

    public void SetFrequenceSpawn(GameParameters.PowerUpsMode mode)
    {
        switch (mode)
        {
            case GameParameters.PowerUpsMode.Aucun:
                powerUpAllowed = false;
                break;
            case GameParameters.PowerUpsMode.Normal:
                // On ne fait rien, parametrage normal
                break;
            case GameParameters.PowerUpsMode.Beaucoup:
                tempsMaxSpawn = tempsMinSpawn;
                break;
            default:
                break;
        }
    }

    private void Apparition()
    {
        // Choix de l'emplacement
        Transform spawn = ChoixSpawn();

        // Choix du PowerUps
        TypePowerUps type = ChoixPowerUps();

        // Instanciation du PowerUp
        GameObject PowerUpObject = GameObject.Instantiate(GameManagerScript.instance.PrefabPowerUp, spawn);
        PowerUpsScript PowerScript = PowerUpObject.GetComponent<PowerUpsScript>();
        PowerScript.type = type;
        PowerScript.duration = dureePowerUps;

        Debug.Log("Apparition d'un PowerUp : Type = " + type + " / Spawn = " + spawn.localPosition);
    }

    // Permet de choisir la position d'apparition du PowerUps parmi les Spawns de l'arène (fils de l'objet)
    private Transform ChoixSpawn()
    {
        // 1 - Récupération des Spawns (enfants)
        if(listeSpawn.Count == 0)
        {
            foreach(Transform child in this.transform)
            {
                listeSpawn.Add(child);
            }
            Debug.Log("Nombre de points de spawn de PowerUps détectés : " + listeSpawn.Count);
        }

        // 2 - Choix du meilleur spawn
        Transform spawnChoisi = null;
        float DistanceRetenue = 0f;
        // On choisi le spawn le plus éloigné des joueurs
        foreach(Transform spawn in listeSpawn)
        {
            // Détermination de la plus petite distance
            float DistanceJ1 = Vector3.Distance(spawn.position, GameManagerScript.instance.listeJoueurs[0].transform.position);
            float DistanceJ2 = Vector3.Distance(spawn.position, GameManagerScript.instance.listeJoueurs[1].transform.position);
            float plusPetiteDistance = 0f;
            if(DistanceJ1 <= DistanceJ2)
            {
                plusPetiteDistance = DistanceJ1;
            }
            else
            {
                plusPetiteDistance = DistanceJ2;
            }

            // Détermination du spawn le plus loin des 2 joueurs équitablement
            if(spawnChoisi == null)
            {
                spawnChoisi = spawn;
                DistanceRetenue = plusPetiteDistance;
            }
            else
            {
                if(DistanceRetenue < plusPetiteDistance)
                {
                    spawnChoisi = spawn;
                    DistanceRetenue = plusPetiteDistance;
                }
            }
        }

        // 3 - On renvoit le Transform du spawn choisi
        return spawnChoisi;
    }

    private TypePowerUps ChoixPowerUps()
    {
        Array values = Enum.GetValues(typeof(TypePowerUps));
        System.Random random = new System.Random();
        return (TypePowerUps) values.GetValue(random.Next(values.Length));
    }


}
