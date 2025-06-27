using UnityEngine;

public class PlayerSoundManager : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioSource WedgieaudioSource;

    [Header("Footstep Sounds")]
    public AudioClip[] footstepClips;
    public float footstepInterval = 0.3f; // 발소리 간격
    private float footstepTimer;

    [Header("Jump & Land Sounds")]
    public AudioClip jumpClip;
    public AudioClip landClip;

    [Header("Attack & Death")]
    public AudioClip grabstartClip;
    public AudioClip grabSuccessClip;
    public AudioClip pantyStretchedClip;
    public AudioClip pantySetPosClip;
    public AudioClip KillClip;
    public AudioClip DeathClip;

    public AudioClip clapClip;
    public AudioClip whistleClip;

    public AudioClip getPointClip;

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
    public void PlayGrabstart()
    {
        WedgieaudioSource.PlayOneShot(grabstartClip);
    }
    public void PlayGrabSuccess()
    {
        WedgieaudioSource.PlayOneShot(grabSuccessClip);
    }
    public void PlayStretching()
    {
        WedgieaudioSource.Stop();
        WedgieaudioSource.PlayOneShot(pantyStretchedClip);
    }
    public void PlaySetPos()
    {
        WedgieaudioSource.Stop();
        WedgieaudioSource.PlayOneShot(pantySetPosClip);
    }
    public void PlayKill()
    {
        WedgieaudioSource.Stop();
        WedgieaudioSource.PlayOneShot(KillClip);
    }
    public void PlayDeath()
    {
        WedgieaudioSource.Stop();
        WedgieaudioSource.PlayOneShot(DeathClip);
    }
    public void PlayClap()
    {
        WedgieaudioSource.Stop();
        WedgieaudioSource.PlayOneShot(clapClip);
    }
    public void PlayWistle()
    {
        audioSource.Stop();
        audioSource.PlayOneShot(whistleClip);
    }
    public void PlayGetPoint()
    {
        WedgieaudioSource.Stop();
        WedgieaudioSource.PlayOneShot(getPointClip);
    }
}
