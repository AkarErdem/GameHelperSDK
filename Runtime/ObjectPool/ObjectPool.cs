using UnityEngine;
using System.Collections.Generic;

namespace GameHelperSDK 
{
	internal enum ObjectPoolLogType : byte
	{
		None,
		Warnings,
		All
	}
	
	[System.Serializable]
	class PoolInfo
	{
		public string PoolName;
		public PooledObject Prefab;
		public uint PoolSize;
		public bool IsFixedSize;
	}

	class Pool
	{
		private readonly Stack<PooledObject> _availableObjStack = new Stack<PooledObject>();

		private readonly bool _isFixedSize;
		private readonly PooledObject _pooledObjectPrefab;
		private readonly string _poolName;
		private readonly ObjectPoolDebug _logger;
		private uint _poolSize;

		public Pool(string PoolName, PooledObject pooledObjectPrefab, uint initialSize, bool isFixedSize, ObjectPoolDebug logger) 
		{
			this._poolName = PoolName;
			this._pooledObjectPrefab = pooledObjectPrefab;
			this._poolSize = initialSize;
			this._isFixedSize = isFixedSize;
			this._logger = logger;
			
			// Populate the pool
			for (int index = 0; index < initialSize; index++) 
			{
				AddObjectToPool(NewObjectInstance());
			}
		}
		
		private void AddObjectToPool(PooledObject po) 
		{
			// Add to pool
			po.gameObject.SetActive(false);
			_availableObjStack.Push(po);
			po.IsPooled = true;
		}
		
		private PooledObject NewObjectInstance() 
		{
			var po = Object.Instantiate(_pooledObjectPrefab);
			// var go = Object.Instantiate(_pooledObjectPrefab);

			// var po = go.GetComponent<PooledObject>();
			// 
			// if (po == null) 
			// {
			// 	  po = go.AddComponent<PooledObject>();
			// }
			
			// Set data
			po.PoolName = _poolName;
			return po;
		}

		public PooledObject NextAvailableObject(Vector3 position, Quaternion rotation) 
		{
			PooledObject po = null;
			
			if (_availableObjStack.Count > 0) 
			{
				po = _availableObjStack.Pop();
			} 
			else if (_isFixedSize == false) 
			{
				// Increment size var, this is for info purpose only
				_poolSize++;
				_logger.Log($"Growing pool {_poolName}. New size: {_poolSize}");
				
				// Create new object
				po = NewObjectInstance();
			} 
			else 
			{
				_logger.LogWarning($"No object available & cannot grow pool: {_poolName}");
			}

			if (po == null)
			{
				return null;
			}

			po.IsPooled = false;
			po.transform.SetPositionAndRotation(position, rotation);
			po.gameObject.SetActive(true);

			return po;
		} 
		
		public void ReturnObjectToPool(PooledObject po) 
		{
			if (_poolName.Equals(po.PoolName)) 
			{
				if (po.IsPooled) 
				{
					_logger.LogWarning($"{po.gameObject.name} is already in pool. Why are you trying to return it again? Check usage.");	
				} 
				else 
				{
					AddObjectToPool(po);
				}
			} 
			else 
			{
				_logger.LogError($"Trying to add object to incorrect pool {po.PoolName} {_poolName}");
			}
		}
	}

	/// <summary>
	/// Object pool
	/// </summary>
	public class ObjectPool : Singleton<ObjectPool>
	{
		[Header("Pools")]
		[SerializeField] private PoolInfo[] _poolInfo;

		[Header("Debug")] 
		[SerializeField] private ObjectPoolLogType _logType = ObjectPoolLogType.Warnings;
		
		// Mapping of pool name and pool
		private Dictionary<string, Pool> _poolDictionary  = new Dictionary<string, Pool>();
		private ObjectPoolDebug _logger;
		
		internal ObjectPoolLogType LogType => _logType;

