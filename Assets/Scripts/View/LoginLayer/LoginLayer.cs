using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using DG.Tweening;

public class LoginLayer : BasePanel
{
    [Header("UI组件引用")]
    [SerializeField] private InputField usernameInputField;
    [SerializeField] private Button loginButton;
    [SerializeField] private Text placeHolder;
    [SerializeField] private float sparkTime;
    private Transform newPlayerBtn;

    [SerializeField] private float loginProcessTime = 1.5f;

    // 可扩展的事件接口
    [Header("事件接口")]
    public UnityEvent onLoginStarted;          // 登录开始时触发
    public UnityEvent onLoginSuccess;          // 登录成功时触发
    public UnityEvent onLoginFailed;           // 登录失败时触发

    // 用于存储登录状态
    private bool isLoggingIn = false;
    private string currentUsername = "";
    private PlayerModel _playerModel;

    protected override void onOpenPanel()
    {
        this.newPlayerBtn = transform.Find("down/BtnGroup/newPlayBtn");
        currentUsername = PlayerPrefsManager.Instance.GetString("CurrentUser");
        InitializeUI();
        SetupEventListeners();
        this.name = UIConst.LoginLayer;
    }

    void InitializeUI()
    {

        // 设置输入框默认属性
        if (usernameInputField != null)
        {
            usernameInputField.contentType = InputField.ContentType.Standard; ;
            usernameInputField.text = currentUsername;
            if (currentUsername == "" && placeHolder != null && this.transform.gameObject != null)
            {
                placeHolder.text = "请输入昵称";
                placeHolder.GetComponent<Text>().DOFade(0, sparkTime).SetLoops(-1, LoopType.Yoyo);
            }
        }

        usernameInputField.gameObject.SetActive(false);
        // 判断是否有其它存档
        if (PlayerPrefsManager.Instance.GetString("CurrentUser") != "")
        {
            newPlayerBtn.gameObject.SetActive(true);
            loginButton.gameObject.SetActive(true);
            loginButton.transform.Find("txt").GetComponent<Text>().text = "继续游戏";
        }
        else
        {
            newPlayerBtn.gameObject.SetActive(false);
            usernameInputField.gameObject.SetActive(true);
        }
    }

    public void onSelect()
    {
        placeHolder.gameObject.SetActive(false);
    }

    public void onDeselect()
    {
        if (placeHolder || placeHolder.text == "")
        {
            placeHolder.gameObject.SetActive(true);
        }
        return;
    }
    void SetupEventListeners()
    {
        // 绑定登录按钮点击事件
        if (loginButton != null)
        {
            loginButton.onClick.AddListener(OnLoginButtonClicked);
        }

        if (newPlayerBtn != null)
        {
            newPlayerBtn.GetComponent<Button>().onClick.AddListener(OnNewPlayer);
        }

        // 绑定输入框回车键提交
        if (usernameInputField != null)
        {
            usernameInputField.onEndEdit.AddListener((value) =>
            {
                if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
                {
                    currentUsername = value;
                }
            });
        }
    }


    // 登录按钮点击事件处理
    public void OnLoginButtonClicked()
    {
        // 防止重复点击
        if (isLoggingIn) return;

        // 触发登录开始事件
        onLoginStarted?.Invoke();

        currentUsername = usernameInputField.text;
        PlayerPrefsManager.Instance.SetString("CurrentUser", currentUsername);

        if (currentUsername != "")
        {
            PlayerModel.Instance.playerJson = PlayerPrefsManager.Instance.GetObject<PlayerJson>(currentUsername);
            // 开始登录流程
            StartCoroutine(LoginProcess(currentUsername));
            return;
        }

        // 获取输入的用户名和密码
        string username = usernameInputField != null ? usernameInputField.text : "";

        // 验证输入
        if (!ValidateInput(username))
        {
            return;
        }

        // 开始登录流程
        StartCoroutine(LoginProcess(username));
    }

    // 输入验证
    bool ValidateInput(string username)
    {
        // 检查用户名是否为空
        if (string.IsNullOrWhiteSpace(username))
        {
            G.ShowMessage("用户名不能为空");
            return false;
        }

        // 检查用户名长度（示例）
        if (username.Length < 3 || username.Length > 7)
        {
            G.ShowMessage("用户名长度需在3-7个字符之间");
            return false;
        }

        return true;
    }

    // 登录流程协程
    IEnumerator LoginProcess(string username)
    {
        isLoggingIn = true;
        currentUsername = username;

        // 禁用登录按钮防止重复点击
        SetLoginButtonInteractable(false);

        // 模拟登录处理时间（可替换为真实的网络请求）
        Debug.Log($"开始登录处理: 用户[{username}]");
        float timer = 0f;

        while (timer < loginProcessTime)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        // 模拟登录验证（在实际项目中替换为服务器验证）
        LoginModel.Instance.SimulateLoginValidation(username);

        // 登录成功
        OnLoginSuccess();

        // 恢复登录按钮
        SetLoginButtonInteractable(true);
        isLoggingIn = false;
    }



    // 登录成功处理
    void OnLoginSuccess()
    {
        //判断是否过了新手引导
        if (PlayerModel.Instance.playerJson.isNewPlayer)
        {
            UIManager.Instance.OpenPanel(UIConst.NewGuideView);
            ClosePanel();
        }
        else
        {
            UIManager.Instance.OpenPanel(UIConst.MainCityLayer);
            EventManager.QueueEvent(new login_Success(PlayerModel.Instance.playerJson.username, PlayerModel.Instance.playerJson.heroId));
        }
    }

    void OnNewPlayer()
    {
        loginButton.transform.Find("txt").GetComponent<Text>().text = "开始游戏";
        usernameInputField.gameObject.SetActive(true);
        loginButton.gameObject.SetActive(true);
        newPlayerBtn.gameObject.SetActive(false);
    }

    // 登录失败处理
    void OnLoginFailed(string errorMsg = "登录失败")
    {
        Debug.Log($"登录失败: {errorMsg}");

        // 触发登录失败事件
        onLoginFailed?.Invoke();

        // 显示错误信息
        G.ShowMessage(errorMsg);
    }


    // 设置登录按钮交互状态
    void SetLoginButtonInteractable(bool interactable)
    {
        if (loginButton != null)
        {
            loginButton.interactable = interactable;
        }
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        onLoginSuccess.RemoveAllListeners();
        onLoginStarted.RemoveAllListeners();
        onLoginFailed.RemoveAllListeners();
    }
}