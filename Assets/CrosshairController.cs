using UnityEngine;
using UnityEngine.UI;

public class CrosshairController : MonoBehaviour
{
    [Header("Линии прицела")]
    public RectTransform top;
    public RectTransform bottom;
    public RectTransform left;
    public RectTransform right;

    [Header("Настройки анимации")]
    public float normalOffset = 20f;
    public float recoilOffset = 40f;
    public float recoilDuration = 0.2f;

    private bool isRecoiling = false;
    private float recoilTimer = 0f;

    void Start()
    {
        ResetCrosshair();
    }

    void Update()
    {
        if (isRecoiling)
        {
            recoilTimer += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(recoilTimer / recoilDuration);

            float currentOffset = Mathf.Lerp(recoilOffset, normalOffset, t);
            SetCrosshairOffset(currentOffset);

            if (t >= 1f)
            {
                isRecoiling = false;
            }
        }
    }

    public void TriggerRecoil()
    {
        isRecoiling = true;
        recoilTimer = 0f;
        SetCrosshairOffset(recoilOffset);
    }

    void ResetCrosshair()
    {
        SetCrosshairOffset(normalOffset);
    }

    void SetCrosshairOffset(float offset)
    {
        top.anchoredPosition = new Vector2(0, offset);
        bottom.anchoredPosition = new Vector2(0, -offset);
        left.anchoredPosition = new Vector2(-offset, 0);
        right.anchoredPosition = new Vector2(offset, 0);
    }

    public void Show()
    {
        gameObject.SetActive(true);
        ResetCrosshair();
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}