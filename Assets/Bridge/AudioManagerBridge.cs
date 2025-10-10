using UnityEngine;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [System.Serializable]
    public class Sound
    {
        public string name;
        public AudioClip clip;
        [Range(0f, 1f)]
        public float volume = 1f;
        [Range(0.5f, 1.5f)]
        public float pitch = 1f;
        public bool loop = false;
        [HideInInspector]
        public AudioSource source;
    }

    [Header("Background Music")]
    public Sound backgroundMusic;

    [Header("Player Sounds")]
    public Sound playerWalk;
    public Sound playerJump;
    public Sound playerDoubleJump;
    public Sound playerAttack;
    public Sound playerCrouch;
    public Sound playerHurt;
    public Sound playerDeath;
    public Sound playerRespawn;

    [Header("Box Sounds")]
    public Sound boxBreak;
    public Sound boxExplosion;
    public Sound jumpBoost;

    [Header("Coin Sounds")]
    public Sound coinCollect;
    public Sound coinSpawn;

    [Header("Enemy Sounds")]
    public Sound goatDeath;
    public Sound goatPush;
    public Sound enemyHit;

    [Header("Trap Sounds")]
    public Sound logWarning;
    public Sound logRoll;
    public Sound trapActivate;

    [Header("UI Sounds")]
    public Sound checkpoint;
    public Sound gameOver;
    public Sound buttonClick;

    private Dictionary<string, Sound> soundDictionary = new Dictionary<string, Sound>();
    private List<Sound> allSounds = new List<Sound>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeAudioManager();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void InitializeAudioManager()
    {
        //  ⁄ÌÌ‰ «·√”„«¡  ·ﬁ«∆Ì« ≈–« ﬂ«‰  ›«—€…
        SetSoundNames();

        // Ã„⁄ ﬂ· «·√’Ê«  ›Ì ﬁ«∆„… Ê«Õœ…
        AddSoundToDictionary(backgroundMusic, "BackgroundMusic");

        // √’Ê«  «··«⁄»
        AddSoundToDictionary(playerWalk, "PlayerWalk");
        AddSoundToDictionary(playerJump, "PlayerJump");
        AddSoundToDictionary(playerDoubleJump, "PlayerDoubleJump");
        AddSoundToDictionary(playerAttack, "PlayerAttack");
        AddSoundToDictionary(playerCrouch, "PlayerCrouch");
        AddSoundToDictionary(playerHurt, "PlayerHurt");
        AddSoundToDictionary(playerDeath, "PlayerDeath");
        AddSoundToDictionary(playerRespawn, "PlayerRespawn");

        // √’Ê«  «·’‰«œÌﬁ
        AddSoundToDictionary(boxBreak, "BoxBreak");
        AddSoundToDictionary(boxExplosion, "BoxExplosion");
        AddSoundToDictionary(jumpBoost, "JumpBoost");

        // √’Ê«  «·⁄„·« 
        AddSoundToDictionary(coinCollect, "CoinCollect");
        AddSoundToDictionary(coinSpawn, "CoinSpawn");

        // √’Ê«  «·√⁄œ«¡
        AddSoundToDictionary(goatDeath, "GoatDeath");
        AddSoundToDictionary(goatPush, "GoatPush");
        AddSoundToDictionary(enemyHit, "EnemyHit");

        // √’Ê«  «·›Œ«Œ
        AddSoundToDictionary(logWarning, "LogWarning");
        AddSoundToDictionary(logRoll, "LogRoll");
        AddSoundToDictionary(trapActivate, "TrapActivate");

        // √’Ê«  «·Ê«ÃÂ…
        AddSoundToDictionary(checkpoint, "Checkpoint");
        AddSoundToDictionary(gameOver, "GameOver");
        AddSoundToDictionary(buttonClick, "ButtonClick");

        // ≈‰‘«¡ AudioSource ·ﬂ· ’Ê 
        foreach (Sound sound in allSounds)
        {
            if (sound.clip != null)
            {
                AudioSource source = gameObject.AddComponent<AudioSource>();
                source.clip = sound.clip;
                source.volume = sound.volume;
                source.pitch = sound.pitch;
                source.loop = sound.loop;
                sound.source = source;

                // «” Œœ«„ «·«”„ «·„Õœœ »œ·« „‰ sound.name
                string soundName = GetSoundName(sound);
                if (!string.IsNullOrEmpty(soundName))
                {
                    soundDictionary[soundName] = sound;
                    Debug.Log($"Added sound to dictionary: {soundName}");
                }
            }
        }

        //  ‘€Ì· «·„Ê”ÌﬁÏ «·Œ·›Ì…
        PlayBackgroundMusic();
    }

    void SetSoundNames()
    {
        //  ⁄ÌÌ‰ «·√”„«¡ ≈–« ﬂ«‰  ›«—€…
        if (string.IsNullOrEmpty(backgroundMusic.name)) backgroundMusic.name = "BackgroundMusic";
        if (string.IsNullOrEmpty(playerWalk.name)) playerWalk.name = "PlayerWalk";
        if (string.IsNullOrEmpty(playerJump.name)) playerJump.name = "PlayerJump";
        if (string.IsNullOrEmpty(playerDoubleJump.name)) playerDoubleJump.name = "PlayerDoubleJump";
        if (string.IsNullOrEmpty(playerAttack.name)) playerAttack.name = "PlayerAttack";
        if (string.IsNullOrEmpty(playerCrouch.name)) playerCrouch.name = "PlayerCrouch";
        if (string.IsNullOrEmpty(playerHurt.name)) playerHurt.name = "PlayerHurt";
        if (string.IsNullOrEmpty(playerDeath.name)) playerDeath.name = "PlayerDeath";
        if (string.IsNullOrEmpty(playerRespawn.name)) playerRespawn.name = "PlayerRespawn";

        if (string.IsNullOrEmpty(boxBreak.name)) boxBreak.name = "BoxBreak";
        if (string.IsNullOrEmpty(boxExplosion.name)) boxExplosion.name = "BoxExplosion";
        if (string.IsNullOrEmpty(jumpBoost.name)) jumpBoost.name = "JumpBoost";

        if (string.IsNullOrEmpty(coinCollect.name)) coinCollect.name = "CoinCollect";
        if (string.IsNullOrEmpty(coinSpawn.name)) coinSpawn.name = "CoinSpawn";

        if (string.IsNullOrEmpty(goatDeath.name)) goatDeath.name = "GoatDeath";
        if (string.IsNullOrEmpty(goatPush.name)) goatPush.name = "GoatPush";
        if (string.IsNullOrEmpty(enemyHit.name)) enemyHit.name = "EnemyHit";

        if (string.IsNullOrEmpty(logWarning.name)) logWarning.name = "LogWarning";
        if (string.IsNullOrEmpty(logRoll.name)) logRoll.name = "LogRoll";
        if (string.IsNullOrEmpty(trapActivate.name)) trapActivate.name = "TrapActivate";

        if (string.IsNullOrEmpty(checkpoint.name)) checkpoint.name = "Checkpoint";
        if (string.IsNullOrEmpty(gameOver.name)) gameOver.name = "GameOver";
        if (string.IsNullOrEmpty(buttonClick.name)) buttonClick.name = "ButtonClick";
    }

    void AddSoundToDictionary(Sound sound, string defaultName = "")
    {
        if (sound.clip != null)
        {
            // ≈–« ﬂ«‰ «·«”„ ›«—€«° «” Œœ„ «·«”„ «·«› —«÷Ì
            if (string.IsNullOrEmpty(sound.name) && !string.IsNullOrEmpty(defaultName))
            {
                sound.name = defaultName;
            }

            allSounds.Add(sound);
        }
    }

    string GetSoundName(Sound sound)
    {
        if (!string.IsNullOrEmpty(sound.name))
            return sound.name;

        // ≈–« ﬂ«‰ «·«”„ ·« Ì“«· ›«—€«° «” Œœ„ «”„ «·‹ clip
        if (sound.clip != null)
            return sound.clip.name;

        return "";
    }

    void PlayBackgroundMusic()
    {
        if (backgroundMusic.clip != null && backgroundMusic.source != null)
        {
            backgroundMusic.source.Play();
            Debug.Log("Background music started playing");
        }
        else
        {
            Debug.LogWarning("Background music clip or source is null!");
        }
    }

    public void PlaySound(string soundName)
    {
        if (soundDictionary.ContainsKey(soundName))
        {
            Sound sound = soundDictionary[soundName];
            if (sound.source != null && sound.clip != null)
            {
                sound.source.Play();
                Debug.Log($"Playing sound: {soundName}");
            }
            else
            {
                Debug.LogWarning($"Sound source or clip is null for: {soundName}");
            }
        }
        else
        {
            Debug.LogWarning($"Sound '{soundName}' not found in dictionary!");
            Debug.Log($"Available sounds: {string.Join(", ", soundDictionary.Keys)}");
        }
    }

    public void StopSound(string soundName)
    {
        if (soundDictionary.ContainsKey(soundName))
        {
            Sound sound = soundDictionary[soundName];
            if (sound.source != null && sound.source.isPlaying)
            {
                sound.source.Stop();
            }
        }
    }

    public void PlayOneShot(string soundName)
    {
        if (soundDictionary.ContainsKey(soundName))
        {
            Sound sound = soundDictionary[soundName];
            if (sound.source != null && sound.clip != null)
            {
                sound.source.PlayOneShot(sound.clip, sound.volume);
                Debug.Log($"Playing one shot: {soundName}");
            }
        }
        else
        {
            Debug.LogWarning($"Sound '{soundName}' not found for one shot!");
        }
    }

    // œÊ«· „”«⁄œ… ·√’Ê«  „Õœœ… - „⁄œ·… ·«” Œœ«„ «·√”„«¡ «·’ÕÌÕ…
    public void PlayFootstep()
    {
        if (playerWalk.clip != null)
            PlayOneShot("PlayerWalk");
    }

    public void PlayJumpSound(bool isDoubleJump = false)
    {
        if (isDoubleJump && playerDoubleJump.clip != null)
        {
            PlayOneShot("PlayerDoubleJump");
        }
        else if (playerJump.clip != null)
        {
            PlayOneShot("PlayerJump");
        }
    }

    public void PlayCoinCollect()
    {
        if (coinCollect.clip != null)
            PlayOneShot("CoinCollect");
    }

    public void PlayBoxBreak()
    {
        if (boxBreak.clip != null)
            PlayOneShot("BoxBreak");
    }

    public void PlayExplosion()
    {
        if (boxExplosion.clip != null)
            PlayOneShot("BoxExplosion");
    }

    public void PlayEnemyDeath()
    {
        if (goatDeath.clip != null)
            PlayOneShot("GoatDeath");
    }

    public void PlayCheckpoint()
    {
        if (checkpoint.clip != null)
            PlayOneShot("Checkpoint");
    }

    public void PlayGameOver()
    {
        if (gameOver.clip != null)
            PlayOneShot("GameOver");
    }

    public void PlayButtonClick()
    {
        if (buttonClick.clip != null)
            PlayOneShot("ButtonClick");
    }

    // «· Õﬂ„ »«·„Ê”ÌﬁÏ «·Œ·›Ì…
    public void SetMusicVolume(float volume)
    {
        if (backgroundMusic.source != null)
        {
            backgroundMusic.source.volume = volume;
        }
    }

    public void PauseMusic()
    {
        if (backgroundMusic.source != null)
        {
            backgroundMusic.source.Pause();
        }
    }

    public void ResumeMusic()
    {
        if (backgroundMusic.source != null)
        {
            backgroundMusic.source.UnPause();
        }
    }

    // œ«·… ··„”«⁄œ… ›Ì  ’ÕÌÕ «·√Œÿ«¡
    public void PrintAvailableSounds()
    {
        Debug.Log("Available sounds in dictionary:");
        foreach (var sound in soundDictionary)
        {
            Debug.Log($"- {sound.Key}: {(sound.Value.clip != null ? "Has clip" : "No clip")}");
        }
    }
}