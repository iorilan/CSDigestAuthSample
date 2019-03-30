
	public class DigestProxy
	{
		private static ILog _log = LogManager.GetLogger(typeof(DigestProxy));
		public static byte[] GetData(string url, string userName, string password)
		{
			try
			{
				Uri uri = new Uri(url);

				DigestHttpWebRequest req = new DigestHttpWebRequest(userName, password);

				using (HttpWebResponse webResponse = req.GetResponse(uri))
				using (Stream responseStream = webResponse.GetResponseStream())
				{
					using (var memoryStream = new MemoryStream())
					{
						responseStream.CopyTo(memoryStream);
						return memoryStream.ToArray();
					}
				}
			}
			catch (WebException caught)
			{
				throw new WebException(string.Format("Exception in WebServiceCall: {0}", caught.Message));
			}
			catch (Exception caught)
			{
				throw new Exception(string.Format("Exception in WebServiceCall: {0}", caught.Message));
			}
		}
		
		////listen response from long http connection
		public static async Task BlockListenEvent(string url, string userName, string password, Action<string> onData)
		{
			try
			{
				while (true)
				{
					try
					{
						string response = string.Empty;

						Uri uri = new Uri(url);

						DigestHttpWebRequest req = new DigestHttpWebRequest(userName, password);

						using (HttpWebResponse webResponse = req.GetResponse(uri))
						using (Stream responseStream = webResponse.GetResponseStream())
						{
							if (responseStream != null)
							{
								using (StreamReader streamReader = new StreamReader(responseStream))
								{
									while (!streamReader.EndOfStream)
									{
										response = await streamReader.ReadLineAsync();
										onData(response);
									}
									_log.Error($"!!!end of line stream ! retrying in another 5 seconds");
									Thread.Sleep(5000);
								}
							}
						}
					}
					catch (Exception exe)
					{
						_log.Error(exe);
					}
				
				}
			}
			catch (WebException caught)
			{
				throw new WebException(string.Format("Exception in WebServiceCall: {0}", caught.Message));
			}
			catch (Exception caught)
			{
				throw new Exception(string.Format("Exception in WebServiceCall: {0}", caught.Message));
			}
		}

	}
