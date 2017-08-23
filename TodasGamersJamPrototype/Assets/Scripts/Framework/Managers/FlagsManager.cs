using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class FlagsManager
{
    #region "Types"

    public abstract class Flag
    {
        public string Key = "";
        public abstract object GetValue();
    }

    public class BoolFlag : Flag
    {
        private bool value;
        public override object GetValue()
        {
            return value;
        }

        public bool Get() { return this.value; }
        public BoolFlag(string key, bool value) { this.Key = key; this.value = value; }
    }

    public class NumberFlag : Flag
    {
        private double value;
        public override object GetValue()
        {
            return value;
        }

        public double Get() { return this.value; }
        public NumberFlag(string key, double value) { this.Key = key; this.value = value; }
    }

    public class StringFlag : Flag
    {
        private string value;
        public override object GetValue()
        {
            return value;
        }
        public string Get() { return this.value; }
        public StringFlag(string key, string value) { this.Key = key; this.value = value; }
    }

    #endregion

    #region "Initizalition"

    public void Initialize()
    {
        this.flagsDictionary = new Dictionary<string, Flag>();
    }

    #endregion

    #region "Flags dictionary"

    private Dictionary<string, Flag> flagsDictionary;
    
    public void SetFlag(string key, bool val)
    {
        if (flagsDictionary.ContainsKey(key))
        {
            flagsDictionary.Remove(key);
        }
        flagsDictionary.Add(key, new BoolFlag(key, val));
    }

    public void SetFlag(string key, double val)
    {
        if (flagsDictionary.ContainsKey(key))
        {
            flagsDictionary.Remove(key);
        }
        flagsDictionary.Add(key, new NumberFlag(key, val));
    }

    public void SetFlag(string key, string val)
    {
        if (flagsDictionary.ContainsKey(key))
        {
            flagsDictionary.Remove(key);
        }
        flagsDictionary.Add(key, new StringFlag(key, val));
    }

    public bool GetFlagBool(string key)
    {
        Flag f = null;
        flagsDictionary.TryGetValue(key, out f);
        BoolFlag bF = f as BoolFlag;
        return (bF != null) ? bF.Get() : false;
    }

    public double GetFlagNumber(string key)
    {
        Flag f = null;
        flagsDictionary.TryGetValue(key, out f);
        NumberFlag nF = f as NumberFlag;
        return (nF != null) ? nF.Get() : 0.0f;
    }

    public string GetFlagString(string key)
    {
        Flag f = null;
        flagsDictionary.TryGetValue(key, out f);
        StringFlag sF = f as StringFlag;
        return (sF != null) ? sF.Get() : "";
    }

    public bool IsFlagSet(string key)
    {
        return flagsDictionary.ContainsKey(key);
    }

    public void Clear()
    {
        this.flagsDictionary.Clear();
    }

    #endregion

    #region "Savegame integration"

    /// <summary>
    /// Returns a list of all the flags currently in memory
    /// </summary>
    public Flag[] GetForSavegame()
    {
        Flag[] flagArray = new Flag[this.flagsDictionary.Count];
        this.flagsDictionary.Values.CopyTo(flagArray, 0);
        return flagArray;
    }

    #endregion
}
