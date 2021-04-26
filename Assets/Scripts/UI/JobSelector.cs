using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class JobSelector : MonoBehaviour {
    private const float jobButtonWidth = 0.32f;
    private const float jobButtonLeftLimit = -0.64f;
  
    [SerializeField] private SpriteRenderer border;
    [SerializeField] private GameObject costParent;
    [SerializeField] private TextMeshPro costText;
    [SerializeField] private TextMeshPro freeText;
    [SerializeField] private SpriteRenderer costIcon;
    [SerializeField] private Sprite tileIcon;
    [SerializeField] private Sprite stopSignIcon;
    [SerializeField] private Sprite ropeIcon;
      
    private static List<DwarfJob> jobs;
    private static int selectedJob;
    private int newSelectedJob;
    private static JobSelector instance;

  public static DwarfJob GetSelectedJob() {
        return jobs[selectedJob] == null ? null : Instantiate(jobs[selectedJob]);
    }

    public static DwarfJob GetBreakJob()
    {
        return Instantiate(jobs[4]);
    }

  private void Start() {
    EnsureSingleton();
    InitializeJobs();
    selectedJob = 0;
    newSelectedJob = 0;
    UpdateSelectedJob();
  }

  private void Update() {
    if (Input.GetKeyDown(KeyCode.Alpha1)) {
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
    if (Input.GetKeyDown(KeyCode.Escape)) {
        newSelectedJob = 5;
    }
        if (newSelectedJob != selectedJob) {
      UpdateSelectedJob();
    }
  }

    private void UpdateSelectedJob()
    {
        selectedJob = newSelectedJob;
        border.transform.localPosition = new Vector3(jobButtonLeftLimit + jobButtonWidth * selectedJob, 0.0f, 0.0f);
        
        // No job selected
        if (selectedJob == 5)
        {
            costParent.gameObject.SetActive(false);

            border.color = Constants.clearColor;
            freeText.color = Constants.clearColor;
        }
        else if (selectedJob == 4)
        {
            costParent.gameObject.SetActive(false);
            freeText.color = Color.white;
        }
        else
        {
            costParent.gameObject.SetActive(true);
            freeText.color = Constants.clearColor;

            border.color = Color.white;
            switch (selectedJob)
            {
                case 0:
                case 1:
                    costText.text = Constants.digCost.ToString();
                    costIcon.sprite = tileIcon;
                    break;
                case 2:
                    costText.text = Constants.stopSignCost.ToString();
                    costIcon.sprite = stopSignIcon;
                    break;
                case 3:
                    costText.text = Constants.ropeCost.ToString();
                    costIcon.sprite = ropeIcon;
                    break;
            }
        }

        // Set transparent when no job selected
        border.color = new Color(1f, 1f, 1f, selectedJob == 5 ? 0f : 1f);
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
      ScriptableObject.CreateInstance<GetBoozeJob>(),
      null
    };
  }
}
