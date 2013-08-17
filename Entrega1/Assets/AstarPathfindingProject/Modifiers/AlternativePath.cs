using UnityEngine;
using System.Collections;
using Pathfinding;

[System.Serializable]
/** Applies penalty to the paths it processes telling other units to avoid choosing the same path.
 * \bug Can cause errors on Random.seed when using multithreading */
public class AlternativePath : MonoModifier {
	
	public override ModifierData input {
		get { return ModifierData.Original; }
	}
	
	public override ModifierData output {
		get { return ModifierData.All; }
	}
	
	public int penalty = 1000;
	
	public int randomStep = 10;
	
	Node[] prevNodes;
	int prevSeed;
	int prevPenalty = 0;
	
	bool waitingForApply = false;
	
	System.Object lockObject = new System.Object ();
	
	//Stack<Node[]> toBeApplied = new Stack<Node[]>();
	Node[] toBeApplied;
	public override void Apply (Path p, ModifierData source) {
		
		lock (lockObject) {
			toBeApplied = p.path;
			//AstarPath.active.RegisterCanUpdateGraphs (ApplyNow);
			if (!waitingForApply) {
				waitingForApply = true;
				AstarPath.OnPathPreSearch += ApplyNow;
			}
		}
	}
		
	void ApplyNow (Path somePath) {
		lock (lockObject) {
			waitingForApply = false;
			AstarPath.OnPathPreSearch -= ApplyNow;
			//toBeApplied.Add (p.nodes);
			int seed = prevSeed;
			Random.seed = prevSeed;
			
			//Add previous penalty
			if (prevNodes != null) {
				int rnd = Random.Range (0,randomStep);
				for (int i=rnd;i<prevNodes.Length;i+= Random.Range (1,randomStep)) {
					prevNodes[i].penalty -= prevPenalty;
				}
			}
			
			//Calculate a new seed
			seed = Random.Range (0,10000);
			Random.seed = seed;
			
			if (toBeApplied != null) {
				int rnd = Random.Range (0,randomStep);
				for (int i=rnd;i<toBeApplied.Length;i+= Random.Range (1,randomStep)) {
					toBeApplied[i].penalty += penalty;
				}
			}
			
			prevPenalty = penalty;
			prevSeed = seed;
			prevNodes = toBeApplied;
		}
	}
}
