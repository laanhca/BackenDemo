using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class UnityWebService : MonoBehaviour
{
    private const string BaseUrl = "http://3.1.231.84:8080/";
    public static UnityWebService Instance;

    private void Awake()
    {
        DontDestroyOnLoad(this);
        Instance = this;
        
    }

    public UnityWebRequest CreateApiGetRequest(string actionUrl, object body = null)
    {
        return CreateApiRequest(BaseUrl + actionUrl, UnityWebRequest.kHttpVerbGET, body);
    }
 
    public UnityWebRequest CreateApiPostRequest(string actionUrl, object body = null)
    {
        return CreateApiRequest(BaseUrl + actionUrl, UnityWebRequest.kHttpVerbPOST, body);
    }
 
    UnityWebRequest CreateApiRequest(string url, string method, object body)
    {
        string bodyString = null;
        if (body is string)
        {
            bodyString = (string)body;
        }
        else if (body != null)
        {
            bodyString = JsonUtility.ToJson(body);
        }
 
        var request = new UnityWebRequest();
        request.url = url;
        request.method = method;
        request.downloadHandler = new DownloadHandlerBuffer();
        request.uploadHandler = new UploadHandlerRaw(string.IsNullOrEmpty(bodyString) ? null : Encoding.UTF8.GetBytes(bodyString));
        request.SetRequestHeader("Accept", "application/json");
        request.SetRequestHeader("Content-Type", "application/json");
        request.timeout = 60;
        return request;
    }
    public IEnumerator IESendRequest(UnityWebRequest webRequest, Action<UnityWebRequest> callback)
    {
        yield return webRequest.SendWebRequest();
        callback.Invoke(webRequest);
    }
    
}
