using System;
using System.Reflection;
using UnityEngine;

public static class UnityBinder 
{
	public static void Inject(MonoBehaviour obj)
	{
		var bindingFlags = BindingFlags.Instance |
		                   BindingFlags.NonPublic |
		                   BindingFlags.Public;
		
		var fields = obj.GetType().GetFields(bindingFlags);

		foreach (var field in fields)
		{
			var injections = (BindComponent[])field.GetCustomAttributes(typeof(BindComponent), true);

			if (injections.Length > 0)
			{
				foreach (var inject in injections)
				{
					var fromObjName = inject.fromObject;
					var injectType = field.FieldType;
					var shouldFail = inject.failWhenNull;
					var index = inject.index;
					
					var unityCall = typeof(GameObject).GetMethod("GetComponents", new Type[0]);
					if (unityCall == null)
					{
						Debug.LogError("Could not find method GetComponents !!");
						break;
					}

					GameObject fromObj;
					if (string.IsNullOrEmpty(fromObjName))
					{
						fromObj = obj.gameObject;
					}
					else
					{
						fromObj = GameObject.Find(fromObjName);

						if (fromObj == null)
						{
							fromObj = FindInActiveObjectByName(fromObjName);

							if (fromObj == null)
							{
								if (shouldFail)
								{
									Fail("Could not find GameObject with name " + fromObjName + " for field " + field.Name);
								}

								continue;
							}
						}
					}
					

					var genericMethod = unityCall.MakeGenericMethod(injectType);
					var rawResult = genericMethod.Invoke(fromObj, null);

					if (rawResult == null)
					{
						if (shouldFail)
						{
							Fail("Could not find component of type " + injectType + " for field " + field.Name);
						}
					} 
					else if (rawResult is object[])
					{
						var result = rawResult as object[];

						if (result.Length > 0)
						{
							if (index >= result.Length)
							{
								Fail("Could not find component of type " + injectType + " for field " + field.Name + " at index " + index);
							}
							else
							{
								var found = result[index];
								
								field.SetValue(obj, found);
							}
						}
						else
						{
							if (shouldFail)
							{
								Fail("Could not find component of type " + injectType + " for field " + field.Name);
							}
						}
					}
				}
			}
		}
	}
	
	private static void Fail(string reason)
	{
		Debug.LogError(reason);
		Application.Quit();
	}

	private static GameObject DeepFind(string name)
	{
		if (name.StartsWith("/"))
		{
			string[] temp = name.Split('/');

			GameObject current = null;
			for (int i = 1; i < temp.Length; i++)
			{
				string n = temp[i];
				if (current == null)
				{
					current = GameObject.Find(n);
					if (current == null)
					{
						current = FindInActiveObjectByName(n);
					}
				}
				else
				{
					current = current.transform.Find(n).gameObject;
				}
			}

			return current;
		}
		
		return GameObject.Find(name);
	}
	
	private static GameObject FindInActiveObjectByName(string name)
	{
		if (name.StartsWith("/"))
			return DeepFind(name);
		
		Transform[] objs = Resources.FindObjectsOfTypeAll<Transform>();
		for (int i = 0; i < objs.Length; i++)
		{
			if (objs[i].hideFlags == HideFlags.None)
			{
				if (objs[i].name == name)
				{
					return objs[i].gameObject;
				}
			}
		}
		return null;
	}
}

[AttributeUsage(AttributeTargets.Field)] 
public class BindComponent : Attribute
{

	public int index = 0;
	public bool failWhenNull = false;
	public string fromObject = "";

	public BindComponent(int index = 0, bool failWhenNull = false, string fromObject = "")
	{
		this.index = index;
		this.failWhenNull = failWhenNull;
		this.fromObject = fromObject;
	}
}

public class BindableMonoBehavior : MonoBehaviour {

	public virtual void Awake()
	{
		UnityBinder.Inject(this);
	}
}
