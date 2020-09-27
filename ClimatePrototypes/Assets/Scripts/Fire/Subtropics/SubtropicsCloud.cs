using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

public class SubtropicsCloud : MonoBehaviour {
	[SerializeField] float speed = 0;
	Vector2 worldMin, worldMax;

	Vector2 velocity;

	void Start() {
		var world = SubtropicsController.World;
		// Debug.Log(SubtropicsController.World.GetComponent<UnityEngine.Tilemaps.TilemapRenderer>().bounds);
		worldMin = world.GetComponentInParent<GridLayout>().CellToWorld(world.bottomLeftCell);
		worldMax = world.GetComponentInParent<GridLayout>().CellToWorld(world.topRightCell);
		this.Print(worldMin, worldMax);
	}

	void Update() {
		transform.position += CheckVelocity() * speed * Time.deltaTime;
		Wrap();
	}

	Vector3 CheckVelocity() =>
		velocity = SubtropicsController.Instance.wind.dir.ToString()
		.Select(d => (
			new Dictionary<char, Vector3> { 
				{ 'S', Vector3.down },
				{ 'N', Vector3.up },
				{ 'W', Vector3.left },
				{ 'E', Vector3.right }
			}
		) [d]).Aggregate((acc, d) => acc + d);

	void Wrap() {
		Vector3 worldCentre = (worldMax + worldMin) / 2;
		Vector3 worldDist = (worldMax - worldMin) / 2;
		if (transform.position.x > worldMax.x && velocity.x > 0)
			transform.position = new Vector3(worldCentre.x + worldDist.x * 1.25f, transform.position.y, transform.position.z);
		if (transform.position.x < worldMin.x && velocity.x < 0)
			transform.position = new Vector3(worldCentre.x + worldDist.x * 1.25f, transform.position.y, transform.position.z);
		if (transform.position.y > worldMax.y && velocity.y > 0)
			transform.position = new Vector3(transform.position.x, worldCentre.y + worldDist.y * 1.25f, transform.position.z);
		if (transform.position.y < worldMin.y && velocity.y < 0)
			transform.position = new Vector3(transform.position.x, worldCentre.y + worldDist.y * 1.25f, transform.position.z);
		// if (transform.position.x > worldMax.x || transform.position.x < worldMin.x)
		// 	transform.position += Vector3.right * (-transform.position.x + (transform.position.x > worldMax.x ? worldMin.x : worldMax.x) * 1.25f);
		// if (transform.position.y > worldMax.y || transform.position.y < worldMin.y)
		// 	transform.position += Vector3.up * (-transform.position.y + (transform.position.y > worldMax.y ? worldMin.y : worldMax.y) * 1.25f);
	}
}
