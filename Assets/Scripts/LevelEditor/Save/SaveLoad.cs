using System;
using PlayerData;
using UnityEngine;

namespace LevelEditor.Save
{
    public static class SaveLoad<T>
    {
        /*public static void SaveToLocal(T data, string file)
        {
            string jsonData = JsonUtility.ToJson(data, true);
            PlayerPrefs.SetString(file, jsonData);
            PlayerPrefs.Save();
        }

        public static T LoadFromLocal(string file)
        {
            if (PlayerPrefs.HasKey(file))
            {
                string jsonData = PlayerPrefs.GetString(file);
                T returnedData = JsonUtility.FromJson<T>(jsonData);
                return (T)Convert.ChangeType(returnedData, typeof(T));
            }
            return default;
        }*/
    }
}