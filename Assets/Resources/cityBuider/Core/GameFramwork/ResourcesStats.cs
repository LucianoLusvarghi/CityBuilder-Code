using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ResourcesStats{

  [System.Serializable]
  public struct ResourseMonitor {
    public string name;
    public float produced;
    public float consumed;
    public int stored;
    public int avaible;
  }

  [System.Serializable]
  public struct ResourseAverage {
    public string name;
    public List<ResourseMonitor> average;
  }

  public List<ResourseMonitor> record_;
  public List<ResourseAverage> average_;


  public float TimeToUpdateInfo;

  public float currentTimePassed;

  public int maxAverageCount;

  ManagerGame manager;

	// Use this for initialization
	public void Init (ManagerGame theManager) {

    manager = theManager;

    record_ = new List<ResourseMonitor>();
    average_ = new List<ResourseAverage>();


    {
      ResourseMonitor newResourse = new ResourseMonitor {
        name = "wood",
        produced = 0.0f,
        consumed = 0.0f,
        stored = 0,
        avaible = 0
      };

      record_.Add(newResourse);


      ResourseAverage newAverage = new ResourseAverage {
        average = new List<ResourseMonitor>(),
        name = "wood"
      };

      average_.Add(newAverage);

    }

    {
      ResourseMonitor newResourse = new ResourseMonitor {
        name = "fish",
        produced = 0.0f,
        consumed = 0.0f,
        stored = 0,
        avaible = 0
      };

      record_.Add(newResourse);


      ResourseAverage newAverage = new ResourseAverage {
        average = new List<ResourseMonitor>(),
        name = "fish"
      };

      average_.Add(newAverage);

    }

    {
      ResourseMonitor newResourse = new ResourseMonitor {
        name = "utilities",
        produced = 0.0f,
        consumed = 0.0f,
        stored = 0,
        avaible = 0
      };

      record_.Add(newResourse);


      ResourseAverage newAverage = new ResourseAverage {
        average = new List<ResourseMonitor>(),
        name = "utilities"
      };

      average_.Add(newAverage);

    }

    {
      ResourseMonitor newResourse = new ResourseMonitor {
        name = "vegetables",
        produced = 0.0f,
        consumed = 0.0f,
        stored = 0,
        avaible = 0
      };

      record_.Add(newResourse);


      ResourseAverage newAverage = new ResourseAverage {
        average = new List<ResourseMonitor>(),
        name = "vegetables"
      };

      average_.Add(newAverage);

    }

  }
	
	public void Update(float deltaTime) {

    if(currentTimePassed < TimeToUpdateInfo) {
      currentTimePassed += deltaTime;
      return;
    }

    for(int i=0; i< record_.Count; i++) {
      ResourseAverage averageResourse = average_[i];
      ResourseMonitor recordResourse = record_[i];

      ResourseMonitor newAverage = new ResourseMonitor {
        produced = recordResourse.produced / currentTimePassed,
        consumed = recordResourse.consumed / currentTimePassed
      };

      recordResourse.produced = 0.0f;
      recordResourse.consumed = 0.0f;

      record_[i] = recordResourse;

      averageResourse.average.Add(newAverage);

      if(averageResourse.average.Count > maxAverageCount) {
        averageResourse.average.RemoveAt(0);
      }

      UpdateResourseLine(recordResourse.name);

    }
    
    currentTimePassed = 0.0f;



  }

  public void AddResourseConsumed(string resourseName, float ammount) {

    int index = GetResourse(resourseName);
    if (0 > index) {
      return;
    }
    ResourseMonitor recordResourse = record_[index];

    recordResourse.consumed += ammount;

    record_[index] = recordResourse;
  }

  public void AddResourseProduced(string resourseName, float ammount) {

    int index = GetResourse(resourseName);
    if (0 > index) {
      return;
    }
    ResourseMonitor recordResourse = record_[index];
    recordResourse.produced += ammount;
    record_[index] = recordResourse;


  }

  public void ChangeResourseStored(string resourseName, int ammount) {
    int index = GetResourse(resourseName);

    if(0 > index) {
      return;
    }

    ResourseMonitor recordResourse = record_[index];
    recordResourse.stored += ammount;
    record_[index] = recordResourse;

    manager.ui_Stats.UpdateResourseStore(resourseName, recordResourse);
  }

  public void ChangeResourseAvaible(string resourseName, int ammount) {
    int index = GetResourse(resourseName);
    if (0 > index) {
      return;
    }
    ResourseMonitor recordResourse = record_[index];
    recordResourse.avaible += ammount;
    record_[index] = recordResourse;

    manager.ui_Stats.UpdateResourseStore(resourseName, recordResourse);
  }



  int GetResourse(string resourceName) {


    for (int i = 0; i < record_.Count; i++) {
      if (record_[i].name == resourceName) {
        return i;
      }

    }

    return -1;

  }




  public void UpdateResourseLine(string resourceName) {

    manager.ui_Stats.UpdateResourseLine(resourceName);

  }


}
