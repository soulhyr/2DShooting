using System;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int damage;
    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Bullet collided with: " + other.gameObject.name + " Tag: " + other.tag);
        if (other.gameObject.CompareTag("BorderBullet"))
            Destroy(gameObject);
    }
}