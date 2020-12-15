// ------------------------------------------------------------------------------
// Author: Myles Cardiff, myles@mylescardiff.com
// Created: 9/18/2020
// Disclaimer: Got some help on the math of the spiral from here: http://wiki.unity3d.com/index.php/Particle_Spiral_Effect
// ------------------------------------------------------------------------------

using Cinemachine;
using UnityEngine;
using UnityEngine.UI;

namespace PcgUniverse2
{
	/// <summary>
	/// Generates a spiral galaxy with stars and a background particle effect 
	/// to make the "arms" ala the milky way
	/// </summary>
	public class Galaxy : MonoBehaviour
	{
		[Header("Galaxy")]
		[SerializeField] private Material m_starMaterial = null;
		[SerializeField] private Material m_graphLineMaterial = null;
		[SerializeField] private ParticleSystem m_particleSystem = null;
		[SerializeField] private CosmicBodyUI m_cosmicBodyUI = null;
		[SerializeField] private Transform m_positionIndicatorUI = null;
		[SerializeField] private CinemachineVirtualCamera m_camera;

		[Header("Camera")]
		[SerializeField] private Transform m_cameraFocus = null;
        [SerializeField] private Text m_nameLabel = null;

		private GalaxyStar m_lastClickedStar = null;
		private bool m_isUnloading = false;
		private GameManager m_gameManager = null;

		/// <summary>
		/// Sets the state and calls the galaxy generator
		/// </summary>
		private void Start()
		{
			m_gameManager = GameManager.GetInstance();
			m_gameManager.UpdateUI();

			Transform starParent = GameObject.FindGameObjectWithTag("Galaxy").transform;

			for (int i = 0; i < m_gameManager.sectors.Count; ++i)
            {
				GalaxySector sector = m_gameManager.sectors[i];

                // instantiate particle system and have the number and density fall off as we get further from the center
                ParticleSystem particleSystem = Instantiate(m_particleSystem, sector.center, Quaternion.identity);
                ParticleSystem.MainModule mainMod = particleSystem.GetComponent<ParticleSystem>().main;
                mainMod.maxParticles = 1000 - (i * 35);
                ParticleSystem.ShapeModule ps = particleSystem.GetComponent<ParticleSystem>().shape;
                ps.radius = 3f - ((float)i * 0.1f);
				particleSystem.transform.parent = starParent;

                // spawn game objects for the stars already genereated and create references to them 
                foreach (GalaxyStar galaxyStar in sector.stars)
				{
					if (!galaxyStar.discovered)
						continue;

					GameObject star = GameObject.CreatePrimitive(PrimitiveType.Sphere);
					star.transform.parent = starParent;
					GalaxyStarComp starComp = star.AddComponent<GalaxyStarComp>();
					starComp.galaxyStar = galaxyStar;
					star.transform.position = galaxyStar.position;

					star.layer = m_gameManager.galaxyStarLayer;

					MeshRenderer meshRenderer = star.GetComponent<MeshRenderer>();
					meshRenderer.material = m_starMaterial;
					meshRenderer.material.SetColor("_EmissionColor", galaxyStar.starType.m_coreColor);

					star.transform.localScale *= 0.05f;

					
				}
			}

			// put the camera near sol
			m_cameraFocus.position = m_gameManager.lastStarVisited.position;
			m_camera.ForceCameraPosition(m_cameraFocus.position, Quaternion.identity);


			// set the 
			m_positionIndicatorUI.position = m_gameManager.lastStarVisited.position;

			
			for (int i = 0; i < m_gameManager.sectors.Count - 1; ++i)
            {

				GalaxySector fromSector = m_gameManager.sectors[i];
				GalaxySector toSector = m_gameManager.sectors[i + 1];

				if (!toSector.centerStar.discovered)
					continue;

				GameObject sector = new GameObject("Sector " + i);

				// lines
				LineRenderer lineRen = sector.AddComponent<LineRenderer>();
				lineRen.material = m_graphLineMaterial;

				Vector3[] positions = { fromSector.center, toSector.center };

				lineRen.SetPositions(positions);
				lineRen.enabled = true;
				lineRen.startWidth = 0.01f;
				lineRen.endWidth = 0.01f;
			}
		}

		private void Update()
		{
			if (Input.GetMouseButtonDown(0))
			{
				// clicked on something, check if it's a star
				var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
				RaycastHit hit = new RaycastHit();

				if (Physics.Raycast(ray, out hit)) //, m_galaxyStaLayer))
				{
					GameObject hitObject = hit.collider.gameObject;
					GalaxyStarComp starObject = hitObject.GetComponent<GalaxyStarComp>();
					if (starObject != null)
					{
						// set the UI
						SetFocusStar(starObject);
					}
				}
			}

			if (Input.GetKeyDown(KeyCode.Return) && !m_isUnloading)
			{
				// expend fuel, if necessary
				if (m_lastClickedStar != m_gameManager.lastStarVisited)
					m_gameManager.FlyToSystem();

				// remember what was clicked on
				m_gameManager.lastSystemSeed = m_lastClickedStar.seed;
				m_gameManager.lastStarVisited = m_lastClickedStar;

				// enter solar system command recieved, set discovered so it shows next time
				m_lastClickedStar.discovered = true;
				foreach (StarNode neighbor in m_lastClickedStar.node.m_linkedNodes)
                {
					neighbor.star.discovered = true;
                }

				if (m_gameManager == null)
					return;

				m_gameManager.LoadScene(GameManager.ESceneIndex.kSolarSystem, true);
				m_isUnloading = true;
			}
		}
		
		/// <summary>
		/// Set the focus 
		/// </summary>
		/// <param name="starObject"></param>
		private void SetFocusStar(GalaxyStarComp starObject)
        {
			m_nameLabel.text = starObject.galaxyStar.name;
            m_cosmicBodyUI.gameObject.SetActive(true);
            m_cosmicBodyUI.objectName = starObject.galaxyStar.name;
            m_cosmicBodyUI.transform.position = starObject.transform.position;
            m_cosmicBodyUI.transform.SetParent(starObject.transform);

			m_lastClickedStar = starObject.galaxyStar;
        }

	}
}
