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
    NavMeshAgent Agent;
    Vector3 NextPosition, PrevPosition;
    [SerializeField] StateMachine MyState, PrevFrameState;
    float CurrentStateTime = 2;
    [SerializeField] float IdleStateTime = 1.5f;
    [SerializeField] Material MyMaterial;
    [SerializeField] [Range(0, 1)] float Agressive = 0;
    [SerializeField] [Range(0, 10)] float ViewLeight = 3;
    [SerializeField] Transform ViewTrigger;
    float AgressiveSaver=2, ViewLeightSaver=2;
    void Start()
    {
        Agent = GetComponent<NavMeshAgent>();
        MyMaterial = new Material(GetComponentInChildren<MeshRenderer>().material);
        GetComponentInChildren<MeshRenderer>().material = MyMaterial;
        Player.Instance.makedToHighNoise += GetPos;
    }
    private void OnDestroy() { Player.Instance.makedToHighNoise -= GetPos; }
    private void OnDisable() { Player.Instance.makedToHighNoise -= GetPos; }

    void GetPos()
    {
        MyState = StateMachine.Catching;
        NextPosition = Player.Instance.transform.position;
    }
    internal void EyeView(Vector3 Position)
    {
        MyState = StateMachine.Catching;
        NextPosition = Position;
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        ChangeViewLeight();
        ChangeMyColor();
        Action();
    }
    void Action()
    {
        switch (MyState)
        {
            case StateMachine.Idle:
                if (CurrentStateTime <= IdleStateTime) CurrentStateTime += Time.fixedDeltaTime;
                else
                {
                    if (!Agent.hasPath)
                    {
                        GetNewPoint();
                    }
                    else
                    {
                        CurrentStateTime = 0;
                        MyState = StateMachine.Walking;
                    }
                }
                break;
            case StateMachine.Walking:

                if (!Agent.hasPath || Agent.pathPending)
                {
                    MyState = StateMachine.Idle;
                }
                break;
            case StateMachine.Catching:
                if (PrevPosition != NextPosition)
                {
                    Agent.SetDestination(NextPosition);
                    PrevPosition = NextPosition;
                }
                if (!Agent.pathPending && !Agent.hasPath)
                {
                    StartCoroutine(ChangeColor(false));
                    CurrentStateTime = 0;
                    MyState = StateMachine.Idle;
                }
                    break;
        }
        if (PrevFrameState!= MyState)
        {
            if (MyState == StateMachine.Catching)
            {
                GetComponent<AudioSource>().Play();
                StartCoroutine(ChangeColor(true));
            }
            PrevFrameState = MyState;
        }
    }
    void GetNewPoint()
    {
        Agent.SetDestination(MapMaker.Instance.GetPath(transform.position));
    }
    IEnumerator ChangeColor(bool GoingRed)
    {
        for (int i = 0; i <= 30; i++)
        {
            Agressive = GoingRed? i / 30f : 1-(i-30f);
            yield return new WaitForEndOfFrame();
        }
        yield return null;
    }
    void ChangeViewLeight()
    {
        if (ViewLeightSaver != ViewLeight)
        {
            ViewTrigger.localScale = new Vector3(ViewLeight, 1, ViewLeight);
            ViewLeightSaver = ViewLeight;
        }
    }
    void ChangeMyColor()
    {
        if (AgressiveSaver != Agressive)
        {
            MyMaterial.SetColor("_Color", new Color(1,1 - Agressive, 1 - Agressive));
            AgressiveSaver = Agressive;
        }
    }
}
