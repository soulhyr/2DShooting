using System;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    Animator animator;

    void Awake() => animator = GetComponent<Animator>();
    private void OnEnable() => Invoke(nameof(Disable), 2f);
    private void Disable() => gameObject.SetActive(false);

    public void StartExplosion(ObjectType objectType)
    {
        animator.SetTrigger(GameDef.Hash.IsExplosion);

        transform.localScale = objectType switch
        {
            ObjectType.EnemyA => Vector3.one * 0.7f,
            ObjectType.EnemyC => Vector3.one * 2f,
            ObjectType.BossA => Vector3.one * 3f,
            _ => Vector3.one * 1f
        };
    }
}