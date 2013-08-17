using UnityEngine;
using System.Collections;

public class ChangeBehaviorGUI : MonoBehaviour {
	public Enemy[] enemies;
	private Rect btn1Rect = new Rect(100,20,250,30);
	private Rect btn2Rect = new Rect(100,50,250,30);
	
	void OnGUI() {

		if(GUI.Button(btn1Rect, enemies[0].gameObject.name + ": " + enemies[0].enemyAI))
		{
			if(enemies[0].enemyAI == ENEMY_AI.ASTAR_TO_PLAYER)
				enemies[0].enemyAI = ENEMY_AI.LODE_RUNNER_CLONE;
			else
				enemies[0].enemyAI = ENEMY_AI.ASTAR_TO_PLAYER;
		}
		if(GUI.Button(btn2Rect, enemies[1].gameObject.name + ": " + enemies[1].enemyAI))
		{
			if(enemies[1].enemyAI == ENEMY_AI.ASTAR_TO_PLAYER)
				enemies[1].enemyAI = ENEMY_AI.LODE_RUNNER_CLONE;
			else
				enemies[1].enemyAI = ENEMY_AI.ASTAR_TO_PLAYER;
		}
	}
}
