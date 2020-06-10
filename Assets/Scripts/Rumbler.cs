using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
public enum RumblePattern
{
    Constant,
    Pulse,
    Linear
}

public class Rumbler : MonoBehaviour
{
    private PlayerInput _playerInput;
    private RumblePattern activeRumbePattern;
    private float rumbleDurration;
    private float pulseDurration;
    private float lowA;
    private float lowStep;
    private float highA;
    private float highStep;
    private float rumbleStep;
    private bool isMotorActive = false;

    // Public Methods
    public void RumbleConstant(float low, float high, float durration)
    {
        activeRumbePattern = RumblePattern.Constant;
        lowA = low;
        highA = high;
        rumbleDurration = Time.time + durration;
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
    }

    public void RumbleLinear(float lowStart, float lowEnd, float highStart, float highEnd, float durration)
    {
        activeRumbePattern = RumblePattern.Linear;
        lowA = lowStart;
        highA = highStart;
        lowStep = (lowEnd - lowStart) / durration;
        highStep = (highEnd - highStart) / durration;
        rumbleDurration = Time.time + durration;
    }

    public void StopRumble()
    {
        var gamepad = GetGamepad();
        if (gamepad != null)
        {
            gamepad.SetMotorSpeeds(0, 0);
        }
    }


    // Unity MonoBehaviors
    private void Awake()
    {
        _playerInput = GetComponent<PlayerInput>();
    }

    private void Update()
    {
        if (Time.time > rumbleDurration)
        {
            StopRumble();
            return;
        }

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
                gamepad.SetMotorSpeeds(lowA, highA);
                lowA += (lowStep * Time.deltaTime);
                highA += (highStep * Time.deltaTime);
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

    // Private helpers

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