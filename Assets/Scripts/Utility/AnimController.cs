using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimController : MonoBehaviour
{
	private bool _winValue = false;
	private int _runHashID;
	private int _winHashID;
	private int _attackTypeHashID;
	[SerializeField] private Animator _animator;
	[SerializeField] private float _smoothTime = .1f;
	private void Awake()
	{
		_runHashID = Animator.StringToHash("Forward");
		_attackTypeHashID = Animator.StringToHash("AttackType");
		_winHashID = Animator.StringToHash("Victory");
	}


	public void Win(bool value)
	{
		if(_winValue == value)
			return;
		_winValue = value;
		if(value)
		{
			_animator.SetInteger(_attackTypeHashID, -1);
			_animator.SetFloat(_runHashID, 0);
			_animator.SetTrigger(_winHashID);
			return;
		}
		_animator.ResetTrigger(_winHashID);
		
	}
	public void Attack(int value)
	{
		_animator.SetInteger(_attackTypeHashID, value);
	}

	public void Run(float value)
	{	
		value = Mathf.Clamp01(value);
		_animator.SetFloat(_runHashID, value, _smoothTime, Time.deltaTime);
	}
}
