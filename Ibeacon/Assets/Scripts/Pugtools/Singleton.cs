using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PugTools
{
    public enum SingletonType
    {
        //The singleton is global in game & not destory when scene change.
        GlobalInstance,
        //The singleton is global in game & not destory when scene change.
        //This singleton must use information from data.
        GlobalInstanceDataSpecify,
        //The singleton only use in this scene & will destory when scene change.
        PerSceneInstance,
    }

    //-----------------------------------------------------------------
    //class parameter
    //-----------------------------------------------------------------
    public class SingletonTypeAliveChecker
    {
        static SingletonTypeAliveChecker _instance;

        public Dictionary<System.Type, GameObject> typeAliveCheckers = new Dictionary<System.Type, GameObject>();

        protected SingletonTypeAliveChecker()
        {
        }

        public static SingletonTypeAliveChecker Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new SingletonTypeAliveChecker();
                }
                return _instance;
            }
        }
    }



    //-----------------------------------------------------------------
    //class parameter
    //-----------------------------------------------------------------
    public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        //-----------------------------------------------------------------
        //static parameter
        //-----------------------------------------------------------------
        private static T _instance;
        private static object _lock = new object();

        private static bool dontDestroyFindObjOnLoad = false;   // Instance from FindObjectOfType.
        private static bool dontDestroyNewObjOnLoad = true;     // Instance from AddComponent.

        protected SingletonType singletonType
        {
            set
            {
                switch (value)
                {
                    case SingletonType.GlobalInstance:
                        dontDestroyFindObjOnLoad = true;
                        dontDestroyNewObjOnLoad = true;
                        break;

                    //case SingletonType.GlobalInstanceDataSpecify:
                    //	dontDestroyFindObjOnLoad = true;
                    //	dontDestroyNewObjOnLoad = false;
                    //	break;

                    case SingletonType.PerSceneInstance:
                        dontDestroyFindObjOnLoad = false;
                        dontDestroyNewObjOnLoad = false;
                        break;
                }
            }
        }

        public static T Instance
        {
            get
            {
                if (applicationIsQuitting)
                {
                    /*
                    if (Debug.isDebugBuild) Debug.LogWarning("[Singleton] Instance '" + typeof(T) +
                        "' already destroyed on application quit." +
                        " Won't create again - returning null.");
                    */
                    return null;
                }
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = (T)FindObjectOfType(typeof(T));

                        if (FindObjectsOfType(typeof(T)).Length > 1)
                        {
                            if (Debug.isDebugBuild) Debug.LogError("[Singleton] Something went really wrong " +
                                " - there should never be more than 1 singleton!" + " Reopenning the scene might fix it.");
                            return _instance;
                        }

                        if (_instance == null)
                        {
                            GameObject singleton = new GameObject();
                            _instance = singleton.AddComponent<T>();
                            singleton.name = "(singleton) " + typeof(T).ToString();

                            if (dontDestroyNewObjOnLoad)
                            {
                                DontDestroyOnLoad(singleton);
                            }
                            /*if (Debug.isDebugBuild) Debug.Log("[Singleton] An instance of " + typeof(T) + " is needed in the scene, so '" + singleton + "' was created with DontDestroyOnLoad.");*/
                        }
                        else
                        {
                            /*if (Debug.isDebugBuild) Debug.Log("[Singleton] Using instance already created: " + _instance.gameObject.name);*/
                            if (dontDestroyFindObjOnLoad)
                            {
                                DontDestroyOnLoad(_instance.transform.root);
                            }
                        }
                    }
                    return _instance;
                }
            }
        }



        public virtual void Awake()
        {
            if (!SingletonTypeAliveChecker.Instance.typeAliveCheckers.ContainsKey(typeof(T)))
            {
                SingletonTypeAliveChecker.Instance.typeAliveCheckers.Add(typeof(T), gameObject);
            }
        }

        public virtual void OnDestroy()
        {
            if (SingletonTypeAliveChecker.Instance.typeAliveCheckers.ContainsKey(typeof(T)) && SingletonTypeAliveChecker.Instance.typeAliveCheckers[typeof(T)] == gameObject)
            {
                SingletonTypeAliveChecker.Instance.typeAliveCheckers.Remove(typeof(T));
            }
        }

        private static bool applicationIsQuitting = false;
        /// <summary>
        /// When Unity quits, it destroys objects in a random order.
        /// In principle, a Singleton is only destroyed when application quits.
        /// If any script calls Instance after it have been destroyed, 
        ///   it will create a buggy ghost object that will stay on the Editor scene
        ///   even after stopping playing the Application. Really bad!
        /// So, this was made to be sure we're not creating that buggy ghost object.
        /// </summary>
        public void OnApplicationQuit()
        {
            applicationIsQuitting = true;
        }
    }


}
