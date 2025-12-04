using UnityEngine;

public interface IHackable
{
    event System.Action OnHacked;
    public void Hack();
}
