using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class LevelManager : SingletonBehaviour<LevelManager>
{

    [SerializeField] private CanvasGroup startGame;
    [SerializeField] private int _targetFrameRate = 60;
    public GameObject continueGame;
    public GameObject retryButton;
    private int _currentLevel = 0;
    private const string levelPrefsName = "Level_No";
    public int CurrentLevel { get { return _currentLevel; } }
    protected override void Awake()
    {
        base.Awake();
        Application.targetFrameRate = _targetFrameRate;
        _currentLevel = PlayerPrefs.GetInt(levelPrefsName,_currentLevel);
    }

    public void StartGame()
    {
        startGame.DOFade(0, .50f).SetEase(Ease.InSine);
        GameManager.Instance.gameStarted = true;
        StartCoroutine(CloseButton());

    }
    public void RetryGame() // level failed
    {
        SceneManager.LoadScene(0);
    }
    public void ContinueGame() // level passed
    {
        SceneManager.LoadScene(0);
        _currentLevel += 1;
        PlayerPrefs.SetInt(levelPrefsName, _currentLevel);
    }
    private IEnumerator CloseButton()
    {
        yield return new WaitForSeconds(.7f);
        startGame.gameObject.SetActive(false);
    }
}
