using System.Collections;
using UnityEngine;
public class Character : MonoBehaviour
{
    private PlayerInput _playerInput;
    private CharacterController _characterController;
    [SerializeField] private float moveSpeed;
    private Vector3 _characterVelocity;
    private Animator _characterAnimator;

    public int totalCoin;
    //Enemy
    public bool isPlayer = true;
    private Transform _targetPlayer;
    private UnityEngine.AI.NavMeshAgent _navMeshAgent;
    //Damage Caster
    private DamageCaster _damageCaster;
    //Health
    private Health _health;
    //Player Slides
    private float _attackStartTime;
    [SerializeField] private float attackSlideDuration = 0.4f;
    [SerializeField] private float attackSlideSpeed = 0.06f;

    private Vector3 _impactOnCharacter;
    private bool _isInvincible;
    private float _invincibleDuration = 2f;
    private float _attackAnimationDuration;
    private float _slideSpeed = 9f;
    
    public enum CharacterState
    {
        Normal, Attacking, Dead, BeingHit, Slide, Spawn
    }

    public CharacterState currentState;
    [SerializeField] private float spawnDuration = 2f;

    private float _currentSpawnTime;
    // Material animation
    private MaterialPropertyBlock _materialPropertyBlock;
    private SkinnedMeshRenderer _skinnedMeshRenderer;

    [SerializeField] private GameObject itemToDrop;

    private bool _isButtonClicked;
    void Awake()
    {
        _damageCaster = GetComponentInChildren<DamageCaster>();
        _characterController = GetComponent<CharacterController>();
        _characterAnimator = GetComponent<Animator>();
        _health = GetComponent<Health>();
        _skinnedMeshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
        _materialPropertyBlock = new MaterialPropertyBlock();
        _skinnedMeshRenderer.GetPropertyBlock(_materialPropertyBlock);
        if (!isPlayer)
        {
            _targetPlayer = GameObject.FindWithTag("Player").transform;
            _navMeshAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();
            _navMeshAgent.speed = moveSpeed;
            SwitchStateTo(CharacterState.Spawn);
        }
        else
        {
            _playerInput = GetComponent<PlayerInput>();
        }
    }

    void CalculateEnemyMovement()
    {
        if (Vector3.Distance(_targetPlayer.position, transform.position) >= _navMeshAgent.stoppingDistance)
        {
            _navMeshAgent.SetDestination(_targetPlayer.position);
            _characterAnimator.SetFloat(Animator.StringToHash("Speed"), 0.2f);
        }
        else
        {
            _navMeshAgent.SetDestination(transform.position);
            _characterAnimator.SetFloat(Animator.StringToHash("Speed"), 0f);
            SwitchStateTo(CharacterState.Attacking);
        }
    }

    public void TriggerAttack()
    {
        _isButtonClicked = true;
    }

    void CalculateMovement()
    {
        if (_isButtonClicked && _characterController.isGrounded)
        {
            _isButtonClicked = false;
            SwitchStateTo(CharacterState.Attacking);
            return;
        } 
        if (_playerInput.isSpaceDown && _characterController.isGrounded)
        {
            SwitchStateTo(CharacterState.Slide);
            return;
        }
        _characterVelocity.Set(_playerInput.horizontalInput, 0f, _playerInput.verticalInput);
        _characterVelocity.Normalize();
        _characterVelocity = Quaternion.Euler(0f, -45f, 0f) * _characterVelocity;
        _characterVelocity *= moveSpeed * Time.deltaTime;
        _characterAnimator.SetFloat(Animator.StringToHash("Speed"), _characterVelocity.magnitude);
        if (_characterVelocity != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(_characterVelocity);
        }
        _characterAnimator.SetBool(Animator.StringToHash("AirBorne"), !_characterController.isGrounded);
    }
    
