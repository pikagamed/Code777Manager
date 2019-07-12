using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum Language { Chinese, Japanese, English }
public enum Difficulty { Assist, Normal, Advanced }

public class Code777Menu : MonoBehaviour
{
    public GameObject menuUI;

    public Text languageText;
    public Text difficultyText;
    public Text hintText;
    public Text startText;

    public static Language language = Language.Chinese;
    public static Difficulty difficulty = Difficulty.Assist;

    // Start is called before the first frame update
    void Start()
    {
        switch (language)
        {
            case Language.English:
                languageText.text = "English";
                startText.text = "START";
                switch (difficulty)
                {
                    case Difficulty.Assist:
                        difficultyText.text = "Assist";
                        hintText.text = "The possibility of tiles is presented at right down corner";
                        break;
                    case Difficulty.Normal:
                        difficultyText.text = "Normal";
                        hintText.text = "The log since the latest tiles change is presented at right down corner";
                        break;
                    case Difficulty.Advanced:
                        difficultyText.text = "Advanced";
                        hintText.text = "You have to call the correct color for each tiles";
                        break;
                    default:
                        break;
                }
                break;
            case Language.Chinese:
                languageText.text = "中文";
                startText.text = "開始";
                switch (difficulty)
                {
                    case Difficulty.Assist:
                        difficultyText.text = "輔助模式";
                        hintText.text = "畫面右下角會提示剩餘數字牌的可能性";
                        break;
                    case Difficulty.Normal:
                        difficultyText.text = "一般模式";
                        hintText.text = "畫面右下角會提示上次更換數字牌開始的紀錄";
                        break;
                    case Difficulty.Advanced:
                        difficultyText.text = "進階模式";
                        hintText.text = "回答數字時顏色必須正確";
                        break;
                    default:
                        break;
                }
                break;
            case Language.Japanese:
                languageText.text = "日本語";
                startText.text = "スタート";
                switch (difficulty)
                {
                    case Difficulty.Assist:
                        difficultyText.text = "アシスト";
                        hintText.text = "残るタイルの可能性は右下隅で表してある";
                        break;
                    case Difficulty.Normal:
                        difficultyText.text = "ふつう";
                        hintText.text = "前回タイルチェンジからのログは右下隅で表してある";
                        break;
                    case Difficulty.Advanced:
                        difficultyText.text = "アドバンス";
                        hintText.text = "タイルのカラーも正しく答えなければならない";
                        break;
                    default:
                        break;
                }
                break;
            default:
                break;
        }

        //languageText.text = "中文";
        //difficultyText.text = "輔助模式";
        //hintText.text = "畫面右下角會提示剩餘數字牌的可能性";
        //startText.text = "開始";
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// 語言按鈕左鍵按下
    /// </summary>
    public void LanguageLeft()
    {
        //中文→英文；日文→中文；英文→日文
        switch (language)
        {
            case Language.Chinese:
                language = Language.English;
                languageText.text = "English";
                startText.text = "START";
                switch (difficulty)
                {
                    case Difficulty.Assist:
                        difficultyText.text = "Assist";
                        hintText.text = "The possibility of tiles is presented at right down corner";
                        break;
                    case Difficulty.Normal:
                        difficultyText.text = "Normal";
                        hintText.text = "The log since the latest tiles change is presented at right down corner";
                        break;
                    case Difficulty.Advanced:
                        difficultyText.text = "Advanced";
                        hintText.text = "You have to call the correct color for each tiles";
                        break;
                    default:
                        break;
                }
                break;
            case Language.Japanese:
                language = Language.Chinese;
                languageText.text = "中文";
                startText.text = "開始";
                switch (difficulty)
                {
                    case Difficulty.Assist:
                        difficultyText.text = "輔助模式";
                        hintText.text = "畫面右下角會提示剩餘數字牌的可能性";
                        break;
                    case Difficulty.Normal:
                        difficultyText.text = "一般模式";
                        hintText.text = "畫面右下角會提示上次更換數字牌開始的紀錄";
                        break;
                    case Difficulty.Advanced:
                        difficultyText.text = "進階模式";
                        hintText.text = "回答數字時顏色必須正確";
                        break;
                    default:
                        break;
                }
                break;
            case Language.English:
                language = Language.Japanese;
                languageText.text = "日本語";
                startText.text = "スタート";
                switch (difficulty)
                {
                    case Difficulty.Assist:
                        difficultyText.text = "アシスト";
                        hintText.text = "残るタイルの可能性は右下隅で表してある";
                        break;
                    case Difficulty.Normal:
                        difficultyText.text = "ふつう";
                        hintText.text = "前回タイルチェンジからのログは右下隅で表してある";
                        break;
                    case Difficulty.Advanced:
                        difficultyText.text = "アドバンス";
                        hintText.text = "タイルのカラーも正しく答えなければならない";
                        break;
                    default:
                        break;
                }
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// 語言按鈕右鍵按下
    /// </summary>
    public void LanguageRight()
    {
        //中文→日文；日文→英文；英文→中文
        switch (language)
        {
            case Language.Chinese:
                language = Language.Japanese;
                languageText.text = "日本語";
                startText.text = "スタート";
                switch (difficulty)
                {
                    case Difficulty.Assist:
                        difficultyText.text = "アシスト";
                        hintText.text = "残るタイルの可能性は右下隅で表してある";
                        break;
                    case Difficulty.Normal:
                        difficultyText.text = "ふつう";
                        hintText.text = "前回タイルチェンジからのログは右下隅で表してある";
                        break;
                    case Difficulty.Advanced:
                        difficultyText.text = "アドバンス";
                        hintText.text = "タイルのカラーも正しく答えなければならない";
                        break;
                    default:
                        break;
                }
                break;
            case Language.Japanese:
                language = Language.English;
                languageText.text = "English";
                startText.text = "START";
                switch (difficulty)
                {
                    case Difficulty.Assist:
                        difficultyText.text = "Assist";
                        hintText.text = "The possibility of tiles is presented at right down corner";
                        break;
                    case Difficulty.Normal:
                        difficultyText.text = "Normal";
                        hintText.text = "The log since the latest tiles change is presented at right down corner";
                        break;
                    case Difficulty.Advanced:
                        difficultyText.text = "Advanced";
                        hintText.text = "You have to call the correct color for each tiles";
                        break;
                    default:
                        break;
                }
                break;
            case Language.English:
                language = Language.Chinese;
                languageText.text = "中文";
                startText.text = "開始";
                switch (difficulty)
                {
                    case Difficulty.Assist:
                        difficultyText.text = "輔助模式";
                        hintText.text = "畫面右下角會提示剩餘數字牌的可能性";
                        break;
                    case Difficulty.Normal:
                        difficultyText.text = "一般模式";
                        hintText.text = "畫面右下角會提示上次更換數字牌開始的紀錄";
                        break;
                    case Difficulty.Advanced:
                        difficultyText.text = "進階模式";
                        hintText.text = "回答數字時顏色必須正確";
                        break;
                    default:
                        break;
                }
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// 難度按鈕左鍵按下
    /// </summary>
    public void DifficultyLeft()
    {
        //輔助→進階；一般→輔助；進階→一般
        switch (difficulty)
        {
            case Difficulty.Assist:
                difficulty = Difficulty.Advanced;
                switch (language)
                {
                    case Language.Chinese:
                        difficultyText.text = "進階模式";
                        hintText.text = "回答數字時顏色必須正確";
                        break;
                    case Language.Japanese:
                        difficultyText.text = "アドバンス";
                        hintText.text = "タイルのカラーも正しく答えなければならない";
                        break;
                    case Language.English:
                        difficultyText.text = "Advanced";
                        hintText.text = "You have to call the correct color for each tiles";
                        break;
                    default:
                        break;
                }
                break;
            case Difficulty.Normal:
                difficulty = Difficulty.Assist;
                switch (language)
                {
                    case Language.Chinese:
                        difficultyText.text = "輔助模式";
                        hintText.text = "畫面右下角會提示剩餘數字牌的可能性";
                        break;
                    case Language.Japanese:
                        difficultyText.text = "アシスト";
                        hintText.text = "残るタイルの可能性は右下隅で表してある";
                        break;
                    case Language.English:
                        difficultyText.text = "Assist";
                        hintText.text = "The possibility of tiles is presented at right down corner";
                        break;
                    default:
                        break;
                }
                break;
            case Difficulty.Advanced:
                difficulty = Difficulty.Normal;
                switch (language)
                {
                    case Language.Chinese:
                        difficultyText.text = "一般模式";
                        hintText.text = "畫面右下角會提示上次更換數字牌開始的紀錄";
                        break;
                    case Language.Japanese:
                        difficultyText.text = "ふつう";
                        hintText.text = "前回タイルチェンジからのログは右下隅で表してある";
                        break;
                    case Language.English:
                        difficultyText.text = "Normal";
                        hintText.text = "The log since the latest tiles change is presented at right down corner";
                        break;
                    default:
                        break;
                }
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// 難度按鈕右鍵按下
    /// </summary>
    public void DifficultyRight()
    {
        //輔助→一般；一般→進階；進階→輔助
        switch (difficulty)
        {
            case Difficulty.Assist:
                difficulty = Difficulty.Normal;
                switch (language)
                {
                    case Language.Chinese:
                        difficultyText.text = "一般模式";
                        hintText.text = "畫面右下角會提示上次更換數字牌開始的紀錄";
                        break;
                    case Language.Japanese:
                        difficultyText.text = "ふつう";
                        hintText.text = "前回タイルチェンジからのログは右下隅で表してある";
                        break;
                    case Language.English:
                        difficultyText.text = "Normal";
                        hintText.text = "The log since the latest tiles change is presented at right down corner";
                        break;
                    default:
                        break;
                }
                break;
            case Difficulty.Normal:
                difficulty = Difficulty.Advanced;
                switch (language)
                {
                    case Language.Chinese:
                        difficultyText.text = "進階模式";
                        hintText.text = "回答數字時顏色必須正確";
                        break;
                    case Language.Japanese:
                        difficultyText.text = "アドバンス";
                        hintText.text = "タイルのカラーも正しく答えなければならない";
                        break;
                    case Language.English:
                        difficultyText.text = "Advanced";
                        hintText.text = "You have to call the correct color for each tiles";
                        break;
                    default:
                        break;
                }
                break;
            case Difficulty.Advanced:
                difficulty = Difficulty.Assist;
                switch (language)
                {
                    case Language.Chinese:
                        difficultyText.text = "輔助模式";
                        hintText.text = "畫面右下角會提示剩餘數字牌的可能性";
                        break;
                    case Language.Japanese:
                        difficultyText.text = "アシスト";
                        hintText.text = "残るタイルの可能性は右下隅で表してある";
                        break;
                    case Language.English:
                        difficultyText.text = "Assist";
                        hintText.text = "The possibility of tiles is presented at right down corner";
                        break;
                    default:
                        break;
                }
                break;
            default:
                break;
        }
    }

    public void StartButton()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Code777GamePlay");
    }
}
