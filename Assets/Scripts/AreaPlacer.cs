
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AreaPlacer : MonoBehaviour
{
	[SerializeField] private ForceShield _base;
	public Team BaseTeam => _base.Team;
	public Dictionary<Team, List<ITarget>> _targets = new Dictionary<Team, List<ITarget>>();
	private int allianceCount;
	private int enemyCount;
	public int AllianceCount { get { return allianceCount; } }

	public List<Soldier> Alliances= new List<Soldier>();
	public List<Soldier> Enemies = new List<Soldier>();

	private void OnTriggerEnter(Collider other)
	{
		var projectile = other.GetComponent<Projectile>();
		var aiProjectile = other.GetComponent<AIProjectile>();
		if (projectile != null)
		{
			projectile.ArenaFinder(this);
		}
		if(aiProjectile != null)
        {
			aiProjectile.ArenaFinder(this);
        }
	}
	private void Update()
	{
		Searcher(Team.Enemy);
		Searcher(Team.Alliance);
		Counter(Team.Alliance);
		Counter(Team.Enemy);
	}

	private void Searcher(Team team)
	{
		if (_targets.ContainsKey(team))
		{
			if(Team.Alliance == team)
				Alliances.Clear();
			else
				Enemies.Clear();
			foreach (var soldier in _targets[team])
			{
				var s = soldier as Soldier;
				if (s is Soldier)
				{
					if(Team.Alliance == team)
						Alliances.Add(s);
					else
						Enemies.Add(s);
					if (s.Target == null)
						s.SetTarget(GetRandomTarget(team == Team.Enemy ? Team.Alliance : Team.Enemy));
				}
			}
		}
	}
	private void Counter(Team team)
    {
		if (_targets.ContainsKey(team))
		{
			var list = _targets[team];
			if(list != null)
            {
                switch (team)
                {
					case Team.Alliance:
						allianceCount = list.Count;
						break;
					case Team.Enemy:
						enemyCount = list.Count;
						break;
                }
			}
		}
	}
	public void UnBindTarget(ITarget target)
	{
		target.UnBindHandler -= UnBindTarget;
		RemoveTarget(target);
	}
	public void RemoveTarget(ITarget target)
	{
		_targets[target.Team].Remove(target);
	}

	//Istenilen Dusman
	public ITarget GetRandomTarget(Team team)
	{
		if (_targets.ContainsKey(team))
		{
			var list = _targets[team].FindAll(x => x.CanTargeted);
			
			if (list.Count > 0)
			{
				return list[Random.Range(0, list.Count)];
			}
			if (_targets[team].Count > 0)
				return null;
		}
		return team == _base.Team ? _base : null;
	}
	
	public void KillThemAll(Team team)
    {
        if (_targets.ContainsKey(team))
        {
			int i = 0;
			while(true)
			{
				var list = _targets[team];
				i = list.Count - 1;
				if (i <= 0)
					break;
				var soldier = list[i];
				soldier.TakeDamage(10);
				RemoveTarget(soldier);
			}

		}
    }

#if UNITY_EDITOR
	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.red * 0.3f;
		Gizmos.matrix = transform.localToWorldMatrix;
		Gizmos.DrawCube(Vector3.zero, Vector3.one);
		Gizmos.color = Color.clear;
	}
#endif
}

