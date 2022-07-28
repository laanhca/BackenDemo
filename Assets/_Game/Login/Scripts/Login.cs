using System;
using System.Collections;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
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
        [SerializeField] private TMP_InputField txtMail;
        [SerializeField] private Button btnSend;
        [SerializeField] private TMP_InputField txtCode;
        [SerializeField] private Button btnMailLogin;

        [Space(20)]
        [SerializeField] private Object panelRegWithUsername;
        [SerializeField] private Button btnReg;
        [SerializeField] private TextMeshProUGUI txtRegUsername;
        [SerializeField] private TMP_InputField txtRegPassword;
        [SerializeField] private TMP_InputField txtRegRepeatPassword;
        
        [Space(20)]
        [SerializeField] private Object panelLoginWithUsername;
        [SerializeField] private Button btnLogin;
        [SerializeField] private TextMeshProUGUI txtLoginUsername;
        [SerializeField] private TMP_InputField txtLoginPassword;

        [Space(20)] 
        [SerializeField] private GameObject panelNoti;
        [SerializeField] private TextMeshProUGUI txtNoti;
        
        
        
        private void Start()
        {
            btnHomeLogin.onClick.AddListener(OnBtnHomeLoginClick);
            btnLogin.onClick.AddListener(OnBtnLoginClick);
            
            btnHomeReg.onClick.AddListener(OnBtnHomeRegClick);
            btnReg.onClick.AddListener(OnBtnRegClick);
            
            btnSend.onClick.AddListener(OnBtnSendClick);
            btnMailLogin.onClick.AddListener(OnBtnMailLoginClick);

        }

        #region For Mail Login

        private void OnBtnSendClick()
        {
            
            string email = txtMail.text.ToLower().Replace("\u200B","");
            
            if (email == "")
            {
                ShowNotification("Mail không được để trống");
                return;
            }
            
            if (IsValidEmail(email))
            {
                txtMail.interactable = false;
                btnSend.interactable = false;
                var request = UnityWebService.Instance.CreateApiPostRequest("api/login_screen/send_mail", email);
                StartCoroutine(UnityWebService.Instance.IESendRequest(request,OnSendMailComplete));
                StartCoroutine(IEEnableBtnSend(30));
            }
            else
            {
                ShowNotification("Email không hợp lệ");
            }
        }
        private void OnBtnMailLoginClick()
        {
            MailLoginForm form = new MailLoginForm();
            form.mail = txtMail.text.ToLower().Replace("\u200B","");
            string code = txtCode.text.ToLower().Replace("\u200B", "");
            if(code != "") form.code = Int32.Parse(txtCode.text.ToLower().Replace("\u200B",""));
            
            if (form.mail == "" || code == "")
            {
                ShowNotification("Thông tin không được để trống");
                return;
            }
            
            var request = UnityWebService.Instance.CreateApiPostRequest("api/login_screen/login_with_mail", form);
            StartCoroutine(UnityWebService.Instance.IESendRequest(request,OnPostMailLoginComplete));
        }

        private IEnumerator IEEnableBtnSend(int seconds)
        {
            yield return new WaitForSeconds(30);
            txtMail.interactable = true;
            btnSend.interactable = true;
        }

        private void OnSendMailComplete(UnityWebRequest request)
        {
            if (request.result == UnityWebRequest.Result.Success)
            {
                if (request.downloadHandler.text != null)
                {
                    ShowNotification("Vui lòng kiểm tra hộp thư email");
                    Debug.Log("Send code thành công, "+request.downloadHandler.text);
                }
                else
                {
                    ShowNotification("Yêu cầu thất bại, vui lòng thử lại sau");
                }
            }
            else
            {
                Debug.Log(request.error);
            }
        }

        private void OnPostMailLoginComplete(UnityWebRequest request)
        {
            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Request success");
                var result = request.downloadHandler.text;
                if (result == "false")
                {
                    Debug.Log("Sai code");
                    ShowNotification("Mã xác nhận không chính xác");
                }
                else
                {
                    JObject jObject = JObject.Parse(result);

                    string token = (jObject["token"] ?? throw new InvalidOperationException()).Value<string>();
                    Debug.Log(token);
                    PlayerPrefs.SetString("token", token);
                    StartCoroutine(IELoadGameScene("Gameplay"));
                }
            }
            else
            {
                Debug.Log(request.error);
            }
        }

        #endregion

        #region For Username login

        private void OnBtnLoginClick()
        {
            
            UsernameLoginForm form = new UsernameLoginForm
            {
                credentials = txtLoginUsername.text.ToLower().Replace("\u200B",""),
                password = txtLoginPassword.text.ToLower().Replace("\u200B","")
            };

            if (form.credentials == "" || form.password == "")
            {
                ShowNotification("Thông tin không được để trống");
                return;
            }
            
            var request = UnityWebService.Instance.CreateApiPostRequest("api/login_screen/login_with_username", form);
            StartCoroutine(UnityWebService.Instance.IESendRequest(request,OnPostLoginComplete));
            
        }
     

        private void OnPostLoginComplete(UnityWebRequest request)
        {
            if (request.result == UnityWebRequest.Result.Success)
            {
                var result = request.downloadHandler.text;
                if (result == "false")
                {
                    Debug.Log("Sai tai khoan hoac mat khau");
                    ShowNotification("Tài khoản hoặc mật khẩu không chính xác");
                }
                else
                {
                    JObject jObject = JObject.Parse(result);

                    string token = (jObject["token"] ?? throw new InvalidOperationException()).Value<string>();
                    Debug.Log(token);
                    PlayerPrefs.SetString("token", token);
                    StartCoroutine(IELoadGameScene("Gameplay"));
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
            form.credentials = txtRegUsername.text.ToLower().Replace("\u200B","");
            form.password = txtRegPassword.text.ToLower().Replace("\u200B","");
            if (form.credentials == "" || form.password == "")
            {
                ShowNotification("Thông tin không được để trống");
                return;
            }
            if (form.password != txtRegRepeatPassword.text.ToLower().Replace("\u200B", ""))
            {
                Debug.Log("mat khau khong trung");
                ShowNotification("Mật khẩu không trùng khớp");
                return;
            }
            
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
                    ShowNotification("Tên tài khoản đã tồn tại");
                }
                else
                {
                    JObject jObject = JObject.Parse(result);

                    string userId = (jObject["UserId"] ?? throw new InvalidOperationException()).Value<string>();
                    Debug.Log(userId);
                    ShowNotification("Đăng ký thành công, mời bạn đăng nhập");
                    panelRegWithUsername.GameObject().SetActive(false);
                    panelLoginWithUsername.GameObject().SetActive(true);
                }
                
            }
            else
            {
                Debug.Log(request.error);
            }
        }

        #endregion

        IEnumerator IELoadGameScene(string sceneName)
        {
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);

            while (!asyncLoad.isDone)
            {
                yield return null;
            }
        }
        
        bool IsValidEmail(string email)
        {
            var trimmedEmail = email.Trim();

            if (trimmedEmail.EndsWith(".")) {
                return false;
            }
            try {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == trimmedEmail;
            }
            catch {
                return false;
            }
        }
        
        private void ShowNotification(string content)
        {
            panelNoti.GameObject().SetActive(true);
            txtNoti.text = content;
        }
        
        
    }
}
