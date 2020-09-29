using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManagerScript : MonoBehaviour
{
    // PARAMETRES DEBUGS
    [Header("Paramètres Debug")]
    public bool CountdownEnabled = true;

    // SINGLETON
    public static GameManagerScript instance = null;

    // EVENTS
    [HideInInspector]
    public UnityEvent MAJScore;
    [HideInInspector]
    public UnityEvent HurryUpEvent;

    // PREFAB
    [Header("Prefab")]
    public GameObject PrefabPowerUp = null;

    // REFERENCES
    [Header("Références")]
    public AudioSource musicAudio = null;
    public AudioSource sfxAudio = null;
    public AudioSource menuAudio = null;
    public TextMeshProUGUI timer = null;
    public Image cacheLuminosite = null;
    public List<GameObject> listeJoueurs = new List<GameObject>();
    public PowerSpawnScript powerScript = null;
    public TimerScript powerTimerScript = null;
    public TimerScript P1RespawnTimerScript = null;
    public TimerScript P2RespawnTimerScript = null;
    public Camera camera = null;
    public GameObject arena = null; // Object that regroups all possible arenas
    public GameObject playersGameObject = null; // Object that regroups all possible players

    // PROPRIETES
    [Header("Propriétés")]
    public float gameLength = 10.0f;    // In seconds
    private bool gameHasEnded = false;
    //public float delayBeforeRestart = 1.0f;
    //public GameObject gameOverPanel
    GameParameters.TypeArena selectedArena;

    [HideInInspector]
    public bool pause = true;               // Permet de savoir quand la partie est en pause ou non (personnages peuvent bouger, timer tourne)
    [HideInInspector]
    public bool pausePossible = false;      // Permet de savoir quand il est possible de mettre pause
    [HideInInspector]
    public bool hurryUp = false;            // Permet de savoir quand on est en HurryUp

    [HideInInspector]
    public Dictionary<Color, string> players = new Dictionary<Color, string>();                     // 
    [HideInInspector]
    public Dictionary<GameObject, Vector2> spawnPoints = new Dictionary<GameObject, Vector2>();     //
    [HideInInspector]
    public Dictionary<Color, int> tilesColors = new Dictionary<Color, int>();                       // Couleur / Score

    void Awake()
    {
        // INSTANCIATION SINGLETON
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        // REFERENCES MANQUANTES
        if(PrefabPowerUp == null)
        {
            Debug.LogError("ATTENTION : Le prefab PowerUp n'a pas été renseigné dans le GameManager, veuillez compléter cette référence");
        }
        if (cacheLuminosite == null)
        {
            cacheLuminosite = GameObject.Find("CacheLuminosite").GetComponent<Image>();
        }
        if (listeJoueurs.Count == 0)
        {
            Transform playersParent = GameObject.Find("players").GetComponent<Transform>();
            foreach (Transform child in playersParent)
            {
                listeJoueurs.Add(child.transform.GetChild(0).gameObject);
            }
            Debug.Log("Nombre de joueurs détectés : " + listeJoueurs.Count);
        }
        if(powerScript == null)
        {
            powerScript = GameObject.Find("PowerSpawn").GetComponent<PowerSpawnScript>();
        }
        if(powerTimerScript == null)
        {
            powerTimerScript = GameObject.Find("PowerUpTimer").GetComponent<TimerScript>();
        }
        if (P1RespawnTimerScript == null)
        {
            P1RespawnTimerScript = GameObject.Find("P1RespawnTimer").GetComponent<TimerScript>();
        }
        if (P2RespawnTimerScript == null)
        {
            P2RespawnTimerScript = GameObject.Find("P2RespawnTimer").GetComponent<TimerScript>();
        }
        if (camera == null)
        {
            camera = Camera.main;
        }
        if (arena == null) 
        {
            arena = GameObject.Find("Arena");
		}
        if (playersGameObject == null) 
        {
            playersGameObject = GameObject.Find("players");
		}

        // On paramètre la partie avec les GameParameters du menu
        SetupInitial();

        SetActiveArena();
        SetActivePlayers();
    }

    // Update is called once per frame
    void Update()
    {
        // TIMER : On décrémente le temps au fur et à mesure
        if (!pause)
        {
            gameLength -= Time.deltaTime;
            TimeSpan ts = TimeSpan.FromSeconds(gameLength + 1f);
            timer.text = ts.ToString("mm\\:ss");
            if (!hurryUp && gameLength <= 30.0f)
            {
                hurryUp = true;
                HurryUpEvent.Invoke();
            }
            if (gameLength <= -1)
            {
                endGame();
            }
        }

        // PAUSE : On met en pause la partie
        if (pausePossible && Input.GetButtonDown("Pause"))
        {
            if (!pause)
            {
                Debug.Log("PAUSE");
                pause = true;
                Time.timeScale = 0;
                cacheLuminosite.color = new Color(cacheLuminosite.color.r, cacheLuminosite.color.g, cacheLuminosite.color.b, cacheLuminosite.color.a + 0.25f);
            }

            // REPRISE
            else
            {
                Debug.Log("UNPAUSE");
                pause = false;
                Time.timeScale = 1;
                cacheLuminosite.color = new Color(cacheLuminosite.color.r, cacheLuminosite.color.g, cacheLuminosite.color.b, cacheLuminosite.color.a - 0.25f);
            }
        }
    }

    // METHODES AXIL

    public void addPlayerToDict(Color c, string playerName)
    {
        if (!players.ContainsKey(c))
            players.Add(c, playerName);
	}

    public void addSpawnPointToDict(GameObject go, Vector2 location) //Might be useless now (since the spawn system is now included in the PlayerCharacteristics. I might delete it later
    {
        if (!spawnPoints.ContainsKey(go))
            spawnPoints.Add(go, location);
	}

    public void setMaxHealthPointsForPlayers(int hp) {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach(GameObject player in players)
        {
            player.GetComponent<PlayerCharacteristics>().setMaxHealthPoints(hp);
		}
	}

    public void setRespawnTimeForPlayers(float t) {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach(GameObject player in players)
        {
            player.GetComponent<PlayerCharacteristics>().setRespawnTime(t);
		}
	}

    public void killPlayer(GameObject player)
    {
        player.SetActive(false);
        player.transform.position = spawnPoints[player];
        player.SetActive(true);
	}

    public void addColorToDict(Color c)
    {
        if (!tilesColors.ContainsKey(c))
            tilesColors.Add(c, 0);
	}
    public void substractOneFromColor(Color c)
    {
        tilesColors[c] -= 1;
        MAJScore.Invoke();
    }
    public void addOneFromColor(Color c)
    {
        tilesColors[c] += 1;
        MAJScore.Invoke();
    }
    
    public bool getGameHasEnded(){return gameHasEnded;}

    public void getWinner() {
        int highestScore = -1;
        string winner = "No winner";
        Color winnerColor = new Color(0, 0, 0, 0);

        string looser = "No looser";
        Color looserColor = new Color(0, 0, 0, 0);

        // DETERMINATION DU GAGNANT
        foreach(KeyValuePair<Color, int> entry in tilesColors)
        {
            if (players.ContainsKey(entry.Key))
            {
                if (entry.Value > highestScore)
                {
                    highestScore = entry.Value;
                    winner = players[entry.Key];
                    winnerColor = entry.Key;
                }
                else if (entry.Value == highestScore) //If mutiple players have the same number of tiles colored
                {
                    winner = winner + " and " + players[entry.Key];
                    winnerColor = entry.Key;
			    }
            }
        }

        foreach(KeyValuePair<Color, string> entry in players) {
            if (entry.Key != winnerColor) {
                looserColor = entry.Key;
                looser = entry.Value;
			}  
		}

        // MISE A JOUR VICTORY CLASS
        VictoryClass.VictoryName = winner;
        VictoryClass.VictoryColor = winnerColor;
        VictoryClass.LooserColor = looserColor;
        VictoryClass.LooserName = looser;

        // CHARGEMENT SCENE VICTORY
        SceneManager.LoadScene("Victory");
    }

    public void endGame() 
    {
        gameHasEnded = true;
        getWinner();
    }

    // METHODES ROBIN

    public void SetupInitial()
    {
        // PAUSE INITIALE
        pause = true;

        // PARAMETRAGE VOLUME + LUMINOSITE
        menuAudio.volume = GameParameters.SfxVolume * 1.75f;
        sfxAudio.volume = GameParameters.SfxVolume;
        musicAudio.volume = GameParameters.MusicVolume;
        cacheLuminosite.color = new Color(cacheLuminosite.color.r, cacheLuminosite.color.g, cacheLuminosite.color.b, 1 - GameParameters.Luminosite);

        // PARAMETRAGE JOUEURS
        //GameParameters.Player1Character;
        //GameParameters.Player2Character;

        // PARAMETRAGE ARENE
        selectedArena = GameParameters.SelectedArena;

        // PARAMETRAGE PARTIE (Temps Respawn, Temps partie, Taux apparition powersUps, PVs)
        setRespawnTimeForPlayers(GameParameters.getRespawnTimeInSeconds());
        gameLength = GameParameters.getTimeModeInSeconds();
        TimeSpan ts = TimeSpan.FromSeconds(gameLength);
        timer.text = ts.ToString("mm\\:ss");
        powerScript.SetFrequenceSpawn(GameParameters.PowerUpsFrequency);
        setMaxHealthPointsForPlayers(GameParameters.getMaxHealthPointsSetting());

        // MUSIQUE ALEATOIRE
        System.Random rnd = new System.Random();
        int nbMusic = rnd.Next(1, 4);
        if (nbMusic == 1)
        {
            JouerMusic(Sons.Musics.Action1);
        }
        if (nbMusic == 2)
        {
            JouerMusic(Sons.Musics.Action2);
        }
        if (nbMusic == 3)
        {
            JouerMusic(Sons.Musics.Action3);
        }

        Debug.Log("Paramètres partie (1) : " + "Volume Musique : " + GameParameters.MusicVolume + " / Volume Son : " + GameParameters.SfxVolume);
        //Debug.Log("Paramètres partie (2) : " + "Player1 : " + GameParameters.Player1Character + " / Player2 : " + GameParameters.Player2Character);
        //Debug.Log("Paramètres partie (3) : " + "Arène : " + GameParameters.SelectedArena);
        Debug.Log("Paramètres partie (4) : " + "Temps : " + GameParameters.Time + " / PowerUps : " + GameParameters.PowerUpsFrequency + " / Respawn : " + GameParameters.RespawnTime  + " / PV : " + GameParameters.PVMode);
    }

    public void JouerSonMenu(Sons.Menu SonAJouer)
    {
        string chemin = "Sounds/Menu/" + SonAJouer.ToString();
        menuAudio.clip = Resources.Load(chemin, typeof(AudioClip)) as AudioClip;
        menuAudio.Play();
    }

    public void JouerSonVoix(Sons.Voice SonAJouer)
    {
        string chemin = "Sounds/Voice/" + SonAJouer.ToString();
        menuAudio.clip = Resources.Load(chemin, typeof(AudioClip)) as AudioClip;
        menuAudio.Play();
    }

    public void JouerSonJeu(Sons.InGame SonAJouer)
    {
        string chemin = "Sounds/InGame/" + SonAJouer.ToString();
        sfxAudio.clip = Resources.Load(chemin, typeof(AudioClip)) as AudioClip;
        sfxAudio.Play();
    }

    public void JouerMusic(Sons.Musics MusicAJouer)
    {
        string chemin = "Musics/" + MusicAJouer.ToString();
        musicAudio.clip = Resources.Load(chemin, typeof(AudioClip)) as AudioClip;
        musicAudio.Play();
    }

    private void SetActiveArena() {

        // RECUP POWER UPS SPAWN
        Transform Power1 = powerScript.transform.GetChild(0);
        Transform Power2 = powerScript.transform.GetChild(1);
        Transform Power3 = powerScript.transform.GetChild(2);

        switch (selectedArena) {
            case GameParameters.TypeArena.Ares:
                // ACTIVATION ARENE
                arena.transform.Find("Ares").gameObject.SetActive(true);
                // PLACEMENT POWER UPS
                // Aucun, placement par défaut
                break;
            case GameParameters.TypeArena.OddBone:
                // ACTIVATION ARENE
                arena.transform.Find("OddBone").gameObject.SetActive(true);
                // PLACEMENT POWER UPS
                Power1.localPosition = new Vector3(-7.4f, -5.74f, 0f);
                Power2.localPosition = new Vector3(17.6f, 3.26f, 0f);
                Power3.localPosition = new Vector3(5.1f, -1.27f, 0f);
                break;
            case GameParameters.TypeArena.Sonar:
                // ACTIVATION ARENE
                arena.transform.Find("Sonar").gameObject.SetActive(true);
                // PLACEMENT POWER UPS
                Power1.localPosition = new Vector3(-9.4f, -5.74f, 0f);
                Power2.localPosition = new Vector3(15.6f, 5.26f, 0f);
                Power3.localPosition = new Vector3(3.1f, 4.26f, 0f);
                break;
            default:
                arena.transform.Find("Ares").gameObject.SetActive(true);
                break;
		}
	}

    private void SetActivePlayers() {
        GameObject p1 = playersGameObject.transform.Find("P1").gameObject;
        GameObject p2 = playersGameObject.transform.Find("P2").gameObject;

        //First, we need to set the position of those players;
        switch (selectedArena) {
        case GameParameters.TypeArena.Ares:
            break;
        case GameParameters.TypeArena.OddBone:
            p1.transform.position = new Vector3(-19f, 4f, p1.transform.position.z);
            p2.transform.position = new Vector3(20f, -8f, p2.transform.position.z);
            break;
        case GameParameters.TypeArena.Sonar:
            p1.transform.position = new Vector3(-21f, 5f, p1.transform.position.z);
            p2.transform.position = new Vector3(19f, -7f, p2.transform.position.z);
            break;
        default:
            break;
		}

        p1.SetActive(true);
        p2.SetActive(true);

        p1.transform.Find("France").gameObject.SetActive(true);
        p2.transform.Find("England").gameObject.SetActive(true);
	}
}
