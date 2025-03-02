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

    readonly AnimationController animations;
    readonly Sensor sensor;
    readonly GameObject silkPrefab;
    readonly NavMeshAgent agent;

    private float attackDuration = 2f;
    private float attackTimer = 0f;
    private bool hasAttacked = false;
    private bool hasSetEffect = false;

    private float cooldownTime = 2f;
    private float cooldownTimer = 0f;

    private bool isRotating = false;
    private float rotationSpeed = 2f; 
    private Vector3 targetDirection;

    Vector3 lastKnownPosition;

    public AttackStrategy1(AnimationController animations, Sensor sensor, GameObject silkPrefab, NavMeshAgent agent)
    {
        this.animations = animations;
        this.sensor = sensor;
        this.silkPrefab = silkPrefab;
        this.agent = agent;
        lastKnownPosition = Vector3.zero;
        Complete = false;
    }

    public void Start()
    {
        Complete = false;
        hasAttacked = false;
        hasSetEffect = false;
        attackTimer = 0f;

        agent.isStopped = true;

        if (sensor.IsTargetInRange)
        {
            lastKnownPosition = sensor.TargetPosition;
            targetDirection = (lastKnownPosition - agent.transform.position).normalized;
            isRotating = true;  
            animations.AnimMove();   
        }
        else
        {
            Complete = true;
        }
    }

    public void Update(float deltaTime)
    {
        if (cooldownTimer > 0)
        {
            cooldownTimer -= deltaTime;
            Complete = true;
            return;
        }

        if (isRotating)
        {
            RotateTowardsTarget();
            return;
        }

        attackTimer += deltaTime;
        if (!hasAttacked && attackTimer >= 0.5f)  
        {
            hasAttacked = true;
            ShootWeb();
        }

        if (attackTimer >= attackDuration)
        {
            if (!hasSetEffect)
            {
                hasSetEffect = true;
                cooldownTimer = cooldownTime;
            }

            Complete = true;
        }
    }

    public void Stop()
    {
        agent.isStopped = false;
    }

    void RotateTowardsTarget()
    {
        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
        agent.transform.rotation = Quaternion.Slerp(agent.transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);

        if (Quaternion.Angle(agent.transform.rotation, targetRotation) < 2f)
        {
            isRotating = false;
            animations.Attack(2);
        }
    }

    void ShootWeb()
    {
        if (lastKnownPosition != Vector3.zero)
        {
            Vector3 spawnPosition = agent.transform.position + agent.transform.forward * 1f + Vector3.up * 1.5f;
            Vector3 targetDirection = (lastKnownPosition + Vector3.up * 1f - spawnPosition).normalized;
            GameObject web = GameObject.Instantiate(silkPrefab, spawnPosition, Quaternion.identity);
            web.GetComponent<SpiderSilkProjectile>().SetDirection(targetDirection);
        }
    }
}


public class AttackStrategy2 : IActionStrategy
{
    public bool CanPerform => true;
    public bool Complete { get; private set; }

    readonly AnimationController animations;
    readonly Sensor sensor;
    readonly GameObject poison;
    readonly NavMeshAgent agent;

    private float delayBeforeCharge = 1.5f;
    private float chargeSpeed = 10f;
    private float chargeDuration = 1.2f;
    private float cooldownTime = 5f;
    private float rotationSpeed = 5f;

    private float timer = 0f;
    private Vector3 chargeDirection;
    private AttackState currentState;
    private Quaternion targetRotation;

    private enum AttackState
    {
        Waiting,
        Rotating,
        Charging,
        Cooldown
    }

    public AttackStrategy2(AnimationController animations, Sensor sensor, GameObject poison, NavMeshAgent agent)
    {
        this.animations = animations;
        this.sensor = sensor;
        this.poison = poison;
        this.agent = agent;
        Complete = false;
        currentState = AttackState.Waiting;
    }

    public void Start()
    {
        Complete = false;
        timer = 0f;
        currentState = AttackState.Waiting;
        agent.isStopped = true;
        animations.AnimIdle();

        if (sensor.IsTargetInRange)
        {
            chargeDirection = (sensor.TargetPosition - agent.transform.position).normalized;
            targetRotation = Quaternion.LookRotation(chargeDirection);
            currentState = AttackState.Rotating;
        }
        else
        {
            Complete = true;
        }
    }

    public void Update(float deltaTime)
    {
        timer += deltaTime;

        switch (currentState)
        {
            case AttackState.Waiting:
                if (timer >= delayBeforeCharge)
                {
                    currentState = AttackState.Rotating;
                }
                break;

            case AttackState.Rotating:
                RotateTowardsTarget(deltaTime);
                break;

            case AttackState.Charging:
                agent.Move(chargeDirection * chargeSpeed * deltaTime);
                if (timer >= chargeDuration)
                {
                    StartCooldown();
                }
                break;

            case AttackState.Cooldown:
                if (timer >= cooldownTime)
                {
                    Complete = true;
                }
                break;
        }
    }

    public void Stop()
    {
        agent.isStopped = false;
    }

    private void RotateTowardsTarget(float deltaTime)
    {
        agent.transform.rotation = Quaternion.Slerp(agent.transform.rotation, targetRotation, deltaTime * rotationSpeed);

        if (Quaternion.Angle(agent.transform.rotation, targetRotation) < 3f)  
        {
            StartCharge();
        }
    }

    private void StartCharge()
    {
        timer = 0f;
        currentState = AttackState.Charging;
        agent.isStopped = false;
        animations.Attack(3);
    }

    private void StartCooldown()
    {
        timer = 0f;
        currentState = AttackState.Cooldown;
        agent.isStopped = true;
        animations.AnimIdle();
    }
}


public class MoveStrategy : IActionStrategy
{
    readonly NavMeshAgent agent;
    readonly Func<Vector3> destination;

    public bool CanPerform => !Complete;
    public bool Complete => agent.remainingDistance <= 2f && !agent.pathPending;
    AnimationController animations;
    public MoveStrategy(NavMeshAgent agent, Func<Vector3> destination, AnimationController animations)
    {
        this.agent = agent;
        this.destination = destination;
        this.animations = animations;
    }

    public void Start()
    {
        agent.SetDestination(destination());
        animations.AnimMove();
    } 

    public void Stop() => agent.ResetPath();
}

public class WanderStrategy : IActionStrategy
{
    readonly NavMeshAgent agent;
    readonly float wanderRadius;
    private AnimationController animations;
    public bool CanPerform => !Complete;
    public bool Complete => agent.remainingDistance <= 2f && !agent.pathPending;

    public WanderStrategy(NavMeshAgent agent, float wanderRadius, AnimationController animations)
    {
        this.agent = agent;
        this.wanderRadius = wanderRadius;
        this.animations = animations;
    }

    public void Start()
    {
        animations.AnimMove();
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
    public bool CanPerform => true; 
    public bool Complete { get; private set; }

    readonly CountdownTimer timer;
    AnimationController animations;
    public IdleStrategy(float duration, AnimationController animations)
    {
        this.animations = animations;
        timer = new CountdownTimer(duration);
        timer.OnTimerStart += () => Complete = false;
        timer.OnTimerStop += () => Complete = true;
    }

    public void Start()
    {
        timer.Start();
        animations.AnimIdle();
    } 

    public void Update(float deltaTime) => timer.Tick(deltaTime);
}
