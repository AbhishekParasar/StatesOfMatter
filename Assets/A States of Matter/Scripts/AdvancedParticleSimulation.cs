using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class AdvancedParticleSimulation : MonoBehaviour
{
    public enum State { Solid, Liquid, Gas }
    [Header("Simulation State")]
    public State currentState;
   [SerializeField] private State oldState;
    [SerializeField]
    Molecules molecules;
    [Header("Particle Settings")]
    public GameObject particlePrefab;
    public int particleCount = 100;
    public Vector3 areaSize = new Vector3(5, 5, 5);

    [Header("State Colors")]
    public Color solidColor = Color.blue;
    public Color liquidColor = Color.green;
    public Color gasColor = Color.red;

    [Header("Transition Settings")]
    public float transitionSpeed = 2.0f;

    [Header("Temperature Settings")]
    public TMP_Text temperatureLabel, tempTemperatureLabel;

    [Header("Solid State Settings")]
    public float vibrationAmplitude = 0.1f; // Adjust this for vibration strength
    public float randomForceMagnitude = 1.0f;

    [Header("Liquid State Settings")]
    public float cohesionRadius = 1.5f; // Radius for particle attraction
    public float repulsionRadius = 0.5f; // Radius for particle repulsion
    public float cohesionForce = 2.0f;  // Strength of cohesion force
    public float repulsionForce = 5.0f; // Strength of repulsion force
    public float damping = 0.98f;       // Damping factor for velocity smoothing
    public float jitterMagnitude, jitterFrequency;       // Damping factor for velocity smoothing

    [Header("Gas State Settings")]
    public float gasRandomMotionMin = -5f;
    public float gasRandomMotionMax = 5f;

    private GameObject[] particles;
    public float temperature; // Current temperature value
    private Vector3[] latticePositions; // To store lattice positions for solid state
    private Vector3[] vibrationFrequencies; // Unique frequencies for each particle
    private Vector3[] vibrationOffsets;    // Unique phase offsets for each particle

    [Header("Glass Material Settings")]
    public Material glassMaterial; // Base material for the glass
    public GameObject containerMesh;
    public Material stoveMaterial; // Base material for the glass
    public GameObject stoveMesh;
    public TemperatureManager temperatureManager;
    // Texture maps for different states
    public Texture coldAlbedo;  // Albedo map for the cold state
    public Texture neutralAlbedo; // Albedo map for the neutral state
    public Texture hotAlbedo;  // Albedo map for the hot state

    public Texture coldNormal; // Normal map for the cold state
    public Texture neutralNormal; // Normal map for the neutral state
    public Texture hotNormal; // Normal map for the hot state

    // Emission colors for different states
    public Color coldEmissionColor = new Color(0.2f, 0.6f, 1f); // Bluish emission for cold
    public Color neutralEmissionColor = Color.clear; // No emission for neutral
    public Color hotEmissionColor = new Color(1f, 0.4f, 0.2f); // Reddish emission for hot

    public float colortransitionSpeed = 2.0f, gasdtatespeed; // Speed of transitioning between states
    [Header("Argon Specific Settings")]
    public float argonMeltingPoint = -185f; // °C
    public float argonBoilingPoint = -184f; // °C
    public MasterGameAudioManager masterGameAudioManager;
    public AudioClip solidAudio, liquidAudio, gasAudio;
    public GameObject DecreaseTempPopup;
    private bool isSolidAudioPlayed, isLiquidAudioPlayed, isGasAudioPlayed,isMinusAudioPlayed;
    private bool isSolidTAudioPlayed, isLiquidTAudioPlayed, isGasAudioTPlayed;

    [Header("Buttons")]
    public GameObject solid, liquid, gas,cameraLiquid;


    public AudioClip gasToLiquidAudio;
    public AudioClip liquidToSolidAudio;
    public CameraSwitcher cameraSwitch;

    [SerializeField] bool isSolidStateDone;
    [SerializeField] bool isGasStateDone;
    [SerializeField] bool isLiquidStateDone;
    [SerializeField] Image tickImageState;

    UnityEngine.SceneManagement.Scene scene;
    [SerializeField] Button nextSceneButton;
    void Start()
    {
        solid.SetActive(true);
        liquid.SetActive(false);
        gas.SetActive(false);

        tempTemperatureLabel.gameObject.transform.localScale = Vector3.zero;

        temperatureLabel.gameObject.transform.localScale = Vector3.zero;

        scene = SceneManager.GetActiveScene();
        //cameraSwitch = FindObjectOfType<CameraSwitcher>();
         masterGameAudioManager = FindObjectOfType<MasterGameAudioManager>();

        // Initialize the states
        oldState = currentState; // Start with the initial state
        TransitionToAudio(currentState); // Play initial state audio
        // Initialize particles and lattice positions
        particles = new GameObject[particleCount];
        latticePositions = new Vector3[particleCount];
        vibrationFrequencies = new Vector3[particleCount];
        vibrationOffsets = new Vector3[particleCount];

        CreateLatticeStructure(); // Generate lattice positions

        for (int i = 0; i < particleCount; i++)
        {
            particles[i] = Instantiate(particlePrefab, latticePositions[i], Quaternion.identity);
            particles[i].GetComponent<Renderer>().material.color = solidColor; // Default to solid

            vibrationFrequencies[i] = new Vector3(
                Random.Range(2f, 5f),
                Random.Range(2f, 5f),
                Random.Range(2f, 5f)
            );

            vibrationOffsets[i] = new Vector3(
                Random.Range(0f, Mathf.PI * 2),
                Random.Range(0f, Mathf.PI * 2),
                Random.Range(0f, Mathf.PI * 2)
            );
        }
       
        UpdateStateByTemperature(); // Set initial state
        UpdateButtons(); // Set initial button visibility
        SetToSolid(); // Initialize the solid state
    }
    public void TransitionToAudio(State newState)
    {
        // Handle specific transitions
        if (oldState == State.Gas && newState == State.Liquid)
        {
            if (!isGasAudioTPlayed)
            {
                Debug.Log("Gas to Liquid audio");
                masterGameAudioManager.PlayAudio(gasToLiquidAudio);
                isGasAudioTPlayed = true;
            }
           
        }
        else if (oldState == State.Liquid && newState == State.Solid)
        {
            if(!isSolidTAudioPlayed)
            {
                Debug.Log("Liquid to Solid audio");
                masterGameAudioManager.PlayAudio(liquidToSolidAudio);
                isSolidTAudioPlayed = true;
            }
        }
        //else if (oldState == State.Solid && newState == State.Solid)
        //{
        //    if (!isSolidTAudioPlayed)
        //    {
        //        Debug.Log("Liquid to Solid audio");
        //        masterGameAudioManager.PlayAudio(solidAudio);
        //        isSolidTAudioPlayed = true;
        //    }
        //}
        // Play audio for the new state if transitioning
        switch (newState)
        {
            case State.Solid:
             //   cameraSwitch.ToggleActiveState();
                if (!isSolidAudioPlayed)
                {
                    masterGameAudioManager.PlayAudio(solidAudio);
                    isSolidAudioPlayed = true;
                    isLiquidAudioPlayed = false;
                    isGasAudioPlayed = false;
                    isMinusAudioPlayed = false;
                }
                break;

            case State.Liquid:
              //  cameraSwitch.ToggleActiveState();
                if (!isLiquidAudioPlayed)
                {
                    masterGameAudioManager.PlayAudio(liquidAudio);
                    isLiquidAudioPlayed = true;
                    isSolidAudioPlayed = false;
                    isGasAudioPlayed = false;
                    isMinusAudioPlayed = false;
                }
                break;

            case State.Gas:
               // cameraSwitch.ToggleActiveState();
                if (!isGasAudioPlayed)
                {
                    masterGameAudioManager.PlayAudio(gasAudio);
                    isGasAudioPlayed = true;
                    isSolidAudioPlayed = false;
                    isLiquidAudioPlayed = false;
                }
                break;
        }
    }

    void Update()
    {
        tempTemperatureLabel.text = $"{-Mathf.Round(temperature * 10f) / 10f}°C";
        temperatureLabel.text = $"{Mathf.Round(temperature * 10f) / 10f}°C";


        //if (temperature == -185f)
        //{

        //    if (!gas.activeSelf)
        //    {
        //        tempTemperatureLabel.gameObject.transform.localScale = Vector3.zero;

        //        temperatureLabel.gameObject.transform.localScale = Vector3.one;
        //    }
        //    else
        //    {
        //        tempTemperatureLabel.gameObject.transform.localScale = Vector3.one;

        //        temperatureLabel.gameObject.transform.localScale = Vector3.zero;
        //    }
           
        //}
        //else
        //{
        //        tempTemperatureLabel.gameObject.transform.localScale = Vector3.zero;

        //        temperatureLabel.gameObject.transform.localScale = Vector3.one;

        //}




        if (oldState != currentState)
        {
            TransitionToAudio(currentState);
            oldState = currentState; // Update oldState after transition

        }

        // Update state based on temperature
        UpdateStateByTemperature();

                

        // Simulate behavior for the current state
        switch (currentState)
        {
            case State.Solid:
                SimulateSolid();
               // cameraLiquid.SetActive(false);
                break;
            case State.Liquid:
                SimulateLiquid();
               

                //cameraLiquid.SetActive(true);
                break;
            case State.Gas:
                SimulateGas();
                
               // cameraLiquid.SetActive(false);
                break;
        }

        // Update container mesh materials
        UpdateContainerMeshMaterials();
    }


    public void UpdateStateByTemperature()
    {
        if (temperature < argonMeltingPoint) // Below melting point -> Solid
        {
            currentState = State.Solid;
            isSolidStateDone = true;
           
            if (!isSolidAudioPlayed)
            {
               
                masterGameAudioManager.PlayAudio(solidAudio);
                isSolidAudioPlayed = true;

            }
        }
        else if (temperature >= (argonMeltingPoint) && temperature < argonBoilingPoint) // Between melting and boiling point -> Liquid
        {
            currentState = State.Liquid;
            isLiquidStateDone = true;
            
            if (!isLiquidAudioPlayed)
            {
                
                masterGameAudioManager.PlayAudio(liquidAudio);
                isLiquidAudioPlayed = true;
                temperatureManager.SetLiquidMode();
            }
        }
        else if(temperature >= argonBoilingPoint)// Above boiling point -> Gas
        {
            isGasStateDone = true;
            currentState = State.Gas;

            if (!isGasAudioPlayed)
            {
                masterGameAudioManager.PlayAudio(gasAudio);
                isGasAudioPlayed = true;
                temperatureManager.SetGasMode();
            }
            else
            {
                if (!masterGameAudioManager.audioSource.isPlaying && !isMinusAudioPlayed)
                {
                    isMinusAudioPlayed = true;
                    DecreaseTempPopup.SetActive(true);
                }
            }
        }

        UpdateButtons(); // Update button visibility
    }



    void SimulateSolid()
    {
        for (int i = 0; i < particles.Length; i++)
        {
            GameObject particle = particles[i];
            Rigidbody rb = particle.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.useGravity = true;
                rb.linearVelocity = Vector3.zero; // Stop motion

                // Generate random jitter
                Vector3 jitter = new Vector3(
                    Random.Range(-randomForceMagnitude, randomForceMagnitude),
                    Random.Range(-randomForceMagnitude, randomForceMagnitude),
                    Random.Range(-randomForceMagnitude, randomForceMagnitude)
                );
                Vector3 targetPosition = latticePositions[i] + jitter;

                // Calculate the distance between the particle and its target position
                float distance = Vector3.Distance(particle.transform.position, targetPosition);

                // Adjust transition speed if particles are close
                transitionSpeed = distance < 0.1f ? 3.7f : transitionSpeed;

                // Move towards the target position
                particle.transform.position = Vector3.Lerp(
                    particle.transform.position,
                    targetPosition,
                    Time.deltaTime * transitionSpeed
                );
            }
        }
    }


   void SimulateLiquid()
{
        transitionSpeed = .5f;
    if (containerMesh == null) return; // Ensure container mesh exists

    // Get the half-height limit based on the container mesh's height
    float halfHeight = containerMesh.transform.position.y + (containerMesh.transform.localScale.y / 2) / 2;

    foreach (GameObject particle in particles)
    {
        Rigidbody rb = particle.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.useGravity = true;

            // Enhanced jitter using sinusoidal motion
            Vector3 jitter = new Vector3(
                Mathf.Sin(Time.time * jitterFrequency + particle.GetInstanceID()) * jitterMagnitude,
                Mathf.Cos(Time.time * jitterFrequency + particle.GetInstanceID()) * jitterMagnitude,
                Mathf.Sin(Time.time * jitterFrequency * 0.5f + particle.GetInstanceID()) * jitterMagnitude
            );

            rb.AddForce(jitter, ForceMode.VelocityChange);

            // Cohesion and repulsion forces for sliding behavior
            foreach (GameObject otherParticle in particles)
            {
                if (particle == otherParticle) continue;

                float distance = Vector3.Distance(particle.transform.position, otherParticle.transform.position);
                Vector3 direction = (otherParticle.transform.position - particle.transform.position).normalized;

                if (distance < cohesionRadius)
                {
                    float normalizedDistance = Mathf.InverseLerp(repulsionRadius, cohesionRadius, distance);
                    float forceStrength = Mathf.Lerp(repulsionForce, cohesionForce, normalizedDistance);
                    rb.AddForce(direction * forceStrength);
                }
            }

            // Apply damping to smooth motion
            rb.linearVelocity *= damping;

            // Constrain particles to stay below half the container's height
            if (particle.transform.position.y > halfHeight)
            {
                particle.transform.position = new Vector3(
                    particle.transform.position.x,
                    halfHeight,
                    particle.transform.position.z
                );

                // Reset vertical velocity if it exceeds the limit
                rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
            }
        }
    }
}


    public string stateOfMalecule;




    void SimulateGas()
    {
       
        foreach (GameObject particle in particles)
        {
            Rigidbody rb = particle.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.useGravity = false;

                // Generate a random force
                if (rb.linearVelocity.magnitude < Mathf.Abs(gasRandomMotionMin))
                {
                    rb.linearVelocity = new Vector3(
                        Random.Range(gasRandomMotionMin, gasRandomMotionMax),
                        Random.Range(gasRandomMotionMin, gasRandomMotionMax),
                        Random.Range(gasRandomMotionMin, gasRandomMotionMax)
                    );
                    float kineticEnergy = Mathf.Lerp(1f, 5f, Mathf.InverseLerp(argonMeltingPoint, argonBoilingPoint, temperature));
                    rb.linearVelocity = rb.linearVelocity.normalized * kineticEnergy*gasdtatespeed;

                }

            }
        }
    }

    private IEnumerator TransitionToGas()
    {
        foreach (GameObject particle in particles)
        {
            Rigidbody rb = particle.GetComponent<Rigidbody>();
            if (rb != null)
            {
                Vector3 randomVelocity = new Vector3(
                    Random.Range(gasRandomMotionMin, gasRandomMotionMax),
                    Random.Range(gasRandomMotionMin, gasRandomMotionMax),
                    Random.Range(gasRandomMotionMin, gasRandomMotionMax)
                );

                float transitionDuration = 2.0f; // Adjust transition speed
                float elapsedTime = 0;

                while (elapsedTime < transitionDuration)
                {
                    rb.linearVelocity = Vector3.Lerp(rb.linearVelocity, randomVelocity, elapsedTime / transitionDuration);
                    elapsedTime += Time.deltaTime;
                    yield return null;
                }
            }
        }
    }

    public void CreateLatticeStructure()
    {
        int gridSize = Mathf.CeilToInt(Mathf.Pow(particleCount, 1f / 3f));
        float spacingX = areaSize.x / gridSize;
        float spacingY = areaSize.y / gridSize;
        float spacingZ = areaSize.z / gridSize;

        int index = 0;

        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
            {
                for (int z = 0; z < gridSize; z++)
                {
                    if (index >= particleCount) break;

                    latticePositions[index] = transform.position + new Vector3(
                        x * spacingX - areaSize.x / 2 + spacingX / 2,
                        y * spacingY - areaSize.y / 2 + spacingY / 2,
                        z * spacingZ - areaSize.z / 2 + spacingZ / 2
                    );
                    index++;
                }
            }
        }
    }
    void UpdateContainerMeshMaterials()
    {
        // Determine the transition value based on the temperature
        float transitionValue = 0f;

        if (temperatureManager.isDecreasing)
        {
            transitionValue = -15f; // Fully cold state
        }
        else if (temperatureManager.isIncreasing)
        {
            transitionValue = 15f; // Fully hot state
        }
        else
        {
            // Interpolate transition value for intermediate state
            transitionValue = 0; // Normalize temperature between 20�C and 80�C
        }

        // Check if the container mesh and its renderer are available
        if (containerMesh != null)
        {
            Renderer renderer = containerMesh.GetComponent<Renderer>();
            if (renderer != null)
            {
                Material material = renderer.material;

                // Smoothly update the Transition property in the shader
                material.SetFloat("_Transition", Mathf.Lerp(material.GetFloat("_Transition"), transitionValue, Time.deltaTime * colortransitionSpeed));
            }
        }
        if (stoveMesh != null)
        {
            Renderer renderer = stoveMesh.GetComponent<Renderer>();
            if (renderer != null)
            {
                Material material = renderer.material;

                // Smoothly update the Transition property in the shader
                material.SetFloat("_Transition", Mathf.Lerp(material.GetFloat("_Transition"), transitionValue, Time.deltaTime * colortransitionSpeed));
            }
        }
    }
    public void SetToSolid()
    {
        // Set the state to Solid
        temperature = -189.3f;
        currentState = State.Solid;

        // Update particle properties to reflect the Solid state and reposition them
        for (int i = 0; i < particles.Length; i++)
        {
            GameObject particle = particles[i];

            // Set the particle's color to solidColor
            Renderer renderer = particle.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material.color = solidColor;
            }

            // Update Rigidbody settings for solid state
            Rigidbody rb = particle.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.useGravity = true;
                rb.linearVelocity = Vector3.zero; // Stop any existing motion
            }

            // Smoothly move the particle to its lattice position
            StartCoroutine(MoveToPosition(particle.transform, latticePositions[i]));
        }
    }

    private IEnumerator MoveToPosition(Transform particleTransform, Vector3 targetPosition)
    {
        float transitionDuration = 2.0f; // Duration of the transition
        float elapsedTime = 0;

        Vector3 startingPosition = particleTransform.position;

        while (elapsedTime < transitionDuration)
        {
            particleTransform.position = Vector3.Lerp(startingPosition, targetPosition, elapsedTime / transitionDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        particleTransform.position = targetPosition; // Ensure the final position is set
    }



    void UpdateParticleColors()
    {
        Color targetColor = solidColor;
        switch (currentState)
        {
            case State.Liquid:
                targetColor = liquidColor;
                break;
            case State.Gas:
                targetColor = gasColor;
                break;
        }

        foreach (GameObject particle in particles)
        {
            Renderer renderer = particle.GetComponent<Renderer>();
            renderer.material.color = Color.Lerp(renderer.material.color, targetColor, Time.deltaTime * colortransitionSpeed);
        }
    }

    public void SetTransitionSpeed()
    {

        transitionSpeed = 3.7f;

    }
    public void SetTransitionSpeed2()
    {

        transitionSpeed = 0.5f;

    }
    void UpdateButtons()
    {
        if (temperatureManager.isIncreasing || temperatureManager.isDecreasing)
        {
            solid.SetActive(currentState == State.Solid);
            liquid.SetActive(currentState == State.Liquid);
            gas.SetActive(currentState == State.Gas);
        }

        if(isGasStateDone && isSolidStateDone == true)
        {
            StateStatus.Instance.SetMoleculeState(molecules);
            if(!tickImageState.gameObject.activeSelf)
                tickImageState.gameObject.SetActive(true);

            if(scene.name == "Argon")
            {
                nextSceneButton.interactable = true;
            }
        }
    }
}
