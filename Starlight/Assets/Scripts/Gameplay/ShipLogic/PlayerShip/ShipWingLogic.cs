using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ShipWingLogic : HealthAndArmor
{
    [Space(8)]

    //The minimum and maximum X maneuverability speed this wing gives
    public Vector2 minMaxRightSpeed = new Vector2();
    public Vector2 minMaxLeftSpeed = new Vector2();
    //The curve that determines how quickly the X speed decreases based on health %
    public AnimationCurve xSpeedDamageCurve;
    //The current X maneuverability speed
    [HideInInspector]
    public float currentRightSpeed = 1;
    [HideInInspector]
    public float currentLeftSpeed = 1;

    [Space(8)]

    //The minimum and maximum Y maneuverability speed this wing gives
    public Vector2 minMaxUpSpeed = new Vector2();
    public Vector2 minMaxDownSpeed = new Vector2();
    //The curve that determines how quickly the Y speed decreases based on health %
    public AnimationCurve ySpeedDamageCurve;
    //The current Y maneuverability speed
    [HideInInspector]
    public float currentUpSpeed = 1;
    [HideInInspector]
    public float currentDownSpeed = 1;

    [Space(8)]

    //The amount this wing drifts when damaged
    public Vector2 minMaxDriftStrength = new Vector2();
    //The direction that this wing drifts when damaged
    public Vector2 damagedDriftDirection = new Vector2();
    //The curve that determines how quickly the drift sets in based on health %
    public AnimationCurve driftDamageCurve;
    //The current drift amount
    [HideInInspector]
    public float currentDrift = 1;



    //Function called on the first frame
    private void Start()
    {
        //Setting the starting speed and drift based on damage
        this.CalculateDamageCurves();
    }


    //Function inherited from HealthAndArmor.cs to heal this object
    public override void RestoreHealth(int amountToRestore_)
    {
        base.RestoreHealth(amountToRestore_);
        this.CalculateDamageCurves();
    }


    //Function inherited from HealthAndArmor.cs to replenish shields on this object
    public override void RestoreShields(int amountToRestore_)
    {
        base.RestoreShields(amountToRestore_);
    }


    //Function inherited from HealthAndArmor.cs to damage this object
    public override void DealDamage(int amountOfDamage_)
    {
        base.DealDamage(amountOfDamage_);
        this.CalculateDamageCurves();
    }


    //Function called from Start, RestoreHealth and DealDamage to set our maneuverability and drift
    private void CalculateDamageCurves()
    {
        //Getting the current health %
        float healthPercent = this.currentHealth / this.maxHealth;

        //Finding the position on the X curve
        float xCurveValue = this.xSpeedDamageCurve.Evaluate(healthPercent);
        //Setting our current X speed using the curve value and the min/max values
        this.currentRightSpeed = ((this.minMaxRightSpeed.y - this.minMaxRightSpeed.x) * xCurveValue) + this.minMaxRightSpeed.x;
        this.currentLeftSpeed = ((this.minMaxLeftSpeed.y - this.minMaxLeftSpeed.x) * xCurveValue) + this.minMaxLeftSpeed.x;

        //Finding the position on the Y curve
        float yCurveValue = this.ySpeedDamageCurve.Evaluate(healthPercent);
        //Setting our current Y speed using the curve value and the min/max values
        this.currentUpSpeed = ((this.minMaxUpSpeed.y - this.minMaxUpSpeed.x) * yCurveValue) + this.minMaxUpSpeed.x;
        this.currentDownSpeed = ((this.minMaxDownSpeed.y - this.minMaxDownSpeed.x) * yCurveValue) + this.minMaxDownSpeed.x;

        //Finding the position on the drift curve
        float driftCurveValue = this.driftDamageCurve.Evaluate(healthPercent);
        //Setting our current XY drift using the curve value
        this.currentDrift = this.minMaxDriftStrength.x + ((this.minMaxDriftStrength.y - this.minMaxDriftStrength.x) * driftCurveValue);
    }
}
