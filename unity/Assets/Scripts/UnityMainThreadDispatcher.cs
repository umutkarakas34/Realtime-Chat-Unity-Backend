using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnityMainThreadDispatcher : MonoBehaviour
{
    // Queue to hold actions to be executed on the main thread
    private static readonly Queue<Action> _executionQueue = new Queue<Action>();

    // Update is called once per frame
    public void Update()
    {
        // Execute all queued actions
        lock (_executionQueue)
        {
            while (_executionQueue.Count > 0)
            {
                _executionQueue.Dequeue().Invoke();
            }
        }
    }

    // Enqueue an action to be executed on the main thread
    public static void Enqueue(Action action)
    {
        lock (_executionQueue)
        {
            _executionQueue.Enqueue(action);
        }
    }

    // Singleton instance
    private static UnityMainThreadDispatcher _instance = null;

    // Check if the instance exists
    public static bool Exists()
    {
        return _instance != null;
    }

    // Get the instance of UnityMainThreadDispatcher
    public static UnityMainThreadDispatcher Instance()
    {
        if (!Exists())
        {
            throw new Exception("UnityMainThreadDispatcher is not initialized");
        }
        return _instance;
    }

    // Awake is called when the script instance is being loaded
    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject); // Persist this instance between scenes
        }
        else
        {
            Destroy(gameObject); // Destroy duplicate instances
        }
    }
}
