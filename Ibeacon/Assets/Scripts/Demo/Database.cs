using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.Networking;
using UnityEngine.UI;
using System;
public class Database : MonoBehaviour
{

    [System.Serializable]
    private class UserIdClass
    {
        public string userID;
    }

    //[System.Serializable]
    //private class UpdateUserData
    //{
    //    public int ibeaconMinor;
    //    public float userStayTime;
    //}

    public string getUserID_Path;//php的網路路徑
    public string updateData_Path;
    public Text userID_Text;
    public bool updateFinish = false;
    public Text status_Text;

    private int intoDataCount = 0;
    public int ibeaconSort = 1; 
    private UserIdClass userId;
    //private UpdateUserData userData;
    private string fileName = "Save.json";//要創的檔案名稱
    //private string jsonData;

    void Start()
    {
        status_Text.text="CheckID";
        updateFinish = false;
        //userData = new UpdateUserData();
        userId = new UserIdClass();
        //判斷檔案是否存在
        if (File.Exists(Application.persistentDataPath + fileName))
        {
            print("檔案存在");
            //取得檔案資料
            string dataAsJson = File.ReadAllText(Application.persistentDataPath + fileName);
            userId = JsonUtility.FromJson<UserIdClass>(dataAsJson);
            userID_Text.text = "player_ID:" + userId.userID;
        }
        else //如果檔案不在就向資料庫請求一個
        {
            print("檔案不存在");
            //創建ID
            StartCoroutine(CreatUserID(getUserID_Path));
        }
        

    }

    //儲存到自創的json檔
    public void SaveDataToFile()
    {
        FileStream fs = new FileStream(Application.persistentDataPath + fileName, FileMode.Create);
        StreamWriter sw = new StreamWriter(fs);
        sw.WriteLine(JsonUtility.ToJson(userId));
        sw.Close();
        fs.Close();
        userID_Text.text = "player_ID:" + userId.userID;
        Debug.Log("保存成功");
    }

    //接收Ibeacon腳本傳來的資料
    public void UpdateData(int ibeaconMinor, int stayTime, string nowDate)
    {
        //換算使用者待的時間
        int sec = stayTime % 60;
        int min = stayTime / 60 % 60;
        int hour = stayTime / 60 / 60 % 60;
        string sec_string = ( sec < 10 ) ? "0" + sec : sec.ToString();
        string min_string = ( min < 10 ) ? "0" + min : min.ToString();
        string hour_string = ( sec < 10 ) ? "0" + hour : hour.ToString();
        string time = hour_string + ":" + min_string + ":" + sec_string;

        updateFinish = false;
        intoDataCount++;
        //UpdateToJson (ibeaconMinor, stayTime);
        if (intoDataCount < 2)
        {
            ibeaconSort++;
            StartCoroutine(UpdateData(updateData_Path, ibeaconSort, ibeaconMinor, time, nowDate));
        }

    }

    //private void UpdateToJson(int ibeaconMinor, float stayTime)
    //{
    //    userData.ibeaconMinor = ibeaconMinor;
    //    userData.userStayTime = stayTime;
    //    jsonData = JsonUtility.ToJson(userData);
    //}


    //跟GetIdData.php做連結
    IEnumerator CreatUserID(string uri)
    {
        WWWForm form = new WWWForm();

        using (UnityWebRequest webRequest = UnityWebRequest.Post(( uri ), form))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();


            if (webRequest.isNetworkError)
            {
                status_Text.text = "Staus:NetworkError";
                Debug.Log("Error: " + webRequest.error);
            }
            else
            {
                status_Text.text = "Staus:NetworkError";
                Debug.Log("nReceived: " + webRequest.downloadHandler.text);
                userId.userID = webRequest.downloadHandler.text;//紀錄php傳回來資料，downloadHandler.text會把所有php的echo都記下來，所以echo要的參數就好(ps. echo是php印出的意思)
                SaveDataToFile();
            }
        }
    }

    //跟UpdateData.php做連結
    IEnumerator UpdateData(string uri, int ibeaconSort, int ibeaconMinor, string stayTime, string nowDate)
    {

        WWWForm form = new WWWForm();
        form.AddField("UserID", userId.userID);//把資料傳給php，第一個參數名稱要跟php自定義的要一樣
        form.AddField("ibeaconMinor", ibeaconMinor);
        form.AddField("stayTime", stayTime);
        form.AddField("ibeaconSort", ibeaconSort);
        form.AddField("nowDate", nowDate);
        using (UnityWebRequest webRequest = UnityWebRequest.Post(( uri ), form))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();


            if (webRequest.isNetworkError)
            {
                Debug.Log("Error: " + webRequest.error);
            }



            if (webRequest.isDone)
            {
                status_Text.text = "Staus:資料庫上傳完成";
                updateFinish = true;
                intoDataCount = 0;
            }
        }
    }

}
