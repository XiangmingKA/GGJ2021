using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public enum GameStatus
    {
        Playing,
        End
    }

    public GameStatus gameStatus = GameStatus.Playing;
    public GameObject gameoverUI;

    private void OnEnable()
    {
        Monster.OnPlayerTouchedMonster.AddListener(PlayerDead);
    }

    private void OnDisable()
    {
        Monster.OnPlayerTouchedMonster.RemoveListener(PlayerDead);
    }

    // Start is called before the first frame update
    void Start()
    {
        gameoverUI?.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void PlayerDead()
    {
        if (gameStatus == GameStatus.Playing)
        {
            gameStatus = GameStatus.End;
            gameoverUI?.SetActive(true);
            StartCoroutine(WaitAndReload());
        }
    }

    IEnumerator WaitAndReload()
    {
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
