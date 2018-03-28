using System;
using Helpers;
using Managers;
/// <summary>
/// Synchronizable behaves just like a property in singleplayer mode or in a server,
/// but in a client it requests a variable from server.
/// 
/// Every property has individual name.
/// </summary>
/// <typeparam name="T">Type of property</typeparam>
public class Synchronizable<T>
{
	public Synchronizable(string name)
	{
		_name = name;
	}
	private readonly string _name;
	private T _value;

	public T Get()
	{
		return _value;
//		return GameStarter.Instance.Game.Type == GameType.MultiplayerClient ? GameStarter.Instance.Game.Client.TryToGetActiveVariable<T>(_name).Result : _value;
	}
	public void Set(T value)
	{

		if (GameStarter.Instance.Game.Type == GameType.MultiplayerClient)
		{
			var serializedValue = value.SynchronizableSerialize(_name);
			GameStarter.Instance.Game.Client.TryToSetActiveVariable(_name, serializedValue);
		}

		_value = value;
		//else _value = value;
	}

//	public string Serialize(T value)
//	{
//		string serializedValue;
//		if (value == null) serializedValue = null;
//		else
//		{
//			switch (_name)
//			{
//				case "GamePlayer":
//					serializedValue = (value as GamePlayer).Name;
//					break;
//				default:
//					throw new ArgumentOutOfRangeException();
//
//			}
//		}
//
//		return serializedValue;
//	}
}