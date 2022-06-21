using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Builds_manager{

  struct BuildsLevels {
    public List<BasicBuilds> all_builds_;
    public float deltaTime;
  }


  BuildsLevels[] all_builds_;

  public int updateLevels = 10;

  int currentLevel = 0;

	public void Init () {
    all_builds_ = new BuildsLevels[updateLevels];
    
    for(int i=0; i< updateLevels; i++) {
      all_builds_[i] = new BuildsLevels {
        all_builds_ = new List<BasicBuilds>(),
        deltaTime = 0.0f
      };

    }

  }
	
	
	public void Update (float deltaTime) {
		
    for(int i=0; i < updateLevels; i++) {

      all_builds_[i].deltaTime += deltaTime;

      if (i == currentLevel) {
        for(int j=0; j < all_builds_[i].all_builds_.Count; j++) {

          all_builds_[i].all_builds_[j].UpdateBuild(all_builds_[i].deltaTime);

        }
        all_builds_[i].deltaTime = 0.0f;
      }

    }

    currentLevel++;

    if(currentLevel >= updateLevels) {
      currentLevel = 0;
    }

  }

  public void RegisterBuild(BasicBuilds theBuild) {

    int index = 0;
    int count = all_builds_[0].all_builds_.Count;

    for(int i=0; i< updateLevels; i++) {

      if(all_builds_[i].all_builds_.Count < count) {
        index = i;
      }

    }

    BuildsLevels chosedLevel = all_builds_[index];

    if (!chosedLevel.all_builds_.Contains(theBuild)) {
      chosedLevel.all_builds_.Add(theBuild);
    }

    all_builds_[index] = chosedLevel;

  }


  public void RemoveBuild(BasicBuilds theBuild) {

    bool finded = false;

    for(int i=0; i < updateLevels && !finded; i++) {
      if (all_builds_[i].all_builds_.Contains(theBuild)) {
        all_builds_[i].all_builds_.Remove(theBuild);
        finded = true;
      }
    }
  }

}
