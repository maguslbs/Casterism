using UnityEngine;

public class AudioManager : Singleton<AudioManager>
{
    [SerializeField] private AudioClip playCardSFX;
    [SerializeField] private AudioClip cardDrawSFX;
    [SerializeField] private AudioClip swingStaffSFX;
    [SerializeField] private AudioClip swordSliceSFX;
    [SerializeField] private AudioClip playerDeathSFX;
    [SerializeField] private AudioClip bossDeathSFX;
    [SerializeField] private AudioClip healSFX;
    [SerializeField] private AudioClip reshuffleSFX;

    private AudioSource audioSource;

    protected override void Awake()
    {
        base.Awake();
        audioSource = gameObject.GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        PlayerEvents.OnCardPlayed += CardPlayed;
        PlayerEvents.OnDrawCardRequested += CardDrawn;
        BossEvents.OnBossHit += SwordSlice;
        PlayerEvents.OnPlayerHit += SwingStaff;
        BossEvents.OnBossDeath += BossDeath;
        PlayerEvents.OnPlayerDeath += PlayerDeath;
        PlayerEvents.OnPlayerHealed += PlayerHealed;
        PlayerEvents.OnReshuffleRequested += Reshuffle;
    }

    private void OnDisable()
    {
        PlayerEvents.OnCardPlayed -= CardPlayed;
        PlayerEvents.OnDrawCardRequested -= CardDrawn;
        BossEvents.OnBossHit -= SwordSlice;
        PlayerEvents.OnPlayerHit -= SwingStaff;
        BossEvents.OnBossDeath -= BossDeath;
        PlayerEvents.OnPlayerDeath -= PlayerDeath;
        PlayerEvents.OnPlayerHealed -= PlayerHealed;
        PlayerEvents.OnReshuffleRequested -= Reshuffle;
    }

    private void CardPlayed(CardData _)
    {
        PlaySFX(playCardSFX);
    }

    private void CardDrawn()
    {
        PlaySFX(cardDrawSFX);
    }

    private void SwordSlice(CardData _)
    {
        PlaySFX(swordSliceSFX);
    }

    private void SwingStaff(int _)
    {
        PlaySFX(swingStaffSFX);
    }

    private void BossDeath()
    {
        PlaySFX(bossDeathSFX);
    }

    private void PlayerDeath()
    {
        PlaySFX(playerDeathSFX);
    }

    private void PlayerHealed()
    {
        PlaySFX(healSFX);
    }

    private void Reshuffle()
    {
        PlaySFX(reshuffleSFX);
    }


    private void PlaySFX(AudioClip audioClip)
    {
        if (audioClip)
            audioSource.PlayOneShot(audioClip);
    }
}
