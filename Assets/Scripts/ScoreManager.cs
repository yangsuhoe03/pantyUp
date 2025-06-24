using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using System.Linq;

public class ScoreManager : MonoBehaviour
{
    private Dictionary<string, (int score, string nickname)> playerScores = new Dictionary<string, (int score, string nickname)>();
    //public TextMeshProUGUI scoreText; // UI용 TextMeshPro
    public TextMeshProUGUI[] rankTexts;
    public TextMeshProUGUI[] rankTexts2;
    public TextMeshProUGUI[] rankTexts3;
    public TextMeshProUGUI myRankText;
    public TMP_InputField nicknameInput; // 닉네임 입력 필드
    public TextMeshProUGUI randomNicknameText; // 랜덤 닉네임 표시 텍스트
    public string userName = null;
    public SocketManager socketManager;
    public int myRank;

    private void Start()
    {
        socketManager = GameObject.Find("SocketManager").GetComponent<SocketManager>();

        /*
        if (scoreText == null)
        {
            Debug.LogWarning("Score TextMeshProUGUI가 할당되지 않았습니다.");
        }
        */
        // 게임 시작 시 마우스 커서 표시
        //SetCursorState(false);
        if (string.IsNullOrEmpty(userName))
        {
            userName = GenerateRandomNickname();
            if (socketManager != null)
            {
                socketManager.SetPlayerNickname(userName);
            }
        }
        nicknameInput.text = userName;
        UpdateScoreDisplay();
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

            if (socketManager != null)
            {
                socketManager.SetPlayerNickname(nickname);
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
        else
        {
            userName = GenerateRandomNickname();
            if (socketManager != null)
            {
                socketManager.SetPlayerNickname(userName);
            }
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

                if (playerId != socketManager.GetMySocketID())
                {
                    socketManager.playerDict[playerId].GetComponent<OtherPlayer>().SetNickname(nickname);
                }
            }
        }

        UpdateScoreDisplay();
    }

    private void UpdateScoreDisplay()
    {
        // 점수를 기준으로 내림차순 정렬
        /*
        var sortedScores = playerScores
            .OrderByDescending(x => x.Value.score)
            .ThenBy(x => x.Value.nickname); // 점수가 같을 경우 닉네임으로 정렬
            */

        var sortedScores = playerScores
            .OrderByDescending(x => x.Value.score)
            .ThenBy(x => x.Value.nickname)
            .ToList();

        string myId = socketManager.GetMySocketID();
        int myIndex = sortedScores.FindIndex(x => x.Key == myId);

        myRank = myIndex + 1;

        myRankText.text = $"your rank : {myRank}";

        foreach (var text in rankTexts)
        {
            text.text = "";
        }
        foreach (var text in rankTexts2)
        {
            text.text = "";
        }
        foreach (var text in rankTexts3)
        {
            text.text = "";
        }

        if (myIndex < 6)
        {
            for (int i = 0; i < Mathf.Min(6, sortedScores.Count); i++)
            {
                var (score, nickname) = sortedScores[i].Value;
                bool isMe = sortedScores[i].Key == myId;
                rankTexts[i].text = isMe ?
                    $"<color=#FFD700><b>{nickname} </b></color>" :
                    $"{nickname}";
                rankTexts2[i].text = isMe ?
                    $"<color=#FFD700><b>{i + 1}</b></color>" :
                    $"{i + 1}";
                rankTexts3[i].text = isMe ?
                    $"<color=#FFD700><b>{score}</b></color>" :
                    $"{score}";
            }
        }
        else
        {
            // 상위 5명 + 내 순위
            for (int i = 0; i < Mathf.Min(5, sortedScores.Count); i++)
            {
                var (score, nickname) = sortedScores[i].Value;
                rankTexts[i].text = $"{nickname}";
                rankTexts2[i].text = $"{i + 1}";
                rankTexts3[i].text = $"{score}";
            }

            // 내 순위는 마지막에
            if (myIndex >= 0 && myIndex < sortedScores.Count)
            {
                var (score, nickname) = sortedScores[myIndex].Value;
                rankTexts[5].text = $"<color=#FFD700><b>{nickname}</b></color>";
                rankTexts2[5].text = $"<color=#FFD700><b>{myIndex + 1}</b></color>";
                rankTexts3[5].text = $"<color=#FFD700><b>{score}</b></color>";
            }
        }
        /*
        foreach (var kvp in sortedScores)
        {
            var (score, nickname) = kvp.Value;
            displayText += $"{nickname}: {score}\n";
        }
        */
    }
    public string GetTopPlayerNickname()
        {
            if (playerScores.Count == 0) return "";

            var sorted = playerScores
                .OrderByDescending(x => x.Value.score)
                .ThenBy(x => x.Value.nickname)
                .ToList();

            return sorted[0].Value.nickname;
        }
}