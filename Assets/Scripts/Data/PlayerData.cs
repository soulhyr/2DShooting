using System;

[Serializable]
public class PlayerData
{
    public ObjectType objectType;
    public string name;
    public string description;
    public float attackSpeed;
    public float moveSpeed;
    public int damage;
    public int health;

    public PlayerData Clone()
    {
        PlayerData copy = new PlayerData();
        copy.objectType = objectType;
        copy.name = name;
        copy.description = description;
        copy.attackSpeed = attackSpeed;
        copy.moveSpeed = moveSpeed;
        copy.damage = damage;
        copy.health = health;
        return copy;
    }
}