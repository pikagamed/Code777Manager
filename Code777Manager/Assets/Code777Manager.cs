﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Code777Manager : MonoBehaviour
{
    public Sprite[] images;
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

    public GameObject[] playerIcon;
    public TextMesh[] playerName;

    public List<Tile> tilePile = new List<Tile>(28);    //未打開的Tile堆
    public List<Tile> setTile = new List<Tile>(3); //設置中的Tile暫存器
    public List<Tile> discardTile = new List<Tile>(28); //使用過被棄置於場中央的Tile堆

    public bool assistMode = true;  //輔助模式，此模式開啟下會於場景右下角提示可能TILE
    public GameObject[] assistTile;

    public List<Player> activePlayer;

    public List<Rack> racks = new List<Rack>(5);

    // Start is called before the first frame update
    void Start()
    {

        #region  初始化TILE堆
        initialTile[0] = new Tile(1, "G", images[0]);
        initialTile[1] = new Tile(2, "Y", images[1]);
        initialTile[2] = new Tile(2, "Y", images[1]);
        initialTile[3] = new Tile(3, "K", images[2]);
        initialTile[4] = new Tile(3, "K", images[2]);
        initialTile[5] = new Tile(3, "K", images[2]);
        initialTile[6] = new Tile(4, "B", images[3]);
        initialTile[7] = new Tile(4, "B", images[3]);
        initialTile[8] = new Tile(4, "B", images[3]);
        initialTile[9] = new Tile(4, "B", images[3]);
        initialTile[10] = new Tile(5, "R", images[4]);
        initialTile[11] = new Tile(5, "R", images[4]);
        initialTile[12] = new Tile(5, "R", images[4]);
        initialTile[13] = new Tile(5, "R", images[4]);
        initialTile[14] = new Tile(5, "K", images[5]);
        initialTile[15] = new Tile(6, "P", images[6]);
        initialTile[16] = new Tile(6, "P", images[6]);
        initialTile[17] = new Tile(6, "P", images[6]);
        initialTile[18] = new Tile(6, "G", images[7]);
        initialTile[19] = new Tile(6, "G", images[7]);
        initialTile[20] = new Tile(6, "G", images[7]);
        initialTile[21] = new Tile(7, "Y", images[8]);
        initialTile[22] = new Tile(7, "Y", images[8]);
        initialTile[23] = new Tile(7, "P", images[9]);
        initialTile[24] = new Tile(7, "C", images[10]);
        initialTile[25] = new Tile(7, "C", images[10]);
        initialTile[26] = new Tile(7, "C", images[10]);
        initialTile[27] = new Tile(7, "C", images[10]);

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

            playerIcon[i].GetComponent<SpriteRenderer>().sprite = activePlayerIcon;
            playerName[i].text = activePlayerName;

            Player newPlayer = new Player(activePlayerName, activePlayerIcon, i, 0, tileSprite, victoryLight);

            activePlayer.Add(newPlayer);

            presetPlayerName.Remove(activePlayerName);
            presetPlayerIcon.Remove(activePlayerIcon);

            /*
            Vector3[] playerTiles;
            switch(i)
            {
                case 1:
                    playerTiles = new Vector3[] { new Vector3(-6.25F, 1F, 0F), new Vector3(-5F, 1F, 0F), new Vector3(-3.75F, 1F, 0F) };
                    break;
                case 2:
                    playerTiles = new Vector3[] { new Vector3(-6.25F, 4F, 0F), new Vector3(-5F, 4F, 0F), new Vector3(-3.75F, 4F, 0F) };
                    break;
                case 3:
                    playerTiles = new Vector3[] { new Vector3(3.75F, 4F, 0F), new Vector3(5F, 4F, 0F), new Vector3(6.25F, 4F, 0F) };
                    break;
                case 4:
                    playerTiles = new Vector3[] { new Vector3(3.75F, 1F, 0F), new Vector3(5F, 1F, 0F), new Vector3(6.25F, 1F, 0F) };
                    break;
                default:
                    playerTiles = new Vector3[] { new Vector3(-2.25F, -3.25F, 0F), new Vector3(0F, -3.25F, 0F), new Vector3(2.25F, -3.25F, 0F) };
                    break;
            }
            */

            /*
            setTile.Clear();
            for (int j = 0; j < 3; j++)
            {
                drawTile = Random.Range(0, tilePile.Count);
                setTile.Add(tilePile[drawTile]);
                tilePile.Remove(tilePile[drawTile]);
            }

            Rack playerRack = new Rack(new Tile(setTile[0].number, setTile[0].color, playerTiles[0].x, playerTiles[0].y, playerTiles[0].z, setTile[0].image, "Player0Tile0", i==0?1F:0.75F),
                                            new Tile(setTile[1].number, setTile[1].color, playerTiles[1].x, playerTiles[1].y, playerTiles[1].z, setTile[1].image, "Player0Tile1", i == 0 ? 1F : 0.75F),
                                            new Tile(setTile[2].number, setTile[2].color, playerTiles[2].x, playerTiles[2].y, playerTiles[2].z, setTile[2].image, "Player0Tile2", i == 0 ? 1F : 0.75F));
            */
        }

        #endregion

        #region 初始化RACKS
        for(int i=0; i<5; i++)
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

            activePlayer[i].newRack(tile0, tile1, tile2);
        }
        #endregion

        //for (int i=0; i<28; i++)
        //{
        //    drawTile = Random.Range(0, tilePile.Count);
        //    discardTile.Add( new Tile(tilePile[drawTile].number, tilePile[drawTile].color, (i%7)*2F-8F, 3.75F-(i/7)*2.5F, 0,  true, tilePile[drawTile].image, images[11], "TileObject"+i.ToString() ) );
        //    tilePile.Remove(tilePile[drawTile]);
        //}


        //setTile.Clear();
        ////0號玩家(真玩家)


        //setTile.Clear();
        ////1號玩家(電腦1)
        //for (int i = 0; i < 3; i++)
        //{
        //    drawTile = Random.Range(0, tilePile.Count);
        //    setTile.Add(tilePile[drawTile]);
        //    tilePile.Remove(tilePile[drawTile]);
        //}
        //racks.Add(new Rack(new Tile(setTile[0].number, setTile[0].color, -6.25F, 1F, 0F, setTile[0].image, "Player1Tile0",0.75F),
        //                                new Tile(setTile[1].number, setTile[1].color, -5F, 1F, 0F, setTile[1].image, "Player1Tile1", 0.75F),
        //                                new Tile(setTile[2].number, setTile[2].color, -3.75F, 1F, 0F, setTile[2].image, "Player1Tile2", 0.75F)));


        //setTile.Clear();
        ////2號玩家(電腦2)
        //for (int i = 0; i < 3; i++)
        //{
        //    drawTile = Random.Range(0, tilePile.Count);
        //    setTile.Add(tilePile[drawTile]);
        //    tilePile.Remove(tilePile[drawTile]);
        //}

        //racks.Add( new Rack(new Tile(setTile[0].number, setTile[0].color, -6.25F, 4F, 0F, setTile[0].image, "Player2Tile0", 0.75F),
        //                                new Tile(setTile[1].number, setTile[1].color, -5F, 4F, 0F, setTile[1].image, "Player2Tile1", 0.75F),
        //                                new Tile(setTile[2].number, setTile[2].color, -3.75F, 4F, 0F, setTile[2].image, "Player2Tile2", 0.75F)));


        //setTile.Clear();
        ////3號玩家(電腦3)
        //for (int i = 0; i < 3; i++)
        //{
        //    drawTile = Random.Range(0, tilePile.Count);
        //    setTile.Add(tilePile[drawTile]);
        //    tilePile.Remove(tilePile[drawTile]);
        //}
        //racks.Add( new Rack(new Tile(setTile[0].number, setTile[0].color, 3.75F, 4F, 0F, setTile[0].image, "Player3Tile0", 0.75F),
        //                                new Tile(setTile[1].number, setTile[1].color, 5F, 4F, 0F, setTile[1].image, "Player3Tile1", 0.75F),
        //                                new Tile(setTile[2].number, setTile[2].color, 6.25F, 4F, 0F, setTile[2].image, "Player3Tile2", 0.75F)));

        //setTile.Clear();
        ////4號玩家(電腦4)
        //for (int i = 0; i < 3; i++)
        //{
        //    drawTile = Random.Range(0, tilePile.Count);
        //    setTile.Add(tilePile[drawTile]);
        //    tilePile.Remove(tilePile[drawTile]);
        //}

        //racks.Add(new Rack(new Tile(setTile[0].number, setTile[0].color, 3.75F, 1F, 0F, setTile[0].image, "Player4Tile0", 0.75F),
        //                                new Tile(setTile[1].number, setTile[1].color, 5F, 1F, 0F, setTile[1].image, "Player4Tile1", 0.75F),
        //                                new Tile(setTile[2].number, setTile[2].color, 6.25F, 1F, 0F, setTile[2].image, "Player4Tile2", 0.75F)));

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.anyKeyDown)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                //Debug.Log("PRESS SPACE");
                UnityEngine.SceneManagement.SceneManager.LoadScene("Code777GamePlay");
            }
        }
    }
}