    void FixedUpdate()
    {
        switch (currentState)
        {
            case CharacterState.Normal:
                if (isPlayer)
                {
                    CalculateMovement();
                }
                else
                {
                    CalculateEnemyMovement();
                }
                break;
            case CharacterState.Attacking:
                if (isPlayer)
                {
                    if (Time.time < _attackStartTime + attackSlideDuration)
                    {
                        float timePassed = Time.time - _attackStartTime;
                        float lerpTime = timePassed / attackSlideDuration;
                        _characterVelocity = Vector3.Lerp(transform.forward * attackSlideSpeed, Vector3.zero, lerpTime);
                    }
                    if (_playerInput.isMouseDown && _characterController.isGrounded)
                    {
                        string currentClipName = _characterAnimator.GetCurrentAnimatorClipInfo(0)[0].clip.name;
                        _attackAnimationDuration = _characterAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime;
                        if (currentClipName != "LittleAdventurerAndie_ATTACK_03" && _attackAnimationDuration > 0.1f && _attackAnimationDuration < 0.7f)
                        {
                            _playerInput.isMouseDown = false;
                            SwitchStateTo(CharacterState.Attacking);
                        }
                    }
                }
                break;
            case CharacterState.Dead:
                return;
            case CharacterState.BeingHit:
                break;
            case CharacterState.Slide:
                _characterVelocity = Time.deltaTime * _slideSpeed * transform.forward;
                break;
            case CharacterState.Spawn:
                _currentSpawnTime -= Time.deltaTime;
                if (_currentSpawnTime <= 0)
                {
                    SwitchStateTo(CharacterState.Normal);
                }
                break;
        }
        if (_impactOnCharacter.magnitude > 0.2f)
        {
            _characterVelocity = _impactOnCharacter * Time.deltaTime;
        }
        _impactOnCharacter = Vector3.Lerp(_impactOnCharacter, Vector3.zero, Time.deltaTime * 5);

        if (isPlayer)
        {
            if (!_characterController.isGrounded)
            {
                _characterVelocity += -20f * Time.deltaTime * Vector3.up;
            }
            else
            {
                _characterVelocity += -20f * 0.3f * Time.deltaTime * Vector3.up;
            }
            _characterController.Move(_characterVelocity);
            _characterVelocity = Vector3.zero;
        }
        else
        {
            if (currentState != CharacterState.Normal)
            {
                _characterController.Move(_characterVelocity);
                _characterVelocity = Vector3.zero;
            }
        }
    }
    public void SwitchStateTo(CharacterState newState)
    {
        if (isPlayer)
        {
            _playerInput.ClearCache();
        }
        switch (currentState)
        {
            case CharacterState.Attacking:
                if (_damageCaster != null)
                {
                    DisableDamageCaster();
                }

                if (isPlayer)
                {
                    GetComponent<PlayerVFXManager>().StopBlade();
                }
                break;
            case CharacterState.Normal:
                break;
            case CharacterState.Dead:
                return;
            case CharacterState.BeingHit:
                break;
            case CharacterState.Slide:
                break;
            case CharacterState.Spawn:
                _isInvincible = false;
                break;
        }

        switch (newState)
        {
            case CharacterState.Attacking:
                _characterAnimator.SetTrigger(Animator.StringToHash("Attack"));
                if (isPlayer)
                {
                    //RotateToCursor();
                    _attackStartTime = Time.time;
                }
                else
                {
                    Quaternion newRotation = Quaternion.LookRotation(_targetPlayer.position - transform.position);
                    transform.rotation = newRotation;
                }
                break;
            case CharacterState.Normal:
                break;
            case CharacterState.Dead:
                _characterController.enabled = false;
                _characterAnimator.SetTrigger(Animator.StringToHash("Dead"));
                StartCoroutine(MaterialDissolve());
                break;
            case CharacterState.BeingHit:
                _characterAnimator.SetTrigger(Animator.StringToHash("BeingHit"));
                if (isPlayer)
                {
                    _isInvincible = true;
                    StartCoroutine(DelayCancelInvincible());
                }
                break;
            case CharacterState.Slide:
                _characterAnimator.SetTrigger(Animator.StringToHash("Slide"));
                break;
            case CharacterState.Spawn:
                _isInvincible = true;
                _currentSpawnTime = spawnDuration;
                StartCoroutine(MaterialAppear());
                break;
        }
        currentState = newState;
    }

    public void SlideAnimationEnds()
    {
        SwitchStateTo(CharacterState.Normal);
    }

    public void BeingHitAnimationEnds()
    {
        SwitchStateTo(CharacterState.Normal);
    }
    public void AttackAnimationEnds()
    {
        SwitchStateTo(CharacterState.Normal);
    }

    public void ApplyDamage(int damage, Vector3 attackPos)
    {
        if(_isInvincible) {return;}
        if (_health != null)
        {
            _health.ApplyDamage(damage);
        }

        if (!isPlayer)
        {
            GetComponent<EnemyVFXManager>().PlayBeingHitVFX(attackPos);
        }

        StartCoroutine(MaterialBlink());
        if (isPlayer)
        {
            SwitchStateTo(CharacterState.BeingHit);
            AddImpact(attackPos, 10f);
        }
        else
        {
            AddImpact(attackPos, 2.5f);
        }
    }

