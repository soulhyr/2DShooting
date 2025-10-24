using System;

[Serializable]
public class CharacterData
{
    public int id;
    public string name;
    public string description;
    public string prefabName;
    public string spriteName;
    public float attackSpeed;
    public float moveSpeed;

    public CharacterData Clone()
    {
        CharacterData copy = new CharacterData();
        copy.id = id;
        copy.name = name;
        copy.description = description;
        copy.prefabName = prefabName;
        copy.spriteName = spriteName;
        copy.attackSpeed = attackSpeed;
        copy.moveSpeed = moveSpeed;
        return copy;
    }
}