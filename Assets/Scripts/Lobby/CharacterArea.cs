using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterArea : MonoBehaviour
{
    public TMP_Text infoText;
    public float blinkInterval = 0.2f;
    public Action<GameObject> onSelected;
    private List<CharacterSlot> characterSlots = new List<CharacterSlot>();
    private GameObject fighter;
    private Coroutine blinkCoroutine;
    void Start()
    {
        LoadFighterData();
    }

    private void LoadFighterData()
    {
        CharacterData[] arr = DataManager.Instance.LoadCahracterData();
        foreach (CharacterData data in arr)
        {
            GameObject prefab = Resources.Load<GameObject>("Prefabs/CharacterSlotPrefab");
            if (prefab == null)
            {
                Debug.LogError($"프리팹 '{data.prefabName}' 을(를) 찾을 수 없습니다.");
                continue;
            }

            GameObject instance = Instantiate(prefab, transform);
            instance.name = data.spriteName;

            CharacterSlot characterSlot = instance.GetComponent<CharacterSlot>();
            characterSlot.SetData(data);
            characterSlot.SetNormal();
            characterSlots.Add(characterSlot);
            characterSlot.onSelected = () =>
            {
                if (DataManager.Instance.selectedCharacterData != null &&
                    DataManager.Instance.selectedCharacterData.prefabName == characterSlot.characterData.prefabName)
                {
                    StopAllCoroutines();
                    Utility.ChangeSceneAsync(GameDef.Scenes.Game, DataManager.Instance.selectedCharacterData);
                    return;
                }

                if (fighter != null)
                    Destroy(fighter);
                ResetAllSlots();
                characterSlot.image.color = Color.white;
                onSelected(instance);
                
                GameObject go = Resources.Load<GameObject>($"Prefabs/{data.prefabName}");
                Debug.Log($"go: {go.name}, prefabName: {data.prefabName}");
                fighter = Instantiate(go);
                FighterController ctr = fighter.GetComponent<FighterController>();
                ctr.TestFlight();
                if (blinkCoroutine != null)
                    StopCoroutine(blinkCoroutine);
                else
                    infoText.gameObject.SetActive(true);
                blinkCoroutine = StartCoroutine(Blink());
            };
            
            
        }
    }

    
    private void ResetAllSlots()
    {
        foreach (var slot in characterSlots)
            slot.SetNormal();
    }
    
    IEnumerator Blink()
    {
        while (true)
        {
            infoText.enabled = !infoText.enabled;
            yield return new WaitForSeconds(blinkInterval);
        }
    }
}