using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Ball ball;

    [SerializeField] private PlayerPaddle playerPaddle;
    [SerializeField] private RobotPaddle robotPaddle;

    [SerializeField] private ScoringArea PlayerScoringArea;
    [SerializeField] private ScoringArea RobotScoringArea;

    [SerializeField] private int PlayerScore;
    [SerializeField] private int RobotScore;

    [SerializeField] private TextMeshProUGUI PlayerText;
    [SerializeField] private TextMeshProUGUI RobotText;

    //玩家得分
    public void PlayerGetScore()
    {
        PlayerScore++;
        PlayerText.text = PlayerScore.ToString();
        ResetStatus();
    }


    //机器人得分
    public void RobotGetScore()
    {
        RobotScore++;
        RobotText.text = RobotScore.ToString();
        ResetStatus();
    }

    //重置状态
    private void ResetStatus()
    {
        //小球位置重置，小球速度重置
        ball.ResetPosition();
        ball.ResetForce();

        //玩家和机器人位置重置
        playerPaddle.ResetPosition();
        robotPaddle.ResetPosition();
    }

}
