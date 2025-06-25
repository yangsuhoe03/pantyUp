using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using Unity.VisualScripting;

public class UIManager : MonoBehaviour
{
    public bool gameStarted = false;

    public GameObject pausePanel;
    public GameObject customPanel;
    public GameObject settingPanel;
    public GameObject rankingPanel;
    public GameObject tutorialPanel;
    public GameObject youdiedPanel;
    public GameObject invincibleShieldPanel;
    public GameObject restartPanel;

    public GameObject CurtainLeft;
    public GameObject CurtainRight;
    public GameObject RewardPlayer;

    public TextMeshProUGUI usernameText;
    public TextMeshProUGUI respawnTimeText;
    public TextMeshProUGUI invincibleShieldText;

    public Button customizeButton;
    public Button settingButton;
    public Button tutorialButton;
    public Button restartButton;

    public Image wedgieHealthBar;

    public ScoreManager scoreManager;
    public CameraManager cameraManager;
    public PlayerMove playerMove;

    public float playTimer = 0;
    public TextMeshProUGUI playTimerText;
    public bool youdied = false;
    public bool rewarding = false;
    SocketManager socketManager;
    public TextMeshPro rewardPlayerText;
    public PlayerSoundManager playerSoundManager;
    void Start()
    {
        socketManager = GameObject.Find("SocketManager").GetComponent<SocketManager>();
        if (pausePanel != null)
        {
            pausePanel.GetComponent<Button>().onClick.AddListener(ResumeGame);
        }
        if (customPanel != null)
        {
            customPanel.GetComponent<Button>().onClick.AddListener(CloseCustomPanel);
        }
        if (customizeButton != null)
        {
            customizeButton.onClick.AddListener(CustomPanel);
        }
        if (settingPanel != null)
        {
            settingPanel.GetComponent<Button>().onClick.AddListener(CloseSettingPanel);
        }
        if (settingButton != null)
        {
            settingButton.onClick.AddListener(SettingPanel);
        }
        if (tutorialButton != null)
        {
            tutorialButton.onClick.AddListener(TutorialPanel);
        }
        if (tutorialPanel != null)
        {
            tutorialPanel.GetComponent<Button>().onClick.AddListener(CloseTutorialPanel);
        }
        if (scoreManager == null)
        {
            scoreManager = GameObject.Find("ScoreManager").GetComponent<ScoreManager>();
        }
        if (restartButton != null)
        {
            restartButton.onClick.AddListener(GameSet);
        }
        cameraManager = GetComponent<CameraManager>();
        usernameText.text = scoreManager.userName;
        playerMove = GameObject.Find("Player").GetComponent<PlayerMove>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !pausePanel.activeSelf)
        {
            PauseGame();
        }
        else if (Input.GetKeyDown(KeyCode.Escape) && customPanel.activeSelf)
        {
            CloseCustomPanel();
        }
        else if (Input.GetKeyDown(KeyCode.Escape) && settingPanel.activeSelf)
        {
            CloseSettingPanel();
        }
        else if (Input.GetKeyDown(KeyCode.Escape) && tutorialPanel.activeSelf)
        {
            CloseTutorialPanel();
        }

