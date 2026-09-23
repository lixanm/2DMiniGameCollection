using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Snake : MonoBehaviour
{
    private Vector2 direction = Vector2.right/*移动方向*/;
    private List<Transform> segments = new List<Transform>() /*蛇身*/;
    [SerializeField] private Transform segmentPrefab/*蛇身预制体*/;
    public int initialSize = 4/*初始长度*/;

    private void Start()
    {
        RemakeSnake();
    }

    private void Update()//每一帧改变移动方向
    {
        

        if (Input.GetKeyDown(KeyCode.W) && direction!=Vector2.down)
        {
            direction = Vector2.up;
        }else if (Input.GetKeyDown(KeyCode.S) && direction != Vector2.up)
        {
            direction = Vector2.down;
        }else if (Input.GetKeyDown(KeyCode.A) && direction != Vector2.right)
        {
            direction = Vector2.left;
        }else if (Input.GetKeyDown(KeyCode.D) && direction != Vector2.left)
        {
            direction = Vector2.right;
        }
    }
    private void FixedUpdate()
    {
        for (int i = segments.Count - 1; i > 0; i--)
        {
            segments[i].position = segments[i - 1].position;//蛇身跟随前一个蛇身
        }

        this.transform.position = new Vector3(
            Mathf.Round(this.transform.position.x) + direction.x,
            Mathf.Round(this.transform.position.y) + direction.y,
            0f
            );

    }

    private void RemakeSnake()
    {
        for (int i = 1; i < segments.Count; i++)//销毁蛇身
        {
            Destroy(segments[i].gameObject);
        }
        segments.Clear();//清空蛇身列表
        segments.Add(this.transform);//把蛇头加入蛇身列表
        for (int i = 1; i < initialSize; i++)//生成初始长度的蛇身
        {
            Grow();
        }
        this.transform.position = Vector3.zero;//重置蛇头位置
        direction = Vector2.right;//重置移动方向
    }

    private void Grow()//蛇身增长
    {
        Transform segment = Instantiate(this.segmentPrefab);//实例化蛇身
        segment.position = segments[segments.Count - 1].position;//生成蛇身的位置
        segments.Add(segment);//将蛇身加入列表
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Food")//蛇碰到食物
        {
            Grow();
        }
        if(collision.tag == "Barrier")
        {
            RemakeSnake();
        }
    }
}

