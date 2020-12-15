// ------------------------------------------------------------------------------
// Author: Myles Cardiff
// Created: 6/14/2020

// Disclaimer: Some of the structure of this code was written following Sebastian 
// Lague's  tutorials for both plane generation and terrain generattion. 
// https://www.patreon.com/SebastianLague
// https://www.youtube.com/watch?v=lctXaT9pxA0
// -------------------------------------------------------------------------------

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace PcgUniverse2
{
    /// <summary>
    /// Represents a single planet, with 6-sided ico-sphere mesh faces
    /// </summary>
    public class Planet : MonoBehaviour
    {

        /// <summary>
        /// Planet type setting enum
        /// </summary>
        public enum EPlanetType
        {
            kRocky,
            kGasGiant,
            kAsteroid
        }

        #region Tunables

        [Header("Planet Settings")]
        [SerializeField]
        private int m_seed = 12345;
        public int seed { get => m_seed; set => m_seed = value; }
        public string planetName { get => m_planetSettings.planetName; }

        /// <summary>
        /// Distance from the orbiting star. This value is used to determine temperature and surface water
        /// </summary>
        [SerializeField]
        private float m_orbitDistance = 0f;
        public float orbitDistance { get => m_orbitDistance; set => m_orbitDistance = value; }

        /// <summary>
        /// Mesh detail level. Represents one dimesion (x,y) of each of the 6 sides of the generated
        /// planet. The maximum number Unity allows is 255 x 255, abouyt 65k verrices. 
        /// </summary>
        [SerializeField, Range(2, 255), Tooltip("How detailed is the generated surface mesh")]
        private int m_resolution = 50;
        public int resolution { get => m_resolution; set => m_resolution = value; }

        /// <summary>
        /// This is the scriptable object field to hold all planet-level details, excluding external
        /// factors like distance from the star
        /// </summary>
        [Expandable, SerializeField]
        private PlanetSettings m_planetSettings;
        public PlanetSettings planetSettings { get => m_planetSettings; set => m_planetSettings = value; }

        [Header("UI")]
        [SerializeField] private Text m_planetNameText = null;
        [SerializeField] private Text m_geoReadout = null;
        [SerializeField] private Text m_atmosReadout = null;
        [SerializeField] private Text m_lifeSignText = null;
        [SerializeField] private Text m_oceanText = null;
        [SerializeField] private Camera m_camera = null;
        [SerializeField] private float m_cameraAcceleration = 100f;
        [SerializeField] private float m_approachSpeed = 200f;
        [SerializeField] private float m_approachModifier = 3f;

        [SerializeField] private Material m_planetMaterial = null;
        [SerializeField] private Material m_thinAtmoMaterial = null;
        [SerializeField] private Material m_thickAtmoMaterial = null;
        [SerializeField] private GameObject m_cloudSphere = null;

        // hidden serialized fields
        [SerializeField, HideInInspector]
        private MeshFilter[] m_meshFilters;
        [SerializeField, HideInInspector]
        private TerrainFace[] m_terrainFaces;
        [SerializeField, HideInInspector]
        private Texture2D m_texture;

        #endregion

        #region Private Non-Serialized Data

        private MainUI m_mainUi = null;
        private MinMax m_elevationMinMax;
        private Color m_primaryAtmoColor;
        private Color m_secondaryAtmoColor;
        private float m_cameraVelocity = 0f;
        private float m_minCameraDistance = 30f;
        private bool m_onApproach = true;
        private float m_approachDistance = 300f;
        private GameManager m_gameManager = null;
        private Randomizer m_randomizer = null;

        #endregion

        /// <summary>
        /// Loads teh game manager and other global objects needed for generation later
        /// </summary>
        private void Awake()
        {
            m_randomizer = GameObject.FindObjectOfType<RandomizerComponent>().m_randomizer;
            m_gameManager = GameManager.GetInstance();

            m_gameManager.UpdateUI();

            if (m_seed == 0 && m_gameManager != null)
            {
                m_seed = m_gameManager.lastPlanetSeed;
                m_planetSettings = m_gameManager.lastPlanetSettings;
                Random.InitState(m_seed);
                m_resolution = 255;

                if (!m_gameManager.knownSystemDict.ContainsKey(m_gameManager.lastSystemSeed))
                {
                    // this is not a known systems, it needs to randomize still
                    GenerateBasics(m_seed, m_gameManager.lastOrbitDistance, m_gameManager.lastStarType);
                    GenerateDetail();
                }
                else
                {
                    // this is a known system, just set the atmo color based on the settings
                    m_secondaryAtmoColor = m_planetSettings.atmoColor;
                    m_primaryAtmoColor = Color.white;
                }

                GeneratePlanet();
            }

            if (m_planetSettings != null)
                m_minCameraDistance = m_planetSettings.radius * 2f;

            // approach 
            if (m_planetSettings != null)
                m_approachDistance = m_planetSettings.radius * m_approachModifier;

            // get UI elements
            m_mainUi = GameObject.FindObjectOfType<MainUI>();

            GameplayEvent triggeredEvent = m_gameManager.TriggerEvent();
            if (triggeredEvent)
                m_mainUi.ShowMessage(triggeredEvent.m_description);

        }

        /// <summary>
        /// Randomize the seed, this is not always used if you want to just regen without randomizing
        /// </summary>
        public void RandomizeSeed()
        {
            m_seed = Random.Range(0, int.MaxValue);
        }

        /// <summary>
        /// Generates only the basic stuff, like name, size and type
        /// </summary>
        /// <param name="seed"></param>
        /// <param name="orbitDistance"></param>
        /// <param name="starType"></param>
        public void GenerateBasics(int seed = 0, float orbitDistance = 0f, StarType starType = null)
        {
            if (seed != 0)
                m_seed = seed;

            m_orbitDistance = orbitDistance;


            if (m_randomizer == null)
            {
                Debug.LogError("There needs to be a a RandomizerComponent loaded into memory to generate anything.");
                return;
            }

            if (m_planetMaterial == null)
                m_planetMaterial = new Material(Shader.Find("Shader Graphs/PlanetShader"));

            float habitableZoneStart = 0;
            float habitableZoneEnd = 50;
            if (starType != null)
            {
                habitableZoneStart = starType.HabitableZoneStart();
                habitableZoneEnd = starType.HabitableZoneEnd();
            }

            // seed the rng with this planet's seed
            Random.InitState(m_seed);

            // create the planet settings object
            if (m_planetSettings == null)
                m_planetSettings = ScriptableObject.CreateInstance("PlanetSettings") as PlanetSettings;

            m_planetSettings.m_noiseLayers = new NoiseSettings[2];

            m_planetSettings.m_noiseLayers[0] = ScriptableObject.CreateInstance("NoiseSettings") as NoiseSettings;
            m_planetSettings.m_noiseLayers[1] = ScriptableObject.CreateInstance("NoiseSettings") as NoiseSettings;

            // basics only needed for solar system; start with name...
            NameGenerator nameGen = new NameGenerator();
            m_planetSettings.planetName = nameGen.Generate(m_seed);

            // ...type
            bool isRockyPlanet = m_orbitDistance < habitableZoneEnd || Random.Range(0f, 1f) < 0.4f;
            m_planetSettings.planetType = isRockyPlanet ? EPlanetType.kRocky : EPlanetType.kGasGiant;

            m_planetSettings.hasLiquidOcean = false;

            if (m_planetSettings.m_biomeNoiseSettings == null)
                m_planetSettings.m_biomeNoiseSettings = ScriptableObject.CreateInstance("NoiseSettings") as NoiseSettings;

            int numBiomes;
            if (m_planetSettings.planetType == EPlanetType.kRocky)
            {
                // ocean and life
                m_planetSettings.hasLiquidOcean = m_orbitDistance >= habitableZoneStart && m_orbitDistance < habitableZoneEnd;

                m_planetSettings.radius = Random.Range(m_randomizer.m_rockyMinSize, m_randomizer.m_rockyMaxSize);

                m_planetSettings.m_noiseLayers[0].Randomize(m_randomizer.m_rockyLandmassMinNoise, m_randomizer.m_rockyLandmassMaxNoise);
                m_planetSettings.m_noiseLayers[1].Randomize(m_randomizer.m_rockyMountiansMinNoise, m_randomizer.m_rockyMountiansMaxNoise);

                if (m_planetSettings.hasLiquidOcean)
                {
                    float lifeRoll = Random.Range(0f, 1f);
                    m_planetSettings.hasLife = lifeRoll < m_randomizer.m_chanceOfLife;
                }
                else
                {
                    m_planetSettings.hasLife = false;
                    m_planetSettings.m_noiseLayers[0].minValue = m_randomizer.m_nonOceanFloorLevel;
                }

                m_planetSettings.m_biomeNoiseSettings.strength = Random.Range(0.3f, 0.6f);
                m_planetSettings.m_biomeNoiseStrength = Random.Range(0.3f, 0.6f);

                numBiomes = Random.Range(3, 5);
            }
            else
            {
                m_planetSettings.hasLife = false;
                m_planetSettings.hasLiquidOcean = false;

                m_planetSettings.radius = Random.Range(m_randomizer.m_gasGiantMinSize, m_randomizer.m_gasGiantMaxSize);

                // we don't need any landmass or noise settings for gas giants, just biomes, and generally more of them
                m_planetSettings.m_noiseLayers[0].m_enabled = false;
                m_planetSettings.m_noiseLayers[1].m_enabled = false;

                // gas layers
                m_planetSettings.m_biomeNoiseStrength = Random.Range(m_randomizer.m_gasGiantBiomeNoiseStrengthMin, m_randomizer.m_gasGiantBiomeNoiseStrengthMax);
                m_planetSettings.m_biomeNoiseSettings.strength = Random.Range(0.1f, 0.4f);

                numBiomes = Random.Range(1, 20);
            }

            // biome color randomization
            m_planetSettings.m_biomes = new Biome[numBiomes];

        }

        /// <summary>
        /// Generate the detail of the view that only the planetary view needs
        /// </summary>
        public void GenerateDetail()
        {
            // done here because only this loads when they really get to a planet
            m_gameManager.turn++;

            Randomizer randomizer = GameObject.FindObjectOfType<RandomizerComponent>().m_randomizer;
            if (randomizer == null)
            {
                Debug.LogError("There needs to be a a RandomizerComponent loaded into memory to generate anything.");
                return;
            }

            // biome noise 
            if (m_planetSettings.m_biomeNoiseSettings == null)
            {
                m_planetSettings.m_biomeNoiseSettings = ScriptableObject.CreateInstance("NoiseSettings") as NoiseSettings;
            }

            // biome colors
            for (int i = 0; i < m_planetSettings.m_biomes.Length; i++)
            {
                m_planetSettings.m_biomes[i] = new Biome();
                m_planetSettings.m_biomes[i].m_gradient = new Gradient();

                int numColors = 5;
                GradientColorKey[] colorKeys = new GradientColorKey[numColors];
                GradientAlphaKey[] alphaKeys = new GradientAlphaKey[numColors];

                int colorIndex = 0;
                if (m_planetSettings.hasLiquidOcean && m_planetSettings.planetType == EPlanetType.kRocky)
                {
                    // set blue ocean
                    colorKeys[colorIndex].color = randomizer.m_oceanColors.Evaluate(Random.Range(0f, 1f));
                    colorKeys[colorIndex].time = 0.01f;
                    alphaKeys[colorIndex].alpha = 1f;
                    ++colorIndex;

                    ElementColorDatabase.Element randElement = randomizer.m_elementColorDb.GetRandomElement(ElementColorDatabase.EElementType.kSolid);

                    colorKeys[colorIndex].color = randElement.m_color;
                    colorKeys[colorIndex].time = 0.02f;
                    alphaKeys[colorIndex].alpha = 1f;

                    m_planetSettings.AddGeoElement(randElement, 1f);

                    ++colorIndex;
                }

                if (m_planetSettings.hasLife)
                {
                    // set green vegetation
                    colorKeys[colorIndex].color = randomizer.m_vegetationColors.Evaluate(Random.Range(0f, 1f));
                    colorKeys[colorIndex].time = 0.30f;
                    alphaKeys[colorIndex].alpha = 1f;
                    ++colorIndex;

                    colorKeys[colorIndex].color = randomizer.m_vegetationColors.Evaluate(Random.Range(0f, 1f));
                    colorKeys[colorIndex].time = 0.60f;
                    alphaKeys[colorIndex].alpha = 1f;
                    ++colorIndex;

                }

                for (int c = colorIndex; c < numColors; c++)
                {
                    ElementColorDatabase.Element randElement;
                    if (m_planetSettings.planetType == EPlanetType.kRocky)
                    {
                        randElement = randomizer.m_elementColorDb.GetRandomElement(ElementColorDatabase.EElementType.kSolid);
                    }
                    else
                    {
                        randElement = randomizer.m_elementColorDb.GetRandomElement(ElementColorDatabase.EElementType.kGas);
                    }
                    colorKeys[c].color = randElement.m_color;
                    colorKeys[c].time = (float)c / (float)numColors; // (float)colorIndex + 1f / (float)c;
                    alphaKeys[c].alpha = 1f;
                    m_planetSettings.AddGeoElement(randElement, 1f);
                }

                m_planetSettings.m_biomes[i].startHeight = (float)i / (float)m_planetSettings.m_biomes.Length;
                m_planetSettings.m_biomes[i].m_gradient.SetKeys(colorKeys, alphaKeys);
            }

            // atmosphere
            if (m_planetSettings.planetType == EPlanetType.kGasGiant)
            {
                m_planetSettings.atmosElements = m_planetSettings.geoElements;
                m_planetSettings.geoElements = new Dictionary<ElementColorDatabase.Element, float>();
            }
            else
            {
                int numAtmosElements = Random.Range(0, 12);


                for (int i = 0; i < numAtmosElements; i++)
                {

                    ElementColorDatabase.Element randomGas = randomizer.m_elementColorDb.GetRandomElement(ElementColorDatabase.EElementType.kGas);
                    m_planetSettings.AddAtmosElement(randomGas, 1f);



                    // set colors to pass to the shader later
                    if (m_primaryAtmoColor.r == 0f && m_primaryAtmoColor.g == 0f)
                        m_primaryAtmoColor = randomGas.m_color;
                    else if (m_secondaryAtmoColor.r == 0f && m_secondaryAtmoColor.g == 0f)
                        m_secondaryAtmoColor = randomGas.m_color;

                }

                m_planetSettings.thickAtmosphere = Random.Range(0f, 1f) < randomizer.m_chanceOfThickAtmos;
                m_planetSettings.cloudThickness = Random.Range(randomizer.m_minCloudThickness, randomizer.m_maxCloudThickness);



            }




        }

        /// <summary>
        /// Initialize the mesh faces
        /// </summary>
        public void Init()
        {
            // don't reallocate every time if we already have an array
            if (m_meshFilters == null || m_meshFilters.Length == 0)
            {
                // six sides of the cube
                m_meshFilters = new MeshFilter[6];
            }
            m_terrainFaces = new TerrainFace[6];


            Vector3[] directions = { Vector3.up, Vector3.down, Vector3.left, Vector3.right, Vector3.forward, Vector3.back };

            for (int i = 0; i < m_meshFilters.Length; i++)
            {
                // TODO: This is so it doesn't regenerate, but we may want that 
                // for LOD later. 
                if (m_meshFilters[i] == null)
                {
                    GameObject meshObject = new GameObject("mesh");
                    meshObject.transform.parent = transform;
                    meshObject.AddComponent<MeshRenderer>();

                    m_meshFilters[i] = meshObject.AddComponent<MeshFilter>();

                    m_meshFilters[i].sharedMesh = new Mesh();
                }
                m_meshFilters[i].GetComponent<MeshRenderer>().sharedMaterial = m_planetMaterial;

                m_terrainFaces[i] = new TerrainFace(this, m_meshFilters[i].sharedMesh, m_resolution, directions[i]);
            }
        }

        /// <summary>
        ///  Builds the planet based on the current land mass and biome settings. This
        ///  function will create those things if they aren't defined. 
        /// </summary>
        public void GeneratePlanet()
        {
            if (m_planetMaterial == null)
                m_planetMaterial = new Material(Shader.Find("Shader Graphs/PlanetShader"));

            if (m_planetSettings.m_biomes == null || m_planetSettings.m_biomes.Length == 0)
            {
                m_planetSettings.m_biomes = new Biome[1];
                m_planetSettings.m_biomes[0] = new Biome();
            }

            if (m_planetSettings.m_noiseLayers == null || m_planetSettings.m_noiseLayers.Length == 0)
            {
                m_planetSettings.m_noiseLayers = new NoiseSettings[1];
                m_planetSettings.m_noiseLayers[0] = ScriptableObject.CreateInstance("NoiseSettings") as NoiseSettings;
            }

            if (m_planetSettings.m_biomeNoiseSettings == null)
                m_planetSettings.m_biomeNoiseSettings = ScriptableObject.CreateInstance("NoiseSettings") as NoiseSettings;

            if (m_planetNameText != null)
                m_planetNameText.text = m_planetSettings.planetName;

            Init();
            UpdateMesh();

            // output geo elements
            if (m_geoReadout != null && m_planetSettings.geoElements != null)
            {
                m_geoReadout.text = "";
                foreach (var element in m_planetSettings.geoElements)
                    m_geoReadout.text += $"{element.Key.name} \n";
            }


            // output atmos elements 
            if (m_atmosReadout != null && m_planetSettings.atmosElements != null)
            {
                m_atmosReadout.text = "";
                foreach (var element in m_planetSettings.atmosElements)
                    m_atmosReadout.text += $"{element.Key.name} \n";
            }

            if (m_lifeSignText == null)
                return;

            m_lifeSignText.gameObject.SetActive(m_planetSettings.hasLife);
            m_oceanText.gameObject.SetActive(m_planetSettings.hasLiquidOcean);

        }

        /// <summary>
        /// If only shape related settings have been updated, this will update the mesh.
        /// </summary>
        private void UpdateMesh()
        {
            m_elevationMinMax = new MinMax();

            foreach (TerrainFace face in m_terrainFaces)
            {
                face.ConstructMesh();
            }

            UpdateElevation();
            UpdateColors();
        }

        /// <summary>
        /// If only the color settings have changed, this will update the 
        /// colors / shaders rendering the planet
        /// </summary>
        private void UpdateColors()
        {
            m_texture = new Texture2D(m_resolution, m_planetSettings.m_biomes.Length, TextureFormat.RGBA32, false);

            Color[] colors = new Color[m_texture.width * m_texture.height];

            int colorIndex = 0;
            foreach (Biome biome in m_planetSettings.m_biomes)
            {
                for (int i = 0; i < m_resolution; i++)
                {
                    colors[colorIndex] = biome.m_gradient.Evaluate(i / (m_resolution - 1f));
                    ++colorIndex;
                }
            }

            m_texture.SetPixels(colors);
            m_texture.Apply();
            m_planetMaterial.SetTexture("_texture", m_texture);

            foreach (TerrainFace face in m_terrainFaces)
            {
                face.UpdateUVs();
            }
        }

        /// <summary>
        /// Uses biome color settings and biome hights to get a color percentage
        /// </summary>
        /// <param name="pointOnUnitSphere"></param>
        /// <returns></returns>
        public float BiomePercentFromPoint(Vector3 pointOnUnitSphere)
        {
            float heightPercent = (pointOnUnitSphere.y + 1f) / 2f;

            // disturb the boundaries of the biomes a bit so they aren't straight lines
            heightPercent += (m_planetSettings.m_biomeNoiseSettings.Evaluate(pointOnUnitSphere, m_seed) - m_planetSettings.m_biomeNoiseOffset) * m_planetSettings.m_biomeNoiseStrength;
            float blendRange = m_planetSettings.m_biomeBlendAmount / 2f + 0.001f;
            float biomeIndex = 0; //TODO: don't like this "index" being a float
            for (int i = 0; i < m_planetSettings.m_biomes.Length; i++)
            {
                float distanceFromStart = heightPercent - m_planetSettings.m_biomes[i].startHeight;
                float blendWeight = Mathf.InverseLerp(-blendRange, blendRange, distanceFromStart);
                biomeIndex *= (1f - blendWeight);
                biomeIndex += i * blendWeight;
            }

            return biomeIndex / Mathf.Max(1, m_planetSettings.m_biomes.Length - 1);
        }

        /// <summary>
        /// Sets material properties before rendering, including atmosphere
        /// </summary>
        private void UpdateElevation()
        {
            m_planetMaterial.SetVector("_elevationMinMax", new Vector4(m_elevationMinMax.Min(), m_elevationMinMax.Max()));
            m_planetMaterial.SetInt("_hasOcean", m_planetSettings.hasLiquidOcean ? 1 : 0);

            if (m_planetSettings.planetType == EPlanetType.kRocky)
            {
                // set the cloud sphere size to the same plus a bit as the planet
                float atmosphereHeight = (m_elevationMinMax.Max() * 2) - ((m_elevationMinMax.Max() - m_elevationMinMax.Min()) / 4) + 1f;

                if (m_cloudSphere != null)
                {
                    m_cloudSphere.transform.localScale = new Vector3(atmosphereHeight, atmosphereHeight, atmosphereHeight);

                    Material atmosMat;
                    if (m_planetSettings.thickAtmosphere)
                    {
                        atmosMat = m_thickAtmoMaterial;
                        atmosMat.SetColor("_primaryColor", m_primaryAtmoColor);
                        atmosMat.SetColor("_secondaryColor", m_secondaryAtmoColor);
                    }
                    else
                    {
                        atmosMat = m_thinAtmoMaterial;
                        atmosMat.SetColor("_primaryColor", Color.white);
                        atmosMat.SetColor("_secondaryColor", m_secondaryAtmoColor);

                    }


                    atmosMat.SetFloat("_secondaryNoiseScale", m_planetSettings.cloudThickness);

                    m_cloudSphere.GetComponent<MeshRenderer>().material = atmosMat;
                }


            }
        }

        /// <summary>
        /// Generate a vertex with the proper height based on the planet's noise settings
        /// </summary>
        /// <param name="pointOnUnitSphere"></param>
        /// <returns></returns>
        public Vector3 CalculatePointOnPlanet(Vector3 pointOnUnitSphere)
        {
            float firstLayerValue = 0;
            float elevation = 0f;

            // this masks the secondary layers so that random mountains don't pop out of the ocean, e.g.
            if (m_planetSettings.m_noiseLayers.Length > 0)
            {
                firstLayerValue = m_planetSettings.m_noiseLayers[0].Evaluate(pointOnUnitSphere, m_seed);
                if (m_planetSettings.m_noiseLayers[0].m_enabled)
                {
                    elevation = firstLayerValue;
                }
            }

            // don't need to start at zero here becuase we've already evaluated
            for (int i = 1; i < m_planetSettings.m_noiseLayers.Length; i++)
            {
                if (!m_planetSettings.m_noiseLayers[i].m_enabled)
                    continue;

                // actually use the mask, if enabled; this is multiplicitave, so 1 does nothing here
                float mask = m_planetSettings.m_noiseLayers[i].m_useFirstLayerAsMask ? firstLayerValue : 1;
                elevation += m_planetSettings.m_noiseLayers[i].Evaluate(pointOnUnitSphere, m_seed + 1) * mask;
            }
            elevation = m_planetSettings.radius * (1f + elevation);
            m_elevationMinMax.Add(elevation); // track min and max for shader
            return pointOnUnitSphere * elevation;
        }

        /// <summary>
        /// Seralized this planet to disk to save later
        /// </summary>
        public void SavePlanetSettings()
        {
            m_planetSettings.SaveToDisk();
        }

        /// <summary>
        /// Leave and go back to the solar system view
        /// </summary>
        public void LeavePlanet()
        {
            GameManager.GetInstance().LoadScene(GameManager.ESceneIndex.kSolarSystem, true);
        }

        /// <summary>
        /// Update camera position
        /// </summary>
        private void Update()
        {
            if (m_camera == null)
                return;

            if (Input.mouseScrollDelta != Vector2.zero)
                m_cameraVelocity += m_cameraAcceleration * 10f * Input.mouseScrollDelta.y;

            float distanceToPlanet = Vector3.Distance(transform.position, m_camera.transform.position);
            if (distanceToPlanet < m_approachDistance)
                m_onApproach = false;

            if (m_onApproach)
                m_cameraVelocity = m_approachSpeed;

            Vector3 towardsObject = (transform.position - m_camera.transform.position).normalized;
            Vector3 movement = towardsObject * m_cameraVelocity * Time.deltaTime;
            Vector3 newPosition = m_camera.transform.position + movement;
            if (Vector3.Distance(transform.position, newPosition) > m_minCameraDistance)
                m_camera.transform.position += movement;

            if (m_cameraVelocity != 0)
            {
                m_cameraVelocity -= m_cameraAcceleration * (m_cameraVelocity / Mathf.Abs(m_cameraVelocity));
            }
        }

        /// <summary>
        /// Show or hide the cloud layer for a better view of the planet surface.
        /// </summary>
        public void ToggleAtmosphere()
        {
            Debug.Log("Atmosphere Toggled");
            m_cloudSphere.SetActive(!m_cloudSphere.activeSelf);
        }

        /// <summary>
        /// Consumes any resources from the planet
        /// </summary>
        public void MinePlanet()
        {
            Debug.Log("Planet Mined");

            int amount = 0;
            if (m_planetSettings.planetType == EPlanetType.kRocky)
                amount = m_gameManager.AddFood(this);
            else
                amount = m_gameManager.AddFuel(this);
            if (amount > 0)
            {
                m_mainUi.ShowMessage($"You have mined {amount} metric tons of resouces\n your supply has grown.");
            }
            else
            {
                m_mainUi.ShowMessage("This planet has already been stripped of collectable resources.");
            }

        
        }
    }

}

