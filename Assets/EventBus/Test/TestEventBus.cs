using System;
using UnityEngine;

public class TestEventBus : MonoBehaviour
{
    HealthComponent health;
    ManaComponent mana;

    EventBinding<TestEvent> TestEventBinding;
    EventBinding<PlayerEvent> PlayerEventBinding;

    private void OnEnable()
    {
        TestEventBinding = new EventBinding<TestEvent>(HandleTestEvent);
        EventBus<TestEvent>.Register(TestEventBinding);

        PlayerEventBinding = new EventBinding<PlayerEvent>(HandlePlayerEvent);
        EventBus<PlayerEvent>.Register(PlayerEventBinding);
    }

    private void OnDisable()
    {
        EventBus<TestEvent>.Deregister(TestEventBinding);
        EventBus<PlayerEvent>.Deregister(PlayerEventBinding);
    }

    private void Awake()
    {
        health = GetComponent<HealthComponent>();
        mana = GetComponent<ManaComponent>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            EventBus<TestEvent>.Raise(new TestEvent() { });
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            EventBus<PlayerEvent>.Raise(
                new PlayerEvent() { health = health.GetHealth(), mana = mana.GetMana() }
            );
        }
    }

    void HandleTestEvent(TestEvent e)
    {
        Debug.Log("TestEvent: ");
    }

    void HandlePlayerEvent(PlayerEvent e)
    {
        Debug.Log($"Player event received! Health: {e.health}, Mana: {e.mana}");
    }
}
