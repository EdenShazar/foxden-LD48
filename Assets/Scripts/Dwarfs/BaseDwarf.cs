using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseDwarf : MonoBehaviour {

  [SerializeField]
  private float speed;
  private Vector3 moveDirection = Vector3.right;

  private void Update() {
    gameObject.transform.Translate(moveDirection * speed * Time.deltaTime);
  }

  private void OnTriggerStay2D(Collider2D collision) {
    HandleCollision(collision);
  }

  private void OnTriggerEnter2D(Collider2D collision) {
    HandleCollision(collision);
  }

  private void HandleCollision(Collider2D collision) {
    if(Mathf.Abs(collision.ClosestPoint(transform.position).y - transform.position.y) < 0.01) {
      //Horizontal collision
      moveDirection *= -1.0f;
    }
  }

}
