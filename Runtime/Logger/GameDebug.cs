using UnityEngine;

namespace GameHelperSDK
{
    public class GameDebug
    {
        public virtual void Log(object message)
        {
			Debug.Log(message);
        }
		
        public virtual void LogWarning(object message)
        {
	        Debug.LogWarning(message);
        }
		
        public virtual void LogError(object message)
        {
	        Debug.LogError(message);
        }
    }
}
