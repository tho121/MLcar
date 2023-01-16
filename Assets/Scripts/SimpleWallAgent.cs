using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class SimpleWallAgent : Agent
{
    // Start is called before the first frame update
    [SerializeField] private Transform targetTransform;
    [SerializeField] private Material winMat;
    [SerializeField] private Material loseMat;
    [SerializeField] private MeshRenderer floorMeshRenderer;
    [SerializeField] private Transform startingLocation;

    private float stepPenalty = 1.0f;
    private float totalDistance = 1.0f;

    public override void OnEpisodeBegin()
    {
        transform.position = startingLocation.position;
        stepPenalty = 1.0f / (float)MaxStep;


        totalDistance = GetDistance();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(targetTransform.localPosition);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float moveX = actions.ContinuousActions[0];
        float moveZ = actions.ContinuousActions[1];

        float moveSpeed = 3.0f;
        transform.position += new Vector3(moveX, 0, moveZ) * Time.deltaTime * moveSpeed;

        //penalty per step (1.0 / max step) * distance (current distance / total distance)
        AddReward(-(stepPenalty * GetDistance()/totalDistance) * 2.0f);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continousActions = actionsOut.ContinuousActions;
        continousActions[0] = Input.GetAxisRaw("Horizontal");
        continousActions[1] = Input.GetAxisRaw("Vertical");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Goal>(out Goal goal))
        {
            AddReward(1.0f);
            floorMeshRenderer.material = winMat;
            EndEpisode();
        }

        if (other.TryGetComponent<Wall>(out Wall wall))
        {
            AddReward(-1.0f);
            floorMeshRenderer.material = loseMat;
            EndEpisode();
        }
    }

    private float GetDistance()
	{
        float x = targetTransform.localPosition.x - transform.localPosition.x;
        float z = targetTransform.localPosition.z - transform.localPosition.z;

        return Mathf.Sqrt(x * x + z * z);
    }
}
