using UnityEngine;

public class BackGroundMusic : MonoBehaviour
{
    public bool isMuted;
    public AudioSource audioSource;
    public AudioClip win;
    public AudioClip lose;
    public AudioClip playerAttack;
    public AudioClip playerHit;
    public AudioClip playerDodge;
    public AudioClip enemyAttack1;
    public AudioClip enemyAttack2;
    public AudioClip heal;
    public AudioClip button;
    

    public void PlaySound(AudioClip sound)
    {
        if (!isMuted)
        {
            audioSource.PlayOneShot(sound);
        }
    }
    private void Awake()
    {
        Application.targetFrameRate = 120;
        GameObject[] goj = GameObject.FindGameObjectsWithTag("BackGroundMusic");
        if (goj.Length > 1)
        {
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
        }
        audioSource = GetComponent<AudioSource>();
    }
}

