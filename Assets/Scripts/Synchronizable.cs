using Managers;

public class Synchronizable<T>
{
	//	private Game Game;
	public Synchronizable(string name)
	{
		_name = name;
	}
	private readonly string _name;
	private T _value;

	public T Get()
	{
		return GameStarter.Instance.Game.Type == GameType.MultiplayerClient ? GameStarter.Instance.Game.Client.TryToGetActiveVariable<T>(_name).Result : _value;
	}
	public void Set(T value)
	{
		if (GameStarter.Instance.Game.Type == GameType.MultiplayerClient) GameStarter.Instance.Game.Client.TryToSetActiveVariable(_name, value);
		else _value = value;
	}
}