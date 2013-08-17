/*
Lode runner enemy behaviour implemented by Adrian Seeto http://www.FunMobGames.com

Behaviour description was taken from http://games.groups.yahoo.com/group/LodeRunner/message/1883
and copy/pasted below for reference:
	 
1. First, they will attempt to reach the player's Y position (vertically),
then, once they are at the same height, the monks will then move in on the
player's X position (horizontally), meaning that they will try to match your
height, then move in for the kill.

2. To reach the same Y value of the player, they will reach for and climb the
nearest ladder that will go UP to that height, but not necessarily the one that
can reach the maximum height of the ladder, but rather, simply the one that goes
straight up to the player's current position. Meaning that if you climb a
ladder that's 20 squares high (for example), and there are two other ladders on
the screen, one reaching up to 5 squares, and 10 squares, then once you reach
the 5 square high one, they will attempt to climb THAT ladder up to it's maximum
height, even if the one that you are currently on is much much closer. Once you
get between 6-10 squares, they'll abandon the 5 ladder and head for the top of
the 10. It's really only once you get up to the 11th square, past the maximum
possible for the 10 square, that they will abandon their hunt and chase after
you on the 20 square ladder.

3. If your Y value is lower than theirs, they will climb down to your level.
They will never jump off a ladder, unless a. There is nothing between their
current position, and your current Y value. b. There is space for them to jump.
c. You are further away from them in the Y position than a certain number.


4. They do not seem to be aware of trap holes, or pits that are dug.

5. A corollary to 3, they will immediately jump off of a hanging rope if your Y
value is lower than theirs by even one square.

6. Their pick-up/drop gold rate is random between games. In the original,
which I'll admit to not having played very much, they seem to pick up the FIRST
gold they run across, and carry it with them until they die. In LR:
TLR/OL-TMMR, They have a specific percentage of picking up and dropping it,
which seems to be 60%, and usually drop it within 8 squares, but that's on
average.
*/
using UnityEngine;
using System.Collections;
using Pathfinding;

public enum ENEMY_AI { ASTAR_TO_PLAYER, LODE_RUNNER_CLONE }

[RequireComponent (typeof(Seeker))]
public class Enemy : Character {
	
	public ENEMY_AI enemyAI = ENEMY_AI.LODE_RUNNER_CLONE;
	
	/** Target to move to */
	public Player player;
	public Transform playerTr;
	public Transform target;
	
	/** How often to search for a new path */
	public float repathRate = 0.1F;
	
	/** The minimum distance to a waypoint to consider it as "reached" */
	public float pickNextWaypointDistance = 1F;
	
	
	/** Seeker component which handles pathfinding calls */
	protected Seeker seeker;
	
	
	
	/** Transform, cached because of performance */
	protected Transform tr;
	
	protected float lastPathSearch = -9999;
	
	protected int pathIndex = 0;
	
	/** This is the path the AI is currently following */
	protected Vector3[] path;
	
	/** Use this for initialization */
	public override void Start () {
		base.Start();
		
		seeker = GetComponent<Seeker>();
		
		tr = transform;
		Repath ();
	}
	
	/** Called when a path has completed it's calculation */
	public void OnPathComplete (Path p) {
		
		StartCoroutine (WaitToRepath ());
		
		//If the path didn't succeed, don't proceed
		if (p.error) {
			return;
		}
		
		//Get the calculated path as a Vector3 array
		path = p.vectorPath;
		
		//Find the segment in the path which is closest to the AI
		//If a closer segment hasn't been found in '6' iterations, break because it is unlikely to find any closer ones then
		float minDist = Mathf.Infinity;
		int notCloserHits = 0;
		
		for (int i=0;i<path.Length-1;i++) {
			float dist = Mathfx.DistancePointSegmentStrict (path[i],path[i+1],tr.position);
			
			if (dist < minDist) {
				notCloserHits = 0;
				minDist = dist;
				pathIndex = i+1;
			} else if (notCloserHits > 6) {
				break;
			}
		}
	}
	
	public IEnumerator WaitToRepath () {
		float timeLeft = repathRate - (Time.time-lastPathSearch);
		
		yield return new WaitForSeconds (timeLeft);
		Repath ();
	}
	
	/** Stops the AI. Does not prevent new path calls from making the AI move again */
	public void Stop () {
		pathIndex = -1;
	}
	
