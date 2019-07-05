using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum GameStatus { Idle, NextStep, NumberCall, PlayerCall, SecondJudge }

public class Code777Manager : MonoBehaviour
{
    public Sprite[] tileImages;
    int drawPlayer; //抽Player亂數
    int drawTile;   //抽TILE亂數
    int drawCard; //抽Card亂數

    #region 初始化玩家

    public List<string> presetPlayerName;
    public List<Sprite> presetPlayerIcon;

    #endregion

    #region 初始化TILE堆
    public static Tile[] initialTile = new Tile[28];

    #endregion

    //public GameObject[] playerIcon;
    //public TextMesh[] playerName;
    public GameObject[] playerIcon;
    public Text[] playerName;
    public Text[] possibleCount;

    public List<Tile> tilePile = new List<Tile>(28);    //未打開的Tile堆
    public List<Tile> setTile = new List<Tile>(3); //設置中的Tile暫存器
    public List<Tile> discardTile = new List<Tile>(28); //使用過被棄置於場中央的Tile堆
    public List<int> questionCard = new List<int>(23);//用來給予情報的問題卡

    public bool assistMode = true;  //輔助模式，此模式開啟下會於場景右下角提示可能TILE
    //public GameObject[] assistTile;
    public Image[] assistTile;

    public List<Player> activePlayer;

    public int answerPlayer;

    public Text questionText;
    public Text answerText;
    public Text speakerText;

    public Button startButton;
    public Button callButton;
    public Button passButton;

    //操控控制項
    public static bool answerCall = false;
    public static GameStatus status = GameStatus.Idle;

    //遊戲的狀態

    //Idle：讓遊戲處於閒置狀態，不會任意在畫面演示任何動作

    //NextStep：進入觸發方法後會切回Idle。
    //按下按鈕「Start」「Continue」時會進入，
    //會將「CALL」「PASS」按鈕隱藏
    //使遊戲畫面演示一次問題卡回答。
    //結束後顯示「CALL」「PASS」按鈕

    //NumberCall：進入觸發方法後會切回Idle。
    //按下「CALL」或「PASS」時會進入
    //如果沒有玩家要呼叫數字，執行同NextStep狀態時的操作
    //在有玩家要呼叫數字的情況，
    //讓電腦玩家執行一次呼叫數字及刷新牌架的操作
    //操作結束後，如果還有其他玩家要叫用數字，會切換回NumberCall狀態，否則進入SecondJudge狀態

    //PlayerCall：輪到玩家呼叫的情況會進入此狀態。此狀態設定為玩家可以操作畫面的設定。
    //當玩家呼叫完數字，並更新牌架。更新牌架前，先暫時切為Idle狀態
    //如果還有其他玩家要叫用數字，會切換回NumberCall狀態，否則進入SecondJudge狀態

    //SecondJudge：進入觸發方法後會切回Idle。
    //在同時呼叫數字的一輪中，如果有玩家根據其他玩家的牌架刷新而有了新的情報，使之可以猜測數字時
    //不能當下馬上宣告要呼叫數字(因為其他玩家的呼叫數字仍在連鎖中處理)
    //而是在此狀態下做二次判定，進入新的一輪NumberCall的狀態。
    //結束後顯示「CALL」「PASS」按鈕

    //玩家是否要叫用數字，以布林值playerCall來判斷
    public static bool playerCall = false;    //玩家如果要呼叫數字，則為true；否則為false

