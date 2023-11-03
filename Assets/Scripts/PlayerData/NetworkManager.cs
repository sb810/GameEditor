using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace PlayerData
{
    public class NetworkManager : MonoBehaviour
    {
        private void Awake()
        {
            if (!string.IsNullOrEmpty(PlayerDataManager.Data.id))
                GetNetworkSavedData();
        }

        public IEnumerator SendWebRequest(string method)
        {
            WebData dataToSave = PlayerDataManager.Data;

            string jsonData = JsonUtility.ToJson(dataToSave);
            string uri = "https://api.studioxp.ca/items/game_editor_data_v2" + (method is "PATCH" or "GET"
                ? "/" + dataToSave.id
                : "");

            Debug.Log("Preparing " + method + " webrequest. URI is " + uri + ", data is " + jsonData);

            using UnityWebRequest request = new UnityWebRequest(uri, method);
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Authorization", "Bearer 3prCBvKlJncjeXjAOROUBQZ3qIUCMHDQ");
            byte[] rawData = Encoding.UTF8.GetBytes(jsonData);
            if (method is not "GET") request.uploadHandler = new UploadHandlerRaw(rawData);
            request.downloadHandler = new DownloadHandlerBuffer();
            yield return request.SendWebRequest();

            if (request.result is not UnityWebRequest.Result.Success)
            {
                if (method is "PATCH" or "GET")
                {
                    Debug.Log(method + " error ! Trying again with POST...\nResponse : " + request.responseCode + "; " +
                              request.error + "; " + request.downloadHandler.text);
                    PlayerDataManager.Data.id = "";
                    UploadNewSaveData();
                    yield break;
                }

                // Debug.LogError(method + " error in NetworkManager.CreateWebRequest function : " + request.downloadHandler.text);
            }
            else
            {
                if (request.downloadHandler == null || string.IsNullOrEmpty(request.downloadHandler.text))
                {
                    Debug.LogError(method + " error in NetworkManager.CreateWebRequest function : " +
                                   request.downloadHandler.text);
                    yield break;
                }

                if (method is "POST" or "GET")
                {
                    // The server returns an ID, which we then use
                    Debug.Log("BEFORE JSON TRIMMING : " + request.downloadHandler.text);
                    jsonData = request.downloadHandler.text;
                    jsonData = jsonData.Remove(0, 8);
                    jsonData = jsonData.Remove(jsonData.Length - 1, 1);

                    if (jsonData.StartsWith("["))
                        jsonData = jsonData.Remove(0, 1);
                    if (jsonData.EndsWith("]"))
                        jsonData = jsonData.Remove(jsonData.Length - 1, 1);

                    Debug.Log("AFTER JSON TRIMMING : " + jsonData);
                    PlayerDataManager.Data = JsonUtility.FromJson<WebData>(jsonData);
                    PlayerPrefs.SetString("clientID", PlayerDataManager.Data.id);
                    PlayerPrefs.Save();
                }

                Debug.Log(method + " complete in NetworkManager.UpdateData !\nDATA : " + PlayerDataManager.Data);
            }
        }

        public void UploadNewSaveData()
        {
            StartCoroutine(SendWebRequest("POST"));
        }

        public void UpdateNetworkSavedData()
        {
            StartCoroutine(SendWebRequest("PATCH"));
        }

        public void GetNetworkSavedData()
        {
            StartCoroutine(SendWebRequest("GET"));
        }
    }
}