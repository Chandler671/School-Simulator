using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class LoadingPanel : MonoBehaviour
{
    [Header("UI引用")]
    [SerializeField] private Slider loadingSlider;
    [SerializeField] private Text loadingText;
    
    [Header("加载设置")]
    [SerializeField] private float fakeLoadTime;

    void Start()
    {
        StartCoroutine(LoadGames());
    }
    
    IEnumerator LoadGames()
    {
        // 1. 假加载阶段（进度条平滑增长）
        float fakeProgress = 0f;
        float fakeTimer = 0f;
        
        while (fakeTimer < fakeLoadTime)
        {
            fakeTimer += Time.deltaTime;
            fakeProgress = Mathf.Clamp01(fakeTimer / fakeLoadTime) * 0.7f; // 假加载到70%
            
            // 更新UI
            if (loadingSlider != null)
                loadingSlider.value = fakeProgress;
            if (loadingText != null)
                loadingText.text = $"加载中... {(fakeProgress * 100):F0}%";
            
            yield return null;
        }
        yield return new WaitForSeconds(0.6f);
        // 2. 真加载阶段（实际加载场景）
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("Main");
        asyncLoad.allowSceneActivation = false; // 不自动激活场景
        
        while (!asyncLoad.isDone)
        {
            // 计算总进度：假加载的70% + 真加载的30%
            float realProgress = asyncLoad.progress / 0.9f; // Unity的progress最大到0.9
            float totalProgress = 0.7f + realProgress * 0.3f;
            
            // 更新UI
            if (loadingSlider != null)
                loadingSlider.value = totalProgress;
            if (loadingText != null)
                loadingText.text = $"加载中... {(totalProgress * 100):F0}%";
            
            // 当加载到90%时
            if (asyncLoad.progress >= 0.9f)
            {
                // 确保显示100%
                if (loadingSlider != null)
                    loadingSlider.value = 1f;
                if (loadingText != null)
                    loadingText.text = "加载完成！";
                
                // 短暂等待，然后激活场景
                yield return new WaitForSeconds(0.5f);
                asyncLoad.allowSceneActivation = true;
            }
            
            yield return null;
        }
    }
}