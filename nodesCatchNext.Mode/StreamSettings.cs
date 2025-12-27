namespace nodesCatchNext.Mode;

public class StreamSettings
{
	public string network { get; set; }

	public string security { get; set; }

	public TlsSettings tlsSettings { get; set; }

	public TcpSettings tcpSettings { get; set; }

	public KcpSettings kcpSettings { get; set; }

	public WsSettings wsSettings { get; set; }

	public HttpSettings httpSettings { get; set; }

	public QuicSettings quicSettings { get; set; }

	public TlsSettings xtlsSettings { get; set; }

	public GrpcSettings grpcSettings { get; set; }

	public RealitySettings realitySettings { get; set; }
}
