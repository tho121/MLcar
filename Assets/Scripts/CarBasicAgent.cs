using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class CarBasicAgent : Agent
{
    // Start is called before the first frame update
    [SerializeField] private Transform targetTransform;
    [SerializeField] private Material winMat;
    [SerializeField] private Material loseMat;
    [SerializeField] private MeshRenderer floorMeshRenderer;
    [SerializeField] private MovingGoalEnvironment env;
    [SerializeField] private AIWheelDrive carScript;
    [SerializeField] private Rigidbody carRigidbody;

    private float stepPenalty = 1.0f;
    private float totalDistance = 1.0f;
    private float rewardBase = 10.0f;

    public override void OnEpisodeBegin()
    {
        env.Reset();

        carRigidbody.velocity = Vector3.zero;
        carRigidbody.angularVelocity = Vector3.zero;

        targetTransform.position = env.GetGoalPosition();

        transform.position = env.GetStartPosition();
        transform.localRotation = Quaternion.Euler(0.0f, Random.value * 180.0f, 0.0f);
        stepPenalty = rewardBase / (float)MaxStep;

        totalDistance = GetDistance();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(transform.localRotation);
        sensor.AddObservation(targetTransform.localPosition - transform.localPosition);

        sensor.AddObservation(carRigidbody.velocity);
        sensor.AddObservation(carRigidbody.angularVelocity);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        carScript.angleInput = actions.ContinuousActions[0];
        carScript.torqueInput = actions.ContinuousActions[1];
        carScript.isBraking = actions.DiscreteActions[0] == 1;

        //penalty per step (1.0 / max step) * distance (current distance / total distance)
        AddReward(-((stepPenalty * 0.1f) + (stepPenalty * GetDistance()/totalDistance * 0.9f) * 1.0f) );     
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continousActions = actionsOut.ContinuousActions;
        continousActions[0] = Input.GetAxisRaw("Horizontal");
        continousActions[1] = Input.GetAxisRaw("Vertical");

        ActionSegment<int> discreteActions = actionsOut.DiscreteActions;

        discreteActions[0] = Input.GetKey(KeyCode.X) ? 1 : 0;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Goal>(out Goal goal))
        {
            AddReward(rewardBase);
            floorMeshRenderer.material = winMat;
            EndEpisode();
        }

        if (other.TryGetComponent<Wall>(out Wall wall))
        {
            AddReward(- rewardBase * (Mathf.Min( GetDistance() / totalDistance, 1.0f )));
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
