using System;
using System.Collections;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace LoginScene
{
    public class Login : MonoBehaviour
    {
        [SerializeField] private Button btnHomeLogin;
        [SerializeField] private Button btnHomeReg;
        [SerializeField] private Object panelHome;
        
        [Space(20)]
        [SerializeField] private Object panelLoginWithMail;
        
        [Space(20)]
        [SerializeField] private Object panelRegWithUsername;
        [SerializeField] private Button btnReg;
        [SerializeField] private TextMeshProUGUI txtRegUername;
        [SerializeField] private TextMeshProUGUI txtRegPassword;
        [SerializeField] private TextMeshProUGUI txtRegRepeatPassword;
        
        [Space(20)]
        [SerializeField] private Object panelLoginWithUsername;
        [SerializeField] private Button btnLogin;
        [SerializeField] private TextMeshProUGUI txtLoginUsername;
        [SerializeField] private TextMeshProUGUI txtLoginPassword;

        private void Start()
        {
            btnHomeLogin.onClick.AddListener(OnBtnHomeLoginClick);
            btnLogin.onClick.AddListener(OnBtnLoginClick);
            
            btnHomeReg.onClick.AddListener(OnBtnHomeRegClick);
            btnReg.onClick.AddListener(OnBtnRegClick);

        }

        #region For login

        private void OnBtnLoginClick()
        {
            UsernameLoginForm form = new UsernameLoginForm();
            form.credentials = txtLoginUsername.text.ToLower().Replace("\u200B","");
            form.password = txtLoginPassword.text.ToLower().Replace("\u200B","");
            var request = UnityWebService.Instance.CreateApiPostRequest("api/login_screen/login_with_username", form);
            StartCoroutine(UnityWebService.Instance.IESendRequest(request,OnPostLoginComplete));
        }
     

        private void OnPostLoginComplete(UnityWebRequest request)
        {
            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Request success");
                var result = request.downloadHandler.text;
                if (result == "false")
                {
                    Debug.Log("Sai tai khoan hoac mat khau");
                }
                else
                {
                    JObject jObject = JObject.Parse(result);

                    string token = (jObject["token"] ?? throw new InvalidOperationException()).Value<string>();
                    Debug.Log(token);
                }
            }
            else
            {
                Debug.Log(request.error);
            }
        }

        private void OnBtnHomeLoginClick()
        {
            panelHome.GameObject().SetActive(false);
            panelLoginWithUsername.GameObject().SetActive(true);
        }

        #endregion

        #region For register
        private void OnBtnHomeRegClick()
        {
            panelHome.GameObject().SetActive(false);
            panelRegWithUsername.GameObject().SetActive(true);
        }

        private void OnBtnRegClick()
        {
            UsernameLoginForm form = new UsernameLoginForm();
            form.credentials = txtRegUername.text.ToLower().Replace("\u200B","");
            form.password = txtRegPassword.text.ToLower().Replace("\u200B","");
            var request = UnityWebService.Instance.CreateApiPostRequest("api/login_screen/reg_with_username", form);
            StartCoroutine(UnityWebService.Instance.IESendRequest(request, OnPostRegComplete));

        }

        private void OnPostRegComplete(UnityWebRequest request)
        {
            if (request.result == UnityWebRequest.Result.Success)
            {
                var result = request.downloadHandler.text;
                if (result == "false")
                {
                    Debug.Log("Tai khoan da ton tai");
                }
                else
                {
                    JObject jObject = JObject.Parse(result);

                    string userId = (jObject["UserId"] ?? throw new InvalidOperationException()).Value<string>();
                    Debug.Log(userId);
                }
                
            }
            else
            {
                Debug.Log(request.error);
            }
        }

        #endregion
        
    }
}
