using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.VirtualTexturing.Debugging;

public class RobotPaddle : Paddle
{
    public Rigidbody2D ball;

    

    private void FixedUpdate()
    {
        if (this.ball.velocity.x  > 0)
        {
            if (ball.position.y > this.transform.position.y)
            {
                rb.AddForce(Vector2.up * speed);
            }
            else if(ball.position.y < this.transform.position.y)
            {
                rb.AddForce(Vector2.down * speed);
            }
        }
        else
        {
            if (0f > this.transform.position.y)
            {
                rb.AddForce(Vector2.up * speed);
            }
            else if (0f < this.transform.position.y)
            {
                rb.AddForce(Vector2.down * speed);
            }
        }
    }

    public void ResetPosition()
    {
        rb.position = new Vector2(rb.position.x, 0f);
    }

}
