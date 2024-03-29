﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Tile
{
    #region 欄位宣告

    private int _number;
    private string _color;
    ///private Color _color;
    ///**Green(0.00, 0.60, 0.25), Yellow(0.95, 0.80, 0.00), blacK(0.25, 0.25, 0.25), Brown(0.65, 0.30, 0.15),**///
    ///**Red(0.85, 0.000, 0.000), Pink(0.85, 0.100, 0.80), Cyan(0.000, 0.35, 1.000)**///
    private float _xPosition;
    private float _yPosition;
    private float _zPosition;
    private Sprite _image;
    private string _objectName;
    private GameObject _gameObject;

    #endregion

    #region 屬性設定

    public int number { get { return _number; } }
    public string color { get { return _color; } }
    public float xPosition { get { return _xPosition; } set { _xPosition = value; } }
    public float yPosition { get { return _yPosition; } set { _yPosition = value; } }
    public float zPosition { get { return _zPosition; } set { _zPosition = value; } }
    public Sprite image { get { return _image; } }
    public string objectName { get { return _objectName; } }

    #endregion

    #region 建構方法

    /// <summary>
    /// 類別Tile會於場景生成物件的建構方法
    /// </summary>
    /// <param name="number"></param>
    /// <param name="color"></param>
    /// <param name="xPosition"></param>
    /// <param name="yPosition"></param>
    /// <param name="zPosition"></param>
    /// <param name="openTile"></param>
    /// <param name="image"></param>
    /// <param name="backImage"></param>
    /// <param name="objectName"></param>
    public Tile(int number, string color, float xPosition, float yPosition, float zPosition, Sprite image, string objectName, float scale=1F)
    {
        _number = number;
        _color = color;
        _xPosition = xPosition;
        _yPosition = yPosition;
        _zPosition = zPosition;
        _image = image;
        _objectName = objectName;

        _gameObject = new GameObject(objectName);
        _gameObject.AddComponent<SpriteRenderer>();
        _gameObject.GetComponent<SpriteRenderer>().sprite = image;
        _gameObject.GetComponent<Transform>().position = new Vector3(xPosition, yPosition, zPosition);
        _gameObject.GetComponent<Transform>().localScale = new Vector3(scale, scale, scale);
    }

    /// <summary>
    /// 類別Tile不於場景生成物件的建構方法
    /// </summary>
    /// <param name="number"></param>
    /// <param name="color"></param>
    /// <param name="image"></param>
    public Tile(int number, string color,  Sprite image)
    {
        _number = number;
        _color = color;
        _image = image;
    }


    #endregion

}
