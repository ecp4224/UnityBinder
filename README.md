# UnityBinder
Annotate fields with [BindComponent] and automatically cast the corresponding component from the scene.

## Installing

Simply put this script anywhere in your project. Once in your project, you can start using UnityBinder by either invoking

```
void Start() {
    UnityBinder.Inject(this)
}
```

or by extending the `BindableMonoBehavior` class instead of the default `MonoBehavior`

```
public class SimpleScript : BindableMonoBehavior {
    //Nothing else is needed, sub-class automatically injects on Awake()
}
```

## How to use

To Bind a field to a component, simply annotate the field with `[BindComponent]`. With the default constructor, it'll simple search for the Component on the gameObject attached to the current script.

`[BindComponent]` has several options for more flexability

* fromObject [string] - The name of the GameObject in the scene to search in (can be a path)
* failWhenNull [bool] - Should the game fail if this component can't be find
* index [int] - Which component to bind to, if a gameObject has multiple of the same type
* fromAsset [string] **coming soon** - The name of the Object in your assets to search in (can be a path) 

## Example

```
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
		
		//This will already be done by InjectComponent
		//_rigidbody2D = GetComponent<Rigidbody2D>();
		
		_rigidbody2D.AddForce(new Vector2(100f, 50f));
		
		Debug.Log("IT WORKED! " + _rigidbody2D);

		Debug.Log("IT ALSO WORKED! " + _otherCollider);
		
		Debug.Log("YOOO " + _renderer);
	}
}
```
