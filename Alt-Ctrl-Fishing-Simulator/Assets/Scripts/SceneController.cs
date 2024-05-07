using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneController : MonoBehaviour
{
    public BoidController boidPrefab;

    public int spawnBoids = 100;
    public float boidSpeed = 10f;
    public float boidSteeringSpeed = 100f;
    public float boidNoClumpingArea = 10f;
    public float boidLocalArea = 10f;
    public float boidSimulationArea = 50f;

    private List<BoidController> _boids;

    [SerializeField] private GameObject hooked = null;

    public BobberManager bobber;
    public PlayerManager player;

    private void Start()
    {
        _boids = new List<BoidController>();

        for (int i = 0; i < spawnBoids; i++)
        {
            SpawnBoid(boidPrefab.gameObject, 0);
        }
    }

    private void Update()
    {
         foreach (BoidController boid in _boids)
        {
            if(boid.reeled)
            {
                hooked = boid.gameObject;
                GoToHook(boid);
                continue;
            }
            MoveClamp(boid);
        }
    }

    public void MoveClamp(BoidController boid)
    {
        boid.SimulateMovement(_boids);

        var boidPos = boid.transform.position;

        if (boidPos.x > boidSimulationArea)
            boidPos.x -= boidSimulationArea * 2;
        else if (boidPos.x < -boidSimulationArea)
            boidPos.x += boidSimulationArea * 2;

        if (boidPos.y > 0)
            boidPos.y = -1;
        else if (boidPos.y < -2)
            boidPos.y = -1;

        if (boidPos.z > boidSimulationArea + 15)
            boidPos.z -= boidSimulationArea * 2;
        else if (boidPos.z < -boidSimulationArea + 15)
            boidPos.z += boidSimulationArea * 2;

        boid.transform.position = boidPos;
    }

    private void GoToHook(BoidController boid)
    {
        if(boid.transform.position == new Vector3(0, 3, 0) || boid.transform.position == new Vector3(0, (float) 1, (float) 1.49))
        {
            boid.transform.position = new Vector3(0, (float) 1, (float) 1.49);
            hooked = null;
        }
        else
        {
            boid.transform.position = bobber.transform.position;
        }
    }

    private void SpawnBoid(GameObject prefab, int swarmIndex)
    {
        var boidInstance = Instantiate(prefab);

        boidInstance.transform.localPosition += new Vector3(Random.Range(-10, 10), Random.Range(-10, 10), Random.Range(-10, 10));
        boidInstance.transform.localRotation = Quaternion.Euler(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360));

        var boidController = boidInstance.GetComponent<BoidController>();
        boidController.SwarmIndex = swarmIndex;
        boidController.Speed = boidSpeed;
        boidController.SteeringSpeed = boidSteeringSpeed;
        boidController.LocalAreaRadius = boidLocalArea;
        boidController.NoClumpingRadius = boidNoClumpingArea;

        _boids.Add(boidController);
    }
}