using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Rack
{
    //RACK指的是牌架，表示牌架上的整個三張牌
    //在做情報判斷時，會有需要判斷整個牌架的情況

    #region 欄位宣告

    private Tile[] _tiles = new Tile[3];  //紀錄牌架上tile，固定為3個

    private bool _reachEighteen;            //牌架問題1，你看到幾個牌架上的總和大於等於18
    private bool _untilTwelve;              //牌架問題2，你看到幾個牌架上的總和小於等於12
    private bool _sameNumber;            //牌架問題3，你看到幾個牌架上有不同顏色的相同數字
    private bool _threeColor;               //牌架問題4，你看到幾個牌架上有三個不同顏色
    private bool _allOddEven;               //牌架問題5，你看到幾個牌架上三個數字均是奇數或均是偶數
    private bool _sameColorNumber;   //牌架問題6，你看到幾個牌架上有相同顏色的相同數字
    private bool _consecutiveNumber;    //牌架問題7，你看到幾個牌架上有連續的數字

    private string _numberMatch;     //用以確認玩家的回答是否正確
    private string _numberColorMatch;     //用以確認玩家的回答是否正確(進階版，需要顏色)

    #endregion

    #region 屬性設定

    public Tile[] tiles { get { return _tiles; } }

    public bool reachEighteen { get { return _reachEighteen; } }
    public bool untilTwelve { get { return _untilTwelve; } }
    public bool sameNumber { get { return _sameNumber; } }
    public bool threeColor { get { return _threeColor; } }
    public bool allOddEven { get { return _allOddEven; } }
    public bool sameColorNumber { get { return _sameColorNumber; } }
    public bool consecutiveNumber { get { return _consecutiveNumber; } }

    public string numberMatch { get { return _numberMatch; } }
    public string numberColorMatch { get { return _numberColorMatch; } }

    #endregion

    #region 建構方法

    public Rack(Tile tile1, Tile tile2, Tile tile3)
    {
        _tiles[0] = tile1;
        _tiles[1] = tile2;
        _tiles[2] = tile3;

        int[] numbers = { _tiles[0].number, _tiles[1].number, _tiles[2].number };
        string[] colors = { _tiles[0].color, _tiles[1].color, _tiles[2].color };

        //答案檢核機能
        List<string> tilesString = new List<string>(3);

        tilesString.Add(numbers[0].ToString());
        tilesString.Add(numbers[1].ToString());
        tilesString.Add(numbers[2].ToString());
        tilesString.Sort();
        _numberMatch = tilesString[0] + tilesString[1] + tilesString[2];

        tilesString.Clear();
        tilesString.Add(numbers[0].ToString()+colors[0]);
        tilesString.Add(numbers[1].ToString() + colors[1]);
        tilesString.Add(numbers[2].ToString() + colors[2]);
        tilesString.Sort();
        _numberColorMatch = tilesString[0] + tilesString[1] + tilesString[2];

        //牌架問題1，你看到幾個牌架上的總和大於等於18
        _reachEighteen = (numbers[0] + numbers[1] + numbers[2] >= 18) ? true : false ;

        //牌架問題2，你看到幾個牌架上的總和小於等於12
        _untilTwelve = (numbers[0] + numbers[1] + numbers[2] <= 12) ? true : false;

        //牌架問題3，你看到幾個牌架上有不同顏色的相同數字
        //牌架問題6，你看到幾個牌架上有相同顏色的相同數字
        if (numbers[0]!=numbers[1] && numbers[1]!=numbers[2] && numbers[0] != numbers[2])
        {
            //三個數字均不相同
            _sameNumber = false;
            _sameColorNumber = false;
        }
        else
        {
            int differentColorCount = 0;
            int sameColorCount = 0;
            if(numbers[0] == numbers[1])
            {
                differentColorCount += colors[0] != colors[1] ? 1 : 0;
                sameColorCount += colors[0] == colors[1] ? 1 : 0;
            }
            if (numbers[1] == numbers[2])
            {
                differentColorCount += colors[1] != colors[2] ? 1 : 0;
                sameColorCount += colors[1] == colors[2] ? 1 : 0;
            }
            if (numbers[0] == numbers[2])
            {
                differentColorCount += colors[0] != colors[2] ? 1 : 0;
                sameColorCount += colors[0] == colors[2] ? 1 : 0;
            }

            _sameNumber = differentColorCount > 0 ? true : false;
            _sameColorNumber = sameColorCount > 0 ? true : false;
        }

        //牌架問題4，你看到幾個牌架上有三個不同顏色
        _threeColor = (colors[0] != colors[1] && colors[1] != colors[2] && colors[0] != colors[2]);

        //牌架問題5，你看到幾個牌架上三個數字均是奇數或均是偶數
        _allOddEven =( (numbers[0]%2==numbers[1]%2) && (numbers[0]%2 == numbers[2]%2));

        //牌架問題7，你看到幾個牌架上有連續的數字
        _consecutiveNumber = (_numberMatch == "123" || _numberMatch == "234" || _numberMatch == "345"
                                                                                    || _numberMatch == "456" || _numberMatch == "567");
    }

    #endregion
}
