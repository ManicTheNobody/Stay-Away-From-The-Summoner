using Godot;
using System;

public partial class LoseScreen : Control
{
	Button tryAgainButton;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		tryAgainButton = GetNode<Button>("PanelContainer/VBoxContainer/TryAgainButton");
		tryAgainButton.ButtonDown += () =>{
			GetTree().ChangeSceneToFile("res://Level.tscn");
		};
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
