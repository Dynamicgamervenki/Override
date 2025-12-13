using System;

public interface IDamgable
{
    public void TakeDamage();
}

public enum DamageType
{
    Hack,
    ImmediateKill
}