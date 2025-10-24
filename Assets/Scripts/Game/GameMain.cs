using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameMain : MonoBehaviour, IMain
{
    public GameObject[] enemies;
    public Transform[] spawnPoints;

    public float maxSpawnDelay;
    public float curSpawnDelay;

    private GameObject player;
    public TMP_Text scoreText;
    public Image[] lifeIcons;
    public GameObject gameOver;
    public Button btnRetry;
    public Button btnExit;

    private void Awake()
    {
        Debug.Log("Awake");
        btnRetry.onClick.AddListener(GameRetry);
        btnExit.onClick.AddListener(GemeExit);
        Debug.Log("Awake End");
    }
    
    public void Init(object obj)
    {
        Debug.Log("Init");
        Debug.Log($"GameMain init, obj: {obj}");
        CharacterData c = (CharacterData)obj;
        GameObject go = Resources.Load<GameObject>($"Prefabs/{c.prefabName}");
        player = Instantiate(go, new Vector3(0, -4f, 0), Quaternion.identity);
    }

    private void Update()
    {
        if (player == null)
            return;
        
        // Debug.Log("Update");
        curSpawnDelay += Time.deltaTime;
        
        if (curSpawnDelay > maxSpawnDelay)
        {
            SpawnEnemy();
            maxSpawnDelay = Random.Range(0.5f, 3f);
            curSpawnDelay = 0;
        }

        FighterController playerLogic = player.GetComponent<FighterController>();
        scoreText.text = Utility.CommaString(playerLogic.score);
    }

    void SpawnEnemy()
    {
        int ranEnemy = Random.Range(0, 3);
        int ranPoint = Random.Range(0, 9);
        GameObject enemy = Instantiate(enemies[ranEnemy], spawnPoints[ranPoint].position, spawnPoints[ranPoint].rotation);
        Rigidbody2D rigid = enemy.GetComponent<Rigidbody2D>();
        Enemy enemyLogic = enemy.GetComponent<Enemy>();
        enemyLogic.player = player;
        if (ranPoint == 5 || ranPoint == 6)
        {
            enemy.transform.Rotate(Vector3.back * 90);
            rigid.linearVelocity = new Vector2(enemyLogic.speed * -1, -1);
        }
        else if (ranPoint == 7 || ranPoint == 8)
        {
            enemy.transform.Rotate(Vector3.forward * 90);
            rigid.linearVelocity = new Vector2(enemyLogic.speed, -1);
        }
        else
            rigid.linearVelocity = new Vector2(0, enemyLogic.speed * -1);
    }

    public void RespawnPlayer()
    {
        Invoke(nameof(RespawPlayerExe), 2f);
    }
    void RespawPlayerExe()
    {
        player.transform.position = Vector3.down * 3.5f;
        player.SetActive(true);
    }

    public void UpdateLifeIcon(int life)
    {
        for (int i = 0; i < lifeIcons.Length; i++)
            lifeIcons[i].color = new Color(1, 1, 1, i <= life - 1 ? 1 : 0);
    }

    public void GameOver()
    {
        gameOver.SetActive(true);
    }

    private void GameRetry()
    {
        Utility.ChangeSceneAsync(GameDef.Scenes.Game, DataManager.Instance.selectedCharacterData);
    }

    private void GemeExit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}