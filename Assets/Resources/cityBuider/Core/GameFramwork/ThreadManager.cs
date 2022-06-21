using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ThreadManager{

  Queue<ThreadPath> Threads_;

	public void Init(int numOfThread, ManagerGame manager) {
    Threads_ = new Queue<ThreadPath>();    

    for (int i=0; i < numOfThread; i++) {
      ThreadPath newThread = new ThreadPath();

      newThread.Init(manager.mapSys.Width(), manager.mapSys.Height());

      Threads_.Enqueue(newThread);
    }
       
  }

  public ThreadPath GetThread() {

    if (Threads_.Count > 0) {
      
      ThreadPath theThreadToReturn = null;
      theThreadToReturn = Threads_.Dequeue();

      return theThreadToReturn;
    }

    return null;
  }

  public void ReturnThread(ThreadPath theThread) {
    
    Threads_.Enqueue(theThread);
  }


}
