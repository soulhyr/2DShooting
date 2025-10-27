using UnityEngine;

[SerializeField]
public class PlayerInfo
{
    public ObjectType objectType;
    public string name;
    public int life;
    public float attackSpeed;
    public float moveSpeed;
    public int boom;
    public int power;
    public int damage;
    public int score;
    
    public PlayerInfo(PlayerData data)
    {
        objectType = data.objectType;
        name = data.name;
        life = data.health;
        attackSpeed = data.attackSpeed;
        moveSpeed = data.moveSpeed;
        damage = data.damage;
        score = 0;
        boom = 1;
        power = 1;
    }
}