	/** Recalculates the path to #target */
	public virtual void Repath () {
		lastPathSearch = Time.time;
		
		if (seeker == null || target == null) {
			StartCoroutine (WaitToRepath ());
			return;
		}
	
		if(enemyAI == ENEMY_AI.ASTAR_TO_PLAYER)
		{	// vanilla A* to player
			target.position = playerTr.position;
		}
		else if(enemyAI == ENEMY_AI.LODE_RUNNER_CLONE)
		{
			// Recreate Lode Runner AI
			
			bool foundWalkableNode = false;
			
			MyGridGraph gridGraph = AstarPath.active.graphs[0] as MyGridGraph;
			
			int playerNodeRow = PlayerNodeRow();
		
			// Attempt to match vertical height of player
			if(Mathf.Abs(tr.position.y - playerTr.position.y) > 0.1f)
			{
				float minDist = Mathf.Infinity;
				int i, j;
				// scan the row the player is on for the nearest walkable node
				for(i = playerNodeRow * gridGraph.width, j = (playerNodeRow+1) * gridGraph.width;
				    i < j; i++)
				{
					if(gridGraph.myGridNodes[i].walkable && !gridGraph.myGridNodes[i].isFallLane)
					{
						Vector3 nodePosV3 = (Vector3) gridGraph.nodes[i].position;
						float dist = Vector3.Distance(tr.position, nodePosV3);
						if (dist < minDist)
						{
							minDist = dist;
							foundWalkableNode = true;
							target.position = nodePosV3;
						}
					}
				}
			}
			
			if(!foundWalkableNode)	// target player when vertical height matched 
				target.position = playerTr.position;
		}

		//Start a new path from transform.positon to target.position, return the result to the function 	OnPathComplete
		seeker.StartPath (tr.position,target.position,OnPathComplete);
	}
	private int PlayerNodeRow()
	{
		GridGraph gridGraph = AstarPath.active.graphs[0] as GridGraph;
		return Mathf.RoundToInt(playerTr.position.y - (float)gridGraph.nodes[0].position.y / 100f); // node.position is an Int3
	}
	/*
	private int MyNodeRow()
	{
		GridGraph gridGraph = AstarPath.active.graphs[0] as GridGraph;
		return Mathf.RoundToInt(tr.position.y - (float)gridGraph.nodes[0].position.y / 100f); // node.position is an Int3
	}
	private int PlayerNodeCol()
	{
		GridGraph gridGraph = AstarPath.active.graphs[0] as GridGraph;
		return Mathf.RoundToInt(playerTr.position.x - (float)gridGraph.nodes[0].position.x / 100f); // node.position is an Int3
	}
	private int MyNodeCol()
	{
		GridGraph gridGraph = AstarPath.active.graphs[0] as GridGraph;
		return Mathf.RoundToInt(tr.position.x - (float)gridGraph.nodes[0].position.x / 100f); // node.position is an Int3
	}*/
	
	//The AI has reached the end of the path
	public virtual void ReachedEndOfPath () {
	}
	
	public void Update ()
	{
		bool followPath = true; // follow path given by A*
		
		// implement behaviour specific to lode runner
		if(enemyAI == ENEMY_AI.LODE_RUNNER_CLONE)
		{
			// if we are on a ladder, and at the same vertical height as player
			if((!onRope && onLadder) && Mathf.Abs(tr.position.y - playerTr.position.y) < 0.1f)
			{
				// if there is no wall between us, then jump off the ladder in direction of player
				if(xa.IsGroundBetween(tr.position, playerTr.position) == false)
				{
					isUp = false;
					isDown = false;
					isLeft = false;
					isRight = false;
					
					if(tr.position.x - playerTr.position.x > 0)
						isLeft = true;
					isRight = !isLeft;
					
					followPath = false; // don't follow A* path for this Update
				}
			}
			else if(onRope)
			{
				// on rope and player is lower than us
				if(tr.position.y - playerTr.position.y >= 1) // jump off rope if player is >= 1 below 
				{
					isDown = true;
					followPath = false; // don't follow A* path for this Update
				}
			}
		}
		
		// standard A* path following
		if (followPath && path != null && pathIndex < path.Length && pathIndex >= 0)
		{
			//Change target to the next waypoint if the current one is close enough
			Vector3 currentWaypoint = path[pathIndex];
			currentWaypoint.z = tr.position.z;
		
			while ((currentWaypoint - tr.position).sqrMagnitude < pickNextWaypointDistance*pickNextWaypointDistance)
			{
				pathIndex++;
				if (pathIndex >= path.Length)
				{
					//Use a lower pickNextWaypointDistance for the last point. If it isn't that close, then decrement the pathIndex to the previous value and break the loop
					if ((currentWaypoint - tr.position).sqrMagnitude < (pickNextWaypointDistance*0.2)*(pickNextWaypointDistance*0.2)) {
						ReachedEndOfPath ();
						break;
					} else {
						pathIndex--;
						//Break the loop, otherwise it will try to check for the last point in an infinite loop
						break;
					}
				}
				currentWaypoint = path[pathIndex];
				currentWaypoint.z = tr.position.z;
			}
		
			Vector3 dir = currentWaypoint - tr.position;
		
			isUp = false;
			isDown = false;
			isLeft = false;
			isRight = false;
		
			if(dir.magnitude > 0)
			{	
				if(onRope)
					isDown =  dir.y <= -0.1f;
				else
					isDown = dir.y < -0f; // don't fall off rope but do descend ladders
				
				isUp = dir.y > 0f;
				isLeft = dir.x < -0f;
				isRight = dir.x > 0f;
			}
		}
		
		// sctual Character movement 
		UpdateMovement();
	}
}
