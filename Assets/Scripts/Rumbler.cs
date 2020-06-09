using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class Rumbler : MonoBehaviour
{
    private PlayerInput _playerInput;
    private RumblePattern activeRumbePattern;
    private float rumbleDurration;
    private float pulseDurration;
    private float lowA;
    private float highA;
    private float lowB;
    private float highB;
    private float rumbleStep;
    private bool isMotorActive = false;
    public void RumbleConstant(float low, float high, float durration)
    {
        activeRumbePattern = RumblePattern.Constant;
        lowA = low;
        highA = high;
        rumbleDurration = Time.time + durration;
        Invoke(nameof(StopRumble), durration);
    }

    public void RumblePulse(float low, float high, float burstTime, float durration)
    {
        activeRumbePattern = RumblePattern.Pulse;
        lowA = low;
        highA = high;
        rumbleStep = burstTime;
        pulseDurration = Time.time + burstTime;
        rumbleDurration = Time.time + durration;
        isMotorActive = true;
        var g = GetGamepad();
        g?.SetMotorSpeeds(lowA, highA);
        Invoke(nameof(StopRumble), durration);
    }

    public void RumbleLinear(float lowStart, float lowEnd, float highStart, float highEnd, float durration)
    {
        // TODO
    }

    public void StopRumble()
    {
        var gamepad = GetGamepad();
        if (gamepad != null)
        {
            gamepad.SetMotorSpeeds(0, 0);
        }
    }

    private void Awake()
    {
        _playerInput = GetComponent<PlayerInput>();
    }

    private void Update()
    {
        if (Time.time > rumbleDurration)
            return;
        var gamepad = GetGamepad();
        if (gamepad == null)
            return;

        switch (activeRumbePattern)
        {
            case RumblePattern.Constant:
                gamepad.SetMotorSpeeds(lowA, highA);
                break;
            case RumblePattern.Pulse:
                if(Time.time > pulseDurration)
                {
                    isMotorActive = !isMotorActive;
                    pulseDurration = Time.time + rumbleStep;
                    if (!isMotorActive)
                    {
                        gamepad.SetMotorSpeeds(0, 0);
                    }
                    else
                    {
                        gamepad.SetMotorSpeeds(lowA, highA);
                    }
                }
                break;
            case RumblePattern.Linear:
                break;
            default:
                break;
        }
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
        StopRumble();
    }

    private Gamepad GetGamepad()
    {
        return Gamepad.all.FirstOrDefault(g => _playerInput.devices.Any(d => d.deviceId == g.deviceId));
        #region Linq Query Equivalent Logic
        //Gamepad gamepad = null;
        //foreach (var g in Gamepad.all)
        //{
        //    foreach (var d in _playerInput.devices)
        //    {
        //        if(d.deviceId == g.deviceId)
        //        {
        //            gamepad = g;
        //            break;
        //        }
        //    }
        //    if(gamepad != null)
        //    {
        //        break;
        //    }
        //}
        //return gamepad;
        #endregion
    }
}

public enum RumblePattern
{
    Constant,
    Pulse,
    Linear
}
