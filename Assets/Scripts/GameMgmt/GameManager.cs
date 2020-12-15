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
			kWin,
			kLose,
			kNone
        }

		/// <summary>
		/// The overall galaxy seed
		/// </summary>
        [SerializeField] private int m_seed = 32381;
        public int seed { get => m_seed; }

		/// <summary>
		/// the last planet seed that was generated. this is used to move between scenes and remember where we are
		/// </summary>
        private int m_lastPlanetSeed = 0;
        public int lastPlanetSeed { get => m_lastPlanetSeed; set => m_lastPlanetSeed = value; }

		/// <summary>
		/// the last planet setting that was generated, for reference
		/// </summary>
		private PlanetSettings m_lastPlanetSettings = null;
		public PlanetSettings lastPlanetSettings { get => m_lastPlanetSettings; set => m_lastPlanetSettings = value; }

		/// <summary>
		/// the last system setting that was generated, for reference
		/// </summary>
        private int m_lastSystemSeed = 0;
        public int lastSystemSeed { get => m_lastSystemSeed; set => m_lastSystemSeed = value; }

		/// <summary>
		/// The last star traveled to, for reference
		/// </summary>
		private GalaxyStar m_lastStarVisited = null;
		public GalaxyStar lastStarVisited { get => m_lastStarVisited; set => m_lastStarVisited = value; }

		/// <summary>
		/// The last orbit distance, for reference
		/// </summary>
        private float m_lastOrbitDist = 0f;
        public float lastOrbitDistance { get => m_lastOrbitDist; set => m_lastOrbitDist = value; }

		/// <summary>
		///  The last star type generated, for reference
		/// </summary>
        private StarType m_lastStarType = null;
        public StarType lastStarType { get => m_lastStarType; set => m_lastStarType = value; }

        public static GameManager s_instance;
        private Scene m_currentScene;
		private Dictionary<int, Planet> m_minedPlanets;

		[Header("Galaxy Setup")]
		[SerializeField] private int m_numberOfArms = 4;
		[SerializeField] private int m_sectorsPerArm = 5;
		[SerializeField] private int m_starsPerSector = 10;
		[SerializeField] private float m_turnDistance = 2f;
		[SerializeField] private float m_sectorRadius = 2f;
		[SerializeField] private float m_scale = 1f;

		[Header("Gameplay Content")]
        [SerializeField] private List<SystemSettings> m_knownSystems;
        public List<SystemSettings> knownSystems { get => m_knownSystems; }

		[SerializeField] private List<GameplayEvent> m_gameplayEvents = null;
		[SerializeField] private float m_chanceOfEvent = 0.1f;
		[SerializeField] private float m_fuelToNextStar = 50;
		[SerializeField] private float m_foodExpendedPerTurn = 10;
		[SerializeField, Tooltip("The minimum amount of resources that should be found on a planet when mined")]
		private int m_minResourcesFound = 25;
		public int minResoucesFound { get => m_minResourcesFound; }
		[SerializeField, Tooltip("The maximum amount of resources that should be found on a planet when mined")]
		private int m_maxResourcesFound = 100;
		public int maxResourcesFound { get => m_maxResourcesFound; }

		[SerializeField] private float m_fuel = 500;
		public float fuel
		{
			get => m_fuel;
			set
			{
				m_fuel = value;
				UpdateUI();
				CheckEndGameConditions();
				
			}
		}

		[SerializeField] private float m_food = 500;
		public float food
		{
			get => m_food;
			set
			{
				m_food = value;
				UpdateUI();
				CheckEndGameConditions();
			}
		}

		private int m_gamePlayEventIndex = 0;
        private int m_numTurnsBeforeEvents = 5;

		/// <summary>
		/// Turns are counted by each planet visited
		/// </summary>
		private int m_turn = 0;
		public int turn
		{
			get => m_turn;
			set
			{
				m_turn = value;
				CheckEndGameConditions();
			}
		}
		/// <summary>
		/// Known systems are preprogrammed planetary systems, e.g. Sol system
		/// </summary>
		private Dictionary<int, SystemSettings> m_knownSystemDictionary = null;
        public Dictionary<int, SystemSettings> knownSystemDict { get => m_knownSystemDictionary; }

		/// <summary>
		/// Sectors are arranged along the galazy arms, used to idenfify locall accessible groups of stars
		/// </summary>
        private List<GalaxySector> m_sectors = null;
        public List<GalaxySector> sectors { get => m_sectors; set => m_sectors = value; }

		/// <summary>
		/// Star graph links all stars together, staring with the center starts for each sector as the backbone
		/// </summary>
        private StarGraph m_starGraph = null;
        public StarGraph starGraph { get => m_starGraph; set => m_starGraph = value; }

		/// <summary>
		/// Identifies the clickale layer the stars exsit on.
		/// </summary>
		private LayerMask m_galaxyStarLayer;
		public LayerMask galaxyStarLayer { get => m_galaxyStarLayer; }

		/// <summary>
		/// The star at the center of the galaxy, used for forming the graph
		/// </summary>
		private StarNode m_centerStarNode = null;
		public StarNode centerStarNode { get => m_centerStarNode; }

		private StarNode m_exitStar = null;
		public StarNode exitStar { get => m_exitStar; }

		bool m_gameOver = false;

		/// <summary>
		/// Set the instance
		/// </summary>
		private void Awake()
        {
            DontDestroyOnLoad(this);

            m_seed = Random.Range(1, int.MaxValue);
            s_instance = this;
			m_galaxyStarLayer = LayerMask.NameToLayer("GalaxyStars");
			m_knownSystemDictionary = new Dictionary<int, SystemSettings>();
			m_minedPlanets = new Dictionary<int, Planet>();
			Utility.ShuffleList(m_gameplayEvents);

			SpawnGalaxy();

			m_lastStarVisited = centerStarNode.star;

			LoadScene(ESceneIndex.kTitle, false);

        }

		/// <summary>
		/// Decrement food count, this is a race against the clock.
		/// </summary>
        private void Update()
        {
			food -= Time.deltaTime * m_foodExpendedPerTurn;
        }

        /// <summary>
        /// Sets the singleton instance
        /// </summary>
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

				StarNode lastCenterNode = null;

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
						StarNode thisCenterStarNode = SpawnStar(sector, randomizer, noiseGen, Vector3.zero, 0f);
						sector.centerStar = thisCenterStarNode.star;
						
						if (starGraph.rootNode == null)
						{
							starGraph.rootNode = thisCenterStarNode;
							m_centerStarNode = thisCenterStarNode;
						}

						if (lastCenterNode != null)
                        {
							starGraph.LinkNodes(thisCenterStarNode, lastCenterNode);
                        }
						lastCenterNode = thisCenterStarNode;

						// create the stars
						for (int starIndex = 0; starIndex < m_starsPerSector; starIndex++)
						{
							float randomDistance = Random.Range(0.5f, m_sectorRadius * 3f);
							Vector3 randomVector = new Vector3(
								Random.Range(-1f, 1f),
								Random.Range(-1f, 1f),
								Random.Range(-1f, 1f)
								);

							// create a star and link it to this sector's center node
							StarNode starNode = SpawnStar(sector, randomizer, noiseGen, randomVector, randomDistance);

							starGraph.LinkNodes(thisCenterStarNode, starNode);
						}

						m_sectors.Add(sector);
					}

					// move to the next position on the arm
					theta += m_sectorRadius;
				}


			}

			// set the immediate stars in the vicinity of Sol discovered
			m_centerStarNode.star.discovered = true;
			foreach (StarNode starNode in m_centerStarNode.m_linkedNodes)
            {
				starNode.star.discovered = true;
            }

			m_exitStar = m_sectors[m_sectors.Count - 1].centerStar.node;

		}

		/// <summary>
		/// Create a star in the specified sector
		/// </summary>
		/// <param name="sector"></param>
		/// <param name="randomizer"></param>
		/// <param name="noiseGen"></param>
		/// <param name="randomVector"></param>
		/// <param name="randomDistance"></param>
		/// <returns></returns>
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

		/// <summary>
		/// Loads a scene by index. Note that this needs the persistent scene to be active, so it won't
		/// work without that. 
		/// </summary>
		/// <param name="index"></param>
		/// <param name="unloadCurrent"></param>
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

		/// <summary>
		/// Set a planet seed to mined, so that it can't be double mined.
		/// </summary>
		/// <param name="planet"></param>
		private void AddMinedPlanet(Planet planet)
        {
			m_minedPlanets.Add(planet.seed, planet);
        }

		/// <summary>
		/// Checks if a planet is mined already 
		/// </summary>
		/// <param name="planet"></param>
		/// <returns></returns>
		private bool IsPlanetMined(Planet planet)
        {
			return m_minedPlanets.ContainsKey(planet.seed);
        }

		/// <summary>
		/// Mines a planet for a random amount of food
		/// </summary>
		/// <returns></returns>
		public int AddFood(Planet planet)
        {
			if (IsPlanetMined(planet))
				return 0;

			int amount = Random.Range(m_minResourcesFound, m_maxResourcesFound);
			m_food += amount;

			UpdateUI();
			AddMinedPlanet(planet);

			return amount;
        }

		/// <summary>
		/// Mines a planet for a random amount of fuel
		/// </summary>
		/// <returns></returns>
		public int AddFuel(Planet planet)
		{
			if (IsPlanetMined(planet))
				return 0;

			int amount = Random.Range(m_minResourcesFound, m_maxResourcesFound);
			m_fuel += amount;
			
			UpdateUI();
			AddMinedPlanet(planet);

			return amount;
		}

		/// <summary>
		/// Find the active UI and update the numbers
		/// </summary>
		public void UpdateUI()
        {
			MainUI activeUIObject = FindObjectOfType<MainUI>();
			if (activeUIObject != null)
				activeUIObject.UpdateUI(m_food, m_fuel);
        }

		/// <summary>
		/// Pulls an event out of the list to trigger
		/// </summary>
		/// <returns></returns>
		public GameplayEvent TriggerEvent()
        {
			// only happens by chance...
			if (Random.Range(0f, 1f) > m_chanceOfEvent)
				return null;

			// only happens after a certain # of turns
			if (m_turn < m_numTurnsBeforeEvents)
				return null;

			// only happnens if deisngers have implemented events. 
			if (m_gamePlayEventIndex >= m_gameplayEvents.Count)
				return null;

			GameplayEvent triggeredEvent = m_gameplayEvents[m_gamePlayEventIndex];

			triggeredEvent.Trigger(this);

			++m_gamePlayEventIndex;

			return triggeredEvent;
        }
		
		/// <summary>
		/// Spends the fuel necssary to fly to another star. 
		/// </summary>
		public void FlyToSystem()
        {
			m_fuel -= m_fuelToNextStar;
        }

		/// <summary>
		/// Checks conditions for a win or loss
		/// </summary>
		public void CheckEndGameConditions()
        {
			if (m_lastStarVisited == m_exitStar.star)
				Win();

			if (m_food <= 0 || m_fuel <= 0)
				Lose();
        }

		/// <summary>
		/// Do stuff when you win the game
		/// </summary>
		public void Win()
        {
			if (!m_gameOver)
            {
				m_gameOver = true;
				LoadScene(ESceneIndex.kWin, true);
            }
        }

		/// <summary>
		/// Do stuff when you lose the game
		/// </summary>
		public void Lose()
        {
			if (!m_gameOver)
			{
				m_gameOver = true;
				LoadScene(ESceneIndex.kLose, true);
			}
			
        }

	}

}