    // Start is called before the first frame update
    void Start()
    {
        questionText.text = "";
        answerText.text = "";
        speakerText.text = "";

        #region  初始化TILE堆
        initialTile[0] = new Tile(1, "G", tileImages[0]);
        initialTile[1] = new Tile(2, "Y", tileImages[1]);
        initialTile[2] = new Tile(2, "Y", tileImages[1]);
        initialTile[3] = new Tile(3, "K", tileImages[2]);
        initialTile[4] = new Tile(3, "K", tileImages[2]);
        initialTile[5] = new Tile(3, "K", tileImages[2]);
        initialTile[6] = new Tile(4, "B", tileImages[3]);
        initialTile[7] = new Tile(4, "B", tileImages[3]);
        initialTile[8] = new Tile(4, "B", tileImages[3]);
        initialTile[9] = new Tile(4, "B", tileImages[3]);
        initialTile[10] = new Tile(5, "R", tileImages[4]);
        initialTile[11] = new Tile(5, "R", tileImages[4]);
        initialTile[12] = new Tile(5, "R", tileImages[4]);
        initialTile[13] = new Tile(5, "R", tileImages[4]);
        initialTile[14] = new Tile(5, "K", tileImages[5]);
        initialTile[15] = new Tile(6, "P", tileImages[6]);
        initialTile[16] = new Tile(6, "P", tileImages[6]);
        initialTile[17] = new Tile(6, "P", tileImages[6]);
        initialTile[18] = new Tile(6, "G", tileImages[7]);
        initialTile[19] = new Tile(6, "G", tileImages[7]);
        initialTile[20] = new Tile(6, "G", tileImages[7]);
        initialTile[21] = new Tile(7, "Y", tileImages[8]);
        initialTile[22] = new Tile(7, "Y", tileImages[8]);
        initialTile[23] = new Tile(7, "P", tileImages[9]);
        initialTile[24] = new Tile(7, "C", tileImages[10]);
        initialTile[25] = new Tile(7, "C", tileImages[10]);
        initialTile[26] = new Tile(7, "C", tileImages[10]);
        initialTile[27] = new Tile(7, "C", tileImages[10]);

        for (int i = 0; i < 28; i++)
        {
            tilePile.Add(initialTile[i]);
        }

        #endregion

        #region 初始化玩家

        for ( int i=0; i<5; i++ )
        {
            drawPlayer = Random.Range(0, presetPlayerName.Count);

            string activePlayerName = presetPlayerName[drawPlayer];
            Sprite activePlayerIcon = presetPlayerIcon[drawPlayer];
            string[] tileSprite = new string[] { "Player" + i + "Tile0", "Player" + i + "Tile1", "Player" + i + "Tile2" };
            string[] victoryLight = new string[] { "Player" + i + "Victory0", "Player" + i + "Victory1", "Player" + i + "Victory2" };

            playerIcon[i].GetComponent<Image>().sprite = activePlayerIcon;
            playerName[i].text = activePlayerName;

            Player newPlayer = new Player(activePlayerName, activePlayerIcon, i, 0, tileSprite, victoryLight, possibleCount[i]);

            activePlayer.Add(newPlayer);

            presetPlayerName.Remove(activePlayerName);
            presetPlayerIcon.Remove(activePlayerIcon);

        }

        #endregion

        #region 初始化問題卡
        for(int i=1; i<=23; i++)
        {
            questionCard.Add(i);
        }
        #endregion

        #region 初始化RACKS
        for (int i=0; i<5; i++)
        {
            Tile tile0, tile1, tile2;

            drawTile = Random.Range(0, tilePile.Count);
            tile0 = new Tile( tilePile[drawTile].number, tilePile[drawTile].color, tilePile[drawTile].image);   //宣告Tile0
            tilePile.Remove(tilePile[drawTile]);

            drawTile = Random.Range(0, tilePile.Count);
            tile1 = new Tile(tilePile[drawTile].number, tilePile[drawTile].color, tilePile[drawTile].image);   //宣告Tile1
            tilePile.Remove(tilePile[drawTile]);

            drawTile = Random.Range(0, tilePile.Count);
            tile2 = new Tile(tilePile[drawTile].number, tilePile[drawTile].color, tilePile[drawTile].image);   //宣告Tile2
            tilePile.Remove(tilePile[drawTile]);

            activePlayer[i].NewRack(tile0, tile1, tile2);
        }
        #endregion

        #region 牌架過濾

        activePlayer[0].RackCheck(activePlayer, discardTile);
        activePlayer[1].RackCheck(activePlayer, discardTile);
        activePlayer[2].RackCheck(activePlayer, discardTile);
        activePlayer[3].RackCheck(activePlayer, discardTile);
        activePlayer[4].RackCheck(activePlayer, discardTile);

        //玩家0的輔助模式
        activePlayer[0].TileLight(assistMode);

        #endregion

        #region 隨機決定起始玩家
        answerPlayer = Random.Range(0, 5);
        #endregion

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.anyKeyDown)
        {
            if (Input.GetKeyDown(KeyCode.F2))
            {
                //Debug.Log("PRESS SPACE");
                UnityEngine.SceneManagement.SceneManager.LoadScene("Code777GamePlay");
            }
            //if (spaceKeyLock && Input.GetKeyDown(KeyCode.Space))
            //{
            //    //Debug.Log("PRESS SPACE");
            //    //UnityEngine.SceneManagement.SceneManager.LoadScene("Code777GamePlay");
            //    spaceKeyLock = false;   //鎖死空白鍵
            //    drawCard = Random.Range(0, questionCard.Count);
            //    int cardNum = questionCard[drawCard];
            //    questionCard.Remove(cardNum);
            //    StartCoroutine(AnswerCard( cardNum, activePlayer ));
            //}
        }