    IEnumerator DelayCancelInvincible()
    {
        yield return new WaitForSeconds(_invincibleDuration);
        _isInvincible = false;
    }

    public void AddImpact(Vector3 attackerPos, float force)
    {
        Vector3 impactDir = transform.position - attackerPos;
        impactDir.Normalize();
        impactDir.y = 0;
        _impactOnCharacter = impactDir * force;
    }

    public void EnableDamageCaster()
    {
        _damageCaster.EnableDamageCaster();
    }

    public void DisableDamageCaster()
    {
        _damageCaster.DisableDamageCaster();
    }

    IEnumerator MaterialBlink()
    {
        _materialPropertyBlock.SetFloat(Shader.PropertyToID("_blink"), 0.4f);
        _skinnedMeshRenderer.SetPropertyBlock(_materialPropertyBlock);
        yield return new WaitForSeconds(0.2f);
        _materialPropertyBlock.SetFloat(Shader.PropertyToID("_blink"), 0f);
        _skinnedMeshRenderer.SetPropertyBlock(_materialPropertyBlock);
    }

    IEnumerator MaterialDissolve()
    {
        yield return new WaitForSeconds(2);
        float dissolveDuration = 2f;
        float currentDissolveTime = 0f;
        float dissolveHighStart = 20f;
        float dissolveHighTarget = -10f;
        float dissolveHigh;
        _materialPropertyBlock.SetFloat(Shader.PropertyToID("_enableDissolve"), 1f);
        _skinnedMeshRenderer.SetPropertyBlock(_materialPropertyBlock);
        while (currentDissolveTime < dissolveDuration)
        {
            currentDissolveTime += Time.deltaTime;
            dissolveHigh = Mathf.Lerp(dissolveHighStart, dissolveHighTarget, currentDissolveTime / dissolveDuration);
            _materialPropertyBlock.SetFloat(Shader.PropertyToID("_dissolve_height"), dissolveHigh);
            _skinnedMeshRenderer.SetPropertyBlock(_materialPropertyBlock);
            yield return null;
        }
        DropItem();
    }

    public void DropItem()
    {
        if (itemToDrop != null)
        {
            Instantiate(itemToDrop, transform.position, Quaternion.identity);
        }
    }

    public void PickUpItem(PickUp item)
    {
        switch (item.type)
        {
            case PickUp.PickUpType.Coin:
                AddCoin(item.value);
                break;
            case PickUp.PickUpType.Heal:
                AddHeal(item.value);
                break;
        }
    }

    private void AddHeal(int value)
    {
        _health.AddHealth(value);
        GetComponent<PlayerVFXManager>().PlayHealVFX();
    }

    private void AddCoin(int value)
    {
        totalCoin += value;
    }

    public void RotateToTarget()
    {
        if (currentState != CharacterState.Dead)
        {
            transform.LookAt(_targetPlayer, Vector3.up);
        }
    }

    IEnumerator MaterialAppear()
    {
        float dissolveTimeDuration = spawnDuration;
        float currentDissolveTime = 0;
        float dissolveHightStart = -10f;
        float dissolveHightTarget = 20f;
        float dissolveHight;
        _materialPropertyBlock.SetFloat(Shader.PropertyToID("_enableDissolve"),1f);
        _skinnedMeshRenderer.SetPropertyBlock(_materialPropertyBlock);
        while (currentDissolveTime < dissolveTimeDuration)
        {
            currentDissolveTime += Time.deltaTime;
            dissolveHight = Mathf.Lerp(dissolveHightStart, dissolveHightTarget,
                currentDissolveTime / dissolveTimeDuration);
            _materialPropertyBlock.SetFloat(Shader.PropertyToID("_dissolve_height"), dissolveHight);
            _skinnedMeshRenderer.SetPropertyBlock(_materialPropertyBlock);
            yield return null;
        }
        _materialPropertyBlock.SetFloat(Shader.PropertyToID("_enableDissolve"),0f);
        _skinnedMeshRenderer.SetPropertyBlock(_materialPropertyBlock);
    }

    private void RotateToCursor()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitResult;
        if (Physics.Raycast(ray, out hitResult, 1000, 1 << LayerMask.NameToLayer("CursorTest")))
        {
            Vector3 cursorPos = hitResult.point;
            transform.rotation = Quaternion.LookRotation(cursorPos - transform.position, Vector3.up);
        }
    }
}