		protected override void InitializeSingleton()
		{
			base.InitializeSingleton();
			
			// Create logger
			_logger = new ObjectPoolDebug(this);
			
			// Check for duplicate names
			CheckForDuplicatePoolNames();
			
			// Create pools
			CreatePools();
		}
		
		private void CheckForDuplicatePoolNames() 
		{
			for (int index = 0; index < _poolInfo.Length; index++) 
			{
				string poolName = _poolInfo[index].PoolName;
				if (poolName.Length == 0) 
				{
					_logger.LogError($"Pool {index} does not have a name!");
				}
				for (int internalIndex = index + 1; internalIndex < _poolInfo.Length; internalIndex++) 
				{
					if (poolName.Equals(_poolInfo[internalIndex].PoolName)) 
					{
						_logger.LogError($"Pool {index} & {internalIndex} have the same name. Assign different names.");
					}
				}
			}
		}

		private void CreatePools() 
		{
			foreach (var currentPoolInfo in _poolInfo) {
				
				var pool = new Pool(currentPoolInfo.PoolName, currentPoolInfo.Prefab, 
				                     currentPoolInfo.PoolSize, currentPoolInfo.IsFixedSize,
				                     _logger);

				
				_logger.Log("Creating pool: " + currentPoolInfo.PoolName);
				
				// Add to mapping dict
				_poolDictionary[currentPoolInfo.PoolName] = pool;
			}
		}

		/// <summary>
		/// Returns an available object from the pool or null in case the pool does not have any object available & can grow size is false
		/// </summary>
		public PooledObject GetObjectFromPool(string poolName) => GetObjectFromPool(poolName, Vector3.zero, Quaternion.identity); 
		/// <summary>
		/// Returns an available object from the pool or null in case the pool does not have any object available & can grow size is false
		/// </summary>
		public PooledObject GetObjectFromPool(string poolName, Vector3 position, Quaternion rotation) 
		{
			PooledObject result = null;
			
			if (_poolDictionary.TryGetValue(poolName, out var pool)) 
			{
				result = pool.NextAvailableObject(position, rotation);
				
				// Scenario when no available object is found in pool
				if (result == null) 
				{
					_logger.LogWarning("No object available in pool. Consider setting fixedSize to false.: " + poolName);
				}
			} 
			else 
			{
				_logger.LogError("Invalid pool name specified: " + poolName);
			}
			
			return result; 
		}

		public void ReturnObjectToPool(PooledObject po) 
		{
			// var po = go.GetComponent<PooledObject>();
			// 
			// if (po == null) 
			// {
			// 	_logger.LogWarning("Specified object is not a pooled instance: " + go.name);
			// }
			// else 
			// {
			// 	if (_poolDictionary.TryGetValue(po.PoolName, out var pool)) 
			// 	{
			// 		pool.ReturnObjectToPool(po);
			// 	} 
			// 	else 
			// 	{
			// 		_logger.LogWarning("No pool available with name: " + po.PoolName);
			// 	}
			// }
			
			if (_poolDictionary.TryGetValue(po.PoolName, out var pool)) 
			{
				pool.ReturnObjectToPool(po);
			} 
			else 
			{
				_logger.LogWarning("No pool available with name: " + po.PoolName);
			}
		}
	}

	public class ObjectPoolDebug : GameDebug
	{
		private readonly ObjectPool _objectPool;
		
		public ObjectPoolDebug(ObjectPool objectPool)
		{
			_objectPool = objectPool;
		}
		
		public override void Log(object message)
		{
			if (_objectPool.LogType == ObjectPoolLogType.All)
				base.Log(message);
		}

		public override void LogWarning(object message)
		{
			if (_objectPool.LogType != ObjectPoolLogType.None)
				base.LogWarning(message);
		}

		public override void LogError(object message)
		{
			if (_objectPool.LogType != ObjectPoolLogType.None)
				base.LogError(message);
		}
	}
}
