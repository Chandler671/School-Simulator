using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("动画控制")]
    public Animator animator;           // 人物的 Animator 组件
    public string walkBoolParam = "IsWalking"; // 控制行走动画的布尔参数名
    public int moveSpeed = 80; //人物行走速度
    public float baseSpeedForAni; //动画播放速度

    private float originalScaleX;        // 父节点local.x值，用于控制人物左右移动动画

    void Awake()
    {
        // 如果没有手动指定 Animator，则尝试获取
        if (animator == null)
            animator = GetComponent<Animator>();

        // 记录原始缩放（通常人物默认朝右，scale.x = 1）
        originalScaleX = transform.parent.localScale.x;
        baseSpeedForAni = 45;
    }

    /// <summary>
    /// 在移动的每一帧调用，根据移动方向设置朝向并确保行走动画播放
    /// </summary>
    /// <param name="direction">移动方向向量（世界坐标）</param>
    public void SetMoving(Vector2 direction)
    {
        // 激活行走动画
        if (animator != null)
        {
            animator.SetBool(walkBoolParam, true);
             // 根据实际速度映射动画速度
            float speedFactor = moveSpeed / baseSpeedForAni;
            animator.speed = Mathf.Max(0.1f, speedFactor); // 防止速度过低或负值
        }
            
        // 根据水平方向设置翻转
        if (direction.x > 0.01f)          // 向右移动
        {
            SetScaleX(originalScaleX);     // 保持原始朝向（正）
        }
        else if (direction.x < -0.01f)     // 向左移动
        {
            SetScaleX(-originalScaleX);    // 水平翻转
        }
        // 如果 direction.x 接近 0（垂直移动），则保持当前朝向不变
    }

    /// <summary>
    /// 移动结束时调用，停止行走动画
    /// </summary>
    public void StopMoving()
    {
        if (animator != null)
        {
            animator.SetBool(walkBoolParam, false);
            animator.speed = 1f; // 重置为默认速度（Idle 动画正常播放）
        }
    }

    // 辅助方法：安全修改 localScale.x
    private void SetScaleX(float newX)
    {
        Vector3 scale = transform.parent.localScale;
        scale.x = newX;
        transform.parent.localScale = scale;
    }
}