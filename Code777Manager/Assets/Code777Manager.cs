using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum GameStatus { Idle, NextStep, NumberCall, PlayerCall, SecondJudge, GameSet }

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
    public Sprite[] tileSprites;
    public Sprite[] tileSpritesColor;
    #endregion

    //public GameObject[] playerIcon;
    //public TextMesh[] playerName;
    public GameObject[] playerIcon;
    public GameObject[] correctCall;
    public GameObject[] wrongCall;
    public Text[] playerName;
    public Text[] possibleCount;

    public List<Tile> tilePile = new List<Tile>(28);    //未打開的Tile堆
    public List<Tile> discardTile = new List<Tile>(3); //使用過被棄置於場中央的Tile堆
    public List<int> questionCard = new List<int>(23);//用來給予情報的問題卡

    public bool assistMode = true;  //輔助模式，此模式開啟下會於場景右下角提示可能TILE
    public bool advancedMode = false;   //進階模式，此模式開啟下呼叫數字必須連顏色都正確
    //public GameObject[] assistTile;
    public Image[] assistTile;
    public Image[] playerTileBack;

    public List<Player> activePlayer;

    public int answerPlayer;

    public Text questionText;
    public Text answerText;
    public Text speakerText;

    public Button startButton;
    public Button callButton;
    public Button passButton;
    public Button restartButton;

    public GameObject playerInputUI;
    public Button[] numberUp;
    public Button[] numberDown;
    public Image[] numberTile;
    public Button submitCall;
    public int[] numberCallTile = { -1, -1, -1 };


    //操控控制項
    public static bool answerCall = false;
    //玩家是否要叫用數字，以布林值playerCall來判斷
    public static bool playerCall = false;    //玩家如果要呼叫數字，則為true；否則為false

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

    // Start is called before the first frame update
    void Start()
    {
        questionText.text = "";
        answerText.text = "";
        speakerText.text = "";

        #region DEBUG顯示所有玩家可能性

        //possibleCount[0].gameObject.SetActive(true);
        //possibleCount[1].gameObject.SetActive(true);
        //possibleCount[2].gameObject.SetActive(true);
        //possibleCount[3].gameObject.SetActive(true);
        //possibleCount[4].gameObject.SetActive(true);

        #endregion

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

            playerIcon[i].GetComponent<Image>().sprite = activePlayerIcon;
            playerName[i].text = activePlayerName;

            Player newPlayer = new Player(activePlayerName, activePlayerIcon, i, 0, possibleCount[i]);

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

        #region 初始化玩家回答介面按鈕
        numberUp[0].onClick.AddListener(delegate { PlayerCallKey(0, true); });
        numberDown[0].onClick.AddListener(delegate { PlayerCallKey(0, false); });
        numberUp[1].onClick.AddListener(delegate { PlayerCallKey(1, true); });
        numberDown[1].onClick.AddListener(delegate { PlayerCallKey(1, false); });
        numberUp[2].onClick.AddListener(delegate { PlayerCallKey(2, true); });
        numberDown[2].onClick.AddListener(delegate { PlayerCallKey(2, false); });
        submitCall.onClick.AddListener(delegate { StartCoroutine(PlayerCallSubmit()); });
        restartButton.onClick.AddListener(RestartKey);
        #endregion
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.anyKeyDown)
        {
            //if (Input.GetKeyDown(KeyCode.F2))
            //{
            //    UnityEngine.SceneManagement.SceneManager.LoadScene("Code777GamePlay");
            //}
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Application.Quit();
            }
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

                //隱藏CALL PASS按鈕
                callButton.gameObject.SetActive(false);
                passButton.gameObject.SetActive(false);

                activePlayer[answerPlayer].TileRecover();

                //同時回答時，以AnswerPlayer自身，往右手邊的玩家順序進行
                if ( (answerPlayer!=0 && activePlayer[answerPlayer].callOk) || (answerPlayer==0 && playerCall) )
                {
                    //回答玩家的回合
                    if (answerPlayer == 0)
                        StartCoroutine(PlayerCallPrepare());
                    else
                        StartCoroutine(NumberCall(answerPlayer));
                }
                else if( ( (answerPlayer + 4) % 5!=0 &&  activePlayer[(answerPlayer+4)%5].callOk) || ( (answerPlayer + 4) % 5==0 && playerCall ) )
                {
                    //回答玩家右手邊一位玩家的回合
                    if ((answerPlayer + 4) % 5 == 0)
                        StartCoroutine(PlayerCallPrepare());
                    else
                        StartCoroutine(NumberCall((answerPlayer + 4) % 5));
                }
                else if( ( (answerPlayer + 3) % 5!=0 &&  activePlayer[(answerPlayer+3)%5].callOk) || ( (answerPlayer + 3) % 5==0 && playerCall ) )
                {
                    //回答玩家右手邊二位玩家的回合
                    if ((answerPlayer + 3) % 5 == 0)
                        StartCoroutine(PlayerCallPrepare());
                    else
                        StartCoroutine(NumberCall((answerPlayer + 3) % 5));
                }
                else if (((answerPlayer + 2) % 5 != 0 && activePlayer[(answerPlayer + 2) % 5].callOk) || ((answerPlayer + 2) % 5 == 0 && playerCall))
                {
                    //回答玩家左手邊二位玩家的回合
                    if ((answerPlayer + 2) % 5 == 0)
                        StartCoroutine(PlayerCallPrepare());
                    else
                        StartCoroutine(NumberCall((answerPlayer + 2) % 5));
                }
                else if (((answerPlayer + 1) % 5 != 0 && activePlayer[(answerPlayer + 1) % 5].callOk) || ((answerPlayer + 1) % 5== 0 && playerCall))
                {
                    //回答玩家左手邊一位玩家的回合
                    if ((answerPlayer + 1) % 5 == 0)
                        StartCoroutine(PlayerCallPrepare());
                    else
                        StartCoroutine(NumberCall((answerPlayer + 1) % 5));
                }
                else
                {
                    //沒有玩家要再回答，進入二輪判定
                    //剛才在同時回答中才確定數字組合的玩家的判定
                    //呼叫數字判定
                    answerCall = activePlayer[1].solution || activePlayer[2].solution || activePlayer[3].solution || activePlayer[4].solution;
                    activePlayer[1].callOk = activePlayer[1].solution;
                    activePlayer[2].callOk = activePlayer[2].solution;
                    activePlayer[3].callOk = activePlayer[3].solution;
                    activePlayer[4].callOk = activePlayer[4].solution;
                    status = GameStatus.SecondJudge;
                }
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

        if(status == GameStatus.SecondJudge)
        {
            status = GameStatus.Idle;
            //CALL PASS按鈕顯示
            callButton.gameObject.SetActive(true);
            passButton.gameObject.SetActive(true);
        }

    }

    void PlayerCallKey(int index, bool upKey)
    {
        if (upKey)
        {
            if (numberCallTile[index] == -1) numberCallTile[index] = 0;
            else if (advancedMode) numberCallTile[index] += (numberCallTile[index] == 10) ? (-10) : 1;
            else numberCallTile[index] += (numberCallTile[index] == 6) ? (-6) : 1;
        }
        else
        {
            if (numberCallTile[index] == -1) numberCallTile[index] = advancedMode ? 10 : 6;
            else if (advancedMode) numberCallTile[index] += (numberCallTile[index] == 0) ? 10 : (-1);
            else numberCallTile[index] += (numberCallTile[index] == 0) ? 6 : (-1);
        }

        numberTile[index].sprite = advancedMode ? tileSpritesColor[numberCallTile[index]] : tileSprites[numberCallTile[index]];
        submitCall.gameObject.SetActive(!(numberCallTile[0] == -1 || numberCallTile[1] == -1 || numberCallTile[2] == -1));
    }

    void RestartKey()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Code777GamePlay");
    }

    IEnumerator AnswerCard(int cardId, List<Player> players)
    {
        //Debug用測試訊息
        string debugMessage = "";

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

        #region  回答問題答案
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
        debugMessage = debugMessage + activePlayer[answerPlayer].name + "回答： " + questionText.text + "　" + answerText.text +"\n";
        debugMessage = debugMessage + activePlayer[answerPlayer].name + "所見： ";
        for(int i= answerPlayer+1; i< answerPlayer+activePlayer.Count;i++)
        {
            debugMessage += activePlayer[i% activePlayer.Count].name + "[ <b>";
            for (int j = 0; j < 3; j++)
            {
                switch (activePlayer[i % activePlayer.Count].rack.tiles[j].color)
                {
                    case "G": debugMessage += "<color=#009960>"; break;
                    case "Y": debugMessage += "<color=#F2CC00>"; break;
                    case "K": debugMessage += "<color=#404040>"; break;
                    case "B": debugMessage += "<color=#A64C26>"; break;
                    case "R": debugMessage += "<color=#D90000>"; break;
                    case "P": debugMessage += "<color=#D91ACC>"; break;
                    case "C": debugMessage += "<color=#0059FF>"; break;
                    default: break;
                }
                debugMessage += activePlayer[i % activePlayer.Count].rack.tiles[j].number + "</color> ";
            }
            debugMessage += "</b>]　";
        }
        Debug.Log(debugMessage);
        #endregion

        activePlayer[(answerPlayer + 4) % 5].AnswerFilter(answerPlayer, activePlayer, cardId, answerKey, compareKey1 > compareKey2 ? true : false, compareKey1 < compareKey2 ? true : false);
        activePlayer[(answerPlayer + 3) % 5].AnswerFilter(answerPlayer, activePlayer, cardId, answerKey, compareKey1 > compareKey2 ? true : false, compareKey1 < compareKey2 ? true : false);
        activePlayer[(answerPlayer + 2) % 5].AnswerFilter(answerPlayer, activePlayer, cardId, answerKey, compareKey1 > compareKey2 ? true : false, compareKey1 < compareKey2 ? true : false);
        activePlayer[(answerPlayer + 1) % 5].AnswerFilter(answerPlayer, activePlayer, cardId, answerKey, compareKey1 > compareKey2 ? true : false, compareKey1 < compareKey2 ? true : false);

        //玩家0的輔助模式
        activePlayer[0].TileLight(assistMode);

        //呼叫數字判定
        answerCall = activePlayer[1].solution || activePlayer[2].solution || activePlayer[3].solution || activePlayer[4].solution;
        activePlayer[1].callOk = activePlayer[1].solution;
        activePlayer[2].callOk = activePlayer[2].solution;
        activePlayer[3].callOk = activePlayer[3].solution;
        activePlayer[4].callOk = activePlayer[4].solution;

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

    IEnumerator NumberCall(int callPlayer)
    {
        //DEBUG用訊息
        string debugMessage;

        #region 紀錄玩家猜測前牌架
        debugMessage = "[ <b>";
        for (int j = 0; j < 3; j++)
        {
            switch (activePlayer[callPlayer].rack.tiles[j].color)
            {
                case "G": debugMessage += "<color=#009960>"; break;
                case "Y": debugMessage += "<color=#F2CC00>"; break;
                case "K": debugMessage += "<color=#404040>"; break;
                case "B": debugMessage += "<color=#A64C26>"; break;
                case "R": debugMessage += "<color=#D90000>"; break;
                case "P": debugMessage += "<color=#D91ACC>"; break;
                case "C": debugMessage += "<color=#0059FF>"; break;
                default: break;
            }
            debugMessage += activePlayer[callPlayer].rack.tiles[j].number + " </color>";
        }
        debugMessage += "</b>]";
        #endregion

        //當玩家確定自己牌架上的數字時，會立即進行猜測。
        //電腦一定會答對，但是電腦只會在剩餘一個組合時才會進行猜測，即使未確定的部分是不同顏色的相同數字

        activePlayer[callPlayer].BecomeCallPlayer();

        //畫面訊息刷新
        questionText.text = "";
        answerText.text = "";
        speakerText.text = "輪到 <color=#00C0FF>" + activePlayer[callPlayer].name + "</color> 作答數字";
        GameObject.Find("Player" + callPlayer + "Name").GetComponent<Text>().fontStyle = FontStyle.Bold;
        GameObject.Find("Player" + callPlayer + "Name").GetComponent<Text>().color = new Color(0, 0.75F, 1, 1);

        //暫停1秒
        yield return new WaitForSeconds(1);

        debugMessage = activePlayer[callPlayer].name + "答對了牌架" + debugMessage;
        //讓答對的效果跳出來
        //GameObject.Find("Player" + callPlayer + "Correct").SetActive(true);
        correctCall[callPlayer].SetActive(true);
        questionText.text = "<color=#00C0FF>" + activePlayer[callPlayer].name + "</color> 答對了數字";
        activePlayer[callPlayer].GetPoint();

        //暫停2秒
        yield return new WaitForSeconds(2);

        //目前邏輯是一定會答對
        correctCall[callPlayer].SetActive(false);
        if (activePlayer[callPlayer].victory>=3)
        {
            Debug.Log(debugMessage);

            //如果玩家獲得3分，遊戲就結束。
            questionText.text = "<color=#00C0FF>" + activePlayer[callPlayer].name + "</color> 答對了數字\n" +
                                            "<color=#00C0FF>" + activePlayer[callPlayer].name + "</color> 獲得勝利";
            answerText.text = "遊戲結束";

            //開啟Player側的覆蓋Tile
            playerTileBack[0].color = new Color(1, 1, 1, 0);
            playerTileBack[1].color = new Color(1, 1, 1, 0);
            playerTileBack[2].color = new Color(1, 1, 1, 0);

            restartButton.gameObject.SetActive(true);
            status = GameStatus.GameSet;
        }
        else
        {
            //答對跟答錯時，玩家獲得的情報不一樣
            questionText.text = "<color=#00C0FF>" + activePlayer[callPlayer].name + "</color> 答對了數字\n" +
                                "<color=#00C0FF>" + activePlayer[callPlayer].name + "</color> 重設牌架";

            //不論答對或答錯，玩家都會丟棄目前自己架上的3張牌，然後再從Pile抽3張起來
            discardTile.Add( new Tile(activePlayer[callPlayer].rack.tiles[0].number,
                                                      activePlayer[callPlayer].rack.tiles[0].color,
                                                      activePlayer[callPlayer].rack.tiles[0].image) );
            discardTile.Add(new Tile(activePlayer[callPlayer].rack.tiles[1].number,
                                                      activePlayer[callPlayer].rack.tiles[1].color,
                                                      activePlayer[callPlayer].rack.tiles[1].image));
            discardTile.Add(new Tile(activePlayer[callPlayer].rack.tiles[2].number,
                                                      activePlayer[callPlayer].rack.tiles[2].color,
                                                      activePlayer[callPlayer].rack.tiles[2].image));

            Tile tile0, tile1, tile2;

            drawTile = Random.Range(0, tilePile.Count);
            tile0 = new Tile(tilePile[drawTile].number, tilePile[drawTile].color, tilePile[drawTile].image);   //宣告Tile0
            tilePile.Remove(tilePile[drawTile]);

            drawTile = Random.Range(0, tilePile.Count);
            tile1 = new Tile(tilePile[drawTile].number, tilePile[drawTile].color, tilePile[drawTile].image);   //宣告Tile1
            tilePile.Remove(tilePile[drawTile]);

            drawTile = Random.Range(0, tilePile.Count);
            tile2 = new Tile(tilePile[drawTile].number, tilePile[drawTile].color, tilePile[drawTile].image);   //宣告Tile2
            tilePile.Remove(tilePile[drawTile]);

            activePlayer[callPlayer].NewRack(tile0, tile1, tile2);


            //如果是答對的玩家，則可以確認自己剛才丟棄的TILE
            //也就是過濾時可以加入discardTile的情報
            activePlayer[callPlayer].RackCheck(activePlayer, discardTile);

            //discardTile要洗回Tile堆中
            tilePile.Add(discardTile[0]);
            tilePile.Add(discardTile[1]);
            tilePile.Add(discardTile[2]);
            discardTile.Clear();

            activePlayer[(callPlayer+1)%5].RackCheck(activePlayer, discardTile);
            activePlayer[(callPlayer + 2) % 5].RackCheck(activePlayer, discardTile);
            activePlayer[(callPlayer + 3) % 5].RackCheck(activePlayer, discardTile);
            activePlayer[(callPlayer + 4) % 5].RackCheck(activePlayer, discardTile);

            //玩家0的輔助模式
            activePlayer[0].TileLight(assistMode);

            activePlayer[callPlayer].IconRecover();
            GameObject.Find("Player" + callPlayer + "Name").GetComponent<Text>().fontStyle =
                callPlayer == answerPlayer ? FontStyle.Bold : FontStyle.Normal;
            GameObject.Find("Player" + callPlayer + "Name").GetComponent<Text>().color =
                callPlayer == answerPlayer ? new Color(1, 0.75F, 0, 1) : new Color(1, 1, 1, 1);
            yield return new WaitForSeconds(2);
            status = GameStatus.NumberCall;

            #region 紀錄玩家猜測後牌架
            debugMessage = debugMessage + "\n牌架更換為";
            debugMessage = debugMessage + "[ <b>";
            for (int j = 0; j < 3; j++)
            {
                switch (activePlayer[callPlayer].rack.tiles[j].color)
                {
                    case "G": debugMessage += "<color=#009960>"; break;
                    case "Y": debugMessage += "<color=#F2CC00>"; break;
                    case "K": debugMessage += "<color=#404040>"; break;
                    case "B": debugMessage += "<color=#A64C26>"; break;
                    case "R": debugMessage += "<color=#D90000>"; break;
                    case "P": debugMessage += "<color=#D91ACC>"; break;
                    case "C": debugMessage += "<color=#0059FF>"; break;
                    default: break;
                }
                debugMessage += activePlayer[callPlayer].rack.tiles[j].number + " </color>";
            }
            debugMessage += "</b>]";
            #endregion

            Debug.Log(debugMessage);
        }
    }

    IEnumerator PlayerCallPrepare()
    {
        playerCall = false;
        activePlayer[0].BecomeCallPlayer();

        //畫面訊息刷新
        questionText.text = "";
        answerText.text = "";
        speakerText.text = "輪到 <color=#00C0FF>" + activePlayer[0].name + "</color> 作答數字";
        GameObject.Find("Player0Name").GetComponent<Text>().fontStyle = FontStyle.Bold;
        GameObject.Find("Player0Name").GetComponent<Text>().color = new Color(0, 0.75F, 1, 1);

        //暫停1秒
        yield return new WaitForSeconds(1);

        playerInputUI.SetActive(true);
        numberUp[0].gameObject.SetActive(true);
        numberUp[1].gameObject.SetActive(true);
        numberUp[2].gameObject.SetActive(true);
        numberDown[0].gameObject.SetActive(true);
        numberDown[1].gameObject.SetActive(true);
        numberDown[2].gameObject.SetActive(true);

        status = GameStatus.PlayerCall;
    }

    IEnumerator PlayerCallSubmit()
    {
        //Debug用訊息
        string debugMessage = "";

        #region 紀錄玩家猜測前牌架
        debugMessage = "[ <b>";
        for (int j = 0; j < 3; j++)
        {
            switch (activePlayer[0].rack.tiles[j].color)
            {
                case "G": debugMessage += "<color=#009960>"; break;
                case "Y": debugMessage += "<color=#F2CC00>"; break;
                case "K": debugMessage += "<color=#404040>"; break;
                case "B": debugMessage += "<color=#A64C26>"; break;
                case "R": debugMessage += "<color=#D90000>"; break;
                case "P": debugMessage += "<color=#D91ACC>"; break;
                case "C": debugMessage += "<color=#0059FF>"; break;
                default: break;
            }
            debugMessage += activePlayer[0].rack.tiles[j].number + " </color>";
        }
        debugMessage += "</b>]";
        #endregion


        List<int> sortAnswer = new List<int>(3);
        sortAnswer.Add(numberCallTile[0]);
        sortAnswer.Add(numberCallTile[1]);
        sortAnswer.Add(numberCallTile[2]);
        sortAnswer.Sort();
        string defineAnswer = "";
        for(int i=0;i<3;i++)
        {
            if( advancedMode )
            {
                switch(sortAnswer[i])
                {
                    case 0: defineAnswer += "1G";  break;
                    case 1: defineAnswer += "2Y"; break;
                    case 2: defineAnswer += "3K"; break;
                    case 3: defineAnswer += "4B"; break;
                    case 4: defineAnswer += "5R"; break;
                    case 5: defineAnswer += "5K"; break;
                    case 6: defineAnswer += "6P"; break;
                    case 7: defineAnswer += "6G"; break;
                    case 8: defineAnswer += "7Y"; break;
                    case 9: defineAnswer += "7P"; break;
                    case 10: defineAnswer += "7C"; break;
                }
            }
            else
            {
                defineAnswer += (sortAnswer[i] + 1).ToString();
            }
        }

        //正誤判定
        if( (advancedMode && defineAnswer == activePlayer[0].rack.numberColorMatch) || (!advancedMode && defineAnswer == activePlayer[0].rack.numberMatch))
        {
            debugMessage = activePlayer[0].name + "答對了牌架" + debugMessage;

            //正解
            correctCall[0].SetActive(true);
            questionText.text = "<color=#00C0FF>" + activePlayer[0].name + "</color> 答對了數字";
            activePlayer[0].GetPoint();

            //開啟Player側的覆蓋Tile
            playerTileBack[0].color = new Color(1, 1, 1, 0);
            playerTileBack[1].color = new Color(1, 1, 1, 0);
            playerTileBack[2].color = new Color(1, 1, 1, 0);

            yield return new WaitForSeconds(2);

            playerInputUI.SetActive(false);
            correctCall[0].SetActive(false);
            if (activePlayer[0].victory >= 3)
            {
                Debug.Log(debugMessage);

                //如果玩家獲得3分，遊戲就結束。
                questionText.text = "<color=#00C0FF>" + activePlayer[0].name + "</color> 答對了數字\n" +
                                                "<color=#00C0FF>" + activePlayer[0].name + "</color> 獲得勝利";
                answerText.text = "遊戲結束";

                restartButton.gameObject.SetActive(true);
                status = GameStatus.GameSet;
            }
            else
            {
                //重設牌架，關閉Player側的覆蓋Tile
                playerTileBack[0].color = new Color(1, 1, 1, 1);
                playerTileBack[1].color = new Color(1, 1, 1, 1);
                playerTileBack[2].color = new Color(1, 1, 1, 1);

                //答對跟答錯時，玩家獲得的情報不一樣
                questionText.text = "<color=#00C0FF>" + activePlayer[0].name + "</color> 答對了數字\n" +
                                    "<color=#00C0FF>" + activePlayer[0].name + "</color> 重設牌架";

                //不論答對或答錯，玩家都會丟棄目前自己架上的3張牌，然後再從Pile抽3張起來
                discardTile.Add(new Tile(activePlayer[0].rack.tiles[0].number,
                                                          activePlayer[0].rack.tiles[0].color,
                                                          activePlayer[0].rack.tiles[0].image));
                discardTile.Add(new Tile(activePlayer[0].rack.tiles[1].number,
                                                          activePlayer[0].rack.tiles[1].color,
                                                          activePlayer[0].rack.tiles[1].image));
                discardTile.Add(new Tile(activePlayer[0].rack.tiles[2].number,
                                                          activePlayer[0].rack.tiles[2].color,
                                                          activePlayer[0].rack.tiles[2].image));

                Tile tile0, tile1, tile2;

                drawTile = Random.Range(0, tilePile.Count);
                tile0 = new Tile(tilePile[drawTile].number, tilePile[drawTile].color, tilePile[drawTile].image);   //宣告Tile0
                tilePile.Remove(tilePile[drawTile]);

                drawTile = Random.Range(0, tilePile.Count);
                tile1 = new Tile(tilePile[drawTile].number, tilePile[drawTile].color, tilePile[drawTile].image);   //宣告Tile1
                tilePile.Remove(tilePile[drawTile]);

                drawTile = Random.Range(0, tilePile.Count);
                tile2 = new Tile(tilePile[drawTile].number, tilePile[drawTile].color, tilePile[drawTile].image);   //宣告Tile2
                tilePile.Remove(tilePile[drawTile]);

                activePlayer[0].NewRack(tile0, tile1, tile2);


                //如果是答對的玩家，則可以確認自己剛才丟棄的TILE
                //也就是過濾時可以加入discardTile的情報
                activePlayer[0].RackCheck(activePlayer, discardTile);

                //discardTile要洗回Tile堆中
                tilePile.Add(discardTile[0]);
                tilePile.Add(discardTile[1]);
                tilePile.Add(discardTile[2]);
                discardTile.Clear();

                activePlayer[1].RackCheck(activePlayer, discardTile);
                activePlayer[2].RackCheck(activePlayer, discardTile);
                activePlayer[3].RackCheck(activePlayer, discardTile);
                activePlayer[4].RackCheck(activePlayer, discardTile);

                //玩家0的輔助模式
                activePlayer[0].TileLight(assistMode);

                activePlayer[0].IconRecover();
                GameObject.Find("Player0Name").GetComponent<Text>().fontStyle =
                    0== answerPlayer ? FontStyle.Bold : FontStyle.Normal;
                GameObject.Find("Player0Name").GetComponent<Text>().color =
                    0 == answerPlayer ? new Color(1, 0.75F, 0, 1) : new Color(1, 1, 1, 1);
                yield return new WaitForSeconds(2);
                status = GameStatus.NumberCall;

                #region 紀錄玩家猜測後牌架
                debugMessage = debugMessage + "\n牌架更換為";
                debugMessage = debugMessage + "[ <b>";
                for (int j = 0; j < 3; j++)
                {
                    switch (activePlayer[0].rack.tiles[j].color)
                    {
                        case "G": debugMessage += "<color=#009960>"; break;
                        case "Y": debugMessage += "<color=#F2CC00>"; break;
                        case "K": debugMessage += "<color=#404040>"; break;
                        case "B": debugMessage += "<color=#A64C26>"; break;
                        case "R": debugMessage += "<color=#D90000>"; break;
                        case "P": debugMessage += "<color=#D91ACC>"; break;
                        case "C": debugMessage += "<color=#0059FF>"; break;
                        default: break;
                    }
                    debugMessage += activePlayer[0].rack.tiles[j].number + " </color>";
                }
                debugMessage += "</b>]";
                #endregion

                Debug.Log(debugMessage);
            }
        }
        else
        {
            debugMessage = activePlayer[0].name + "答錯了牌架" + debugMessage;

            //答錯
            wrongCall[0].SetActive(true);
            questionText.text = "<color=#00C0FF>" + activePlayer[0].name + "</color> 答錯了數字";

            yield return new WaitForSeconds(2);

            wrongCall[0].SetActive(false);
            playerInputUI.SetActive(false);

            //答對跟答錯時，玩家獲得的情報不一樣
            questionText.text = "<color=#00C0FF>" + activePlayer[0].name + "</color> 答錯了數字\n" +
                                "<color=#00C0FF>" + activePlayer[0].name + "</color> 重設牌架";

            //不論答對或答錯，玩家都會丟棄目前自己架上的3張牌，然後再從Pile抽3張起來
            discardTile.Add(new Tile(activePlayer[0].rack.tiles[0].number,
                                                      activePlayer[0].rack.tiles[0].color,
                                                      activePlayer[0].rack.tiles[0].image));
            discardTile.Add(new Tile(activePlayer[0].rack.tiles[1].number,
                                                      activePlayer[0].rack.tiles[1].color,
                                                      activePlayer[0].rack.tiles[1].image));
            discardTile.Add(new Tile(activePlayer[0].rack.tiles[2].number,
                                                      activePlayer[0].rack.tiles[2].color,
                                                      activePlayer[0].rack.tiles[2].image));

            Tile tile0, tile1, tile2;

            drawTile = Random.Range(0, tilePile.Count);
            tile0 = new Tile(tilePile[drawTile].number, tilePile[drawTile].color, tilePile[drawTile].image);   //宣告Tile0
            tilePile.Remove(tilePile[drawTile]);

            drawTile = Random.Range(0, tilePile.Count);
            tile1 = new Tile(tilePile[drawTile].number, tilePile[drawTile].color, tilePile[drawTile].image);   //宣告Tile1
            tilePile.Remove(tilePile[drawTile]);

            drawTile = Random.Range(0, tilePile.Count);
            tile2 = new Tile(tilePile[drawTile].number, tilePile[drawTile].color, tilePile[drawTile].image);   //宣告Tile2
            tilePile.Remove(tilePile[drawTile]);

            activePlayer[0].NewRack(tile0, tile1, tile2);


            //discardTile要洗回Tile堆中
            tilePile.Add(discardTile[0]);
            tilePile.Add(discardTile[1]);
            tilePile.Add(discardTile[2]);
            discardTile.Clear();

            //答錯的玩家 要在不知道自己原本牌架的情況下對下一個牌架做篩選
            activePlayer[0].RackCheck(activePlayer, discardTile);

            activePlayer[1].RackCheck(activePlayer, discardTile);
            activePlayer[2].RackCheck(activePlayer, discardTile);
            activePlayer[3].RackCheck(activePlayer, discardTile);
            activePlayer[4].RackCheck(activePlayer, discardTile);

            //玩家0的輔助模式
            activePlayer[0].TileLight(assistMode);

            activePlayer[0].IconRecover();
            GameObject.Find("Player0Name").GetComponent<Text>().fontStyle =
                0 == answerPlayer ? FontStyle.Bold : FontStyle.Normal;
            GameObject.Find("Player0Name").GetComponent<Text>().color =
                0 == answerPlayer ? new Color(1, 0.75F, 0, 1) : new Color(1, 1, 1, 1);
            yield return new WaitForSeconds(2);
            status = GameStatus.NumberCall;

            #region 紀錄玩家猜測後牌架
            debugMessage = debugMessage + "\n牌架更換為";
            debugMessage = debugMessage + "[ <b>";
            for (int j = 0; j < 3; j++)
            {
                switch (activePlayer[0].rack.tiles[j].color)
                {
                    case "G": debugMessage += "<color=#009960>"; break;
                    case "Y": debugMessage += "<color=#F2CC00>"; break;
                    case "K": debugMessage += "<color=#808080>"; break;
                    case "B": debugMessage += "<color=#A64C26>"; break;
                    case "R": debugMessage += "<color=#D90000>"; break;
                    case "P": debugMessage += "<color=#D91ACC>"; break;
                    case "C": debugMessage += "<color=#0059FF>"; break;
                    default: break;
                }
                debugMessage += activePlayer[0].rack.tiles[j].number + " </color>";
            }
            debugMessage += "</b>]";
            #endregion

            Debug.Log(debugMessage);
        }

    }
}
