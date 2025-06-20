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

    public TextMeshProUGUI usernameText;
    public TextMeshProUGUI respawnTimeText;
    public TextMeshProUGUI invincibleShieldText;

    public Button customizeButton;
    public Button settingButton;
    public Button tutorialButton;

    public Image wedgieHealthBar;

    public ScoreManager scoreManager;
    public CameraManager cameraManager;
    public PlayerMove playerMove;

    public float playTimer = 0;
    public TextMeshProUGUI playTimerText;
    public bool youdied = false;

    void Start()
    {
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
        cameraManager = GetComponent<CameraManager>();
        usernameText.text = scoreManager.userName;
        playerMove = GameObject.Find("Player").GetComponent<PlayerMove>();

        GameStart(10); //테스트용
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

        if (gameStarted) //게임 시작시 타이머 시작
        {
            if (playTimer > 0)
            {
                playTimer -= Time.deltaTime;
                UpdateTimerDisplay(playTimer);
            }
            else
            {
                GameEnd();
            }
        }
    }

    public void PauseGame()
    {
        if (!gameStarted && !youdied)
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
        if (gameStarted && !youdied)
        {
            cameraManager.SwitchCamera(0);
        }
        else if(!gameStarted && !youdied)
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
    public void GameStart(int timer)
    {
        gameStarted = true;
        playTimer = timer;
        playTimerText.text = playTimer.ToString();
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
        cameraManager.SwitchCamera(2);
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
    public void Respawn()
    {
        cameraManager.SwitchCamera(0);
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
}
