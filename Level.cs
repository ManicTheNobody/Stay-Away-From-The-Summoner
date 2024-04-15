using Godot;
using System;

public partial class Level : Node2D
{

	public int left, right, top, bottom;

	TileMap map;
	Camera2D camera;
	StaticBody2D stageBounds;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		map = GetNode<TileMap>("TileMap");
		camera = GetNode<Camera2D>("Camera2D");
		stageBounds = GetNode<StaticBody2D>("StageBounds");
		var leftBounds = stageBounds.GetNode<CollisionShape2D>("LeftBounds");
		var rightBounds = stageBounds.GetNode<CollisionShape2D>("RightBounds");
		var topBounds = stageBounds.GetNode<CollisionShape2D>("TopBounds");
		var bottomBounds = stageBounds.GetNode<CollisionShape2D>("BottomBounds");

		//Assume the tilemap is a fixed size at level ready time
		var mapRect = map.GetUsedRect();
		var mapTileSize = map.TileSet.TileSize;

		left = mapRect.Position.X * mapTileSize.X;
		right = (mapRect.Position.X + mapRect.Size.X) * mapTileSize.X;
		top = mapRect.Position.Y * mapTileSize.Y;
		bottom = (mapRect.Position.Y + mapRect.Size.Y) * mapTileSize.Y;

		var leftSegment = new SegmentShape2D();
		leftSegment.A = new Vector2(left, top);
		leftSegment.B = new Vector2(left, bottom);
		var rightSegment = new SegmentShape2D();
		rightSegment.A = new Vector2(right, top);
		rightSegment.B = new Vector2(right, bottom);
		var topSegment = new SegmentShape2D();
		topSegment.A = new Vector2(left, top);
		topSegment.B = new Vector2(right, top);
		var bottomSegment = new SegmentShape2D();
		bottomSegment.A = new Vector2(left, bottom);
		bottomSegment.B = new Vector2(right, bottom);
		
		leftBounds.Shape = leftSegment;
		rightBounds.Shape = rightSegment;
		topBounds.Shape = topSegment;
		bottomBounds.Shape = bottomSegment;

		camera.LimitLeft = left;
		camera.LimitRight = right;
		camera.LimitTop = top - 800;
		camera.LimitBottom = bottom;
		
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{

	}
}
