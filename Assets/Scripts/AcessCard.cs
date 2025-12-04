using System;
using UnityEngine;
using UnityEngine.Rendering;

public class AcessCard : MonoBehaviour, IHackable
{
    Renderer _renderer;
    Material material;
    public GameEvent acessCardHacked;

    public event Action OnHacked;

    public bool IsHacked { get; set; }

    private void Start()
    {
        _renderer = GetComponent<Renderer>();
        material = _renderer.material;
    }

    public void Hack()
    {
        IsHacked = true;
        material.SetColor("_EmissionColor", Color.red * 5f);
        acessCardHacked.Raise();
        OnHacked?.Invoke();
    }
}