        if (Input.GetKey(KeyCode.Tab) && !pausePanel.activeSelf)
        {
            rankingPanel.SetActive(true);
        }
        else
        {
            rankingPanel.SetActive(false);
        }
    }
    void LateUpdate()
    {
        if (rewarding)
        {

            Vector3 camPos = cameraManager.subCamera3.transform.position;

            // 수평 방향으로만 바라보도록 Y축 고정
            Vector3 direction = RewardPlayer.transform.position - camPos;
            direction.y = 0f; // 위아래 각도 제거

            if (direction != Vector3.zero)
            {
                rewardPlayerText.transform.rotation = Quaternion.LookRotation(direction);
            }
        }
    }
    public void PauseGame()
    {
        if (!gameStarted && !youdied && !rewarding)
        {
            cameraManager.SwitchCamera(0); //일단은 메인으로 돌리고 나중에 조건 나오면 그때 수정하기로
        }
        pausePanel.SetActive(true);
        SetCursorState(false);
        usernameText.text = scoreManager.userName;
    }

    public void CustomPanel()
    {
        customPanel.SetActive(true);
        scoreManager.nicknameInput.text = scoreManager.userName;
    }

    public void CloseCustomPanel()
    {
        customPanel.SetActive(false);
        usernameText.text = scoreManager.userName;
    }

    public void ResumeGame()
    {
        if (gameStarted && !youdied && !rewarding)
        {
            cameraManager.SwitchCamera(0);
        }
        else if (!gameStarted && !youdied && !rewarding)
        {
            cameraManager.SwitchCamera(0); //일단은 메인카메라로 돌아가게 함 게임 시작여부가 나오면 그 때 바꾸도록
        }
        pausePanel.SetActive(false);
        SetCursorState(true);
    }

    public void SettingPanel()
    {
        settingPanel.SetActive(true);
    }

    public void CloseSettingPanel()
    {
        settingPanel.SetActive(false);
    }

    public void TutorialPanel()
    {
        tutorialPanel.SetActive(true);
    }

    public void CloseTutorialPanel()
    {
        tutorialPanel.SetActive(false);
    }

    public void SetCursorState(bool locked)
    {
        Cursor.visible = !locked;
        Cursor.lockState = locked ? CursorLockMode.Locked : CursorLockMode.None;
    }
    public void UpdateTimerDisplay(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60);
        int seconds = Mathf.FloorToInt(time % 60);
        playTimerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
    public void GameEnd()
    {
        gameStarted = false;
        playTimerText.text = "00:00";
        StartCoroutine(RewardCamera());
    }
    public IEnumerator YouDied()
    {
        StopCoroutine(DeathCamera());
        StartCoroutine(DeathCamera());
        youdiedPanel.SetActive(true);
        youdied = true;

        float countdown = playerMove.respawnTime;
        while (countdown > 0f)
        {
            respawnTimeText.text = $"Respawning in {Mathf.CeilToInt(countdown)}...";
            yield return new WaitForSeconds(1f);
            countdown -= 1f;
        }

        youdiedPanel.SetActive(false);
        youdied = false;
    }
    IEnumerator DeathCamera()
    {
        if (!rewarding)
        {
            cameraManager.SwitchCamera(2);
        }
        cameraManager.subCamera2.transform.position = cameraManager.mainCamera.transform.position;
        cameraManager.subCamera2.transform.rotation = cameraManager.mainCamera.transform.rotation;
        Vector3 startPos = cameraManager.subCamera2.transform.position;
        Vector3 targetPos = startPos + new Vector3(0, 5, -5);

        float rotateSpeed = 2f;   // 회전 속도
        float duration = 10f;      // 연출 지속 시간
        float timer = 0f;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            cameraManager.subCamera2.transform.position = Vector3.Lerp(startPos, targetPos, timer / duration);
            Vector3 lookDir = playerMove.transform.position - cameraManager.subCamera2.transform.position;
            Quaternion targetRot = Quaternion.LookRotation(lookDir);
            cameraManager.subCamera2.transform.rotation = Quaternion.Slerp(cameraManager.subCamera2.transform.rotation, targetRot, rotateSpeed * Time.deltaTime);
            yield return null;
        }
    }
    IEnumerator RewardCamera()
    {
        rewarding = true;
        rewardPlayerText.text = scoreManager.GetTopPlayerNickname();
        Vector3 curtainLeftStartPos = CurtainLeft.transform.position;
        Vector3 curtainRightStartPos = CurtainRight.transform.position;
        cameraManager.SwitchCamera(3);
        float duration1 = 3f;
        float timer = 0f;
        Vector3 startPos = cameraManager.subCamera3.transform.position;
        Vector3 targetPos = cameraManager.subCamera3.transform.position + new Vector3(7f, 0, 0);
        cameraManager.subCamera3.transform.LookAt(RewardPlayer.transform.position + Vector3.up * 1f);

        playerSoundManager.PlayClap();

        while (timer < duration1)
        {
            timer += Time.deltaTime;
            float t = timer / duration1;
            cameraManager.subCamera3.transform.position = Vector3.Lerp(startPos, targetPos, t);
            CurtainLeft.transform.position = Vector3.Lerp(curtainLeftStartPos, curtainLeftStartPos + new Vector3(0, 0, 5f), t);
            CurtainRight.transform.position = Vector3.Lerp(curtainRightStartPos, curtainRightStartPos + new Vector3(0, 0, -5f), t);
            yield return null;
        }
        float duration2 = 3f;
        timer = 0f;
        float radiusNear = 1f;
        float heightNear = 0.7f;
        while (timer < duration2)
        {
            timer += Time.deltaTime;
            float angle = timer / duration2 * 100f + 0.25f;

            Vector3 offset = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), 0, Mathf.Sin(angle * Mathf.Deg2Rad)) * radiusNear;
            offset.y = heightNear;

            cameraManager.subCamera3.transform.position = RewardPlayer.transform.position + offset;
            cameraManager.subCamera3.transform.LookAt(RewardPlayer.transform.position + Vector3.up * 1.5f);
            yield return null;
        }
        timer = 0f;
        radiusNear = 2f;
        heightNear = 1.8f;
        while (timer < duration2)
        {
            timer += Time.deltaTime;
            float angle = -timer / duration2 * 100f + 0.8f;

            Vector3 offset = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), 0, Mathf.Sin(angle * Mathf.Deg2Rad)) * radiusNear;
            offset.y = -heightNear;

            cameraManager.subCamera3.transform.position = RewardPlayer.transform.position - offset;
            cameraManager.subCamera3.transform.LookAt(RewardPlayer.transform.position + Vector3.up * 1.5f);
            yield return null;
        }

        playerSoundManager.PlayClap();

        duration1 = 7f;
        timer = 0f;
        while (timer < duration1)
        {
            timer += Time.deltaTime;
            float t = timer / duration1;
            cameraManager.subCamera3.transform.position = Vector3.Lerp(targetPos, targetPos - new Vector3(0, 0, 3f), t);
            cameraManager.subCamera3.transform.LookAt(RewardPlayer.transform.position + Vector3.up * 1f);
            yield return null;
        }

        yield return new WaitForSeconds(1f);
        CurtainLeft.transform.position = curtainLeftStartPos;
        CurtainRight.transform.position = curtainRightStartPos;
        cameraManager.subCamera3.transform.position = startPos;

        PauseGame();
        restartPanel.SetActive(true);
        cameraManager.SwitchCamera(0);
        rewarding = false;
    }
    public void Respawn()
    {
        if (!rewarding)
        {
            cameraManager.SwitchCamera(0);
        }
    }
    public void UpdateWedgieHealthBar(float currenthealth)
    {
        wedgieHealthBar.fillAmount = currenthealth;
    }
    IEnumerator InvincibleShield()
    {
        playerMove.invincibleShield.SetActive(true);
        playerMove.attackPointBack.SetActive(false);
        invincibleShieldPanel.SetActive(true);

        float duration = 3f;
        float remaining = duration;

        while (remaining > 0f)
        {
            invincibleShieldText.text = $"Shield deactivating in {Mathf.Ceil(remaining).ToString()}..."; // 소수점 올림해서 보여줌 (3, 2, 1)
            yield return null;
            remaining -= Time.deltaTime;
        }

        invincibleShieldPanel.SetActive(false);
        playerMove.attackPointBack.SetActive(true);
        playerMove.invincibleShield.SetActive(false);
    }
    public void Invincible()
    {
        StartCoroutine(InvincibleShield());
    }
    public void GameSet()
    {
        restartPanel.SetActive(false);
        socketManager.Gamerestart(socketManager.mySocketID);
    }

}
