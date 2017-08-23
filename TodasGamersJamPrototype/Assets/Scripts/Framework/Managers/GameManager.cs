using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    #region "Delegates"

    public delegate void GameEvent();

    public delegate void GameEventParam(System.Object param);

    #endregion

    #region "Initialization"

    private bool initialized = false;

    /// <summary>
    /// Initilizes the engine, only if it wans't already initialized
    /// </summary>
    public void Initialize()
    {
        if (!initialized)
        {
            initialized = true;
       
            //create subsystems
            this.settings = new GameSettings();
            this.logic = new LogicManager();
            this.ui = new UIManager();
            this.input = new InputManager();
            this.options = new OptionsManager();
            this.flags = new FlagsManager();
            this.savegame = new Savegame();
            this.audioManager = new AudioManager();
            this.adsManager = new AdsManager();

            //initialize subsystems
            this.settings.Initialize();
            this.logic.Initialize();
            this.ui.Initialize();
            this.input.Initialize();
            this.options.Initialize();
            this.flags.Initialize();
            this.audioManager.Initialize();
            this.adsManager.Initialize();

        }

    }

    #endregion

    #region "Update"

    void Update()
    {
        UpdateManagers();
    }

    /// <summary>
    /// Update all game subsystems
    /// </summary>
    public void UpdateManagers()
    {
        //call Update on those manager who require so
        this.input.Update();
        this.audioManager.Update();
    }

    #endregion

    #region "Logic"

    private LogicManager logic;

    /// <summary>
    /// Logic manager of the game
    /// </summary>
    public LogicManager Logic { get { return this.logic; } }

    #endregion

    #region "User Interface"

    private UIManager ui;

    /// <summary>
    /// Returns the UI manager
    /// </summary>
    public UIManager UI { get { return ui; } }

    #endregion

    #region "Input Manager"

    private InputManager input;

    /// <summary>
    /// Returns the Input Manager of the game
    /// </summary>
    public InputManager Input { get { return this.input; } }

    #endregion

    #region "Options"

    private OptionsManager options;

    /// <summary>
    /// Game Options management
    /// </summary>
    public OptionsManager Options { get { return this.options; } }

    #endregion

    #region "Settings"

    private GameSettings settings;

    public GameSettings Settings { get { return this.settings; } }

    #endregion

    #region "Flags"

    private FlagsManager flags;

    /// <summary>
    /// Flags: Logic variables that are persisted in the savegame
    /// </summary>
    public FlagsManager Flags { get { return this.flags; } }

    #endregion

    #region "Savegame"

    private Savegame savegame;

    public Savegame Savegame { get { return this.savegame; } }

    #endregion

    #region "Audio"

    private AudioManager audioManager;

    public AudioManager Audio { get { return this.audioManager; } }

    #endregion

    #region Advertisement

    private AdsManager adsManager;

    public AdsManager AdsManager { get { return this.adsManager; } }

    #endregion

}
