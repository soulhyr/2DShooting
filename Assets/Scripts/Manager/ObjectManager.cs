using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ObjectManager : MonoBehaviour
{
    #region # Prefab References 
    public GameObject bulletEnemyAPrefab;
    public GameObject bulletEnemyBPrefab;
    public GameObject bulletEnemyCPrefab;
    public GameObject bulletEnemyDPrefab;
    public GameObject bulletPlayerAPrefab;
    public GameObject bulletPlayerBPrefab;
    public GameObject bulletFollowPrefab;

    public GameObject enemyAPrefab;
    public GameObject enemyBPrefab;
    public GameObject enemyCPrefab;
    public GameObject bossAPrefab;

    public GameObject itemBoomPrefab;
    public GameObject itemCoinPrefab;
    public GameObject itemPowerPrefab;

    public GameObject followerPrefab;
    public GameObject explosionPrefab;
    
    public GameObject playerSlotPrefab;
    #endregion
    
    #region # Pools
    private GameObject[] arrBulletEnemyA;
    private GameObject[] arrBulletEnemyB;
    private GameObject[] arrBulletEnemyC;
    private GameObject[] arrBulletEnemyD;
    private GameObject[] arrBulletPlayerA;
    private GameObject[] arrBulletPlayerB;
    private GameObject[] arrBulletFollow;

    private GameObject[] arrEnemyA;
    private GameObject[] arrEnemyB;
    private GameObject[] arrEnemyC;
    private GameObject[] arrBossA;

    private GameObject[] arrItemBoom;
    private GameObject[] arrItemCoin;
    private GameObject[] arrItemPower;

    private GameObject[] arrFollower;
    private GameObject[] arrExplosion;

    private GameObject[] arrPlayerSlot;
    public readonly Dictionary<ObjectType, Sprite> dictPlayerSprites = new();
    private readonly Dictionary<ObjectType, GameObject> dictPlayers = new();

    private GameObject[] targetPool;
    #endregion
    
    void Awake()
    {
        MakePlayerBullet();
    }

    public void SetupPlayerSlot()
    {
        GeneratePlayersAndSlots();
    }
    
    public void SetupObjectPools(IObjectProvider provider, Action<ObjectType, int, Vector3> onEnemyDie)
    {
        Debug.Log("SetupObjectPools");
        MakeEnemy();
        InitEnemies(arrEnemyA, provider, onEnemyDie);
        InitEnemies(arrEnemyB, provider, onEnemyDie);
        InitEnemies(arrEnemyC, provider, onEnemyDie);
        InitEnemies(arrBossA, provider, onEnemyDie);
        
        GenerateLifeImage();
        
        MakeFollower();
        
        MakeExplosion();
    }
    
    private void InitEnemies(GameObject[] arr, IObjectProvider provider, Action<ObjectType, int, Vector3> onEnemyDie)
    {
        if (arr == null)
            return;

        foreach (var obj in arr)
        {
            if (obj == null)
                continue;
            Enemy enemy = obj.GetComponent<Enemy>();
            if (enemy == null) 
                continue;

            enemy.SetupProvider(provider);
            enemy.onEnemyDie -= onEnemyDie;
            enemy.onEnemyDie += onEnemyDie;
        }
    }

    private void InitFollowers(GameObject[] arr, IObjectProvider provider)
    {
        if (arr == null)
            return;

        foreach (var obj in arr)
        {
            if (obj == null)
                continue;
            Follower enemy = obj.GetComponent<Follower>();
            if (enemy == null)
                continue;

            enemy.SetupProvider(provider);
        }
    }

    #region # Create Pool
    private void MakeEnemy()
    {
        Generate(ref arrEnemyA, enemyAPrefab, 20);
        Generate(ref arrEnemyB, enemyBPrefab, 10);
        Generate(ref arrEnemyC, enemyCPrefab, 10);
        Generate(ref arrBossA, bossAPrefab, 1);
        
        Generate(ref arrBulletEnemyA, bulletEnemyAPrefab, 100);
        Generate(ref arrBulletEnemyB, bulletEnemyBPrefab, 100);
        Generate(ref arrBulletEnemyC, bulletEnemyCPrefab, 300);
        Generate(ref arrBulletEnemyD, bulletEnemyDPrefab, 100);
        
        Generate(ref arrItemBoom, itemBoomPrefab, 10);
        Generate(ref arrItemCoin, itemCoinPrefab, 20);
        Generate(ref arrItemPower, itemPowerPrefab, 10);
    }

    private void MakeFollower(int cntFollower = 3, int cntBullet = 100)
    {
        Generate(ref arrFollower, followerPrefab, cntFollower);
        
        Generate(ref arrBulletFollow, bulletFollowPrefab, cntBullet);
    }

    private void MakePlayerBullet(int count = 100)
    {
        Generate(ref arrBulletPlayerA, bulletPlayerAPrefab, count);
        Generate(ref arrBulletPlayerB, bulletPlayerBPrefab, count);
    }

    private void MakeExplosion(int count = 20)
    {
        Generate(ref arrExplosion, explosionPrefab, count);
    }

    private GameObject CreatInstance(GameObject prefab)
    {
        GameObject go = Instantiate(prefab);
        go.SetActive(false);
        return go;
    }

    private void Generate(ref GameObject[] arr, GameObject prefab, int count)
    {
        arr = new GameObject[count];
        for (int i = 0; i < arr.Length; i++)
            arr[i] = CreatInstance(prefab);
    }

    private void GenerateLifeImage()
    {
        var allCharacters = DataManager.Instance.PlayerDataDict;
        for (int i = 0; i < allCharacters.Count; i++)
        {
            PlayerData data = allCharacters[i];
            Sprite slotSprite = Resources.Load<Sprite>($"Arts/{data.objectType}");
            dictPlayerSprites[data.objectType] = slotSprite;
        }
    }

    private void GeneratePlayersAndSlots()
    {
        var allCharacters = DataManager.Instance.PlayerDataDict;
        arrPlayerSlot = new GameObject[allCharacters.Count];
        for (int i = 0; i < allCharacters.Count; i++)
        {
            PlayerData data = allCharacters[i];
            GameObject slot = CreatInstance(playerSlotPrefab);
            
            Sprite slotSprite = Resources.Load<Sprite>($"Arts/{data.objectType}");
            dictPlayerSprites[data.objectType] = slotSprite;
            
            var characterSlot = slot.GetComponent<CharacterSlot>();
            slot.name = data.name;
            characterSlot.SetData(data, slotSprite);
            arrPlayerSlot[i] = slot;

            GameObject playerPrefab = Resources.Load<GameObject>($"Prefabs/{data.objectType}");
            if (playerPrefab != null)
            {
                GameObject player = Instantiate(playerPrefab);
                player.SetActive(false);
                dictPlayers[data.objectType] = player;
            }
            else
            {
                Debug.LogWarning($"[ObjectManager] '{data.objectType}' 프리팹을 찾을 수 없습니다.");
            }
        }
    }
    #endregion
    
    #region # Provided method
    public GameObject MakeObject(ObjectType objectType, Vector3? position = null)
    {
        if (objectType == ObjectType.Player1 || 
            objectType == ObjectType.Player2 || 
            objectType == ObjectType.Player3)
        {
            dictPlayers.TryGetValue(objectType, out GameObject player);
            player.SetActive(true);
            if (position.HasValue)
            {
                Debug.Log($"change player position, position: {position.Value}");
                player.transform.position = position.Value;
            }

            return player;
        }
        
        targetPool = GetPool(objectType);
        
        if (targetPool == null)
            return null;

        foreach (var obj in targetPool)
        {
            if (obj != null && !obj.activeSelf)
            {
                obj.SetActive(true);
                if (position.HasValue)
                    obj.transform.position = position.Value;
                
                if (objectType == ObjectType.EnemyA || 
                    objectType == ObjectType.EnemyB || 
                    objectType == ObjectType.EnemyC ||
                    objectType == ObjectType.BossA)
                {
                    if (DataManager.Instance.EnemyDataDict.TryGetValue((int)objectType, out EnemyData data))
                    {
                        Enemy enemy = obj.GetComponent<Enemy>();
                        if (enemy != null)
                        {
                            enemy.enemyData = data;
                        }
                    }
                }
                
                return obj;
            }
        }

        return null;
    }

    public GameObject[] GetPool(ObjectType objectType)
    {
        targetPool = objectType switch
        {
            ObjectType.BulletEnemyA => arrBulletEnemyA,
            ObjectType.BulletEnemyB => arrBulletEnemyB,
            ObjectType.BulletEnemyC => arrBulletEnemyC,
            ObjectType.BulletEnemyD => arrBulletEnemyD,
            ObjectType.BulletPlayerA => arrBulletPlayerA,
            ObjectType.BulletPlayerB => arrBulletPlayerB,
            ObjectType.BulletFollow => arrBulletFollow,
            ObjectType.EnemyA => arrEnemyA,
            ObjectType.EnemyB => arrEnemyB,
            ObjectType.EnemyC => arrEnemyC,
            ObjectType.BossA => arrBossA,
            ObjectType.ItemBoom => arrItemBoom,
            ObjectType.ItemCoin => arrItemCoin,
            ObjectType.ItemPower => arrItemPower,
            ObjectType.PlayerSlot => arrPlayerSlot,
            ObjectType.FxExplosion => arrExplosion,
            _ => null
        };
        return targetPool;
    }
    
    #endregion
}