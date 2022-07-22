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
        
        [Space(20)]
        [SerializeField] private Object panelLoginWithUsername;
        [SerializeField] private Button btnLogin;
        [SerializeField] private TextMeshProUGUI txtUsername;
        [SerializeField] private TextMeshProUGUI txtPassword;

        private void Start()
        {
            btnHomeLogin.onClick.AddListener(OnBtnHomeLoginClick);
            btnLogin.onClick.AddListener(OnBtnLoginClick);
            
            btnHomeReg.onClick.AddListener(OnBtnHomeRegClick);

        }

        #region For login

        private void OnBtnLoginClick()
        {
            UsernameLoginForm form = new UsernameLoginForm();
            form.credentials = txtUsername.text.Replace("\u200B","");
            form.password = txtPassword.text.Replace("\u200B","");
            var request = UnityWebService.Instance.CreateApiPostRequest("api/login_screen/login_with_username", form);
            StartCoroutine(UnityWebService.Instance.IESendRequest(request,OnPostLoginDone));
        }
     

        private void OnPostLoginDone(UnityWebRequest request)
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
        }
        
        

        #endregion
        
    }
}
