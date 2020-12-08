// ------------------------------------------------------------------------------
// Author: Myles Cardiff, myles@mylescardiff.com
// Created: 9/18/2020
// ------------------------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PcgUniverse2
{
    /// <summary>
    /// Overall manager for the game. This must be loaded for some parts of each scene
    /// to function properly
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        public enum ESceneIndex
        {
            kPersist,
            kTitle,
            kLoading,
			kInstructions,
            kGalaxy,
            kSolarSystem,
            kPlanet,
			kNone
        }

        // three seeds for Galaxy, System, Planet
        [SerializeField] private int m_seed = 32381;
        public int seed { get => m_seed; }

        private int m_lastPlanetSeed = 0;
        public int lastPlanetSeed { get => m_lastPlanetSeed; set => m_lastPlanetSeed = value; }

		private PlanetSettings m_lastPlanetSettings = null;
		public PlanetSettings lastPlanetSettings { get => m_lastPlanetSettings; set => m_lastPlanetSettings = value; }

        private int m_lastSystemSeed = 0;
        public int lastSystemSeed { get => m_lastSystemSeed; set => m_lastSystemSeed = value; }

        private float m_lastOrbitDist = 0f;
        public float lastOrbitDistance { get => m_lastOrbitDist; set => m_lastOrbitDist = value; }

        private StarType m_lastStarType = null;
        public StarType lastStarType { get => m_lastStarType; set => m_lastStarType = value; }

		[SerializeField] private int m_fuel = 500;
		public int fuel { get => m_fuel; set => m_fuel = value; }

		[SerializeField] private int m_crew = 10000;
		public int crew { get => m_crew; set => m_crew = value; }

        public static GameManager s_instance;
        private Scene m_currentScene;

		[Header("Galaxy Setup")]
		[SerializeField] private int m_numberOfArms = 4;
		[SerializeField] private int m_sectorsPerArm = 5;
		[SerializeField] private int m_starsPerSector = 10;
		[SerializeField] private float m_turnDistance = 2f;
		[SerializeField] private float m_sectorRadius = 2f;
		[SerializeField] private float m_scale = 1f;

		[Header("Known Systems")]
        [SerializeField] private List<SystemSettings> m_knownSystems;
        public List<SystemSettings> knownSystems { get => m_knownSystems; }

        private Dictionary<int, SystemSettings> m_knownSystemDictionary = null;
        public Dictionary<int, SystemSettings> knownSystemDict { get => m_knownSystemDictionary; }

        // two data structures are maintained here for ease of use in different cases
        private List<GalaxySector> m_sectors = null;
        public List<GalaxySector> sectors { get => m_sectors; set => m_sectors = value; }

        private StarGraph m_starGraph = null;
        public StarGraph starGraph { get => m_starGraph; set => m_starGraph = value; }

		private LayerMask m_galaxyStarLayer;
		public LayerMask galaxyStarLayer { get => m_galaxyStarLayer; }

		private StarNode m_centerStarNode = null;
		public StarNode centerStarNode { get => m_centerStarNode; }

		/// <summary>
		/// Set the instance
		/// </summary>
		private void Start()
        {
            DontDestroyOnLoad(this);

            m_seed = Random.Range(1, int.MaxValue);
            s_instance = this;
			m_galaxyStarLayer = LayerMask.NameToLayer("GalaxyStars");
			m_knownSystemDictionary = new Dictionary<int, SystemSettings>();

			SpawnGalaxy();

			LoadScene(ESceneIndex.kTitle, false);
        }

        private void OnEnable()
        {
            s_instance = this;
        }

		/// <summary>
		/// Generates the galaxy
		/// </summary>
		public void SpawnGalaxy()
		{

			Randomizer randomizer = GameObject.FindObjectOfType<RandomizerComponent>().m_randomizer;
			if (randomizer == null)
			{
				Debug.LogError("There needs to be a a RandomizerComponent loaded into memory to generate anything.");
				return;
			}

			Noise noiseGen = new Noise(m_seed);

			// The rotation angle, in radians, between the arms of the spiral
			float armSpacing = (2 * Mathf.PI) / m_numberOfArms;

			// create structures for sectors and stars
			starGraph = new StarGraph();
			m_sectors = new List<GalaxySector>();


			// create each "arm"
			for (int j = 0; j < m_numberOfArms; j++)
			{

				float r = 0; // this is from the math formula... i have no idea what it represents
				float theta = 0;
				float armRotation = j * armSpacing;

				// Loop thru the particles for this arm and place them
				for (int i = 0; i < m_sectorsPerArm; i++)
				{
					// spiral 
					r = m_turnDistance * theta;

					// set the intial position
					Vector3 sectorCenter = transform.position;

					// get cartisean coordinates, and rotate about the center
					sectorCenter.x = sectorCenter.x + r * Mathf.Cos(theta);
					sectorCenter.z = sectorCenter.z + r * Mathf.Sin(theta);
					float rotatedX = sectorCenter.x * Mathf.Cos(armRotation) + sectorCenter.z * Mathf.Sin(armRotation);
					float rotatedZ = -sectorCenter.x * Mathf.Sin(armRotation) + sectorCenter.z * Mathf.Cos(armRotation);
					sectorCenter.x = rotatedX;
					sectorCenter.z = rotatedZ;

					sectorCenter = transform.TransformPoint(sectorCenter) * m_scale;

					if (sectorCenter != Vector3.zero)
					{
						GalaxySector sector = new GalaxySector(i, sectorCenter);

						// Spawn a star at the middle of this sector
						StarNode centerStarNode = SpawnStar(sector, randomizer, noiseGen, Vector3.zero, 0f);

						if (starGraph.rootNode == null)
						{
							starGraph.rootNode = centerStarNode;
							m_centerStarNode = centerStarNode;
						}


						// create the stars
						for (int starIndex = 0; starIndex < m_starsPerSector; starIndex++)
						{
							float randomDistance = Random.Range(0f, m_sectorRadius * 3f);
							Vector3 randomVector = new Vector3(
								Random.Range(-1f, 1f),
								Random.Range(-1f, 1f),
								Random.Range(-1f, 1f)
								);

							// create a star and link it to this sector's center node
							StarNode starNode = SpawnStar(sector, randomizer, noiseGen, randomVector, randomDistance);

							starGraph.LinkNodes(centerStarNode, starNode);

						}

						m_sectors.Add(sector);

					}

					// move to the next position on the arm
					theta += m_sectorRadius;
				}
			}
		}

		public StarNode SpawnStar(GalaxySector sector, Randomizer randomizer, Noise noiseGen, Vector3 randomVector, float randomDistance)
		{
			
			Vector3 starPosition = sector.center + (randomVector * randomDistance);

			GalaxyStar star = new GalaxyStar();
			star.position = starPosition;

			// chance to spawn real star system TODO: create roll for this
			if (knownSystemDict.Count < knownSystems.Count)
			{
				// pull already created system. only do this if we haven't used all the systems
				int systemIndex = (knownSystems.Count - knownSystemDict.Count) - 1;
				knownSystemDict.Add(knownSystems[systemIndex].m_specialSeed, knownSystems[systemIndex]);
				star.seed = knownSystems[systemIndex].m_specialSeed;
				star.name = knownSystems[systemIndex].name;
				star.starType = knownSystems[systemIndex].m_starType;

			}
			else
			{
				// generate a random star
				int systemSeed = (int)Mathf.Abs(noiseGen.Evaluate(starPosition) * (float)int.MaxValue);
				Random.InitState(systemSeed);
				star.seed = systemSeed;
				star.GenerateName();
				int starTypeIndex = Utility.WeightedRandom(randomizer.m_starTypes);
				star.starType = randomizer.m_starTypes[starTypeIndex];

			}

			// set the star colors
			
			sector.stars.Add(star);
			return starGraph.AddNode(star);

		}


		/// <summary>
		/// Get the pre-determined instance of this object
		/// </summary>
		/// <returns>GameManager object </returns>
		public static GameManager GetInstance() { return s_instance; }

        public void LoadScene(ESceneIndex index, bool unloadCurrent = false)
        {
            SceneManager.LoadScene((int)ESceneIndex.kLoading, LoadSceneMode.Additive);
            LoadSceneInternal(index, unloadCurrent);
        }

        /// <summary>
        /// Load any scene based on it's index
        /// </summary>
        /// <param name="sceneIndex"></param>
        private void LoadSceneInternal(ESceneIndex sceneIndex, bool unloadCurrent = false)
        {
            // m_currentSceneIndex = sceneIndex;
            StartCoroutine(LoadAsynchronously((int)sceneIndex, unloadCurrent));
        }

        /// <summary>
        /// Coroutine for async loading of the scene
        /// </summary>
        /// <param name="sceneIndex"></param>
        /// <returns></returns>
        IEnumerator LoadAsynchronously(int sceneIndex, bool unloadCurrent = false)
        {
            if (m_currentScene != null && unloadCurrent)
            {
                var unloadOp = SceneManager.UnloadSceneAsync(m_currentScene);
                while (!unloadOp.isDone)
                {
                    // Debug.Log(unloadOp.progress);
                    yield return null;
                }

            }

            AsyncOperation loadOp = SceneManager.LoadSceneAsync(sceneIndex, LoadSceneMode.Additive);

            while (!loadOp.isDone)
            {
                // Debug.Log(loadOp.progress);
                yield return null;
            }

            m_currentScene = SceneManager.GetSceneByBuildIndex(sceneIndex);
            SceneManager.SetActiveScene(m_currentScene);

            SceneManager.UnloadSceneAsync((int)ESceneIndex.kLoading);


        }


    }

}
