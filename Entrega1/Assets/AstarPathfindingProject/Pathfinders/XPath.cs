using UnityEngine;
using System.Collections;

namespace Pathfinding {
	/** Extended Path.
	 * This is the same as a standard path but with a lot more customizations possible.
	 * \note With more customizations does make it slower to calculate but not by very much.
	 */
	public class XPath : Path {
	
		public PathEndingCondition endingCondition = new PathEndingCondition ();
		
		public XPath ()  : base () {}
		
		public XPath (Vector3 start, Vector3 end, OnPathDelegate callbackDelegate) : base (start,end,callbackDelegate) {}
			
		public override void Initialize () {
			base.Initialize ();
			
			if (current != null && endingCondition.TargetFound (this,startNode)) {
				foundEnd = true;
				endNode = startNode;
				Trace (endNode);
			}
		}
		
		public override float CalculateStep (float remainingFrameTime) {
			
			System.DateTime startTime = System.DateTime.Now;
			
			System.Int64 maxTicks = (System.Int64)(remainingFrameTime*10000);
			
			int counter = 0;
			
			//Continue to search while there hasn't ocurred an error and the end hasn't been found
			while (!foundEnd && !error) {
				
				//@Performance Just for debug info
				searchedNodes++;
				
				//Close the current node, if the current node satisfies the ending condition, terminate the path
				if (endingCondition.TargetFound (this,current)) {
					foundEnd = true;
					endNode = current;
					break;
				}
				
				/*if (current == null) {
					Debug.LogWarning ("Current is Null");
					return;
				}*/
				
				//Debug.DrawRay ( current.position, Vector3.up*10,Color.cyan);
				
				
				//Loop through all walkable neighbours of the node
				current.Open (open, hTarget,this);
				
				//any nodes left to search?
				if (open.numberOfItems <= 1) {
					LogError ("No open points, whole area searched");
					
					float durationThisFrame = (System.DateTime.Now.Ticks-startTime.Ticks)*0.0001F;
					duration += durationThisFrame;
						
					return durationThisFrame;
				}
				
				//Select the node with the lowest F score and remove it from the open list
				current = open.Remove ();
				
				//Check for time every 500 nodes to save cycles
				if (counter > 500) {
					
					//Have we exceded the maxFrameTime, if so we should wait one frame before continuing the search since we don't want the game to lag
					if ((System.DateTime.Now.Ticks-startTime.Ticks) > maxTicks) {//searchedNodesThisFrame > 20000) {
						
						
						float durationThisFrame = (System.DateTime.Now.Ticks-startTime.Ticks)*0.0001F;
						duration += durationThisFrame;
						
						//Return instead of yield'ing, a separate function handles the yield (CalculatePaths)
						return durationThisFrame;
					}
					
					counter = 0;
				}
				
				counter++;
			
			}
			
			if (foundEnd && !error) {
				Trace (endNode);
			}
			
			float durationThisFrame2 = (System.DateTime.Now.Ticks-startTime.Ticks)*0.0001F;
			duration += durationThisFrame2;
			
			//Return instead of yield'ing, a separate function handles the yield (CalculatePaths)
			return durationThisFrame2;
		}
	}
	
	/** Customized ending condition for a path.
	 * This class can be used to implement a custom ending condition for an Pathfinding::XPath.\n
	 * Inherit from this class and override the #TargetFound function to implement you own ending condition logic */
	public class PathEndingCondition {
		
		/** Has the ending condition been fulfilled.
		 * \param p The calculating path
		 * \param node The current node.
		 * This is per default the same as asking if \a node == \a p.endNode */
		public virtual bool TargetFound (Path p, Node node) {
			return node == p.endNode;
		}
	}
}