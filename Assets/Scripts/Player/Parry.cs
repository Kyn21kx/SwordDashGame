using Auxiliars;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(DashMovement))]
[RequireComponent(typeof(PlayerHealth))]
public class Parry : MonoBehaviour
{
    private const int RIGHT_MOUSE_INDEX = 1;
    private const string PARRY_TEXT = "SHARP!";
    //If we click, then get damaged, count the amount of time passed from one click to the damage
    [SerializeField]
    private float parryThresholdMillis;

    [SerializeField]
    private TextPopup parryPopupPrefab;

    private PlayerHealth healthController;
    private SpartanTimer parryTimer;

    private void Start()
    {
        this.healthController = GetComponent<PlayerHealth>();
        this.healthController.OnDamageParryCheckCallback = DamageCallback;
        this.parryTimer = new SpartanTimer(TimeMode.Framed);
    }

    private void Update()
    {
        this.HandleInput();
    }

    private void HandleInput()
    {
        if (Input.GetMouseButtonDown(RIGHT_MOUSE_INDEX)) { 
            //Start counting the time
            this.parryTimer.Reset();
        }
    }

    /// <summary>
    /// Called by the player's damage method as a callback, returns true if the parry was succesful, false otherwise
    /// </summary>
    /// <param name="value">The amount of damage received</param>
    /// <param name="source">The source position of that damage</param>
    /// <returns></returns>
    private bool DamageCallback(int value, Vector2 source)
    {
        if (!this.parryTimer.Started) return false;
        //Stop the timer here and check if we can cancel the damage
        float elapsedMillis = this.parryTimer.CurrentTimeMS;
        this.parryTimer.Stop();
        if (elapsedMillis <= this.parryThresholdMillis)
        {
            TextPopup instance = Instantiate(this.parryPopupPrefab, this.transform.position, Quaternion.identity);
            instance.PopupText = PARRY_TEXT;
            instance.StartAnimation();
            return true;
        }
        return false;
    }

}
