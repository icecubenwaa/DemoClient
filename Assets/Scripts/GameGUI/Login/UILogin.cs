﻿
using UnityEngine;
using System.Collections;
using System;
using Protocol;

public class UILogin : GTWindow
{
	private GameObject       state2;
	private GameObject       state3;

	private GameObject       btnNotice;
	private GameObject       btnLoginGame;
	private GameObject       btnServer;

	private GameObject       btnAccount;
	private GameObject       btnAccountLogin;

	private UILabel          versionNumber;
	private UILabel          curServerName;
	private UILabel          curAccount;
	private LoginViewMode    loginMode;

	public enum LoginViewMode
	{
		State2,
		State3,
	}

	public UILogin()
	{
		Resident = false;
		Path = "Login/UILogin";
		Type = EWindowType.Window;
		MaskType = EWindowMaskType.None;
	}

	protected override void OnAwake()
	{
		state2 = transform.Find("State2").gameObject;
		state3 = transform.Find("State3").gameObject;

		btnAccount = state2.transform.Find("Btn_Account").gameObject;
		btnAccountLogin = state2.transform.Find("Btn_AccountLogin").gameObject;

		btnNotice = state3.transform.Find("Btn_Notice").gameObject;
		btnLoginGame = state3.transform.Find("Btn_LoginGame").gameObject;
		btnServer  = state3.transform.Find("Btn_Server").gameObject;

		curServerName = btnServer.transform.Find("Label").GetComponent<UILabel>();
		curAccount  = btnAccount.transform.Find("Label").GetComponent<UILabel>();
		versionNumber = transform.Find("Bottom/Version").GetComponent<UILabel>();
	}

	protected override void OnAddButtonListener()
	{
		UIEventListener.Get(btnAccount).onClick         = OnAccountClick;
		UIEventListener.Get(btnAccountLogin).onClick    = OnAccountLoginClick;
		UIEventListener.Get(btnNotice).onClick          = OnNoticeClick;
		UIEventListener.Get(btnLoginGame).onClick       = OnLoginGameClick;
		UIEventListener.Get(btnServer).onClick          = OnServerClick;
	}



	protected override void OnAddHandler()
	{
		GTEventCenter.AddHandler(GTEventID.TYPE_LOGIN_SELECTSERVER,       OnRecvLoginGame);
		GTEventCenter.AddHandler(GTEventID.TYPE_LOGIN_ACCOUNT_REGISTER,   OnRecvRegister);
		GTEventCenter.AddHandler(GTEventID.TYPE_LOGIN_ACCOUNT_LOGIN,      OnRecvAccLogin);
		GTEventCenter.AddHandler(GTEventID.TYPE_LOGIN_GETSERVERLIST,      OnRecvGetServers);
		GTEventCenter.AddHandler(GTEventID.TYPE_LOGIN_SELECTSERVER,       OnRecvSelectServer);
		GTEventCenter.AddHandler(GTEventID.TYPE_LOGIN_SET_ACCOUNT,        ShowUsernameAndPassword);

	}

	protected override void OnDelHandler()
	{
		GTEventCenter.DelHandler(GTEventID.TYPE_LOGIN_SELECTSERVER,       OnRecvLoginGame);
		GTEventCenter.DelHandler(GTEventID.TYPE_LOGIN_ACCOUNT_REGISTER,   OnRecvRegister);
		GTEventCenter.DelHandler(GTEventID.TYPE_LOGIN_ACCOUNT_LOGIN,      OnRecvAccLogin);
		GTEventCenter.DelHandler(GTEventID.TYPE_LOGIN_GETSERVERLIST,      OnRecvGetServers);
		GTEventCenter.DelHandler(GTEventID.TYPE_LOGIN_SELECTSERVER,       OnRecvSelectServer);
		GTEventCenter.DelHandler(GTEventID.TYPE_LOGIN_SET_ACCOUNT,        ShowUsernameAndPassword);
	}

	protected override void OnEnable()
	{
		this.loginMode = LoginViewMode.State2;
		this.InitView();
		this.InitGameModeView();
		this.ShowCurrServer();
		this.ShowUsernameAndPassword();
	}

	protected override void OnClose()
	{

	}

	private void InitView()
	{
		versionNumber.text = GTTools.Format("版本号：{0}", Application.version);
		GTXmlHelper.CanWrite = false;
	}

	private void InitGameModeView()
	{
		state2.SetActive(loginMode == LoginViewMode.State2);
		state3.SetActive(loginMode == LoginViewMode.State3);

		btnNotice. SetActive(true);
		btnServer. SetActive(true);
	}

	private void OnServerClick(GameObject go)
	{
		GTAudioManager.Instance.PlayEffectAudio(GTAudioKey.SOUND_UI_CLICK);
		GTNetworkSend.Instance.TryGetSvrList();
	}

	private void OnLoginGameClick(GameObject go)
	{
		GTAudioManager.Instance.PlayEffectAudio(GTAudioKey.SOUND_UI_CLICK);
		ClientServerNode node = MLLogin.Instance.GetCurrServer();
		GTNetworkSend.Instance.TrySelectServer(node == null ? 0 : node.SvrID);
	}

	private void OnAccountClick(GameObject go)
	{
		GTAudioManager.Instance.PlayEffectAudio(GTAudioKey.SOUND_UI_CLICK);
		GTWindowManager.Instance.OpenWindow(EWindowID.UIAccount);
	}

	private void OnAccountLoginClick(GameObject go)
	{
		GTAudioManager.Instance.PlayEffectAudio(GTAudioKey.SOUND_UI_CLICK);

		if (string.IsNullOrEmpty(MLLogin.Instance.LastUsername))
		{
			GTWindowManager.Instance.OpenWindow(EWindowID.UIAccount);
			return;
		}

		//开始连接服务器
		NetworkManager.Instance.ConnectServer(GTLauncher.Instance.LoginIP, GTLauncher.Instance.LoginPort, () =>
		{
			GTNetworkSend.Instance.TryLogin(MLLogin.Instance.LastUsername, MLLogin.Instance.LastPassword);
		});
	}

	private void OnNoticeClick(GameObject go)
	{
		GTAudioManager.Instance.PlayEffectAudio(GTAudioKey.SOUND_UI_CLICK);
		GTWindowManager.Instance.OpenWindow(EWindowID.UINotice);
	}

	private void OnRecvLoginGame()
	{
		this.Hide();
	}

	private void OnRecvRegister()
	{
		this.ShowUsernameAndPassword();
	}

	private void OnRecvAccLogin()
	{
		this.loginMode = LoginViewMode.State3;
		this.InitGameModeView();
		this.ShowCurrServer();
	}

	private void OnRecvGetServers()
	{
		GTWindowManager.Instance.OpenWindow(EWindowID.UIServer);
	}

	private void OnRecvSelectServer()
	{
		ShowCurrServer();
	}

	public void ShowCurrServer()
	{
		ClientServerNode data = MLLogin.Instance.GetCurrServer();
		curServerName.text = data == null ? "服务器" : data.SvrName;
	}

	public void ShowUsernameAndPassword()
	{
		string username = MLLogin.Instance.LastUsername;
		curAccount.text = string.IsNullOrEmpty(username) ? "帐号" : username;
	}
}
