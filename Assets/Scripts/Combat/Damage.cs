using System;
using UnityEngine;

[Serializable]
public class Damage
{
    [Min(0f)]public int amount;
    [Min(0f),Tooltip("Check Animation Time")]public float graceTime;
    [Min(0f)] public float knockBackForce = 0f;


    public Damage(int _amount, float _graceTime)
    {
        this.amount = _amount;
        this.graceTime = _graceTime;
    }
    public Damage(int _amount, float _graceTime, float _knockBackForce) : this(_amount,_graceTime)
    {
        this.knockBackForce = _knockBackForce;
    }
}
