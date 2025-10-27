using System;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [HideInInspector] public EnemyData enemyData;
    // public int point;
    // public float speed;
    // public int health;
    // public Sprite[] sprites;

    // public float maxShotDelay;
    public float curShotDelay;

    public Transform playerTransform;

    public Action<ObjectType, int, Vector3> onEnemyDie;
    [NonSerialized]
    private IObjectProvider objectProvider;
    private SpriteRenderer spriteRenderer;
    Animator animator;

    private int patternIndex = 0;
    private int curPatternCount = 0;
    private int[] maxPatternCount= new[] { 2, 3, 99, 10 };

    void Awake() => spriteRenderer = GetComponent<SpriteRenderer>();

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (enemyData.objectType == ObjectType.BossA)
        {
            animator = GetComponent<Animator>();
            Invoke(nameof(Stop), 2);
        }
    }

    void Stop()
    {
        if (!gameObject.activeSelf)
        {
            Debug.Log("return");
            return;
        }

        Rigidbody2D rigidbody2D = GetComponent<Rigidbody2D>();
        rigidbody2D.linearVelocity = Vector2.zero;

        Invoke(nameof(Think), 2);
    }

    void Think()
    {
        Debug.Log("Think");
        patternIndex = patternIndex == 3 ? 0 : patternIndex + 1;
        curPatternCount = 0;
        Debug.Log($"patternIndex:{patternIndex}");
        switch (patternIndex)
        {
            case 0:
                FireFoward();
                break;
            case 1:
                FireShot();
                break;
            case 2:
                FireArc();
                break;
            case 3:
                FireAround();
                break;
        }
    }

    void FireFoward()
    {
        Debug.Log("FireFoward");
        GetBulletRigidBody(
            new BulletSpawnData(ObjectType.BulletEnemyD, transform.position + Vector3.right * 0.3f, playerTransform.position - (transform.position + Vector3.right * 0.3f), 8), 
            new BulletSpawnData(ObjectType.BulletEnemyD, transform.position + Vector3.right * 0.45f, playerTransform.position - (transform.position + Vector3.right * 0.3f), 8),
            new BulletSpawnData(ObjectType.BulletEnemyD, transform.position + Vector3.left * 0.3f, playerTransform.position - (transform.position + Vector3.left * 0.3f), 8),
            new BulletSpawnData(ObjectType.BulletEnemyD, transform.position + Vector3.left * 0.45f, playerTransform.position - (transform.position + Vector3.left * 0.3f), 8));
        
        curPatternCount++;
        if (curPatternCount < maxPatternCount[patternIndex])
            Invoke(nameof(FireFoward), 2);
        else
            Invoke(nameof(Think), 3);
    }
    
    void FireShot()
    {
        Debug.Log("FireShot");
        for (int i = 0; i < 5; i++)
        {
            Vector2 dicVec = playerTransform.position - transform.position;
            Vector2 ranVec = new Vector2(UnityEngine.Random.Range(-0.5f, 0.5f), UnityEngine.Random.Range(0f, 2f));
            GetBulletRigidBody(new BulletSpawnData(ObjectType.BulletEnemyB, transform.position, dicVec + ranVec, 3));
        }

        curPatternCount++;
        if (curPatternCount < maxPatternCount[patternIndex])
            Invoke(nameof(FireShot), 3.5f);
        else
            Invoke(nameof(Think), 3);
    }
    
    void FireArc()
    {
        Debug.Log("FireArc");
        Vector2 dicVec = new Vector2(Mathf.Cos(Mathf.PI * 10 * curPatternCount / maxPatternCount[patternIndex]), -1);
        GetBulletRigidBody(new BulletSpawnData(ObjectType.BulletEnemyA, transform.position, dicVec, 3, Quaternion.identity));
        
        curPatternCount++;
        if (curPatternCount < maxPatternCount[patternIndex])
            Invoke(nameof(FireArc), 0.15f);
        else
            Invoke(nameof(Think), 3);
    }
    
    void FireAround()
    {
        Debug.Log("FireAround");
        int maxNum = curPatternCount % 2 == 0 ? 50 : 40;
        for (int i = 0; i < maxNum; i++)
        {
            Vector2 dicVec = new Vector2(Mathf.Cos(Mathf.PI * 2 * i / maxNum), Mathf.Sin(Mathf.PI * 2 * i / maxNum));
            Vector3 rotVec = Vector3.forward * 360 * i / maxNum + Vector3.forward * 90;
            GetBulletRigidBody(new BulletSpawnData(ObjectType.BulletEnemyC, transform.position, dicVec, 2, null, rotVec));
        }

        curPatternCount++;
        if (curPatternCount < maxPatternCount[patternIndex])
            Invoke(nameof(FireAround), 0.7f);
        else
            Invoke(nameof(Think), 3);
    }

    void Update()
    {
        if (enemyData.objectType == ObjectType.BossA)
            return;
        
        Fire();
        Reload();
    }

    public void SetupProvider(IObjectProvider provider) => objectProvider = provider;

    private void Reload() => curShotDelay += Time.deltaTime;
    private void ReturnSprite() => spriteRenderer.sprite = enemyData.sprites[0];

    void Fire()
    {
        if (curShotDelay < enemyData.attackSpeed)
            return;

        switch (enemyData.objectType)
        {
            case ObjectType.EnemyA:
                
                GetBulletRigidBody(new BulletSpawnData(ObjectType.BulletEnemyA, transform.position, playerTransform.position - transform.position, 3));
                break;
            case ObjectType.EnemyC:
                GetBulletRigidBody(
                    new BulletSpawnData(ObjectType.BulletEnemyB, transform.position + Vector3.right * 0.3f, playerTransform.position - (transform.position + Vector3.right * 0.3f), 4), 
                    new BulletSpawnData(ObjectType.BulletEnemyB, transform.position + Vector3.left * 0.3f, playerTransform.position - (transform.position + Vector3.left * 0.3f), 3));
                break;
        }
        
        curShotDelay = 0;
    }

    private void GetBulletRigidBody(params BulletSpawnData[] dataList)
    {
        Dictionary<Rigidbody2D, Vector2> result = new Dictionary<Rigidbody2D, Vector2>();
        foreach (var data in dataList)
        {
            GameObject bullet = objectProvider.MakeObject(data.prefab, data.pos);
            if (data.rotPrev.HasValue)
                bullet.transform.rotation = data.rotPrev.Value;

            try
            {
                Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
                result.Add(rb, data.dirVec.normalized * data.speed);
                if (data.rotNext.HasValue)
                    bullet.transform.Rotate(data.rotNext.Value);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Debug.Log(e.StackTrace);
                throw;
            }
        }
        foreach (var rb in result)
            rb.Key.AddForce(rb.Value, ForceMode2D.Impulse);
    }
    
    public void OnHit(int dmg)
    {
        // Debug.Log($"{enemyData.objectType} hit {dmg}!");
        if (enemyData.health <= 0)
            return;
        
        enemyData.health -= dmg;

        if (enemyData.objectType == ObjectType.BossA)
            animator.SetTrigger(GameDef.Hash.IsHit);
        else
        {
            spriteRenderer.sprite = enemyData.sprites[1];
            Invoke(nameof(ReturnSprite), 0.1f);
        }

        if (enemyData.health <= 0)
        {
            onEnemyDie(enemyData.objectType, enemyData.point, transform.position);
            gameObject.SetActive(false);
            transform.rotation = Quaternion.identity;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("BorderBullet") && enemyData.objectType != ObjectType.BossA)
        {
            // Debug.Log($"test, other: {other.gameObject.name}");
            gameObject.SetActive(false);
            transform.rotation = Quaternion.identity;
        }
        else if (other.gameObject.CompareTag("PlayerBullet"))
        {
            Bullet bullet = other.gameObject.GetComponent<Bullet>();
            OnHit(bullet.totalDamage);
            other.gameObject.SetActive(false);
        }
    }
}