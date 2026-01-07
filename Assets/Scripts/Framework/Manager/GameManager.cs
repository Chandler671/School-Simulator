using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;

    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                // 自动创建管理器（如果场景中没有）
                GameObject obj = new GameObject("GameManager");
                _instance = obj.AddComponent<GameManager>();
                DontDestroyOnLoad(obj); // 跨场景不销毁
            }
            return _instance;
        }
    }

    [Header("分辨率设置")]
    [SerializeField] private int targetWidth = 1920;
    [SerializeField] private int targetHeight = 1080;
    [SerializeField] private int targetRefreshRate = 60;
    
    [Header("场景设置")]
    [SerializeField] private string loadingSceneName = "Loading";

    private PackageTable packageTable;
    private bool isInitialized = false;

    // 自动保存数据间隔（秒）
    private float _autoSaveInterval = PlayerPrefsManager.Instance._autoSaveInterval;
    // 自动保存计时器
    private float _autoSaveTimer = PlayerPrefsManager.Instance._autoSaveTimer;
    private void Awake()
    {
        // 确保单例
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(gameObject);
        InitializeGame();
        Debug.Log("Game已初始化");
    }


    void Start()
    {
        
    }

    private void Update() 
    {
        // 自动保存数据
        if (this._autoSaveInterval > 0 && PlayerPrefsManager.Instance._isDirty)
        {
            this._autoSaveTimer += Time.deltaTime;
            if (this._autoSaveTimer >=this. _autoSaveInterval)
            {
                PlayerPrefsManager.Instance.ForceSave();
                this._autoSaveTimer = 0f;
            }
        }
    }

    void InitializeGame()
    {
        if (isInitialized) return;
        
        // 设置初始分辨率
        SetInitialResolution();
        
        // 确保当前是加载场景
        if (SceneManager.GetActiveScene().name != loadingSceneName)
        {
            SceneManager.LoadScene(loadingSceneName);
        }
        
        isInitialized = true;
    }
    
    void SetInitialResolution()
    {
        bool foundExactMatch = false;
        // 获取当前显示器支持的分辨率
        Resolution res = Screen.currentResolution;
        // 设置目标帧率
        RefreshRate targetRate = new RefreshRate
        {
            numerator = (uint)targetRefreshRate, // 例如 60
            denominator = 1
        };

        if (res.width == targetWidth && res.height == targetHeight )
        {
            foundExactMatch = true;
        }
        
        if (foundExactMatch)
        {
            Screen.SetResolution(res.width, 
                               res.height, 
                               FullScreenMode.Windowed, 
                               targetRate);
        }
        else
        {
            // 如果没有精确匹配，使用默认设置
            Screen.SetResolution(targetWidth, targetHeight, FullScreenMode.Windowed, targetRate);
        }
    }

    public PackageTable GetPackageTable()
    {
        if (packageTable == null)
        {
            packageTable = Resources.Load<PackageTable>("Data/PackageTable");
        }
        return packageTable;
    }

    public List<PackageLocalItem> GetPackageLocalData()
    {
        return PackageLocalData.Instance.LoadPackage();
    }

    public PackageTableItem GetPackageItemById(int id)
    {
        List<PackageTableItem> packageDataList = GetPackageTable().DataList;
        foreach (PackageTableItem item in packageDataList)
        {
            if (item.id == id)
            {
                return item;
            }
        }
        return null;
    }

    public PackageLocalItem GetPackageLocalItemByUId(string uid)
    {
        List<PackageLocalItem> packageDataList = GetPackageLocalData();
        foreach (PackageLocalItem item in packageDataList)
        {
            if (item.uid == uid)
            {
                return item;
            }
        }
        return null;
    }

    public List<PackageLocalItem> GetSortPackageLocalData()
    {
        List<PackageLocalItem> localItems = PackageLocalData.Instance.LoadPackage();
        localItems.Sort(new PackageItemComparer());
        return localItems;
    }
}

public class PackageItemComparer : IComparer<PackageLocalItem>
{
    public int Compare(PackageLocalItem a, PackageLocalItem b)
    {
        PackageTableItem x = GameManager.Instance.GetPackageItemById(a.id);
        PackageTableItem y = GameManager.Instance.GetPackageItemById(b.id);
        // 首先按star从大到小排序
        int starComparison = y.star.CompareTo(x.star);

        // 如果star相同，则按id从大到小排序
        if (starComparison == 0)
        {
            int idComparison = x.id.CompareTo(y.id);
            if (idComparison == 0)
            {
                return b.num.CompareTo(a.num);
            }
            return idComparison;
        }

        return starComparison;
    }
}
