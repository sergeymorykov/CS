using System;
using UnityEngine;

public class Target : MonoBehaviour
{
    public event Action<Target> OnDestroyed;

    private bool isHit = false;

    public void Hit()
    {
        if (isHit) return;
        isHit = true;

        OnDestroyed?.Invoke(this);
        Destroy(gameObject);
    }
}
