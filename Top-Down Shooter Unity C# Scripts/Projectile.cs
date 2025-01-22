using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {

    public float speed;
    public float lifetime;

    public GameObject explosion;

    public GameObject soundObject;

    public int damage;

    private void Start()
    {
        Invoke("DestroyProjectile", lifetime);
        Instantiate(soundObject, transform.position, transform.rotation);
    }

    private void Update()
    {
        transform.Translate(Vector2.up * speed * Time.deltaTime);
    }

    void DestroyProjectile()
    {
        Instantiate(explosion, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Enemy")
        {
            collision.GetComponent<Enemy>().TakeDamage(damage);
            DestroyProjectile();
        }

        if (collision.tag == "boss")
        {
            collision.GetComponent<Boss>().TakeDamage(damage);
            DestroyProjectile();
        }

    }
}
