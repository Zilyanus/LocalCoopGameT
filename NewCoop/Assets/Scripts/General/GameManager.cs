using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    List<GameObject> Players;
    [SerializeField] PlayerJoinManager playerJoinManager;
    [SerializeField] List<TextMeshProUGUI> PlayerScoresText;
    [SerializeField] List<int> PlayerScores;

    GameObject LastPlayer;
    // Start is called before the first frame update
    void Start()
    {
        Players = playerJoinManager.Players;
        for (int i = 0; i < Players.Count; i++)
        {
            PlayerScoresText[i].gameObject.SetActive(true);
            PlayerScoresText[i].color = playerJoinManager.PlayerColors[i];
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangeScore(int PlayerIndex,int Score)
    {
        PlayerScores[PlayerIndex - 1] += Score;
        PlayerScoresText[PlayerIndex - 1].text = "Player "+ PlayerIndex + "\n" + PlayerScores[PlayerIndex - 1];
        if (ControlPlayerDead())
        {
            StartCoroutine(WaitForRound());
        }
    }

    bool ControlPlayerDead()
    {
        int AliveCount = 0;
        foreach (var Player in Players)
        {
            if (Player.activeSelf)
            {
                AliveCount++;
                LastPlayer = Player;
            }
        }
        if (AliveCount > 1)
        {
            LastPlayer = null;
            return false;
        }
        return true;
    }

    void NextRound()
    {
        playerJoinManager.SpawnPlayers();
    }

    IEnumerator WaitForRound()
    {
        yield return new WaitForSeconds(2);
        NextRound();
    }
}
