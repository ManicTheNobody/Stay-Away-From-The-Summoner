using Godot;
using System;

public partial class TypewriterExplain : Label
{
	Timer timer;
	AudioStreamPlayer2D keySound;
	AudioStreamPlayer2D keySound2;
	AudioStreamPlayer2D keySound3;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		keySound = GetNode<AudioStreamPlayer2D>("TypewriterClack");
		keySound2 = GetNode<AudioStreamPlayer2D>("TypewriterClack2");
		keySound3 = GetNode<AudioStreamPlayer2D>("TypewriterClack3");
		timer = GetNode<Timer>("CharacterTimer");
		timer.Timeout += processTimeout;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	public void processTimeout(){
		if(VisibleCharacters < Text.Length){
			VisibleCharacters++;
			if(Text.Substr(VisibleCharacters+1, 1) == "."){
				timer.WaitTime = 0.45;
			}
			else{
				timer.WaitTime = 0.15;
			}
			if(Text.Substr(VisibleCharacters, 1) == "."){
				keySound2.Play();
				return;
			}
			switch(GD.RandRange(0,2)){
				case 0:
					keySound.Play();
					break;
				case 1:
					keySound2.Play();
					break;
				case 2:
					keySound3.Play();
					break;
				
			}
		}
	}
}
