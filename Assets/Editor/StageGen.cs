#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

public class StageEditorWindow : EditorWindow
{
    private int spawnCount = 44;
    private int pointMin = 0;
    private int pointMax = 5;

    private SpData lastSpawn = new SpData { delay = 4f, objectType = 150, point = 2 };

    private List<SpawnRule> spawnRules = new List<SpawnRule>()
    {
        new SpawnRule { type = 100, minDelay = 0.4f, maxDelay = 1f, isInteger = false },
        new SpawnRule { type = 101, minDelay = 0.8f, maxDelay = 2f, isInteger = false },
        new SpawnRule { type = 102, minDelay = 2f, maxDelay = 4f, isInteger = true }
    };

    private List<SpData> spawns = new List<SpData>();
    private Vector2 scrollPos;

    [MenuItem("Tools/Stage Editor")]
    public static void ShowWindow()
    {
        GetWindow<StageEditorWindow>("Stage Editor");
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField("Stage Random Spawn Settings", EditorStyles.boldLabel);

        spawnCount = EditorGUILayout.IntField("Total Spawn Count", spawnCount);

        EditorGUILayout.BeginHorizontal();
        pointMin = EditorGUILayout.IntField("Point Min", pointMin);
        pointMax = EditorGUILayout.IntField("Point Max", pointMax);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Spawn Rules", EditorStyles.boldLabel);

        for (int i = 0; i < spawnRules.Count; i++)
        {
            EditorGUILayout.BeginHorizontal("box");

            // 타입
            EditorGUILayout.LabelField("Type", GUILayout.Width(40));
            spawnRules[i].type = EditorGUILayout.IntField(spawnRules[i].type, GUILayout.Width(50));

            // Min Delay
            EditorGUILayout.LabelField("Min", GUILayout.Width(30));
            spawnRules[i].minDelay = EditorGUILayout.FloatField(spawnRules[i].minDelay, GUILayout.Width(50));

            // Max Delay
            EditorGUILayout.LabelField("Max", GUILayout.Width(30));
            spawnRules[i].maxDelay = EditorGUILayout.FloatField(spawnRules[i].maxDelay, GUILayout.Width(50));

            // Integer
            spawnRules[i].isInteger = EditorGUILayout.Toggle("Int", spawnRules[i].isInteger, GUILayout.Width(40));

            // Percent
            EditorGUILayout.LabelField("%", GUILayout.Width(15));
            spawnRules[i].percent = EditorGUILayout.FloatField(spawnRules[i].percent, GUILayout.Width(50));

            // Remove 버튼
            if (GUILayout.Button("Remove", GUILayout.Width(60)))
            {
                spawnRules.RemoveAt(i);
                break; // GUI 갱신
            }

            EditorGUILayout.EndHorizontal();
        }


        if (GUILayout.Button("Add New Rule"))
        {
            spawnRules.Add(new SpawnRule { type = 100, minDelay = 0.5f, maxDelay = 1f, isInteger = false });
        }

        EditorGUILayout.Space();
        if (GUILayout.Button("Generate Random Stage"))
        {
            GenerateRandomStage();
        }

        EditorGUILayout.Space();
        // Spawn Preview
        EditorGUILayout.LabelField("Spawn Preview", EditorStyles.boldLabel);
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Height(300));

        if (spawns != null)
        {
            for (int i = 0; i < spawns.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();

                // Delay
                EditorGUILayout.LabelField("Delay", GUILayout.Width(40));
                spawns[i].delay = EditorGUILayout.FloatField(spawns[i].delay, GUILayout.Width(50));

                // Type
                EditorGUILayout.LabelField("Type", GUILayout.Width(40));
                spawns[i].objectType = EditorGUILayout.IntField(spawns[i].objectType, GUILayout.Width(50));

                // Point
                EditorGUILayout.LabelField("Point", GUILayout.Width(40));
                spawns[i].point = EditorGUILayout.IntField(spawns[i].point, GUILayout.Width(50));

                EditorGUILayout.EndHorizontal();
            }
        }

        EditorGUILayout.EndScrollView();


        EditorGUILayout.Space();
        if (GUILayout.Button("Save JSON"))
        {
            SaveJson();
        }
    }

    private void GenerateRandomStage()
    {
        spawns.Clear();
        int count = Mathf.Max(0, spawnCount - 1); // 마지막 고정 제외

        for (int i = 0; i < count; i++)
        {
            var rule = GetRandomRuleByProbability(); // 확률 기반 선택
            int point = Random.Range(pointMin, pointMax + 1);
            float delay = rule.isInteger
                ? Random.Range((int)rule.minDelay, (int)rule.maxDelay + 1)
                : Mathf.Floor(Random.Range(rule.minDelay, rule.maxDelay) * 10f) / 10f;

            spawns.Add(new SpData { delay = delay, objectType = rule.type, point = point });
        }

        // 마지막 고정 스폰 추가
        spawns.Add(new SpData
        {
            delay = lastSpawn.delay,
            objectType = lastSpawn.objectType,
            point = lastSpawn.point
        });
    }
    
    private SpawnRule GetRandomRuleByProbability()
    {
        // 총합 계산
        float total = 0f;
        foreach (var rule in spawnRules) total += rule.percent;

        // 확률 비율 맞추기
        float rand = Random.Range(0f, total);
        float cumulative = 0f;

        foreach (var rule in spawnRules)
        {
            cumulative += rule.percent;
            if (rand <= cumulative)
                return rule;
        }

        // 혹시 안 걸리면 마지막 반환
        return spawnRules[spawnRules.Count - 1];
    }


    private void SaveJson()
    {
        // 저장 전 모든 delay를 소수점 한 자리 버림 처리
        for (int i = 0; i < spawns.Count; i++)
        {
            spawns[i].delay = Mathf.Floor(spawns[i].delay * 10f) / 10f;
        }

        // 기본 폴더 경로
        string defaultFolder = Path.Combine(Application.dataPath, "Resources", "Data");
        // 저장 다이얼로그 열기 (기본 폴더 지정)
        string path = EditorUtility.SaveFilePanel(
            "Save Stage JSON",   // 창 제목
            defaultFolder,       // 기본 열리는 폴더
            "stage.json",        // 기본 파일명
            "json"               // 확장자 필터
        );
        if (string.IsNullOrEmpty(path)) return;
        // Newtonsoft.Json 사용, 배열 그대로 저장
        string json = JsonConvert.SerializeObject(spawns, Formatting.Indented);
        // 선택한 경로에 파일 저장
        File.WriteAllText(path, json);
        Debug.Log($"✅ Stage JSON saved at {path}");
    }

    [System.Serializable]
    public class SpData
    {
        public float delay;
        public int objectType;
        public int point;
    }

    [System.Serializable]
    public class SpawnRule
    {
        public int type;
        public float minDelay;
        public float maxDelay;
        public bool isInteger;
        public float percent = 33f; // 기본 확률
    }

}
#endif