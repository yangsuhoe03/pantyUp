using UnityEngine;
using UnityEngine.UI;

public class SettingPanel : MonoBehaviour
{
    public Toggle soundToggle;

    void Start()
    {
        ResetSettings();

        if (soundToggle != null)
        {
            soundToggle.onValueChanged.AddListener(delegate {
                ToggleSound(soundToggle.isOn);
            });
        }
    }

    public void ResetSettings()
    {
        soundToggle.isOn = true; // 기본값은 사운드 ON
        ToggleSound(true);
    }

    public void ToggleSound(bool isOn)
    {
        // 오디오 켜기/끄기
        AudioListener.volume = isOn ? 1f : 0f;

        // (선택) 전역 볼륨 조절 방식 사용 시
        // AudioListener.volume = isOn ? 1f : 0f;
    }
}