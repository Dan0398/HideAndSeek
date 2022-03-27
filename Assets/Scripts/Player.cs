using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] float Speed;
    Vector3 MoveDirection;
    float RealSpeed;

    //Noise
    const int COOLDOWN_EFFECT = -2; 
    const int STEPS_SOUND_LEVEL = 3;
    const int NOISE_MAX_LEVEL = 10;
    [SerializeField, Range(0,10)] float NoiseLevel;    

    //Steps and sound
    public static System.Action<Vector3> PlayerMakesToHighNoise;
    [Header("Steps"), SerializeField] AudioClip[] StepsClips;
    [SerializeField] float StepFreq;
    AudioSource StepsSource;
    float StepTimer;

    bool isMoving => RealSpeed > 0;

    private void Awake()
    {
        ResetState();
        StepsSource = GetComponent<AudioSource>();
    }

    public void ResetState()
    {
        StepTimer = 0;
        NoiseLevel = 0;
    }

    void FixedUpdate()
    {
        MovePlayer();
        RotatePlayer();
        MakeStepNoise();
        CalculateNoiseAndAlarm();
        UpdateUI();
    }

    void MovePlayer()
    {
        MoveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        RealSpeed = MoveDirection.magnitude;
        //Without Speedup by diagonals
        if (RealSpeed > 1) MoveDirection /= RealSpeed;
        transform.position += MoveDirection * Speed * Time.fixedDeltaTime;
    }

    void RotatePlayer()
    {
        if (!isMoving) return;
        transform.LookAt(transform.position + MoveDirection * 2);
    }

    void MakeStepNoise()
    {
        if (!isMoving)
        {
            StepTimer = 0;
            return;
        }
        StepTimer += Time.fixedDeltaTime;
        if (StepTimer > 1 / StepFreq) 
        {
            StepTimer = 0;
            StepsSource.clip = StepsClips[Random.Range(0, StepsClips.Length)] ;
            StepsSource.Play();
        }
    }

    void CalculateNoiseAndAlarm()
    {
        NoiseLevel += Time.fixedDeltaTime * (isMoving?(STEPS_SOUND_LEVEL):(COOLDOWN_EFFECT));
        NoiseLevel = Mathf.Clamp(NoiseLevel , 0f, NOISE_MAX_LEVEL);
        if (NoiseLevel >= NOISE_MAX_LEVEL) 
        {
            PlayerMakesToHighNoise?.Invoke(transform.position);
        }
    }

    void UpdateUI()
    {
        InGameInterface.ApplyNoiseLevel(NoiseLevel / (float)NOISE_MAX_LEVEL);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Finish")) InGameInterface.LevelCompleted();
        if (other.CompareTag("Enemy")) InGameInterface.GameOver();
    }
}