using System;
using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;
using NKMCore;
using Action = System.Action;

namespace Unity.Managers
{
    public class Client
    {
	    public event Delegates.Void OnConnection;
	    public event Delegates.Void OnDisconnect;
	    public event Delegates.String OnMessage;
	    public event Delegates.String OnError;

	    private TcpClient _client;
	    private bool _isConnected;

	    public async void TryConnecting(string hostname, int port)
		{
			if (_isConnected)
			{
				OnError?.Invoke("Already connected!");
				return;
			}
			try
			{
				_client = new TcpClient(hostname, port);
				OnConnection?.Invoke();
				_isConnected = true;
				await Task.Run((Action) ListenForMessages);
			}
			catch (Exception e)
			{
				OnError?.Invoke(e.Message);
			}
			finally
			{
				_client?.Close();
				if (_isConnected)
				{
                    OnDisconnect?.Invoke();
                    _isConnected = false;
				}
			}
		}

	    private void ListenForMessages()
	    {
			using (NetworkStream s = _client.GetStream())
			{
				using (var sr = new StreamReader(s))
				{
					string message;
					while((message = sr.ReadLine()) != null)
					{
						OnMessage?.Invoke(message);
					}
				}
			}	
	    }

	    public void SendMessage(string message)
	    {
		    NetworkStream s = _client.GetStream();
		    var sw = new StreamWriter(s) {AutoFlush = true};
            sw.WriteLine(message);
	    }
    }
}
