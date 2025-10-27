using System;
using UnityEngine;

public class Item : MonoBehaviour
{
    public ObjectType type;
    Rigidbody2D rigid;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        rigid.linearVelocity = Vector2.down * 1.5f;
    }
}