using System;
using System.Collections.Generic;
using UnityEngine;

public class CharacterArea : MonoBehaviour
{
    public Action<CharacterSlot> onSelected;
    private GameObject[] characterSlots;
    private IObjectProvider objectProvider;

    public void Setup(IObjectProvider slot)
    {
        objectProvider = slot;
    }
    
    public void LoadAllCharacterSlot()
    {
        characterSlots = objectProvider.GetPool(ObjectType.PlayerSlot);
        
        foreach (GameObject go in characterSlots)
        {
            go.SetActive(true);
            go.transform.SetParent(transform);
            CharacterSlot slot = go.GetComponent<CharacterSlot>();
            slot.SetNormal();

            slot.onSelected = (cs) =>
            {
                ResetAllSlots();
                cs.SetSelected();
                onSelected(cs);
            };
        }
    }

    public void ResetAllSlots()
    {
        foreach (var slot in characterSlots)
            slot.GetComponent<CharacterSlot>().SetNormal();
    }
}