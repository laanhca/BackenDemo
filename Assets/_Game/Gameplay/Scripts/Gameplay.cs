using System;
using System.Collections.Generic;
using _Game.Gameplay.Scripts;
using LoginScene;
using Michsky.UI.ModernUIPack;
using Newtonsoft.Json.Linq;
using TMPro;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using Leaderboard = UnityEngine.SocialPlatforms.Impl.Leaderboard;

public class Gameplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI txtNameShow;
    [SerializeField] private TextMeshProUGUI txtPoint;
    [SerializeField] private GameObject player;

    [SerializeField] private Button btnSavePoint;
    [SerializeField] private Button btnLeaderboard;
    [SerializeField] private Button btnCloseLeaderboard;

    [SerializeField] private ListView listLeaderboard;
    
    private int lastPoint = 0;
    private string _token;
    public static Gameplay Instance;
    private int prevPoint = 0;

    private void Awake()
    {
        Instance = this;
        _token = PlayerPrefs.GetString("token");
        Init();
        Movement.OnMovement += OnPlayerMove;
        btnSavePoint.onClick.AddListener(OnBtnSaveClick);
        btnLeaderboard.onClick.AddListener(OnBtnLeaderboardClick);
        btnCloseLeaderboard.onClick.AddListener(OnBtnCloseLeaderboardClick);
    }

    private void OnBtnCloseLeaderboardClick()
    {
        listLeaderboard.ResetList();

    }

    private void OnBtnLeaderboardClick()
    {
        var request = UnityWebService.Instance.CreateApiGetRequest("api/login_screen/get_leaderboard");
        StartCoroutine(UnityWebService.Instance.IESendRequest(request, OnGetLeaderboardComplete));
    }

    private void OnGetLeaderboardComplete(UnityWebRequest request)
    {
        if (request.result == UnityWebRequest.Result.Success)
        {
            var result = request.downloadHandler.text;
            Debug.Log(result);
            Ranker[] rankers = JsonHelper.FromJson<Ranker>(result);
            Debug.Log(rankers[0].ToString());
            rankers = SortRanker(rankers);

            for (int i = rankers.Length-1; i >= 0; i--)
            {
                ListView.ListItem item = new ListView.ListItem
                {
                    row0 = new ListView.ListRow(),
                    row1 = new ListView.ListRow(),
                    row2 = new ListView.ListRow()
                };
                item.row0.rowType = ListView.RowType.Text;
                item.row1.rowType = ListView.RowType.Text;
                item.row2.rowType = ListView.RowType.Text;

                item.row0.rowText = (rankers.Length-1 - i +1).ToString();
                item.row1.rowText = rankers[i].NameShow;
                item.row2.rowText = rankers[i].Point;
                
                listLeaderboard.listItems.Add(item);
            }
            listLeaderboard.InitializeItems();

            
        }
    }

    private Ranker[] SortRanker(Ranker[] rankers)
    {
        int n = rankers.Length;
        Ranker temp = new Ranker();
        for (int i = 0; i < n; i++) {

            for (int j = 1; j < (n - i); j++) {

                
                if (Int32.Parse(rankers[j - 1].Point) > Int32.Parse(rankers[j].Point)) {
                    temp = rankers[j - 1];
                    rankers[j - 1] = rankers[j];
                    rankers[j] = temp;

                }

            }
        }

        return rankers;
    }

    private void OnBtnSaveClick()
    {
        UpdatePoint(_token);
    }

    private void Init()
    {
        GetNameShow(_token);
        GetPoint(_token);
        
    }

    private void GetNameShow(String token)
    {
        if (token == null) return;
        if (UnityWebService.Instance == null) return;
        var request = UnityWebService.Instance.CreateApiPostRequest("api/login_screen/get_nameshow", token);
        StartCoroutine(UnityWebService.Instance.IESendRequest(request, OnGetNameComplete));
    }

    private void GetPoint(String token)
    {
        if (token == null) return;
        if (UnityWebService.Instance == null) return;
        var request = UnityWebService.Instance.CreateApiPostRequest("api/login_screen/get_point", token);
        StartCoroutine(UnityWebService.Instance.IESendRequest(request, OnGetPointComplete));
    }

    private void OnGetNameComplete(UnityWebRequest request)
    {
        if (request.result == UnityWebRequest.Result.Success)
        {
            var result = request.downloadHandler.text;
            if (result != "false")
            {
                JObject jObject = JObject.Parse(result);

                string nameShow = (jObject["NameShow"] ?? throw new InvalidOperationException()).Value<string>();
                txtNameShow.text = nameShow;
            }
        }
        
    }

    private void OnGetPointComplete(UnityWebRequest request)
    {
        if (request.result == UnityWebRequest.Result.Success)
        {
            var result = request.downloadHandler.text;
            if (result != "false")
            {
                JObject jObject = JObject.Parse(result);

                string point = (jObject["Point"] ?? throw new InvalidOperationException()).Value<string>();
                txtPoint.text = point;
                prevPoint = Int32.Parse(point);
            }
        }
    }

    private void OnPlayerMove()
    {
        var dis = Vector3.Distance(Vector3.zero, player.transform.position);
        txtPoint.text = (prevPoint + Math.Round(dis, 2)).ToString();
        lastPoint = prevPoint + (int)dis;
    }

    private void OnDestroy()
    {
        Movement.OnMovement -= OnPlayerMove;
    }

    private void UpdatePoint(string token)
    {
        if (token == null) return;
        if (UnityWebService.Instance == null) return;
        UpdatePointForm form = new UpdatePointForm();
        form.token = _token;
        form.point = lastPoint;
        var request = UnityWebService.Instance.CreateApiPostRequest("api/login_screen/update_point", form);
        StartCoroutine(UnityWebService.Instance.IESendRequest(request, OnUpdatePointComplete));
    }

    private void OnUpdatePointComplete(UnityWebRequest request)
    {
        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Save point complete");
        }else Debug.Log(request.error);
    }
}
