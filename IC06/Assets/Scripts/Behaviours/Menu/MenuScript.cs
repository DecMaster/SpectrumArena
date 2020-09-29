using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuScript : MonoBehaviour
{
    // SINGLETON : Evite de transformer toutes les autres variables en variables statiques, vu que l'on renseignera cette Toolbox à la place
    public static MenuScript instance = null;

    // REFERENCES OBJETS
    [Header("Références")]
    public GameObject logo = null;
    public GameObject mainMenu = null;
    public GameObject optionsMenu = null;
    public GameObject creditsMenu = null;
    public GameObject controlsMenu = null;
    public GameObject charactersMenu = null;
    public GameObject parametersMenu = null;
    public Image cacheLuminosite = null;
    public Image background = null;
    public GameObject lastselect;

    // REFERENCES COMPONENTS
    public AudioSource musicAudio = null;
    public AudioSource sfxAudio = null;
    public PressStartScript scriptPressStart = null;
    public ParametersMenuScript parametersMenuScript = null;

    // PROPRIETES PUBLIQUES
    [Header("Propriétés")]
    [Range(0.5f, 5f)]
    public float vitesseCredits = 2f;

    // PROPRIETES PRIVATE
    bool isStarted = false;
    bool creditsON = false;
    bool controlsON = false;
    bool creditsOK = false;
    bool bufferSon = true;

    // ANIMATIONS
    private Vector3 positionBasCredits;
    private Vector3 positionHauteCredits = new Vector3(0,1,0);

    // METHODES
    private void Awake()
    {
        // INSTANCIATION SINGLETON
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        // REFERENCES MANQUANTES
        if (logo == null)
        {
            logo = this.transform.Find("TitleLogo").gameObject;
        }

        if(background == null)
        {
            background = this.transform.Find("Background").GetComponent<Image>();
        }

        if (mainMenu == null)
        {
            mainMenu = this.transform.Find("MainMenu").gameObject;
        }

        if (parametersMenu == null)
        {
            parametersMenu = this.transform.Find("ParametersMenu").gameObject;
        }

        if (charactersMenu == null)
        {
            charactersMenu = this.transform.Find("CharactersMenu").gameObject;
        }

        if (optionsMenu == null)
        {
            optionsMenu = this.transform.Find("OptionsMenu").gameObject;
        }

        if (creditsMenu == null)
        {
            creditsMenu = this.transform.Find("CreditsMenu").gameObject;
        }

        if (controlsMenu == null)
        {
            controlsMenu = this.transform.Find("ControlsMenu").gameObject;
        }

        if (scriptPressStart == null)
        {
            scriptPressStart = this.transform.Find("PressStart").GetComponent<PressStartScript>();
        }

        if(cacheLuminosite == null)
        {
            cacheLuminosite = this.transform.Find("CacheLuminosite").GetComponent<Image>();
        }

        if (parametersMenu != null && parametersMenuScript == null)
        {
            parametersMenuScript = parametersMenu.transform.Find("Parameters").GetComponent<ParametersMenuScript>();
        }

        positionBasCredits = creditsMenu.transform.localPosition;

        // CACHE SOURIS
        if (!Application.isEditor)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    private void Start()
    {
        // ECRAN DE DEPART
        InitAffichage();

        // CURSEUR SOURIS INVISIBLE
        if (!Application.isEditor)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    private void Update()
    {
        // VALIDATION MENU
        if (Input.GetButtonDown("Submit") || Input.anyKeyDown){
            // DETECTION START (Ecran Titre)
            if (isStarted == false)
            {
                isStarted = true;
                scriptPressStart.Accelerer();
            }
        }

        // CORRECTION SOURIS
        if (EventSystem.current.currentSelectedGameObject == null)
        {
            EventSystem.current.SetSelectedGameObject(lastselect);
        }
        else
        {
            lastselect = EventSystem.current.currentSelectedGameObject;
        }

        // RETOUR DES CREDITS
        if (creditsOK)
        {
            for (int i = 0; i < 20; i++)
            {
                if (Input.GetKeyDown("joystick button " + i) || Input.GetButtonDown("Cancel"))
                {
                    if (creditsON)
                    {
                        LeaveCredits();
                    }
                    else if (controlsON)
                    {
                        LeaveControls();
                    }
                }
            }
        }

        // NAVIGATION MENU
        if (bufferSon && !creditsON && !controlsON && !charactersMenu.activeInHierarchy && scriptPressStart.gameObject.activeInHierarchy == false && (Input.GetButtonDown("Vertical") || Input.GetAxisRaw("Vertical") != 0))
        {
            // JOUE SON
            bufferSon = false;
            Invoke("SetBufferON", 0.2f);
            JouerSon(Sons.Menu.Move);
        }

        if (bufferSon && !creditsON && !controlsON && !optionsMenu.activeInHierarchy && scriptPressStart.gameObject.activeInHierarchy == false && (Input.GetButtonDown("Horizontal") || Input.GetAxisRaw("Horizontal") != 0))
        {
            // JOUE SON 
            bufferSon = false;
            Invoke("SetBufferON", 0.2f);
            JouerSon(Sons.Menu.Move);
        }
    }

    // METHODES BOUTONS

    public void LaunchGame()
    {
        SceneManager.LoadScene("InGame");
    }

    public void EnterParameters()
    {
        // JOUE SON
        JouerSon(Sons.Menu.Select);

        // MAJ AFFICHAGE
        logo.SetActive(false);
        mainMenu.SetActive(false);
        charactersMenu.SetActive(false);
        parametersMenu.SetActive(true);
        parametersMenuScript.ArenaButton.Select();
    }

    public void EnterOptions()
    {
        // JOUE SON
        JouerSon(Sons.Menu.Select);

        // MAJ AFFICHAGE
        logo.SetActive(false);
        mainMenu.SetActive(false);
        optionsMenu.SetActive(true);
        optionsMenu.transform.Find("VolumeSlider").GetComponent<Slider>().Select();
    }

    public void EnterCharacters()
    {
        // JOUE SON
        JouerSon(Sons.Menu.Select);

        // CHANGEMENT BACKGROUND
        background.sprite = Resources.Load("Images/CharactersScreen", typeof(Sprite)) as Sprite;

        // MAJ AFFICHAGE
        charactersMenu.SetActive(true);
        logo.SetActive(false);
        mainMenu.SetActive(false);
        optionsMenu.transform.Find("VolumeSlider").GetComponent<Slider>().Select();
    }

    public void LeaveCharacters()
    {
        // JOUE SON
        JouerSon(Sons.Menu.Back);

        // CHANGEMENT BACKGROUND
        background.sprite = Resources.Load("Images/MenuBackground", typeof(Sprite)) as Sprite;

        // MAJ AFFICHAGE
        charactersMenu.SetActive(false);
        logo.SetActive(true);
        mainMenu.SetActive(true);
        optionsMenu.transform.Find("VolumeSlider").GetComponent<Slider>().Select();
    }

    public void EnterCredits()
    {
        // JOUE SON
        JouerSon(Sons.Menu.Select);
        JouerMusic(Sons.Musics.Ending);

        // JOUER ANIMATION
        StopAllCoroutines();
        StartCoroutine(CreditsON());

        // MAJ AFFICHAGE
        logo.SetActive(false);
        mainMenu.SetActive(false);
        optionsMenu.SetActive(false);
        creditsMenu.SetActive(true);

    }

    public void EnterControls()
    {
        // JOUE SON
        JouerSon(Sons.Menu.Select);

        // JOUER ANIMATION
        StopAllCoroutines();
        StartCoroutine(ControlsON());

        // MAJ AFFICHAGE
        logo.SetActive(false);
        mainMenu.SetActive(false);
        optionsMenu.SetActive(false);
        controlsMenu.SetActive(true);

    }

    public void LeaveOptions()
    {
        // JOUE SON
        JouerSon(Sons.Menu.Back);

        // MAJ AFFICHAGE
        logo.SetActive(true);
        mainMenu.SetActive(true);
        optionsMenu.SetActive(false);
        mainMenu.transform.Find("PlayButton").GetComponent<Button>().Select();
    }

    public void LeaveCredits()
    {
        // JOUE SON
        JouerSon(Sons.Menu.Back);
        JouerMusic(Sons.Musics.Title);

        // JOUER ANIMATION
        StopAllCoroutines();
        StartCoroutine(CreditsOFF());

        // MAJ AFFICHAGE
        logo.SetActive(true);
        mainMenu.SetActive(true);
        optionsMenu.SetActive(false);
        mainMenu.transform.Find("PlayButton").GetComponent<Button>().Select();
    }

    public void LeaveControls()
    {
        // JOUE SON
        JouerSon(Sons.Menu.Back);

        // JOUER ANIMATION
        StopAllCoroutines();
        StartCoroutine(ControlsOFF());

        // MAJ AFFICHAGE
        logo.SetActive(true);
        mainMenu.SetActive(true);
        optionsMenu.SetActive(false);
        mainMenu.transform.Find("PlayButton").GetComponent<Button>().Select();
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    // METHODES GENERALES

    private void SetBufferON()
    {
        bufferSon = true;
    }

    private void InitAffichage()
    {
        background.sprite = Resources.Load("Images/MenuBackground", typeof(Sprite)) as Sprite;
        background.gameObject.SetActive(true);
        logo.SetActive(true);
        mainMenu.SetActive(false);
        optionsMenu.SetActive(false);
        parametersMenu.SetActive(false);
        charactersMenu.SetActive(false);
        creditsMenu.SetActive(false);
        controlsMenu.SetActive(false);
        scriptPressStart.gameObject.SetActive(true);
        musicAudio.volume = 0.5f;
        sfxAudio.volume = 0.5f;
    }

    public void ToggleFullscreen()
    {
        Screen.fullScreen = !Screen.fullScreen;
    }

    public void JouerSon(Sons.Menu SonAJouer)
    {
        string chemin = "Sounds/Menu/" + SonAJouer.ToString();
        sfxAudio.clip = Resources.Load(chemin, typeof(AudioClip)) as AudioClip;
        sfxAudio.Play();
    }

    public void JouerVoix(Sons.Voice VoixAJouer)
    {
        string chemin = "Sounds/Voice/" + VoixAJouer.ToString();
        sfxAudio.clip = Resources.Load(chemin, typeof(AudioClip)) as AudioClip;
        sfxAudio.Play();
    }

    public void JouerMusic(Sons.Musics MusicAJouer)
    {
        string chemin = "Musics/" + MusicAJouer.ToString();
        musicAudio.clip = Resources.Load(chemin, typeof(AudioClip)) as AudioClip;
        musicAudio.Play();
    }

    // ANIMATION
    private IEnumerator CreditsON()
    {
        // INIT
        creditsMenu.transform.localPosition = positionBasCredits;
        creditsON = true;
        float delta = 0f;

        // ANIMATION
        while (delta < 1)
        {
            // INCREMENTATION
            delta += Time.deltaTime;

            // POSITION
            creditsMenu.transform.localPosition = Vector3.Lerp(creditsMenu.transform.localPosition, positionHauteCredits, delta);

            yield return new WaitForEndOfFrame();
        }

        creditsOK = true;
        yield return null;
    }

    private IEnumerator CreditsOFF()
    {
        // INIT
        creditsMenu.transform.localPosition = positionHauteCredits;
        creditsON = false;
        creditsOK = false;
        float delta = 0f;

        // ANIMATION
        while (delta < 1)
        {
            // INCREMENTATION
            delta += Time.deltaTime;

            // POSITION
            creditsMenu.transform.localPosition = Vector3.Lerp(creditsMenu.transform.localPosition, positionBasCredits, delta);

            yield return new WaitForEndOfFrame();
        }
        creditsMenu.SetActive(false);
        yield return null;
    }

    private IEnumerator ControlsON()
    {
        // INIT
        controlsMenu.transform.localPosition = positionBasCredits;
        controlsON = true;
        float delta = 0f;

        // ANIMATION
        while (delta < 1)
        {
            // INCREMENTATION
            delta += Time.deltaTime;

            // POSITION
            controlsMenu.transform.localPosition = Vector3.Lerp(controlsMenu.transform.localPosition, positionHauteCredits, delta);

            yield return new WaitForEndOfFrame();
        }

        creditsOK = true;
        yield return null;
    }

    private IEnumerator ControlsOFF()
    {
        // INIT
        controlsMenu.transform.localPosition = positionHauteCredits;
        controlsON = false;
        creditsOK = false;
        float delta = 0f;

        // ANIMATION
        while (delta < 1)
        {
            // INCREMENTATION
            delta += Time.deltaTime;

            // POSITION
            controlsMenu.transform.localPosition = Vector3.Lerp(controlsMenu.transform.localPosition, positionBasCredits, delta);

            yield return new WaitForEndOfFrame();
        }
        controlsMenu.SetActive(false);
        yield return null;
    }
}
