using System;
using System.Text.RegularExpressions;
using UnityEngine;



namespace PlayerData
{
    
    [Serializable]
    public class WebData
    {
        public bool designPasswordEntered;
        public bool codingPasswordEntered;
        public int unlockedCodingLevel;
        public int selectedCodingLevel;
        public bool isTeacher;
        public string id;
        public string group;
        public string username;
        public SavedLevel levelData = new();
    }
    
    public static class PlayerDataManager
    {
        public static readonly string TeacherPassword = "XabiIturioz";
        public static readonly string DesignPassword = "mario";
        public static readonly string CodingPassword = "luigi";

        public static WebData Data = new();

        static PlayerDataManager()
        {
            Data.group = GetGroupFromCurrentURL();
            if (PlayerPrefs.HasKey("clientID"))
            {
                Data.id = PlayerPrefs.GetString("clientID");
                Debug.Log("Client ID found in PlayerPrefs ! " + Data.id);
            } else Debug.Log("Client ID not found... Waiting for a new connection.");
        }

        private static string GetGroupFromCurrentURL()
        {
            string url = Application.absoluteURL;
            
            Regex regex = new Regex ("(?>\\?|&)group=([^&]+)");
            Match match = regex.Match (url);
            if (match.Success)
                return match.Groups[1].Value;
            if (url.Length == 0)
                url = "<Undefined>";
            
            Debug.LogWarning ("No group parameter found in URL \""+ url +"\" ! Using \"default\".");
            return "default";
        }
    }
}