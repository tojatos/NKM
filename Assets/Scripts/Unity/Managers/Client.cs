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

	    public async void TryConnecting(string hostname, int port)
		{
			try
			{
				_client = new TcpClient(hostname, port);
				OnConnection?.Invoke();
				await Task.Run((Action) ListerForMessages);
			}
			catch (Exception e)
			{
				OnError?.Invoke(e.Message);
			}
			finally
			{
				if (_client != null)
				{
					_client.Close();
                    OnDisconnect?.Invoke();
				}
			}
		}

	    private void ListerForMessages()
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