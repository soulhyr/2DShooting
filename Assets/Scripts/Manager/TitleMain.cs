using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class TitleMain : MonoBehaviour
{
    public TMP_Text blinkingText;
    public float blinkInterval = 0.5f;

    public float speed = 5f;
    public float resetPositionY = -10f;
    public float startPositionY = 10f;
    
    void Start()
    {
        StartCoroutine(Blink());
    }
    
    void Update()
    {
        if (Utility.IsClick())
            Utility.ChangeScene(GameDef.Scenes.Lobby);
    }
    
    IEnumerator Blink()
    {
        while (true)
        {
            blinkingText.enabled = !blinkingText.enabled;
            yield return new WaitForSeconds(blinkInterval);
        }
    }
}