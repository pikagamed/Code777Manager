using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum GameStatus { Idle, NextStep, NumberCall, PlayerCall, SecondJudge, GameSet }

public class Code777Manager : MonoBehaviour
{
    [Header("AKino的密藏密碼")]
    public string secretPassword;

    //遊戲狀態
    public GameStatus status = GameStatus.Idle;

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
    public Sprite tileSpriteBack;
    #endregion

    //public GameObject[] playerIcon;
    //public TextMesh[] playerName;
    public GameObject[] playerIcon;
    public GameObject[] correctCall;
    public GameObject[] wrongCall;
    public Text[] playerName;
    public Text[] possibleCount;

    List<Tile> tilePile = new List<Tile>(28);    //未打開的Tile堆
    List<Tile> discardTile = new List<Tile>(3); //使用過被棄置於場中央的Tile堆
    List<int> questionCard = new List<int>(23);//用來給予情報的問題卡

    bool assistMode;  //輔助模式，此模式開啟下會於場景右下角提示可能TILE
    bool advancedMode;   //進階模式，此模式開啟下呼叫數字必須連顏色都正確

    public GameObject assistTileSet;
    public Image[] assistTile;
    public Image[] playerTileBack;

    public List<Player> activePlayer;

    int answerPlayer;

    public Text questionText;
    public Text answerText;
    public Text speakerText;

    #region 遊戲中的LOG
    public GameObject assistLog;
    List<string> logMessage = new List<string>();
    public Text[] logTextId;
    public Text[] logTextContent;
    public Slider logSlider;
    #endregion

    public Button startButton;
    public Button callButton;
    public Button passButton;
    public Button restartButton;
    public Button menuButton;
    public Text startText;
    public Text callText;
    public Text passText;
    public Text restartText;
    public Text menuText;

    public GameObject playerInputUI;
    public Button[] numberUp;
    public Button[] numberDown;
    public Image[] numberTile;
    public Button submitCall;
    public Text submitText;
    int[] numberCallTile = { -1, -1, -1 };


    //操控控制項
    bool answerCall = false;
    //玩家是否要叫用數字，以布林值playerCall來判斷
    bool playerCall = false;    //玩家如果要呼叫數字，則為true；否則為false

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
        assistMode = Code777Menu.difficulty == Difficulty.Assist;  //輔助模式，此模式開啟下會於場景右下角提示可能TILE
        advancedMode = Code777Menu.difficulty == Difficulty.Advanced;   //進階模式，此模式開啟下呼叫數字必須連顏色都正確

        switch (Code777Menu.language)
        {
            case Language.Chinese:
                startText.text = "開始";
                callText.text = "猜測";
                passText.text = "PASS";
                restartText.text = "再玩一局";
                menuText.text = "回主選單";
                submitText.text = "確認";
                break;
            case Language.Japanese:
                startText.text = "スタート";
                callText.text = "コール";
                passText.text = "パス";
                restartText.text = "もう一局";
                menuText.text = "メニュー";
                submitText.text = "決定";
                break;
            case Language.English:
                startText.text = "START";
                callText.text = "CALL";
                passText.text = "PASS";
                restartText.text = "RESTART";
                menuText.text = "MENU";
                submitText.text = "CALL";
                break;
        }

        questionText.text = "";
        answerText.text = "";
        speakerText.text = "";

        assistTileSet.SetActive(assistMode);
        assistLog.SetActive(!assistMode);

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

