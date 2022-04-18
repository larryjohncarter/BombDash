using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RandomGenerator : MonoBehaviour
{
	private float val = 1f;
	private Vector3 _scaleFactor = Vector3.one;
	[SerializeField] private Soldier _prefabSoldier;
	[SerializeField] private SoldierTypeConfig[] _configs;
	[SerializeField] private SoldierInfo[] _infos;
	[SerializeField] private Vector2 _minMaxX;
	[SerializeField] private Vector2 _minMaxZ;
	[SerializeField] private TextMeshProUGUI text;
	private void Start()
	{
		_scaleFactor = Vector3.one * val;
		text.text = "Scale F : " + val;
	}
	private void Update()
	{
		if (Input.GetMouseButtonDown(0))
		{
			StartCoroutine(Creator());
		}
	}

	IEnumerator Creator()
	{
		var pos = new Vector3(Random.Range(_minMaxX.x, _minMaxX.y), 1f, Random.Range(_minMaxZ.x, _minMaxZ.y));

		var soldier = PoolManager.Instance.Soldiers.Pop();
		var valC = Random.Range(0, _infos.Length);
		var val = Random.Range(0, _configs.Length);
		soldier.Setup(pos, Random.rotation, _scaleFactor, _infos[valC], _configs[val], PoolManager.Instance.DeadParticules);
		yield return new WaitForSeconds(.2f);
		soldier.CanTargeted = true;
	}

	public void ScaleFactor(float value)
	{
		_scaleFactor += Vector3.one * value;
		val += value;
		text.text = "Scale F : " + val;
	}
	public void Restart()
	{
		SceneManager.LoadScene(0);
	}
}
