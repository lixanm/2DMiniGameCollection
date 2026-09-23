using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class food : MonoBehaviour
{
    [SerializeField] private BoxCollider2D foodScope/*食物生成范围*/;

    private void Start()
    {
        RandomPosition();
    }
    private void RandomPosition()//随机位置
    {
        Bounds bounds = this.foodScope.bounds;
        float x = Mathf.Round(Random.Range(bounds.min.x, bounds.max.x));
        float y = Mathf.Round(Random.Range(bounds.min.y, bounds.max.y));
        this.transform.position = new Vector3(x, y, 0f);
    }

    private void OnTriggerEnter2D(Collider2D collision)//触发器碰撞检测
    {
        if (collision.tag == "Snake")//蛇碰到食物
        {
            RandomPosition();
        }
    }
}
