using UnityEngine;
using DG.Tweening;
public class GameManager : SingletonBehaviour<GameManager>
{
	public GameObject enemyShield, playerShield;
	private ForceShield enemy, player;
	public bool isGameOver = false;
	public bool gameStarted = false;
	public Material playerShieldMat;
	public Material enemyShieldMat;
	public float endOfDissolve;
	public GameObject playerSideSoldier, enemySideSoldier;
	[SerializeField] private GameObject ringPlayerParticle, ringAIParticle;
	[SerializeField] private AreaPlacer _playerSideArea;
	[SerializeField] private AreaPlacer _enemySideArea;
	[SerializeField] private GameObject allianceSideBombHandle, enemySideBombHandle;
	public Team? LoserTeam { get; private set; } = null;
	[SerializeField] private float alphaSet;
	void Start()
	{
		if (player == null)
			player = playerShield.GetComponent<ForceShield>();
		if (enemy == null)
			enemy = enemyShield.GetComponent<ForceShield>();
		playerShieldMat.SetFloat("Alpha", alphaSet);
		enemyShieldMat.SetFloat("Alpha", alphaSet);
	}
	private void OnEnable()
	{
		if(player == null)
			player = playerShield.GetComponent<ForceShield>();
		if(enemy == null)
			enemy = enemyShield.GetComponent<ForceShield>();
		if(player)
			player.UnBindHandler += ShieldDestored;
			
		if(enemy)
			enemy.UnBindHandler += ShieldDestored;
	}
	private void OnDisable()
	{
		if (player)
			player.UnBindHandler -= ShieldDestored;

		if (enemy)
			enemy.UnBindHandler -= ShieldDestored;
	}

	//Kaybeden takimin teami geliyor burada
	public void ShieldDestored(ITarget target)
	{
		isGameOver = true;
		// _playerSideArea.KillThemAll(target.Team);
		// _enemySideArea.KillThemAll(target.Team);
		LoserTeam = target.Team;
		switch (target.Team)
		{
			case Team.Alliance: // Enemy wins
				isGameOver = true;
				enemyShieldMat.DOFloat(endOfDissolve, "Alpha", .5f);
				ringAIParticle.SetActive(false);
				enemySideSoldier.SetActive(true);
				enemySideBombHandle.transform.DOLocalMoveY(2.15f, .5f);
				LevelManager.Instance.retryButton.SetActive(true);
				break;
			case Team.Enemy: // Player wins
				isGameOver = true;
				//burada kapatacaklarini ayarlarsin
				playerShieldMat.DOFloat(endOfDissolve, "Alpha", .5f);
				ringPlayerParticle.SetActive(false);
				playerSideSoldier.SetActive(true);
				allianceSideBombHandle.transform.DOLocalMoveY(1f, .5f);
				LevelManager.Instance.continueGame.SetActive(true);
				break;
        }
	}
}
