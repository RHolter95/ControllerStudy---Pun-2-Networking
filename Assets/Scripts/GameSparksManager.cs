using UnityEngine;
using System.Collections;
/// <summary>
/// Created for GameSparks tutorial, October 2015, Sean Durkan
/// This class sets up the GameSparksUnity script with a persistant gameobjec
/// </summary>
public class GameSparksManager : MonoBehaviour 
{
	/// <summary>The GameSparks Manager singleton</summary>
	private static GameSparksManager instance = null;

	void Awake()
	{
		if (instance == null) // check to see if the instance has a refrence
		{
			instance = this; // if not, give it a refrence to this class...
			DontDestroyOnLoad(this.gameObject); // and make this object persistant as we load new scenes
		} 
		else // if we already have a refrence then remove the extra manager from the scene
		{
			Destroy(this.gameObject);
		}
	}
}
