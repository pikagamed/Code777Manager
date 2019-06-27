using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Code777Manager : MonoBehaviour
{
    public Sprite[] images;

    #region 初始化TILE堆
    public static Tile[] initialTile = new Tile[28];

    #endregion

    public List<Tile> tilePile = new List<Tile>(28);    //未打開的Tile堆
    public List<Tile> assistTile = new List<Tile>(28);  //輔助模式下右下角的Tile提示
    public List<Tile> discardTile = new List<Tile>(28); //使用過被棄置於場中央的Tile堆

    public bool assistMode = true;  //輔助模式，此模式開啟下會於場景右下角提示可能

    // Start is called before the first frame update
    void Start()
    {
        #region  初始化TILE堆
            initialTile[0] = new Tile(1, new Color(0.00F, 0.60F, 0.25F), images[0]);
            initialTile[1] = new Tile(2, new Color(0.95F, 0.80F, 0.00F), images[1]);
            initialTile[2] = new Tile(2, new Color(0.95F, 0.80F, 0.00F), images[1]);
            initialTile[3] = new Tile(3, new Color(0.25F, 0.25F, 0.25F), images[2]);
            initialTile[4] = new Tile(3, new Color(0.25F, 0.25F, 0.25F), images[2]);
            initialTile[5] = new Tile(3, new Color(0.25F, 0.25F, 0.25F), images[2]);
            initialTile[6] = new Tile(4, new Color(0.65F, 0.30F, 0.15F), images[3]);
            initialTile[7] = new Tile(4, new Color(0.65F, 0.30F, 0.15F), images[3]);
            initialTile[8] = new Tile(4, new Color(0.65F, 0.30F, 0.15F), images[3]);
            initialTile[9] = new Tile(4, new Color(0.65F, 0.30F, 0.15F), images[3]);
            initialTile[10] = new Tile(5, new Color(0.85F, 0.00F, 0.00F), images[4]);
            initialTile[11] = new Tile(5, new Color(0.85F, 0.00F, 0.00F), images[4]);
            initialTile[12] = new Tile(5, new Color(0.85F, 0.00F, 0.00F), images[4]);
            initialTile[13] = new Tile(5, new Color(0.85F, 0.00F, 0.00F), images[4]);
            initialTile[14] = new Tile(5, new Color(0.25F, 0.25F, 0.25F), images[5]);
            initialTile[15] = new Tile(6, new Color(0.85F, 0.10F, 0.80F), images[6]);
            initialTile[16] = new Tile(6, new Color(0.85F, 0.10F, 0.80F), images[6]);
            initialTile[17] = new Tile(6, new Color(0.85F, 0.10F, 0.80F), images[6]);
            initialTile[18] = new Tile(6, new Color(0.00F, 0.60F, 0.25F), images[7]);
            initialTile[19] = new Tile(6, new Color(0.00F, 0.60F, 0.25F), images[7]);
            initialTile[20] = new Tile(6, new Color(0.00F, 0.60F, 0.25F), images[7]);
            initialTile[21] = new Tile(7, new Color(0.95F, 0.80F, 0.00F), images[8]);
            initialTile[22] = new Tile(7, new Color(0.95F, 0.80F, 0.00F), images[8]);
            initialTile[23] = new Tile(7, new Color(0.85F, 0.10F, 0.80F), images[9]);
            initialTile[24] = new Tile(7, new Color(0.00F, 0.35F, 1.00F), images[10]);
            initialTile[25] = new Tile(7, new Color(0.00F, 0.35F, 1.00F), images[10]);
            initialTile[26] = new Tile(7, new Color(0.00F, 0.35F, 1.00F), images[10]);
            initialTile[27] = new Tile(7, new Color(0.00F, 0.35F, 1.00F), images[10]);

            for(int i=0; i<28; i++)
            {
                tilePile.Add(initialTile[i]);
            }

            //AssistMode初始化
            if (assistMode)
            {
                for(int row=0; row<7; row++)
                {
                    for(int col=0; col<=row; col++)
                    {
                        int i = (row == 0) ? col : ((row) * (row + 1)/ 2 + col);
                        assistTile.Add(new Tile(tilePile[i].number, tilePile[i].color, 6.5F-row*0.3F+col*0.6F, -2F-row*0.4F, -0.1F - row * 0.1F, true, tilePile[i].image, images[11], "AssistTileObject" + i.ToString(), 0.25F));
                    }
                }

               //0
               //1 -> 1*2/2=1  -> 1+0 1+1
               //2 -> 2*3/2=3 -> 3+0 3+1 3+2
               //3 -> 3*4/2=6
            }

        #endregion

        int drawTile;

        for (int i=0; i<28; i++)
        {
            drawTile = Random.Range(0, tilePile.Count);
            discardTile.Add( new Tile(tilePile[drawTile].number, tilePile[drawTile].color, (i%7)*2F-8F, 3.75F-(i/7)*2.5F, 0,  true, tilePile[drawTile].image, images[11], "TileObject"+i.ToString() ) );
            tilePile.Remove(tilePile[drawTile]);
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.anyKeyDown)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene("Code777GamePlay");
            }
        }
    }
}
