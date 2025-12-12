using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class HackingInputRouter : MonoBehaviour
{
    [FormerlySerializedAs("btnToogle")] [FormerlySerializedAs("btn_Toogle")] [SerializeField] private Button btnToggle;
    [FormerlySerializedAs("btn_hack")] [SerializeField] private Button btnHack;
    [SerializeField] private Button btnSwitchBack;
    [SerializeField]  Button btnHijack;
    [SerializeField] private Button btnSelfDestruct;
    [SerializeField] private Button btnPatrolRight;
    [SerializeField] private Button btnPatrolLeft;
    [SerializeField] private Button btnStun;
    
    [FormerlySerializedAs("survilanceUi")] [FormerlySerializedAs("SurvilanceUi")] [SerializeField] private GameObject surveillanceUI;
    [SerializeField] private Player player;

    [SerializeField] private GameObject forceField;  //temp

    #region Events
    public event Action OnScanRequested;
    public event Action OnHackRequested;
    public event Action OnSwitchBackRequested;
    public event Action OnHijackReqested;
    public event Action OnSelfDestructRequested;
    public event Action OnLeftPatrolPathSelected;
    public event Action OnRightPatrolPathSelected;
    public event Action OnStunRequested;
    #endregion
    
    public static HackingInputRouter Instance { get; private set; }
    
    public static int _currentMaxPriority = 10;


    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        player.PlayerStateChanged += PlayerOnPlayerStateChanged;
        
        btnToggle.onClick.AddListener(() => OnScanRequested?.Invoke());
        btnHack.onClick.AddListener(() => OnHackRequested?.Invoke());
        btnHijack.onClick.AddListener(() => OnHijackReqested?.Invoke());
        btnStun.onClick.AddListener(() => OnStunRequested?.Invoke());

        HackingCoordinator.Instance.TargetFound += obj =>
        {
            btnHijack.gameObject.SetActive(obj.TryGetComponent(out IHijack _));
            btnStun.gameObject.SetActive(obj.TryGetComponent(out IStun _));
        };
        
        SetButtonsInactive();

        btnSwitchBack.onClick.AddListener(() =>
        {
            OnSwitchBackRequested?.Invoke();
            player.SetPlayerState(ActionStates.Idle);
        });
        btnSelfDestruct.onClick.AddListener(() =>
        {
            OnSelfDestructRequested?.Invoke();
            player.SetPlayerState(ActionStates.Idle);
        });
        btnPatrolLeft.onClick.AddListener(() =>
        {
            OnLeftPatrolPathSelected?.Invoke();
            TogglePatrolSelectionUi(false);
        });
        btnPatrolRight.onClick.AddListener(() =>
        {
            OnRightPatrolPathSelected?.Invoke();
            TogglePatrolSelectionUi(false);
        });


    }

    private void SetButtonsInactive()
    {
        btnSwitchBack.gameObject.SetActive(false);
        btnHijack.gameObject.SetActive(false);
        surveillanceUI.SetActive(false);
        btnPatrolLeft.gameObject.SetActive(false);
        btnPatrolRight.gameObject.SetActive(false);
        btnStun.gameObject.SetActive(false);
    }

    private void PlayerOnPlayerStateChanged(ActionStates obj)
    {
        surveillanceUI.SetActive(obj == ActionStates.ControllingCctv);
        btnSwitchBack.gameObject.SetActive(obj == ActionStates.ControllingCctv || obj == ActionStates.Hijack);
        btnSelfDestruct.gameObject.SetActive(obj == ActionStates.Hijack);
    }

    //temp
    public GameObject InstantiateForceField(Vector3 spawnPos)
    {
        return Instantiate(forceField, spawnPos, Quaternion.identity);
    }

    public void TogglePatrolSelectionUi(bool status)
    {
        btnPatrolLeft.gameObject.SetActive(status);
        btnPatrolRight.gameObject.SetActive(status);
    }
    
    
}
