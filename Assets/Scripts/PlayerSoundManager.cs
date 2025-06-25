using UnityEngine;

public class PlayerSoundManager : MonoBehaviour
{
    public AudioSource audioSource;

    [Header("Footstep Sounds")]
    public AudioClip[] footstepClips;
    public float footstepInterval = 0.3f; // 발소리 간격
    private float footstepTimer;

    [Header("Jump & Land Sounds")]
    public AudioClip jumpClip;
    public AudioClip landClip;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayFootstep()
    {
        if (footstepClips.Length == 0) return;

        footstepTimer -= Time.deltaTime;

        if (footstepTimer <= 0f)
        {
            AudioClip clip = footstepClips[Random.Range(0, footstepClips.Length)];
            audioSource.PlayOneShot(clip);
            footstepTimer = footstepInterval;
        }


    }

    public void PlayJump()
    {
        audioSource.PlayOneShot(jumpClip);
    }

    public void PlayLand()
    {
        audioSource.PlayOneShot(landClip);
    }
}
