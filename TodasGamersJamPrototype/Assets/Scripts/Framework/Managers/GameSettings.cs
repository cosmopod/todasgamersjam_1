using UnityEngine;
using System.Collections;
using System;

public class GameSettings
{
    private const string SETTINGS_ASSET = "settings";

    #region "Properties"

    //
    // Savegame
    //

    private bool savegameEnabled = true;
    public bool SavegameEnabled { get { return this.savegameEnabled; } }

    private bool usePlayerPref = false;
    public bool UsePlayerPref { get { return this.usePlayerPref; } }

    //
    // Localization
    //

    private bool detectLanguage = true;
    public bool DetectLanguage { get { return this.detectLanguage; } }

    private string defaultLanguage = "en";
    public string DefaultLanguage { get { return this.defaultLanguage; } }

    private string[] disabledLanguages = new string[] { };
    public string[] DisabledLanguages { get { return this.disabledLanguages; } }

    #endregion

    #region "Initialization"

    public void Initialize()
    {
        readSettings();
    }

    private void readSettings()
    {
        TextAsset file = (TextAsset)Resources.Load(SETTINGS_ASSET);
        if (file!=null)
        {
            //split file into lines
            string[] lines = file.text.Split(new char[] { '\n' });

            //process line by line
            for (int i=0; i<lines.Length; i++)
            {
                //remove unwated character
                string line = lines[i].Trim(new char[] { '\n', '\r' });

                //check if the line is valid
                if (line.StartsWith(";") || line.StartsWith(" ") || (line.Length == 0))
                    continue;

                string variable; string value;
                if (splitLine(line, out variable, out value))
                {
                    switch (variable.ToUpper())
                    {
                        case "SAVEGAMEENABLED":
                            if (!bool.TryParse(value, out this.savegameEnabled))
                                Debug.LogWarning("Invalid value for savegameEnabled in line " + i);
                            break;

                        case "USEPLAYERPREF":
                            if (!bool.TryParse(value, out this.usePlayerPref))
                                Debug.LogWarning("Invalid value for usePlayerPref in line " + i);
                            break;

                        case "DETECTLANGUAGE":
                            if (!bool.TryParse(value, out this.detectLanguage))
                                Debug.LogWarning("Invalid value for detectLanguage in line " + i);
                            break;

                        case "DEFAULTLANGUAGE":
                            if ((value != null) && (value != ""))
                                this.defaultLanguage = value;
                            else
                                Debug.LogWarning("Invalid value for defaultLanguage in line " + i);
                            break;

                        case "DISABLEDLANGUAGES":
                            this.disabledLanguages = splitStringArray(value);
                            break;

                        default:
                            Debug.LogWarning("Invalid variable found in Settings: " + variable);
                            break;
                    }
                }
            }
        }
    }

    /// <summary>
    /// Splits a setting line into two components: variable and value
    /// </summary>
    private bool splitLine(string line, out string variable, out string value)
    {
        bool retVal = false;
        variable = "";
        value = "";

        int splitPos = line.IndexOf("=");
        if (splitPos >= 0)
        {
            //split the string into variable and value
            variable = line.Substring(0, splitPos - 1).Trim();
            value = line.Substring(splitPos + 1).Trim();

            //flag ok
            retVal = true;
        }

        return retVal;
    }

    /// <summary>
    /// Split a string into values separated by comma
    /// </summary>
    private string[] splitStringArray(string value)
    {
        if (value.IndexOf(",") >= 0)
        {
            string[] values = value.Split(new char[] { ',' });
            for (int i = 0; i < values.Length; i++)
                values[i] = values[i].Trim();
            return values;
        }
        else
        {
            return new string[] { value.Trim() };
        }
    }
    
    #endregion

}