using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using System.Collections;
using DG.Tweening;

public class LoginLayer : BasePanel
{
    [Header("UI组件引用")]
    [SerializeField] private InputField usernameInputField;
    [SerializeField] private Button loginButton;
    [SerializeField] private Text errorMessageText;
    [SerializeField] private Text placeHolder;
    [SerializeField] private float sparkTime;

    [Header("登录设置")]
    [SerializeField] private string defaultUsername = "player";
    [SerializeField] private float loginProcessTime = 1.5f;
    
    [Header("场景设置")]
    [SerializeField] private string nextSceneName = "MainMenu";
    [SerializeField] private float sceneTransitionDelay = 0.5f;
    
    // 可扩展的事件接口
    [Header("事件接口")]
    public UnityEvent onLoginStarted;          // 登录开始时触发
    public UnityEvent onLoginSuccess;          // 登录成功时触发
    public UnityEvent onLoginFailed;           // 登录失败时触发
    public UnityEvent onSceneTransitionStart;  // 场景切换开始时触发
    
    // 用于存储登录状态
    private bool isLoggingIn = false;
    private string currentUsername = "";

    protected override void onOpenPanel()
    {
        InitializeUI();
        SetupEventListeners();
    }

    void InitializeUI()
    {
        // 初始化错误信息文本
        if (errorMessageText != null)
        {
            errorMessageText.text = "";
            errorMessageText.gameObject.SetActive(false);
        }

        // 设置输入框默认属性
        if (usernameInputField != null)
        {
            usernameInputField.contentType = InputField.ContentType.Standard; ;
            usernameInputField.text = currentUsername;
            if (currentUsername == "")
            {
                placeHolder.text = "请输入昵称";
                placeHolder.GetComponent<Text>().DOFade(0, sparkTime).SetLoops(-1, LoopType.Yoyo);
            }
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
        
        // 绑定输入框回车键提交
        if (usernameInputField != null)
        {
            usernameInputField.onEndEdit.AddListener((value) => {
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
            ShowErrorMessage("用户名不能为空");
            return false;
        }
        
        // 检查用户名长度（示例）
        if (username.Length < 3 || username.Length > 20)
        {
            ShowErrorMessage("用户名长度需在3-20个字符之间");
            return false;
        }
        
        // 清除错误信息
        ClearErrorMessage();
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
            
            // 这里可以更新登录进度UI（如果添加了进度条）
            // UpdateLoginProgress(timer / loginProcessTime);
            
            yield return null;
        }
        
        // 模拟登录验证（在实际项目中替换为服务器验证）
        bool loginSuccess = SimulateLoginValidation(username);
        
        if (loginSuccess)
        {
            // 登录成功
            OnLoginSuccess();
        }
        else
        {
            // 登录失败
            OnLoginFailed("不存在该用户名，您是否想要重新创建");
        }
        
        // 恢复登录按钮
        SetLoginButtonInteractable(true);
        isLoggingIn = false;
    }
    
    // 模拟登录验证（在实际项目中替换为网络请求）
    bool SimulateLoginValidation(string username)
    {
        // 这里使用简单的本地验证（实际项目应连接服务器）
        if (username == defaultUsername)
        {
            return true;
        }
        
        // 还可以添加更多测试账号
        if (username == "admin")
        {
            return true;
        }
        
        return false;
    }
    
    // 登录成功处理
    void OnLoginSuccess()
    {
        Debug.Log($"登录成功: 用户[{currentUsername}]");
        
        // 触发登录成功事件
        onLoginSuccess?.Invoke();
        
        // 保存用户数据（可扩展）
        SaveUserData(currentUsername);
        
        // 跳转到下一个场景
        StartCoroutine(TransitionToNextScene());
    }
    
    // 登录失败处理
    void OnLoginFailed(string errorMsg = "登录失败")
    {
        Debug.Log($"登录失败: {errorMsg}");
        
        // 触发登录失败事件
        onLoginFailed?.Invoke();
        
        // 显示错误信息
        ShowErrorMessage(errorMsg);
    }
    
    // 跳转到下一个场景
    IEnumerator TransitionToNextScene()
    {
        // 触发场景切换开始事件
        onSceneTransitionStart?.Invoke();
        
        // 可选的：播放场景切换动画
        // PlayTransitionAnimation();
        
        // 等待短暂延迟
        yield return new WaitForSeconds(sceneTransitionDelay);
        
        // 加载下一个场景
        if (!string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
        }
        else
        {
            Debug.LogWarning("未设置下一个场景名称");
        }
    }
    
    // 显示错误信息
    void ShowErrorMessage(string message)
    {
        if (errorMessageText != null)
        {
            errorMessageText.text = message;
            errorMessageText.gameObject.SetActive(true);
            DOTween.Sequence().Append(errorMessageText.transform.DOLocalMove(new Vector3(0, 410, 0), 0.5f))
            .AppendInterval(1f)
            .AppendCallback(() =>
            {
                errorMessageText.gameObject.SetActive(false);
                errorMessageText.transform.localPosition = new Vector3(0, 563, 0);
            });
        }
    }

    // 清除错误信息
    void ClearErrorMessage()
    {
        if (errorMessageText != null)
        {
            errorMessageText.text = "";
            errorMessageText.gameObject.SetActive(false);
        }
    }
    
    // 设置登录按钮交互状态
    void SetLoginButtonInteractable(bool interactable)
    {
        if (loginButton != null)
        {
            loginButton.interactable = interactable;
        }
    }
    
    // 保存用户数据（预留的扩展接口）
    void SaveUserData(string username)
    {
        // 保存到PlayerPrefs（简单示例）
        PlayerPrefs.SetString("LastLoginUsername", username);
        PlayerPrefs.SetString("LoginTimestamp", System.DateTime.Now.ToString());
        PlayerPrefs.Save();
        
        // 可以扩展：保存到GameManager、本地数据库等
        Debug.Log($"用户数据已保存: {username}");
    }
    
    // 公开方法：供外部调用的接口
    
    // 清空输入框
    public void ClearInputFields()
    {
        if (usernameInputField != null) usernameInputField.text = "";
        ClearErrorMessage();
    }
    
    // 设置测试账号
    public void SetTestCredentials(string username, string password)
    {
        if (usernameInputField != null) usernameInputField.text = username;
    }
    
    // 获取当前登录状态
    public bool IsLoggingIn()
    {
        return isLoggingIn;
    }
    
    // 获取当前输入的用户名
    public string GetCurrentUsername()
    {
        return usernameInputField != null ? usernameInputField.text : "";
    }
    
    // 快速登录方法（供测试使用）
    public void QuickLogin(string username, string password)
    {
        if (usernameInputField != null) usernameInputField.text = username;
        OnLoginButtonClicked();
    }
    
    // 在编辑器中重置到默认值
    void Reset()
    {
        // 这些值会在组件添加到GameObject时自动设置
        nextSceneName = "MainMenu";
        loginProcessTime = 1.5f;
        sceneTransitionDelay = 0.5f;
    }
}