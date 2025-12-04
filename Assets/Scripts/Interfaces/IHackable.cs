using UnityEngine;

public interface IHackable
{
    HackType HackType { get; }
    event System.Action OnHacked;
    public void Hack();
}

public enum HackType
{
    CanBeReHacked,
    DestroyOnHack
}
