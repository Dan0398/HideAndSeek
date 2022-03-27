using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    enum StateMachine
    {
        Idle,
        Walking,
        Catching
    }
    StateMachine currentState;
    NavMeshAgent Agent;
    Vector3 NextPosition;
    Vector3 PrevPosition; //For apply endpoint for agent when updated
    float CurrentStateTime = 0;
    [SerializeField] float IdleStateTime = 1.5f;
    Material MyMaterial;
    AudioSource ExclamationSound;
    [SerializeField] Transform ViewTrigger;
    [SerializeField] [Range(0, 10)] float viewLeight = 3;
    bool agressive = false;
    StateMachine CurrentState
    {
        get => currentState;
        set
        {
            if (currentState == value) return;
            currentState = value;
            ProcessSwitchState();
        }
    }
    bool Agressive
    {
        get => agressive;
        set
        {
            if (agressive == value) return;
            agressive = value;
            ChangeColorAnimated();
        }
    }

    void Start()
    {
        Agent = GetComponent<NavMeshAgent>();
        MyMaterial = new Material(GetComponentInChildren<MeshRenderer>().material);
        ViewTrigger.localScale = new Vector3(viewLeight, 1, viewLeight);
        GetComponentInChildren<MeshRenderer>().material = MyMaterial;
        ExclamationSound = GetComponent<AudioSource>();
        Player.PlayerMakesToHighNoise += UpdatePlayerPosition;
    }

    private void OnDestroy() 
    { 
        Player.PlayerMakesToHighNoise -= UpdatePlayerPosition; 
    }
    private void OnDisable() 
    { 
        Player.PlayerMakesToHighNoise -= UpdatePlayerPosition; 
    }

    public async void ResetState()
    {
        if (Agent == null) return;
        Agent.enabled = false;
        CurrentState = StateMachine.Walking;
        await System.Threading.Tasks.Task.Delay(30);
        Agent.enabled = true;
    }

    internal void UpdatePlayerPosition(Vector3 PlayerPos)
    {
        CurrentState = StateMachine.Catching;
        NextPosition = PlayerPos;
    }
    
    void FixedUpdate()
    {
        if (CurrentState == StateMachine.Idle)
        {
            IdleAction();
        }
        if (CurrentState == StateMachine.Walking)
        {
            WalkingAction();
        }
        if (CurrentState == StateMachine.Catching)
        {
            CatchingAction();
        }
    }

    void IdleAction()
    {
        if (CurrentStateTime <= IdleStateTime) 
        {
            CurrentStateTime += Time.fixedDeltaTime;
        }
        else
        {
            if (!Agent.hasPath)
            {
                Agent.SetDestination(MapMaker.Instance.GetPath(transform.position));
            }
            else
            {
                CurrentStateTime = 0;
                CurrentState = StateMachine.Walking;
            }
        }
    }

    void WalkingAction()
    {
        if (!Agent.hasPath || Agent.pathPending)
        {
            CurrentState = StateMachine.Idle;
        }
    }

    void CatchingAction()
    {
        if (PrevPosition != NextPosition)
        {
            Agent.SetDestination(NextPosition);
            PrevPosition = NextPosition;
        }
        if (!Agent.pathPending && !Agent.hasPath)
        {
            CurrentStateTime = 0;
            CurrentState = StateMachine.Idle;
        }
    }

    void ProcessSwitchState()
    {
        Agressive = CurrentState == StateMachine.Catching;
        if (Agressive)
        {
            ExclamationSound.Play();
        }
    }

    async void ChangeColorAnimated()
    {
        float Lerp = 0;
        Color OldColor = MyMaterial.GetColor("_Color");
        Color NewColor = (agressive ? Color.red : Color.white);
        for (int i = 0; i <= 90; i+=5)
        {
            Lerp = Mathf.Sin(i*Mathf.Deg2Rad);
            MyMaterial.SetColor("_Color", Color.Lerp(OldColor, NewColor, Lerp));
            await System.Threading.Tasks.Task.Delay(30);
        }
    }
}