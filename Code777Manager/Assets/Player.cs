using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum TileType :int
{  G1=0, Y2=1, K3=2, B4=3, R5=4, K5=5, P6=6, G6=7, Y7=8, P7=9, C7=10 }

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

    private int[] _pNumber = new int[11];   //0→綠1、1→黃2、2→黑3、3→棕4、4→紅5、5→黑5、6→粉紅6、7→綠6、8→黃7、9→粉紅7、10→藍7

 
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

    /// <summary>
    /// 玩家牌架上的TILE生成
    /// </summary>
    /// <param name="tile0">左TILE</param>
    /// <param name="tile1">中TILE</param>
    /// <param name="tile2">右TILE</param>
    public void newRack( Tile tile0, Tile tile1, Tile tile2 )
    {
        _rack = new Rack(tile0, tile1, tile2);
        _tileSprite[0].GetComponent<SpriteRenderer>().sprite = tile0.image;
        _tileSprite[1].GetComponent<SpriteRenderer>().sprite = tile1.image;
        _tileSprite[2].GetComponent<SpriteRenderer>().sprite = tile2.image;

        _pNumber[(int)TileType.G1] = 1; //重設綠1
        _pNumber[(int)TileType.Y2] = 2; //重設黃2
        _pNumber[(int)TileType.K3] = 3; //重設黑3
        _pNumber[(int)TileType.B4] = 4; //重設棕4
        _pNumber[(int)TileType.R5] = 4; //重設紅5
        _pNumber[(int)TileType.K5] = 1; //重設黑5
        _pNumber[(int)TileType.P6] = 3; //重設粉紅6
        _pNumber[(int)TileType.G6] = 3; //重設綠6
        _pNumber[(int)TileType.Y7] = 2; //重設黃7
        _pNumber[(int)TileType.P7] = 1; //重設粉紅7
        _pNumber[(int)TileType.C7] = 4; //重設藍7
    }

    /// <summary>
    /// 根據其他玩家牌架上的牌，逐漸削減自己牌架上的可能。 只要有玩家的牌架更新，在更新完牌架後，所有玩家就要執行一次。
    /// </summary>
    /// <param name="player"></param>
    public void rackCheck( List<Player> player )
    {
        //檢查除自己以外的四人的牌架上的牌
        int[] seeNumber = new int[11];
        for(int i=0; i< player.Count; i++)
        {
            if (i == _playerId)
                continue;

            //調查其他人牌架上的牌
            for(int j=0; j<3; j++)
            {
                switch ( player[i].rack.tiles[j].color + player[i].rack.tiles[j].number)
                {
                    default:
                        break;
                    case "G1":
                        seeNumber[(int)TileType.G1]++;
                        break;
                    case "Y2":
                        seeNumber[(int)TileType.Y2]++;
                        break;
                    case "K3":
                        seeNumber[(int)TileType.K3]++;
                        break;
                    case "B4":
                        seeNumber[(int)TileType.B4]++;
                        break;
                    case "R5":
                        seeNumber[(int)TileType.R5]++;
                        break;
                    case "K5":
                        seeNumber[(int)TileType.K5]++;
                        break;
                    case "P6":
                        seeNumber[(int)TileType.P6]++;
                        break;
                    case "G6":
                        seeNumber[(int)TileType.G6]++;
                        break;
                    case "Y7":
                        seeNumber[(int)TileType.Y7]++;
                        break;
                    case "P7":
                        seeNumber[(int)TileType.P7]++;
                        break;
                    case "C7":
                        seeNumber[(int)TileType.C7]++;
                        break;
                }
                
            }
        }

        //減少自己牌架可能數
        _pNumber[(int)TileType.G1] = ( 1 - seeNumber[(int)TileType.G1] < _pNumber[(int)TileType.G1] ) ? ( 1 - seeNumber[(int)TileType.G1] ) : _pNumber[(int)TileType.G1];
        _pNumber[(int)TileType.Y2] = (2 - seeNumber[(int)TileType.Y2] < _pNumber[(int)TileType.Y2]) ? (2 - seeNumber[(int)TileType.Y2]) : _pNumber[(int)TileType.Y2];
        _pNumber[(int)TileType.K3] = (3 - seeNumber[(int)TileType.K3] < _pNumber[(int)TileType.K3]) ? (3 - seeNumber[(int)TileType.K3]) : _pNumber[(int)TileType.K3];
        _pNumber[(int)TileType.B4] = (4 - seeNumber[(int)TileType.B4] < _pNumber[(int)TileType.B4]) ? (4 - seeNumber[(int)TileType.B4]) : _pNumber[(int)TileType.B4];
        _pNumber[(int)TileType.R5] = (4 - seeNumber[(int)TileType.R5] < _pNumber[(int)TileType.R5]) ? (4 - seeNumber[(int)TileType.R5]) : _pNumber[(int)TileType.R5];
        _pNumber[(int)TileType.K5] = (1 - seeNumber[(int)TileType.K5] < _pNumber[(int)TileType.K5]) ? (1 - seeNumber[(int)TileType.K5]) : _pNumber[(int)TileType.K5];
        _pNumber[(int)TileType.P6] = (3 - seeNumber[(int)TileType.P6] < _pNumber[(int)TileType.P6]) ? (3 - seeNumber[(int)TileType.P6]) : _pNumber[(int)TileType.P6];
        _pNumber[(int)TileType.G6] = (3 - seeNumber[(int)TileType.G6] < _pNumber[(int)TileType.G6]) ? (3 - seeNumber[(int)TileType.G6]) : _pNumber[(int)TileType.G6];
        _pNumber[(int)TileType.Y7] = (2 - seeNumber[(int)TileType.Y7] < _pNumber[(int)TileType.Y7]) ? (2 - seeNumber[(int)TileType.Y7]) : _pNumber[(int)TileType.Y7];
        _pNumber[(int)TileType.P7] = (1 - seeNumber[(int)TileType.P7] < _pNumber[(int)TileType.P7]) ? (1 - seeNumber[(int)TileType.P7]) : _pNumber[(int)TileType.P7];
        _pNumber[(int)TileType.C7] = (4 - seeNumber[(int)TileType.C7] < _pNumber[(int)TileType.C7]) ? (4 - seeNumber[(int)TileType.C7]) : _pNumber[(int)TileType.C7];

        /*
        _possibleNumber.G1 = ( 1 - seeNumber[(int)TileType.G1] < _possibleNumber.G1 ) ? ( 1 - seeNumber[(int)TileType.G1] ) : _possibleNumber.G1;
        _possibleNumber.Y2 = (2 - seeNumber[(int)TileType.Y2] < _possibleNumber.Y2) ? (2 - seeNumber[(int)TileType.Y2]) : _possibleNumber.Y2;
        _possibleNumber.K3 = (3 - seeNumber[(int)TileType.K3] < _possibleNumber.K3) ? (3 - seeNumber[(int)TileType.K3]) : _possibleNumber.K3;
        _possibleNumber.B4 = (4 - seeNumber[(int)TileType.B4] < _possibleNumber.B4) ? (4 - seeNumber[(int)TileType.B4]) : _possibleNumber.B4;
        _possibleNumber.R5 = (4 - seeNumber[(int)TileType.R5] < _possibleNumber.R5) ? (4 - seeNumber[(int)TileType.R5]) : _possibleNumber.R5;
        _possibleNumber.K5 = (1 - seeNumber[(int)TileType.K5] < _possibleNumber.K5) ? (1 - seeNumber[(int)TileType.K5]) : _possibleNumber.K5;
        _possibleNumber.P6 = (3 - seeNumber[(int)TileType.P6] < _possibleNumber.P6) ? (3 - seeNumber[(int)TileType.P6]) : _possibleNumber.P6;
        _possibleNumber.G6 = (3 - seeNumber[(int)TileType.G6] < _possibleNumber.G6) ? (3 - seeNumber[(int)TileType.G6]) : _possibleNumber.G6;
        _possibleNumber.Y7 = (2 - seeNumber[(int)TileType.Y7] < _possibleNumber.Y7) ? (2 - seeNumber[(int)TileType.Y7]) : _possibleNumber.Y7;
        _possibleNumber.P7 = (1 - seeNumber[(int)TileType.P7] < _possibleNumber.P7) ? (1 - seeNumber[(int)TileType.P7]) : _possibleNumber.P7;
        _possibleNumber.C7 = (4 - seeNumber[(int)TileType.C7] < _possibleNumber.C7) ? (4 - seeNumber[(int)TileType.C7]) : _possibleNumber.C7;
         * */
    }

    #endregion
}
