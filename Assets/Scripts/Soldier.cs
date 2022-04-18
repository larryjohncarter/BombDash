using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;



public class Soldier : MonoBehaviour, ITarget
{
	private float _yOffset, _health;
	private CharacterController _controller;
	private float  _waitingTimeCount = 0;
	private Vector3 _targetPos;
	private NavMeshAgent _agent;
	private AnimController _animController;
	private IGunController _currentGunController;
	private List<IGunController> _guncontrollers;
	[SerializeField] private GameObject[] _gunGameObjects;
	[SerializeField] private Renderer _renderer;
	[SerializeField] private bool _isBase = false;
	[SerializeField] private float _waitingTimeSc = 1.5f;
	public SoldierTypeConfig SoldierConfig { get; private set; }
	public SoldierInfo SoldierInfo { get; private set; }
	public GameObject MainObject => gameObject;
	public Team Team => SoldierInfo.Team;
	public bool IsBase => _isBase;
	public float Health => SoldierConfig.Health;
	public float AttackRange => SoldierConfig.AttackRange * SoldierConfig.AttackRange;
	public float Damage => SoldierConfig.Damage;
	public ITarget Target { get; private set; }
	public bool CanTargeted { get; set; }
	public Action<ITarget> UnBindHandler { get; set; }
	private GenericPool<ImpactControl> _impacts;
	public Team TeamS;
	private Ray ray;
	private RaycastHit hit;

	public AreaPlacer Area;
	private void Awake()
	{
		_agent = GetComponent<NavMeshAgent>();
		_animController = GetComponent<AnimController>();
		_controller = GetComponent<CharacterController>();
		_yOffset = _controller.center.y;
		
		_agent.updateRotation = false;
		_agent.updatePosition = false;
		_guncontrollers = new List<IGunController>();
		foreach(var gun in _gunGameObjects)
		{
			var gunTemp = gun.GetComponent<IGunController>();
			gunTemp.AttackOwner = this;
			_guncontrollers.Add(gunTemp);
		}
	}

	public bool TakeDamage(float value)
	{
		_health -= value;
		if (_health <= 0)
		{
			if (!_isBase)
			{
				SelfDemolish();
			}
			return false;
		}
		return true;
	}

	public void Setup(Vector3 position, Quaternion rotation, Vector3 scale, SoldierInfo info, SoldierTypeConfig config, GenericPool<ImpactControl> impacts)
	{
		transform.position = position;
		transform.localRotation = rotation;
		//transform.localScale = scale;
		_agent.nextPosition = transform.position;
		SoldierInfo = info;
		SoldierConfig = config;
		_health = SoldierConfig.Health;
		_renderer.material = SoldierInfo.Material;
		gameObject.layer = (int)Math.Log(SoldierInfo.OwnLayer, 2);
		SetGunController(config);
		transform.name = Team.ToString();
		_impacts = null;
		_impacts = impacts;
		_animController.Win(false);
		_animController.Attack(-1);
	}

	private void SetGunController(SoldierTypeConfig config)
	{
		foreach(var gun in _guncontrollers)
		{
			gun.SetActive(false);
		}
		_currentGunController = _guncontrollers[(int)config.GunType];
		_currentGunController.Setup(config);
		_currentGunController.SetActive(true);
	}
	private void Update()
	{
		if(Physics.Raycast(transform.position,-Vector3.up,out hit, 5f))
        {
            if (hit.collider.CompareTag(TagControl.Water))
            {
				TakeDamage(10);
				Debug.Log($"This GameObject: {gameObject.name} Has Killed the enemy.");
			}
        }
		TeamS = Team;
		if(GameManager.Instance.isGameOver)
		{
			if(Team == GameManager.Instance.LoserTeam)
				TakeDamage(10);
			else
			{
				_animController.Win(true);
				_currentGunController.SetActive(false);
			}
		}
		//_agent.nextPosition = transform.position;
		if (_agent.pathPending || Target == null)
		{
			_animController.Run(0);
			return;
		}
		if (Target.CanTargeted)
		{
			CheckDestiantionReach();
			LookAt();
		}
		else
		{
			_animController.Run(0);
			_animController.Attack(-1);
		}
	}
	private void LateUpdate()
	{
		if (_agent.pathPending || Target == null || !Target.CanTargeted)
			return;
		_agent.nextPosition = transform.position;
	}

