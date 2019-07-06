using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    private int _victory = 0;
    private Rack _rack;
    private bool _solution;
    private bool _callOk;

    private GameObject[] _tileSprite = new GameObject[3];
    private GameObject[] _victoryLight = new GameObject[3];
    private GameObject _answerBar = new GameObject();

    private TileInfer _possibleNumber;
    private List<string> _possibleSet;

    private Text _possibleCount;

    #endregion

    #region 屬性設定
    public string name { get { return _name; } }
    public Sprite icon { get { return _icon; } }
    public int playerId { get { return _playerId; } }
    public int victory { get { return _victory; } }
    public Rack rack { get { return _rack; } }
    public bool solution { get { return _solution; } set { _solution = value; } }
    public bool callOk { get { return _callOk; } set { _callOk = value; } }
    public TileInfer possibleNumber { get { return _possibleNumber; } }

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
    public Player(string name, Sprite icon, int playerId, int victory, Text playerCount)
    {
        _name = name;
        _icon = icon;
        _playerId = playerId;
        _victory = 0;
        _tileSprite[0] = GameObject.Find("Player" + playerId + "Tile0");
        _tileSprite[1] = GameObject.Find("Player" + playerId + "Tile1");
        _tileSprite[2] = GameObject.Find("Player" + playerId + "Tile2");
        _victoryLight[0] = GameObject.Find("Player" + playerId + "Victory0");
        _victoryLight[1] = GameObject.Find("Player" + playerId + "Victory1");
        _victoryLight[2] = GameObject.Find("Player" + playerId + "Victory2");
        _answerBar = GameObject.Find("Player" + playerId + "AnswerBar");

        _possibleCount = playerCount;
    }

    #endregion

    #region 運算方法

    /// <summary>
    /// 玩家牌架上的TILE生成
    /// </summary>
    /// <param name="tile0">左TILE</param>
    /// <param name="tile1">中TILE</param>
    /// <param name="tile2">右TILE</param>
    public void NewRack(Tile tile0, Tile tile1, Tile tile2)
    {
        _rack = new Rack(tile0, tile1, tile2);
        _tileSprite[0].GetComponent<Image>().sprite = tile0.image;
        _tileSprite[1].GetComponent<Image>().sprite = tile1.image;
        _tileSprite[2].GetComponent<Image>().sprite = tile2.image;


        _callOk = false;
        _possibleNumber = new TileInfer(true);
        PossibleSetReset();
    }

    /// <summary>
    /// 重置可能的組合數。這個方法只會在玩家自己腦中運行，即外面是看不到的
    /// </summary>
    private void PossibleSetReset()
    {
        int[] possibleTiles = new int[] { 1, 2, 3, 3, 3, 1, 3, 3, 2, 1, 3 };
        List<string> possibleSet = new List<string>();
        int[] gotTiles = new int[11];//已取Tile數，用以判斷是否已經取到上限使用
        int[] tile = new int[3];   //可能的組合Tile數

        for (int tile0 = 0; tile0 < 11; tile0++)
        {
            //1st tile
            if (gotTiles[tile0] >= possibleTiles[tile0]) continue;  //如果這張數字牌的已取Tile數已到達上限，要跳過這個數字牌

            tile[0] = tile0;
            gotTiles[tile0]++;

            for (int tile1 = tile0; tile1 < 11; tile1++)
            {
                //2nd tile
                if (gotTiles[tile1] >= possibleTiles[tile1]) continue;  //如果這張數字牌的已取Tile數已到達上限，要跳過這個數字牌

                tile[1] = tile1;
                gotTiles[tile1]++;

                for (int tile2 = tile1; tile2 < 11; tile2++)
                {
                    //3rd tile
                    if (gotTiles[tile2] >= possibleTiles[tile2]) continue;  //如果這張數字牌的已取Tile數已到達上限，要跳過這個數字牌

                    tile[2] = tile2;

                    //INSERT SetString
                    string setString = "";
                    for (int x = 0; x < 3; x++)
                    {
                        switch (tile[x])
                        {
                            case 0: setString = setString + "1G"; break;
                            case 1: setString = setString + "2Y"; break;
                            case 2: setString = setString + "3K"; break;
                            case 3: setString = setString + "4B"; break;
                            case 4: setString = setString + "5R"; break;
                            case 5: setString = setString + "5K"; break;
                            case 6: setString = setString + "6P"; break;
                            case 7: setString = setString + "6G"; break;
                            case 8: setString = setString + "7Y"; break;
                            case 9: setString = setString + "7P"; break;
                            case 10: setString = setString + "7C"; break;
                        }
                        if (x != 2)
                            setString = setString + "-";
                    }
                    possibleSet.Add(setString);
                }
                gotTiles[tile1]--;  //TILE用完記得歸還
            }
            gotTiles[tile0]--;  //TILE用完記得歸還
        }

        _possibleSet = possibleSet;
    }

    /// <summary>
    /// 根據其他玩家牌架上的牌，逐漸削減自己牌架上的可能。 只要有玩家的牌架更新，在更新完牌架後，所有玩家就要執行一次。
    /// </summary>
    /// <param name="player"></param>
    public void RackCheck( List<Player> player, List<Tile> discard )
    {
        int[] seeNumber = new int[11];

        //檢查除自己以外的四人的牌架上的牌
        for (int i=0; i< player.Count; i++)
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

        //檢查場中央棄牌堆的牌
        for (int i = 0; i < discard.Count; i++)
        {
            switch (discard[i].color + discard[i].number)
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

        PossibleSetSubRack();
    }

    /// <summary>
    /// 透過牌架的更新減少目前存在的可能數量。 牌架更新後所有玩家都要運行。邏輯同重置所有可能組合數，但是不同的是已經確定不對的組合不能放進去，才不會出現可能性增加的矛盾。
    /// 同樣這個方法只會在玩家自己腦中運行，即外面是看不到的
    /// </summary>
    private void PossibleSetSubRack()
    {
        //輸入的數字為possibleNumber取出的內容為準
        int[] possibleTiles = new int[] { _possibleNumber.G1, _possibleNumber.Y2, _possibleNumber.K3, _possibleNumber.B4, _possibleNumber.R5, _possibleNumber.K5,
                                                           _possibleNumber.P6, _possibleNumber.G6, _possibleNumber.Y7, _possibleNumber.P7, _possibleNumber.C7 };
        List<string> possibleSet = new List<string>();
        int[] gotTiles = new int[11];//已取Tile數，用以判斷是否已經取到上限使用
        int[] tile = new int[3];   //可能的組合Tile數

        for (int tile0 = 0; tile0 < 11; tile0++)
        {
            //1st tile
            if (gotTiles[tile0] >= possibleTiles[tile0]) continue;  //如果這張數字牌的已取Tile數已到達上限，要跳過這個數字牌

            tile[0] = tile0;
            gotTiles[tile0]++;

            for (int tile1 = tile0; tile1 < 11; tile1++)
            {
                //2nd tile
                if (gotTiles[tile1] >= possibleTiles[tile1]) continue;  //如果這張數字牌的已取Tile數已到達上限，要跳過這個數字牌

                tile[1] = tile1;
                gotTiles[tile1]++;

                for (int tile2 = tile1; tile2 < 11; tile2++)
                {
                    //3rd tile
                    if (gotTiles[tile2] >= possibleTiles[tile2]) continue;  //如果這張數字牌的已取Tile數已到達上限，要跳過這個數字牌

                    tile[2] = tile2;

                    //INSERT SetString
                    string setString = "";
                    for (int x = 0; x < 3; x++)
                    {
                        switch (tile[x])
                        {
                            case 0: setString = setString + "1G"; break;
                            case 1: setString = setString + "2Y"; break;
                            case 2: setString = setString + "3K"; break;
                            case 3: setString = setString + "4B"; break;
                            case 4: setString = setString + "5R"; break;
                            case 5: setString = setString + "5K"; break;
                            case 6: setString = setString + "6P"; break;
                            case 7: setString = setString + "6G"; break;
                            case 8: setString = setString + "7Y"; break;
                            case 9: setString = setString + "7P"; break;
                            case 10: setString = setString + "7C"; break;
                        }
                        if (x != 2)
                            setString = setString + "-";
                    }

                    if(_possibleSet.Contains(setString))    possibleSet.Add(setString); //只有在原本參考中的可能性裡存在的組合，才會被保留下來
                }
                gotTiles[tile1]--;  //TILE用完記得歸還
            }
            gotTiles[tile0]--;  //TILE用完記得歸還
        }

        _possibleSet = possibleSet;
        _possibleCount.text = _possibleSet.Count.ToString();
        _solution = _possibleSet.Count <= 1 ? true : false;
    }

    /// <summary>
    /// 對Player0適用的輔助模式
    /// </summary>
    /// <param name="assistMode">輔助模式是否開啟</param>
    public void TileLight(bool assistMode)
    {
        if(_playerId==0)
        {
            if (assistMode)
            {
                GameObject.Find("AssistTile1G").GetComponent<Image>().color = new Vector4(0.75F, 0.75F, 0.75F, 1F);
                GameObject.Find("AssistTile2Y_1").GetComponent<Image>().color = new Vector4(0.75F, 0.75F, 0.75F, 1F);
                GameObject.Find("AssistTile2Y_2").GetComponent<Image>().color = new Vector4(0.75F, 0.75F, 0.75F, 1F);
                GameObject.Find("AssistTile3K_1").GetComponent<Image>().color = new Vector4(0.75F, 0.75F, 0.75F, 1F);
                GameObject.Find("AssistTile3K_2").GetComponent<Image>().color = new Vector4(0.75F, 0.75F, 0.75F, 1F);
                GameObject.Find("AssistTile3K_3").GetComponent<Image>().color = new Vector4(0.75F, 0.75F, 0.75F, 1F);
                GameObject.Find("AssistTile4B_1").GetComponent<Image>().color = new Vector4(0.75F, 0.75F, 0.75F, 1F);
                GameObject.Find("AssistTile4B_2").GetComponent<Image>().color = new Vector4(0.75F, 0.75F, 0.75F, 1F);
                GameObject.Find("AssistTile4B_3").GetComponent<Image>().color = new Vector4(0.75F, 0.75F, 0.75F, 1F);
                GameObject.Find("AssistTile4B_4").GetComponent<Image>().color = new Vector4(0.75F, 0.75F, 0.75F, 1F);
                GameObject.Find("AssistTile5R_1").GetComponent<Image>().color = new Vector4(0.75F, 0.75F, 0.75F, 1F);
                GameObject.Find("AssistTile5R_2").GetComponent<Image>().color = new Vector4(0.75F, 0.75F, 0.75F, 1F);
                GameObject.Find("AssistTile5R_3").GetComponent<Image>().color = new Vector4(0.75F, 0.75F, 0.75F, 1F);
                GameObject.Find("AssistTile5R_4").GetComponent<Image>().color = new Vector4(0.75F, 0.75F, 0.75F, 1F);
                GameObject.Find("AssistTile5K").GetComponent<Image>().color = new Vector4(0.75F, 0.75F, 0.75F, 1F);
                GameObject.Find("AssistTile6P_1").GetComponent<Image>().color = new Vector4(0.75F, 0.75F, 0.75F, 1F);
                GameObject.Find("AssistTile6P_2").GetComponent<Image>().color = new Vector4(0.75F, 0.75F, 0.75F, 1F);
                GameObject.Find("AssistTile6P_3").GetComponent<Image>().color = new Vector4(0.75F, 0.75F, 0.75F, 1F);
                GameObject.Find("AssistTile6G_1").GetComponent<Image>().color = new Vector4(0.75F, 0.75F, 0.75F, 1F);
                GameObject.Find("AssistTile6G_2").GetComponent<Image>().color = new Vector4(0.75F, 0.75F, 0.75F, 1F);
                GameObject.Find("AssistTile6G_3").GetComponent<Image>().color = new Vector4(0.75F, 0.75F, 0.75F, 1F);
                GameObject.Find("AssistTile7Y_1").GetComponent<Image>().color = new Vector4(0.75F, 0.75F, 0.75F, 1F);
                GameObject.Find("AssistTile7Y_2").GetComponent<Image>().color = new Vector4(0.75F, 0.75F, 0.75F, 1F);
                GameObject.Find("AssistTile7P").GetComponent<Image>().color = new Vector4(0.75F, 0.75F, 0.75F, 1F);
                GameObject.Find("AssistTile7C_1").GetComponent<Image>().color = new Vector4(0.75F, 0.75F, 0.75F, 1F);
                GameObject.Find("AssistTile7C_2").GetComponent<Image>().color = new Vector4(0.75F, 0.75F, 0.75F, 1F);
                GameObject.Find("AssistTile7C_3").GetComponent<Image>().color = new Vector4(0.75F, 0.75F, 0.75F, 1F);

                if (_possibleNumber.G1 < 1) GameObject.Find("AssistTile1G").GetComponent<Image>().color = new Vector4(0.25F,0.25F,0.25F,1F);
                if (_possibleNumber.Y2 < 1) GameObject.Find("AssistTile2Y_1").GetComponent<Image>().color = new Vector4(0.25F, 0.25F, 0.25F, 1F);
                if (_possibleNumber.Y2 < 2) GameObject.Find("AssistTile2Y_2").GetComponent<Image>().color = new Vector4(0.25F, 0.25F, 0.25F, 1F);
                if (_possibleNumber.K3 < 1) GameObject.Find("AssistTile3K_1").GetComponent<Image>().color = new Vector4(0.25F, 0.25F, 0.25F, 1F);
                if (_possibleNumber.K3 < 2) GameObject.Find("AssistTile3K_2").GetComponent<Image>().color = new Vector4(0.25F, 0.25F, 0.25F, 1F);
                if (_possibleNumber.K3 < 3) GameObject.Find("AssistTile3K_3").GetComponent<Image>().color = new Vector4(0.25F, 0.25F, 0.25F, 1F);
                if (_possibleNumber.B4 < 1) GameObject.Find("AssistTile4B_1").GetComponent<Image>().color = new Vector4(0.25F, 0.25F, 0.25F, 1F);
                if (_possibleNumber.B4 < 2) GameObject.Find("AssistTile4B_2").GetComponent<Image>().color = new Vector4(0.25F, 0.25F, 0.25F, 1F);
                if (_possibleNumber.B4 < 3) GameObject.Find("AssistTile4B_3").GetComponent<Image>().color = new Vector4(0.25F, 0.25F, 0.25F, 1F);
                if (_possibleNumber.B4 < 4) GameObject.Find("AssistTile4B_4").GetComponent<Image>().color = new Vector4(0.25F, 0.25F, 0.25F, 1F);
                if (_possibleNumber.R5 < 1) GameObject.Find("AssistTile5R_1").GetComponent<Image>().color = new Vector4(0.25F, 0.25F, 0.25F, 1F);
                if (_possibleNumber.R5 < 2) GameObject.Find("AssistTile5R_2").GetComponent<Image>().color = new Vector4(0.25F, 0.25F, 0.25F, 1F);
                if (_possibleNumber.R5 < 3) GameObject.Find("AssistTile5R_3").GetComponent<Image>().color = new Vector4(0.25F, 0.25F, 0.25F, 1F);
                if (_possibleNumber.R5 < 4) GameObject.Find("AssistTile5R_4").GetComponent<Image>().color = new Vector4(0.25F, 0.25F, 0.25F, 1F);
                if (_possibleNumber.K5 < 1) GameObject.Find("AssistTile5K").GetComponent<Image>().color = new Vector4(0.25F, 0.25F, 0.25F, 1F);
                if (_possibleNumber.P6 < 1) GameObject.Find("AssistTile6P_1").GetComponent<Image>().color = new Vector4(0.25F, 0.25F, 0.25F, 1F);
                if (_possibleNumber.P6 < 2) GameObject.Find("AssistTile6P_2").GetComponent<Image>().color = new Vector4(0.25F, 0.25F, 0.25F, 1F);
                if (_possibleNumber.P6 < 3) GameObject.Find("AssistTile6P_3").GetComponent<Image>().color = new Vector4(0.25F, 0.25F, 0.25F, 1F);
                if (_possibleNumber.G6 < 1) GameObject.Find("AssistTile6G_1").GetComponent<Image>().color = new Vector4(0.25F, 0.25F, 0.25F, 1F);
                if (_possibleNumber.G6 < 2) GameObject.Find("AssistTile6G_2").GetComponent<Image>().color = new Vector4(0.25F, 0.25F, 0.25F, 1F);
                if (_possibleNumber.G6 < 3) GameObject.Find("AssistTile6G_3").GetComponent<Image>().color = new Vector4(0.25F, 0.25F, 0.25F, 1F);
                if (_possibleNumber.Y7 < 1) GameObject.Find("AssistTile7Y_1").GetComponent<Image>().color = new Vector4(0.25F, 0.25F, 0.25F, 1F);
                if (_possibleNumber.Y7 < 2) GameObject.Find("AssistTile7Y_2").GetComponent<Image>().color = new Vector4(0.25F, 0.25F, 0.25F, 1F);
                if (_possibleNumber.P7 < 1) GameObject.Find("AssistTile7P").GetComponent<Image>().color = new Vector4(0.25F, 0.25F, 0.25F, 1F);
                if (_possibleNumber.C7 < 1) GameObject.Find("AssistTile7C_1").GetComponent<Image>().color = new Vector4(0.25F, 0.25F, 0.25F, 1F);
                if (_possibleNumber.C7 < 2) GameObject.Find("AssistTile7C_2").GetComponent<Image>().color = new Vector4(0.25F, 0.25F, 0.25F, 1F);
                if (_possibleNumber.C7 < 3) GameObject.Find("AssistTile7C_3").GetComponent<Image>().color = new Vector4(0.25F, 0.25F, 0.25F, 1F);
                if (_possibleNumber.C7 < 4) GameObject.Find("AssistTile7C_4").GetComponent<Image>().color = new Vector4(0.25F, 0.25F, 0.25F, 1F);
            }
        }
    }

    /// <summary>
    /// 成為回答問題的玩家
    /// </summary>
    public void BecomeAnswerPlayer()
    {
        //GameObject.Find("Player" + _playerId + "Icon").GetComponent<Image>().color = new Color(1F, 0.75F, 0.75F, 1F);
        //_tileSprite[0].GetComponent<Image>().color = new Color(0.25F, 0.25F, 0.25F, 1F);
        //_tileSprite[1].GetComponent<Image>().color = new Color(0.25F, 0.25F, 0.25F, 1F);
        //_tileSprite[2].GetComponent<Image>().color = new Color(0.25F, 0.25F, 0.25F, 1F);

        GameObject.Find("Player" + _playerId + "Name").GetComponent<Text>().fontStyle = FontStyle.Bold;
        GameObject.Find("Player" + _playerId + "Name").GetComponent<Text>().color = new Color(1, 0.75F, 0, 1);
        _tileSprite[0].GetComponent<Image>().CrossFadeColor(new Color(0.25F, 0.25F, 0.25F, 1F), 0.2f, false, true);
        _tileSprite[1].GetComponent<Image>().CrossFadeColor(new Color(0.25F, 0.25F, 0.25F, 1F), 0.2f, false, true);
        _tileSprite[2].GetComponent<Image>().CrossFadeColor(new Color(0.25F, 0.25F, 0.25F, 1F), 0.2f, false, true);
    }

    /// <summary>
    /// 不再是回答問題的玩家
    /// </summary>
    public void NolongerAnswerPlayer()
    {
        //GameObject.Find("Player" + _playerId + "Icon").GetComponent<Image>().color = new Color(1F, 1F, 1F, 1F);
        //_tileSprite[0].GetComponent<Image>().color = new Color(1F, 1F, 1F, 1F);
        //_tileSprite[1].GetComponent<Image>().color = new Color(1F, 1F, 1F, 1F);
        //_tileSprite[2].GetComponent<Image>().color = new Color(1F, 1F, 1F, 1F);

        GameObject.Find("Player" + _playerId + "Name").GetComponent<Text>().fontStyle = FontStyle.Normal;
        GameObject.Find("Player" + _playerId + "Name").GetComponent<Text>().color = new Color(1, 1, 1, 1);
        _tileSprite[0].GetComponent<Image>().CrossFadeColor(new Color(1F, 1F, 1F, 1F), 0.2f, false, true);
        _tileSprite[1].GetComponent<Image>().CrossFadeColor(new Color(1F, 1F, 1F, 1F), 0.2f, false, true);
        _tileSprite[2].GetComponent<Image>().CrossFadeColor(new Color(1F, 1F, 1F, 1F), 0.2f, false, true);
    }

    /// <summary>
    /// 卡牌恢復明亮度
    /// </summary>
    public void TileRecover()
    {
        _tileSprite[0].GetComponent<Image>().CrossFadeColor(new Color(1F, 1F, 1F, 1F), 0.2f, false, true);
        _tileSprite[1].GetComponent<Image>().CrossFadeColor(new Color(1F, 1F, 1F, 1F), 0.2f, false, true);
        _tileSprite[2].GetComponent<Image>().CrossFadeColor(new Color(1F, 1F, 1F, 1F), 0.2f, false, true);
    }

    /// <summary>
    /// 成為呼叫數字的玩家
    /// </summary>
    public void BecomeCallPlayer()
    {
        //GameObject.Find("Player" + _playerId + "Icon").GetComponent<Image>().color = new Color(1F, 0.75F, 0.75F, 1F);
        //_tileSprite[0].GetComponent<Image>().color = new Color(0.25F, 0.25F, 0.25F, 1F);
        //_tileSprite[1].GetComponent<Image>().color = new Color(0.25F, 0.25F, 0.25F, 1F);
        //_tileSprite[2].GetComponent<Image>().color = new Color(0.25F, 0.25F, 0.25F, 1F);

        GameObject.Find("Player" + _playerId + "Icon").GetComponent<Image>().CrossFadeColor(new Color(1F, 0.5F, 0.5F, 1F), 0.2f, false, true);
    }

    /// <summary>
    /// 回復頭像顏色
    /// </summary>
    public void IconRecover()
    {
        GameObject.Find("Player" + _playerId + "Icon").GetComponent<Image>().CrossFadeColor(new Color(1F, 1F, 1F, 1F), 0.2f, false, true);
    }

    /// <summary>
    /// 根據玩家的回答，過濾牌架剩下的可能性
    /// </summary>
    /// <param name="answerPlayer">回答問題的玩家</param>
    /// <param name="questionId">回答的問題編號，為0~23</param>
    /// <param name="answerKey">回答的數量</param>
    /// <param name="compareKey1">回答選項1的數量較多</param>
    /// <param name="compareKey2">回答選項2的數量較多</param>
    public void AnswerFilter( int answerPlayer, List<Player> player, int questionId, int answerKey, bool compareKey1, bool compareKey2 )
    {
        //如果回答的人是自己，這個程序不執行
        if (answerPlayer == _playerId) return;

        //問題1~11的回答都是數字，所以只以answerKey參數判斷。 
        //問題12~23的回答都是「哪個較多」或「一樣多」，compareKey1表示先問出的內容較多傳入true，compareKey2表示後問出的內容較多傳入true，否則就兩個都傳入false

        List<Rack> checkRack = new List<Rack>(3);

        for ( int i=0; i<player.Count; i++)
        {
            if (i == _playerId || i == answerPlayer) continue;   //收集除自己、回答問題的玩家以外其他三個玩家的牌架資訊

            checkRack.Add(player[i].rack);
        }

        bool answerKeyCheck;   //檢查自己牌架問題的真值 問題1~問題7適用
        bool conditionCheck;  //複雜條件(或簡單條件也適用)下判斷自己是否達成與否
        int checkNumberKey; //檢查自己絕對數字總和 問題1、問題2適用
        int[] checkNumberColorKey = new int[7]; //統計顏色及數字用的陣列
        int compareTileKey1;
        int compareTileKey2;

        //輸入的數字為possibleNumber取出的內容為準
        int[] possibleTiles = new int[] { _possibleNumber.G1, _possibleNumber.Y2, _possibleNumber.K3, _possibleNumber.B4, _possibleNumber.R5, _possibleNumber.K5,
                                                           _possibleNumber.P6, _possibleNumber.G6, _possibleNumber.Y7, _possibleNumber.P7, _possibleNumber.C7 };
        List<string> possibleSet = new List<string>();
        int[] gotTiles = new int[11];//已取Tile數，用以判斷是否已經取到上限使用
        int[] tile = new int[3];   //可能的組合Tile數

        switch (questionId)
        {
            case 1: //". 有多少個牌架上的牌，三個數字總和是18以上？";
                #region 問題1的判斷
                //true：自己的rack總和在18以上；false：自己的rack總和在17以下
                answerKeyCheck = ((checkRack[0].reachEighteen ? 1 : 0) + (checkRack[1].reachEighteen ? 1 : 0) + (checkRack[2].reachEighteen ? 1 : 0)) < answerKey;
                for (int tile0 = 0; tile0 < 11; tile0++)
                {
                    //1st tile
                    if (gotTiles[tile0] >= possibleTiles[tile0]) continue;  //如果這張數字牌的已取Tile數已到達上限，要跳過這個數字牌

                    tile[0] = tile0;
                    gotTiles[tile0]++;

                    for (int tile1 = tile0; tile1 < 11; tile1++)
                    {
                        //2nd tile
                        if (gotTiles[tile1] >= possibleTiles[tile1]) continue;  //如果這張數字牌的已取Tile數已到達上限，要跳過這個數字牌

                        tile[1] = tile1;
                        gotTiles[tile1]++;

                        for (int tile2 = tile1; tile2 < 11; tile2++)
                        {
                            //3rd tile
                            if (gotTiles[tile2] >= possibleTiles[tile2]) continue;  //如果這張數字牌的已取Tile數已到達上限，要跳過這個數字牌

                            tile[2] = tile2;
                            gotTiles[tile2]++;

                            //INSERT SetString
                            string setString = "";
                            checkNumberKey = 0;

                            for (int x = 0; x < 3; x++)
                            {
                                switch (tile[x])
                                {
                                    case 0: setString = setString + "1G"; checkNumberKey += 1; break;
                                    case 1: setString = setString + "2Y"; checkNumberKey += 2; break;
                                    case 2: setString = setString + "3K"; checkNumberKey += 3; break;
                                    case 3: setString = setString + "4B"; checkNumberKey += 4; break;
                                    case 4: setString = setString + "5R"; checkNumberKey += 5; break;
                                    case 5: setString = setString + "5K"; checkNumberKey += 5; break;
                                    case 6: setString = setString + "6P"; checkNumberKey += 6; break;
                                    case 7: setString = setString + "6G"; checkNumberKey += 6; break;
                                    case 8: setString = setString + "7Y"; checkNumberKey += 7; break;
                                    case 9: setString = setString + "7P"; checkNumberKey += 7; break;
                                    case 10: setString = setString + "7C"; checkNumberKey += 7; break;
                                }
                                if (x != 2)
                                    setString = setString + "-";
                            }

                            conditionCheck = (checkNumberKey >= 18);
                            //answerKeyCheck為true表示自己總和在18以上，為false表示自己總和在17以下
                            //conditionCheck為true表示此組檢驗在18以上，為false表示此組檢驗總和在17以下
                            //兩者邏輯值要相同，並原本參考中的可能性裡存在的組合，才會被保留下來
                            if ((answerKeyCheck == conditionCheck) && _possibleSet.Contains(setString)) possibleSet.Add(setString);

                            gotTiles[tile2]--;  //TILE用完記得歸還
                        }
                        gotTiles[tile1]--;  //TILE用完記得歸還
                    }
                    gotTiles[tile0]--;  //TILE用完記得歸還
                }
                #endregion
                break;
            case 2: //". 有多少個牌架上的牌，三個數字總和是12以下？";
                #region 問題2的判斷
                answerKeyCheck = ((checkRack[0].untilTwelve ? 1 : 0) + (checkRack[1].untilTwelve ? 1 : 0) + (checkRack[2].untilTwelve ? 1 : 0)) < answerKey;
                for (int tile0 = 0; tile0 < 11; tile0++)
                {
                    //1st tile
                    if (gotTiles[tile0] >= possibleTiles[tile0]) continue;  //如果這張數字牌的已取Tile數已到達上限，要跳過這個數字牌

                    tile[0] = tile0;
                    gotTiles[tile0]++;

                    for (int tile1 = tile0; tile1 < 11; tile1++)
                    {
                        //2nd tile
                        if (gotTiles[tile1] >= possibleTiles[tile1]) continue;  //如果這張數字牌的已取Tile數已到達上限，要跳過這個數字牌

                        tile[1] = tile1;
                        gotTiles[tile1]++;

                        for (int tile2 = tile1; tile2 < 11; tile2++)
                        {
                            //3rd tile
                            if (gotTiles[tile2] >= possibleTiles[tile2]) continue;  //如果這張數字牌的已取Tile數已到達上限，要跳過這個數字牌

                            tile[2] = tile2;
                            gotTiles[tile2]++;

                            //INSERT SetString
                            string setString = "";
                            checkNumberKey = 0;

                            for (int x = 0; x < 3; x++)
                            {
                                switch (tile[x])
                                {
                                    case 0: setString = setString + "1G"; checkNumberKey += 1; break;
                                    case 1: setString = setString + "2Y"; checkNumberKey += 2; break;
                                    case 2: setString = setString + "3K"; checkNumberKey += 3; break;
                                    case 3: setString = setString + "4B"; checkNumberKey += 4; break;
                                    case 4: setString = setString + "5R"; checkNumberKey += 5; break;
                                    case 5: setString = setString + "5K"; checkNumberKey += 5; break;
                                    case 6: setString = setString + "6P"; checkNumberKey += 6; break;
                                    case 7: setString = setString + "6G"; checkNumberKey += 6; break;
                                    case 8: setString = setString + "7Y"; checkNumberKey += 7; break;
                                    case 9: setString = setString + "7P"; checkNumberKey += 7; break;
                                    case 10: setString = setString + "7C"; checkNumberKey += 7; break;
                                }
                                if (x != 2)
                                    setString = setString + "-";
                            }

                            conditionCheck = (checkNumberKey <= 12);
                            //conditionCheck為true表示此組檢驗在12以下，為false表示此組檢驗總和在13以
                            //answerKeyCheck和conditionCheck兩者邏輯值要相同，並原本參考中的可能性裡存在的組合，才會被保留下來
                            if ((answerKeyCheck == conditionCheck) && _possibleSet.Contains(setString)) possibleSet.Add(setString);

                            gotTiles[tile2]--;  //TILE用完記得歸還
                        }
                        gotTiles[tile1]--;  //TILE用完記得歸還
                    }
                    gotTiles[tile0]--;  //TILE用完記得歸還
                }
                #endregion
                break;
            case 3: //". 有多少個牌架上的牌，出現不同顏色的相同數字？";
                #region 問題3的判斷
                answerKeyCheck = ((checkRack[0].sameNumberDifColor ? 1 : 0) + (checkRack[1].sameNumberDifColor ? 1 : 0) + (checkRack[2].sameNumberDifColor ? 1 : 0)) < answerKey;
                for (int tile0 = 0; tile0 < 11; tile0++)
                {
                    //1st tile
                    if (gotTiles[tile0] >= possibleTiles[tile0]) continue;  //如果這張數字牌的已取Tile數已到達上限，要跳過這個數字牌

                    tile[0] = tile0;
                    gotTiles[tile0]++;

                    for (int tile1 = tile0; tile1 < 11; tile1++)
                    {
                        //2nd tile
                        if (gotTiles[tile1] >= possibleTiles[tile1]) continue;  //如果這張數字牌的已取Tile數已到達上限，要跳過這個數字牌

                        tile[1] = tile1;
                        gotTiles[tile1]++;

                        for (int tile2 = tile1; tile2 < 11; tile2++)
                        {
                            //3rd tile
                            if (gotTiles[tile2] >= possibleTiles[tile2]) continue;  //如果這張數字牌的已取Tile數已到達上限，要跳過這個數字牌

                            tile[2] = tile2;
                            gotTiles[tile2]++;

                            //INSERT SetString
                            string setString = "";

                            for (int x = 0; x < 3; x++)
                            {
                                switch (tile[x])
                                {
                                    case 0: setString = setString + "1G"; break;
                                    case 1: setString = setString + "2Y"; break;
                                    case 2: setString = setString + "3K"; break;
                                    case 3: setString = setString + "4B"; break;
                                    case 4: setString = setString + "5R"; break;
                                    case 5: setString = setString + "5K"; break;
                                    case 6: setString = setString + "6P"; break;
                                    case 7: setString = setString + "6G"; break;
                                    case 8: setString = setString + "7Y"; break;
                                    case 9: setString = setString + "7P"; break;
                                    case 10: setString = setString + "7C"; break;
                                }
                                if (x != 2)
                                    setString = setString + "-";
                            }
                            //5R和5K同時有1以上、或6P和6G同時有1以上、或7Y7P7C三者中兩者同時有1以上
                            //即 (5R>0且5K>0) 或 (6P>0且6G>0) 或 (7Y>0且7P>0) 或 (7Y>0且7C>0) 或 (7P>0且7C>0)

                            conditionCheck = (gotTiles[4]>=1&& gotTiles[5] >= 1)|| (gotTiles[6] >= 1 && gotTiles[7] >= 1)|| (gotTiles[8] >= 1 && gotTiles[9] >= 1)|| (gotTiles[8] >= 1 && gotTiles[10] >= 1)|| (gotTiles[9] >= 1 && gotTiles[10] >= 1);

                            //answerKeyCheck和conditionCheck兩者邏輯值要相同，並原本參考中的可能性裡存在的組合，才會被保留下來
                            if ((answerKeyCheck == conditionCheck) && _possibleSet.Contains(setString)) possibleSet.Add(setString);

                            gotTiles[tile2]--;  //TILE用完記得歸還
                        }
                        gotTiles[tile1]--;  //TILE用完記得歸還
                    }
                    gotTiles[tile0]--;  //TILE用完記得歸還
                }
                #endregion
                break;
            case 4: //". 有多少個牌架上的牌，出現三個不同顏色的數字？";
                #region 問題4的判斷
                answerKeyCheck = ((checkRack[0].threeColor ? 1 : 0) + (checkRack[1].threeColor ? 1 : 0) + (checkRack[2].threeColor ? 1 : 0)) < answerKey;
                for (int tile0 = 0; tile0 < 11; tile0++)
                {
                    //1st tile
                    if (gotTiles[tile0] >= possibleTiles[tile0]) continue;  //如果這張數字牌的已取Tile數已到達上限，要跳過這個數字牌

                    tile[0] = tile0;
                    gotTiles[tile0]++;

                    for (int tile1 = tile0; tile1 < 11; tile1++)
                    {
                        //2nd tile
                        if (gotTiles[tile1] >= possibleTiles[tile1]) continue;  //如果這張數字牌的已取Tile數已到達上限，要跳過這個數字牌

                        tile[1] = tile1;
                        gotTiles[tile1]++;

                        for (int tile2 = tile1; tile2 < 11; tile2++)
                        {
                            //3rd tile
                            if (gotTiles[tile2] >= possibleTiles[tile2]) continue;  //如果這張數字牌的已取Tile數已到達上限，要跳過這個數字牌

                            tile[2] = tile2;
                            gotTiles[tile2]++;

                            //INSERT SetString
                            string setString = "";

                            for (int x = 0; x < 3; x++)
                            {
                                switch (tile[x])
                                {
                                    case 0: setString = setString + "1G"; break;
                                    case 1: setString = setString + "2Y"; break;
                                    case 2: setString = setString + "3K"; break;
                                    case 3: setString = setString + "4B"; break;
                                    case 4: setString = setString + "5R"; break;
                                    case 5: setString = setString + "5K"; break;
                                    case 6: setString = setString + "6P"; break;
                                    case 7: setString = setString + "6G"; break;
                                    case 8: setString = setString + "7Y"; break;
                                    case 9: setString = setString + "7P"; break;
                                    case 10: setString = setString + "7C"; break;
                                }
                                if (x != 2)
                                    setString = setString + "-";
                            }

                            //1G+6G、2Y+7Y、3K+5K、4B、5R、6P+7P、7C 七種顏色每一種顏色都不超過一個(只要任意一種有兩個，就沒有三種顏色)
                            conditionCheck = (gotTiles[0]+gotTiles[7]<=1) && (gotTiles[1] + gotTiles[8] <= 1) && (gotTiles[2] + gotTiles[5] <= 1) && (gotTiles[3]<= 1) && (gotTiles[4]<= 1) && (gotTiles[6] + gotTiles[9] <= 1) && (gotTiles[10] <= 1);

                            //answerKeyCheck和conditionCheck兩者邏輯值要相同，並原本參考中的可能性裡存在的組合，才會被保留下來
                            if ((answerKeyCheck == conditionCheck) && _possibleSet.Contains(setString)) possibleSet.Add(setString);

                            gotTiles[tile2]--;  //TILE用完記得歸還
                        }
                        gotTiles[tile1]--;  //TILE用完記得歸還
                    }
                    gotTiles[tile0]--;  //TILE用完記得歸還
                }
                #endregion
                break;
            case 5: //". 有多少個牌架上的牌，三個數字皆是奇數或皆是偶數？";
                #region 問題5的判斷
                answerKeyCheck = ((checkRack[0].allOddEven ? 1 : 0) + (checkRack[1].allOddEven ? 1 : 0) + (checkRack[2].allOddEven ? 1 : 0)) < answerKey;
                for (int tile0 = 0; tile0 < 11; tile0++)
                {
                    //1st tile
                    if (gotTiles[tile0] >= possibleTiles[tile0]) continue;  //如果這張數字牌的已取Tile數已到達上限，要跳過這個數字牌

                    tile[0] = tile0;
                    gotTiles[tile0]++;

                    for (int tile1 = tile0; tile1 < 11; tile1++)
                    {
                        //2nd tile
                        if (gotTiles[tile1] >= possibleTiles[tile1]) continue;  //如果這張數字牌的已取Tile數已到達上限，要跳過這個數字牌

                        tile[1] = tile1;
                        gotTiles[tile1]++;

                        for (int tile2 = tile1; tile2 < 11; tile2++)
                        {
                            //3rd tile
                            if (gotTiles[tile2] >= possibleTiles[tile2]) continue;  //如果這張數字牌的已取Tile數已到達上限，要跳過這個數字牌

                            tile[2] = tile2;
                            gotTiles[tile2]++;

                            //INSERT SetString
                            string setString = "";

                            for (int x = 0; x < 3; x++)
                            {
                                switch (tile[x])
                                {
                                    case 0: setString = setString + "1G"; break;
                                    case 1: setString = setString + "2Y"; break;
                                    case 2: setString = setString + "3K"; break;
                                    case 3: setString = setString + "4B"; break;
                                    case 4: setString = setString + "5R"; break;
                                    case 5: setString = setString + "5K"; break;
                                    case 6: setString = setString + "6P"; break;
                                    case 7: setString = setString + "6G"; break;
                                    case 8: setString = setString + "7Y"; break;
                                    case 9: setString = setString + "7P"; break;
                                    case 10: setString = setString + "7C"; break;
                                }
                                if (x != 2)
                                    setString = setString + "-";
                            }

                            //1G+3K+5R+5K+7Y+7P+7C合計有3張，或者2Y+4B+6P+6G合計有3張
                            conditionCheck = (gotTiles[0] + gotTiles[2] + gotTiles[4] + gotTiles[5] + gotTiles[8] + gotTiles[9] + gotTiles[10] == 3) || (gotTiles[1] + gotTiles[3] + gotTiles[6] + gotTiles[7] == 3);

                            //answerKeyCheck和conditionCheck兩者邏輯值要相同，並原本參考中的可能性裡存在的組合，才會被保留下來
                            if ((answerKeyCheck == conditionCheck) && _possibleSet.Contains(setString)) possibleSet.Add(setString);

                            gotTiles[tile2]--;  //TILE用完記得歸還
                        }
                        gotTiles[tile1]--;  //TILE用完記得歸還
                    }
                    gotTiles[tile0]--;  //TILE用完記得歸還
                }
                #endregion
                break;
            case 6: //". 有多少個牌架上的牌，出現相同顏色的相同數字？";
                #region 問題6的判斷
                answerKeyCheck = ((checkRack[0].sameColorNumber ? 1 : 0) + (checkRack[1].sameColorNumber ? 1 : 0) + (checkRack[2].sameColorNumber ? 1 : 0)) < answerKey;
                for (int tile0 = 0; tile0 < 11; tile0++)
                {
                    //1st tile
                    if (gotTiles[tile0] >= possibleTiles[tile0]) continue;  //如果這張數字牌的已取Tile數已到達上限，要跳過這個數字牌

                    tile[0] = tile0;
                    gotTiles[tile0]++;

                    for (int tile1 = tile0; tile1 < 11; tile1++)
                    {
                        //2nd tile
                        if (gotTiles[tile1] >= possibleTiles[tile1]) continue;  //如果這張數字牌的已取Tile數已到達上限，要跳過這個數字牌

                        tile[1] = tile1;
                        gotTiles[tile1]++;

                        for (int tile2 = tile1; tile2 < 11; tile2++)
                        {
                            //3rd tile
                            if (gotTiles[tile2] >= possibleTiles[tile2]) continue;  //如果這張數字牌的已取Tile數已到達上限，要跳過這個數字牌

                            tile[2] = tile2;
                            gotTiles[tile2]++;

                            //INSERT SetString
                            string setString = "";

                            for (int x = 0; x < 3; x++)
                            {
                                switch (tile[x])
                                {
                                    case 0: setString = setString + "1G"; break;
                                    case 1: setString = setString + "2Y"; break;
                                    case 2: setString = setString + "3K"; break;
                                    case 3: setString = setString + "4B"; break;
                                    case 4: setString = setString + "5R"; break;
                                    case 5: setString = setString + "5K"; break;
                                    case 6: setString = setString + "6P"; break;
                                    case 7: setString = setString + "6G"; break;
                                    case 8: setString = setString + "7Y"; break;
                                    case 9: setString = setString + "7P"; break;
                                    case 10: setString = setString + "7C"; break;
                                }
                                if (x != 2)
                                    setString = setString + "-";
                            }

                            //2Y、3K、4B、5R、6P、6G、7Y、7C任何一種在2張以上
                            conditionCheck = (gotTiles[1] >= 2) || (gotTiles[2] >= 2) || (gotTiles[3] >= 2) || (gotTiles[4] >= 2) || (gotTiles[6] >= 2) || (gotTiles[7] >= 2) || (gotTiles[8] >= 2) || (gotTiles[10] >= 2);

                            //answerKeyCheck和conditionCheck兩者邏輯值要相同，並原本參考中的可能性裡存在的組合，才會被保留下來
                            if ((answerKeyCheck == conditionCheck) && _possibleSet.Contains(setString)) possibleSet.Add(setString);

                            gotTiles[tile2]--;  //TILE用完記得歸還
                        }
                        gotTiles[tile1]--;  //TILE用完記得歸還
                    }
                    gotTiles[tile0]--;  //TILE用完記得歸還
                }
                #endregion
                break;
            case 7: //". 有多少個牌架上的牌，三個數字是連續的數字？";
                #region 問題7的判斷
                answerKeyCheck = ((checkRack[0].consecutiveNumber ? 1 : 0) + (checkRack[1].consecutiveNumber ? 1 : 0) + (checkRack[2].consecutiveNumber ? 1 : 0)) < answerKey;
                for (int tile0 = 0; tile0 < 11; tile0++)
                {
                    //1st tile
                    if (gotTiles[tile0] >= possibleTiles[tile0]) continue;  //如果這張數字牌的已取Tile數已到達上限，要跳過這個數字牌

                    tile[0] = tile0;
                    gotTiles[tile0]++;

                    for (int tile1 = tile0; tile1 < 11; tile1++)
                    {
                        //2nd tile
                        if (gotTiles[tile1] >= possibleTiles[tile1]) continue;  //如果這張數字牌的已取Tile數已到達上限，要跳過這個數字牌

                        tile[1] = tile1;
                        gotTiles[tile1]++;

                        for (int tile2 = tile1; tile2 < 11; tile2++)
                        {
                            //3rd tile
                            if (gotTiles[tile2] >= possibleTiles[tile2]) continue;  //如果這張數字牌的已取Tile數已到達上限，要跳過這個數字牌

                            tile[2] = tile2;
                            gotTiles[tile2]++;

                            //INSERT SetString
                            string setString = "";

                            for (int x = 0; x < 3; x++)
                            {
                                switch (tile[x])
                                {
                                    case 0: setString = setString + "1G"; break;
                                    case 1: setString = setString + "2Y"; break;
                                    case 2: setString = setString + "3K"; break;
                                    case 3: setString = setString + "4B"; break;
                                    case 4: setString = setString + "5R"; break;
                                    case 5: setString = setString + "5K"; break;
                                    case 6: setString = setString + "6P"; break;
                                    case 7: setString = setString + "6G"; break;
                                    case 8: setString = setString + "7Y"; break;
                                    case 9: setString = setString + "7P"; break;
                                    case 10: setString = setString + "7C"; break;
                                }
                                if (x != 2)
                                    setString = setString + "-";
                            }

                            //這判斷有點複雜……
                            //(1G=1、2Y=1、3K=1) 或 (2Y=1、3K=1、4B=1) 或 (3K=1、4B=1、5R+5K=1) 或 ( 5R+5K=1、6P+6G=1、7Y+7P+7C=1 )
                            conditionCheck = (gotTiles[0] == 1 && gotTiles[1] == 1 && gotTiles[2] == 1) ||
                                                        (gotTiles[1] == 1 && gotTiles[2] == 1 && gotTiles[3] == 1) ||
                                                        (gotTiles[2] == 1 && gotTiles[3] == 1 && gotTiles[4] + gotTiles[5] == 1) ||
                                                        (gotTiles[3] == 1 && gotTiles[4] + gotTiles[5] == 1 && gotTiles[6] + gotTiles[7] == 1) ||
                                                        (gotTiles[4] + gotTiles[5] == 1 && gotTiles[6] + gotTiles[7] == 1 && gotTiles[8] + gotTiles[9] + gotTiles[10] == 1);


                            //answerKeyCheck和conditionCheck兩者邏輯值要相同，並原本參考中的可能性裡存在的組合，才會被保留下來
                            if ((answerKeyCheck == conditionCheck) && _possibleSet.Contains(setString)) possibleSet.Add(setString);

                            gotTiles[tile2]--;  //TILE用完記得歸還
                        }
                        gotTiles[tile1]--;  //TILE用完記得歸還
                    }
                    gotTiles[tile0]--;  //TILE用完記得歸還
                }
                #endregion
                break;
            case 8: //". 你看到多少種顏色的數字牌？";
                #region 問題8的判斷
                for(int playerIndex=0; playerIndex<3; playerIndex++)
                {
                    for(int tileIndex=0; tileIndex<3; tileIndex++)
                    {
                        switch (checkRack[playerIndex].tiles[tileIndex].color)
                        {
                            case "G": checkNumberColorKey[0]++; break;
                            case "Y": checkNumberColorKey[1]++; break;
                            case "K": checkNumberColorKey[2]++; break;
                            case "B": checkNumberColorKey[3]++; break;
                            case "R": checkNumberColorKey[4]++; break;
                            case "P": checkNumberColorKey[5]++; break;
                            case "C": checkNumberColorKey[6]++; break;
                        }
                    }
                }

                for (int tile0 = 0; tile0 < 11; tile0++)
                {
                    //1st tile
                    if (gotTiles[tile0] >= possibleTiles[tile0]) continue;  //如果這張數字牌的已取Tile數已到達上限，要跳過這個數字牌

                    tile[0] = tile0;
                    gotTiles[tile0]++;

                    for (int tile1 = tile0; tile1 < 11; tile1++)
                    {
                        //2nd tile
                        if (gotTiles[tile1] >= possibleTiles[tile1]) continue;  //如果這張數字牌的已取Tile數已到達上限，要跳過這個數字牌

                        tile[1] = tile1;
                        gotTiles[tile1]++;

                        for (int tile2 = tile1; tile2 < 11; tile2++)
                        {
                            //3rd tile
                            if (gotTiles[tile2] >= possibleTiles[tile2]) continue;  //如果這張數字牌的已取Tile數已到達上限，要跳過這個數字牌

                            tile[2] = tile2;
                            gotTiles[tile2]++;

                            //INSERT SetString
                            string setString = "";

                            for (int x = 0; x < 3; x++)
                            {
                                switch (tile[x])
                                {
                                    case 0: setString = setString + "1G"; break;
                                    case 1: setString = setString + "2Y"; break;
                                    case 2: setString = setString + "3K"; break;
                                    case 3: setString = setString + "4B"; break;
                                    case 4: setString = setString + "5R"; break;
                                    case 5: setString = setString + "5K"; break;
                                    case 6: setString = setString + "6P"; break;
                                    case 7: setString = setString + "6G"; break;
                                    case 8: setString = setString + "7Y"; break;
                                    case 9: setString = setString + "7P"; break;
                                    case 10: setString = setString + "7C"; break;
                                }
                                if (x != 2)
                                    setString = setString + "-";
                            }

                            conditionCheck = (( ( checkNumberColorKey[0]+ gotTiles[0] + gotTiles[7] >0) ? 1 : 0 ) +
                                                            ((checkNumberColorKey[1] + gotTiles[1] + gotTiles[8] > 0) ? 1 : 0) +
                                                            ((checkNumberColorKey[2] + gotTiles[2] + gotTiles[5] > 0) ? 1 : 0) +
                                                            ((checkNumberColorKey[3] + gotTiles[3] > 0) ? 1 : 0) +
                                                            ((checkNumberColorKey[4] + gotTiles[4] > 0) ? 1 : 0) +
                                                            ((checkNumberColorKey[5] + gotTiles[6] + gotTiles[9] > 0) ? 1 : 0) +
                                                            ((checkNumberColorKey[6] + gotTiles[10] > 0) ? 1 : 0) ) == answerKey;

                            //conditionCheck需要為True，並原本參考中的可能性裡存在的組合，才會被保留下來
                            if (conditionCheck && _possibleSet.Contains(setString)) possibleSet.Add(setString);

                            gotTiles[tile2]--;  //TILE用完記得歸還
                        }
                        gotTiles[tile1]--;  //TILE用完記得歸還
                    }
                    gotTiles[tile0]--;  //TILE用完記得歸還
                }
                #endregion
                break;
            case 9: //". 有幾種顏色出現了三次以上？";
                #region 問題9的判斷
                for (int playerIndex = 0; playerIndex < 3; playerIndex++)
                {
                    for (int tileIndex = 0; tileIndex < 3; tileIndex++)
                    {
                        switch (checkRack[playerIndex].tiles[tileIndex].color)
                        {
                            case "G": checkNumberColorKey[0]++; break;
                            case "Y": checkNumberColorKey[1]++; break;
                            case "K": checkNumberColorKey[2]++; break;
                            case "B": checkNumberColorKey[3]++; break;
                            case "R": checkNumberColorKey[4]++; break;
                            case "P": checkNumberColorKey[5]++; break;
                            case "C": checkNumberColorKey[6]++; break;
                        }
                    }
                }

                for (int tile0 = 0; tile0 < 11; tile0++)
                {
                    //1st tile
                    if (gotTiles[tile0] >= possibleTiles[tile0]) continue;  //如果這張數字牌的已取Tile數已到達上限，要跳過這個數字牌

                    tile[0] = tile0;
                    gotTiles[tile0]++;

                    for (int tile1 = tile0; tile1 < 11; tile1++)
                    {
                        //2nd tile
                        if (gotTiles[tile1] >= possibleTiles[tile1]) continue;  //如果這張數字牌的已取Tile數已到達上限，要跳過這個數字牌

                        tile[1] = tile1;
                        gotTiles[tile1]++;

                        for (int tile2 = tile1; tile2 < 11; tile2++)
                        {
                            //3rd tile
                            if (gotTiles[tile2] >= possibleTiles[tile2]) continue;  //如果這張數字牌的已取Tile數已到達上限，要跳過這個數字牌

                            tile[2] = tile2;
                            gotTiles[tile2]++;

                            //INSERT SetString
                            string setString = "";

                            for (int x = 0; x < 3; x++)
                            {
                                switch (tile[x])
                                {
                                    case 0: setString = setString + "1G"; break;
                                    case 1: setString = setString + "2Y"; break;
                                    case 2: setString = setString + "3K"; break;
                                    case 3: setString = setString + "4B"; break;
                                    case 4: setString = setString + "5R"; break;
                                    case 5: setString = setString + "5K"; break;
                                    case 6: setString = setString + "6P"; break;
                                    case 7: setString = setString + "6G"; break;
                                    case 8: setString = setString + "7Y"; break;
                                    case 9: setString = setString + "7P"; break;
                                    case 10: setString = setString + "7C"; break;
                                }
                                if (x != 2)
                                    setString = setString + "-";
                            }

                            conditionCheck = (((checkNumberColorKey[0] + gotTiles[0] + gotTiles[7] >=3 ) ? 1 : 0) +
                                                            ((checkNumberColorKey[1] + gotTiles[1] + gotTiles[8] >= 3) ? 1 : 0) +
                                                            ((checkNumberColorKey[2] + gotTiles[2] + gotTiles[5] >= 3) ? 1 : 0) +
                                                            ((checkNumberColorKey[3] + gotTiles[3] >= 3) ? 1 : 0) +
                                                            ((checkNumberColorKey[4] + gotTiles[4] >= 3) ? 1 : 0) +
                                                            ((checkNumberColorKey[5] + gotTiles[6] + gotTiles[9] >= 3) ? 1 : 0) +
                                                            ((checkNumberColorKey[6] + gotTiles[10] >= 3) ? 1 : 0)) == answerKey;

                            //conditionCheck需要為True，並原本參考中的可能性裡存在的組合，才會被保留下來
                            if (conditionCheck && _possibleSet.Contains(setString)) possibleSet.Add(setString);

                            gotTiles[tile2]--;  //TILE用完記得歸還
                        }
                        gotTiles[tile1]--;  //TILE用完記得歸還
                    }
                    gotTiles[tile0]--;  //TILE用完記得歸還
                }
                #endregion
                break;
            case 10: //". 有幾種數字完全沒有出現？";
                #region 問題10的判斷
                for (int playerIndex = 0; playerIndex < 3; playerIndex++)
                {
                    for (int tileIndex = 0; tileIndex < 3; tileIndex++)
                    {
                        switch (checkRack[playerIndex].tiles[tileIndex].number)
                        {
                            case 1: checkNumberColorKey[0]++; break;
                            case 2: checkNumberColorKey[1]++; break;
                            case 3: checkNumberColorKey[2]++; break;
                            case 4: checkNumberColorKey[3]++; break;
                            case 5: checkNumberColorKey[4]++; break;
                            case 6: checkNumberColorKey[5]++; break;
                            case 7: checkNumberColorKey[6]++; break;
                        }
                    }
                }

                for (int tile0 = 0; tile0 < 11; tile0++)
                {
                    //1st tile
                    if (gotTiles[tile0] >= possibleTiles[tile0]) continue;  //如果這張數字牌的已取Tile數已到達上限，要跳過這個數字牌

                    tile[0] = tile0;
                    gotTiles[tile0]++;

                    for (int tile1 = tile0; tile1 < 11; tile1++)
                    {
                        //2nd tile
                        if (gotTiles[tile1] >= possibleTiles[tile1]) continue;  //如果這張數字牌的已取Tile數已到達上限，要跳過這個數字牌

                        tile[1] = tile1;
                        gotTiles[tile1]++;

                        for (int tile2 = tile1; tile2 < 11; tile2++)
                        {
                            //3rd tile
                            if (gotTiles[tile2] >= possibleTiles[tile2]) continue;  //如果這張數字牌的已取Tile數已到達上限，要跳過這個數字牌

                            tile[2] = tile2;
                            gotTiles[tile2]++;

                            //INSERT SetString
                            string setString = "";

                            for (int x = 0; x < 3; x++)
                            {
                                switch (tile[x])
                                {
                                    case 0: setString = setString + "1G"; break;
                                    case 1: setString = setString + "2Y"; break;
                                    case 2: setString = setString + "3K"; break;
                                    case 3: setString = setString + "4B"; break;
                                    case 4: setString = setString + "5R"; break;
                                    case 5: setString = setString + "5K"; break;
                                    case 6: setString = setString + "6P"; break;
                                    case 7: setString = setString + "6G"; break;
                                    case 8: setString = setString + "7Y"; break;
                                    case 9: setString = setString + "7P"; break;
                                    case 10: setString = setString + "7C"; break;
                                }
                                if (x != 2)
                                    setString = setString + "-";
                            }

                            conditionCheck = (((checkNumberColorKey[0] + gotTiles[0] == 0) ? 1 : 0) +
                                                            ((checkNumberColorKey[1] + gotTiles[1] == 0) ? 1 : 0) +
                                                            ((checkNumberColorKey[2] + gotTiles[2] == 0) ? 1 : 0) +
                                                            ((checkNumberColorKey[3] + gotTiles[3] == 0) ? 1 : 0) +
                                                            ((checkNumberColorKey[4] + gotTiles[4] + gotTiles[5] == 0) ? 1 : 0) +
                                                            ((checkNumberColorKey[5] + gotTiles[6] + gotTiles[7] == 0) ? 1 : 0) +
                                                            ((checkNumberColorKey[6] + gotTiles[8] + gotTiles[9] +gotTiles[10] == 0) ? 1 : 0)) == answerKey;

                            //conditionCheck需要為True，並原本參考中的可能性裡存在的組合，才會被保留下來
                            if (conditionCheck && _possibleSet.Contains(setString)) possibleSet.Add(setString);

                            gotTiles[tile2]--;  //TILE用完記得歸還
                        }
                        gotTiles[tile1]--;  //TILE用完記得歸還
                    }
                    gotTiles[tile0]--;  //TILE用完記得歸還
                }
                #endregion
                break;
            case 11: //". 綠1、黑5、粉紅7這三種牌，你總共看到幾張？";
                #region 問題11的判斷
                checkNumberKey = 0;
                for (int playerIndex = 0; playerIndex < 3; playerIndex++)
                {
                    for (int tileIndex = 0; tileIndex < 3; tileIndex++)
                    {
                        switch ((checkRack[playerIndex].tiles[tileIndex].number.ToString() + checkRack[playerIndex].tiles[tileIndex].color))
                        {
                            default: checkNumberKey += 0; break;
                            case "1G": checkNumberKey++; break;
                            case "5K": checkNumberKey++; break;
                            case "7P": checkNumberKey++; break;
                        }
                    }
                }

                for (int tile0 = 0; tile0 < 11; tile0++)
                {
                    //1st tile
                    if (gotTiles[tile0] >= possibleTiles[tile0]) continue;  //如果這張數字牌的已取Tile數已到達上限，要跳過這個數字牌

                    tile[0] = tile0;
                    gotTiles[tile0]++;

                    for (int tile1 = tile0; tile1 < 11; tile1++)
                    {
                        //2nd tile
                        if (gotTiles[tile1] >= possibleTiles[tile1]) continue;  //如果這張數字牌的已取Tile數已到達上限，要跳過這個數字牌

                        tile[1] = tile1;
                        gotTiles[tile1]++;

                        for (int tile2 = tile1; tile2 < 11; tile2++)
                        {
                            //3rd tile
                            if (gotTiles[tile2] >= possibleTiles[tile2]) continue;  //如果這張數字牌的已取Tile數已到達上限，要跳過這個數字牌

                            tile[2] = tile2;
                            gotTiles[tile2]++;

                            //INSERT SetString
                            string setString = "";

                            for (int x = 0; x < 3; x++)
                            {
                                switch (tile[x])
                                {
                                    case 0: setString = setString + "1G"; break;
                                    case 1: setString = setString + "2Y"; break;
                                    case 2: setString = setString + "3K"; break;
                                    case 3: setString = setString + "4B"; break;
                                    case 4: setString = setString + "5R"; break;
                                    case 5: setString = setString + "5K"; break;
                                    case 6: setString = setString + "6P"; break;
                                    case 7: setString = setString + "6G"; break;
                                    case 8: setString = setString + "7Y"; break;
                                    case 9: setString = setString + "7P"; break;
                                    case 10: setString = setString + "7C"; break;
                                }
                                if (x != 2)
                                    setString = setString + "-";
                            }

                            conditionCheck = ( ( checkNumberKey + gotTiles[0] + gotTiles[5] + gotTiles[9] ) == answerKey);

                            //conditionCheck需要為True，並原本參考中的可能性裡存在的組合，才會被保留下來
                            if (conditionCheck && _possibleSet.Contains(setString)) possibleSet.Add(setString);

                            gotTiles[tile2]--;  //TILE用完記得歸還
                        }
                        gotTiles[tile1]--;  //TILE用完記得歸還
                    }
                    gotTiles[tile0]--;  //TILE用完記得歸還
                }
                #endregion
                break;
            case 12: //". 黑3和粉紅6，何者較多？";
                #region 問題12的判斷
                //compareKey1為true時，表示前者較多；compareKey2為true時，表示後者較多；若都為false則表示一樣多
                compareTileKey1 = 0;
                compareTileKey2 = 0;
                for (int playerIndex = 0; playerIndex < 3; playerIndex++)
                {
                    for (int tileIndex = 0; tileIndex < 3; tileIndex++)
                    {
                        switch ((checkRack[playerIndex].tiles[tileIndex].number + checkRack[playerIndex].tiles[tileIndex].color))
                        {
                            case "3K": compareTileKey1++; break;
                            case "6P": compareTileKey2++; break;
                            default: break;
                        }
                    }
                }

                for (int tile0 = 0; tile0 < 11; tile0++)
                {
                    //1st tile
                    if (gotTiles[tile0] >= possibleTiles[tile0]) continue;  //如果這張數字牌的已取Tile數已到達上限，要跳過這個數字牌

                    tile[0] = tile0;
                    gotTiles[tile0]++;

                    for (int tile1 = tile0; tile1 < 11; tile1++)
                    {
                        //2nd tile
                        if (gotTiles[tile1] >= possibleTiles[tile1]) continue;  //如果這張數字牌的已取Tile數已到達上限，要跳過這個數字牌

                        tile[1] = tile1;
                        gotTiles[tile1]++;

                        for (int tile2 = tile1; tile2 < 11; tile2++)
                        {
                            //3rd tile
                            if (gotTiles[tile2] >= possibleTiles[tile2]) continue;  //如果這張數字牌的已取Tile數已到達上限，要跳過這個數字牌

                            tile[2] = tile2;
                            gotTiles[tile2]++;

                            //INSERT SetString
                            string setString = "";

                            for (int x = 0; x < 3; x++)
                            {
                                switch (tile[x])
                                {
                                    case 0: setString = setString + "1G"; break;
                                    case 1: setString = setString + "2Y"; break;
                                    case 2: setString = setString + "3K"; break;
                                    case 3: setString = setString + "4B"; break;
                                    case 4: setString = setString + "5R"; break;
                                    case 5: setString = setString + "5K"; break;
                                    case 6: setString = setString + "6P"; break;
                                    case 7: setString = setString + "6G"; break;
                                    case 8: setString = setString + "7Y"; break;
                                    case 9: setString = setString + "7P"; break;
                                    case 10: setString = setString + "7C"; break;
                                }
                                if (x != 2)
                                    setString = setString + "-";
                            }

                            //compare1>compare2且compareKey1為true、 compare1<compare2且compareKey1為false、 compare1==compare2且compareKey1、compareKey2均為false
                            conditionCheck = ((compareTileKey1 + gotTiles[2] > compareTileKey2 + gotTiles[6]) && compareKey1) ||
                                                        ((compareTileKey1 + gotTiles[2] < compareTileKey2 + gotTiles[6]) && compareKey2) ||
                                                        ((compareTileKey1 + gotTiles[2] == compareTileKey2 + gotTiles[6]) && !compareKey1 && !compareKey2);

                            //conditionCheck需要為True，並原本參考中的可能性裡存在的組合，才會被保留下來
                            if (conditionCheck && _possibleSet.Contains(setString)) possibleSet.Add(setString);

                            gotTiles[tile2]--;  //TILE用完記得歸還
                        }
                        gotTiles[tile1]--;  //TILE用完記得歸還
                    }
                    gotTiles[tile0]--;  //TILE用完記得歸還
                }
                #endregion
                break;
            case 13: //". 綠6和黃7，何者較多？";
                #region 問題13的判斷
                //compareKey1為true時，表示前者較多；compareKey2為true時，表示後者較多；若都為false則表示一樣多
                compareTileKey1 = 0;
                compareTileKey2 = 0;
                for (int playerIndex = 0; playerIndex < 3; playerIndex++)
                {
                    for (int tileIndex = 0; tileIndex < 3; tileIndex++)
                    {
                        switch ((checkRack[playerIndex].tiles[tileIndex].number + checkRack[playerIndex].tiles[tileIndex].color))
                        {
                            case "6G": compareTileKey1++; break;
                            case "7Y": compareTileKey2++; break;
                            default: break;
                        }
                    }
                }

                for (int tile0 = 0; tile0 < 11; tile0++)
                {
                    //1st tile
                    if (gotTiles[tile0] >= possibleTiles[tile0]) continue;  //如果這張數字牌的已取Tile數已到達上限，要跳過這個數字牌

                    tile[0] = tile0;
                    gotTiles[tile0]++;

                    for (int tile1 = tile0; tile1 < 11; tile1++)
                    {
                        //2nd tile
                        if (gotTiles[tile1] >= possibleTiles[tile1]) continue;  //如果這張數字牌的已取Tile數已到達上限，要跳過這個數字牌

                        tile[1] = tile1;
                        gotTiles[tile1]++;

                        for (int tile2 = tile1; tile2 < 11; tile2++)
                        {
                            //3rd tile
                            if (gotTiles[tile2] >= possibleTiles[tile2]) continue;  //如果這張數字牌的已取Tile數已到達上限，要跳過這個數字牌

                            tile[2] = tile2;
                            gotTiles[tile2]++;

                            //INSERT SetString
                            string setString = "";

                            for (int x = 0; x < 3; x++)
                            {
                                switch (tile[x])
                                {
                                    case 0: setString = setString + "1G"; break;
                                    case 1: setString = setString + "2Y"; break;
                                    case 2: setString = setString + "3K"; break;
                                    case 3: setString = setString + "4B"; break;
                                    case 4: setString = setString + "5R"; break;
                                    case 5: setString = setString + "5K"; break;
                                    case 6: setString = setString + "6P"; break;
                                    case 7: setString = setString + "6G"; break;
                                    case 8: setString = setString + "7Y"; break;
                                    case 9: setString = setString + "7P"; break;
                                    case 10: setString = setString + "7C"; break;
                                }
                                if (x != 2)
                                    setString = setString + "-";
                            }

                            //compare1>compare2且compareKey1為true、 compare1<compare2且compareKey1為false、 compare1==compare2且compareKey1、compareKey2均為false
                            conditionCheck = ((compareTileKey1 + gotTiles[7] > compareTileKey2 + gotTiles[8]) && compareKey1) ||
                                                        ((compareTileKey1 + gotTiles[7] < compareTileKey2 + gotTiles[8]) && compareKey2) ||
                                                        ((compareTileKey1 + gotTiles[7] == compareTileKey2 + gotTiles[8]) && !compareKey1 && !compareKey2);

                            //conditionCheck需要為True，並原本參考中的可能性裡存在的組合，才會被保留下來
                            if (conditionCheck && _possibleSet.Contains(setString)) possibleSet.Add(setString);

                            gotTiles[tile2]--;  //TILE用完記得歸還
                        }
                        gotTiles[tile1]--;  //TILE用完記得歸還
                    }
                    gotTiles[tile0]--;  //TILE用完記得歸還
                }
                #endregion
                break;
            case 14: //". 黃2和黃7，何者較多？";
                #region 問題14的判斷
                //compareKey1為true時，表示前者較多；compareKey2為true時，表示後者較多；若都為false則表示一樣多
                compareTileKey1 = 0;
                compareTileKey2 = 0;
                for (int playerIndex = 0; playerIndex < 3; playerIndex++)
                {
                    for (int tileIndex = 0; tileIndex < 3; tileIndex++)
                    {
                        switch ((checkRack[playerIndex].tiles[tileIndex].number + checkRack[playerIndex].tiles[tileIndex].color))
                        {
                            case "2Y": compareTileKey1++; break;
                            case "7Y": compareTileKey2++; break;
                            default: break;
                        }
                    }
                }

                for (int tile0 = 0; tile0 < 11; tile0++)
                {
                    //1st tile
                    if (gotTiles[tile0] >= possibleTiles[tile0]) continue;  //如果這張數字牌的已取Tile數已到達上限，要跳過這個數字牌

                    tile[0] = tile0;
                    gotTiles[tile0]++;

                    for (int tile1 = tile0; tile1 < 11; tile1++)
                    {
                        //2nd tile
                        if (gotTiles[tile1] >= possibleTiles[tile1]) continue;  //如果這張數字牌的已取Tile數已到達上限，要跳過這個數字牌

                        tile[1] = tile1;
                        gotTiles[tile1]++;

                        for (int tile2 = tile1; tile2 < 11; tile2++)
                        {
                            //3rd tile
                            if (gotTiles[tile2] >= possibleTiles[tile2]) continue;  //如果這張數字牌的已取Tile數已到達上限，要跳過這個數字牌

                            tile[2] = tile2;
                            gotTiles[tile2]++;

                            //INSERT SetString
                            string setString = "";

                            for (int x = 0; x < 3; x++)
                            {
                                switch (tile[x])
                                {
                                    case 0: setString = setString + "1G"; break;
                                    case 1: setString = setString + "2Y"; break;
                                    case 2: setString = setString + "3K"; break;
                                    case 3: setString = setString + "4B"; break;
                                    case 4: setString = setString + "5R"; break;
                                    case 5: setString = setString + "5K"; break;
                                    case 6: setString = setString + "6P"; break;
                                    case 7: setString = setString + "6G"; break;
                                    case 8: setString = setString + "7Y"; break;
                                    case 9: setString = setString + "7P"; break;
                                    case 10: setString = setString + "7C"; break;
                                }
                                if (x != 2)
                                    setString = setString + "-";
                            }

                            //compare1>compare2且compareKey1為true、 compare1<compare2且compareKey1為false、 compare1==compare2且compareKey1、compareKey2均為false
                            conditionCheck = ((compareTileKey1 + gotTiles[1] > compareTileKey2 + gotTiles[8]) && compareKey1) ||
                                                        ((compareTileKey1 + gotTiles[1] < compareTileKey2 + gotTiles[8]) && compareKey2) ||
                                                        ((compareTileKey1 + gotTiles[1] == compareTileKey2 + gotTiles[8]) && !compareKey1 && !compareKey2);

                            //conditionCheck需要為True，並原本參考中的可能性裡存在的組合，才會被保留下來
                            if (conditionCheck && _possibleSet.Contains(setString)) possibleSet.Add(setString);

                            gotTiles[tile2]--;  //TILE用完記得歸還
                        }
                        gotTiles[tile1]--;  //TILE用完記得歸還
                    }
                    gotTiles[tile0]--;  //TILE用完記得歸還
                }
                #endregion
                break;
            case 15: //". 粉紅6和綠6，何者較多？";
                #region 問題15的判斷
                //compareKey1為true時，表示前者較多；compareKey2為true時，表示後者較多；若都為false則表示一樣多
                compareTileKey1 = 0;
                compareTileKey2 = 0;
                for (int playerIndex = 0; playerIndex < 3; playerIndex++)
                {
                    for (int tileIndex = 0; tileIndex < 3; tileIndex++)
                    {
                        switch ((checkRack[playerIndex].tiles[tileIndex].number + checkRack[playerIndex].tiles[tileIndex].color))
                        {
                            case "6P": compareTileKey1++; break;
                            case "6G": compareTileKey2++; break;
                            default: break;
                        }
                    }
                }

                for (int tile0 = 0; tile0 < 11; tile0++)
                {
                    //1st tile
                    if (gotTiles[tile0] >= possibleTiles[tile0]) continue;  //如果這張數字牌的已取Tile數已到達上限，要跳過這個數字牌

                    tile[0] = tile0;
                    gotTiles[tile0]++;

                    for (int tile1 = tile0; tile1 < 11; tile1++)
                    {
                        //2nd tile
                        if (gotTiles[tile1] >= possibleTiles[tile1]) continue;  //如果這張數字牌的已取Tile數已到達上限，要跳過這個數字牌

                        tile[1] = tile1;
                        gotTiles[tile1]++;

                        for (int tile2 = tile1; tile2 < 11; tile2++)
                        {
                            //3rd tile
                            if (gotTiles[tile2] >= possibleTiles[tile2]) continue;  //如果這張數字牌的已取Tile數已到達上限，要跳過這個數字牌

                            tile[2] = tile2;
                            gotTiles[tile2]++;

                            //INSERT SetString
                            string setString = "";

                            for (int x = 0; x < 3; x++)
                            {
                                switch (tile[x])
                                {
                                    case 0: setString = setString + "1G"; break;
                                    case 1: setString = setString + "2Y"; break;
                                    case 2: setString = setString + "3K"; break;
                                    case 3: setString = setString + "4B"; break;
                                    case 4: setString = setString + "5R"; break;
                                    case 5: setString = setString + "5K"; break;
                                    case 6: setString = setString + "6P"; break;
                                    case 7: setString = setString + "6G"; break;
                                    case 8: setString = setString + "7Y"; break;
                                    case 9: setString = setString + "7P"; break;
                                    case 10: setString = setString + "7C"; break;
                                }
                                if (x != 2)
                                    setString = setString + "-";
                            }

                            //compare1>compare2且compareKey1為true、 compare1<compare2且compareKey1為false、 compare1==compare2且compareKey1、compareKey2均為false
                            conditionCheck = ((compareTileKey1 + gotTiles[6] > compareTileKey2 + gotTiles[7]) && compareKey1) ||
                                                        ((compareTileKey1 + gotTiles[6] < compareTileKey2 + gotTiles[7]) && compareKey2) ||
                                                        ((compareTileKey1 + gotTiles[6] == compareTileKey2 + gotTiles[7]) && !compareKey1 && !compareKey2);

                            //conditionCheck需要為True，並原本參考中的可能性裡存在的組合，才會被保留下來
                            if (conditionCheck && _possibleSet.Contains(setString)) possibleSet.Add(setString);

                            gotTiles[tile2]--;  //TILE用完記得歸還
                        }
                        gotTiles[tile1]--;  //TILE用完記得歸還
                    }
                    gotTiles[tile0]--;  //TILE用完記得歸還
                }
                #endregion
                break;
            case 16: //". 藍7和其他顏色的7，何者較多？";
                #region 問題16的判斷
                //compareKey1為true時，表示前者較多；compareKey2為true時，表示後者較多；若都為false則表示一樣多
                compareTileKey1 = 0;
                compareTileKey2 = 0;
                for (int playerIndex = 0; playerIndex < 3; playerIndex++)
                {
                    for (int tileIndex = 0; tileIndex < 3; tileIndex++)
                    {
                        switch ((checkRack[playerIndex].tiles[tileIndex].number + checkRack[playerIndex].tiles[tileIndex].color))
                        {
                            case "7C": compareTileKey1++; break;
                            case "7Y": compareTileKey2++; break;
                            case "7P": compareTileKey2++; break;
                            default: break;
                        }
                    }
                }

                for (int tile0 = 0; tile0 < 11; tile0++)
                {
                    //1st tile
                    if (gotTiles[tile0] >= possibleTiles[tile0]) continue;  //如果這張數字牌的已取Tile數已到達上限，要跳過這個數字牌

                    tile[0] = tile0;
                    gotTiles[tile0]++;

                    for (int tile1 = tile0; tile1 < 11; tile1++)
                    {
                        //2nd tile
                        if (gotTiles[tile1] >= possibleTiles[tile1]) continue;  //如果這張數字牌的已取Tile數已到達上限，要跳過這個數字牌

                        tile[1] = tile1;
                        gotTiles[tile1]++;

                        for (int tile2 = tile1; tile2 < 11; tile2++)
                        {
                            //3rd tile
                            if (gotTiles[tile2] >= possibleTiles[tile2]) continue;  //如果這張數字牌的已取Tile數已到達上限，要跳過這個數字牌

                            tile[2] = tile2;
                            gotTiles[tile2]++;

                            //INSERT SetString
                            string setString = "";

                            for (int x = 0; x < 3; x++)
                            {
                                switch (tile[x])
                                {
                                    case 0: setString = setString + "1G"; break;
                                    case 1: setString = setString + "2Y"; break;
                                    case 2: setString = setString + "3K"; break;
                                    case 3: setString = setString + "4B"; break;
                                    case 4: setString = setString + "5R"; break;
                                    case 5: setString = setString + "5K"; break;
                                    case 6: setString = setString + "6P"; break;
                                    case 7: setString = setString + "6G"; break;
                                    case 8: setString = setString + "7Y"; break;
                                    case 9: setString = setString + "7P"; break;
                                    case 10: setString = setString + "7C"; break;
                                }
                                if (x != 2)
                                    setString = setString + "-";
                            }

                            //compare1>compare2且compareKey1為true、 compare1<compare2且compareKey1為false、 compare1==compare2且compareKey1、compareKey2均為false
                            conditionCheck = ((compareTileKey1 + gotTiles[10] > compareTileKey2 + gotTiles[8] + gotTiles[8]) && compareKey1) ||
                                                        ((compareTileKey1 + gotTiles[10] < compareTileKey2 + gotTiles[8] + gotTiles[9]) && compareKey2) ||
                                                        ((compareTileKey1 + gotTiles[10] == compareTileKey2 + gotTiles[8] + gotTiles[9]) && !compareKey1 && !compareKey2);

                            //conditionCheck需要為True，並原本參考中的可能性裡存在的組合，才會被保留下來
                            if (conditionCheck && _possibleSet.Contains(setString)) possibleSet.Add(setString);

                            gotTiles[tile2]--;  //TILE用完記得歸還
                        }
                        gotTiles[tile1]--;  //TILE用完記得歸還
                    }
                    gotTiles[tile0]--;  //TILE用完記得歸還
                }
                #endregion
                break;
            case 17: //". 棕色和藍色，何者較多？";
                #region 問題17的判斷
                //compareKey1為true時，表示前者較多；compareKey2為true時，表示後者較多；若都為false則表示一樣多
                compareTileKey1 = 0;
                compareTileKey2 = 0;
                for (int playerIndex = 0; playerIndex < 3; playerIndex++)
                {
                    for (int tileIndex = 0; tileIndex < 3; tileIndex++)
                    {
                        switch ((checkRack[playerIndex].tiles[tileIndex].color))
                        {
                            case "B": compareTileKey1++; break;
                            case "C": compareTileKey2++; break;
                            default: break;
                        }
                    }
                }

                for (int tile0 = 0; tile0 < 11; tile0++)
                {
                    //1st tile
                    if (gotTiles[tile0] >= possibleTiles[tile0]) continue;  //如果這張數字牌的已取Tile數已到達上限，要跳過這個數字牌

                    tile[0] = tile0;
                    gotTiles[tile0]++;

                    for (int tile1 = tile0; tile1 < 11; tile1++)
                    {
                        //2nd tile
                        if (gotTiles[tile1] >= possibleTiles[tile1]) continue;  //如果這張數字牌的已取Tile數已到達上限，要跳過這個數字牌

                        tile[1] = tile1;
                        gotTiles[tile1]++;

                        for (int tile2 = tile1; tile2 < 11; tile2++)
                        {
                            //3rd tile
                            if (gotTiles[tile2] >= possibleTiles[tile2]) continue;  //如果這張數字牌的已取Tile數已到達上限，要跳過這個數字牌

                            tile[2] = tile2;
                            gotTiles[tile2]++;

                            //INSERT SetString
                            string setString = "";

                            for (int x = 0; x < 3; x++)
                            {
                                switch (tile[x])
                                {
                                    case 0: setString = setString + "1G"; break;
                                    case 1: setString = setString + "2Y"; break;
                                    case 2: setString = setString + "3K"; break;
                                    case 3: setString = setString + "4B"; break;
                                    case 4: setString = setString + "5R"; break;
                                    case 5: setString = setString + "5K"; break;
                                    case 6: setString = setString + "6P"; break;
                                    case 7: setString = setString + "6G"; break;
                                    case 8: setString = setString + "7Y"; break;
                                    case 9: setString = setString + "7P"; break;
                                    case 10: setString = setString + "7C"; break;
                                }
                                if (x != 2)
                                    setString = setString + "-";
                            }

                            //compare1>compare2且compareKey1為true、 compare1<compare2且compareKey1為false、 compare1==compare2且compareKey1、compareKey2均為false
                            conditionCheck = ((compareTileKey1 + gotTiles[3] > compareTileKey2 + gotTiles[10]) && compareKey1) ||
                                                        ((compareTileKey1 + gotTiles[3] < compareTileKey2 + gotTiles[10]) && compareKey2) ||
                                                        ((compareTileKey1 + gotTiles[3] == compareTileKey2 +  gotTiles[10]) && !compareKey1 && !compareKey2);

                            //conditionCheck需要為True，並原本參考中的可能性裡存在的組合，才會被保留下來
                            if (conditionCheck && _possibleSet.Contains(setString)) possibleSet.Add(setString);

                            gotTiles[tile2]--;  //TILE用完記得歸還
                        }
                        gotTiles[tile1]--;  //TILE用完記得歸還
                    }
                    gotTiles[tile0]--;  //TILE用完記得歸還
                }
                #endregion
                break;
            case 18: //". 紅色和粉紅色，何者較多？";
                #region 問題18的判斷
                //compareKey1為true時，表示前者較多；compareKey2為true時，表示後者較多；若都為false則表示一樣多
                compareTileKey1 = 0;
                compareTileKey2 = 0;
                for (int playerIndex = 0; playerIndex < 3; playerIndex++)
                {
                    for (int tileIndex = 0; tileIndex < 3; tileIndex++)
                    {
                        switch ((checkRack[playerIndex].tiles[tileIndex].color))
                        {
                            case "R": compareTileKey1++; break;
                            case "P": compareTileKey2++; break;
                            default: break;
                        }
                    }
                }

                for (int tile0 = 0; tile0 < 11; tile0++)
                {
                    //1st tile
                    if (gotTiles[tile0] >= possibleTiles[tile0]) continue;  //如果這張數字牌的已取Tile數已到達上限，要跳過這個數字牌

                    tile[0] = tile0;
                    gotTiles[tile0]++;

                    for (int tile1 = tile0; tile1 < 11; tile1++)
                    {
                        //2nd tile
                        if (gotTiles[tile1] >= possibleTiles[tile1]) continue;  //如果這張數字牌的已取Tile數已到達上限，要跳過這個數字牌

                        tile[1] = tile1;
                        gotTiles[tile1]++;

                        for (int tile2 = tile1; tile2 < 11; tile2++)
                        {
                            //3rd tile
                            if (gotTiles[tile2] >= possibleTiles[tile2]) continue;  //如果這張數字牌的已取Tile數已到達上限，要跳過這個數字牌

                            tile[2] = tile2;
                            gotTiles[tile2]++;

                            //INSERT SetString
                            string setString = "";

                            for (int x = 0; x < 3; x++)
                            {
                                switch (tile[x])
                                {
                                    case 0: setString = setString + "1G"; break;
                                    case 1: setString = setString + "2Y"; break;
                                    case 2: setString = setString + "3K"; break;
                                    case 3: setString = setString + "4B"; break;
                                    case 4: setString = setString + "5R"; break;
                                    case 5: setString = setString + "5K"; break;
                                    case 6: setString = setString + "6P"; break;
                                    case 7: setString = setString + "6G"; break;
                                    case 8: setString = setString + "7Y"; break;
                                    case 9: setString = setString + "7P"; break;
                                    case 10: setString = setString + "7C"; break;
                                }
                                if (x != 2)
                                    setString = setString + "-";
                            }

                            //compare1>compare2且compareKey1為true、 compare1<compare2且compareKey1為false、 compare1==compare2且compareKey1、compareKey2均為false
                            conditionCheck = ((compareTileKey1 + gotTiles[4] > compareTileKey2 + gotTiles[6] + gotTiles[9] ) && compareKey1) ||
                                                        ((compareTileKey1 + gotTiles[4] < compareTileKey2 + gotTiles[6] + gotTiles[9]) && compareKey2) ||
                                                        ((compareTileKey1 + gotTiles[4] == compareTileKey2 + gotTiles[6] + gotTiles[9]) && !compareKey1 && !compareKey2);

                            //conditionCheck需要為True，並原本參考中的可能性裡存在的組合，才會被保留下來
                            if (conditionCheck && _possibleSet.Contains(setString)) possibleSet.Add(setString);

                            gotTiles[tile2]--;  //TILE用完記得歸還
                        }
                        gotTiles[tile1]--;  //TILE用完記得歸還
                    }
                    gotTiles[tile0]--;  //TILE用完記得歸還
                }
                #endregion
                break;
            case 19: //". 綠色和藍色，何者較多？";
                #region 問題19的判斷
                //compareKey1為true時，表示前者較多；compareKey2為true時，表示後者較多；若都為false則表示一樣多
                compareTileKey1 = 0;
                compareTileKey2 = 0;
                for (int playerIndex = 0; playerIndex < 3; playerIndex++)
                {
                    for (int tileIndex = 0; tileIndex < 3; tileIndex++)
                    {
                        switch ((checkRack[playerIndex].tiles[tileIndex].color))
                        {
                            case "G": compareTileKey1++; break;
                            case "C": compareTileKey2++; break;
                            default: break;
                        }
                    }
                }

                for (int tile0 = 0; tile0 < 11; tile0++)
                {
                    //1st tile
                    if (gotTiles[tile0] >= possibleTiles[tile0]) continue;  //如果這張數字牌的已取Tile數已到達上限，要跳過這個數字牌

                    tile[0] = tile0;
                    gotTiles[tile0]++;

                    for (int tile1 = tile0; tile1 < 11; tile1++)
                    {
                        //2nd tile
                        if (gotTiles[tile1] >= possibleTiles[tile1]) continue;  //如果這張數字牌的已取Tile數已到達上限，要跳過這個數字牌

                        tile[1] = tile1;
                        gotTiles[tile1]++;

                        for (int tile2 = tile1; tile2 < 11; tile2++)
                        {
                            //3rd tile
                            if (gotTiles[tile2] >= possibleTiles[tile2]) continue;  //如果這張數字牌的已取Tile數已到達上限，要跳過這個數字牌

                            tile[2] = tile2;
                            gotTiles[tile2]++;

                            //INSERT SetString
                            string setString = "";

                            for (int x = 0; x < 3; x++)
                            {
                                switch (tile[x])
                                {
                                    case 0: setString = setString + "1G"; break;
                                    case 1: setString = setString + "2Y"; break;
                                    case 2: setString = setString + "3K"; break;
                                    case 3: setString = setString + "4B"; break;
                                    case 4: setString = setString + "5R"; break;
                                    case 5: setString = setString + "5K"; break;
                                    case 6: setString = setString + "6P"; break;
                                    case 7: setString = setString + "6G"; break;
                                    case 8: setString = setString + "7Y"; break;
                                    case 9: setString = setString + "7P"; break;
                                    case 10: setString = setString + "7C"; break;
                                }
                                if (x != 2)
                                    setString = setString + "-";
                            }

                            //compare1>compare2且compareKey1為true、 compare1<compare2且compareKey1為false、 compare1==compare2且compareKey1、compareKey2均為false
                            conditionCheck = ((compareTileKey1 + gotTiles[0] + gotTiles[7] > compareTileKey2 + gotTiles[10]) && compareKey1) ||
                                                        ((compareTileKey1 + gotTiles[0] + gotTiles[7] < compareTileKey2 + gotTiles[10]) && compareKey2) ||
                                                        ((compareTileKey1 + gotTiles[0] + gotTiles[7] == compareTileKey2 + gotTiles[10]) && !compareKey1 && !compareKey2);

                            //conditionCheck需要為True，並原本參考中的可能性裡存在的組合，才會被保留下來
                            if (conditionCheck && _possibleSet.Contains(setString)) possibleSet.Add(setString);

                            gotTiles[tile2]--;  //TILE用完記得歸還
                        }
                        gotTiles[tile1]--;  //TILE用完記得歸還
                    }
                    gotTiles[tile0]--;  //TILE用完記得歸還
                }
                #endregion
                break;
            case 20: //". 黃色和粉紅色，何者較多？";
                #region 問題20的判斷
                //compareKey1為true時，表示前者較多；compareKey2為true時，表示後者較多；若都為false則表示一樣多
                compareTileKey1 = 0;
                compareTileKey2 = 0;
                for (int playerIndex = 0; playerIndex < 3; playerIndex++)
                {
                    for (int tileIndex = 0; tileIndex < 3; tileIndex++)
                    {
                        switch ((checkRack[playerIndex].tiles[tileIndex].color))
                        {
                            case "Y": compareTileKey1++; break;
                            case "P": compareTileKey2++; break;
                            default: break;
                        }
                    }
                }

                for (int tile0 = 0; tile0 < 11; tile0++)
                {
                    //1st tile
                    if (gotTiles[tile0] >= possibleTiles[tile0]) continue;  //如果這張數字牌的已取Tile數已到達上限，要跳過這個數字牌

                    tile[0] = tile0;
                    gotTiles[tile0]++;

                    for (int tile1 = tile0; tile1 < 11; tile1++)
                    {
                        //2nd tile
                        if (gotTiles[tile1] >= possibleTiles[tile1]) continue;  //如果這張數字牌的已取Tile數已到達上限，要跳過這個數字牌

                        tile[1] = tile1;
                        gotTiles[tile1]++;

                        for (int tile2 = tile1; tile2 < 11; tile2++)
                        {
                            //3rd tile
                            if (gotTiles[tile2] >= possibleTiles[tile2]) continue;  //如果這張數字牌的已取Tile數已到達上限，要跳過這個數字牌

                            tile[2] = tile2;
                            gotTiles[tile2]++;

                            //INSERT SetString
                            string setString = "";

                            for (int x = 0; x < 3; x++)
                            {
                                switch (tile[x])
                                {
                                    case 0: setString = setString + "1G"; break;
                                    case 1: setString = setString + "2Y"; break;
                                    case 2: setString = setString + "3K"; break;
                                    case 3: setString = setString + "4B"; break;
                                    case 4: setString = setString + "5R"; break;
                                    case 5: setString = setString + "5K"; break;
                                    case 6: setString = setString + "6P"; break;
                                    case 7: setString = setString + "6G"; break;
                                    case 8: setString = setString + "7Y"; break;
                                    case 9: setString = setString + "7P"; break;
                                    case 10: setString = setString + "7C"; break;
                                }
                                if (x != 2)
                                    setString = setString + "-";
                            }

                            //compare1>compare2且compareKey1為true、 compare1<compare2且compareKey1為false、 compare1==compare2且compareKey1、compareKey2均為false
                            conditionCheck = ((compareTileKey1 + gotTiles[1] + gotTiles[8] > compareTileKey2 + gotTiles[6] + gotTiles[9]) && compareKey1) ||
                                                        ((compareTileKey1 + gotTiles[1] + gotTiles[8] < compareTileKey2 + gotTiles[6] + gotTiles[9]) && compareKey2) ||
                                                        ((compareTileKey1 + gotTiles[1] + gotTiles[8] == compareTileKey2 + gotTiles[6] + gotTiles[9]) && !compareKey1 && !compareKey2);

                            //conditionCheck需要為True，並原本參考中的可能性裡存在的組合，才會被保留下來
                            if (conditionCheck && _possibleSet.Contains(setString)) possibleSet.Add(setString);

                            gotTiles[tile2]--;  //TILE用完記得歸還
                        }
                        gotTiles[tile1]--;  //TILE用完記得歸還
                    }
                    gotTiles[tile0]--;  //TILE用完記得歸還
                }
                #endregion
                break;
            case 21: //". 黑色和棕色，何者較多？";
                #region 問題21的判斷
                //compareKey1為true時，表示前者較多；compareKey2為true時，表示後者較多；若都為false則表示一樣多
                compareTileKey1 = 0;
                compareTileKey2 = 0;
                for (int playerIndex = 0; playerIndex < 3; playerIndex++)
                {
                    for (int tileIndex = 0; tileIndex < 3; tileIndex++)
                    {
                        switch ((checkRack[playerIndex].tiles[tileIndex].color))
                        {
                            case "K": compareTileKey1++; break;
                            case "B": compareTileKey2++; break;
                            default: break;
                        }
                    }
                }

                for (int tile0 = 0; tile0 < 11; tile0++)
                {
                    //1st tile
                    if (gotTiles[tile0] >= possibleTiles[tile0]) continue;  //如果這張數字牌的已取Tile數已到達上限，要跳過這個數字牌

                    tile[0] = tile0;
                    gotTiles[tile0]++;

                    for (int tile1 = tile0; tile1 < 11; tile1++)
                    {
                        //2nd tile
                        if (gotTiles[tile1] >= possibleTiles[tile1]) continue;  //如果這張數字牌的已取Tile數已到達上限，要跳過這個數字牌

                        tile[1] = tile1;
                        gotTiles[tile1]++;

                        for (int tile2 = tile1; tile2 < 11; tile2++)
                        {
                            //3rd tile
                            if (gotTiles[tile2] >= possibleTiles[tile2]) continue;  //如果這張數字牌的已取Tile數已到達上限，要跳過這個數字牌

                            tile[2] = tile2;
                            gotTiles[tile2]++;

                            //INSERT SetString
                            string setString = "";

                            for (int x = 0; x < 3; x++)
                            {
                                switch (tile[x])
                                {
                                    case 0: setString = setString + "1G"; break;
                                    case 1: setString = setString + "2Y"; break;
                                    case 2: setString = setString + "3K"; break;
                                    case 3: setString = setString + "4B"; break;
                                    case 4: setString = setString + "5R"; break;
                                    case 5: setString = setString + "5K"; break;
                                    case 6: setString = setString + "6P"; break;
                                    case 7: setString = setString + "6G"; break;
                                    case 8: setString = setString + "7Y"; break;
                                    case 9: setString = setString + "7P"; break;
                                    case 10: setString = setString + "7C"; break;
                                }
                                if (x != 2)
                                    setString = setString + "-";
                            }

                            //compare1>compare2且compareKey1為true、 compare1<compare2且compareKey1為false、 compare1==compare2且compareKey1、compareKey2均為false
                            conditionCheck = ((compareTileKey1 + gotTiles[2] + gotTiles[5] > compareTileKey2 + gotTiles[3]) && compareKey1) ||
                                                        ((compareTileKey1 + gotTiles[2] + gotTiles[5] < compareTileKey2 + gotTiles[3]) && compareKey2) ||
                                                        ((compareTileKey1 + gotTiles[2] + gotTiles[5] == compareTileKey2 + gotTiles[3]) && !compareKey1 && !compareKey2);

                            //conditionCheck需要為True，並原本參考中的可能性裡存在的組合，才會被保留下來
                            if (conditionCheck && _possibleSet.Contains(setString)) possibleSet.Add(setString);

                            gotTiles[tile2]--;  //TILE用完記得歸還
                        }
                        gotTiles[tile1]--;  //TILE用完記得歸還
                    }
                    gotTiles[tile0]--;  //TILE用完記得歸還
                }
                #endregion
                break;
            case 22: //". 黑色和紅色，何者較多？";
                #region 問題22的判斷
                //compareKey1為true時，表示前者較多；compareKey2為true時，表示後者較多；若都為false則表示一樣多
                compareTileKey1 = 0;
                compareTileKey2 = 0;
                for (int playerIndex = 0; playerIndex < 3; playerIndex++)
                {
                    for (int tileIndex = 0; tileIndex < 3; tileIndex++)
                    {
                        switch ((checkRack[playerIndex].tiles[tileIndex].color))
                        {
                            case "K": compareTileKey1++; break;
                            case "R": compareTileKey2++; break;
                            default: break;
                        }
                    }
                }

                for (int tile0 = 0; tile0 < 11; tile0++)
                {
                    //1st tile
                    if (gotTiles[tile0] >= possibleTiles[tile0]) continue;  //如果這張數字牌的已取Tile數已到達上限，要跳過這個數字牌

                    tile[0] = tile0;
                    gotTiles[tile0]++;

                    for (int tile1 = tile0; tile1 < 11; tile1++)
                    {
                        //2nd tile
                        if (gotTiles[tile1] >= possibleTiles[tile1]) continue;  //如果這張數字牌的已取Tile數已到達上限，要跳過這個數字牌

                        tile[1] = tile1;
                        gotTiles[tile1]++;

                        for (int tile2 = tile1; tile2 < 11; tile2++)
                        {
                            //3rd tile
                            if (gotTiles[tile2] >= possibleTiles[tile2]) continue;  //如果這張數字牌的已取Tile數已到達上限，要跳過這個數字牌

                            tile[2] = tile2;
                            gotTiles[tile2]++;

                            //INSERT SetString
                            string setString = "";

                            for (int x = 0; x < 3; x++)
                            {
                                switch (tile[x])
                                {
                                    case 0: setString = setString + "1G"; break;
                                    case 1: setString = setString + "2Y"; break;
                                    case 2: setString = setString + "3K"; break;
                                    case 3: setString = setString + "4B"; break;
                                    case 4: setString = setString + "5R"; break;
                                    case 5: setString = setString + "5K"; break;
                                    case 6: setString = setString + "6P"; break;
                                    case 7: setString = setString + "6G"; break;
                                    case 8: setString = setString + "7Y"; break;
                                    case 9: setString = setString + "7P"; break;
                                    case 10: setString = setString + "7C"; break;
                                }
                                if (x != 2)
                                    setString = setString + "-";
                            }

                            //compare1>compare2且compareKey1為true、 compare1<compare2且compareKey1為false、 compare1==compare2且compareKey1、compareKey2均為false
                            conditionCheck = ((compareTileKey1 + gotTiles[2] + gotTiles[5] > compareTileKey2 + gotTiles[4]) && compareKey1) ||
                                                        ((compareTileKey1 + gotTiles[2] + gotTiles[5] < compareTileKey2 + gotTiles[4]) && compareKey2) ||
                                                        ((compareTileKey1 + gotTiles[2] + gotTiles[5] == compareTileKey2 + gotTiles[4]) && !compareKey1 && !compareKey2);

                            //conditionCheck需要為True，並原本參考中的可能性裡存在的組合，才會被保留下來
                            if (conditionCheck && _possibleSet.Contains(setString)) possibleSet.Add(setString);

                            gotTiles[tile2]--;  //TILE用完記得歸還
                        }
                        gotTiles[tile1]--;  //TILE用完記得歸還
                    }
                    gotTiles[tile0]--;  //TILE用完記得歸還
                }
                #endregion
                break;
            case 23: //". 綠色和黃色，何者較多？";
                #region 問題23的判斷
                //compareKey1為true時，表示前者較多；compareKey2為true時，表示後者較多；若都為false則表示一樣多
                compareTileKey1 = 0;
                compareTileKey2 = 0;
                for (int playerIndex = 0; playerIndex < 3; playerIndex++)
                {
                    for (int tileIndex = 0; tileIndex < 3; tileIndex++)
                    {
                        switch ((checkRack[playerIndex].tiles[tileIndex].color))
                        {
                            case "G": compareTileKey1++; break;
                            case "Y": compareTileKey2++; break;
                            default: break;
                        }
                    }
                }

                for (int tile0 = 0; tile0 < 11; tile0++)
                {
                    //1st tile
                    if (gotTiles[tile0] >= possibleTiles[tile0]) continue;  //如果這張數字牌的已取Tile數已到達上限，要跳過這個數字牌

                    tile[0] = tile0;
                    gotTiles[tile0]++;

                    for (int tile1 = tile0; tile1 < 11; tile1++)
                    {
                        //2nd tile
                        if (gotTiles[tile1] >= possibleTiles[tile1]) continue;  //如果這張數字牌的已取Tile數已到達上限，要跳過這個數字牌

                        tile[1] = tile1;
                        gotTiles[tile1]++;

                        for (int tile2 = tile1; tile2 < 11; tile2++)
                        {
                            //3rd tile
                            if (gotTiles[tile2] >= possibleTiles[tile2]) continue;  //如果這張數字牌的已取Tile數已到達上限，要跳過這個數字牌

                            tile[2] = tile2;
                            gotTiles[tile2]++;

                            //INSERT SetString
                            string setString = "";

                            for (int x = 0; x < 3; x++)
                            {
                                switch (tile[x])
                                {
                                    case 0: setString = setString + "1G"; break;
                                    case 1: setString = setString + "2Y"; break;
                                    case 2: setString = setString + "3K"; break;
                                    case 3: setString = setString + "4B"; break;
                                    case 4: setString = setString + "5R"; break;
                                    case 5: setString = setString + "5K"; break;
                                    case 6: setString = setString + "6P"; break;
                                    case 7: setString = setString + "6G"; break;
                                    case 8: setString = setString + "7Y"; break;
                                    case 9: setString = setString + "7P"; break;
                                    case 10: setString = setString + "7C"; break;
                                }
                                if (x != 2)
                                    setString = setString + "-";
                            }

                            //compare1>compare2且compareKey1為true、 compare1<compare2且compareKey1為false、 compare1==compare2且compareKey1、compareKey2均為false
                            conditionCheck = ((compareTileKey1 + gotTiles[0] + gotTiles[7] > compareTileKey2 + gotTiles[1] + gotTiles[8]) && compareKey1) ||
                                                        ((compareTileKey1 + gotTiles[0] + gotTiles[7] < compareTileKey2 + gotTiles[1] + gotTiles[8]) && compareKey2) ||
                                                        ((compareTileKey1 + gotTiles[0] + gotTiles[7] == compareTileKey2 + gotTiles[1] + gotTiles[8]) && !compareKey1 && !compareKey2);

                            //conditionCheck需要為True，並原本參考中的可能性裡存在的組合，才會被保留下來
                            if (conditionCheck && _possibleSet.Contains(setString)) possibleSet.Add(setString);

                            gotTiles[tile2]--;  //TILE用完記得歸還
                        }
                        gotTiles[tile1]--;  //TILE用完記得歸還
                    }
                    gotTiles[tile0]--;  //TILE用完記得歸還
                }
                #endregion
                break;

        }

        _possibleSet = possibleSet;
        _possibleCount.text = _possibleSet.Count.ToString();
        _solution = _possibleSet.Count <= 1 ? true : false;

        //過濾自己可能拿取的最大數量
        TileInfer assignPossibleSet = new TileInfer(false); //一進場給予最大可能數值，一旦可能數量減少就會遞減，不會再增加
        int[] assignPossible;
        for ( int i=0; i<_possibleSet.Count; i++ )
        {
            assignPossible = new int[11];
            string[] splitTiles = _possibleSet[i].Split('-');
            for(int j=0;j<splitTiles.Length;j++)
            {
                switch (splitTiles[j])
                {
                    case "1G": assignPossible[0]++;  break;
                    case "2Y": assignPossible[1]++; break;
                    case "3K": assignPossible[2]++; break;
                    case "4B": assignPossible[3]++; break;
                    case "5R": assignPossible[4]++; break;
                    case "5K": assignPossible[5]++; break;
                    case "6P": assignPossible[6]++; break;
                    case "6G": assignPossible[7]++; break;
                    case "7Y": assignPossible[8]++; break;
                    case "7P": assignPossible[9]++; break;
                    case "7C": assignPossible[10]++; break;
                }
            }

            if (assignPossible[0] > assignPossibleSet.G1) assignPossibleSet.G1 = assignPossible[0];
            if (assignPossible[1] > assignPossibleSet.Y2) assignPossibleSet.Y2 = assignPossible[1];
            if (assignPossible[2] > assignPossibleSet.K3) assignPossibleSet.K3 = assignPossible[2];
            if (assignPossible[3] > assignPossibleSet.B4) assignPossibleSet.B4 = assignPossible[3];
            if (assignPossible[4] > assignPossibleSet.R5) assignPossibleSet.R5 = assignPossible[4];
            if (assignPossible[5] > assignPossibleSet.K5) assignPossibleSet.K5 = assignPossible[5];
            if (assignPossible[6] > assignPossibleSet.P6) assignPossibleSet.P6 = assignPossible[6];
            if (assignPossible[7] > assignPossibleSet.G6) assignPossibleSet.G6 = assignPossible[7];
            if (assignPossible[8] > assignPossibleSet.Y7) assignPossibleSet.Y7 = assignPossible[8];
            if (assignPossible[9] > assignPossibleSet.P7) assignPossibleSet.P7 = assignPossible[9];
            if (assignPossible[10] > assignPossibleSet.C7) assignPossibleSet.C7 = assignPossible[10];
        }

        if (assignPossibleSet.G1 < _possibleNumber.G1) _possibleNumber.G1 = assignPossibleSet.G1;
        if (assignPossibleSet.Y2 < _possibleNumber.Y2) _possibleNumber.Y2 = assignPossibleSet.Y2;
        if (assignPossibleSet.K3 < _possibleNumber.K3) _possibleNumber.K3 = assignPossibleSet.K3;
        if (assignPossibleSet.B4 < _possibleNumber.B4) _possibleNumber.B4 = assignPossibleSet.B4;
        if (assignPossibleSet.R5 < _possibleNumber.R5) _possibleNumber.R5 = assignPossibleSet.R5;
        if (assignPossibleSet.K5 < _possibleNumber.K5) _possibleNumber.K5 = assignPossibleSet.K5;
        if (assignPossibleSet.P6 < _possibleNumber.P6) _possibleNumber.P6 = assignPossibleSet.P6;
        if (assignPossibleSet.G6 < _possibleNumber.G6) _possibleNumber.G6 = assignPossibleSet.G6;
        if (assignPossibleSet.Y7 < _possibleNumber.Y7) _possibleNumber.Y7 = assignPossibleSet.Y7;
        if (assignPossibleSet.P7 < _possibleNumber.P7) _possibleNumber.P7 = assignPossibleSet.P7;
        if (assignPossibleSet.C7 < _possibleNumber.C7) _possibleNumber.C7 = assignPossibleSet.C7;

    }

    public void GetPoint()
    {
        _victoryLight[_victory>2?2:_victory].GetComponent<Image>().color = new Color(1F, 1F, 1F, 1F);
        _victory++;
    }

    #endregion
}
