using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingGoalEnvironment : MonoBehaviour
{
    public Transform[] GoalLocations;
    public Transform[] StartingLocations;
	public GameObject[] StaticWalls;

	// Start is called before the first frame update
	public void Reset()
	{
		if(GoalLocations.Length > 0)
		{
			goalIndex = Random.Range(0, GoalLocations.Length);
		}

		if (StartingLocations.Length > 0)
		{
			startIndex = Random.Range(0, StartingLocations.Length);
			
		}

		foreach(var wall in StaticWalls)
		{
			wall.SetActive(Random.value < 0.5f);
		}
	}

	public Vector3 GetGoalPosition()
	{
		return GoalLocations[goalIndex].position;
	}

	public Vector3 GetStartPosition()
	{
		return StartingLocations[startIndex].position;
	}

	private int goalIndex = 0;
	private int startIndex = 0;
}
