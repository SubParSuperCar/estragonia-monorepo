using System;
using CommunityToolkit.Mvvm.Input;
using GameTemplate.Main;

namespace GameTemplate.UI.ViewModels;

public partial class DialogViewModel : ViewModel
{
	public enum Response
	{
		// ReSharper disable InconsistentNaming
		Cancel = 0,
		Deny = 1,

		Confirm = 2
		// ReSharper restore InconsistentNaming
	}

	public DialogViewModel()
	{
	}

	public DialogViewModel(string message, string? cancelText = null, string? denyText = null,
		string? confirmText = null)
	{
		Message = message;
		CancelText = cancelText;
		DenyText = denyText;
		ConfirmText = confirmText;
	}

	public string Message { get; private set; } = "";
	public string? CancelText { get; }
	public string? DenyText { get; }
	public string? ConfirmText { get; }

	public event Action<Response>? Responded;

	public static void OpenDialog(UserInterface dialogUserInterface, FocusStack focusStack, DialogViewModel dialog,
		Action<Response> onResponse)
	{
		dialog.Responded += OnResponse;

		dialogUserInterface.MainViewModel?.NavigateTo(dialog);
		focusStack.Push(dialogUserInterface);
		dialog.Closed += _ => focusStack.Pop();
		return;

		void OnResponse(Response response)
		{
			dialog.Responded -= OnResponse;
			onResponse(response);
		}
	}

	[RelayCommand]
	public void ButtonResponse(int type)
	{
		Responded?.Invoke((Response)type);
		Close();
	}
}
