using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace PcgUniverse2
{
    /// <summary>
    /// Generates and supports the Solar System view
    /// </summary>
    public class SolarSystem : MonoBehaviour
    {
        [Header("Solar System")]
        [SerializeField] private int m_seed = 0;
        [SerializeField] private string m_name = "Star";

        [SerializeField] StarType m_starType = null;
        [SerializeField] private int m_numberOfPlanets = 4;
        [SerializeField] private float m_minOrbitDistance = 200;
        [SerializeField] private float m_maxOrbitDistance = 300;
        [SerializeField] private int m_planetResolution = 100;
        [SerializeField] private float m_planetScale = 2f;
        [SerializeField] private ParticleSystem m_surfaceParticles = null;
        [SerializeField] private MeshRenderer m_coreMesh = null;
        [SerializeField] private Material m_planetMaterial = null;
        [SerializeField] private Material m_lineMaterial = null;
        [SerializeField] private Material m_habitableZomeMaterial = null;
        [SerializeField] private float m_lineWidth = 7f;
        [SerializeField] private int m_numLineSegments = 50;
        [SerializeField, HideInInspector] private List<Planet> m_planets = null;
        [SerializeField] private AnimationCurve m_orbitSpeedDropoff = null;
        [SerializeField] private float m_rotationSpeed = 1;

        [Header("Camera / UI")]
        [SerializeField] private float m_cameraAcceleration = 100f;
        [SerializeField] private CosmicBodyUI m_hud = null;
        [SerializeField] private Text m_nameLabel = null;
        [SerializeField] private Camera m_camera = null;

        // non-serialized props
        private Planet m_currentPlanet = null;
        private SystemSettings m_systemSettings = null;
        private float m_cameraVelocity = 0f;
        private GameManager m_gameManager = null;
        private Randomizer m_randomizer = null;
        private bool m_isUnloading = false;
        float m_starRadius = 100f;

        /// <summary>
        ///  Grams the camera and sets the current focus to the star. 
        /// </summary>
        private void Start()
        {
            m_gameManager = GameManager.GetInstance();
            if (m_gameManager != null)
            {
                m_seed = m_gameManager.lastSystemSeed;
                GeneratePlanets();
            }

            m_nameLabel.text = m_name;

        }

        /// <summary>
        /// Randomize the seed only. Does not generate planets, see GeneratePlanets() for that.
        /// </summary>
        public void Randomize()
        {
            m_seed = Random.Range(0, int.MaxValue);
        }

        /// <summary>
        /// Generate all planets for the system. Be sure to remove any already-generated 
        /// ones, as this will just add to the system
        /// </summary>
        public void GeneratePlanets()
        {
            m_randomizer = GameObject.FindObjectOfType<RandomizerComponent>().m_randomizer;
            if (m_randomizer == null)
            {
                Debug.LogError("There needs to be a a RandomizerComponent loaded into memory to generate anything.");
                return;
            }

            m_planets = new List<Planet>();

            if (m_seed == 0)
                Randomize();

            if (m_gameManager.knownSystemDict.ContainsKey(m_seed))
            {
                m_systemSettings = m_gameManager.knownSystemDict[m_seed];
            }

            if (m_systemSettings == null)
            {
                Random.InitState(m_seed);

                NameGenerator nameGen = new NameGenerator();
                Random.InitState(m_seed);
                m_name = nameGen.Generate(m_seed);

                int starTypeIndex = Utility.WeightedRandom(m_randomizer.m_starTypes);
                m_starType = m_randomizer.m_starTypes[starTypeIndex];

                // set the star colors
                m_starRadius = Random.Range(m_starType.m_minSize, m_starType.m_maxSize);
                m_coreMesh.material.SetColor("_BaseColor", m_starType.m_coreColor);
                m_coreMesh.transform.localScale = new Vector3(m_starRadius, m_starRadius, m_starRadius);

                m_numberOfPlanets = Random.Range(1, 10);
            }
            else
            {
                

                m_name = m_systemSettings.name;
                m_starRadius = m_systemSettings.m_starType.m_maxSize;
                m_coreMesh.material.SetColor("_BaseColor", m_systemSettings.m_starType.m_coreColor);
                m_coreMesh.transform.localScale = new Vector3(m_starRadius, m_starRadius, m_starRadius);

                m_numberOfPlanets = m_systemSettings.m_planetSettings.Count;
            }

            float actualRadius = m_coreMesh.bounds.extents.magnitude;
            // particle system changes
            ParticleSystem.ColorOverLifetimeModule colorOverTime = m_surfaceParticles.colorOverLifetime;
            colorOverTime.color = m_starType.m_surfaceColor;

            ParticleSystem.ShapeModule shapeMod = m_surfaceParticles.shape;
            shapeMod.radius = actualRadius / 2f;

            float lastOrbitDistance = 0f;
            for (int i = 0; i < m_numberOfPlanets; i++)
            {
                float randomOrbit = lastOrbitDistance + Random.Range(m_minOrbitDistance, m_maxOrbitDistance);
                if (m_gameManager != null)
                    m_gameManager.lastStarType = m_starType;

                // check if this seed is one of the special ones
                if (m_gameManager.knownSystemDict.ContainsKey(m_seed))
                {
                    PlanetSettings planetSettings = m_systemSettings.m_planetSettings[i];
                    CreatePlanet(m_seed + i, randomOrbit, planetSettings);
                }
                else
                {
                    CreatePlanet(m_seed + i, randomOrbit);
                }

                lastOrbitDistance = randomOrbit;
            }

            DrawCircle(m_starType.HabitableZoneStart(), m_habitableZomeMaterial);
            DrawCircle(m_starType.HabitableZoneEnd(), m_habitableZomeMaterial);


        }

        /// <summary>
        /// Creates a circle around the star using a line renderer
        /// </summary>
        /// <param name="radius"></param>
        /// <param name="material"></param>
        private void DrawCircle(float radius, Material material)
        {
            // draw goldilocks zone
            // draw a line
            GameObject lineGameObject = new GameObject("Line Container");
            lineGameObject.transform.SetParent(transform);

            LineRenderer lineRenderer = lineGameObject.AddComponent<LineRenderer>();
            lineRenderer.material = material;

            lineRenderer.numCornerVertices = m_numLineSegments + 1;
            Vector3[] linePositions = new Vector3[m_numLineSegments + 1];
            for (int i = 0; i < m_numLineSegments; i++)
            {
                var rad = Mathf.Deg2Rad * (i * 360f / m_numLineSegments);
                linePositions[i] = new Vector3(Mathf.Sin(rad) * radius, 0, Mathf.Cos(rad) * radius);
            }
            linePositions[m_numLineSegments] = linePositions[0]; // close circle
            lineRenderer.positionCount = m_numLineSegments;
            lineRenderer.SetPositions(linePositions);
            lineRenderer.alignment = LineAlignment.View;
            lineRenderer.startWidth = m_lineWidth;
            lineRenderer.endWidth = m_lineWidth;
        }

        /// <summary>
        /// Create planet with real details at a low LOD
        /// </summary>
        /// <param name="seed"></param>
        /// <param name="orbitDistance"></param>
        /// <returns></returns>
        private Planet CreatePlanet(int seed, float orbitDistance, PlanetSettings knownSettings = null)
        {
            GameObject planetGameObject = new GameObject("Planet " + seed);
            planetGameObject.transform.position = Vector3.zero;
            planetGameObject.transform.localScale *= 2f; // TODO: fix
            
            planetGameObject.AddComponent<OrbitingBody>().orbitSpeed = Random.Range(1f, 54f) * m_orbitSpeedDropoff.Evaluate(orbitDistance / m_maxOrbitDistance);
            planetGameObject.AddComponent<Spin>().speed = Random.Range(0f, 20f);

            Planet planetComponent = planetGameObject.AddComponent<Planet>();
            planetComponent.seed = seed;
            planetComponent.resolution = 40;

            if (knownSettings != null)
            {
                // use the known settings passed in (Earth, e.g.)
                planetComponent.planetSettings = knownSettings;
            }
            else
            {
                //  generate random surface details
                planetComponent.GenerateBasics(seed, orbitDistance, m_starType);
                planetComponent.GenerateDetail();
            }

            planetComponent.GeneratePlanet();


            // add collider for clicking 
            SphereCollider sphereColl = planetGameObject.AddComponent<SphereCollider>();
            sphereColl.radius = planetComponent.planetSettings.radius;

            // set position & Parent
            planetGameObject.transform.position = Vector3.right * orbitDistance;
            planetGameObject.transform.parent = transform.parent;
            planetGameObject.transform.RotateAround(Vector3.zero, Vector3.up, Random.Range(0f, 40f));

            DrawCircle(orbitDistance, m_lineMaterial);

            return planetComponent;
        }

     

      

        /// <summary>
        /// Generates a single planet and places in the solar system. This version only products a lit sphere
        /// </summary>
        /// <param name="seed"></param>
        /// <param name="orbitDistance"></param>
        /// <returns></returns>
        private Planet CreateSimplePlanet(int seed, float orbitDistance)
        {
            // create the planet game object
            GameObject planetGameObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            planetGameObject.name = "Planet " + seed;
            planetGameObject.AddComponent<Spin>().speed = Random.Range(0f, 20f);
            planetGameObject.AddComponent<OrbitingBody>().orbitSpeed = Random.Range(1f, 54f) * m_orbitSpeedDropoff.Evaluate(orbitDistance / m_maxOrbitDistance);
            planetGameObject.GetComponent<MeshRenderer>().material = m_planetMaterial;
          

            // seed and generate the planet object
            int planetSeed = Random.Range(0, int.MaxValue);
            Planet planetComponent = planetGameObject.AddComponent<Planet>();
            planetComponent.resolution = m_planetResolution;
            planetComponent.orbitDistance = orbitDistance;
            planetComponent.Init();
            planetComponent.GenerateBasics(planetSeed, orbitDistance, m_starType);


            // position the planet
            planetGameObject.transform.localScale *= planetComponent.planetSettings.radius * m_planetScale;
            planetGameObject.transform.position = Vector3.right * orbitDistance;
            planetGameObject.transform.RotateAround(Vector3.zero, Vector3.up, Random.Range(0f, 40f));
            planetGameObject.transform.parent = transform.parent;

            m_planets.Add(planetComponent);
          
            return planetComponent;
        }

        /// <summary>
        /// Moves the camera, checks for user input and sets UI elements
        /// </summary>
        private void Update()
        {
            if (Input.mouseScrollDelta != Vector2.zero)
                m_cameraVelocity += m_cameraAcceleration * 25f * Input.mouseScrollDelta.y;

            Vector3 towardsObject = (transform.position - m_camera.transform.position).normalized;
            m_camera.transform.position += towardsObject * m_cameraVelocity * Time.deltaTime;

            if (m_cameraVelocity != 0)
                m_cameraVelocity -= m_cameraAcceleration * 10 * (m_cameraVelocity / Mathf.Abs(m_cameraVelocity));
        
            // planet loaading
            if (Input.GetKeyDown(KeyCode.Return) && !m_isUnloading)
            {
                GameManager gameManager = GameManager.GetInstance();
                gameManager.lastPlanetSeed = m_currentPlanet.seed;
                gameManager.lastOrbitDistance = m_currentPlanet.orbitDistance;
                gameManager.lastPlanetSettings = m_currentPlanet.planetSettings;

                if (gameManager == null)
                    return;

                gameManager.LoadScene(GameManager.ESceneIndex.kPlanet, true);
                m_isUnloading = true;
            }

            if (Input.GetMouseButtonDown(0))
            {
                // clicked on something, check if it's a star
                var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit = new RaycastHit();

                if (Physics.Raycast(ray, out hit))
                {
                    GameObject hitObject = hit.collider.gameObject;
                    Planet planetObject = hitObject.GetComponent<Planet>();
                    if (planetObject != null)
                    {
                        // set the UI
                        m_hud.gameObject.SetActive(true);
                        m_nameLabel.text = planetObject.planetName;
                        m_hud.objectName = planetObject.planetName;
                        m_hud.transform.position = planetObject.transform.position;
                        m_currentPlanet = planetObject;
                        m_hud.transform.parent = m_currentPlanet.transform;
                    }
                }
            }

            // update orbit speed
            foreach (Planet planet in m_planets)
            {
                planet.GetComponent<OrbitingBody>().orbitSpeed *= m_rotationSpeed;
            }

        }

        /// <summary>
        /// Leave this systema and return to galaxy view
        /// </summary>
        public void LeaveSystem()
        {
            GameManager.GetInstance().LoadScene(GameManager.ESceneIndex.kGalaxy, true);
        }
    }

}
