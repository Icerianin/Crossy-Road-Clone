using UnityEngine;

namespace Core.Scripts.Player
{
    public class PlayerData
    {
        public Transform Model { get; }
        public PlayerDamageable PlayerDamageable { get; }
        public PlayerMovement PlayerMovement { get; }
        public Vector3 StartPosition { get; }
        public Transform Player => PlayerMovement.transform;
        
        private readonly PlayerMovementJoystick _playerJoystick;
        private readonly PlayerAttack _playerAttack;

        private bool _isJumpControl = true;

        public PlayerData(
            Transform playerModel,
            Vector3 startPosition,
            PlayerMovement movement,
            PlayerMovementJoystick joystick,
            PlayerDamageable playerDamageable,
            PlayerAttack playerAttack)
        {
            Model = playerModel;
            StartPosition =  startPosition;
            PlayerMovement = movement;
            PlayerDamageable = playerDamageable;
            _playerAttack = playerAttack;
            
            _playerJoystick = joystick;
        }

        public void SwitchControlsToJump()
        {
            PlayerMovement.enabled = true;
            _playerJoystick.enabled = false;
            _playerAttack.enabled = false;
            _isJumpControl = true;
        }

        public void SwitchControlsToJoystick()
        {
            PlayerMovement.enabled = false;
            _playerJoystick.enabled = true;
            _playerAttack.enabled = true;
            _isJumpControl = false;
        }

        public void DisableControls()
        {
            PlayerMovement.enabled = false;
            _playerJoystick.enabled = false;
            _playerAttack.enabled = false;
        }

        public void PauseJoystickMovement()
        {
            if(_isJumpControl)
                return;
            
            _playerJoystick.enabled = false;
        }

        public void ContinueJoystickMovement()
        {
            if(_isJumpControl)
                return;

            _playerJoystick.enabled = true;
        }
    }
}