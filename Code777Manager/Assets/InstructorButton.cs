using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InstructorButton : MonoBehaviour
{
    [Header("呼叫數字")]
    public bool numberCall;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GameStart()
    {
        //玩家叫用
        Debug.Log("遊戲開始");
        Code777Manager.nextTurn = true;
    }

    public void InstructorHit()
    {
        //玩家叫用
        Debug.Log(numberCall?"按鈕CALL已經按下":"按鈕PASS已經按下");
        Code777Manager.callTurn = true;
        Code777Manager.playerCall = numberCall ? true : false;
    }
}
