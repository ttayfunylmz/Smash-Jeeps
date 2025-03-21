using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlayerSkillController : NetworkBehaviour
{
    public static event Action OnTimerFinished;

    [Header("References")]
    [SerializeField] private Transform _rocketLauncherTransform;

    [Header("Settings")]
    [SerializeField] private bool _hasSkillAlready;
    [SerializeField] private float _resetDelay;

    private MysteryBoxSkillsSO _mysteryBoxSkill;
    private PlayerVehicleController _playerVehicleController;
    private PlayerInteractionController _playerInteractionController;
    private PlayerDodgeController _playerDodgeController;

    private bool _isSkillUsed;
    private bool _hasTimerStarted;
    private float _timer;
    private float _timerMax;
    private int _amountCounter;

    public override void OnNetworkSpawn()
    {
        if(!IsOwner) { return; }

        _playerVehicleController = GetComponent<PlayerVehicleController>();
        _playerInteractionController = GetComponent<PlayerInteractionController>();
        _playerDodgeController = GetComponent<PlayerDodgeController>();

        _playerVehicleController.OnVehicleCrashed += PlayerVehicleController_OnVehicleCrashed;
        _playerDodgeController.OnDodgeStarted += PlayerDodgeController_OnDodgeStarted;
        _playerDodgeController.OnDodgeFinished += PlayerDodgeController_OnDodgeFinished;
    }

    private void PlayerDodgeController_OnDodgeFinished()
    {
        enabled = true;
    }

    private void PlayerDodgeController_OnDodgeStarted()
    {
        enabled = false;
    }

    private void PlayerVehicleController_OnVehicleCrashed()
    {
        SkillsUI.Instance.SetSkillToNone();
        _hasTimerStarted = false;
        _hasSkillAlready = false;
        enabled = false;
        SetRocketLauncherActiveRpc(false);
        _playerInteractionController.SetSpikeActive(false);
    }

    private void Update() 
    {
        if(!IsOwner) { return; }
        if(!_hasSkillAlready) { return ; }
        if(GameManager.Instance.GetGameState() != GameState.Playing) { return; }

        if(Input.GetKeyDown(KeyCode.Space) && !_isSkillUsed)
        {
            ActivateSkill();
            _isSkillUsed = true;
        }

        if(_hasTimerStarted)
        {
            _timer -= Time.deltaTime;
            SkillsUI.Instance.SetTimerCounterText((int)_timer);
            if(_timer <= 0f)
            {
                OnTimerFinished?.Invoke();
                SkillsUI.Instance.SetSkillToNone();
                _hasTimerStarted = false;
                _hasSkillAlready = false;

                if(_mysteryBoxSkill.SkillType == SkillType.Shield)
                {
                    _playerInteractionController.SetShieldActive(false);
                }

                if(_mysteryBoxSkill.SkillType == SkillType.Spike)
                {
                    _playerInteractionController.SetSpikeActive(false);
                }
            }
        }
    }

    public void SetupSkill(MysteryBoxSkillsSO skill)
    {
        _mysteryBoxSkill = skill;
        

        if(_mysteryBoxSkill.SkillType == SkillType.Rocket)
        {
            SetRocketLauncherActiveRpc(true);
        }

        _isSkillUsed = false;
        _hasSkillAlready = true;
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void SetRocketLauncherActiveRpc(bool active)
    {
        _rocketLauncherTransform.gameObject.SetActive(active);
    }

    public void ActivateSkill()
    {
        if(!_hasSkillAlready) { return; }

        SkillManager.Instance.ActivateSkill(_mysteryBoxSkill.SkillType, transform, OwnerClientId);
        SetSkillToNone();

        if(_mysteryBoxSkill.SkillType == SkillType.Rocket)
        {
            StartCoroutine(ResetRocketLauncher());
        }

        if(_mysteryBoxSkill.SkillType == SkillType.Shield)
        {
            _playerInteractionController.SetShieldActive(true);
        }

        if(_mysteryBoxSkill.SkillType == SkillType.Spike)
        {
            _playerInteractionController.SetSpikeActive(true);
        }
    }

    private IEnumerator ResetRocketLauncher()
    {
        yield return new WaitForSeconds(_resetDelay);
        SetRocketLauncherActiveRpc(false);
    }

    private void SetSkillToNone()
    {
        if(_mysteryBoxSkill.SkillUsageType == SkillUsageType.None)
        {
            _hasSkillAlready = false;
            SkillsUI.Instance.SetSkillToNone();
        }
        else if(_mysteryBoxSkill.SkillUsageType == SkillUsageType.Timer)
        {
            _hasTimerStarted = true;
            _timerMax = _mysteryBoxSkill.SkillData.SpawnAmountOrTimer;
            _timer = _timerMax;
        }
        else if(_mysteryBoxSkill.SkillUsageType == SkillUsageType.Amount)
        {
            _amountCounter = _mysteryBoxSkill.SkillData.SpawnAmountOrTimer;

            SkillManager.Instance.OnMineCountReduced += SkillManager_OnMineCountReduced;
        }
    }

    private void SkillManager_OnMineCountReduced()
    {
        _amountCounter--;
        SkillsUI.Instance.SetTimerCounterText(_amountCounter);

        if(_amountCounter <= 0)
        {
            _hasSkillAlready = false;
            SkillsUI.Instance.SetSkillToNone();
            SkillManager.Instance.OnMineCountReduced -= SkillManager_OnMineCountReduced;
        }
    }

    public bool HasSkillAlready()
    {
        return _hasSkillAlready;
    }

    public void OnPlayerRespawned() => enabled = true;

    public override void OnNetworkDespawn()
    {
        if(!IsOwner) { return; }

        _playerVehicleController.OnVehicleCrashed -= PlayerVehicleController_OnVehicleCrashed;
        _playerDodgeController.OnDodgeStarted -= PlayerDodgeController_OnDodgeStarted;
        _playerDodgeController.OnDodgeFinished -= PlayerDodgeController_OnDodgeFinished;
    }
}
