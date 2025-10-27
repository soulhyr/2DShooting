using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Follower : MonoBehaviour
{
    public ObjectType objectType;
    public PlayerInfo playerInfo;

    public Vector3 followPos;
    private int followDelay = 12;
    public Transform parent;
    public Queue<Vector3> parentPos;
    
    private float curShotDelay;
    [NonSerialized]
    private IObjectProvider objectProvider;

    void Awake()
    {
        parentPos = new Queue<Vector3>();
    }
    
    void Update()
    {
        Watch();
        Follow();
        Fire();
        Reload();
    }

    void Watch()
    {
        if (!parentPos.Contains(parent.position))
            parentPos.Enqueue(parent.position);
        
        if (parentPos.Count > followDelay)
            followPos = parentPos.Dequeue();
        else if (parentPos.Count < followDelay)
            followPos = parent.position;
    }

    void Follow()
    {
        transform.position = followPos;
    }
    
    public void SetupProvider(IObjectProvider provider)
    {
        objectProvider = provider;
    }

    void Fire(bool isAuto = false)
    {
        if (!isAuto && !Utility.GetFire())
            return;
        
        if (curShotDelay < playerInfo.attackSpeed)
            return;

        GetBulletRigidBody((ObjectType.BulletFollow, transform.position));
        curShotDelay = 0;
    }
    
    void GetBulletRigidBody(params (ObjectType bulletPrefab, Vector3 pos)[] data)
    {
        List<Rigidbody2D> result = new List<Rigidbody2D>();
        foreach (var (prefab, pos) in data)
        {
            GameObject bullet = objectProvider.MakeObject(prefab, pos);
            bullet.transform.position = pos;
            bullet.GetComponent<Bullet>().SetTotalDamage(playerInfo.damage);
            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            result.Add(rb);
        }

        foreach (var rb in result)
            rb.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
    }
    
    void Reload()
    {
        curShotDelay += Time.deltaTime;
    }
}