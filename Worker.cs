using Godot;
using System;

public partial class Worker : CharacterBody2D
{
	[Signal]
	public delegate void ResourceMineStartedEventHandler();

	//External references
	Timer harvestTimer;
	AudioStreamPlayer2D successSound;
	AudioStreamPlayer2D persistentSound;
	bool shunted = false;

    public override void _Ready()
    {
        harvestTimer = GetNode<Timer>("ResourceHarvestTimer");
		harvestTimer.Timeout += handleHarvestTimeout;
		successSound = GetNode<AudioStreamPlayer2D>("MiningSuccessSFX");
		persistentSound = GetNode<AudioStreamPlayer2D>("MiningPersistentSFX");
    }
    public override void _Process(double delta)
    {
		if(!shunted){
			Velocity = Vector2.Down;
			MoveAndSlide();
			Velocity = Vector2.Zero;
			shunted = true;
		}
    }
	public void HandleResourceDepleted(){
		QueueFree();
	}
	private void handleHarvestTimeout(){
		EmitSignal(SignalName.ResourceMineStarted);
		successSound.Play();
	}
}
