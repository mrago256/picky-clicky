using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    //set up references
    Rigidbody2D rb2d;
    SpriteRenderer sprite;

    GameManager gameManager;
    Animator animator;

    public static int lives;
    public static float money;

    public float speed;
    public bool facingRight = true;

    void Start()
    {
        //set gameManager to the GameManager object
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        animator = gameObject.GetComponent<Animator>();

        rb2d = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();

        //inisialize values
        money = 0;
        lives = 3;
    }

    void Update()
    {
        //update position of player
        if(Input.GetKey(KeyCode.W))
        {
            rb2d.position += new Vector2(0, speed * Time.deltaTime);
            animator.Play("PickyRun");
        }

        if (Input.GetKey(KeyCode.S))
        {
            rb2d.position -= new Vector2(0, speed * Time.deltaTime);
            animator.Play("PickyRun");
        }

        if (Input.GetKey(KeyCode.A))
        {
            if(facingRight)
            {
                Flip();
            }

            animator.Play("PickyRun");

            rb2d.position -= new Vector2(speed * Time.deltaTime, 0);
        }

        if (Input.GetKey(KeyCode.D))
        {
            if(!facingRight)
            {
                Flip();
            }

            animator.Play("PickyRun");

            rb2d.position += new Vector2(speed * Time.deltaTime, 0);
        }

        if(!Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D))
        {
            animator.Play("PickyIdle");
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene("MenuScene");
        }

        if (Input.GetKey(KeyCode.G))
        {
            if (money <= 0) return;
            money--;
        }

        if(Input.GetKey(KeyCode.H))
        {
            money++;
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("PlayerBlocked"))
        {
            //dim sprite when "behind" buildings
            var dimColor = sprite.color;
            dimColor.a = 0.4f;
            sprite.color = dimColor;
        }

        if(collision.gameObject.layer == 8)
        {
            gameManager.TriggerEntered(collision.gameObject);
        }

        if(collision.gameObject.name == "EnvelopeTrigger1")
        {
            if(GameManager.gameState == 1) GameManager.gameState++;
            gameManager.AdvanceStory();
        }

        if(collision.gameObject.name == "EnvelopeTrigger2")
        {
            if(GameManager.gameState <= 2) GameManager.gameState++;
            gameManager.AdvanceStory();
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.CompareTag("PlayerBlocked"))
        {
            //make sprite full clor value when leaving buildings
            var brightColor = sprite.color;
            brightColor.a = 1f;
            sprite.color = brightColor;
        }
    }

    void Flip()
    {
        facingRight = !facingRight;
        Vector3 objScale = transform.localScale;
        objScale.x *= -1;
        transform.localScale = objScale;
    }
}
