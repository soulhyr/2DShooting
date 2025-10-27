using System;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int damage;
    public int totalDamage;
    public bool isRotate;

    private void Update()
    {
        if (isRotate)
            transform.Rotate(Vector3.forward * 10);
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Debug.Log("Bullet collided with: " + other.gameObject.name + " Tag: " + other.tag);
        if (other.gameObject.CompareTag("BorderBullet"))
            gameObject.SetActive(false);
    }
    
    public void SetTotalDamage(int playerDamage)
    {
        totalDamage = damage + playerDamage;
    }
}