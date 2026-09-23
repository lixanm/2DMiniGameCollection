using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    
    private Rigidbody2D rb;
    [SerializeField] private float speed;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        ResetPosition();
        ResetForce();
    }

    private void AddFirstForce()
    {
        float x = Random.value < 0.5f ? -1f : 1f;
        float y = Random.value < 0.5f ? Random.Range(0.5f, 1f):
                                        Random.Range(-1f, -0.5f);
        Vector2 vector = new Vector2(x, y);
        vector = vector.normalized;
        rb.AddForce(vector * speed);
    }

    public void AddForce(Vector2 force)
    { 
        rb.AddForce(force); 
    }

    //重置球的位置
    public void ResetPosition()
    {
        rb.position = Vector2.zero;
    }

    //重置球的速度
    public void ResetForce()
    {
        rb.velocity = Vector3.zero;
        AddFirstForce();
    }
}
