using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineCreator : MonoBehaviour {

	int vertexCount = 0;
	bool mouseDown = false;
	LineRenderer line;

	public GameObject blast;

	public GameObject splash;

	public AudioClip explosionSound;

	GameplayController gc;

	// Track objects already processed by this line to avoid double-counting collisions
	private System.Collections.Generic.HashSet<int> processedObjects = new System.Collections.Generic.HashSet<int>();

	void Awake () {
		line = GetComponent<LineRenderer> ();
	}

	void Start () {
		gc = FindFirstObjectByType<GameplayController>();
	}

	void Update () {
		// Android specific code
		if (Application.platform == RuntimePlatform.Android) {
			if (Input.touchCount > 0) {
				// Gets only the first touch (No multi-touch)
				if (Input.GetTouch (0).phase == TouchPhase.Began) {
					// New touch/slice started - allow processing of objects again
					processedObjects.Clear();
				}
				if (Input.GetTouch (0).phase == TouchPhase.Moved) {
					line.positionCount = vertexCount + 1;
					Vector3 mousePos = Camera.main.ScreenToWorldPoint (Input.mousePosition);
					line.SetPosition (vertexCount, mousePos);
					vertexCount++;

					BoxCollider2D box = gameObject.AddComponent<BoxCollider2D> ();
					box.transform.position = line.transform.position;
					box.size = new Vector2 (0.1f, 0.1f);
				}

				if (Input.GetTouch (0).phase == TouchPhase.Ended) {
					line.positionCount = 0;
					vertexCount = 0;

					BoxCollider2D[] colliders = GetComponents<BoxCollider2D> ();
					foreach (BoxCollider2D box in colliders) {
						Destroy (box);
					}
					// Clear processed IDs so future slices can process new objects
					processedObjects.Clear();
				}
			}

		// Windows specific code (Editor)
		} else if (Application.platform == RuntimePlatform.WindowsEditor) {

			if (Input.GetMouseButtonDown (0)) {
				mouseDown = true;
				// New slice started with mouse - allow processing of objects again
				processedObjects.Clear();
			}

			if (mouseDown) {
				line.positionCount = vertexCount + 1;
				Vector3 mousePos = Camera.main.ScreenToWorldPoint (Input.mousePosition);
				line.SetPosition (vertexCount, mousePos);
				vertexCount++;

				BoxCollider2D box = gameObject.AddComponent<BoxCollider2D> ();
				box.transform.position = line.transform.position;
				box.size = new Vector2 (0.1f, 0.1f);
			}

			if (Input.GetMouseButtonUp (0)) {
				mouseDown = false;
				line.positionCount = 0;
				vertexCount = 0;

				BoxCollider2D[] colliders = GetComponents<BoxCollider2D> ();
				foreach (BoxCollider2D box in colliders) {
					Destroy (box);
				}
				// Clear processed IDs so future slices can process new objects
				processedObjects.Clear();
			}
		}
	} // Update

	void OnCollisionEnter2D (Collision2D target) {
		int objId = target.gameObject.GetInstanceID();
		if (processedObjects.Contains(objId)) {
			return;
		}
		processedObjects.Add(objId);
	if (target.gameObject.tag == "Bomb") {
		GameObject b = Instantiate (blast, target.transform.position, Quaternion.identity) as GameObject;
		Destroy (b.gameObject, 2f);

		// Play explosion sound at bomb position
		if (explosionSound != null) {
			AudioSource.PlayClipAtPoint(explosionSound, target.transform.position);
		}

		// Notify the gameplay controller that a life should be lost (only once per bomb)
		if (gc != null)
		{
			gc.DecreaseLife();
		}			// Increase bombs-per-group so the game gets harder when bombs are destroyed
			FruitSpawner fs = FindFirstObjectByType<FruitSpawner>();
			if (fs != null) {
				fs.IncrementBombsPerGroup(1);
			} else {
				Debug.LogWarning("LineCreator: FruitSpawner not found; cannot increase bomb spawn count.");
			}

			Destroy (target.gameObject);
		}

		if (target.gameObject.tag == "Fruit") {
			GameObject s = Instantiate (splash, new Vector3(target.transform.position.x -1, target.transform.position.y,0), Quaternion.identity) as GameObject;
			Destroy (s.gameObject, 2f);
			Destroy (target.gameObject);

			int rand = 100;
			if (gc != null) {
				gc.playerScore += rand;
			} else {
				Debug.LogWarning("LineCreator: GameplayController reference not found; score not updated.");
			}
		}
	}

} // LineCreator