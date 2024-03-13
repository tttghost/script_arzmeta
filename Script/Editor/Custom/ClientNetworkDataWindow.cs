using UnityEngine;
using UnityEditor;

public class ClientNetworkDataWindow : EditorWindow
{
	[MenuItem( "클라이언트팀/네트워크 주소 설정", priority = 1 )]
	public static void ShowWindow()
	{
		ClientNetworkDataWindow window = GetWindow<ClientNetworkDataWindow>( false, "클라이언트 네트워크 설정", true );
	}

	private ClientNetworkData clientNetworkDataInstance = default;

	public ClientNetworkData clienteNetworkData
	{
		get
		{
			if ( clientNetworkDataInstance == null )
				clientNetworkDataInstance = ClientNetworkData.Load();

			return clientNetworkDataInstance;
		}
	}

    private bool isCloud = true;
    private string ip_realtime = "";

	private void OnGUI()
	{
		CommonFunctionEditor.DrawUILine( Color.gray );

        isCloud = GUILayout.Toggle(clienteNetworkData.isCloud, "실시간 클라우드 접속");
        if ( isCloud != clienteNetworkData.isCloud )
		{
			clienteNetworkData.isCloud = isCloud;
		}

		GUILayout.Space( 10 );

		if (!isCloud)
		{
			ip_realtime = CommonFunctionEditor.SetStringField("실시간 내부서버 IP", clienteNetworkData.INNTER_IP_REALTIME);
			if (ip_realtime.Equals(clienteNetworkData.INNTER_IP_REALTIME) == false)
			{
				clienteNetworkData.INNTER_IP_REALTIME = ip_realtime;
			}
		}

		CommonFunctionEditor.DrawUILine( Color.gray );

		GUILayout.Space( 10 );
	}
}
