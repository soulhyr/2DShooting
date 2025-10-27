using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;

public class DataManager
{
    private static DataManager instance;
    private static bool isDataLoaded = false; 
    public static DataManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new DataManager();
                instance.InitializeData(); 
            }
            return instance;
        }
    }

    /// <summary>
    /// 플레이어 데이터
    /// </summary>
    public Dictionary<int, PlayerData> PlayerDataDict { get; private set; }
    /// <summary>
    /// 적 데이터
    /// </summary>
    public Dictionary<int, EnemyData> EnemyDataDict { get; private set; }
    /// <summary>
    /// 스테이지 데이터
    /// </summary>
    public List<StageData> StageDataList { get; private set; }
    
    private DataManager() { }

    /// <summary>
    /// 모든 데이터 파일을 로드하고 캐싱, 한 번만 실행되도록 보장.
    /// </summary>
    public void InitializeData()
    {
        if (isDataLoaded)
            return;
        Debug.Log("Loading Data");
        PlayerDataDict = LoadCharacterData();
        EnemyDataDict = LoadEnemyData();
        StageDataList = LoadStageData();
        
        Debug.Log($"DataManager: Loaded and caching all game data, PlayerData:{PlayerDataDict.Count}, EnemyDataDict:{EnemyDataDict.Count},  StageDataList:{StageDataList.Count}");
        
        isDataLoaded = true;
    }

    private List<T> LoadData<T>(string resourcePath, Action<T> postLoadAction = null) where T : class
    {
        string jsonText = string.Empty;
        if (File.Exists(resourcePath))
        {
            jsonText = File.ReadAllText(resourcePath);
            Debug.Log($"[DataManager] Load from file path: {resourcePath}");
        }
        else
        {
            TextAsset textAsset = Resources.Load<TextAsset>(resourcePath);
            if (textAsset == null)
            {
                Debug.LogError($"JSON 파일을 찾을 수 없습니다: {resourcePath}");
                return null;
            }

            jsonText = textAsset.text;
            Debug.Log($"[DataManager] Load from Resources: {resourcePath}");
        }

        List<T> dataList = JsonConvert.DeserializeObject<List<T>>(jsonText);
        Debug.Log("DataManager: Loaded " + dataList.Count);
        if (dataList == null)
            return null;

        if (postLoadAction != null)
        {
            foreach (var item in dataList)
                postLoadAction.Invoke(item);
        }

        return dataList;
    }

    private Dictionary<TKey, T> LoadData<T, TKey>(string resourcePath, Func<T, TKey> keySelector, Action<T> postLoadAction = null) where T : class
    {
        TextAsset textAsset = Resources.Load<TextAsset>(resourcePath);
        if (textAsset == null)
        {
            Debug.LogError($"JSON 파일을 찾을 수 없습니다: Resources/{resourcePath}");
            return null;
        }

        List<T> dataList = JsonConvert.DeserializeObject<List<T>>(textAsset.text);
        if (dataList == null)
            return null;

        if (postLoadAction != null)
        {
            foreach (var item in dataList)
                postLoadAction.Invoke(item);
        }
        
        return dataList.ToDictionary(keySelector, item => item);
    }
    
    private Dictionary<int, PlayerData> LoadCharacterData() => LoadData<PlayerData, int>("Data/character_data", data => (int)data.objectType);

    private Dictionary<int, EnemyData> LoadEnemyData() => LoadData<EnemyData, int>("Data/enemy_data", data => (int)data.objectType, data => data.LoadSpritesFromResources());

    private List<StageData> LoadStageData() => LoadData<StageData>($"Data/stage_data");
    
    public List<ScoreData> LoadScoreData() => LoadData<ScoreData>(GameDef.Paths.ScorePath);
    public List<SpawnData> LoadSpawnData(int stage) => LoadData<SpawnData>($"Data/{StageDataList.First(x => x.stage == stage).file}");
}