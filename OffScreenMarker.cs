using Godot;
using System;

public partial class OffScreenMarker : Node2D
{
	// Called when the node enters the scene tree for the first time.
	Vector2 initialOffset = Vector2.Zero;
	Camera2D camera;
	Sprite2D sprite;
	public override void _Ready()
	{
		camera = GetNode<Camera2D>("/root/Level/Camera2D");
		sprite = GetNode<Sprite2D>("Marker");
		initialOffset = Position;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		Position = initialOffset;
		var upperLeft = camera.GetScreenCenterPosition() - (camera.GetViewportRect().Size / 2);
		var lowerRight = camera.GetScreenCenterPosition() + (camera.GetViewportRect().Size / 2);

		if(new Rect2(upperLeft, camera.GetViewportRect().Size).HasPoint(GlobalPosition)){
			Hide();
		}
		else{
			Show();
		}

		GlobalPosition = GlobalPosition.Clamp(upperLeft, lowerRight);
		sprite.GlobalRotation = (sprite.GlobalPosition - camera.GetScreenCenterPosition()).Angle();
	}
}
