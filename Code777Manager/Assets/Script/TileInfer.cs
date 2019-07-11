using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TileInfer
{
    #region 欄位宣告
    private int _green1 = 0;
    private int _yellow2 = 0;
    private int _black3 = 0;
    private int _brown4 = 0;
    private int _red5 = 0;
    private int _black5 = 0;
    private int _pink6 = 0;
    private int _green6 = 0;
    private int _yellow7 = 0;
    private int _pink7 = 0;
    private int _cyan7 = 0;
    #endregion

    #region 屬性設定
    public int G1 { get { return _green1; } set { _green1 = value; } }
    public int Y2 { get { return _yellow2; } set { _yellow2 = value; } }
    public int K3 { get { return _black3; } set { _black3 = value; } }
    public int B4 { get { return _brown4; } set { _brown4 = value; } }
    public int R5 { get { return _red5; } set { _red5 = value; } }
    public int K5 { get { return _black5; } set { _black5 = value; } }
    public int P6 { get { return _pink6; } set { _pink6 = value; } }
    public int G6 { get { return _green6; } set { _green6 = value; } }
    public int Y7{ get { return _yellow7; } set { _yellow7 = value; } }
    public int P7 { get { return _pink7; } set { _pink7 = value; } }
    public int C7 { get { return _cyan7; } set { _cyan7 = value; } }
    #endregion

    #region 建構方法
    public TileInfer(bool possible)
    {
        _green1 = possible ? 1 : 0 ;
        _yellow2 = possible ? 2 : 0;
        _black3 = possible ? 3 : 0;
        _brown4 = possible ? 4 : 0;
        _red5 = possible ? 4 : 0;
        _black5 = possible ? 1 : 0;
        _pink6 = possible ? 3 : 0;
        _green6 = possible ? 3 : 0;
        _yellow7 = possible ? 2 : 0;
        _pink7 = possible ? 1 : 0;
        _cyan7 = possible ? 4 : 0;
    }
    #endregion
}
