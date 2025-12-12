using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class Computer : MonoBehaviour , IHackable ,IZoom
{
    [SerializeField] private GameObject textMesh;
    [SerializeField] private MeshRenderer screenRenderer;
    [SerializeField] private Material emissiveMaterial;
    [SerializeField] private Material normalMaterial;
    [SerializeField] private float hackingDuration;
    [SerializeField] private GameEvent computerHacked;
    [SerializeField] private float fov = 12;
    private bool isOn = false;
    
    
    private void Start()
    {
        TurnOffComputer();
    }

    private void TurnOnComputer()
    {
        if (screenRenderer != null && emissiveMaterial != null)
        {
            Material[] mats = screenRenderer.materials;
            mats[1] = emissiveMaterial; 
            screenRenderer.materials = mats;
        }

        if (textMesh != null)
            textMesh.SetActive(true);

        isOn = true;
        if (!insideCourtine)
        {
            IsHacked = true;
        }
    }

    private void TurnOffComputer()
    {
        if (screenRenderer != null && normalMaterial != null)
        {
            Material[] mats = screenRenderer.materials;
            mats[1] = normalMaterial;
            screenRenderer.materials = mats;
        }

        if (textMesh != null)
            textMesh.SetActive(false);

        isOn = false;
        if (!insideCourtine)
        {
            IsHacked = false;
        }
    }

    public HackType HackType => HackType.CanBeReHacked;
    public event Action OnHacked;
    public bool IsHacked { get; set; }
    public void Hack()
    {
        IsHacked = true;
        OnHacked?.Invoke();
        StartCoroutine(FlickerRoutine(true,hackingDuration));
    }
    
    private bool insideCourtine = false;
    private IEnumerator FlickerRoutine(bool finalStateOn, float duration)
    {
        insideCourtine = true;
        var timer = 0f;

        while (timer < duration)
        {
            var wait = Random.Range(0.05f, 0.25f);

            if (Random.value > 0.5f)
                TurnOnComputer();
            else
                TurnOffComputer();

            yield return new WaitForSeconds(wait);
            timer += wait;
        }

        if (finalStateOn)
        {
           TurnOnComputer();
           IsHacked = true;
           computerHacked.Raise();
        }
        else
        {
            TurnOffComputer();
            IsHacked = false;
        }
        insideCourtine = false;
    }

    public void TurnOffComputerWithDelay(float timer)
    {
        Invoke(nameof(TurnOffComputer),timer);
       // StartCoroutine(FlickerRoutine(false,timer));
    }

    public void TurnOnComputerWithDelay(float timer)
    {
        StartCoroutine(FlickerRoutine(true,timer));
    }

    public float FOV => fov;
}
