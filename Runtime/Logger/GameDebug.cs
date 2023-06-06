using UnityEngine;

namespace GameHelperSDK
{
    public class GameDebug
    {
	    private object _header;
	    
	    public GameDebug(object header)
	    {
		    _header = header;
	    }
	    
        public virtual void Log(object message)
        {
			Debug.Log($"{_header}{message}");
        }
		
        public virtual void LogWarning(object message)
        {
	        Debug.LogWarning($"{_header}{message}");
        }
		
        public virtual void LogError(object message)
        {
	        Debug.LogError($"{_header}{message}");
        }
    }
}
