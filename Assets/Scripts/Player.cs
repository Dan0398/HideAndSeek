using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    static public Player Instance;
    [SerializeField] float Speed;
    [SerializeField] Vector3 MoveDirection;
    public Transform PlayerModel;

    int CoolDownNoise = 2, StepsSoundLVL = 3;

    [SerializeField] bool isMoving;
    [SerializeField] [Range(0,10)] float Noise;

    [SerializeField] AudioClip[] Steps;
    AudioSource StepsSource;
    [SerializeField] float StepFreq;
    float StepTimer;

    Interface ui;
    public System.Action makedToHighNoise;

    private void Awake()
    {
        if (Instance != null) Destroy(gameObject);
        else Instance = this;
        makedToHighNoise += TooHigh;
        StepsSource = GetComponent<AudioSource>();
    }
    void FixedUpdate()
    {
        //Движение игрока
        MoveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        //Без читов при движении по диагонали
        if (MoveDirection.magnitude > 1) MoveDirection /= MoveDirection.magnitude;
        transform.position +=(MoveDirection * Speed* Time.fixedDeltaTime);
        isMoving = MoveDirection.magnitude > 0;
        //Поворот модельки
        if (isMoving && PlayerModel!= null) PlayerModel.transform.LookAt(transform.position + MoveDirection * 2);
        //Собсна шум
        Noise = Mathf.Clamp(Noise + Time.fixedDeltaTime * (isMoving?(StepsSoundLVL):(-CoolDownNoise)), 0f, 10f);
        if (Noise >= 10) makedToHighNoise.Invoke();
        if (isMoving)
        {
            if (StepTimer > 1 / StepFreq) {
                StepTimer = 0;
                MakeStepNoise();
            }
            else StepTimer += Time.fixedDeltaTime;
        }
        else StepTimer = 0;

        UpdateUI();
    }
    void UpdateUI()
    {
        if (ui == null) ui = Interface.instance;
        ui.AnimateNoise(Noise / 10f);
    }
    void MakeStepNoise()
    {
        StepsSource.clip = Steps[Random.Range(0, Steps.Length)] ;
        StepsSource.Play();
    }
    void TooHigh()
    {
        //Debug.Log("Какого дьявола ты здесь шумишь?!?!");
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Finish")) Interface.instance.End(true);
        if (other.CompareTag("Enemy")) Interface.instance.End(false);
    }
    public void SwitchCollisions(bool isEnabled)
    {
        foreach (Collider coll in GetComponentsInChildren<Collider>()) coll.enabled = isEnabled;
    }
}
