using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipEngineLogic : HealthAndArmor
{
    [Space(8)]

    //The minimum, average, and maximum forward velocity this engine gives for rail zones
    public Vector3 minRailVelocity = new Vector3();
    public Vector3 maxRailVelocity = new Vector3();

    [Space(8)]

    //The slow, average, and boost forward velocity this engine gives in free flight zones
    public Vector3 minFreeVelocity = new Vector3();
    public Vector3 maxFreeVelocity = new Vector3();

    [Space(8)]

    //The curve that determines how quickly the forward velocity decreases based on health %
    public AnimationCurve velocityDamageCurve;
    //The current forward velocities
    [HideInInspector]
    public Vector3 currentRailVelocity = new Vector3();
    [HideInInspector]
    public Vector3 currentFreeVelocity = new Vector3();



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

        //Finding the position on the damage curve
        float velocityCurveValue = this.velocityDamageCurve.Evaluate(healthPercent);
        //Setting our current rail speed values using the curve value and the min/max values
        this.currentRailVelocity = new Vector3(this.minRailVelocity.x + ((this.maxRailVelocity.x - this.minRailVelocity.x) * velocityCurveValue),
                                                this.minRailVelocity.y + ((this.maxRailVelocity.y - this.minRailVelocity.y) * velocityCurveValue),
                                                this.minRailVelocity.z + ((this.maxRailVelocity.z - this.minRailVelocity.z) * velocityCurveValue));

        //Setting our current free flight speed values using the curve value and the min/max values
        this.currentFreeVelocity = new Vector3(this.minFreeVelocity.x + ((this.maxFreeVelocity.x - this.minFreeVelocity.x) * velocityCurveValue),
                                                this.minFreeVelocity.y + ((this.maxFreeVelocity.y - this.minFreeVelocity.y) * velocityCurveValue),
                                                this.minFreeVelocity.z + ((this.maxFreeVelocity.z - this.minFreeVelocity.z) * velocityCurveValue));
    }
}
