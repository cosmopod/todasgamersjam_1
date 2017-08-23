using UnityEngine;
using System.Collections;
using JSON;
using System;

public class OptionsManager
{
    
    #region "Game Options"

    public class GameOptions : StorageManager.IJSONSerializable
    {

        private bool ccEnabled;
        private float sfxVolume;
        private float musicVolume;
        private string language;

        #region "Public Accessors"

        public bool CCEnabled
        {
            get { return ccEnabled; }
            set { ccEnabled = value; }
        }

        public float SfxVolume
        {
            get { return sfxVolume; }
            set { sfxVolume = value; }
        }

        public float MusicVolume
        {
            get{return musicVolume;}
            set{musicVolume = value;}
        }

        public string Language
        {
            get{return language;}
            set{language = value;}
        }

        #endregion

        #region "JSON Serialization"

        public void LoadDefaults()
        {
            this.sfxVolume = 1.0f;
            this.musicVolume = 0.75f;
            this.ccEnabled = false;
            this.language = "en";
        }

        public void Deserialize(JSONObject j)
        {
            this.sfxVolume = (float)j.GetNumber("sfxVolume");
            this.musicVolume = (float)j.GetNumber("musicVolume");
            this.ccEnabled = j.GetBoolean("ccEnabled");
            this.language = j.GetString("language");
        }

        public void Serialize(JSONObject j)
        {
            j.Add("sfxVolume", new JSONValue((double)this.sfxVolume));
            j.Add("musicVolume", new JSONValue((double)this.musicVolume));
            j.Add("ccEnabled", new JSONValue(this.ccEnabled));
            j.Add("language", new JSONValue(this.language));
        }

        #endregion

    }

    private GameOptions options;

    public GameOptions Options { get { return this.options; } }

    #endregion

    #region "Initialization"

    public void Initialize()
    {
        this.options = new GameOptions();
        this.options.LoadDefaults();
    }

    #endregion

    #region "Save & Load"

    /// <summary>
    /// Load options from disk
    /// </summary>
    public void Load()
    {
        StorageManager.LoadFromStorage("OPTIONS", this.options);
    }

    /// <summary>
    /// Save options to disk
    /// </summary>
    public void Save()
    {
        StorageManager.SaveToStorage("OPTIONS", this.options);
    }

    #endregion
    
}
