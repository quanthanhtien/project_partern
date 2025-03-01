using System;
using UnityEngine;
using UnityEngine.AI;

// TODO Migrate Strategies, Beliefs, Actions and Goals to Scriptable Objects and create Node Editor for them
public interface IActionStrategy
{
    bool CanPerform { get; }
    bool Complete { get; }

    void Start()
    {
        // noop
    }

    void Update(float deltaTime)
    {
        // noop
    }

    void Stop()
    {
        // noop
    }
}

public class AttackStrategyDefault : IActionStrategy
{
    public bool CanPerform => true; // Agent can always attack
    public bool Complete { get; private set; }
    readonly AnimationController animations;
    bool check;
    Sensor sensor;

    public AttackStrategyDefault(AnimationController animations, Sensor sensor)
    {
        this.animations = animations;
        this.sensor = sensor;
    }

    public void Update(float deltaTime)
    {
        if (sensor.IsTargetInRange == true)
        {
            animations.Attack(1);
        }
    }
}

public class AttackStrategy1 : IActionStrategy
{
    public bool CanPerform => true;
    public bool Complete { get; private set; }

    public GameObject silk;
    readonly AnimationController animations;
    bool check;
    Sensor sensor;

    public AttackStrategy1(AnimationController animations, Sensor sensor, GameObject silk)
    {
        this.animations = animations;
        this.sensor = sensor;
        this.silk = silk;
    }

    public void Update(float deltaTime)
    {
        animations.Attack(2);
    }
}

public class AttackStrategy2 : IActionStrategy
{
    public bool CanPerform => !Complete;
    public bool Complete { get; private set; }

    readonly AnimationController animations;
    bool check;
    Sensor sensor;

    public AttackStrategy2(AnimationController animations, Sensor sensor)
    {
        this.animations = animations;
        this.sensor = sensor;
        this.animations.Attack(3);
    }
}

public class MoveStrategy : IActionStrategy
{
    readonly NavMeshAgent agent;
    readonly Func<Vector3> destination;

    public bool CanPerform => !Complete;
    public bool Complete => agent.remainingDistance <= 2f && !agent.pathPending;

    public MoveStrategy(NavMeshAgent agent, Func<Vector3> destination)
    {
        this.agent = agent;
        this.destination = destination;
    }

    public void Start() => agent.SetDestination(destination());

    public void Stop() => agent.ResetPath();
}

public class WanderStrategy : IActionStrategy
{
    readonly NavMeshAgent agent;
    readonly float wanderRadius;

    public bool CanPerform => !Complete;
    public bool Complete => agent.remainingDistance <= 2f && !agent.pathPending;

    public WanderStrategy(NavMeshAgent agent, float wanderRadius)
    {
        this.agent = agent;
        this.wanderRadius = wanderRadius;
    }

    public void Start()
    {
        for (int i = 0; i < 5; i++)
        {
            Vector3 randomDirection = (UnityEngine.Random.insideUnitSphere * wanderRadius).With(
                y: 0
            );
            NavMeshHit hit;

            if (
                NavMesh.SamplePosition(
                    agent.transform.position + randomDirection,
                    out hit,
                    wanderRadius,
                    1
                )
            )
            {
                agent.SetDestination(hit.position);
                return;
            }
        }
    }
}

public class IdleStrategy : IActionStrategy
{
    public bool CanPerform => true; // Agent can always Idle
    public bool Complete { get; private set; }

    readonly CountdownTimer timer;

    public IdleStrategy(float duration)
    {
        timer = new CountdownTimer(duration);
        timer.OnTimerStart += () => Complete = false;
        timer.OnTimerStop += () => Complete = true;
    }

    public void Start() => timer.Start();

    public void Update(float deltaTime) => timer.Tick(deltaTime);
}
