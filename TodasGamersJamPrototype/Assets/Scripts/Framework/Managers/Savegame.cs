using UnityEngine;
using System.Collections;
using System;
using JSON;

public class Savegame : StorageManager.IJSONSerializable
{

    private const string SAVEGAME_FILENAME = "SAVEGAME";

    #region "Load & Save"

    public void Load()
    {
        StorageManager.LoadFromStorage(SAVEGAME_FILENAME, this);
    }

    public void Save()
    {
        StorageManager.SaveToStorage(SAVEGAME_FILENAME, this);
    }



    #endregion

    #region "IJSONSerializable"

    public void LoadDefaults()
    {
        GameManager.Instance.Flags.Clear();
    }

    public void Serialize(JSONObject j)
    {
        this.serializeFlags(j);
    }

    private void serializeFlags(JSONObject j)
    {
        //create a container for the flags in the JS
        JSONArray flagsJS = new JSONArray();
        j.Add("FLAGS", flagsJS);

        //get all existing flags and iterate though them
        FlagsManager.Flag[] flags = GameManager.Instance.Flags.GetForSavegame();
        for (int i = 0; i < flags.Length; i++)
        {
            //add a new array, and Key as a value of type string
            JSONArray flagJS = new JSONArray();
            flagJS.Add(new JSONValue(flags[i].Key));

            //
            //now add the value of the flag according to its type
            //

            //bool?
            FlagsManager.BoolFlag bF = flags[i] as FlagsManager.BoolFlag;
            if (bF != null)
            {
                flagJS.Add(new JSONValue(bF.Get()));
            }
            else
            {
                //string?
                FlagsManager.StringFlag sF = flags[i] as FlagsManager.StringFlag;
                if (sF != null)
                {
                    flagJS.Add(new JSONValue(sF.Get()));
                }
                else
                {
                    //number?
                    FlagsManager.NumberFlag nF = flags[i] as FlagsManager.NumberFlag;
                    if (nF != null)
                    {
                        flagJS.Add(new JSONValue(nF.Get()));
                    }
                }
            }

            //add the flag JS to the general JS
            flagsJS.Add(flagJS);
        }
    }

    public void Deserialize(JSONObject j)
    {
        this.deserializeFlags(j);
    }

    private void deserializeFlags(JSONObject j)
    {
        //get the JSON array that contains the flags data
        JSONArray flagsJS = j.GetArray("FLAGS");
        if ((flagsJS != null) && (flagsJS.Length > 0))
        {
            //iterate through the JS data
            for (int i = 0; i < flagsJS.Length; i++)
            {
                //get the flag key from the first item in the array
                JSONArray flagJS = flagsJS[i].Array;
                string key = flagJS[0].Str;

                //is of type boolean?
                if (flagJS[1].Type == JSONValueType.Boolean)
                {
                    GameManager.Instance.Flags.SetFlag(key, flagJS[1].Boolean);
                }

                //is of type number?
                else if (flagJS[1].Type == JSONValueType.Number)
                {
                    GameManager.Instance.Flags.SetFlag(key, flagJS[1].Number);
                }

                //is of type number?
                else if (flagJS[1].Type == JSONValueType.String)
                {
                    GameManager.Instance.Flags.SetFlag(key, flagJS[1].Str);
                }
            }
        }
    }

    #endregion
    
}
