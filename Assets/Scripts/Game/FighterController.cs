using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class FighterController : MonoBehaviour
{
    public bool isTouchTop;
    public bool isTouchBottom;
    public bool isTouchRight;
    public bool isTouchLeft;

    public int life;
    public int score;
    public float speed;
    public float maxShotDelay;
    public float curShotDelay;
    public float power;
    public CharacterData characterData;
    
    public GameObject bulletAPrefab;
    public GameObject bulletBPrefab;
    
    Animator animator;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }
    void Update()
    {
        Move();
        Fire();
        Reload();
    }

    void Move()
    {
        float h = Input.GetAxisRaw("Horizontal");
        if ((isTouchLeft && h >= 1f) || (isTouchRight && h <= -1f))
            h = 0;
        
        float v = Input.GetAxisRaw("Vertical");
        if ((isTouchTop && v >= 1f) || (isTouchBottom && v <= -1f))
            v = 0;
        
        Vector3 cusPos = transform.position;
        Vector3 nextPos = new Vector3(h, v, 0) * speed * Time.deltaTime;
        transform.position = cusPos + nextPos;

        if (Input.GetButtonDown("Horizontal") || Input.GetButtonUp("Horizontal"))
            animator.SetInteger(GameDef.Hash.State, (int)h);
    }

    void Fire(bool isAuto = false)
    {
        if (!isAuto && !Input.GetButton("Fire1"))
            return;
        
        if (curShotDelay < maxShotDelay)
            return;

        switch (power)
        {
            case 1:
                GetBulletRigidBody((bulletAPrefab, transform.position));
                break;
            case 2:
                GetBulletRigidBody((bulletAPrefab, transform.position + Vector3.right * 0.1f), 
                    (bulletAPrefab, transform.position + Vector3.left * 0.1f));
                break;
            case 3:
                GetBulletRigidBody((bulletAPrefab, transform.position + Vector3.right * 0.25f), 
                    (bulletBPrefab, transform.position),
                    (bulletAPrefab, transform.position + Vector3.left * 0.25f));
                break;
        }
        
        curShotDelay = 0;
    }

    void GetBulletRigidBody(params (GameObject bulletPrefab, Vector3 pos)[] data)
    {
        List<Rigidbody2D> result = new List<Rigidbody2D>();
        foreach (var (prefab, pos) in data)
        {
            GameObject bullet = Instantiate(prefab, pos, transform.rotation);
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
    
    public void TestFlight()
    {
        float moveSpeed = 5f;
        float rangeX = 3f;
        float rangeY = 3f;
        Vector3 originPos = transform.position;
        StartCoroutine(MoveLoop(originPos, rangeX, rangeY, moveSpeed));
    }
    
    IEnumerator MoveLoop(Vector3 originPos, float rangeX, float rangeY, float moveSpeed)
    {
        while (true)
        {
            Vector3 targetPos = SetNewTarget(rangeX, rangeY, originPos);

            while (Vector3.Distance(transform.position, targetPos) > 0.1f)
            {
                float directionX = targetPos.x > transform.position.x ? 1f : (targetPos.x < transform.position.x ? -1f : 0f);
                animator.SetInteger(GameDef.Hash.State, (int)directionX);
                transform.position = Vector3.MoveTowards(
                    transform.position,
                    targetPos,
                    moveSpeed * Time.deltaTime);

                Fire(true);
                Reload();

                yield return null;
            }

            animator.SetInteger(GameDef.Hash.State, 0);
        }
    }

    Vector3 SetNewTarget(float rangeX, float rangeY, Vector3 originPos)
    {
        float randomX = Random.Range(-rangeX, rangeX);
        float randomY = Random.Range(0, rangeY);
        Vector3 targetPos = originPos + new Vector3(randomX, randomY, 0f);
        return targetPos;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Border"))
        {
            switch (other.gameObject.name)
            {
                case "Top":
                    isTouchTop = true;
                    break;
                case "Bottom":
                    isTouchBottom = true;
                    break;
                case "Right":
                    isTouchRight = true;
                    break;
                case "Left":
                    isTouchLeft = true;
                    break;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Border"))
        {
            switch (other.gameObject.name)
            {
                case "Top":
                    isTouchTop = false;
                    break;
                case "Bottom":
                    isTouchBottom = false;
                    break;
                case "Right":
                    isTouchRight = false;
                    break;
                case "Left":
                    isTouchLeft = false;
                    break;
            }
        }
        else if (other.gameObject.CompareTag("Enemy") || other.gameObject.CompareTag("EnemyBullet"))
        {
            life--;
            gameMain.UpdateLifeIcon(life);

            if (life == 0)
                gameMain.GameOver();
            else
                gameMain.RespawnPlayer();
            
            gameObject.SetActive(false);
            Destroy(other.gameObject);
        }
    }
}