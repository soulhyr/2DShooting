using System.Collections;
using TMPro;
using UnityEngine;

public class TitleMain : MonoBehaviour
{
    public TMP_Text blinkingText;
    public float blinkInterval = 0.5f;

    public Transform backgroundTransform;
    public float speed = 5f;
    public float resetPositionY = -10f;
    public float startPositionY = 10f;
    
    void Start()
    {
        StartCoroutine(Blink());
    }
    
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
            Utility.ChangeScene(GameDef.Scenes.Lobby);
        
        backgroundTransform.transform.Translate(Vector3.down * speed * Time.deltaTime);
        if (backgroundTransform.transform.position.y <= resetPositionY)
        {
            Vector3 pos = backgroundTransform.transform.position;
            pos.y = startPositionY;
            backgroundTransform.transform.position = pos;
        }
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