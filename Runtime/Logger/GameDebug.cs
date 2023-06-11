using UnityEngine;

namespace GameHelperSDK
{
    public class GameDebug
    {
	    private readonly object _header;
	    
	    public bool IsEnabled { get; set; }

	    public GameDebug(object header = default, bool isEnabled = false)
	    {
		    _header = header;
		    IsEnabled = isEnabled;
	    }
	    
        public virtual void Log(object message)
        {
	        if (!IsEnabled) return;
			Debug.Log($"{_header}{message}");
        }
		
        public virtual void LogWarning(object message)
        {
	        if (!IsEnabled) return;
	        Debug.LogWarning($"{_header}{message}");
        }
		
        public virtual void LogError(object message)
        {
	        if (!IsEnabled) return;
	        Debug.LogError($"{_header}{message}");
        }
    }
}
