using System;
using System.Collections.Generic;
using UnityEngine;

namespace BiharSurfers.PowerUps
{
    [System.Serializable]
    public class ActivePowerUpState
    {
        public PowerUpType type;
        public string displayName;
        public float duration;
        public float remainingTime;
    }

    /// <summary>
    /// Master manager tracking active power-ups (Magnet, Shield, Speed Boost, Coin Multiplier, Invincibility),
    /// duration countdowns, UI indicators, and audio/VFX feedback.
    /// </summary>
    public class PowerUpManager : MonoBehaviour
    {
        public static PowerUpManager Instance { get; private set; }

        [Header("Active Power-Up States")]
        public Dictionary<PowerUpType, ActivePowerUpState> activePowerUps = new Dictionary<PowerUpType, ActivePowerUpState>();

        public event Action<PowerUpType, float, float> OnPowerUpUpdated; // (type, remainingTime, totalDuration)
        public event Action<PowerUpType> OnPowerUpExpired;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        private void Update()
        {
            if (Core.GameManager.Instance != null && Core.GameManager.Instance.currentState != Core.GameState.Playing)
                return;

            List<PowerUpType> expiredKeys = new List<PowerUpType>();

            foreach (var kvp in activePowerUps)
            {
                var state = kvp.Value;
                state.remainingTime -= Time.deltaTime;

                OnPowerUpUpdated?.Invoke(state.type, state.remainingTime, state.duration);

                if (state.remainingTime <= 0f)
                {
                    expiredKeys.Add(kvp.Key);
                }
            }

            foreach (var key in expiredKeys)
            {
                DeactivatePowerUp(key);
            }
        }

        public void ActivatePowerUp(PowerUpType type, float duration, string name)
        {
            if (activePowerUps.ContainsKey(type))
            {
                activePowerUps[type].remainingTime = duration;
                activePowerUps[type].duration = duration;
            }
            else
            {
                activePowerUps.Add(type, new ActivePowerUpState
                {
                    type = type,
                    displayName = name,
                    duration = duration,
                    remainingTime = duration
                });
            }

            ApplyPowerUpEffect(type, true);
            Core.AudioManager.Instance?.PlaySFX("PowerUpPickup");
        }

        public void DeactivatePowerUp(PowerUpType type)
        {
            if (activePowerUps.ContainsKey(type))
            {
                activePowerUps.Remove(type);
                ApplyPowerUpEffect(type, false);
                OnPowerUpExpired?.Invoke(type);
            }
        }

        public void ClearAllPowerUps()
        {
            List<PowerUpType> keys = new List<PowerUpType>(activePowerUps.Keys);
            foreach (var key in keys)
            {
                DeactivatePowerUp(key);
            }
        }

        private void ApplyPowerUpEffect(PowerUpType type, bool active)
        {
            var player = Player.PlayerController.Instance;
            if (player == null) return;

            switch (type)
            {
                case PowerUpType.Magnet:
                    player.isMagnetActive = active;
                    break;

                case PowerUpType.Shield:
                    player.isShielded = active;
                    break;

                case PowerUpType.SpeedBoost:
                    player.currentSpeedMultiplier = active ? 1.6f : 1.0f;
                    player.isInvincible = active;
                    break;

                case PowerUpType.CoinMultiplier:
                    if (Coins.CoinManager.Instance != null)
                    {
                        Coins.CoinManager.Instance.coinMultiplier = active ? 2 : 1;
                    }
                    break;

                case PowerUpType.Invincibility:
                    player.isInvincible = active;
                    break;
            }
        }

        public bool IsPowerUpActive(PowerUpType type)
        {
            return activePowerUps.ContainsKey(type);
        }
    }
}
