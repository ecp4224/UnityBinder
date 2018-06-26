# UnityBinder
Get rid of all those messy `GetComponent` and `Resources.Load` calls!

This script allows you to inject your public/private fields with what you need automatically! It's lightweight and easy to use/configure. 

What is injecting? Injecting simply means that the `GetComponent` and `Resources.Load` functions are automatically called for you and "injected" into the field requesting it.

## Installing

Simply put this script anywhere in your project. 

Once in your project, you can start using UnityBinder by either doing

```csharp
void Start() {
    UnityBinder.Inject(this)
}
```

or by extending the `BindableMonoBehavior` class instead of the default `MonoBehavior`

```csharp
public class SimpleScript : BindableMonoBehavior {
    //Nothing else is needed, sub-class automatically injects on Awake()
}
```

The `BindableMonoBehavior` simply invokes `UnityBinder.Inject(this)` on `Awake()`. The `Awake()` function is virtual, so it can be overriden if needed

## How to use

To Bind a field to a component, simply annotate the field with `[BindComponent]`. With the default constructor, it'll simple search for the Component on the gameObject attached to the current script.

Example:
```csharp
public class SimpleScript : BindableMonoBehavior {
    
    [BindComponent]
    private RigidBody _rigidBody;

    ...
}
```
This example will always set the `_rigidBody` field to be equal to the RigidBody component attached to the gameObject that is using this `SimpleScript`. Nothing else is needed!

`[BindComponent]` has several options for more flexability

* fromObject [string] - The name of the GameObject in the scene to search in (can be a path)
* failWhenNull [bool] - Should the game fail if this component can't be find
* index [int] - Which component to bind to, if a gameObject has multiple of the same type

You can also bind any kind of resource using a similar syntax
```csharp
public class SimpleScript : BindableMonoBehavior {
    
    [BindComponent]
    private RigidBody _rigidBody;
    
    [BindResource("Sprites/Square")]
    private Sprite _square;

    ...
}
```

The `[BindResource]` attribute always requires a path.

## To Do

* [BindAsset] - An annotation to bind any other kind of Object from your assets automatically

## Example Script

```csharp
using UnityEngine;


public class SimpleScript : MonoBehaviour
{
	/// <summary>
	/// Get the Rigidbody2D component that's attached to this gameObject
	/// </summary>
	[BindComponent]
	private Rigidbody2D _rigidbody2D;

	/// <summary>
	/// Get the BoxCollider2D component that's attached to the gameObject with the path
	/// "/OtherSquare"
	/// 
	/// This object will be found even if it's inactive in the scene
	/// </summary>
	[BindComponent(fromObject = "/OtherSquare")]
	private BoxCollider2D _otherCollider;

	/// <summary>
	/// Get the Camera component that's attached to the gameObject with the name "Main Camera"
	/// </summary>
	[BindComponent(fromObject = "Main Camera")]
	private Camera camera;

	/// <summary>
	/// Get the SpriteRenderer component that's attached to the gameObject with the path
	/// "/Square/OtherSquare (1)"
	/// </summary>
	[BindComponent(fromObject = "/Square/OtherSquare (1)", failWhenNull = true)]
	private SpriteRenderer _renderer;

	/// <summary>
	/// Get the other SimpleInputPlayer component that's attached to this gameObject
	/// </summary>
	[BindComponent(index = 1)]
	private SimpleScript otherInput;
	

	// Use this for initialization
	void Start()
	{
     		//Inject all variables
		UnityBinder.Inject(this);
		
		//This will already be done by BindComponent
		//_rigidbody2D = GetComponent<Rigidbody2D>();
		
		_rigidbody2D.AddForce(new Vector2(100f, 50f));
		
		Debug.Log("IT WORKED! " + _rigidbody2D);

		Debug.Log("IT ALSO WORKED! " + _otherCollider);
		
		Debug.Log("YOOO " + _renderer);
	}
}
```
