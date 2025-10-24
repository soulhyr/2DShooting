using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class Utility
{
    public static void ChangeScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
    
    public static void ChangeSceneAsync(string sceneName, object param = null)
    {
        // var op = SceneManager.LoadSceneAsync(sceneName);
        // op.completed += (operation) =>
        // {
        //     Debug.Log($"scene: {sceneName}");
        //     var manager = GameObject.FindFirstObjectByType<MonoBehaviour>() as IMain;
        //     Debug.Log($"manager: {manager}");
        //     manager?.Init(param);
        //     Debug.Log($"manager end");
        // };
        
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.LoadSceneAsync(sceneName);

        void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;

            IMain manager = GameObject.FindObjectOfType<GameMain>() as IMain;
            if (manager != null)
            {
                manager.Init(param);
                Debug.Log("Init 호출 완료");
            }
            else
            {
                Debug.LogError("GameMain 오브젝트를 씬에서 찾을 수 없음");
            }
        }
    }

    public static string GetAcitvateScene()
    {
        return SceneManager.GetActiveScene().name;
    }

    public static string ConvertDateTimeToStirng(DateTime dateTime)
    {
        return dateTime.ToString(GameDef.Formats.DateTime);
    }

    public static string CommaString(int value, string tail = "")
    {
        return $"{value:N0} {tail}";
    }
}