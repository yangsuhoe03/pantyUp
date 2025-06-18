using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using System.Linq;

public class ScoreManager : MonoBehaviour
{
    private Dictionary<string, (int score, string nickname)> playerScores = new Dictionary<string, (int score, string nickname)>();
    public TextMeshProUGUI scoreText; // UI용 TextMeshPro
    public TMP_InputField nicknameInput; // 닉네임 입력 필드
    public TextMeshProUGUI randomNicknameText; // 랜덤 닉네임 표시 텍스트
    public string userName = null;

    private void Start()
    {
        if (scoreText == null)
        {
            Debug.LogWarning("Score TextMeshProUGUI가 할당되지 않았습니다.");
        }
        // 게임 시작 시 마우스 커서 표시
        //SetCursorState(false);
        if (userName == "")
        {
            userName = GenerateRandomNickname();
            GameObject socketManager = GameObject.Find("SocketManager");
            if (socketManager != null)
            {
                socketManager.GetComponent<SocketManager>().SetPlayerNickname(userName);
            }
        }
        nicknameInput.text = userName;
    }

    public void SetCursorState(bool locked)
    {
        Cursor.visible = !locked;
        Cursor.lockState = locked ? CursorLockMode.Locked : CursorLockMode.None;
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
                nickname = userName;
            }
            else
            {
                userName = nickname;
            }

            // SocketManager에 닉네임 전달
            GameObject socketManager = GameObject.Find("SocketManager");
            if (socketManager != null)
            {
                socketManager.GetComponent<SocketManager>().SetPlayerNickname(nickname);
            }

            /*
            // UI 패널 비활성화
            if (nicknamePanel != null)
            {
                nicknamePanel.SetActive(false);
            }
            */

            // 닉네임 설정이 완료되면 마우스 커서를 잠금
            //SetCursorState(true);
        }
    }

    public void ReceiveUpdatePlayerStatus(string data)
    {
        // 데이터 파싱
        string[] playerEntries = data.Split('|');
        playerScores.Clear(); // 기존 데이터 초기화

        foreach (string entry in playerEntries)
        {
            string[] parts = entry.Split(',');
            if (parts.Length == 3)
            {
                string playerId = parts[0];
                string nickname = parts[1];
                int score = int.Parse(parts[2]);

                playerScores[playerId] = (score, nickname);
            }
        }

        UpdateScoreDisplay();
    }

    private void UpdateScoreDisplay()
    {
        if (scoreText != null)
        {
            string displayText = "Score:\n";

            // 점수를 기준으로 내림차순 정렬
            var sortedScores = playerScores
                .OrderByDescending(x => x.Value.score)
                .ThenBy(x => x.Value.nickname); // 점수가 같을 경우 닉네임으로 정렬

            foreach (var kvp in sortedScores)
            {
                var (score, nickname) = kvp.Value;
                displayText += $"{nickname}: {score}\n";
            }
            scoreText.text = displayText;
        }
    }
}