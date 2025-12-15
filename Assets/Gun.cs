using System;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class Gun : MonoBehaviour
{
    public float range = 100f;
    public ParticleSystem muzzleFlash;
    public float recoilAmount = 0.1f;
    public float recoilDuration = 0.1f;
    public Transform recoilTarget;
    public CrosshairController crosshair;

    [Tooltip("Время между выстрелами в секундах (например, 0.1 = 10 выстрелов/сек)")]
    public float fireRate = 0.1f;

    public ParticleSystem destroyEffect;

    public AudioClip shootSound;
    public float shootVolume = 1f;

    private InputSystem_Actions actions;
    private Vector3 originalLocalPosition;
    private bool isRecoiling = false;
    private float nextFireTime = 0f;

    void Awake()
    {
        actions = new InputSystem_Actions();
        actions.Player.Enable();

        if (recoilTarget == null)
            recoilTarget = transform;

        originalLocalPosition = recoilTarget.localPosition;

        if (muzzleFlash != null)
            muzzleFlash.Stop();
    }

    public void EnableInput()
    {
        actions.Player.Enable();
    }

    public void DisableInput()
    {
        actions.Player.Disable();
    }

    void Update()
    {
        if (actions.Player.Attack.IsPressed() && Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + fireRate;
        }

        if (isRecoiling)
        {
            recoilTarget.localPosition = Vector3.Lerp(
                recoilTarget.localPosition,
                originalLocalPosition,
                Time.deltaTime / recoilDuration
            );

            if (Vector3.Distance(recoilTarget.localPosition, originalLocalPosition) < 0.001f)
            {
                recoilTarget.localPosition = originalLocalPosition;
                isRecoiling = false;
            }
        }
    }

    void Shoot()
    {
        // Запрет стрельбы, если игра на паузе
        if (Time.timeScale == 0f) return;

        if (shootSound != null)
        {
            AudioSource.PlayClipAtPoint(shootSound, transform.position, shootVolume);
        }

        if (muzzleFlash != null)
        {
            StartCoroutine(PlayMuzzleFlash());
        }

        if (crosshair != null)
        {
            crosshair.TriggerRecoil();
        }

        Ray ray = new Ray(transform.position, transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, range))
        {
            if (hit.collider != null && hit.collider.CompareTag("Target"))
            {
                Target target = hit.collider.GetComponent<Target>();
                if (target != null)
                {
                    // Начисляем очки
                    GameManager.Instance?.AddScore();

                    // Воспроизводим эффект уничтожения
                    if (destroyEffect != null)
                    {
                        ParticleSystem effectInstance =
                            Instantiate(destroyEffect, hit.point, Quaternion.identity);

                        effectInstance.Play();

                        Destroy(effectInstance.gameObject, effectInstance.main.duration);
                    }

                    // Уничтожаем мишень (это вызовет событие OnDestroyed)
                    target.Hit();
                }
            }
        }

        isRecoiling = true;
        recoilTarget.localPosition = originalLocalPosition + Vector3.back * recoilAmount;
    }

    IEnumerator PlayMuzzleFlash()
    {
        muzzleFlash.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        muzzleFlash.Play(true);

        // Ждём окончания эффекта
        yield return new WaitForSeconds(muzzleFlash.main.duration);

        // Останавливаем полностью (на всякий случай)
        muzzleFlash.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
    }

    void OnDestroy()
    {
        actions?.Dispose();
    }
}