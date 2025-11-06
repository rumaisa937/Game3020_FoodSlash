using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FruitSpawner : MonoBehaviour {

	public GameObject fruit;
	public GameObject bomb;
	public float maxX = 8f;
	public float maxY = 5f;  // Maximum Y position for side spawns
	public float minY = -3f; // Minimum Y position for side spawns

	void Start () {
		Invoke ("StartSpawning", 1f);
	}

	void Update () {
		
	}

	public void StartSpawning () {
		InvokeRepeating ("SpawnFruitGroups", 1f, 6f);
	}

	public void StopSpawning () {
		CancelInvoke ("SpawnFruitGroup");
		StopCoroutine ("SpawnFruit");
	}

	public void SpawnFruitGroups () {
		StartCoroutine ("SpawnFruit");

		if (Random.Range (0, 6) > 2) {
			SpawnBomb ();
		}
	}

	Vector3 GetRandomSpawnPosition() {
		// Choose a random side (0: bottom, 1: left, 2: right, 3: top)
		int side = Random.Range(0, 4);
		Vector3 pos = Vector3.zero;

		switch (side) {
			case 0: // Bottom
				pos = new Vector3(Random.Range(-maxX, maxX), -maxY, 0);
				break;
			case 1: // Left
				pos = new Vector3(-maxX, Random.Range(minY, maxY), 0);
				break;
			case 2: // Right
				pos = new Vector3(maxX, Random.Range(minY, maxY), 0);
				break;
			case 3: // Top
				pos = new Vector3(Random.Range(-maxX, maxX), maxY, 0);
				break;
		}
		return pos;
	}

	Vector2 GetForceForPosition(Vector3 spawnPos) {
		// Calculate force direction towards center with some randomness
		Vector2 directionToCenter = new Vector2(-spawnPos.x * 0.5f, -spawnPos.y * 0.5f);
		return directionToCenter.normalized * 15f;
	}

	IEnumerator SpawnFruit () {
		for (int i = 0; i < 5; i++) {
			Vector3 spawnPos = GetRandomSpawnPosition();
			GameObject f = Instantiate(fruit, spawnPos, Quaternion.identity) as GameObject;
			
			Vector2 force = GetForceForPosition(spawnPos);
			f.GetComponent<Rigidbody2D>().AddForce(force, ForceMode2D.Impulse);
			f.GetComponent<Rigidbody2D>().AddTorque(Random.Range(-20f, 20f));

			yield return new WaitForSeconds(0.5f);
		}
	}

	void SpawnBomb () {
		Vector3 spawnPos = GetRandomSpawnPosition();
		GameObject b = Instantiate(bomb, spawnPos, Quaternion.identity) as GameObject;
		
		Vector2 force = GetForceForPosition(spawnPos);
		b.GetComponent<Rigidbody2D>().AddForce(force, ForceMode2D.Impulse);
		b.GetComponent<Rigidbody2D>().AddTorque(Random.Range(-50f, 50f));
	}

} // FruitSpawner