            activePlayer[i].NewRack(tile0, tile1, tile2, assistMode, advancedMode);
        }
        #endregion

        #region 牌架過濾

        activePlayer[0].RackCheck(activePlayer, discardTile);
        activePlayer[1].RackCheck(activePlayer, discardTile);
        activePlayer[2].RackCheck(activePlayer, discardTile);
        activePlayer[3].RackCheck(activePlayer, discardTile);
        activePlayer[4].RackCheck(activePlayer, discardTile);

        //玩家0的輔助模式
        TileLight();

        #endregion

        #region 輸入各其他玩家的起始牌架於LOG內

        for (int i = 0; i < 5; i++)
        {
            logTextId[i].text = "" ;
            logTextContent[i].text = "";
        }

        //起始LOG只有五個
        //0.遊戲開始
        //1.電腦1(玩家下家)的起始牌架
        //2.電腦2(玩家下下家)的起始牌架
        //3.電腦3(玩家上上家)的起始牌架
        //4.電腦4(玩家上家)的起始牌架

        switch (Code777Menu.language)
        {
            case Language.Chinese:
                logMessage.Add("<color=#FF8080>遊戲開始</color>");
                break;
            case Language.Japanese:
                logMessage.Add("<color=#FF8080>ゲームが始まります</color>");
                break;
            case Language.English:
                logMessage.Add("<color=#FF8080>Game Start</color>");
                break;
            default:
                break;
        }

        for (int i = 1; i < activePlayer.Count; i++)
        {
            string tileInformation = "【<b> ";

            for (int j = 0; j < 3; j++)
            {
                switch (activePlayer[i].rack.tiles[j].color)
                {
                    case "G": tileInformation += "<color=#009960>"; break;
                    case "Y": tileInformation += "<color=#F2CC00>"; break;
                    case "K": tileInformation += "<color=#808080>"; break;
                    case "B": tileInformation += "<color=#A64C26>"; break;
                    case "R": tileInformation += "<color=#D90000>"; break;
                    case "P": tileInformation += "<color=#D91ACC>"; break;
                    case "C": tileInformation += "<color=#0059FF>"; break;
                    default: break;
                }
                tileInformation += activePlayer[i].rack.tiles[j].number + " </color>";
            }

            tileInformation += "</b>】";

            switch (Code777Menu.language)
            {
                case Language.Chinese:
                    logMessage.Add("<color=#00C0FF><B>" + activePlayer[i].name + "</B></color>的起始牌架為 " + tileInformation);
                    break;
                case Language.Japanese:
                    logMessage.Add("<color=#00C0FF><B>" + activePlayer[i].name + "</B></color>の最初のラックは " + tileInformation + "である");
                    break;
                case Language.English:
                    logMessage.Add("<color=#00C0FF><B>" + activePlayer[i].name + "</B></color>'s initial rack is " + tileInformation + ".");
                    break;
                default:
                    break;
            }
        }

        JumpToLatestLog();

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
                    answerCall = (activePlayer[1].solution && activePlayer[1].handicap == 0) || (activePlayer[2].solution && activePlayer[2].handicap == 0)
                || (activePlayer[3].solution && activePlayer[3].handicap == 0) || (activePlayer[4].solution && activePlayer[4].handicap == 0);

                    for (int i = 1; i < activePlayer.Count; i++)
                    {
                        if (activePlayer[i].handicap > 0 && activePlayer[i].solution)
                        {
                            activePlayer[i].handicap--;
                            activePlayer[i].callOk = false;
                        }
                        else
                            activePlayer[i].callOk = activePlayer[i].solution;
                    }

                    //activePlayer[1].callOk = activePlayer[1].solution;
                    //activePlayer[2].callOk = activePlayer[2].solution;
                    //activePlayer[3].callOk = activePlayer[3].solution;
                    //activePlayer[4].callOk = activePlayer[4].solution;
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

    /// <summary>
    /// 對Player0適用的輔助模式
    /// </summary>
    public void TileLight()
    {
        assistTile[0].GetComponent<Image>().color = new Vector4(0.75F, 0.75F, 0.75F, 1F);
        assistTile[1].GetComponent<Image>().color = new Vector4(0.75F, 0.75F, 0.75F, 1F);
        assistTile[2].GetComponent<Image>().color = new Vector4(0.75F, 0.75F, 0.75F, 1F);
        assistTile[3].GetComponent<Image>().color = new Vector4(0.75F, 0.75F, 0.75F, 1F);
        assistTile[4].GetComponent<Image>().color = new Vector4(0.75F, 0.75F, 0.75F, 1F);
        assistTile[5].GetComponent<Image>().color = new Vector4(0.75F, 0.75F, 0.75F, 1F);
        assistTile[6].GetComponent<Image>().color = new Vector4(0.75F, 0.75F, 0.75F, 1F);
        assistTile[7].GetComponent<Image>().color = new Vector4(0.75F, 0.75F, 0.75F, 1F);
        assistTile[8].GetComponent<Image>().color = new Vector4(0.75F, 0.75F, 0.75F, 1F);
        assistTile[9].GetComponent<Image>().color = new Vector4(0.75F, 0.75F, 0.75F, 1F);
        assistTile[10].GetComponent<Image>().color = new Vector4(0.75F, 0.75F, 0.75F, 1F);
        assistTile[11].GetComponent<Image>().color = new Vector4(0.75F, 0.75F, 0.75F, 1F);
        assistTile[12].GetComponent<Image>().color = new Vector4(0.75F, 0.75F, 0.75F, 1F);
        assistTile[13].GetComponent<Image>().color = new Vector4(0.75F, 0.75F, 0.75F, 1F);
        assistTile[14].GetComponent<Image>().color = new Vector4(0.75F, 0.75F, 0.75F, 1F);
        assistTile[15].GetComponent<Image>().color = new Vector4(0.75F, 0.75F, 0.75F, 1F);
        assistTile[16].GetComponent<Image>().color = new Vector4(0.75F, 0.75F, 0.75F, 1F);
        assistTile[17].GetComponent<Image>().color = new Vector4(0.75F, 0.75F, 0.75F, 1F);
        assistTile[18].GetComponent<Image>().color = new Vector4(0.75F, 0.75F, 0.75F, 1F);
        assistTile[19].GetComponent<Image>().color = new Vector4(0.75F, 0.75F, 0.75F, 1F);
        assistTile[20].GetComponent<Image>().color = new Vector4(0.75F, 0.75F, 0.75F, 1F);
        assistTile[21].GetComponent<Image>().color = new Vector4(0.75F, 0.75F, 0.75F, 1F);
        assistTile[22].GetComponent<Image>().color = new Vector4(0.75F, 0.75F, 0.75F, 1F);
        assistTile[23].GetComponent<Image>().color = new Vector4(0.75F, 0.75F, 0.75F, 1F);
        assistTile[24].GetComponent<Image>().color = new Vector4(0.75F, 0.75F, 0.75F, 1F);
        assistTile[25].GetComponent<Image>().color = new Vector4(0.75F, 0.75F, 0.75F, 1F);
        assistTile[26].GetComponent<Image>().color = new Vector4(0.75F, 0.75F, 0.75F, 1F);
        assistTile[27].GetComponent<Image>().color = new Vector4(0.75F, 0.75F, 0.75F, 1F);

        if (activePlayer[0].possibleNumber.G1 < 1) assistTile[0].GetComponent<Image>().color = new Vector4(0.25F, 0.25F, 0.25F, 1F);
        if (activePlayer[0].possibleNumber.Y2 < 1) assistTile[1].GetComponent<Image>().color = new Vector4(0.25F, 0.25F, 0.25F, 1F);
        if (activePlayer[0].possibleNumber.Y2 < 2) assistTile[2].GetComponent<Image>().color = new Vector4(0.25F, 0.25F, 0.25F, 1F);
        if (activePlayer[0].possibleNumber.K3 < 1) assistTile[3].GetComponent<Image>().color = new Vector4(0.25F, 0.25F, 0.25F, 1F);
        if (activePlayer[0].possibleNumber.K3 < 2) assistTile[4].GetComponent<Image>().color = new Vector4(0.25F, 0.25F, 0.25F, 1F);
        if (activePlayer[0].possibleNumber.K3 < 3) assistTile[5].GetComponent<Image>().color = new Vector4(0.25F, 0.25F, 0.25F, 1F);
        if (activePlayer[0].possibleNumber.B4 < 1) assistTile[6].GetComponent<Image>().color = new Vector4(0.25F, 0.25F, 0.25F, 1F);
        if (activePlayer[0].possibleNumber.B4 < 2) assistTile[7].GetComponent<Image>().color = new Vector4(0.25F, 0.25F, 0.25F, 1F);
        if (activePlayer[0].possibleNumber.B4 < 3) assistTile[8].GetComponent<Image>().color = new Vector4(0.25F, 0.25F, 0.25F, 1F);
        if (activePlayer[0].possibleNumber.B4 < 4) assistTile[9].GetComponent<Image>().color = new Vector4(0.25F, 0.25F, 0.25F, 1F);
        if (activePlayer[0].possibleNumber.R5 < 1) assistTile[10].GetComponent<Image>().color = new Vector4(0.25F, 0.25F, 0.25F, 1F);
        if (activePlayer[0].possibleNumber.R5 < 2) assistTile[11].GetComponent<Image>().color = new Vector4(0.25F, 0.25F, 0.25F, 1F);
        if (activePlayer[0].possibleNumber.R5 < 3) assistTile[12].GetComponent<Image>().color = new Vector4(0.25F, 0.25F, 0.25F, 1F);
        if (activePlayer[0].possibleNumber.R5 < 4) assistTile[13].GetComponent<Image>().color = new Vector4(0.25F, 0.25F, 0.25F, 1F);
        if (activePlayer[0].possibleNumber.K5 < 1) assistTile[14].GetComponent<Image>().color = new Vector4(0.25F, 0.25F, 0.25F, 1F);
        if (activePlayer[0].possibleNumber.P6 < 1) assistTile[15].GetComponent<Image>().color = new Vector4(0.25F, 0.25F, 0.25F, 1F);
        if (activePlayer[0].possibleNumber.P6 < 2) assistTile[16].GetComponent<Image>().color = new Vector4(0.25F, 0.25F, 0.25F, 1F);
        if (activePlayer[0].possibleNumber.P6 < 3) assistTile[17].GetComponent<Image>().color = new Vector4(0.25F, 0.25F, 0.25F, 1F);
        if (activePlayer[0].possibleNumber.G6 < 1) assistTile[18].GetComponent<Image>().color = new Vector4(0.25F, 0.25F, 0.25F, 1F);
        if (activePlayer[0].possibleNumber.G6 < 2) assistTile[19].GetComponent<Image>().color = new Vector4(0.25F, 0.25F, 0.25F, 1F);
        if (activePlayer[0].possibleNumber.G6 < 3) assistTile[20].GetComponent<Image>().color = new Vector4(0.25F, 0.25F, 0.25F, 1F);
        if (activePlayer[0].possibleNumber.Y7 < 1) assistTile[21].GetComponent<Image>().color = new Vector4(0.25F, 0.25F, 0.25F, 1F);
        if (activePlayer[0].possibleNumber.Y7 < 2) assistTile[22].GetComponent<Image>().color = new Vector4(0.25F, 0.25F, 0.25F, 1F);
        if (activePlayer[0].possibleNumber.P7 < 1) assistTile[23].GetComponent<Image>().color = new Vector4(0.25F, 0.25F, 0.25F, 1F);
        if (activePlayer[0].possibleNumber.C7 < 1) assistTile[24].GetComponent<Image>().color = new Vector4(0.25F, 0.25F, 0.25F, 1F);
        if (activePlayer[0].possibleNumber.C7 < 2) assistTile[25].GetComponent<Image>().color = new Vector4(0.25F, 0.25F, 0.25F, 1F);
        if (activePlayer[0].possibleNumber.C7 < 3) assistTile[26].GetComponent<Image>().color = new Vector4(0.25F, 0.25F, 0.25F, 1F);
        if (activePlayer[0].possibleNumber.C7 < 4) assistTile[27].GetComponent<Image>().color = new Vector4(0.25F, 0.25F, 0.25F, 1F);
    }

    void JumpToLatestLog()
    {
        logSlider.gameObject.SetActive(logMessage.Count > 5);
        logSlider.maxValue = logMessage.Count - 5;
        logSlider.value = logSlider.maxValue;
        for (int i = 0; i < 5; i++)
        {
            logTextId[i].text = (logMessage.Count - 4 + i).ToString();
            logTextContent[i].text = logMessage[logMessage.Count - 5 + i];
        }
    }

    public void LogSliding()
    {
        //Silde的邏輯：
        //假設LogList小於等於5個字串，Slide不起作用
        //假設LogList等於6個字串，會把List分割為 [0],[1],[2],[3],[4] 和 [1],[2],[3],[4],[5] 兩個區間
        //假設LogList等於7個字串，會把List分割為 [0],[1],[2],[3],[4] 和 [1],[2],[3],[4],[5] 和 [2],[3],[4],[5],[6] 三個區間

        //因此邏輯為，將Value分割為 logMessage.Count - 4 個區間
        //如果是6個LOG，區間是2個，每個0.5；7個LOG，區間是3個，每個0.333；8個LOG；區間是4個，每個0.25。

        int segement = (int)logSlider.value;
        for (int i = 0; i < 5; i++)
        {
            logTextId[i].text = (segement + i + 1).ToString();
            logTextContent[i].text = logMessage[segement + i];
        }
    }

    #region 按鈕部分宣告

    public void GameStart()
    {
        //玩家叫用
        Debug.Log("遊戲開始");
        status = GameStatus.NextStep;
    }

    public void InstructorHit(bool numberCall )
    {
        //玩家叫用
        //Debug.Log(numberCall?"按鈕CALL已經按下":"按鈕PASS已經按下");
        status = GameStatus.NumberCall;
        answerCall = numberCall ? true : answerCall;
        playerCall = numberCall ? true : false;
    }

    public void PlayerCallKeyUp(int callTileIndex)
    {
        if (numberCallTile[callTileIndex] == -1) numberCallTile[callTileIndex] = 0;
        else if (advancedMode) numberCallTile[callTileIndex] += (numberCallTile[callTileIndex] == 10) ? (-10) : 1;
        else numberCallTile[callTileIndex] += (numberCallTile[callTileIndex] == 6) ? (-6) : 1;

        numberTile[callTileIndex].sprite = advancedMode ? tileSpritesColor[numberCallTile[callTileIndex]] : tileSprites[numberCallTile[callTileIndex]];
        submitCall.gameObject.SetActive(!(numberCallTile[0] == -1 || numberCallTile[1] == -1 || numberCallTile[2] == -1));
    }

    public void PlayerCallKeyDown(int callTileIndex)
    {
        if (numberCallTile[callTileIndex] == -1) numberCallTile[callTileIndex] = advancedMode ? 10 : 6;
        else if (advancedMode) numberCallTile[callTileIndex] += (numberCallTile[callTileIndex] == 0) ? 10 : (-1);
        else numberCallTile[callTileIndex] += (numberCallTile[callTileIndex] == 0) ? 6 : (-1);

        numberTile[callTileIndex].sprite = advancedMode ? tileSpritesColor[numberCallTile[callTileIndex]] : tileSprites[numberCallTile[callTileIndex]];
        submitCall.gameObject.SetActive(!(numberCallTile[0] == -1 || numberCallTile[1] == -1 || numberCallTile[2] == -1));
    }

    public void PlayerCallSubmitKey()
    {
        StartCoroutine(PlayerCallSubmit());
    }

    public void RestartKey()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Code777GamePlay");
    }

    public void MenuKey()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Menu");
    }

    #endregion

    IEnumerator AnswerCard(int cardId, List<Player> players)
    {
        //Debug用測試訊息
        string debugMessage = "";

        //Log用訊息
        string logInsert = "";

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
        switch (Code777Menu.language)
        {
            case Language.Chinese:
                speakerText.text = "這個問題由 <color=#FFC000>" + activePlayer[answerPlayer].name + "</color> 回答";
                break;
            case Language.Japanese:
                speakerText.text = "<color=#FFC000>" + activePlayer[answerPlayer].name + "</color> は質問を答える";
                break;
            case Language.English:
                speakerText.text = "<color=#FFC000>" + activePlayer[answerPlayer].name + "</color> answers the Question";
                break;
        }
        yield return new WaitForSeconds(1);

        #region 顯示問題卡
        switch (cardId)
        {
            case 1:
                switch (Code777Menu.language)
                {
                    case Language.Chinese:
                        questionText.text = cardId + ". 有多少個牌架上的牌，三個數字總和是<b>18以上</b>？";
                        logInsert = logInsert + cardId + ". 三個數字總和是18以上的牌架";
                        break;
                    case Language.Japanese:
                        questionText.text = cardId + ". 三つの数字の和が<b>18以上</b>のラックがいくつでしょうか？";
                        logInsert = logInsert + cardId + ". 三つの数の和が18以上のラック";
                        break;
                    case Language.English:
                        questionText.text = cardId + ". On how many racks is the sum of the numbers <b>18 or more</b>?";
                        logInsert = logInsert + cardId + ". Racks in sum of the numbers 18+";
                        break;
                }
                break;
            case 2:
                switch (Code777Menu.language)
                {
                    case Language.Chinese:
                        questionText.text = cardId + ". 有多少個牌架上的牌，三個數字總和是<b>12以下</b>？";
                        logInsert = logInsert + cardId + ". 三個數字總和是12以下的牌架";
                        break;
                    case Language.Japanese:
                        questionText.text = cardId + ". 三つの数字の和が<b>12以下</b>のラックがいくつでしょうか？";
                        logInsert = logInsert + cardId + ". 三つの数の和が12以下のラック";
                        break;
                    case Language.English:
                        questionText.text = cardId + ". On how many racks is the sum of the numbers <b>12 or less</b>?";
                        logInsert = logInsert + cardId + ". Racks in sum of the numbers 12-";
                        break;
                }
                break;
            case 3:
                switch (Code777Menu.language)
                {
                    case Language.Chinese:
                        questionText.text = cardId + ". 有多少個牌架上的牌，出現<b>不同顏色的相同數字</b>？";
                        logInsert = logInsert + cardId + ". 出現不同顏色的相同數字的牌架";
                        break;
                    case Language.Japanese:
                        questionText.text = cardId + ". <b>色が違う同じ数字</b>があるラックがいくつでしょうか？";
                        logInsert = logInsert + cardId + ". 色が違う同じ数字があるラック";
                        break;
                    case Language.English:
                        questionText.text = cardId + ". On how many racks is there <b>a same number in different colors</b>? ";
                        logInsert = logInsert + cardId + ". Racks in same number in different colors";
                        break;
                }
                break;
            case 4:
                switch (Code777Menu.language)
                {
                    case Language.Chinese:
                        questionText.text = cardId + ". 有多少個牌架上的牌，出現<b>三個不同顏色</b>的數字？";
                        logInsert = logInsert + cardId + ". 三個數字不同顏色的牌架";
                        break;
                    case Language.Japanese:
                        questionText.text = cardId + ". <b>三つの数字の色が違う</b>ラックがいくつでしょうか？";
                        logInsert = logInsert + cardId + ". 三つの数字の色が違うラック";
                        break;
                    case Language.English:
                        questionText.text = cardId + ". On how many racks are there <b>3 different colors</b>?";
                        logInsert = logInsert + cardId + ". Racks in 3 different colors";
                        break;
                }
                break;
            case 5:
                switch (Code777Menu.language)
                {
                    case Language.Chinese:
                        questionText.text = cardId + ". 有多少個牌架上的牌，三個數字<b>皆是奇數</b>或<b>皆是偶數</b>？";
                        logInsert = logInsert + cardId + ". 三個數字皆是奇數或皆是偶數的牌架";
                        break;
                    case Language.Japanese:
                        questionText.text = cardId + ". 三つの数字が<b>全員奇数</b>か<b>全員偶数</b>であるラックがいくつでしょうか？";
                        logInsert = logInsert + cardId + ". 三つの数字が全員奇数か全員偶数であるラック";
                        break;
                    case Language.English:
                        questionText.text = cardId + ". On how many racks are the numbers either <b>all even</b> or <b>all uneven</b>?";
                        logInsert = logInsert + cardId + ". Racks in all even or all uneven";
                        break;
                }
                break;
            case 6:
                switch (Code777Menu.language)
                {
                    case Language.Chinese:
                        questionText.text = cardId + ". 有多少個牌架上的牌，出現<b>相同顏色的相同數字</b>？";
                        logInsert = logInsert + cardId + ". 出現相同顏色且相同數字的牌架";
                        break;
                    case Language.Japanese:
                        questionText.text = cardId + ". <b>色も数字も同じなカード</b>があるラックがいくつでしょうか？";
                        logInsert = logInsert + cardId + ". 色も数字も同じなカードがあるラック";
                        break;
                    case Language.English:
                        questionText.text = cardId + ". On how many racks are there <b>at least 2 identical cards</b>?";
                        logInsert = logInsert + cardId + ". Racks in 2+ identical cards";
                        break;
                }
                break;
            case 7:
                switch (Code777Menu.language)
                {
                    case Language.Chinese:
                        questionText.text = cardId + ". 有多少個牌架上的牌，三個數字是<b>連續的數字</b>？";
                        logInsert = logInsert + cardId + ". 三個數字是連續數字的牌架";
                        break;
                    case Language.Japanese:
                        questionText.text = cardId + ". 三つの数字が<b>連続番号</b>であるラックがいくつでしょうか？";
                        logInsert = logInsert + cardId + ". 三つの数字が続くラック";
                        break;
                    case Language.English:
                        questionText.text = cardId + ". On how many racks do you see <b>3 consecutive numbers</b>?";
                        logInsert = logInsert + cardId + ". Racks in 3 consecutive numbers";
                        break;
                }
                break;
            case 8:
                switch (Code777Menu.language)
                {
                    case Language.Chinese:
                        questionText.text = cardId + ". 你看到<b>多少種顏色</b>的數字牌？";
                        logInsert = logInsert + cardId + ". 看到的顏色數量";
                        break;
                    case Language.Japanese:
                        questionText.text = cardId + ". 色が<b>いくつ</b>見えるでしょうか？";
                        logInsert = logInsert + cardId + ". 見えた色の数";
                        break;
                    case Language.English:
                        questionText.text = cardId + ". <b>How many colors</b> do you see?";
                        logInsert = logInsert + cardId + ". Number of colors seen";
                        break;
                }
                break;
            case 9:
                switch (Code777Menu.language)
                {
                    case Language.Chinese:
                        questionText.text = cardId + ". 有幾種<b>顏色</b>出現了<b>三次以上</b>？";
                        logInsert = logInsert + cardId + ". 出現三次以上的顏色的種數";
                        break;
                    case Language.Japanese:
                        questionText.text = cardId + ". <b>三枚以上</b>である<b>色</b>がいくつでしょうか？";
                        logInsert = logInsert + cardId + ". 三枚以上の色の数";
                        break;
                    case Language.English:
                        questionText.text = cardId + ". How many <b>colors</b> appear <b>at least 3 times</b>?";
                        logInsert = logInsert + cardId + ". Number of colors which appear 3+ times";
                        break;
                }
                break;
            case 10:
                switch (Code777Menu.language)
                {
                    case Language.Chinese:
                        questionText.text = cardId + ". 有幾種數字<b>完全沒有出現</b>？";
                        logInsert = logInsert + cardId + ". 完全沒出現的數字的種類數";
                        break;
                    case Language.Japanese:
                        questionText.text = cardId + ". <b>全然出ない</b>数字がいくつでしょうか？";
                        logInsert = logInsert + cardId + ". 全然出ない数字の数";
                        break;
                    case Language.English:
                        questionText.text = cardId + ". How many numbers are <b>missing</b>?";
                        logInsert = logInsert + cardId + ". Number of missing numbers";
                        break;
                }
                break;
            case 11:
                switch (Code777Menu.language)
                {
                    case Language.Chinese:
                        questionText.text = cardId + ". <b><color=#009960>綠1</color>、<color=#808080>黑5</color>、<color=#D91ACC>粉紅7</color></b>這三種牌，你總共看到幾張？";
                        logInsert = logInsert + cardId + ". <color=#009960>綠1</color>、<color=#808080>黑5</color>、<color=#D91ACC>粉紅7</color>的合計個數";
                        break;
                    case Language.Japanese:
                        questionText.text = cardId + ". <b><color=#009960>緑1</color>、<color=#808080>黒5</color>、<color=#D91ACC>桃7</color></b>が合計いくつ見えるでしょうか？";
                        logInsert = logInsert + cardId + ". <color=#009960>緑1</color>、<color=#808080>黒5</color>、<color=#D91ACC>桃7</color>の合計の数";
                        break;
                    case Language.English:
                        questionText.text = cardId + ". How many of the following do you see in all: <b><color=#009960>Green ones</color>, <color=#808080>Black fives</color>, <color=#D91ACC>Pink sevens</color></b>?";
                        logInsert = logInsert + cardId + ". <color=#009960>Green ones</color>, <color=#808080>Black fives</color>, <color=#D91ACC>Pink sevens</color> in all";
                        break;
                }
                break;
            case 12:
                switch (Code777Menu.language)
                {
                    case Language.Chinese:
                        questionText.text = cardId + ". <b><color=#808080>黑3</color>和<color=#D91ACC>粉紅6</color></b>，何者較多？";
                        logInsert = logInsert + cardId + ". <color=#808080>黑3</color>和<color=#D91ACC>粉紅6</color>，何者較多？";
                        break;
                    case Language.Japanese:
                        questionText.text = cardId + ". <b><color=#808080>黒3</color>か<color=#D91ACC>桃6</color></b>、どちらがより多いでしょうか？";
                        logInsert = logInsert + cardId + ". <color=#808080>黒3</color>か<color=#D91ACC>桃6</color>、より多いのは？";
                        break;
                    case Language.English:
                        questionText.text = cardId + ". Do you see more <b><color=#808080>Black threes</color> or <color=#D91ACC>Pink sixes</color></b>?";
                        logInsert = logInsert + cardId + ". More <color=#808080>Black threes</color> or <color=#D91ACC>Pink sixes</color>?";
                        break;
                }
                break;
            case 13:
                switch (Code777Menu.language)
                {
                    case Language.Chinese:
                        questionText.text = cardId + ". <b><color=#009960>綠6</color>和<color=#F2CC00>黃7</color></b>，何者較多？";
                        logInsert = logInsert + cardId + ". <color=#009960>綠6</color>和<color=#F2CC00>黃7</color>，何者較多？";
                        break;
                    case Language.Japanese:
                        questionText.text = cardId + ". <b><color=#009960>緑6</color>か<color=#F2CC00>黄7</color></b>、どちらがより多いでしょうか？";
                        logInsert = logInsert + cardId + ". <color=#009960>緑6</color>か<color=#F2CC00>黄7</color>、より多いのは？";
                        break;
                    case Language.English:
                        questionText.text = cardId + ". Do you see more <b><color=#009960>Green sixes</color> or more <color=#F2CC00>Yellow sevens</color></b>?";
                        logInsert = logInsert + cardId + ". More <color=#009960>Green sixes</color> or more <color=#F2CC00>Yellow sevens</color>?";
                        break;
                }
                break;
            case 14:
                switch (Code777Menu.language)
                {
                    case Language.Chinese:
                        questionText.text = cardId + ". <b><color=#F2CC00>黃2</color>和<color=#F2CC00>黃7</color></b>，何者較多？";
                        logInsert = logInsert + cardId + ". <color=#F2CC00>黃2</color>和<color=#F2CC00>黃7</color>，何者較多？";
                        break;
                    case Language.Japanese:
                        questionText.text = cardId + ". <b><color=#F2CC00>黄2</color>か<color=#F2CC00>黄7</color></b>、どちらがより多いでしょうか？";
                        logInsert = logInsert + cardId + ". <color=#F2CC00>黄2</color>か<color=#F2CC00>黄7</color>、より多いのは？";
                        break;
                    case Language.English:
                        questionText.text = cardId + ". Do you see more <b><color=#F2CC00>Yellow twos</color> or <color=#F2CC00>Yellow sevens</color></b>?";
                        logInsert = logInsert + cardId + ". More <color=#F2CC00>Yellow twos</color> or <color=#F2CC00>Yellow sevens</color>?";
                        break;
                }
                break;
            case 15:
                switch (Code777Menu.language)
                {
                    case Language.Chinese:
                        questionText.text = cardId + ". <b><color=#D91ACC>粉紅6</color>和<color=#009960>綠6</color></b>，何者較多？";
                        logInsert = logInsert + cardId + ". <color=#D91ACC>粉紅6</color>和<color=#009960>綠6</color>，何者較多？";
                        break;
                    case Language.Japanese:
                        questionText.text = cardId + ". <b><color=#D91ACC>桃6</color>か<color=#009960>緑6</color></b>、どちらがより多いでしょうか";
                        logInsert = logInsert + cardId + ". <color=#D91ACC>桃6</color>か<color=#009960>緑6</color>、より多いのは？";
                        break;
                    case Language.English:
                        questionText.text = cardId + ". Do you see more <b><color=#D91ACC>Pink sixes</color> or <color=#009960>Green sixes</color></b>?";
                        logInsert = logInsert + cardId + ". More <color=#D91ACC>Pink sixes</color> or <color=#009960>Green sixes</color>?";
                        break;
                }
                break;
            case 16:
                switch (Code777Menu.language)
                {
                    case Language.Chinese:
                        questionText.text = cardId + ". <b><color=#0059FF>藍7</color>和其他顏色的7</b>，何者較多？";
                        logInsert = logInsert + cardId + ". <color=#0059FF>藍7</color>和其他顏色的7，何者較多？";
                        break;
                    case Language.Japanese:
                        questionText.text = cardId + ". <b><color=#0059FF>青7</color>か他の色の7</b>、どちらがより多いでしょうか？";
                        logInsert = logInsert + cardId + ". <color=#0059FF>青7</color>か他の色の7、より多いのは？";
                        break;
                    case Language.English:
                        questionText.text = cardId + ". Do you see more <b><color=#0059FF>Blue sevens</color> or more sevens of other colors</b>?";
                        logInsert = logInsert + cardId + ". More <color=#0059FF>Blue sevens</color> or more sevens of other colors?";
                        break;
                }
                break;
            case 17:
                switch (Code777Menu.language)
                {
                    case Language.Chinese:
                        questionText.text = cardId + ". <b><color=#A64C26>棕色</color>和<color=#0059FF>藍色</color></b>，何者較多？";
                        logInsert = logInsert + cardId + ". <color=#A64C26>棕色</color>和<color=#0059FF>藍色</color>，何者較多？";
                        break;
                    case Language.Japanese:
                        questionText.text = cardId + ". <b><color=#A64C26>茶色</color>か<color=#0059FF>青色</color></b>、どちらがより多いでしょうか？";
                        logInsert = logInsert + cardId + ". <color=#A64C26>茶色</color>か<color=#0059FF>青色</color>、より多いのは？";
                        break;
                    case Language.English:
                        questionText.text = cardId + ". Do you see more <b><color=#A64C26>Brown</color> or <color=#0059FF>Blue</color></b> numbers?";
                        logInsert = logInsert + cardId + ". More <color=#A64C26>Brown</color> or <color=#0059FF>Blue</color>?";
                        break;
                }
                break;
            case 18:
                switch (Code777Menu.language)
                {
                    case Language.Chinese:
                        questionText.text = cardId + ". <b><color=#D90000>紅色</color>和<color=#D91ACC>粉紅色</color></b>，何者較多？";
                        logInsert = logInsert + cardId + ". <color=#D90000>紅色</color>和<color=#D91ACC>粉紅色</color>，何者較多？";
                        break;
                    case Language.Japanese:
                        questionText.text = cardId + ". <b><color=#D90000>赤色</color>か<color=#D91ACC>桃色</color></b>、どちらがより多いでしょうか？";
                        logInsert = logInsert + cardId + ". <color=#D90000>赤色</color>か<color=#D91ACC>桃色</color>、より多いのは？";
                        break;
                    case Language.English:
                        questionText.text = cardId + ". Do you see more <b><color=#D90000>Red</color> or <color=#D91ACC>Pink</color></b> numbers?";
                        logInsert = logInsert + cardId + ". More <color=#D90000>Red</color> or <color=#D91ACC>Pink</color>?";
                        break;
                }
                break;
            case 19:
                switch (Code777Menu.language)
                {
                    case Language.Chinese:
                        questionText.text = cardId + ". <b><color=#009960>綠色</color>和<color=#0059FF>藍色</color></b>，何者較多？";
                        logInsert = logInsert + cardId + ". <color=#009960>綠色</color>和<color=#0059FF>藍色</color>，何者較多？";
                        break;
                    case Language.Japanese:
                        questionText.text = cardId + ". <b><color=#009960>緑色</color>か<color=#0059FF>青色</color></b>、どちらがより多いでしょうか？";
                        logInsert = logInsert + cardId + ". <color=#009960>緑色</color>か<color=#0059FF>青色</color>、より多いのは？";
                        break;
                    case Language.English:
                        questionText.text = cardId + ". Do you see more <b><color=#009960>Green</color> or more <color=#0059FF>Blue</color></b> numbers?";
                        logInsert = logInsert + cardId + ". More <color=#009960>Green</color> or more <color=#0059FF>Blue</color>?";
                        break;
                }
                break;
            case 20:
                switch (Code777Menu.language)
                {
                    case Language.Chinese:
                        questionText.text = cardId + ". <b><color=#F2CC00>黃色</color>和<color=#D91ACC>粉紅色</color></b>，何者較多？";
                        logInsert = logInsert + cardId + ". <color=#F2CC00>黃色</color>和<color=#D91ACC>粉紅色</color>，何者較多？";
                        break;
                    case Language.Japanese:
                        questionText.text = cardId + ". <b><color=#F2CC00>黄色</color>か<color=#D91ACC>桃色</color></b>、どちらがより多いでしょうか？";
                        logInsert = logInsert + cardId + ". <color=#F2CC00>黄色</color>か<color=#D91ACC>桃色</color>、より多いのは？";
                        break;
                    case Language.English:
                        questionText.text = cardId + ". Do you see more <b><color=#F2CC00>Yellow</color> or more <color=#D91ACC>Pink</color></b> numbers?";
                        logInsert = logInsert + cardId + ". More <color=#F2CC00>Yellow</color> or more <color=#D91ACC>Pink</color>?";
                        break;
                }
                break;
            case 21:
                switch (Code777Menu.language)
                {
                    case Language.Chinese:
                        questionText.text = cardId + ". <b><color=#808080>黑色</color>和<color=#A64C26>棕色</color></b>，何者較多？";
                        logInsert = logInsert + cardId + ". <color=#808080>黑色</color>和<color=#A64C26>棕色</color>，何者較多？";
                        break;
                    case Language.Japanese:
                        questionText.text = cardId + ". <b><color=#808080>黒色</color>か<color=#A64C26>茶色</color></b>、どちらがより多いでしょうか？";
                        logInsert = logInsert + cardId + ". <color=#808080>黒色</color>か<color=#A64C26>茶色</color>、より多いのは？";
                        break;
                    case Language.English:
                        questionText.text = cardId + ". Do you see more <b><color=#808080>Black</color> or more <color=#A64C26>Brown</color></b> numbers?";
                        logInsert = logInsert + cardId + ". More <color=#808080>Black</color> or more <color=#A64C26>Brown</color>?";
                        break;
                }
                break;
            case 22:
                switch (Code777Menu.language)
                {
                    case Language.Chinese:
                        questionText.text = cardId + ". <b><color=#808080>黑色</color>和<color=#D90000>紅色</color></b>，何者較多？";
                        logInsert = logInsert + cardId + ". <color=#808080>黑色</color>和<color=#D90000>紅色</color>，何者較多？";
                        break;
                    case Language.Japanese:
                        questionText.text = cardId + ". <b><color=#808080>黒色</color>か<color=#D90000>赤色</color></b>、どちらがより多いでしょうか？";
                        logInsert = logInsert + cardId + ". <color=#808080>黒色</color>か<color=#D90000>赤色</color>、より多いのは？";
                        break;
                    case Language.English:
                        questionText.text = cardId + ". Do you see more <b><color=#808080>Black</color> or more <color=#D90000>Red</color></b> numbers?";
                        logInsert = logInsert + cardId + ". More <color=#808080>Black</color> or more <color=#D90000>Red</color>?";
                        break;
                }
                break;
            case 23:
                switch (Code777Menu.language)
                {
                    case Language.Chinese:
                        questionText.text = cardId + ". <b><color=#009960>綠色</color>和<color=#F2CC00>黃色</color></b>，何者較多？";
                        logInsert = logInsert + cardId + ". <color=#009960>綠色</color>和<color=#F2CC00>黃色</color>，何者較多？";
                        break;
                    case Language.Japanese:
                        questionText.text = cardId + ". <b><color=#009960>緑色</color>か<color=#F2CC00>黄色</color></b>、どちらがより多いでしょうか？";
                        logInsert = logInsert + cardId + ". <color=#009960>緑色</color>か<color=#F2CC00>黄色</color>、より多いのは？";
                        break;
                    case Language.English:
                        questionText.text = cardId + ". Do you see more <b><color=#009960>Green</color> or more <color=#F2CC00>Yellow</color></b> numbers?	";
                        logInsert = logInsert + cardId + ". More <color=#009960>Green</color> or more <color=#F2CC00>Yellow</color>?";
                        break;
                }
                break;
        }

        switch (Code777Menu.language)
        {
            case Language.Chinese:
                logInsert = logInsert + "\n<b><color=#FFC000>" + activePlayer[answerPlayer].name + "</color></b>的回答： ";
                break;
            case Language.Japanese:
                logInsert = logInsert + "\n<b><color=#FFC000>" + activePlayer[answerPlayer].name + "</color></b>の答え： ";
                break;
            case Language.English:
                logInsert = logInsert + "\n<b><color=#FFC000>" + activePlayer[answerPlayer].name + "</color></b>'s answer:  ";
                break;
        }
       
        #endregion

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
                answerText.text = answerKey.ToString();
                break;
            case 2:
                // 有多少個牌架上的牌，三個數字總和是<b>12以下</b>？";
                for (int i = 0; i < players.Count; i++)
                {
                    if (answerPlayer != i)
                        answerKey += activePlayer[i].rack.untilTwelve ? 1 : 0;
                }
                answerText.text = answerKey.ToString();
                break;
            case 3:
                // 有多少個牌架上的牌，出現<b>不同顏色的相同數字</b>？";
                for (int i = 0; i < players.Count; i++)
                {
                    if (answerPlayer != i)
                        answerKey += activePlayer[i].rack.sameNumberDifColor ? 1 : 0;
                }
                answerText.text = answerKey.ToString();
                break;
            case 4:
                //有多少個牌架上的牌，出現三個<b>不同顏色</b>的數字？";
                for (int i = 0; i < players.Count; i++)
                {
                    if (answerPlayer != i)
                        answerKey += activePlayer[i].rack.threeColor ? 1 : 0;
                }
                answerText.text = answerKey.ToString();
                break;
            case 5:
                //有多少個牌架上的牌，三個數字<b>皆是奇數</b>或<b>皆是偶數</b>？";
                for (int i = 0; i < players.Count; i++)
                {
                    if (answerPlayer != i)
                        answerKey += activePlayer[i].rack.allOddEven ? 1 : 0;
                }
                answerText.text = answerKey.ToString();
                break;
            case 6:
                //有多少個牌架上的牌，出現<b>相同顏色的相同數字</b>？";
                for (int i = 0; i < players.Count; i++)
                {
                    if (answerPlayer != i)
                        answerKey += activePlayer[i].rack.sameColorNumber ? 1 : 0;
                }
                answerText.text = answerKey.ToString();
                break;
            case 7:
                //有多少個牌架上的牌，三個數字是<b>連續的數字</b>？";
                for (int i = 0; i < players.Count; i++)
                {
                    if (answerPlayer != i)
                        answerKey += activePlayer[i].rack.consecutiveNumber ? 1 : 0;
                }
                answerText.text = answerKey.ToString();
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
                answerText.text = answerKey.ToString();
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
                answerText.text = answerKey.ToString();
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
                answerText.text = answerKey.ToString();
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
                answerText.text = answerKey.ToString();
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
                switch (Code777Menu.language)
                {
                    case Language.Chinese:
                        answerText.text = (compareKey1 > compareKey2) ? "<b><color=#808080>黑3</color></b>" :
                                            ((compareKey1 < compareKey2) ? "<b><color=#D91ACC>粉紅6</color></b>" : "<b>一樣多</b>");
                        break;
                    case Language.Japanese:
                        answerText.text = (compareKey1 > compareKey2) ? "<b><color=#808080>黒3</color></b>" :
                                            ((compareKey1 < compareKey2) ? "<b><color=#D91ACC>桃6</color></b>" : "<b>等しい</b>");
                        break;
                    case Language.English:
                        answerText.text = (compareKey1 > compareKey2) ? "<b><color=#808080>Black Three</color></b>" :
                                            ((compareKey1 < compareKey2) ? "<b><color=#D91ACC>Pink Six</color></b>" : "<b>Even</b>");
                        break;
                }
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
                switch (Code777Menu.language)
                {
                    case Language.Chinese:
                        answerText.text = (compareKey1 > compareKey2) ? "<b><color=#009960>綠6</color></b>" :
                             ((compareKey1 < compareKey2) ? "<b><color=#F2CC00>黃7</color></b>" : "<b>一樣多</b>");
                        break;
                    case Language.Japanese:
                        answerText.text = (compareKey1 > compareKey2) ? "<b><color=#009960>緑6</color></b>" :
                             ((compareKey1 < compareKey2) ? "<b><color=#F2CC00>黄7</color></b>" : "<b>等しい</b>");
                        break;
                    case Language.English:
                        answerText.text = (compareKey1 > compareKey2) ? "<b><color=#009960>Green Six</color></b>" :
                             ((compareKey1 < compareKey2) ? "<b><color=#F2CC00>Yellow Seven</color></b>" : "<b>Even</b>");
                        break;
                }
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
                switch (Code777Menu.language)
                {
                    case Language.Chinese:
                        answerText.text = (compareKey1 > compareKey2) ? "<b><color=#F2CC00>黃2</color></b>" :
                             ((compareKey1 < compareKey2) ? "<b><color=#F2CC00>黃7</color></b>" : "<b>一樣多</b>");
                        break;
                    case Language.Japanese:
                        answerText.text = (compareKey1 > compareKey2) ? "<b><color=#F2CC00>黄2</color></b>" :
                             ((compareKey1 < compareKey2) ? "<b><color=#F2CC00>黄7</color></b>" : "<b>等しい</b>");
                        break;
                    case Language.English:
                        answerText.text = (compareKey1 > compareKey2) ? "<b><color=#F2CC00>Yellow Two</color></b>" :
                             ((compareKey1 < compareKey2) ? "<b><color=#F2CC00>Yellow Seven</color></b>" : "<b>Even</b>");
                        break;
                }
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
                switch (Code777Menu.language)
                {
                    case Language.Chinese:
                        answerText.text = (compareKey1 > compareKey2) ? "<b><color=#D91ACC>粉紅6</color></b>" :
                             ((compareKey1 < compareKey2) ? "<b><color=#009960>綠6</color></b>" : "<b>一樣多</b>");
                        break;
                    case Language.Japanese:
                        answerText.text = (compareKey1 > compareKey2) ? "<b><color=#D91ACC>桃6</color></b>" :
                             ((compareKey1 < compareKey2) ? "<b><color=#009960>緑6</color></b>" : "<b>等しい</b>");
                        break;
                    case Language.English:
                        answerText.text = (compareKey1 > compareKey2) ? "<b><color=#D91ACC>Pink Six</color></b>" :
                             ((compareKey1 < compareKey2) ? "<b><color=#009960>Green Six</color></b>" : "<b>Even</b>");
                        break;
                }
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
                switch (Code777Menu.language)
                {
                    case Language.Chinese:
                        answerText.text = (compareKey1 > compareKey2) ? "<b><color=#0059FF>藍7</color></b>" :
                                            ((compareKey1 < compareKey2) ? "<b><color=#C0C0C0>其他顏色7</color></b>" : "<b>一樣多</b>");
                        break;
                    case Language.Japanese:
                        answerText.text = (compareKey1 > compareKey2) ? "<b><color=#0059FF>青7</color></b>" :
                                            ((compareKey1 < compareKey2) ? "<b><color=#C0C0C0>他7</color></b>" : "<b>等しい</b>");
                        break;
                    case Language.English:
                        answerText.text = (compareKey1 > compareKey2) ? "<b><color=#0059FF>Blue Seven</color></b>" :
                                            ((compareKey1 < compareKey2) ? "<b><color=#C0C0C0>Other Seven</color></b>" : "<b>Even</b>");
                        break;
                }
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
                switch (Code777Menu.language)
                {
                    case Language.Chinese:
                        answerText.text = (compareKey1 > compareKey2) ? "<b><color=#A64C26>棕色</color></b>" :
                             ((compareKey1 < compareKey2) ? "<b><color=#0059FF>藍色</color></b>" : "<b>一樣多</b>");
                        break;
                    case Language.Japanese:
                        answerText.text = (compareKey1 > compareKey2) ? "<b><color=#A64C26>茶色</color></b>" :
                             ((compareKey1 < compareKey2) ? "<b><color=#0059FF>青色</color></b>" : "<b>等しい</b>");
                        break;
                    case Language.English:
                        answerText.text = (compareKey1 > compareKey2) ? "<b><color=#A64C26>Brown</color></b>" :
                             ((compareKey1 < compareKey2) ? "<b><color=#0059FF>Blue</color></b>" : "<b>Even</b>");
                        break;
                }
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
                switch (Code777Menu.language)
                {
                    case Language.Chinese:
                        answerText.text = (compareKey1 > compareKey2) ? "<b><color=#D90000>紅色</color></b>" :
                                            ((compareKey1 < compareKey2) ? "<b><color=#D91ACC>粉紅色</color></b>" : "<b>一樣多</b>");
                        break;
                    case Language.Japanese:
                        answerText.text = (compareKey1 > compareKey2) ? "<b><color=#D90000>赤色</color></b>" :
                                            ((compareKey1 < compareKey2) ? "<b><color=#D91ACC>桃色</color></b>" : "<b>等しい</b>");
                        break;
                    case Language.English:
                        answerText.text = (compareKey1 > compareKey2) ? "<b><color=#D90000>Red</color></b>" :
                                            ((compareKey1 < compareKey2) ? "<b><color=#D91ACC>Pink</color></b>" : "<b>Even</b>");
                        break;
                }
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
                switch (Code777Menu.language)
                {
                    case Language.Chinese:
                        answerText.text = (compareKey1 > compareKey2) ? "<b><color=#009960>綠色</color></b>" :
                                            ((compareKey1 < compareKey2) ? "<b><color=#0059FF>藍色</color></b>" : "<b>一樣多</b>");
                        break;
                    case Language.Japanese:
                        answerText.text = (compareKey1 > compareKey2) ? "<b><color=#009960>緑色</color></b>" :
                                            ((compareKey1 < compareKey2) ? "<b><color=#0059FF>青色</color></b>" : "<b>等しい</b>");
                        break;
                    case Language.English:
                        answerText.text = (compareKey1 > compareKey2) ? "<b><color=#009960>Green</color></b>" :
                                            ((compareKey1 < compareKey2) ? "<b><color=#0059FF>Blue</color></b>" : "<b>Even</b>");
                        break;
                }
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
                switch (Code777Menu.language)
                {
                    case Language.Chinese:
                        answerText.text = (compareKey1 > compareKey2) ? "<b><color=#F2CC00>黃色</color></b>" :
                             ((compareKey1 < compareKey2) ? "<b><color=#D91ACC>粉紅色</color></b>" : "<b>一樣多</b>");
                        break;
                    case Language.Japanese:
                        answerText.text = (compareKey1 > compareKey2) ? "<b><color=#F2CC00>黄色</color></b>" :
                             ((compareKey1 < compareKey2) ? "<b><color=#D91ACC>桃色</color></b>" : "<b>等しい</b>");
                        break;
                    case Language.English:
                        answerText.text = (compareKey1 > compareKey2) ? "<b><color=#F2CC00>Yellow</color></b>" :
                             ((compareKey1 < compareKey2) ? "<b><color=#D91ACC>Pink</color></b>" : "<b>Even</b>");
                        break;
                }
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
                switch (Code777Menu.language)
                {
                    case Language.Chinese:
                        answerText.text = (compareKey1 > compareKey2) ? "<b><color=#808080>黑色</color></b>" :
                                             ((compareKey1 < compareKey2) ? "<b><color=#A64C26>棕色</color></b>" : "<b>一樣多</b>");
                        break;
                    case Language.Japanese:
                        answerText.text = (compareKey1 > compareKey2) ? "<b><color=#808080>黒色</color></b>" :
                                             ((compareKey1 < compareKey2) ? "<b><color=#A64C26>茶色</color></b>" : "<b>等しい</b>");
                        break;
                    case Language.English:
                        answerText.text = (compareKey1 > compareKey2) ? "<b><color=#808080>Black</color></b>" :
                                             ((compareKey1 < compareKey2) ? "<b><color=#A64C26>Brown</color></b>" : "<b>Even</b>");
                        break;
                }
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
                switch (Code777Menu.language)
                {
                    case Language.Chinese:
                        answerText.text = (compareKey1 > compareKey2) ? "<b><color=#808080>黑色</color></b>" :
                                            ((compareKey1 < compareKey2) ? "<b><color=#D90000>紅色</color></b>" : "<b>一樣多</b>");
                        break;
                    case Language.Japanese:
                        answerText.text = (compareKey1 > compareKey2) ? "<b><color=#808080>黒色</color></b>" :
                                            ((compareKey1 < compareKey2) ? "<b><color=#D90000>赤色</color></b>" : "<b>等しい</b>");
                        break;
                    case Language.English:
                        answerText.text = (compareKey1 > compareKey2) ? "<b><color=#808080>Black</color></b>" :
                                            ((compareKey1 < compareKey2) ? "<b><color=#D90000>Red</color></b>" : "<b>Even</b>");
                        break;
                }
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
                switch (Code777Menu.language)
                {
                    case Language.Chinese:
                        answerText.text = (compareKey1 > compareKey2) ? "<b><color=#009960>綠色</color></b>" :
                                            ((compareKey1 < compareKey2) ? "<b><color=#F2CC00>黃色</color></b>" : "<b>一樣多</b>");
                        break;
                    case Language.Japanese:
                        answerText.text = (compareKey1 > compareKey2) ? "<b><color=#009960>緑色</color></b>" :
                                            ((compareKey1 < compareKey2) ? "<b><color=#F2CC00>黄色</color></b>" : "<b>等しい</b>");
                        break;
                    case Language.English:
                        answerText.text = (compareKey1 > compareKey2) ? "<b><color=#009960>Green</color></b>" :
                                            ((compareKey1 < compareKey2) ? "<b><color=#F2CC00>Yellow</color></b>" : "<b>Even</b>");
                        break;
                }
                break;
        }
        logInsert = logInsert + answerText.text;

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
        TileLight();


        //增加LOG
        #region LOG增加的處理
        logMessage.Add(logInsert);
        JumpToLatestLog();
        #endregion

        //呼叫數字判定
        answerCall = (activePlayer[1].solution&& activePlayer[1].handicap==0) || (activePlayer[2].solution && activePlayer[2].handicap == 0)
                        || (activePlayer[3].solution&& activePlayer[3].handicap == 0) || (activePlayer[4].solution && activePlayer[4].handicap == 0);

        for(int i=1; i<activePlayer.Count; i++)
        {
            if(activePlayer[i].handicap>0 && activePlayer[i].solution)
            {
                activePlayer[i].handicap--;
                activePlayer[i].callOk = false;
            }
            else
                activePlayer[i].callOk = activePlayer[i].solution;
        }

        //activePlayer[1].callOk = activePlayer[1].solution;
        //activePlayer[2].callOk = activePlayer[2].solution;
        //activePlayer[3].callOk = activePlayer[3].solution;
        //activePlayer[4].callOk = activePlayer[4].solution;

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
        string rackMessage = "";
        string logInsert = "";

        #region 紀錄玩家猜測前牌架
        debugMessage = "【<b> ";
        rackMessage = "【<b> ";
        for (int j = 0; j < 3; j++)
        {
            switch (activePlayer[callPlayer].rack.tiles[j].color)
            {
                case "G": debugMessage += "<color=#009960>"; rackMessage += "<color=#009960>"; break;
                case "Y": debugMessage += "<color=#F2CC00>"; rackMessage += "<color=#F2CC00>"; break;
                case "K": debugMessage += "<color=#404040>"; rackMessage += "<color=#808080>"; break;
                case "B": debugMessage += "<color=#A64C26>"; rackMessage += "<color=#A64C26>"; break;
                case "R": debugMessage += "<color=#D90000>"; rackMessage += "<color=#D90000>"; break;
                case "P": debugMessage += "<color=#D91ACC>"; rackMessage += "<color=#D91ACC>"; break;
                case "C": debugMessage += "<color=#0059FF>"; rackMessage += "<color=#0059FF>"; break;
                default: break;
            }
            debugMessage += activePlayer[callPlayer].rack.tiles[j].number + " </color>";
            rackMessage += activePlayer[callPlayer].rack.tiles[j].number + " </color>";
        }
        debugMessage += "</b>】";
        rackMessage += "</b>】";
        #endregion

        //當玩家確定自己牌架上的數字時，會立即進行猜測。
        //電腦一定會答對，但是電腦只會在剩餘一個組合時才會進行猜測，即使未確定的部分是不同顏色的相同數字

        activePlayer[callPlayer].BecomeCallPlayer();

        //畫面訊息刷新
        questionText.text = "";
        answerText.text = "";
        switch (Code777Menu.language)
        {
            case Language.Chinese:
                speakerText.text = "輪到 <color=#00C0FF>" + activePlayer[callPlayer].name + "</color> 作答數字";
                logInsert += "<color=#00C0FF><b>" + activePlayer[callPlayer].name + "</b></color>答對了牌架"+ rackMessage + " ";
                break;
            case Language.Japanese:
                speakerText.text = "<color=#00C0FF>" + activePlayer[callPlayer].name + "</color> のコール";
                logInsert += "<color=#00C0FF><b>" + activePlayer[callPlayer].name + "</b></color>がラック的中" + rackMessage + " ";
                break;
            case Language.English:
                speakerText.text = "<color=#00C0FF>" + activePlayer[callPlayer].name + "</color> calls the number.";
                logInsert += "<color=#00C0FF><b>" + activePlayer[callPlayer].name + "</b></color> calls correctly" + rackMessage + " ";
                break;
        }
        GameObject.Find("Player" + callPlayer + "Name").GetComponent<Text>().fontStyle = FontStyle.Bold;
        GameObject.Find("Player" + callPlayer + "Name").GetComponent<Text>().color = new Color(0, 0.75F, 1, 1);

        //暫停1秒
        yield return new WaitForSeconds(1);

        debugMessage = activePlayer[callPlayer].name + "答對了牌架" + debugMessage;
        //讓答對的效果跳出來
        //GameObject.Find("Player" + callPlayer + "Correct").SetActive(true);
        correctCall[callPlayer].SetActive(true);
        switch (Code777Menu.language)
        {
            case Language.Chinese:
                questionText.text = "<color=#00C0FF>" + activePlayer[callPlayer].name + "</color>答對了數字";
                break;
            case Language.Japanese:
                questionText.text = "<color=#00C0FF>" + activePlayer[callPlayer].name + "</color>あたります";
                break;
            case Language.English:
                questionText.text = "<color=#00C0FF>" + activePlayer[callPlayer].name + "</color>'s calling is correct.";
                break;
        }
        activePlayer[callPlayer].GetPoint();

        //暫停2秒
        yield return new WaitForSeconds(2);

        //目前邏輯是一定會答對
        correctCall[callPlayer].SetActive(false);
        if (activePlayer[callPlayer].victory>=3)
        {
            Debug.Log(debugMessage);
            logMessage.Add(logInsert);

            //如果玩家獲得3分，遊戲就結束。
            switch (Code777Menu.language)
            {
                case Language.Chinese:
                    questionText.text += "\n<color=#00C0FF>" + activePlayer[callPlayer].name + "</color>獲得勝利";
                    answerText.text = "遊戲結束";
                    logMessage.Add("<color=#FF8080>遊戲結束</color>");
                    if(activePlayer[0].victory>=1) logMessage.Add("AKino準備了這段訊息給你\n未來的哪一天也許你會用的上喔！");
                    break;
                case Language.Japanese:
                    questionText.text += "\n<color=#00C0FF>" + activePlayer[callPlayer].name + "</color>はゲームに勝ちます";
                    answerText.text = "ゲームオーバー";
                    logMessage.Add("<color=#FF8080>ゲームオーバー</color>");
                    if (activePlayer[0].victory >= 1) logMessage.Add("AKinoはこのメッセージを用意した\nいつか使えるかもしれませんよ！");
                    break;
                case Language.English:
                    questionText.text += "\n<color=#00C0FF>" + activePlayer[callPlayer].name + "</color> wins the game.";
                    answerText.text = "GAME OVER";
                    logMessage.Add("<color=#FF8080>GAME OVER</color>");
                    if (activePlayer[0].victory >= 1) logMessage.Add("AKino gives you a message.\nMaybe it will be useful someday!");
                    break;
            }

            logMessage.Add("<b>" + secretPassword + "</b>");

            JumpToLatestLog();

            //開啟Player側的覆蓋Tile
            playerTileBack[0].color = new Color(1, 1, 1, 0);
            playerTileBack[1].color = new Color(1, 1, 1, 0);
            playerTileBack[2].color = new Color(1, 1, 1, 0);

            activePlayer[callPlayer].IconRecover();
            GameObject.Find("Player" + callPlayer + "Name").GetComponent<Text>().fontStyle =
                callPlayer == answerPlayer ? FontStyle.Bold : FontStyle.Normal;
            GameObject.Find("Player" + callPlayer + "Name").GetComponent<Text>().color =
                callPlayer == answerPlayer ? new Color(1, 0.75F, 0, 1) : new Color(1, 1, 1, 1);

            restartButton.gameObject.SetActive(true);
            menuButton.gameObject.SetActive(true);
            status = GameStatus.GameSet;
        }
        else
        {
            //答對跟答錯時，玩家獲得的情報不一樣
            switch (Code777Menu.language)
            {
                case Language.Chinese:
                    questionText.text += "\n<color=#00C0FF>" + activePlayer[callPlayer].name + "</color>重設牌架";
                    break;
                case Language.Japanese:
                    questionText.text += "\n<color=#00C0FF>" + activePlayer[callPlayer].name + "</color>ラックリセット";
                    break;
                case Language.English:
                    questionText.text += "\n<color=#00C0FF>" + activePlayer[callPlayer].name + "</color> resets the rack.";
                    break;
            }

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

            activePlayer[callPlayer].NewRack(tile0, tile1, tile2, assistMode, advancedMode);


            //如果是答對的玩家，則可以確認自己剛才丟棄的TILE
            //也就是過濾時可以加入discardTile的情報
            activePlayer[callPlayer].RackCheck(activePlayer, discardTile);

            activePlayer[(callPlayer + 1) % 5].RackCheck(activePlayer, discardTile);
            activePlayer[(callPlayer + 2) % 5].RackCheck(activePlayer, discardTile);
            activePlayer[(callPlayer + 3) % 5].RackCheck(activePlayer, discardTile);
            activePlayer[(callPlayer + 4) % 5].RackCheck(activePlayer, discardTile);

            //discardTile要洗回Tile堆中
            tilePile.Add(discardTile[0]);
            tilePile.Add(discardTile[1]);
            tilePile.Add(discardTile[2]);
            discardTile.Clear();

            //玩家0的輔助模式
            TileLight();

            activePlayer[callPlayer].IconRecover();
            GameObject.Find("Player" + callPlayer + "Name").GetComponent<Text>().fontStyle =
                callPlayer == answerPlayer ? FontStyle.Bold : FontStyle.Normal;
            GameObject.Find("Player" + callPlayer + "Name").GetComponent<Text>().color =
                callPlayer == answerPlayer ? new Color(1, 0.75F, 0, 1) : new Color(1, 1, 1, 1);


            #region 紀錄玩家猜測後牌架
            debugMessage = debugMessage + "\n牌架更換為";
            debugMessage = debugMessage + "【<b> ";
            rackMessage = "【<b> ";
            for (int j = 0; j < 3; j++)
            {
                switch (activePlayer[callPlayer].rack.tiles[j].color)
                {
                    case "G": debugMessage += "<color=#009960>"; rackMessage += "<color=#009960>"; break;
                    case "Y": debugMessage += "<color=#F2CC00>"; rackMessage += "<color=#F2CC00>"; break;
                    case "K": debugMessage += "<color=#404040>"; rackMessage += "<color=#808080>"; break;
                    case "B": debugMessage += "<color=#A64C26>"; rackMessage += "<color=#A64C26>"; break;
                    case "R": debugMessage += "<color=#D90000>"; rackMessage += "<color=#D90000>"; break;
                    case "P": debugMessage += "<color=#D91ACC>"; rackMessage += "<color=#D91ACC>"; break;
                    case "C": debugMessage += "<color=#0059FF>"; rackMessage += "<color=#0059FF>"; break;
                    default: break;
                }
                debugMessage += activePlayer[callPlayer].rack.tiles[j].number + " </color>";
                rackMessage += activePlayer[callPlayer].rack.tiles[j].number + " </color>";
            }
            debugMessage += "</b>】";
            rackMessage += "</b>】";
            #endregion

            switch (Code777Menu.language)
            {
                case Language.Chinese:
                    logInsert += "\n<color=#00C0FF><b>" + activePlayer[callPlayer].name + "</b></color>牌架重設為" + rackMessage ;
                    break;
                case Language.Japanese:
                    logInsert += "\n<color=#00C0FF><b>" + activePlayer[callPlayer].name + "</b></color>ラックリセット" + rackMessage ;
                    break;
                case Language.English:
                    logInsert += "\n<color=#00C0FF><b>" + activePlayer[callPlayer].name + "</b></color>Rack reset" + rackMessage ;
                    break;
            }
            logMessage.Add(logInsert);
            JumpToLatestLog();
            Debug.Log(debugMessage);

            yield return new WaitForSeconds(2);
            status = GameStatus.NumberCall;
        }
    }

    IEnumerator PlayerCallPrepare()
    {
        playerCall = false;
        activePlayer[0].BecomeCallPlayer();

        numberTile[0].sprite = tileSpriteBack;
        numberTile[1].sprite = tileSpriteBack;
        numberTile[2].sprite = tileSpriteBack;
        numberCallTile[0] = -1;
        numberCallTile[1] = -1;
        numberCallTile[2] = -1;

        //畫面訊息刷新
        questionText.text = "";
        answerText.text = "";
        switch (Code777Menu.language)
        {
            case Language.Chinese:
                speakerText.text = "輪到 <color=#00C0FF>" + activePlayer[0].name + "</color> 作答數字";
                break;
            case Language.Japanese:
                speakerText.text = "<color=#00C0FF>" + activePlayer[0].name + "</color> のコール";
                break;
            case Language.English:
                speakerText.text = "<color=#00C0FF>" + activePlayer[0].name + "</color> calls the number.";
                break;
        }
        GameObject.Find("Player0Name").GetComponent<Text>().fontStyle = FontStyle.Bold;
        GameObject.Find("Player0Name").GetComponent<Text>().color = new Color(0, 0.75F, 1, 1);

        //暫停1秒→不暫停
        yield return new WaitForSeconds(0.5F);

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
        string guessMessage = "";
        string rackMessage = "";
        string logInsert = "";

        #region 紀錄玩家猜測前牌架
        debugMessage = "【<b> ";
        rackMessage = "【<b> ";
        for (int j = 0; j < 3; j++)
        {
            switch (activePlayer[0].rack.tiles[j].color)
            {
                case "G": debugMessage += "<color=#009960>"; rackMessage += "<color=#009960>"; break;
                case "Y": debugMessage += "<color=#F2CC00>"; rackMessage += "<color=#F2CC00>"; break;
                case "K": debugMessage += "<color=#404040>"; rackMessage += "<color=#808080>"; break;
                case "B": debugMessage += "<color=#A64C26>"; rackMessage += "<color=#A64C26>"; break;
                case "R": debugMessage += "<color=#D90000>"; rackMessage += "<color=#D90000>"; break;
                case "P": debugMessage += "<color=#D91ACC>"; rackMessage += "<color=#D91ACC>"; break;
                case "C": debugMessage += "<color=#0059FF>"; rackMessage += "<color=#0059FF>"; break;
                default: break;
            }
            debugMessage += activePlayer[0].rack.tiles[j].number + " </color>";
            rackMessage += activePlayer[0].rack.tiles[j].number + " </color>";
        }
        debugMessage += "</b>】";
        rackMessage += "</b>】";
        #endregion


        List<int> sortAnswer = new List<int>(3);
        sortAnswer.Add(numberCallTile[0]);
        sortAnswer.Add(numberCallTile[1]);
        sortAnswer.Add(numberCallTile[2]);
        sortAnswer.Sort();
        string defineAnswer = "";
        guessMessage = "【<b> ";
        for (int i=0;i<3;i++)
        {
            if( advancedMode )
            {
                switch(sortAnswer[i])
                {
                    case 0: defineAnswer += "1G"; guessMessage += "<color=#009960>1</color> "; break;
                    case 1: defineAnswer += "2Y"; guessMessage+= "<color=#F2CC00>2</color> "; break;
                    case 2: defineAnswer += "3K"; guessMessage+= "<color=#808080>3</color> "; break;
                    case 3: defineAnswer += "4B"; guessMessage+= "<color=#A64C26>4</color> "; break;
                    case 4: defineAnswer += "5R"; guessMessage+= "<color=#D90000>5</color> "; break;
                    case 5: defineAnswer += "5K"; guessMessage+= "<color=#808080>5</color> "; break;
                    case 6: defineAnswer += "6P"; guessMessage+= "<color=#D91ACC>6</color> "; break;
                    case 7: defineAnswer += "6G"; guessMessage+="<color=#009960>6</color> "; break;
                    case 8: defineAnswer += "7Y"; guessMessage+= "<color=#F2CC00>7</color> "; break;
                    case 9: defineAnswer += "7P"; guessMessage+= "<color=#D91ACC>7</color> "; break;
                    case 10: defineAnswer += "7C"; guessMessage+= "<color=#0059FF>7</color> "; break;
                }
            }
            else
            {
                defineAnswer += (sortAnswer[i] + 1).ToString();
                guessMessage += (sortAnswer[i] + 1).ToString() + " ";
            }
        }
        guessMessage += "</b>】";

        //正誤判定
        if ( (advancedMode && defineAnswer == activePlayer[0].rack.numberColorMatch) || (!advancedMode && defineAnswer == activePlayer[0].rack.numberMatch))
        {

            debugMessage = activePlayer[0].name + "答對了牌架" + debugMessage;

            //正解
            correctCall[0].SetActive(true);
            switch (Code777Menu.language)
            {
                case Language.Chinese:
                    questionText.text = "<color=#00C0FF>" + activePlayer[0].name + "</color>答對了數字";
                    logInsert += "<color=#00C0FF><b>" + activePlayer[0].name + "</b></color>答對了牌架" + rackMessage + " ";
                    break;
                case Language.Japanese:
                    questionText.text = "<color=#00C0FF>" + activePlayer[0].name + "</color>あたります";
                    logInsert += "<color=#00C0FF><b>" + activePlayer[0].name + "</b></color>がラック的中" + rackMessage + " ";
                    break;
                case Language.English:
                    questionText.text = "<color=#00C0FF>" + activePlayer[0].name + "</color>'s calling is correct.";
                    logInsert += "<color=#00C0FF><b>" + activePlayer[0].name + "</b></color> calls correctly" + rackMessage + " ";
                    break;
            }
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
                logMessage.Add(logInsert);

                activePlayer[0].IconRecover();
                GameObject.Find("Player0Name").GetComponent<Text>().fontStyle =
                    0 == answerPlayer ? FontStyle.Bold : FontStyle.Normal;
                GameObject.Find("Player0Name").GetComponent<Text>().color =
                    0 == answerPlayer ? new Color(1, 0.75F, 0, 1) : new Color(1, 1, 1, 1);

                switch (Code777Menu.language)
                {
                    case Language.Chinese:
                        questionText.text += "\n<color=#00C0FF>" + activePlayer[0].name + "</color>獲得勝利";
                        answerText.text = "遊戲結束";
                        logMessage.Add("<color=#FF8080>遊戲結束</color>");
                        logMessage.Add("AKino準備了這段訊息給你\n未來的哪一天也許你會用的上喔！");
                        break;
                    case Language.Japanese:
                        questionText.text += "\n<color=#00C0FF>" + activePlayer[0].name + "</color>はゲームに勝ちます";
                        answerText.text = "ゲームオーバー";
                        logMessage.Add("<color=#FF8080>ゲームオーバー</color>");
                        logMessage.Add("AKinoはこのメッセージを用意した\nいつか使えるかもしれませんよ！");
                        break;
                    case Language.English:
                        questionText.text += "\n<color=#00C0FF>" + activePlayer[0].name + "</color> wins the game.";
                        answerText.text = "GAME OVER";
                        logMessage.Add("<color=#FF8080>GAME OVER</color>");
                        logMessage.Add("AKino gives you a message.\nMaybe it will be useful someday!");
                        break;
                }
                logMessage.Add("<b>"+secretPassword+"</b>");
                JumpToLatestLog();

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
                switch (Code777Menu.language)
                {
                    case Language.Chinese:
                        questionText.text += "\n<color=#00C0FF>" + activePlayer[0].name + "</color>重設牌架";
                        logInsert += "\n<color=#00C0FF><b>" + activePlayer[0].name + "</b></color> 重設牌架";
                        break;
                    case Language.Japanese:
                        questionText.text += "\n<color=#00C0FF>" + activePlayer[0].name + "</color>ラックリセット";
                        logInsert += "\n<color=#00C0FF><b>" + activePlayer[0].name + "</b></color>ラックリセット";
                        break;
                    case Language.English:
                        questionText.text += "\n<color=#00C0FF>" + activePlayer[0].name + "</color> resets the rack.";
                        logInsert += "\n<color=#00C0FF><b>" + activePlayer[0].name + "</b></color> resets the rack.";
                        break;
                }

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

                activePlayer[1].RackCheck(activePlayer, discardTile);
                activePlayer[2].RackCheck(activePlayer, discardTile);
                activePlayer[3].RackCheck(activePlayer, discardTile);
                activePlayer[4].RackCheck(activePlayer, discardTile);

                //discardTile要洗回Tile堆中
                tilePile.Add(discardTile[0]);
                tilePile.Add(discardTile[1]);
                tilePile.Add(discardTile[2]);
                discardTile.Clear();

                //玩家0的輔助模式
                TileLight();

                activePlayer[0].IconRecover();
                GameObject.Find("Player0Name").GetComponent<Text>().fontStyle =
                    0== answerPlayer ? FontStyle.Bold : FontStyle.Normal;
                GameObject.Find("Player0Name").GetComponent<Text>().color =
                    0 == answerPlayer ? new Color(1, 0.75F, 0, 1) : new Color(1, 1, 1, 1);

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
                logMessage.Add(logInsert);
                JumpToLatestLog();

                yield return new WaitForSeconds(2);
                status = GameStatus.NumberCall;
            }
        }
        else
        {
            debugMessage = activePlayer[0].name + "答錯了牌架" + debugMessage;

            //答錯
            wrongCall[0].SetActive(true);
            switch (Code777Menu.language)
            {
                case Language.Chinese:
                    questionText.text = "<color=#00C0FF>" + activePlayer[0].name + "</color>答錯了數字";
                    logInsert += "<color=#00C0FF><b>" + activePlayer[0].name + "</b></color>猜測" + guessMessage + "但答錯了";
                    break;
                case Language.Japanese:
                    questionText.text = "<color=#00C0FF>" + activePlayer[0].name + "</color>はずれます";
                    logInsert += "<color=#00C0FF><b>" + activePlayer[0].name + "</b></color>が" + guessMessage + "コールしたがはずれた";
                    break;
                case Language.English:
                    questionText.text = "<color=#00C0FF>" + activePlayer[0].name + "</color>'s calling is wrong.";
                    logInsert += "<color=#00C0FF><b>" + activePlayer[0].name + "</b></color> calls " + guessMessage + " but wrong.";
                    break;
            }
            yield return new WaitForSeconds(2);

            wrongCall[0].SetActive(false);
            playerInputUI.SetActive(false);

            //答對跟答錯時，玩家獲得的情報不一樣
            switch (Code777Menu.language)
            {
                case Language.Chinese:
                    questionText.text += "\n<color=#00C0FF>" + activePlayer[0].name + "</color>重設牌架";
                    logInsert += "\n<color=#00C0FF><b>" + activePlayer[0].name + "</b></color>重設牌架";
                    break;
                case Language.Japanese:
                    questionText.text += "\n<color=#00C0FF>" + activePlayer[0].name + "</color>ラックリセット";
                    logInsert += "\n<color=#00C0FF><b>" + activePlayer[0].name + "</b></color>ラックリセット";
                    break;
                case Language.English:
                    questionText.text += "\n<color=#00C0FF>" + activePlayer[0].name + "</color> resets the rack.";
                    logInsert += "\n<color=#00C0FF><b>" + activePlayer[0].name + "</b></color> resets the rack.";
                    break;
            }

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

            activePlayer[1].RackCheck(activePlayer, discardTile);
            activePlayer[2].RackCheck(activePlayer, discardTile);
            activePlayer[3].RackCheck(activePlayer, discardTile);
            activePlayer[4].RackCheck(activePlayer, discardTile);

            //discardTile要洗回Tile堆中
            tilePile.Add(discardTile[0]);
            tilePile.Add(discardTile[1]);
            tilePile.Add(discardTile[2]);
            discardTile.Clear();

            //答錯的玩家 要在不知道自己原本牌架的情況下對下一個牌架做篩選
            activePlayer[0].RackCheck(activePlayer, discardTile);

            //玩家0的輔助模式
            TileLight();

            activePlayer[0].IconRecover();
            GameObject.Find("Player0Name").GetComponent<Text>().fontStyle =
                0 == answerPlayer ? FontStyle.Bold : FontStyle.Normal;
            GameObject.Find("Player0Name").GetComponent<Text>().color =
                0 == answerPlayer ? new Color(1, 0.75F, 0, 1) : new Color(1, 1, 1, 1);

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
            logMessage.Add(logInsert);
            JumpToLatestLog();

            yield return new WaitForSeconds(2);
            status = GameStatus.NumberCall;
        }

    }
}
