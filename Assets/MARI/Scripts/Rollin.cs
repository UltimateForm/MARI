using System.Collections;
using UniRx;
using UnityEngine;
using System.Diagnostics;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.UI;


[Flags]
public enum GameStates
{
    Iddle = 1,
    Playing = 2,
    Paused = 4,
    Focused = 8,
    Unfocused = 16,
    MainMenu = 32,
    InLevel = 64
}

public class Rollin : MonoBehaviour, IGameController {
    public struct Constants
    {
        public const string HS_MIN_KEY = "hs_mins";
        public const string HS_SECS_KEY = "hs_secs";
    }

    string hsMinKey = Constants.HS_MIN_KEY;
    string hsSecsKey = Constants.HS_SECS_KEY;

    public Stopwatch Watch;

    public void GameStart()
    {
        InitUi.gameObject.SetActive(false);
        Watch = new Stopwatch();
        SetState(GameStates.Playing);
        Watch.Reset();
        Watch.Start();
    }

    public void Restart()
    {
        SceneManager.LoadScene(0);
    }

    public RectTransform InitUi;
    public RectTransform GameOverUI;
    public RectTransform VictoryUI;
    public Text counter;
    public Text record;
    public static Rollin Main;
    public void Start()
    {
        Main = this;
        InitUi.gameObject.SetActive(true);
    }

    public Image hsIcon;

    public GameStates State { get; set; } = GameStates.Iddle;
    public event Action<GameStates> OnStateChange;

    public void Victory()
    {
        SetState(GameStates.Paused);
        var span = Watch.Elapsed;
        Watch.Stop();
        var timestring = $"{((int)span.TotalMinutes).ToString("00")}:{span.Seconds.ToString("00")}";
        var hsString = "";
        var mins = (int) span.TotalMinutes;
        var secs = (int)span.Seconds;
        var hsMins = 99;
        var hsSecs = 99;
        if (PlayerPrefs.HasKey(hsSecsKey))
        {
            hsMins = PlayerPrefs.GetInt(hsMinKey);
            hsSecs = PlayerPrefs.GetInt(hsSecsKey);
        }
        if (mins< hsMins)
        {
            PlayerPrefs.SetInt(hsMinKey, mins);
            PlayerPrefs.SetInt(hsSecsKey, secs);
            hsIcon.color = Color.yellow;
            hsString = $"{mins.ToString("00")}:{secs.ToString("00")}";

        }
        else if (secs < hsSecs)
        {
            PlayerPrefs.SetInt(hsMinKey, mins);
            PlayerPrefs.SetInt(hsSecsKey, secs);
            hsIcon.color = Color.yellow;
            hsString = $"{mins.ToString("00")}:{secs.ToString("00")}";

        }
        else
        {
            hsString = $"{hsMins.ToString("00")}:{hsSecs.ToString("00")}";
        }

        counter.text = "Time: " + timestring;
        record.text = "Record: " + hsString;
        VictoryUI.gameObject.SetActive(true);
    }

    public void GameOver()
    {
        SetState(GameStates.Paused);
        GameOverUI.gameObject.SetActive(true);
    }

    public void SetState(GameStates value)
    {
        State = value;
        OnStateChange?.Invoke(State);
    }

    public void AddState(GameStates value)
    {
        State |= value;
        OnStateChange?.Invoke(State);
    }

    public void RemoveState(GameStates value)
    {
        State &= ~value;
        OnStateChange?.Invoke(State);
    }
}
