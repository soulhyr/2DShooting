using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class Player : MonoBehaviour
{
    public ObjectType objectType;
    public PlayerInfo playerInfo;
    
    public GameObject fxBoom;
    
    public Action<Vector3?, ObjectType?> onPlayerLifeChanged;
    public Action onBoomChanged;
    public bool isHit;
    public bool isBoomTime;
    public int maxPower = 6;
    public int maxBoom = 3;
    
    private bool isTouchTop;
    private bool isTouchBottom;
    private bool isTouchRight;
    private bool isTouchLeft;

    private Animator animator;
    private float curShotDelay;
    
    private IObjectProvider objectProvider;

    private bool isRespwanTime;
    private SpriteRenderer spriteRenderer;
    
    private bool isFirst = false;

    private bool isButtonA;
    private bool isButtonB;
    
    public FixedJoystick joystick;
    
    void Awake()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        PlayerData data = DataManager.Instance.PlayerDataDict.FirstOrDefault(x => x.Value.objectType == objectType).Value;
        playerInfo = new PlayerInfo(data);
    }

    public void SetupPlay()
    {
        onPlayerLifeChanged(null, null);
        onBoomChanged();
    }

    private void OnEnable()
    {
        if (!isFirst)
            return;
        
        Unbeatable();
        Invoke(nameof(Unbeatable), 3);
    }

    private void Start()
    {
        isFirst = true;
    }

    void Unbeatable()
    {
        isRespwanTime = !isRespwanTime;
        Debug.Log($"isRespwanTime = {isRespwanTime}");
        spriteRenderer.color = new Color(1, 1, 1, isRespwanTime ? 0.5f : 1);
        GameObject[] arrGo = objectProvider.GetPool(ObjectType.Follow1);
        if (arrGo == null)
            return;
        foreach (var f in arrGo)
            f.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, isRespwanTime ? 0.5f : 1);
    }

    void Update()
    {
        // if (transform.hasChanged)
        // {
        //     // Debug.Log($"[TRACE] Player position changed: {transform.position}, frame={Time.frameCount}");
        //     transform.hasChanged = false;
        // }
        
        Move();
        Fire();
        Boom();
        Reload();
    }

    public void SetupProvider(IObjectProvider provider)
    {
        objectProvider = provider;
    }
    
    void Move()
    {
        float h = Utility.GetHorizontal();
        float v = Utility.GetVertical();
        // float h = joystick.Horizontal;
        // float v = joystick.Vertical;
        
        // Dead zone 처리
        float threshold = 0.5f;
        if (Mathf.Abs(h) < threshold) h = 0f;
        else h = Mathf.Sign(h);
        
        animator.SetInteger(GameDef.Hash.State, (int)h);
        
        if (Mathf.Abs(v) < threshold) v = 0f;
        else v = Mathf.Sign(v);
        
        if ((isTouchLeft && h >= 1f) || (isTouchRight && h <= -1f))
            h = 0;

        if ((isTouchTop && v >= 1f) || (isTouchBottom && v <= -1f))
            v = 0;

        Vector3 cusPos = transform.position;
        Vector3 nextPos = new Vector3(h, v, 0) * playerInfo.moveSpeed * Time.deltaTime;
        transform.position = cusPos + nextPos;
        // Vector3 dir = new Vector3(h, v, 0);
        // transform.Translate(dir * playerInfo.moveSpeed * Time.deltaTime);

    }

    public void ButtonADown() => isButtonA = true;
    public void ButtonAUp() => isButtonA = false;
    public void ButtonBDown() => isButtonB = true;

    void Fire(bool isAuto = false)
    {
        if (!isButtonA)
            return;
        
        if (curShotDelay < playerInfo.attackSpeed)
            return;

        switch (playerInfo.power)
        {
            case 1:
                GetBulletRigidBody((ObjectType.BulletPlayerA, transform.position));
                break;
            case 2:
                GetBulletRigidBody((ObjectType.BulletPlayerA, transform.position + Vector3.right * 0.1f), 
                    (ObjectType.BulletPlayerA, transform.position + Vector3.left * 0.1f));
                break;
            case 3:
            case 4:
            case 5:
            case 6:
                GetBulletRigidBody((ObjectType.BulletPlayerA, transform.position + Vector3.right * 0.25f), 
                    (ObjectType.BulletPlayerB, transform.position),
                    (ObjectType.BulletPlayerA, transform.position + Vector3.left * 0.25f));
                break;
        }
        
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

    void Boom()
    {
        if (!isButtonB)
            return;

        if (isBoomTime)
            return;

        if (playerInfo.boom == 0)
            return;
        
        playerInfo.boom--;
        isBoomTime = true;
        onBoomChanged();
        fxBoom.SetActive(true);
        Invoke(nameof(OffBoomEffect), 4f);
        Dictionary<ObjectType, Action<GameObject>> actionMap = new Dictionary<ObjectType, Action<GameObject>>()
        {
            { ObjectType.EnemyA, go =>
                {
                    // Debug.Log($"{go.name} hit {go.transform.position}!");
                    // Debug.Log($"{go.GetComponent<Enemy>()}");
                    go.GetComponent<Enemy>()?.OnHit(1000);
                }
            },
            { ObjectType.EnemyB, go => go.GetComponent<Enemy>()?.OnHit(1000) },
            { ObjectType.EnemyC, go => go.GetComponent<Enemy>()?.OnHit(1000) },
            { ObjectType.BulletEnemyA, go =>
                {
                    // Debug.Log(go);
                    go.SetActive(false);
                }
            },
            { ObjectType.BulletEnemyB, go => go.SetActive(false) }
        };
        foreach (var kvp in actionMap)
            ProcessByType(kvp.Key, kvp.Value);
    }

    private void ProcessByType(ObjectType tp, Action<GameObject> action)
    {
        // Debug.Log("ProcessByType: " + tp);
        foreach (var obj in objectProvider.GetPool(tp))
        {
            if (obj == null || !obj.activeInHierarchy)
                continue;

            // Debug.Log(obj);
            action?.Invoke(obj);
        }
    }
    
    public void TestFlight()
    {
        float moveSpeed = playerInfo.moveSpeed;
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
        Debug.Log(other.gameObject.name);
        if (!other.CompareTag("Border")) 
            return;
        Debug.Log(other.tag);
        if (other.gameObject.name == "Top") isTouchTop = true;
        else if (other.gameObject.name == "Bottom") isTouchBottom = true;
        else if (other.gameObject.name == "Right") isTouchRight = true;
        else if (other.gameObject.name == "Left") isTouchLeft = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        Debug.Log(other.gameObject.name);
        if (other.gameObject.CompareTag("Border"))
        {
            if (other.gameObject.name == "Top") isTouchTop = false;
            else if (other.gameObject.name == "Bottom") isTouchBottom = false;
            else if (other.gameObject.name == "Right") isTouchRight = false;
            else if (other.gameObject.name == "Left") isTouchLeft = false;
        }
        else if (other.gameObject.CompareTag("Enemy") || other.gameObject.CompareTag("EnemyBullet"))
        {
            if (isHit || isRespwanTime)
                return;
            
            isHit = true;
            
            playerInfo.life--;
            onPlayerLifeChanged(transform.position, playerInfo.objectType);
            gameObject.SetActive(false);
            other.gameObject.SetActive(false);
        }
        else if (other.gameObject.CompareTag("Item"))
        {
            Item item = other.gameObject.GetComponent<Item>();
            switch (item.type)
            {
                case ObjectType.ItemBoom:
                    if (playerInfo.boom == maxBoom)
                        playerInfo.score += 500;
                    else
                    {
                        playerInfo.boom++;
                        onBoomChanged();
                    }

                    break;
                case ObjectType.ItemCoin:
                    playerInfo.score += 1000;
                    break;
                case ObjectType.ItemPower:
                    if (playerInfo.power == maxPower)
                        playerInfo.score += 500;
                    else
                    {
                        playerInfo.power++;
                        AddFollower();
                    }

                    break;
            }
            other.gameObject.SetActive(false);
        }
    }

    void AddFollower()
    {
        if (playerInfo.power == 4)
            objectProvider.GetPool(ObjectType.Follow1)[0].SetActive(true);
        else if (playerInfo.power == 5)
            objectProvider.GetPool(ObjectType.Follow1)[1].SetActive(true);
        else if (playerInfo.power == 6)
            objectProvider.GetPool(ObjectType.Follow1)[2].SetActive(true);
    }

    void OffBoomEffect()
    {
        fxBoom.SetActive(false);
        isBoomTime = false;
    }
}