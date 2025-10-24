using System;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public string enemyName;
    public int enemyScore;
    public float speed;
    public int health;
    public Sprite[] sprites;

    public float maxShotDelay;
    public float curShotDelay;

    public GameObject bulletAPrefab;
    public GameObject bulletBPrefab;

    public GameObject player;
    
    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    void Update()
    {
        Fire();
        Reload();
    }

    void Fire(bool isAuto = false)
    {
        if (curShotDelay < maxShotDelay)
            return;

        switch (enemyName)
        {
            case "A":
                GetBulletRigidBody((bulletAPrefab, transform.position, player.transform.position - transform.position));
                break;
            case "C":
                GetBulletRigidBody((bulletBPrefab, transform.position + Vector3.right * 0.3f, player.transform.position - (transform.position + Vector3.right * 0.3f)), 
                    (bulletBPrefab, transform.position + Vector3.left * 0.3f, player.transform.position - (transform.position + Vector3.left * 0.3f)));
                break;
        }
        
        curShotDelay = 0;
    }

    void GetBulletRigidBody(params (GameObject bulletPrefab, Vector3 pos, Vector3 dirVec)[] data)
    {
        Dictionary<Rigidbody2D, Vector2> result = new Dictionary<Rigidbody2D, Vector2>();
        foreach (var (prefab, pos, dirVec) in data)
        {
            GameObject bullet = Instantiate(prefab, pos, transform.rotation);
            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            result.Add(rb, dirVec.normalized * 10);
        }
        foreach (var rb in result)
            rb.Key.AddForce(rb.Value, ForceMode2D.Impulse);
    }
    
    void Reload()
    {
        curShotDelay += Time.deltaTime;
    }
    
    void OnHit(int dmg)
    {
        health -= dmg;
        spriteRenderer.sprite = sprites[1];
        Invoke(nameof(ReturnSprite), 0.1f);

        if (health <= 0)
        {
            FighterController playerLogic = player.GetComponent<FighterController>();
            playerLogic.score += enemyScore;
            Destroy(gameObject);
        }
    }

    void ReturnSprite()
    {
        spriteRenderer.sprite = sprites[0];
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("BorderBullet"))
            Destroy(gameObject);
        else if (other.gameObject.CompareTag("PlayerBullet"))
        {
            Bullet bullet = other.gameObject.GetComponent<Bullet>();
            OnHit(bullet.damage);
            Destroy(other.gameObject);
        }
    }
}