        if (status == GameStatus.NextStep)
        {
            //新回合的進入點
            status = GameStatus.Idle;
            answerPlayer += (answerPlayer == 4) ? (-4) : 1;
            drawCard = Random.Range(0, questionCard.Count);
            int cardNum = questionCard[drawCard];
            questionCard.Remove(cardNum);
            StartCoroutine(AnswerCard(cardNum, activePlayer));
        }

        if (status == GameStatus.NumberCall)
        {
            //新回合的進入點前
            status = GameStatus.Idle;

            if( answerCall || playerCall )
            {
                //有玩家要叫用數字時，執行叫用數字的部分
                //answerCall的邏輯判定為，各個玩家內的欄位Solution只要有一個人為True，該欄位就為True
                //但是對0號玩家不採用Solution的判定。而是根據選擇CALL或PASS決定是否要叫用

                string logMessage = "";
                logMessage += playerCall ? activePlayer[0].name + " " : "";
                logMessage += activePlayer[1].solution ? activePlayer[1].name + " " : "";
                logMessage += activePlayer[2].solution ? activePlayer[2].name + " " : "";
                logMessage += activePlayer[3].solution ? activePlayer[3].name + " " : "";
                logMessage += activePlayer[4].solution ? activePlayer[4].name + " " : "";
                Debug.Log(logMessage+"已經可以叫用數字，但邏輯尚未完成，故先同一般操作處理");
                //目前因為邏輯尚未完成，執行如同一般操作

                answerPlayer += (answerPlayer == 4) ? (-4) : 1;
                drawCard = Random.Range(0, questionCard.Count);
                int cardNum = questionCard[drawCard];
                questionCard.Remove(cardNum);
                StartCoroutine(AnswerCard(cardNum, activePlayer));
            }
            else
            {
                //沒有玩家要叫用數字，執行如同一般操作
                answerPlayer += (answerPlayer == 4) ? (-4) : 1;
                drawCard = Random.Range(0, questionCard.Count);
                int cardNum = questionCard[drawCard];
                questionCard.Remove(cardNum);
                StartCoroutine(AnswerCard(cardNum, activePlayer));
            }
        }
    }

    IEnumerator AnswerCard(int cardId, List<Player> players)
    {
        //隱藏CALL PASS按鈕
        callButton.gameObject.SetActive(false);
        passButton.gameObject.SetActive(false);
        for (int i=0; i<players.Count; i++)
        {
            if(i== answerPlayer)
            {
                activePlayer[i].BecomeAnswerPlayer();
            }
            else
            {
                activePlayer[i].NolongerAnswerPlayer();
            }
        }
        questionText.text = "";
        answerText.text = "";
        speakerText.text = "這個問題由 <color=#FFC000>"+activePlayer[answerPlayer].name+"</color> 回答" ;
        yield return new WaitForSeconds(1);
        //顯示問題卡
        switch (cardId)
        {
            case 1:
                questionText.text = cardId+". 有多少個牌架上的牌，三個數字總和是<b>18以上</b>？";
                break;
            case 2:
                questionText.text = cardId + ". 有多少個牌架上的牌，三個數字總和是<b>12以下</b>？";
                break;
            case 3:
                questionText.text = cardId + ". 有多少個牌架上的牌，出現<b>不同顏色的相同數字</b>？";
                break;
            case 4:
                questionText.text = cardId + ". 有多少個牌架上的牌，出現三個<b>不同顏色</b>的數字？";
                break;
            case 5:
                questionText.text = cardId + ". 有多少個牌架上的牌，三個數字<b>皆是奇數</b>或<b>皆是偶數</b>？";
                break;
            case 6:
                questionText.text = cardId + ". 有多少個牌架上的牌，出現<b>相同顏色的相同數字</b>？";
                break;
            case 7:
                questionText.text = cardId + ". 有多少個牌架上的牌，三個數字是<b>連續的數字</b>？";
                break;
            case 8:
                questionText.text = cardId + ". 你看到<b>多少種顏色</b>的數字牌？";
                break;
            case 9:
                questionText.text = cardId + ". 有幾種<b>顏色</b>出現了<b>三次以上</b>？" ;
                break;
            case 10:
                questionText.text = cardId + ". 有幾種數字<b>完全沒有出現</b>？";
                break;
            case 11:
                questionText.text = cardId + ". <b><color=#009960>綠1</color>、<color=#808080>黑5</color>、<color=#D91ACC>粉紅7</color></b>這三種牌，你總共看到幾張？";
                break;
            case 12:
                questionText.text = cardId + ". <b><color=#808080>黑3</color>和<color=#D91ACC>粉紅6</color></b>，何者較多？";
                break;
            case 13:
                questionText.text = cardId + ". <b><color=#009960>綠6</color>和<color=#F2CC00>黃7</color></b>，何者較多？";
                break;
            case 14:
                questionText.text = cardId + ". <b><color=#F2CC00>黃2</color>和<color=#F2CC00>黃7</color></b>，何者較多？";
                break;
            case 15:
                questionText.text = cardId + ". <b><color=#D91ACC>粉紅6</color>和<color=#009960>綠6</color></b>，何者較多？";
                break;
            case 16:
                questionText.text = cardId + ". <b><color=#0059FF>藍7</color>和其他顏色的7</b>，何者較多？";
                break;
            case 17:
                questionText.text = cardId + ". <b><color=#A64C26>棕色</color>和<color=#0059FF>藍色</color></b>，何者較多？";
                break;
            case 18:
                questionText.text = cardId + ". <b><color=#D90000>紅色</color>和<color=#D91ACC>粉紅色</color></b>，何者較多？";
                break;
            case 19:
                questionText.text = cardId + ". <b><color=#009960>綠色</color>和<color=#0059FF>藍色</color></b>，何者較多？";
                break;
            case 20:
                questionText.text = cardId + ". <b><color=#F2CC00>黃色</color>和<color=#D91ACC>粉紅色</color></b>，何者較多？";
                break;
            case 21:
                questionText.text = cardId + ". <b><color=#808080>黑色</color>和<color=#A64C26>棕色</color></b>，何者較多？";
                break;
            case 22:
                questionText.text = cardId + ". <b><color=#808080>黑色</color>和<color=#D90000>紅色</color></b>，何者較多？";
                break;
            case 23:
                questionText.text = cardId + ". <b><color=#009960>綠色</color>和<color=#F2CC00>黃色</color></b>，何者較多？";
                break;
        }
        yield return new WaitForSeconds(2);

        int answerKey = 0;
        int compareKey1 = 0;
        int compareKey2 = 0;
        int[] numberColorKey = new int[7];

        //回答問題答案
        switch (cardId)
        {
            case 1:
                // 有多少個牌架上的牌，三個數字總和是<b>18以上</b>？";
                for (int i=0; i<players.Count;i++)
                {
                    if (answerPlayer != i)
                        answerKey += activePlayer[i].rack.reachEighteen ? 1 : 0;
                }
                answerText.text = answerKey + "組";
                break;
            case 2:
                // 有多少個牌架上的牌，三個數字總和是<b>12以下</b>？";
                for (int i = 0; i < players.Count; i++)
                {
                    if (answerPlayer != i)
                        answerKey += activePlayer[i].rack.untilTwelve ? 1 : 0;
                }
                answerText.text = answerKey + "組";
                break;
            case 3:
                // 有多少個牌架上的牌，出現<b>不同顏色的相同數字</b>？";
                for (int i = 0; i < players.Count; i++)
                {
                    if (answerPlayer != i)
                        answerKey += activePlayer[i].rack.sameNumberDifColor ? 1 : 0;
                }
                answerText.text = answerKey + "組";
                break;
            case 4:
                //有多少個牌架上的牌，出現三個<b>不同顏色</b>的數字？";
                for (int i = 0; i < players.Count; i++)
                {
                    if (answerPlayer != i)
                        answerKey += activePlayer[i].rack.threeColor ? 1 : 0;
                }
                answerText.text = answerKey + "組";
                break;
            case 5:
                //有多少個牌架上的牌，三個數字<b>皆是奇數</b>或<b>皆是偶數</b>？";
                for (int i = 0; i < players.Count; i++)
                {
                    if (answerPlayer != i)
                        answerKey += activePlayer[i].rack.allOddEven ? 1 : 0;
                }
                answerText.text = answerKey + "組";
                break;
            case 6:
                //有多少個牌架上的牌，出現<b>相同顏色的相同數字</b>？";
                for (int i = 0; i < players.Count; i++)
                {
                    if (answerPlayer != i)
                        answerKey += activePlayer[i].rack.sameColorNumber ? 1 : 0;
                }
                answerText.text = answerKey + "組";
                break;
            case 7:
                //有多少個牌架上的牌，三個數字是<b>連續的數字</b>？";
                for (int i = 0; i < players.Count; i++)
                {
                    if (answerPlayer != i)
                        answerKey += activePlayer[i].rack.consecutiveNumber ? 1 : 0;
                }
                answerText.text = answerKey + "組";
                break;
            case 8:
                //你看到<b>多少種顏色</b>的數字牌？";
                //顏色順序 綠、黃、黑、棕、紅、粉、藍
                for (int i = 0; i < players.Count; i++)
                {
                    if (answerPlayer != i)
                    {
                        for (int j = 0; j < 3; j++)
                        {
                            switch( activePlayer[i].rack.tiles[j].color )
                            {
                                case "G":   numberColorKey[0]++;    break;
                                case "Y":   numberColorKey[1]++;    break;
                                case "K":   numberColorKey[2]++;    break;
                                case "B":   numberColorKey[3]++;    break;
                                case "R":   numberColorKey[4]++;    break;
                                case "P":   numberColorKey[5]++;    break;
                                case "C":   numberColorKey[6]++;    break;
                            }
                        }
                    }
                }
                for (int i = 0; i < 7; i++)
                {
                    if (numberColorKey[i] > 0)
                        answerKey++;
                }
                answerText.text = answerKey + "種";
                break;
            case 9:
                // 有幾種<b>顏色</b>出現了<b>三次以上</b>？";
                //顏色順序 綠、黃、黑、棕、紅、粉、藍
                for (int i = 0; i < players.Count; i++)
                {
                    if (answerPlayer != i)
                    {
                        for (int j = 0; j < 3; j++)
                        {
                            switch (activePlayer[i].rack.tiles[j].color)
                            {
                                case "G": numberColorKey[0]++; break;
                                case "Y": numberColorKey[1]++; break;
                                case "K": numberColorKey[2]++; break;
                                case "B": numberColorKey[3]++; break;
                                case "R": numberColorKey[4]++; break;
                                case "P": numberColorKey[5]++; break;
                                case "C": numberColorKey[6]++; break;
                            }
                        }
                    }
                }
                for (int i = 0; i < 7; i++)
                {
                    if (numberColorKey[i] >= 3)
                        answerKey++;
                }
                answerText.text = answerKey + "種";
                break;
            case 10:
                //有幾種數字<b>完全沒有出現</b>？";
                for (int i = 0; i < players.Count; i++)
                {
                    if (answerPlayer != i)
                    {
                        for (int j = 0; j < 3; j++)
                        {
                            numberColorKey[activePlayer[i].rack.tiles[j].number - 1]++;
                        }
                    }
                }
                for (int i = 0; i < 7; i++)
                {
                    if (numberColorKey[i] == 0)
                        answerKey++;
                }
                answerText.text = answerKey + "種";
                break;
            case 11:
                //<b><color=#009960>綠1</color>、<color=#808080>黑5</color>、<color=#D91ACC>粉紅7</color></b>這三種牌，你總共看到幾張？";
                for (int i = 0; i < players.Count; i++)
                {
                    if (answerPlayer != i)
                    {
                        for (int j = 0; j < 3; j++)
                        {
                            if (players[i].rack.tiles[j].number == 1 && players[i].rack.tiles[j].color == "G") answerKey++;
                            if (players[i].rack.tiles[j].number == 5 && players[i].rack.tiles[j].color == "K") answerKey++;
                            if (players[i].rack.tiles[j].number == 7 && players[i].rack.tiles[j].color == "P") answerKey++;
                        }
                    }
                }
                answerText.text = answerKey + "張";
                break;
            case 12:
                //<b><color=#808080>黑3</color>和<color=#D91ACC>粉紅6</color></b>，何者較多？";
                for (int i = 0; i < players.Count; i++)
                {
                    if (answerPlayer != i)
                    {
                        for (int j = 0; j < 3; j++)
                        {
                            if (players[i].rack.tiles[j].number == 3 && players[i].rack.tiles[j].color == "K") compareKey1++;
                            if (players[i].rack.tiles[j].number == 6 && players[i].rack.tiles[j].color == "P") compareKey2++;
                        }
                    }
                }
                answerText.text = (compareKey1 > compareKey2) ? "<b><color=#808080>黑3</color></b>" :
                                             ((compareKey1 < compareKey2) ? "<b><color=#D91ACC>粉紅6</color></b>" : "<b>一樣多</b>");
                break;
            case 13:
                //<b><color=#009960>綠6</color>和<color=#F2CC00>黃7</color></b>，何者較多？";
                for (int i = 0; i < players.Count; i++)
                {
                    if (answerPlayer != i)
                    {
                        for (int j = 0; j < 3; j++)
                        {
                            if (players[i].rack.tiles[j].number == 6 && players[i].rack.tiles[j].color == "G") compareKey1++;
                            if (players[i].rack.tiles[j].number == 7 && players[i].rack.tiles[j].color == "Y") compareKey2++;
                        }
                    }
                }
                answerText.text = (compareKey1 > compareKey2) ? "<b><color=#009960>綠6</color></b>" :
                                             ((compareKey1 < compareKey2) ? "<b><color=#F2CC00>黃7</color></b>" : "<b>一樣多</b>");
                break;
            case 14:
                //<b><color=#F2CC00>黃2</color>和<color=#F2CC00>黃7</color></b>，何者較多？";
                for (int i = 0; i < players.Count; i++)
                {
                    if (answerPlayer != i)
                    {
                        for (int j = 0; j < 3; j++)
                        {
                            if (players[i].rack.tiles[j].number == 2 && players[i].rack.tiles[j].color == "Y") compareKey1++;
                            if (players[i].rack.tiles[j].number == 7 && players[i].rack.tiles[j].color == "Y") compareKey2++;
                        }
                    }
                }
                answerText.text = (compareKey1 > compareKey2) ? "<b><color=#F2CC00>黃2</color></b>" :
                                             ((compareKey1 < compareKey2) ? "<b><color=#F2CC00>黃7</color></b>" : "<b>一樣多</b>");
                break;
            case 15:
                //<b><color=#D91ACC>粉紅6</color>和<color=#009960>綠6</color></b>，何者較多？";
                for (int i = 0; i < players.Count; i++)
                {
                    if (answerPlayer != i)
                    {
                        for (int j = 0; j < 3; j++)
                        {
                            if (players[i].rack.tiles[j].number == 6 && players[i].rack.tiles[j].color == "P") compareKey1++;
                            if (players[i].rack.tiles[j].number == 6 && players[i].rack.tiles[j].color == "G") compareKey2++;
                        }
                    }
                }
                answerText.text = (compareKey1 > compareKey2) ? "<b><color=#D91ACC>粉紅6</color></b>" :
                                             ((compareKey1 < compareKey2) ? "<b><color=#009960>綠6</color></b>" : "<b>一樣多</b>");
                break;
            case 16:
                //<b><color=#0059FF>藍7</color>和其他顏色的7</b>，何者較多？";
                for (int i = 0; i < players.Count; i++)
                {
                    if (answerPlayer != i)
                    {
                        for (int j = 0; j < 3; j++)
                        {
                            if (players[i].rack.tiles[j].number == 7 && players[i].rack.tiles[j].color == "C") compareKey1++;
                            if (players[i].rack.tiles[j].number == 7 && players[i].rack.tiles[j].color != "C") compareKey2++;
                        }
                    }
                }
                answerText.text = (compareKey1> compareKey2) ? "<b><color=#0059FF>藍7</color></b>" :
                                             ((compareKey1 < compareKey2) ? "<b><color=#C0C0C0>其他顏色7</color></b>" : "<b>一樣多</b>");
                break;
            case 17:
                //<b><color=#A64C26>棕色</color>和<color=#0059FF>藍色</color></b>，何者較多？";
                for (int i = 0; i < players.Count; i++)
                {
                    if (answerPlayer != i)
                    {
                        for (int j = 0; j < 3; j++)
                        {
                            if (players[i].rack.tiles[j].color == "B") compareKey1++;
                            if (players[i].rack.tiles[j].color == "C") compareKey2++;
                        }
                    }
                }
                answerText.text = (compareKey1 > compareKey2) ? "<b><color=#A64C26>棕色</color></b>" :
                                             ((compareKey1 < compareKey2) ? "<b><color=#0059FF>藍色</color></b>" : "<b>一樣多</b>");
                break;
            case 18:
                //<b><color=#D90000>紅色</color>和<color=#D91ACC>粉紅色</color></b>，何者較多？";
                for (int i = 0; i < players.Count; i++)
                {
                    if (answerPlayer != i)
                    {
                        for (int j = 0; j < 3; j++)
                        {
                            if (players[i].rack.tiles[j].color == "R") compareKey1++;
                            if (players[i].rack.tiles[j].color == "P") compareKey2++;
                        }
                    }
                }
                answerText.text = (compareKey1 > compareKey2) ? "<b><color=#D90000>紅色</color></b>" :
                                             ((compareKey1 < compareKey2) ? "<b><color=#D91ACC>粉紅色</color></b>" : "<b>一樣多</b>");
                break;
            case 19:
                //<b><color=#009960>綠色</color>和<color=#0059FF>藍色</color></b>，何者較多？";
                for (int i = 0; i < players.Count; i++)
                {
                    if (answerPlayer != i)
                    {
                        for (int j = 0; j < 3; j++)
                        {
                            if (players[i].rack.tiles[j].color == "G") compareKey1++;
                            if (players[i].rack.tiles[j].color == "C") compareKey2++;
                        }
                    }
                }
                answerText.text = (compareKey1 > compareKey2) ? "<b><color=#009960>綠色</color></b>" :
                                             ((compareKey1 < compareKey2) ? "<b><color=#0059FF>藍色</color></b>" : "<b>一樣多</b>");
                break;
            case 20:
                //<b><color=#F2CC00>黃色</color>和<color=#D91ACC>粉紅色</color></b>，何者較多？";
                for (int i = 0; i < players.Count; i++)
                {
                    if (answerPlayer != i)
                    {
                        for (int j = 0; j < 3; j++)
                        {
                            if (players[i].rack.tiles[j].color == "Y") compareKey1++;
                            if (players[i].rack.tiles[j].color == "P") compareKey2++;
                        }
                    }
                }
                answerText.text = (compareKey1 > compareKey2) ? "<b><color=#F2CC00>黃色</color></b>" :
                                             ((compareKey1 < compareKey2) ? "<b><color=#D91ACC>粉紅色</color></b>" : "<b>一樣多</b>");
                break;
            case 21:
                //<b><color=#808080>黑色</color>和<color=#A64C26>棕色</color></b>，何者較多？";
                for (int i = 0; i < players.Count; i++)
                {
                    if (answerPlayer != i)
                    {
                        for (int j = 0; j < 3; j++)
                        {
                            if (players[i].rack.tiles[j].color == "K") compareKey1++;
                            if (players[i].rack.tiles[j].color == "B") compareKey2++;
                        }
                    }
                }
                answerText.text = (compareKey1 > compareKey2) ? "<b><color=#808080>黑色</color></b>" :
                                             ((compareKey1 < compareKey2) ? "<b><color=#A64C26>棕色</color></b>" : "<b>一樣多</b>");
                break;
            case 22:
                //<b><color=#808080>黑色</color>和<color=#D90000>紅色</color></b>，何者較多？";
                for (int i = 0; i < players.Count; i++)
                {
                    if (answerPlayer != i)
                    {
                        for (int j = 0; j < 3; j++)
                        {
                            if (players[i].rack.tiles[j].color == "K") compareKey1++;
                            if (players[i].rack.tiles[j].color == "R") compareKey2++;
                        }
                    }
                }
                answerText.text = (compareKey1 > compareKey2) ? "<b><color=#808080>黑色</color></b>" :
                                             ((compareKey1 < compareKey2) ? "<b><color=#D90000>紅色</color></b>" : "<b>一樣多</b>");
                break;
            case 23:
                //<b><color=#009960>綠色</color>和<color=#F2CC00>黃色</color></b>，何者較多？";
                for (int i = 0; i < players.Count; i++)
                {
                    if (answerPlayer != i)
                    {
                        for (int j = 0; j < 3; j++)
                        {
                            if (players[i].rack.tiles[j].color == "G") compareKey1++;
                            if (players[i].rack.tiles[j].color == "Y") compareKey2++;
                        }
                    }
                }
                answerText.text = (compareKey1 > compareKey2) ? "<b><color=#009960>綠色</color></b>" :
                                             ((compareKey1 < compareKey2) ? "<b><color=#F2CC00>黃色</color></b>" : "<b>一樣多</b>");
                break;
        }

        activePlayer[(answerPlayer + 4) % 5].AnswerFilter(answerPlayer, activePlayer, cardId, answerKey, compareKey1 > compareKey2 ? true : false, compareKey1 < compareKey2 ? true : false);
        activePlayer[(answerPlayer + 3) % 5].AnswerFilter(answerPlayer, activePlayer, cardId, answerKey, compareKey1 > compareKey2 ? true : false, compareKey1 < compareKey2 ? true : false);
        activePlayer[(answerPlayer + 2) % 5].AnswerFilter(answerPlayer, activePlayer, cardId, answerKey, compareKey1 > compareKey2 ? true : false, compareKey1 < compareKey2 ? true : false);
        activePlayer[(answerPlayer + 1) % 5].AnswerFilter(answerPlayer, activePlayer, cardId, answerKey, compareKey1 > compareKey2 ? true : false, compareKey1 < compareKey2 ? true : false);

        //玩家0的輔助模式
        activePlayer[0].TileLight(assistMode);

        //呼叫數字判定
        answerCall = activePlayer[1].solution || activePlayer[2].solution || activePlayer[3].solution || activePlayer[4].solution;

        #region 重置問題卡
        if (questionCard.Count==0)
        {
            questionCard.Clear();
            for (int i = 1; i <= 23; i++)
            {
                questionCard.Add(i);
            }
        }
        #endregion

        //CALL PASS按鈕顯示
        callButton.gameObject.SetActive(true);
        passButton.gameObject.SetActive(true);
    }

    void NumberCall()
    {
        //當玩家確定自己牌架上的數字時，會立即進行猜測。
        //電腦一定會答對，但是電腦只會在剩餘一個組合時才會進行猜測，即使未確定的部分是不同顏色的相同數字
        //根據規則，當有兩個以上的玩家要進行猜測時，從回答問題的玩家的右手邊的玩家開始以逆時鐘方向依序進行。

        for(int i = answerPlayer + 4; i>answerPlayer; i--)
        {
            int playerId = i % 5;


        }
    }
}
