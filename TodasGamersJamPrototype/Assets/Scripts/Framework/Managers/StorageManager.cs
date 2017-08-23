using UnityEngine;
using System.Collections;
using System.IO;
using JSON;


public class StorageManager
{
    /// <summary>
    /// All classes the may go to the savegame must implement this interface
    /// in order to be processed by the StorageManager
    /// </summary>
    public interface IJSONSerializable
    {
        void LoadDefaults();
        void Deserialize(JSONObject j);
        void Serialize(JSONObject j);
    }

    /// <summary>
    /// Reconstruct an object in memory from JSON data in storage
    /// </summary>
    public static bool LoadFromStorage(string fileName, IJSONSerializable destination)
    {
        bool result = false;

        if (GameManager.Instance.Settings.SavegameEnabled)
        {
            string data = null;
            if (GameManager.Instance.Settings.UsePlayerPref)
            {
                data = PlayerPrefs.GetString(processFileName(fileName));
            }
            else
            {
                data = safeLoadingDataFromFile(processFileName(fileName));
            }

            //transform the string into JSON and let the object
            //reconstruct itself from it
            JSONObject j = JSONObject.Parse(data);
            if (j != null) destination.Deserialize(j);

            //everything went ok
            result = true;
        }

        //if it didn't go well, load defaults
        if (!result) destination.LoadDefaults();

        return result;
    }

    /// <summary>
    /// Reads a coplete text file from the filesystem.
    /// HACK: In a separated method to avoid runtime error in mobile platforms
    /// </summary>
    private static string safeLoadingDataFromFile(string fileName)
    {
        string data = "";
        fileName = string.Format("{0}/{1}.json", Application.persistentDataPath, fileName);
        if (File.Exists(fileName))
        {
            StreamReader sr = File.OpenText(fileName);
            data = sr.ReadToEnd();
            sr.Close();
        }
        return data;
    }

    /// <summary>
    /// Saves an object from memory to a JSON file in storage
    /// </summary>
    public static bool SaveToStorage(string fileName, IJSONSerializable origin)
    {
        bool result = false;

        if (GameManager.Instance.Settings.SavegameEnabled)
        {
            JSONObject j = new JSONObject();
            origin.Serialize(j);

            if (GameManager.Instance.Settings.UsePlayerPref)
            {
                PlayerPrefs.SetString(processFileName(fileName), j.ToString());
                result = true;
            }
            else
            {
                safeSaveDataToFile(processFileName(fileName), j.ToString());
                result = true;
            }
        }

        return result;
    }

    /// <summary>
    /// Saves data to a text file in the filesystem.
    /// HACK: In a separated method to avoid runtime error in mobile platforms
    /// </summary>
    private static void safeSaveDataToFile(string fileName, string data)
    {
        StreamWriter sw = File.CreateText(string.Format("{0}/{1}.json", Application.persistentDataPath, fileName));
        sw.Write(data);
        sw.Close();
    }

    /// <summary>
    /// Transform the fileName depending on the platform
    /// </summary>
    private static string processFileName(string fileName)
    {
        return Application.isEditor ? "EDITOR_" + fileName : fileName;
    }

}