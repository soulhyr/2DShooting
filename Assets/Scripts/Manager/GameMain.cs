using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class GameMain : MonoBehaviour, IMain, IObjectProvider
{
    private int stage = 1;
    public Animator stageAnimator;
    public Animator clearAnimator;
    public Animator fadeAnimator;
    
    public Transform playerPos;
    
    private ObjectType[] enemies = { ObjectType.EnemyA, ObjectType.EnemyB, ObjectType.EnemyC, ObjectType.BossA };
    public Transform[] spawnPoints;

    public float nextSpawnDelay;
    public float curSpawnDelay;
    
    public Image[] lifeIcons;
    public Image[] boomIcons;
    public TMP_Text scoreText;
    public GameObject gameOver;
    public Button btnContinue;
    public Button btnEixt;
    public GameObject itemBoomPrefab;
    public GameObject itemPowerPrefab;
    public GameObject itemCoinPrefab;
    public GameObject fxBoom;

    public Button btnAttackA;
    public Button btnAttackB;
    
    public ObjectManager objectManager;
    private Player playerController;
    public int spawnIndex;
    public bool spawnEnd;

    private string stageName = string.Empty;
    private List<SpawnData> spawnDataList = new List<SpawnData>();
    
    public FixedJoystick joystick;
    
    public GameObject MakeObject(ObjectType type, Vector3? pos = null) => objectManager.MakeObject(type, pos);
    public GameObject[] GetPool(ObjectType type) => objectManager.GetPool(type);

    private void Awake()
    {
        btnContinue.onClick.AddListener(GameContinue);
        btnEixt.onClick.AddListener(GameExit);
        
        DataManager.Instance.InitializeData();
        objectManager.SetupObjectPools(this, EnemyDie);
        spawnDataList = DataManager.Instance.LoadSpawnData(stage);
        stageName = DataManager.Instance.StageDataList[stage - 1].name;
        StageStart();
    }

    public void StageStart()
    {
        stageAnimator.SetTrigger(GameDef.Hash.IsActive);
        
        stageAnimator.GetComponent<TMP_Text>().text = $"{stageName}\nStart";
        fadeAnimator.SetTrigger(GameDef.Hash.In);
    }

    public void StageEnd()
    {
        clearAnimator.SetTrigger(GameDef.Hash.IsActive);
        clearAnimator.GetComponent<TMP_Text>().text = $"{stageName}\nClear";
        fadeAnimator.SetTrigger(GameDef.Hash.Out);
        playerController.transform.position = playerPos.position;
        
        stage++;
        if (stage > 2)
            Invoke(nameof(GameOver), 6);
        else
            Invoke(nameof(StageStart), 3);
    }
    
    public void Init(object obj)
    {
        Debug.Log($"GameMain init, obj: {obj}");
        string prefabName = obj.ToString();
        GameObject go = Resources.Load<GameObject>($"Prefabs/{prefabName}");
        GameObject player = Instantiate(go, new Vector3(0, -4f, 0), Quaternion.identity);
        player.name = prefabName;
        playerController = player.GetComponent<Player>();
        playerController.joystick = joystick;
        playerController.fxBoom = fxBoom;
        playerController.SetupProvider(this);
        playerController.onPlayerLifeChanged += PlayerLifeChanged;
        playerController.onBoomChanged += PlayerBoomChanged;
        playerController.SetupPlay();
        BindJoystickButtons();

        foreach (Image img in lifeIcons)
            img.sprite = objectManager.dictPlayerSprites[playerController.playerInfo.objectType];
    }
    
    void BindEvent(Button btn, Action onDown, Action onUp = null)
    {
        EventTrigger trigger = btn.GetComponent<EventTrigger>() ?? btn.gameObject.AddComponent<EventTrigger>();

        if (onDown != null)
        {
            var entryDown = new EventTrigger.Entry { eventID = EventTriggerType.PointerDown };
            entryDown.callback.AddListener((data) => onDown.Invoke());
            trigger.triggers.Add(entryDown);
        }

        if (onUp != null)
        {
            var entryUp = new EventTrigger.Entry { eventID = EventTriggerType.PointerUp };
            entryUp.callback.AddListener((data) => onUp.Invoke());
            trigger.triggers.Add(entryUp);
        }
    }
    void BindJoystickButtons()
    {
        BindEvent(btnAttackA, playerController.ButtonADown, playerController.ButtonAUp);
        BindEvent(btnAttackB, playerController.ButtonBDown);
    }
    
    private void Start()
    {
        #if UNITY_EDITOR
        if (playerController == null)
        {
            Debug.LogWarning("playerController is null");
            Init(ObjectType.Player1);
        }
        #endif

        // objectManager.SetupObjectPools(this, EnemyDie);
    }

    private void Update()
    {
        if (playerController == null)
            return;
        
        // Debug.Log("Update");
        curSpawnDelay += Time.deltaTime;
        
        if (curSpawnDelay > nextSpawnDelay && !spawnEnd)
        {
            SpawnEnemy();
            curSpawnDelay = 0;
        }
        
        scoreText.text = Utility.CommaString(playerController.playerInfo.score);
    }
    
    void SpawnEnemy()
    {
        ObjectType enemyType = spawnDataList[spawnIndex].objectType;
        int enemyPoint = spawnDataList[spawnIndex].point;
        Debug.Log($"ranEnemy: {enemyType}, ranPoint: {enemyPoint}");
        GameObject go = objectManager.MakeObject(enemyType);
        go.transform.position = spawnPoints[enemyPoint].position;
        
        Rigidbody2D rigid = go.GetComponent<Rigidbody2D>();
        Enemy enemy = go.GetComponent<Enemy>();
        enemy.playerTransform = playerController.transform;
        if (enemyPoint == 5 || enemyPoint == 6)
        {
            go.transform.Rotate(Vector3.back * 90);
            rigid.linearVelocity = new Vector2(enemy.enemyData.moveSpeed * -1, -1);
        }
        else if (enemyPoint == 7 || enemyPoint == 8)
        {
            go.transform.Rotate(Vector3.forward * 90);
            rigid.linearVelocity = new Vector2(enemy.enemyData.moveSpeed, -1);
        }
        else
            rigid.linearVelocity = new Vector2(0, enemy.enemyData.moveSpeed * -1);
        
        spawnIndex++;
        if (spawnIndex >= spawnDataList.Count)
        {
            spawnEnd = true;
            return;
        }
        nextSpawnDelay = spawnDataList[spawnIndex].delay;
    }

    private void RespawnPlayer()
    {
        playerController.transform.position = Vector3.down * 3.5f;
        playerController.gameObject.SetActive(true);
        playerController.isHit = false;
    }

    private void PlayerLifeChanged(Vector3? pos, ObjectType? type)
    {
        if (pos.HasValue && type.HasValue)
            CallExplosion(pos.Value, type.Value);
        
        for (int i = 0; i < lifeIcons.Length; i++)
            lifeIcons[i].color = new Color(1, 1, 1, i <= playerController.playerInfo.life - 1 ? 1 : 0);
        
        if (playerController.playerInfo.life == 0)
            GameOver();
        else
            Invoke(nameof(RespawnPlayer), 2f);
    }

    private void PlayerBoomChanged()
    {
        for (int i = 0; i < boomIcons.Length; i++)
            boomIcons[i].color = new Color(1, 1, 1, i <= playerController.playerInfo.boom - 1 ? 1 : 0);
    }

    private void EnemyDie(ObjectType objectType, int point, Vector3 position)
    {
        playerController.playerInfo.score += point;
        int ran = objectType == ObjectType.BossA ? 0 : Random.Range(0, 10);
        if (ran < 5)
            Debug.Log("no item");
        else if (ran < 8)
            objectManager.MakeObject(ObjectType.ItemCoin, position);
        else if (ran < 9)
            objectManager.MakeObject(ObjectType.ItemPower, position);
        else if (ran < 10)
            objectManager.MakeObject(ObjectType.ItemBoom, position);
        CallExplosion(position, objectType);
        
        if (objectType == ObjectType.BossA)
            StageEnd();
    }

    public void CallExplosion(Vector3 position, ObjectType objectType)
    {
        GameObject go = objectManager.MakeObject(ObjectType.FxExplosion);
        Explosion explosion = go.GetComponent<Explosion>();
        go.transform.position = position;
        explosion.StartExplosion(objectType);
    }

    private void GameOver()
    {
        Time.timeScale = 0f;
        gameOver.SetActive(true);
        
        List<ScoreData> scoreList = new List<ScoreData>();
        ScoreData newScore = new ScoreData(playerController.playerInfo.score, playerController.playerInfo.name, stage);
        string path = GameDef.Paths.ScorePath;
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            scoreList = JsonConvert.DeserializeObject<List<ScoreData>>(json) ?? new List<ScoreData>();
        }
        scoreList.Add(newScore);

        string newJson = JsonConvert.SerializeObject(scoreList, Formatting.Indented);
        File.WriteAllText(path, newJson);
        Debug.Log(newJson);
    }

    private void GameContinue()
    {
        // 선택화면 이동
        // Utility.ChangeScene(GameDef.Scenes.Lobby);
        // 현재 화면 이어서하기!
        gameOver.SetActive(false);
        RespawnPlayer();
        playerController.playerInfo.boom = 1;
        playerController.playerInfo.life = 3;
        Time.timeScale = 1f;
    }
    
    private void GameExit()
    {
        Time.timeScale = 1f;
        Utility.ChangeScene(GameDef.Scenes.Rank);
// #if UNITY_EDITOR
//         UnityEditor.EditorApplication.isPlaying = false;
// #else
//         Application.Quit();
// #endif
    }
}