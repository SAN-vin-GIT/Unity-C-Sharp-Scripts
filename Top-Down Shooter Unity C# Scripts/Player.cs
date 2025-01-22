using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Player : MonoBehaviour
{

    public float speed;

    private Rigidbody2D rb;

    private Vector2 moveAmount;
    private Animator anim;

    public int health;

    public GameObject[] hearts;
    public Sprite fullHeart;
    public Sprite emptyHeart;

    public Animator hurtAnim;

    private SceneTransitions sceneTransitions;



    private void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        sceneTransitions = FindObjectOfType<SceneTransitions>();
    

    }

    private void Update()
    {


        Vector2 moveInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        moveAmount = moveInput.normalized * speed;
        if (moveInput != Vector2.zero)
        {
            anim.SetBool("isRunning", true);
        }
        else
        {
            anim.SetBool("isRunning", false);
        }
    }

    private void FixedUpdate()
    {
        rb.MovePosition(rb.position + moveAmount * Time.fixedDeltaTime);
    }

    public void TakeDamage(int amount)
    {
       
        health -= amount;
        UpdateHealthUI(health);
        hurtAnim.SetTrigger("hurt");
      
        if (health <= 0)
        {
            Destroy(this.gameObject);
            sceneTransitions.LoadScene("Lose");
        
        }
    }


    void UpdateHealthUI(int currentHealth)
    {

        for (int i = 0; i < hearts.Length; i++)
        {

            if (i < currentHealth)
            {
                hearts[i].GetComponent<Image>().sprite = fullHeart;
            }
            else
            {
                hearts[i].GetComponent<Image>().sprite = emptyHeart;
            }

        }

    }
    public void Heal(int healAmount)
    {
        if (health + healAmount > 7)
        {
            health = 7;
        }
        else
        {
            health += healAmount;
        }
        UpdateHealthUI(health);
    }

}
