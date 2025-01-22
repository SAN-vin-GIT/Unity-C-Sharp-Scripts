using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int damage;
    public int health;

    public Animator anim;

    private void Start()
    {
        // Initialize the Animator component
        anim = GetComponent<Animator>();
        if (anim == null)
        {
            Debug.LogError("Animator component not found on the GameObject.");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            // Trigger the attack animation
            if (anim != null)
            {
                anim.SetTrigger("Attack");
            }

            // Apply damage to the player
            collision.GetComponent<Player>().TakeDamage(damage);
        }
    }

    public void TakeDamage(int damage)
    {
        anim.SetTrigger("Hurt");
        health -= damage;
        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }
}
