using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Platformer
{
    public class PlayerController : MonoBehaviour
    {
        public StateMachine stateMachine;
        public Animator animator;
        CountdownTimer jumpTimer;
        CountdownTimer jumpCooldownTimer;
        private bool isRunning;
        bool GroudCheck;
        float jumpForce = 10f;
        float jumpVelocity = 10f;
        float jumpDuration = 0.5f;
        float jumpCooldown = 0.5f;
        private void Awake()
        { 
            if (animator == null)
            {
                animator = GetComponent<Animator>();
            }
            var locomotionState = new LocomotionState(this, animator);
            var jumpState = new JumpState(this, animator);
            
            jumpTimer = new CountdownTimer(jumpDuration);
            jumpCooldownTimer = new CountdownTimer(jumpCooldown);
            
            jumpTimer.OnTimerStart += () => jumpVelocity = jumpForce;
            jumpTimer.OnTimerStop += () => jumpCooldownTimer.Start();
            
            At(locomotionState, jumpState, new FuncPredicate(() => jumpTimer.IsRunning));
            At(jumpState, locomotionState, new FuncPredicate(() => !jumpTimer.IsRunning));
            
            stateMachine.SetState(locomotionState);
        }
        
        void At(IState from, IState to, IPredicate condition) => stateMachine.AddTransition(from, to, condition);
        void Any(IState to, IPredicate condition) => stateMachine.AddAnyTransition(to, condition);
        
        public void HandleMoment()
        {
            // Handle movement
        }
        
        public void HandleJump()
        {
            // Handle jump
        }

        private void Update()
        {
            stateMachine.Update();
        }

        private void FixedUpdate()
        {
            stateMachine.FixedUpdate();
        }
    }
    
}