using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Player
{
    //玩家進行操作判斷並給出提示
    //玩家有名字、頭像、自己的牌架(但是自己不能檢視)、有勝利分
    //並且會根據其他玩家給出的情報，判斷自己牌架上的牌

    #region 欄位宣告

    private string _name;
    private Sprite _icon;
    private int _playerId = -1;
    private Rack _rack;
    private int _victory = 0;
    private Vector3[] _tilePosition = new Vector3[3];

    private GameObject[] _tileSprite = new GameObject[3];
    private GameObject[] _victoryLight = new GameObject[3];

    #endregion

    #region 屬性設定
    public string name { get { return _name; } }
    public Sprite icon { get { return _icon; } }
    public int playerId { get { return _playerId; } }
    public Rack rack { get { return _rack; } }
    public int victory { get { return _victory; } }
    public Vector3[] tilePosition { get { return _tilePosition; } }

    #endregion

    #region 建構方法
/// <summary>
/// 生成真實遊玩玩家及電腦
/// </summary>
/// <param name="name">玩家名稱</param>
/// <param name="icon">玩家頭像</param>
/// <param name="playerId">玩家順序(0為玩家、1~4為電腦)</param>
/// <param name="rack">牌架</param>
/// <param name="victory">分數</param>
/// <param name="tilePosition">牌架於場景上位置</param>
/// <param name="victoryLight">勝利燈在場景上名稱</param>
    public Player( string name, Sprite icon, int playerId, int victory, string[] tileSprite, string[] victoryLight)
    {
        _name = name;
        _icon = icon;
        _playerId = playerId;
        _victory = victory;
        _tilePosition = tilePosition;
        _tileSprite[0] = GameObject.Find(tileSprite[0]);
        _tileSprite[1] = GameObject.Find(tileSprite[1]);
        _tileSprite[2] = GameObject.Find(tileSprite[2]);
        _victoryLight[0] = GameObject.Find(victoryLight[0]);
        _victoryLight[1] = GameObject.Find(victoryLight[1]);
        _victoryLight[2] = GameObject.Find(victoryLight[2]);
    }

    #endregion

    #region 運算方法

    public void newRack( Tile tile0, Tile tile1, Tile tile2 )
    {
        _rack = new Rack(tile0, tile1, tile2);
        _tileSprite[0].GetComponent<SpriteRenderer>().sprite = tile0.image;
        _tileSprite[1].GetComponent<SpriteRenderer>().sprite = tile1.image;
        _tileSprite[2].GetComponent<SpriteRenderer>().sprite = tile2.image;
    }

    #endregion
}
