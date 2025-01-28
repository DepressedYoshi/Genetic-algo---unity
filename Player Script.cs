using System;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;
public class PlayerScript : MonoBehaviour
{
    public float speed = 5f;
    public float jumpForce = 10f;
    public float mutationRate = 0.12f;
    private bool jumping;
    private int[] DNA = new int[70];
    private int index = 0;
    public float buffer = 0.5f; // Time delay between actions
    private float timer = 0f;   // Timer to track time elapsed since the last action4
    private float score = 0f;
    private Rigidbody2D rb;
    private GameManager gameManager;

    void Start()
    {
        rb = GetComponent<Rigidbody2D> ();
        // getRandomColor();
        // createDNA();
    }

    void Update()
    {
        keyBoardInput();
        // act();
        updateScore();
    }

    public void SetGameManager(GameManager gameManager){
        this.gameManager = gameManager;
    }

    private void act(){

        // Increment the timer
        timer += Time.deltaTime;
        // If the timer exceeds the buffer and there are actions left in the DNA
        if (index < DNA.Length && timer >= buffer)
        {
            doShit(DNA[index]);
            index++;
            timer = 0f; 
        }
        else if (index >= DNA.Length){
            alertCompletion();
        }
    }

    private void updateScore()
    {
        this.score =  (float)(transform.position.x + 0.1 * transform.position.y);;

    }

    private void alertCompletion(){
        if(gameManager != null){
            gameManager.alertPlayerCompletion();
        }
    }
    private void createDNA()
    {
        for (int i = 0; i < DNA.Length; i++)
        {
            DNA[i] = UnityEngine.Random.Range(0, 4);
        }
    }
    public void createDNA(int[] mom, int[] dad)
    {
        for (int i = 0; i < DNA.Length; i++)
        {
            if(i < mom.Length/2){
                DNA[i] = mom[i];
            }else{
                DNA[i] = dad[i];
            }
        }
        mutateDNA();
    }

    private void mutateDNA()
    {
        for (int i = 0; i < DNA.Length; i++)
        {
            if (UnityEngine.Random.Range(0f, 1f) < mutationRate){
                DNA[i] = UnityEngine.Random.Range(0, 4);
            }
        }
    }

    public int[] getDNA(){
        return DNA; 
    }

    public float getScore(){
        return this.score;
    }

    private void doShit(int stuff)
    {
        switch (stuff)
        {
            case 0:
                jump();
                break;
            case 1:
                MoveLeft();
                break;
            case 2:
                MoveRight();
                break;
            case 3:
                stop();
                break;
            default:
                break;
        }
    }
    void jump()
    {
        if (!jumping)
        {
            jumping = true;
            rb.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
        }
    }
    private void MoveRight()
    {
        rb.linearVelocity = new Vector2(speed, rb.linearVelocity.y); // Set horizontal velocity
    }

    private void MoveLeft()
    {
        rb.linearVelocity = new Vector2(-speed, rb.linearVelocity.y);
    }

    private void stop()
    {
        rb.linearVelocity = new Vector2(0,rb.linearVelocity.y);

    }
    void getRandomColor()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        Color color = new Color(
            UnityEngine.Random.Range(0f, 0.75f),
            UnityEngine.Random.Range(0f, 0.95f),
            UnityEngine.Random.Range(0f, 0.85f),
            1f
        );

        spriteRenderer.color = color;
    }

    void keyBoardInput()
    {
        if (Input.GetKey(KeyCode.D))
        {
            MoveRight();
        }
        if (Input.GetKey(KeyCode.A))
        {
            MoveLeft();
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            jump();
        }
        if(Input.GetKey(KeyCode.S)){
            stop();
        }
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        jumping = false;
    }

    internal void Reset()
    {
     index = 0;
     score = 0f;
    }
}
