using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JobSelector : MonoBehaviour {
  private const float jobButtonWidth = 0.32f;
  private const float jobButtonLeftLimit = -0.64f;
  [SerializeField]
  private GameObject border;
  private static List<DwarfJob> jobs;
  private static int selectedJob;
  private int newSelectedJob;
  private static JobSelector instance;

  public static DwarfJob GetSelectedJob() {
    return jobs[selectedJob];
  }

  private void Start() {
    EnsureSingleton();
    InitializeJobs();
    selectedJob = 0;
    newSelectedJob = 0;
    UpdateSelectedJob();
  }

  private void Update() {
    if(Input.GetKeyDown(KeyCode.Alpha1)) {
      newSelectedJob = 0;
    }
    if(Input.GetKeyDown(KeyCode.Alpha2)) {
      newSelectedJob = 1;
    }
    if(Input.GetKeyDown(KeyCode.Alpha3)) {
      newSelectedJob = 2;
    }
    if(Input.GetKeyDown(KeyCode.Alpha4)) {
      newSelectedJob = 3;
    }
    if(Input.GetKeyDown(KeyCode.Alpha5)) {
      newSelectedJob = 4;
    }
    if(newSelectedJob != selectedJob) {
      UpdateSelectedJob();
    }
  }

  private void UpdateSelectedJob() {
    selectedJob = newSelectedJob;
    border.transform.localPosition = new Vector3(jobButtonLeftLimit + jobButtonWidth * selectedJob, 0.0f, 0.0f);
  }

  private void EnsureSingleton() {
    if(instance == null) {
      instance = this;
    } else {
      Debug.LogWarning("JobSelector instance already exists on " + instance.gameObject +
              ". Deleting instance from " + gameObject);
      DestroyImmediate(this);
    }
  }

  private void InitializeJobs() {
    jobs = new List<DwarfJob>() {
      ScriptableObject.CreateInstance<DigDownJob>(),
      ScriptableObject.CreateInstance<DigSideJob>(),
      ScriptableObject.CreateInstance<StopSignJob>(),
      ScriptableObject.CreateInstance<RopeJob>(),
      ScriptableObject.CreateInstance<DigDownJob>()
    };
  }
}
