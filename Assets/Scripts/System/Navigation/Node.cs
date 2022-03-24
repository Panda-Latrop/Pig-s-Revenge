using UnityEngine;
using System.Collections;
using SimpleJSON;

[System.Serializable]
public class Node : IHeapItem<Node> {
	
	public enum NodeState
    {
		open = 0,
		blocked = 2,
		dynamic = 4,
	}

	public NodeState nodeState;
	float posX, posY, posZ;

	public int id, idX, idY;

	public int gCost;
	public int hCost;
	public int parent;
	int heapIndex;

	public Vector3 position => new Vector3(posX, posY, posZ);
	public Node(NodeState nodeState, Vector3 pos,int id, int idX, int idY) {
		this.nodeState = nodeState;
		posX = pos.x;
		posY = pos.y;
		posZ = pos.z;
		this.id = id;
		this.idX = idX;
		this.idY = idY;

	}

	public int fCost {
		get {
			return gCost + hCost;
		}
	}

	public int HeapIndex {
		get {
			return heapIndex;
		}
		set {
			heapIndex = value;
		}
	}
	public void SetBlocked()
    {
		nodeState = NodeState.blocked;
	}
	public void SetOpen()
    {
		nodeState = NodeState.open;
	}
	public void SetDynamic() 
	{
		nodeState = NodeState.dynamic;
	}
	public bool IsBlocked()
	{
		return nodeState == NodeState.blocked;
	}
	public bool IsOpen()
	{
		return nodeState == NodeState.open;
	}
	public bool IsDynamic()
	{
		return nodeState == NodeState.dynamic;
	}
	public int CompareTo(Node nodeToCompare) {
		int compare = fCost.CompareTo(nodeToCompare.fCost);
		if (compare == 0) {
			compare = hCost.CompareTo(nodeToCompare.hCost);
		}
		return -compare;
	}
}
