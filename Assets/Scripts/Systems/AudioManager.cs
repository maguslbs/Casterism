using UnityEngine;

public class AudioManager : Singleton<AudioManager>
{
    [SerializeField] private AudioClip playCardSFX;
    [SerializeField] private AudioClip cardDrawSFX;
    [SerializeField] private AudioClip swingStaffSFX;
    [SerializeField] private AudioClip swordSliceSFX;
    [SerializeField] private AudioClip playerDeathSFX;
    [SerializeField] private AudioClip enemyDeathSFX;
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
        EnemyEvents.OnEnemyHit += SwordSlice;
        EnemyEvents.OnEnemyDeath += EnemyDeath;
        PlayerEvents.OnPlayerHit += SwingStaff;
        PlayerEvents.OnPlayerDeath += PlayerDeath;
        PlayerEvents.OnPlayerHealed += PlayerHealed;
        PlayerEvents.OnReshuffleRequested += Reshuffle;
    }

    private void OnDisable()
    {
        PlayerEvents.OnCardPlayed -= CardPlayed;
        PlayerEvents.OnDrawCardRequested -= CardDrawn;
        EnemyEvents.OnEnemyHit += SwordSlice;
        EnemyEvents.OnEnemyDeath += EnemyDeath;
        PlayerEvents.OnPlayerHit -= SwingStaff;
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

    private void SwordSlice(Enemy _)
    {
        PlaySFX(swordSliceSFX);
    }

    private void SwingStaff(int _)
    {
        PlaySFX(swingStaffSFX);
    }

    private void EnemyDeath(Enemy _)
    {
        PlaySFX(enemyDeathSFX);
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
