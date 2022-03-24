using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LevelGenerator2 : MonoBehaviour
{
	[SerializeField]
	protected Vector2Int gridCount;
	[SerializeField]
	protected Vector2 nodeSize;
	[SerializeField]
	protected Vector2 nodeShift;
	[SerializeField]
	protected Vector2Int unbreakableObstacleMOD = Vector2Int.one;
	[SerializeField]
	protected int breakableObstacleCount;
	[SerializeField]
	protected LevelObstacleActor unbreakableObstacle, breakableObstacle;
	[HideInInspector]
	protected List<Vector3> openPoints = new List<Vector3>(), closePoints = new List<Vector3>();

	[ContextMenu("DEBUG_P")]
	public void DEBUG_P()
	{


		Vector3 gridSize = new Vector3(gridCount.x * nodeSize.x, gridCount.y * nodeSize.y, 0.0f);
		int j = 0;
		for (int x = 0; x < gridCount.x; x++)
		{
			for (int y = 0; y < gridCount.y; y++)
			{
				if (j < transform.childCount && (x + 1) % unbreakableObstacleMOD.x == 0 && (y + 1) % unbreakableObstacleMOD.y == 0)
				{
					transform.GetChild(j).transform.position = transform.position - gridSize / 2.0f +
									new Vector3(nodeSize.x * x + nodeShift.x * y, nodeSize.y * y + nodeShift.y * x, 0.0f) +
									new Vector3(nodeSize.x / 2.0f, nodeSize.y / 2.0f, 0.0f);
					j++;
				}



			}
		}
	}
	[ContextMenu("Generate")]
	public void Generate()
	{
		Vector3 position = Vector3.zero;
		Vector3 gridSize = new Vector3(gridCount.x * nodeSize.x, gridCount.y * nodeSize.y, 0.0f);
		LevelObstacleActor loa = default;
		for (int y = 0; y < gridCount.y; y++)
		{
			for (int x = 1; x < gridCount.x-1; x++)
			{
				position.Set(x * nodeSize.x + nodeSize.x / 2.0f - gridSize.x / 2.0f + nodeShift.x * y, y * nodeSize.y + nodeSize.y / 2.0f - gridSize.y / 2.0f + nodeShift.y * x, 0);
				position += transform.position;

				if ((x + 1) % unbreakableObstacleMOD.x == 0 && (y + 1) % unbreakableObstacleMOD.y == 0)
				{
					loa = GameInstance.Instance.PoolManager.Pop(unbreakableObstacle) as LevelObstacleActor;
					//loa.SetLevelGenerator(this);
					loa.SetPosition(position);
					loa.SetParent(transform);
					closePoints.Add(position);
				}
				else
					openPoints.Add(position);
			}
		}
        for (int i = 0; i < breakableObstacleCount; i++)
        {
			int rand = Random.Range(0, openPoints.Count);
			Vector3 point = openPoints[rand];
			loa = GameInstance.Instance.PoolManager.Pop(breakableObstacle) as LevelObstacleActor;
			//loa.SetLevelGenerator(this);
			loa.SetPosition(point);
			loa.SetParent(transform);
			ClosenPoint(point);
		}
		Debug.Log("Done Level: " + transform.childCount);
	}
	public void OpenPoint(Vector3 point)
    {
		if (closePoints.Remove(point))
			openPoints.Add(point);
    }
	public void ClosenPoint(Vector3 point)
	{
		if (openPoints.Remove(point))
			closePoints.Add(point);
	}
	private void OnDrawGizmosSelected()
	{
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
				if ((x+1)%unbreakableObstacleMOD.x == 0 && (y+1) % unbreakableObstacleMOD.y == 0)
					Gizmos.color = Color.red;
                else
                {
					if(j < breakableObstacleCount)
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