	//Oldugunun bilgisini veriyor
	private void UnBindTarget(ITarget soldier)
	{
		soldier.UnBindHandler -= UnBindTarget;
		Target = null;
		
		// Debug.Log($"{name} Demolish {gameObject.GetInstanceID()}");
		// Debug.Log($"{soldier.MainObject.name} DemolishT {gameObject.GetInstanceID()}");
	}
	public void SelfDemolish()
	{
		//Debug.Log($"{name} SelfDemolish {gameObject.GetInstanceID()}");
		if(CanTargeted)
		{
			UnBindHandler?.Invoke(this);
			PoolManager.Instance.Soldiers.Push(this);
			if (_impacts != null)
			{
				var impact = _impacts.Pop();
				impact.SetupImpact(transform.position, transform.forward, (i) => _impacts.Push(i));
				impact.DoImpact();
			}
		}
		CanTargeted = false;
		Target = null;
	}

	private void CheckDestiantionReach()
	{
		_targetPos = Target.GetClosestPosition(transform.position);
		if (_targetPos != transform.position)
		{
			_targetPos.y = 0;
			var pos = transform.position;
			pos.y = 0;
			var distance = (_targetPos - pos).sqrMagnitude;
			if (distance >= AttackRange)
			{
				SetDestination(_targetPos);
				_controller.Move(_agent.desiredVelocity.normalized * SoldierConfig.Speed * Time.deltaTime);
				if (_agent.desiredVelocity.sqrMagnitude < (SoldierConfig.Speed * .3f))
					_waitingTimeCount += Time.deltaTime;
				if (_waitingTimeCount >= _waitingTimeSc)
				{
					//Debug.Log($"{gameObject.name} is stucked");
					_waitingTimeCount = 0;
				}
				// if (_gunController.IsPlaying)
				// 	_gunController.Stop();
				_animController.Attack(-1);
			}
			else
			{
				_currentGunController.PerformAttack();
				_animController.Attack((int)_currentGunController.GunType);
				//Debug.Log(gameObject.name + " " + _currentGunController.GunType);
				_agent.isStopped = true;
				_controller.Move(Vector3.zero);

			}
			pos = transform.position;
			pos.y = _yOffset;
			transform.position = pos;
		}
		_animController.Run(_controller.velocity.magnitude / SoldierConfig.Speed);
	}

	private void LookAt()
	{
		var dir = (_targetPos - transform.position).normalized;
		if (dir.x == 0 && dir.z == 0)
			return;
		var rot = Quaternion.LookRotation(new Vector3(dir.x, 0, dir.z));
		transform.rotation = Quaternion.Slerp(transform.rotation, rot, Time.deltaTime * 8f);
	}

	private void SetDestination(Vector3 value)
	{
		value.y = 0;
		_agent.destination = value;
		_agent.isStopped = false;
	}

	public void SetTarget(ITarget target)
	{
		Target = target;
		if (Target != null)
		{
			//Debug.Log($"{gameObject.name} targeted : {target.MainObject.name} Attack Range : {_soldierConfig.AttackRange}");
			Target.UnBindHandler += UnBindTarget;
		}
	}

	public Vector3 GetClosestPosition(Vector3 position)
	{
		return transform.position;
	}

#if UNITY_EDITOR
	private void OnDrawGizmos()
	{
		if (Target == null)
			return;

		Gizmos.color = Color.yellow;
		var tempT = _targetPos;
		var temPos = transform.position;
		tempT.y = temPos.y = _yOffset * 2;
		Gizmos.DrawLine(temPos, tempT);
		Gizmos.color = Color.clear;

	}
#endif
}