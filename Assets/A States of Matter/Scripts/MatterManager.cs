using System.Collections.Generic;
using UnityEngine;

public class MatterManager : MonoBehaviour
{

    public enum Molecules { Argon, Nitrogen, Oxygen, Water }
    public GameObject moleculePrefab;
    public GameObject containerMesh;
    public int maxMolecules = 100;
    public float spacing = 2f;
    public float vibrationIntensity = 0.1f;
    public float transitionDuration = 2f;
    public float solidToLiquidThreshold = 50f;
    public float liquidToGasThreshold = 100f;
    public float temperature = 0;
    public float maxConnectionDistance = 1.5f;
    public float cohesionForce = 2f;
    public float randomForceMagnitude = 2f;
    public float drag = 1f;
    public Color solidColor = Color.blue;
    public Color liquidColor = Color.green;
    public Color gasColor = Color.red;

    private float transitionProgress = 0f;
    private bool transitioning = false;
    private MatterState targetState;
    private List<Vector3> targetPositions = new List<Vector3>();

    private List<GameObject> molecules = new List<GameObject>();
    private Bounds containerBounds;
    private List<Vector3> initialPositions = new List<Vector3>();
    private List<Vector3> vibrationDirections = new List<Vector3>();
    private Dictionary<GameObject, Rigidbody> moleculeRigidbodies = new Dictionary<GameObject, Rigidbody>();

    private enum MatterState { Solid, Liquid, Gas }
    private MatterState currentState = MatterState.Solid;

    void Start()
    {
        containerBounds = containerMesh.GetComponent<MeshCollider>().bounds;
        InitializeMolecules();
        ArrangeMoleculesInGrid();
        UpdateMoleculeColors(solidColor);
    }

    void Update()
    {
        if (transitioning)
        {
            transitionProgress += Time.deltaTime / transitionDuration;
            if (transitionProgress >= 1f)
            {
                transitionProgress = 1f;
                transitioning = false;
                currentState = targetState;
               // ApplyFinalStateBehavior();
            }
            HandleStateTransitionSmoothly();
        }
        else
        {
            HandleStateTransition();
            ApplyStateBehavior();
        }
    }

    void InitializeMolecules()
    {
        for (int i = 0; i < maxMolecules; i++)
        {
            GameObject newMolecule = Instantiate(moleculePrefab, transform);
            molecules.Add(newMolecule);

            Rigidbody rb = newMolecule.GetComponent<Rigidbody>() ?? newMolecule.AddComponent<Rigidbody>();
            rb.useGravity = false;
            rb.linearDamping = drag;
            moleculeRigidbodies[newMolecule] = rb;

            Vector3 randomPos = new Vector3(
                Random.Range(containerBounds.min.x, containerBounds.max.x),
                Random.Range(containerBounds.min.y, containerBounds.max.y),
                Random.Range(containerBounds.min.z, containerBounds.max.z)
            );

            newMolecule.transform.localPosition = randomPos;
            initialPositions.Add(randomPos);
            vibrationDirections.Add(Random.insideUnitSphere.normalized);
        }
    }

    void HandleStateTransition()
    {
        if (temperature < solidToLiquidThreshold && currentState != MatterState.Solid)
        {
            SetState(MatterState.Solid);
        }
        else if (temperature >= solidToLiquidThreshold && temperature < liquidToGasThreshold && currentState != MatterState.Liquid)
        {
            SetState(MatterState.Liquid);
        }
        else if (temperature >= liquidToGasThreshold && currentState != MatterState.Gas)
        {
            SetState(MatterState.Gas);
        }
    }

    void HandleStateTransitionSmoothly()
    {
        for (int i = 0; i < molecules.Count; i++)
        {
            Transform molecule = molecules[i].transform;
            molecule.position = Vector3.Lerp(initialPositions[i], targetPositions[i], transitionProgress);
        }
    }

    void SetState(MatterState newState)
    {
        if (currentState == newState || transitioning) return;

        targetState = newState;
        transitioning = true;
        transitionProgress = 0f;

        if (newState == MatterState.Liquid)
        {
            PrecomputeLiquidTargetPositions();
            UpdateMoleculeColors(liquidColor);
        }
        else if (newState == MatterState.Gas)
        {
            UpdateMoleculeColors(gasColor);
        }
        else if (newState == MatterState.Solid)
        {
            UpdateMoleculeColors(solidColor);
        }
    }

    void ApplyStateBehavior()
    {
        if (currentState == MatterState.Solid)
        {
            VibrateMolecules();
        }
        else if (currentState == MatterState.Liquid)
        {
            ApplyLiquidBehavior();
        }
        else if (currentState == MatterState.Gas)
        {
            MoveGasMolecules();
        }
    }

    void VibrateMolecules()
    {
        for (int i = 0; i < molecules.Count; i++)
        {
            Transform molecule = molecules[i].transform;
            molecule.localPosition = initialPositions[i] + vibrationDirections[i] * vibrationIntensity;
        }
    }

    void ApplyLiquidBehavior()
    {
        foreach (var molA in molecules)
        {
            Rigidbody rbA = moleculeRigidbodies[molA];

            foreach (var molB in molecules)
            {
                if (molA == molB) continue;

                float distance = Vector3.Distance(molA.transform.position, molB.transform.position);
                if (distance < maxConnectionDistance)
                {
                    Vector3 force = (molB.transform.position - molA.transform.position).normalized * cohesionForce;
                    rbA.AddForce(force);
                }
            }

            rbA.AddForce(Random.insideUnitSphere * randomForceMagnitude);
        }
    }

    void MoveGasMolecules()
    {
        foreach (var molecule in molecules)
        {
            Rigidbody rb = moleculeRigidbodies[molecule];
            rb.AddForce(Random.insideUnitSphere * randomForceMagnitude);
        }
    }

    void UpdateMoleculeColors(Color color)
    {
        foreach (var molecule in molecules)
        {
            Renderer renderer = molecule.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material.color = color;
            }
        }
    }

    void PrecomputeLiquidTargetPositions()
    {
        targetPositions.Clear();

        for (int i = 0; i < molecules.Count; i++)
        {
            Vector3 randomPos;
            int attempts = 0;

            do
            {
                randomPos = new Vector3(
                    Random.Range(containerBounds.min.x + spacing, containerBounds.max.x - spacing),
                    Random.Range(containerBounds.min.y + spacing, containerBounds.max.y - spacing),
                    Random.Range(containerBounds.min.z + spacing, containerBounds.max.z - spacing)
                );
                attempts++;
            } while (IsTooCloseToOtherTargets(randomPos) && attempts < 100);

            targetPositions.Add(randomPos);
        }
    }

    bool IsTooCloseToOtherTargets(Vector3 position)
    {
        foreach (var target in targetPositions)
        {
            if (Vector3.Distance(position, target) < spacing) return true;
        }
        return false;
    }

    void ArrangeMoleculesInGrid()
    {
        int gridSize = Mathf.CeilToInt(Mathf.Pow(molecules.Count, 1f / 3f));
        int index = 0;

        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
            {
                for (int z = 0; z < gridSize; z++)
                {
                    if (index >= molecules.Count) return;

                    Vector3 position = new Vector3(x, y, z) * spacing;
                    molecules[index].transform.localPosition = position;
                    initialPositions[index] = position;
                    index++;
                }
            }
        }
    }
}
