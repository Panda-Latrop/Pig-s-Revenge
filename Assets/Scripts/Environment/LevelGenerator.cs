using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : NavigationGrid
{
	[SerializeField]
	protected Vector2Int unbreakableObstacleMOD = Vector2Int.one;
	[SerializeField]
	protected int breakableObstacleCount;
	[SerializeField]
	protected LevelObstacleActor unbreakableObstacle, breakableObstacle;

    public override void CreateGrid()
    {
		nodes = new List<Node>(Length);
		int id = -1;
		LevelObstacleActor loa = default;
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
				//Collider2D collider = Physics2D.OverlapCircle(position, (Mathf.Min(nodeSize.x, nodeSize.y)) / 2.0f * 0.75f, (1 << blockLayer) | (1 << dynamicLayer));
				Node.NodeState state = default;
				//if (collider != null)
				//	state = (1 << collider.gameObject.layer) == (1 << blockLayer) ? Node.NodeState.blocked : Node.NodeState.dynamic;
				//else
					state = Node.NodeState.open;			
				Node node = new Node(state, position, id, x, y);
				nodes.Add(node);
				if ((x + 1) % unbreakableObstacleMOD.x == 0 && (y + 1) % unbreakableObstacleMOD.y == 0)
				{
					loa = GameInstance.Instance.PoolManager.Pop(unbreakableObstacle) as LevelObstacleActor;
					loa.SetLevelGenerator(this, node);
					loa.SetPosition(position);
					loa.SetParent(transform);
					closePoints.Add(node);
					node.SetBlocked();
				}
                else
                {
					if(x > 0 && x < gridCount.x-1)
					openPoints.Add(node);

				}
			}
		}
		for (int i = 0; i < breakableObstacleCount; i++)
		{
			int rand = Random.Range(0, openPoints.Count);
			Node node = openPoints[rand];
			Vector3 point = node.position;		
			loa = GameInstance.Instance.PoolManager.Pop(breakableObstacle) as LevelObstacleActor;
			loa.SetLevelGenerator(this, node);
			loa.SetPosition(point);
			loa.SetParent(transform);
			ClosenPoint(node,true);
		}
	}
	public void OpenPoint(Node node)
	{
		if (closePoints.Remove(node))
        {
			node.SetOpen();
			openPoints.Add(node);
		}
			
	}
	public void ClosenPoint(Node node,bool isDynamic = false)
	{
		if (openPoints.Remove(node))
        {

			if (isDynamic)
				node.SetDynamic();
			else 
				node.SetBlocked();
			closePoints.Add(node);
		}
			
	}
	protected override void OnDrawGizmosSelected()
	{
		base.OnDrawGizmosSelected();
		Vector3 gridSize = new Vector3(gridCount.x * nodeSize.x, gridCount.y * nodeSize.y, 0.0f);
		int j = 0;
		for (int x = 0; x < gridCount.x; x++)
		{
			for (int y = 0; y < gridCount.y; y++)
			{
				if (unbreakableObstacleMOD.x <= 0)
					unbreakableObstacleMOD.x = 1;
				if (unbreakableObstacleMOD.y <= 0)
					unbreakableObstacleMOD.y = 1;
				if ((x + 1) % unbreakableObstacleMOD.x == 0 && (y + 1) % unbreakableObstacleMOD.y == 0)
					Gizmos.color = Color.red;
				else
				{
					if (j < breakableObstacleCount)
					{
						j++;
						Gizmos.color = Color.green;
					}
					else
					{
						Gizmos.color = Color.magenta;
					}

				}
				Gizmos.DrawSphere(transform.position - gridSize / 2.0f +
									new Vector3(nodeSize.x * x + nodeShift.x * y, nodeSize.y * y + nodeShift.y * x, 0.0f) +
									new Vector3(nodeSize.x / 2.0f, nodeSize.y / 2.0f, 0.0f),
									0.2f);
			}
		}
	}
}
