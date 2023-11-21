using System.Collections;
using System.Text.RegularExpressions;
using LevelEditor.Save;
using PlayerData;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;

namespace LevelEditor.Network
{
    public class TeacherNetworkManager : MonoBehaviour
    {
        [HideInInspector] public WebData[] groupData;
        public GameObject studentListLocation;
        public GameObject studentButtonPrefab;
        private SaveManager loader;

        private void Start()
        {
            loader = GameManager.Instance.SaveManager;
            StartCoroutine(AutoRefresh());
        }

        private IEnumerator AutoRefresh()
        {
            yield return StartCoroutine(GetGroupData());
            yield return new WaitForSeconds(30);
            StartCoroutine(AutoRefresh());
            yield return null;
        }

        private IEnumerator GetGroupData()
        {
            string uri = "https://api.studioxp.ca/items/game_editor_data_v2?filter[group][_eq]=" + PlayerDataManager.Data.group;
            Debug.Log("Fetching student data from URI : " + uri + " with method GET.");
            
            using UnityWebRequest request = new UnityWebRequest(uri, "GET");
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Authorization", "Bearer 3prCBvKlJncjeXjAOROUBQZ3qIUCMHDQ");
            request.downloadHandler = new DownloadHandlerBuffer();
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.Log("GET error in ProfNetworkManager.GetGroupData function : " + request.downloadHandler.text);
            }
            else
            {
                if (request.downloadHandler == null)
                {
                    Debug.LogError("No downloadHandler for group " + PlayerDataManager.Data.group + " !\nRequest Data :" + request);
                    yield break;
                }

                int tempChildCount = studentListLocation.transform.childCount;
                for (int i = 0; i < tempChildCount; i++)
                {
                    Destroy(studentListLocation.transform.GetChild(i).gameObject);
                    //yield return null;
                }
                
                if (string.IsNullOrEmpty(request.downloadHandler.text) || request.downloadHandler.text.Length < 20)
                {
                    Debug.Log("Empty data found for group " + PlayerDataManager.Data.group + " !\nHandler Data :" + request.downloadHandler);
                    yield break;
                }

                string jsonData = FixJson(request.downloadHandler.text);
                groupData = JsonHelper.FromJson<WebData>(jsonData);
                
                Debug.Log("ProfNetworkManager.GetGroupData complete !\nData : " + groupData);

                int tempId = 0;
                if(groupData.Length > 0) Debug.Log("Group data found ! Populating...");
                foreach (WebData student in groupData)
                {
                    Debug.Log("ID = " + tempId);
                    GameObject button = Instantiate(studentButtonPrefab, studentListLocation.transform);
                    StudentButton studentButton = button.GetComponent<StudentButton>();
                    studentButton.managerRef = GetComponent<TeacherNetworkManager>();
                    studentButton.id = tempId;
                    studentButton.username.text = student.username;
                    studentButton.timestamp.text = student.levelData.timestamp;
                    
                    if (student.id == PlayerDataManager.ClientID)
                    {
                        studentButton.username.text += " (you)";
                        button.transform.SetSiblingIndex(0);
                    }

                    RefreshButtonsVisualStatus();

                    tempId++;
                }
            }
            
            yield return null;
        }

        private void RefreshButtonsVisualStatus()
        {
            foreach (StudentButton studentButton in studentListLocation.GetComponentsInChildren<StudentButton>())
            {
                studentButton.GetComponent<Button>().interactable = groupData[studentButton.id].id != PlayerDataManager.Data.id;
            }
        }

        public void LoadLevel(int id)
        {
            //SavedLevel dataToSave = groupData[id].levelData;
            //SaveLoad<SavedLevel>.SaveToLocal(dataToSave, "Level");

            if (PlayerDataManager.ClientID == PlayerDataManager.Data.id)
                loader.SaveDataToNetwork();

            PlayerDataManager.Data.levelData = groupData[id].levelData;
            PlayerDataManager.Data.id = groupData[id].id;
            RefreshButtonsVisualStatus();
            loader.ReloadLevelData();
        }

        public void RefreshButton()
        {
            /*string url = Application.absoluteURL;
            string groupName = "default";
            
            Regex regex = new Regex ("(?>\\?|&)group=([^&]+)");
            Match match = regex.Match (url);
            if (match.Success)
            {
                groupName = match.Groups [1].Value;
                Debug.Log ("Group name is " + groupName);
            }
            else
            {
                Debug.Log ("No group parameter found in the URL");
            }*/
            
            if(PlayerDataManager.Data.isTeacher)
                StartCoroutine(GetGroupData());
        }

        private IEnumerator DeleteGroupData()
        {
            foreach (var t in groupData)
            {
                string uri = "https://api.studioxp.ca/items/game_editor_data_v2/" + t.id;

                using (UnityWebRequest request = UnityWebRequest.Delete(uri))
                {
                    request.SetRequestHeader("Content-Type", "application/json");
                    request.SetRequestHeader("Authorization", "Bearer tr4mfJ9rvQS9qucARPIQmuAQ61bf1Bgc");

                    yield return request.SendWebRequest();
                }

                yield return null;
            }

            int tempChildCount = studentListLocation.transform.childCount;
            for (int i = 0; i < tempChildCount; i++)
            {
                Destroy(studentListLocation.transform.GetChild(0).gameObject);
                yield return null;
            }

            /*int tempId = 0;
            foreach (WebData student in groupData)
            {
                GameObject studentButton = Instantiate(studentButtonPrefab, studentListLocation.transform);
                studentButton.GetComponent<StudentButton>().managerRef = GetComponent<TeacherNetworkManager>();
                studentButton.GetComponent<StudentButton>().id = tempId;
                tempId++;
            }*/
        }

        public void DeleteButton()
        {
            StartCoroutine(DeleteGroupData());
        }

        private string FixJson(string value)
        {
            value = value.Remove(0, 9);
            value = value.Remove((value.Length - 3), 2);
            value = "{\"Items\":[" + value + "]}";
            return value;
        }
    }

    public static class JsonHelper
    {
        public static T[] FromJson<T>(string json)
        {
            Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(json);
            return wrapper.Items;
        }

        public static string ToJson<T>(T[] array)
        {
            Wrapper<T> wrapper = new Wrapper<T>();
            wrapper.Items = array;
            return JsonUtility.ToJson(wrapper);
        }

        public static string ToJson<T>(T[] array, bool prettyPrint)
        {
            Wrapper<T> wrapper = new Wrapper<T>();
            wrapper.Items = array;
            return JsonUtility.ToJson(wrapper, prettyPrint);
        }

        [System.Serializable]
        private class Wrapper<T>
        {
            public T[] Items;
        }
    }
}