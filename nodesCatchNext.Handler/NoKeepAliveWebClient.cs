using System;
using System.Net;

namespace nodesCatchNext.Handler;

internal class NoKeepAliveWebClient : WebClient
{
	protected override WebRequest GetWebRequest(Uri address)
	{
		WebRequest webRequest = base.GetWebRequest(address);
		if (webRequest is HttpWebRequest httpWebRequest)
		{
			httpWebRequest.KeepAlive = false;
		}
		return webRequest;
	}
}
