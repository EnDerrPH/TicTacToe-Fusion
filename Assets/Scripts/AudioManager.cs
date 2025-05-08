using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    [SerializeField] AudioSource _audioBGM;
    [SerializeField] AudioSource _audioSFX;
    [SerializeField] AudioClip _UIButtonSFX;
    [SerializeField] AudioClip _cellButtonSFX;
    [SerializeField] AudioClip _winSFX;
    [SerializeField] AudioClip _loseSFX;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    void Start()
    {
        _audioBGM.Play();
    }

    public void OnCellSFX()
    {
        _audioSFX.PlayOneShot(_cellButtonSFX);
    }
}
