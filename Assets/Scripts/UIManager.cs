using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public bool gameStarted = false;

    public GameObject pausePanel;
    public GameObject customPanel;
    public GameObject settingPanel;

    public TextMeshProUGUI usernameText;

    public Button customizeButton;
    public Button settingButton;

    public ScoreManager scoreManager;
    public CameraManager cameraManager;

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
        if (scoreManager == null)
        {
            scoreManager = GameObject.Find("ScoreManager").GetComponent<ScoreManager>();
        }
        cameraManager = GetComponent<CameraManager>();
        usernameText.text = scoreManager.userName;
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
    }

    public void PauseGame()
    {
        if (!gameStarted)
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
        if(gameStarted)
        {
            cameraManager.SwitchCamera(0);
        }
        else
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

    public void SetCursorState(bool locked)
    {
        Cursor.visible = !locked;
        Cursor.lockState = locked ? CursorLockMode.Locked : CursorLockMode.None;
    }
}
