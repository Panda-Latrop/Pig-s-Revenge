using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine.SceneManagement;
using SimpleJSON;

public class NavigationGrid : MonoBehaviour
{


	[SerializeField]
	protected Vector2Int gridCount;
	[SerializeField]
	protected Vector2 nodeSize;
	[SerializeField]
	protected Vector2 nodeShift;
	[HideInInspector]
	protected List<Node> nodes;
	[SerializeField]
	protected int blockLayer,dynamicLayer;
	[HideInInspector]
	protected List<Node> openPoints = new List<Node>(), closePoints = new List<Node>();
	protected FileMaster fileMaster = new FileMaster();


	//   public void Start()
	//   {
	//       try
	//       {
	//		LoadGrid();
	//	}
	//       catch (System.Exception)
	//       {
	//		Debug.LogWarning("Not Found " + directory + SceneManager.GetActiveScene().name.ToLower() + ".grid");
	//		CreateGrid();
	//		SaveGrid();			
	//	}
	//}

	//protected string directory => Application.dataPath + "/StreamingAssets/Navigation/";
	protected string directory => Application.dataPath + "/Resources/Navigation/";

	[ContextMenu("Log")]
	protected void Log()
	{
		Debug.Log(nodes.Count + nodes[0].position.ToString());
	}

	[ContextMenu("Save Grid")]
	protected void SaveGrid()
	{
		fileMaster.WriteTo(directory + SceneManager.GetActiveScene().name.ToLower() + ".grid", nodes);
#if UNITY_EDITOR
		UnityEditor.AssetDatabase.Refresh();
#endif
	}
	[ContextMenu("Load Grid")]
	protected void LoadGrid()
	{
		nodes = fileMaster.ReadFrom<List<Node>>(directory + SceneManager.GetActiveScene().name.ToLower() + ".grid");
	}


	public int Length
	{
		get
		{
			return gridCount.x * gridCount.y;
		}
	}

	[ContextMenu("Create Grid")]
	public virtual void CreateGrid()
	{
		nodes = new List<Node>(Length);
		int id = -1;
		Vector3 position = Vector3.zero;
		Vector3 gridSize = new Vector3(gridCount.x * nodeSize.x, gridCount.y * nodeSize.y, 0.0f); 
		for (int y = 0; y < gridCount.y; y++)
		{
			for (int x = 0; x < gridCount.x; x++)
			{
				position = transform.position - gridSize / 2.0f +
									new Vector3(nodeSize.x * x + nodeShift.x * y, nodeSize.y * y + nodeShift.y * x, 0.0f) +
									new Vector3(nodeSize.x / 2.0f, nodeSize.y / 2.0f, 0.0f);
				id = x + y * gridCount.x;
				Collider2D collider = Physics2D.OverlapCircle(position, (Mathf.Min(nodeSize.x, nodeSize.y)) / 2.0f * 0.9f, (1 << blockLayer) | (1 << dynamicLayer));
				Node.NodeState state = default;
				if (collider != null)
					state = (1 << collider.gameObject.layer) == (1 << blockLayer) ? Node.NodeState.blocked : Node.NodeState.dynamic;
				else
					state = Node.NodeState.open;
					nodes.Add(new Node(state, position, id, x, y));
			}
		}
		//Debug.Log("Done Grid: " + nodes.Count);
	}

	[ContextMenu("Clear Grid")]
	protected void ClearGrid()
	{
		nodes = new List<Node>();
	}

	public int GetNeighbours(Node node, ref Node[] neighbours)
	{
		int count = 0;
		for (int x = -1; x <= 1; x++)
		{
			for (int y = -1; y <= 1; y++)
			{
				if ((x == 0 && y == 0) || (x != 0 && y != 0))
					continue;

				int checkX = node.idX + x;
				int checkY = node.idY + y;

				if (checkX >= 0 && checkX < gridCount.x && checkY >= 0 && checkY < gridCount.y)
				{
					neighbours[count] = nodes[checkX + checkY * gridCount.x];
					count++;
				}
			}
		}
		//int count = 0;
		//for (int x = -1; x <= 1; x++)
		//{
		//	for (int y = -1; y <= 1; y++)
		//	{
		//		if (x == 0 && y == 0)
		//			continue;

		//		int checkX = node.idX + x;
		//		int checkY = node.idY + y;

		//		if (checkX >= 0 && checkX < gridCount.x && checkY >= 0 && checkY < gridCount.y)
		//		{
		//			neighbours[count] = nodes[checkX + checkY * gridCount.x];
		//			count++;
		//		}
		//	}
		//}
		return count;
	}

	public Node GetNode(int id)
	{
		return nodes[id];
	}
	public Node GetOpenNode(int id)
	{
		return openPoints[id];
	}
	public int OpenLenth => openPoints.Count;
	public bool NodeFromWorld(Vector3 position, ref Node node)
	{
        node = nodes[0];
        float distance = (position - node.position).sqrMagnitude;
        for (int i = 1; i < nodes.Count; i++)
        {
            float current = (position - nodes[i].position).sqrMagnitude;
            if (distance > current)
            {
				node = nodes[i];
				distance = current;
			}
		}
		return true;
		//float halfNodeSize = nodeSize / 2.0f;
		//Vector3 gridSize = new Vector3(gridCount.x * nodeSize, gridCount.y * nodeSize, 0.0f);
		//Vector3 posShift = position + gridSize / 2.0f - transform.position - (Vector3.one * halfNodeSize);
		//int x = Mathf.RoundToInt(posShift.x / nodeSize);
		//int y = Mathf.RoundToInt(posShift.y / nodeSize);
		//int id = x + y * gridCount.x;
		//if (x >= 0 && x < gridCount.x && y >= 0 && y < gridCount.y && id < nodes.Count)
		//{
		//	node = nodes[id];
		//	return true;
		//      }
		//      else
		//      {
		//	return false;
		//      }
	}

	protected virtual void OnDrawGizmosSelected()
	{
		if (nodes != null && nodes.Count > 0)
		{
			Gizmos.color = Color.cyan;
			Vector3 gridSize = new Vector3(gridCount.x * nodeSize.x, gridCount.y * nodeSize.y, 0.0f);
			Gizmos.DrawWireCube(transform.position, gridSize);

			for (int i = 0; i < nodes.Count; i++)
			{
				if (nodes[i].IsBlocked())
					Gizmos.color = Color.red;
				else
				if (nodes[i].IsDynamic())
					Gizmos.color = Color.magenta;
				else
				if (nodes[i].IsOpen())
					Gizmos.color = Color.green;
				Gizmos.DrawWireCube(nodes[i].position,
										new Vector3(nodeSize.x, nodeSize.y, 0.0f));
			}
		}
		else
		{
			Gizmos.color = Color.cyan;
			Vector3 gridSize = new Vector3(gridCount.x * nodeSize.x, gridCount.y * nodeSize.y, 0.0f);
			Gizmos.DrawWireCube(transform.position, gridSize);
			Gizmos.color = Color.magenta;
			for (int x = 0; x < gridCount.x; x++)
			{
				for (int y = 0; y < gridCount.y; y++)
				{
					Gizmos.DrawWireCube(transform.position - gridSize / 2.0f +
									new Vector3(nodeSize.x * x + nodeShift.x * y, nodeSize.y * y + nodeShift.y * x, 0.0f) +
									new Vector3(nodeSize.x / 2.0f, nodeSize.y / 2.0f, 0.0f),
										new Vector3(nodeSize.x, nodeSize.y, 0.0f));
				}
			}
		}
	}
}