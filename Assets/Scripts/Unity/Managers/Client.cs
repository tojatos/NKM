using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using NKMCore;

namespace Unity.Managers
{
    public class Client
    {
	    public event Delegates.Void OnConnection;
	    public event Delegates.Void OnDisconnect;
	    public event Delegates.String OnMessage;
	    public event Delegates.String OnError;

	    private TcpClient _client;
	    private NetworkStream _msgStream;
	    private bool _running;
        private bool _clientRequestedDisconnect;
	    public bool IsConnected => !_isDisconnected(_client);

	    public void TryConnecting(string hostname, int port)
	    {
			if (_running)
			{
				OnError?.Invoke("Already connected!");
				return;
			}

		    try
		    {
			    _client = new TcpClient(hostname, port);
			    if (!_client.Connected) return;
			    OnConnection?.Invoke();
			    _running = true;
			    _msgStream = _client.GetStream();
			    new Thread(ListenForMessages).Start();
		    }
		    catch (SocketException se)
		    {
			    OnError?.Invoke(se.Message);
		    }

		}
	    
        private void CleanupNetworkResources()
        {
            _msgStream?.Close();
            _msgStream = null;
	        _client.Close();
        }

        public void Disconnect()
        {
            OnDisconnect?.Invoke();
	        SendMessage("bye");
            _running = false;
            _clientRequestedDisconnect = true;
        }
	    private void ListenForMessages()
	    {
			using (NetworkStream s = _client.GetStream())
			{
				using (var sr = new StreamReader(s))
				{
					while(_running)
					{
						if (_client.Available > 0)
						{
							string message;
							if((message = sr.ReadLine()) != null)
	                            OnMessage?.Invoke(message);
						}
						
                        if (_isDisconnected(_client) && !_clientRequestedDisconnect)
                        {
                            _running = false;
                        }
					}
					CleanupNetworkResources();
				}
			}	
	    }

	    public void SendMessage(string message)
	    {
		    if(_msgStream==null) return;
		    var sw = new StreamWriter(_msgStream) {AutoFlush = true};
            sw.WriteLine(message);
	    }
	    
        // Checks if a client has disconnected ungracefully
        // Adapted from: http://stackoverflow.com/questions/722240/instantly-detect-client-disconnection-from-server-socket
        private static bool _isDisconnected(TcpClient client)
        {
	        if (client == null) return true;
            try
            {
                Socket s = client.Client;
                return s.Poll(10 * 1000, SelectMode.SelectRead) && (s.Available == 0);
            }
            catch(Exception ex)
            {
                // We got a socket or disposed error, assume it's disconnected
	            if(ex is ObjectDisposedException || ex is SocketException)
                    return true;
	            
	            throw;
            }
        }
    }
}
