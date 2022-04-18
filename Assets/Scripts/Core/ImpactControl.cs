using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImpactControl : MonoBehaviour
{

	private bool _isAlive = false;
	private float _aliveTimer;
	[SerializeField] private float _aliveTime = .5f;
	[SerializeField] private ParticleSystem _particle;
	private System.Action<ImpactControl> PushHandler;	
	private void OnEnable()
	{
		_aliveTimer = Time.time;
		_particle.gameObject.SetActive(true);
	}
	private void OnDisable()
	{
		_particle.gameObject.SetActive(false);
	}
	private void Update()
	{
		if(!_isAlive)
			return;
		if ((Time.time - _aliveTimer) >= _aliveTime)
		{
			PushHandler?.Invoke(this);
		}
	}

	public void SetupImpact(Vector3 position, Vector3 normal, System.Action<ImpactControl> push)
	{
		transform.position = position;
		transform.forward = normal;
		PushHandler = null;
		PushHandler = push;
		_isAlive = true;
	}
	public void DoImpact()
	{
		_particle.time = 0;
		_particle.Play();
		_aliveTimer = Time.time;
	}
}
