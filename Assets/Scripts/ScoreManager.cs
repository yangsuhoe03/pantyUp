using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class ScoreManager : MonoBehaviour
{
    private Dictionary<string, (int score, string nickname)> playerScores = new Dictionary<string, (int score, string nickname)>();
    public TextMeshProUGUI scoreText; // UI용 TextMeshPro
    public GameObject nicknamePanel; // 닉네임 설정 UI 패널
    public TMP_InputField nicknameInput; // 닉네임 입력 필드
    public TextMeshProUGUI randomNicknameText; // 랜덤 닉네임 표시 텍스트

    private void Start()
    {
        if (scoreText == null)
        {
            Debug.LogWarning("Score TextMeshProUGUI가 할당되지 않았습니다.");
        }
        GenerateRandomNickname();
        // 닉네임 설정 UI가 활성화되어 있을 때는 마우스 커서를 표시
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    private string GenerateRandomNickname()
    {
        return "User" + UnityEngine.Random.Range(1000, 9999);
    }

    public void UseRandomNickname()
    {
        if (randomNicknameText != null)
        {
            string randomNick = randomNicknameText.text;
            if (nicknameInput != null)
            {
                nicknameInput.text = randomNick;
            }
            GenerateRandomNickname(); // 새로운 랜덤 닉네임 생성
        }
    }

    public void SetNickname()
    {
        if (nicknameInput != null)
        {
            string nickname = nicknameInput.text.Trim();
            if (string.IsNullOrEmpty(nickname))
            {
                nickname = GenerateRandomNickname();
            }
            
            // SocketManager에 닉네임 전달
            GameObject socketManager = GameObject.Find("SocketManager");
            if (socketManager != null)
            {
                socketManager.GetComponent<SocketManager>().SetPlayerNickname(nickname);
            }

            // UI 패널 비활성화
            if (nicknamePanel != null)
            {
                nicknamePanel.SetActive(false);
            }

            // 닉네임 설정이 완료되면 마우스 커서를 잠금
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    public void UpdateScores(string scoreData)
    {
        string[] scoreEntries = scoreData.Split(',');
        
        foreach (string entry in scoreEntries)
        {
            string[] parts = entry.Split(':');
            if (parts.Length == 2)
            {
                string playerId = parts[0];
                int score = int.Parse(parts[1]);
                
                if (playerScores.ContainsKey(playerId))
                {
                    var (_, nickname) = playerScores[playerId];
                    playerScores[playerId] = (score, nickname);
                }
                else
                {
                    playerScores.Add(playerId, (score, "Player"));
                }
            }
        }

        UpdateScoreDisplay();
    }

    public void UpdatePlayerNickname(string playerId, string nickname)
    {
        if (playerScores.ContainsKey(playerId))
        {
            var (score, _) = playerScores[playerId];
            playerScores[playerId] = (score, nickname);
            UpdateScoreDisplay();
        }
    }

    private void UpdateScoreDisplay()
    {
        if (scoreText != null)
        {
            string displayText = "Score:\n";
            foreach (var kvp in playerScores)
            {
                var (score, nickname) = kvp.Value;
                displayText += $"{nickname}: {score}\n";
            }
            scoreText.text = displayText;
        }
    }

    public int GetPlayerScore(string playerId)
    {
        if (playerScores.ContainsKey(playerId))
        {
            return playerScores[playerId].score;
        }
        return 0;
    }
} 