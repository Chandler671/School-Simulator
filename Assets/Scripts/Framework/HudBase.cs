using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class HudBase : MonoBehaviour
{
    public GameObject Root;
    protected Canvas canvas;
    protected BasePanel basePanel;
    protected bool isInit = true;

    private void Awake()
    {
        canvas = GetComponentInParent<Canvas>();
        basePanel = GetComponent<BasePanel>();
        Root = this.gameObject;
    }

    private void Start()
    {
        InitState();
        Enter(() => AddListeners());
    }

    private void OnDestroy() => RemoveListeners();

    protected virtual void InitState() { }

    protected virtual void AddListeners() { }

    protected virtual void RemoveListeners() { }

    protected void Enter(UnityAction complete) => canvas.FadeIn(Root, () => complete());

    protected void ExitTo(string sceneName) => canvas.FadeOut(Root, () => SceneManager.LoadScene(sceneName));

    protected void UpdateView<T>(T t) where T : BasePanel => t.Refresh();
}