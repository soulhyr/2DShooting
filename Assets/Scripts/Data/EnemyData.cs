using System;
using System.Linq;
using UnityEngine;

[Serializable]
public class EnemyData
{
    public ObjectType objectType;
    public int point;
    public int health;
    public float moveSpeed;
    public float attackSpeed;
    public string[] spriteNames;
    
    [NonSerialized] 
    public Sprite[] sprites;
    
    public void LoadSpritesFromResources()
    {
        if (spriteNames == null || spriteNames.Length == 0) 
            return;
        
        Sprite[] allSpritesInTexture = Resources.LoadAll<Sprite>(GameDef.Paths.Enemies);
        if (allSpritesInTexture == null || allSpritesInTexture.Length == 0)
        {
            Debug.LogError($"Sprite Texture file not found or contains no sprites: {GameDef.Paths.Enemies}");
            return;
        }
        sprites = spriteNames
            .Select(name => allSpritesInTexture.FirstOrDefault(s => s.name.Equals(name, StringComparison.OrdinalIgnoreCase)))
            .Where(sprite => sprite != null) 
            .ToArray(); 

        if (sprites.Length < spriteNames.Length)
        {
            Debug.LogWarning($"EnemyData (ObjectType: {objectType}): 요청된 스프라이트 {spriteNames.Length - sprites.Length}개를 텍스처({GameDef.Paths.Enemies})에서 찾지 못했습니다. 이름 불일치 확인 필요.");
        }
    }
}