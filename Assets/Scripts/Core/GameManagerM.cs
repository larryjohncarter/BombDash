using UnityEngine;

public class GameManagerM : SingletonBehaviour<GameManagerM>
{

	public AreaPlacer Area;
	protected override void Awake()
	{
		base.Awake();
	}
}