using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public GameObject player1;
    public GameObject player2;
    public GameObject sheep;
    public GameObject timeTextMesh;
    public GameObject p1ScoreTextMesh;
    public GameObject p2ScoreTextMesh;
    public GameObject endSplashScreen;
    public GameObject player1WinText;
    public GameObject player2WinText;
    public GameObject equalityText;
    public GameObject losePointSound;
    public GameObject winPointSound;

    public bool hasStarted = false;
    private DateTime _creationTime = DateTime.Now;

    public int GetTime()
    {
        return (int)(DateTime.Now - _creationTime).TotalSeconds;
    }

    private int _player1Score = 0;
    private int _player2Score = 0;

    public int GetPlayer1Score()
    {
        return _player1Score;
    }

    public int GetPlayer2Score()
    {
        return _player2Score;
    }

    public void OnSheepInCenter()
    {
        if (IsGameFinished() || !hasStarted) return;
        var sheepBehavior = sheep.GetComponent<GhostSheepBehavior>();
        var sheepPosition = sheep.transform.position;
        var distance1 = Vector3.Distance(player1.transform.position, sheepPosition);
        var distance2 = Vector3.Distance(player2.transform.position, sheepPosition);
        if (distance1 < distance2)
        {
            _player1Score++;
            winPointSound.GetComponent<AudioSource>().Play();
        }
        else if (distance1 > distance2)
        {
            _player2Score++;
            winPointSound.GetComponent<AudioSource>().Play();
        }
        else /*TODO what if distances are equal (now +0 for both)*/ ;
    }

    public void OnGhostTouchPlayer(GameObject player)
    {
        if (IsGameFinished() || !hasStarted) return;
        if (player == player1)
        {
            _player1Score--;
            losePointSound.GetComponent<AudioSource>().Play();
        }
        if (player == player2)
        {
            _player2Score--;
            losePointSound.GetComponent<AudioSource>().Play();
            
        }
    }

    public bool IsGameFinished()
    {
        // now 3minutes games
        return GetTime() > 60 * 2;
    }

    public void Restart()
    {
        SceneManager.LoadScene("Menu");
    }

    public void Back()
    {
        SceneManager.LoadScene("Menu"); 
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    public void ButtonStartClick()
    {
        hasStarted = true;
        _creationTime = DateTime.Now;
    }

    // Update is called once per frame
    void Update()
    {
        if (!hasStarted) return;
        if (IsGameFinished())
        {
            endSplashScreen.SetActive(true);
            if (_player1Score>_player2Score) player1WinText.SetActive(true);
            else if (_player1Score<_player2Score) player2WinText.SetActive(true);
            else equalityText.SetActive(true);
        }
        else
        {
            int minutes = GetTime() / 60, seconds = GetTime() % 60;
            timeTextMesh.GetComponent<TextMeshProUGUI>().SetText($"{minutes}:{seconds}");
            p1ScoreTextMesh.GetComponent<TextMeshProUGUI>().SetText(GetPlayer1Score().ToString());
            p2ScoreTextMesh.GetComponent<TextMeshProUGUI>().SetText(GetPlayer2Score().ToString());
        }
    }
}