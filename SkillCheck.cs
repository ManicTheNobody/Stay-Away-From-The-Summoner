using Godot;
using System;

public partial class SkillCheck : Node2D
{
	//Signals
	[Signal]
	public delegate void SkillCheckCompletedEventHandler(bool wasSuccess);
	
	
	public int difficulty = 0;
	//Internal Variables
	int sliderDirection = -1;
	int[] sliderSpeeds = new int[]{500, 750, 1000};
	bool sliderInTarget = false;
	Vector2 sliderStartPosition;
	//External references
	Area2D range;
	Area2D target;
	Area2D slider;
	Timer startupDelay;

	AudioStreamPlayer2D failureSFX;
	AudioStreamPlayer2D successSFX;
	AudioStreamPlayer2D bounceSFX;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		range = GetNode<Area2D>("Range");
		target = GetNode<Area2D>("Range/Target");
		slider = GetNode<Area2D>("Slider");
		sliderStartPosition = slider.Position;
		startupDelay = GetNode<Timer>("StartupDelay");
		failureSFX = GetNode<AudioStreamPlayer2D>("SkillCheckFailedSFX");
		successSFX = GetNode<AudioStreamPlayer2D>("SkillCheckSucceededSFX");
		bounceSFX = GetNode<AudioStreamPlayer2D>("SkillCheckSliderBounceSFX");

		setRandomTarget();
		
		target.AreaEntered+=onTargetEntered;
		target.AreaExited+=onTargetExited;

		range.AreaExited+=onRangeExited;

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		difficulty = Math.Clamp(difficulty, 0, sliderSpeeds.Length-1);
		var velocity = new Vector2((float)(sliderDirection * sliderSpeeds[difficulty] * delta), 0);
		slider.Position += velocity;
		if(Input.IsActionJustPressed("Interact") && startupDelay.IsStopped()){
			if(sliderInTarget){
				successSFX.Play();
			}
			else{
				failureSFX.Play();
			}
			EmitSignal(SignalName.SkillCheckCompleted, sliderInTarget);
			reset();
		}
	}

	private void onTargetEntered(Area2D area){
		sliderInTarget = true;
	}
	private void onTargetExited(Area2D area){
		sliderInTarget = false;
	}
	private void onRangeExited(Area2D area){
		sliderDirection = area.Position.X < range.Position.X ? 1 : -1;
		if(Visible){
			bounceSFX.Play();
		}
	}
	private void reset(){
		Visible = false;
		sliderDirection = -1;
		slider.Position = sliderStartPosition;
		setRandomTarget();
		startupDelay.Start();
		ProcessMode = ProcessModeEnum.Disabled;
	}
	private void setRandomTarget(){
		var rangeRect = range.GetNode<CollisionShape2D>("RangeCollider").Shape.GetRect();
		var targetRect = target.GetNode<CollisionShape2D>("TargetCollider").Shape.GetRect();
		target.Position = new Vector2((rangeRect.Size.X - targetRect.Size.X) * GD.Randf(),target.Position.Y);
	}
}
