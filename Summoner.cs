using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;
using System.Data;
using System.Runtime.CompilerServices;

public partial class Summoner : CharacterBody2D
{
	[Signal]
	public delegate void SummonerStateTransitionEventHandler(string newState, string previousState, Node2D target);

	//Internal variables
	enum State{
		SEEKING,
		CHASING,
		WANDERING
	}
	System.Collections.Generic.Dictionary<State, List<State>> transitions;
	State activeState = State.WANDERING;
	int runSpeed = 480;
	int walkSpeed = 300;
	[Export]
	int seekRange = 1560;
	[Export]
	int chaseRange = 600;
	bool accelerationUnlocked = false;
	Node2D target;
	//External references
	Area2D summonsArea;
	WanderTarget wanderTarget;
	Node2D rays;
	Timer speedUpTimer;
	Player player;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
        transitions = new System.Collections.Generic.Dictionary<State, List<State>>{
			{State.SEEKING, new List<State>(){State.CHASING, State.WANDERING}},
			{State.WANDERING, new List<State>(){State.SEEKING, State.CHASING}},
			{State.CHASING, new List<State>(){State.WANDERING, State.SEEKING}}
		};
		wanderTarget = GetNode<WanderTarget>("WanderTarget");
		speedUpTimer = GetNode<Timer>("SpeedUpTimer");
		player = GetNode<Player>("/root/Level/Player");
		player.DecoySummoned += onDecoySummoned;
		target = player;
		summonsArea = GetNode<Area2D>("SummonsArea");
		summonsArea.BodyEntered += (body) => {
			if(body.GetMeta("IsPlayer", false).AsBool()){
				GetTree().ChangeSceneToFile("res://LoseScreen.tscn");
			}
		};
		rays = GetNode<Node2D>("Rays");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(double delta)
	{
		var targetGlobalPosition = target.GlobalPosition;
		var distanceToPlayer = Position.DistanceSquaredTo(player.GlobalPosition);
		var distanceToTarget = Position.DistanceSquaredTo(targetGlobalPosition);
		switch(activeState){
			case State.SEEKING:{
				if(distanceToPlayer < chaseRange*chaseRange + 1){
					switchState(State.CHASING);
					target = player;
					break;
				}
				if(distanceToTarget > Math.Pow(seekRange+100,2)){
					switchState(State.WANDERING);
					break;
				}
				steerToward(targetGlobalPosition, walkSpeed);
				break;
			}
			case State.CHASING:{
				if(distanceToTarget > chaseRange*chaseRange && !target.GetMeta("IsDecoy", false).AsBool()){
					switchState(State.SEEKING);
					break;
				}
				steerToward(targetGlobalPosition, runSpeed);
				break;
			}
			case State.WANDERING:{
				if(target!=wanderTarget){
					target = wanderTarget;
				}
				if(distanceToPlayer < seekRange*seekRange){
					switchState(State.SEEKING);
					target = player;
					break;
				}
				steerToward(targetGlobalPosition, walkSpeed);
				break;
			}
			default:
				break;
		}
	}

	public void beginSpeedup(){
		accelerationUnlocked = true;
		if(speedUpTimer.IsStopped()){
			speedUpTimer.Start();
		}
	}

	private void switchState(State desiredState){
		if(transitions[activeState].Contains(desiredState)){
			EmitSignal(SignalName.SummonerStateTransition, desiredState.ToString(), activeState.ToString(), target);
			activeState = desiredState;
		}
	}

	private void onDecoySummoned(Decoy decoy){
		decoy.OnVanished += onDecoyVanished;
		target = decoy;
		switchState(State.CHASING);
	}
	private void onDecoyVanished(){
		target = wanderTarget;
		switchState(State.WANDERING);
	}

	private void steerToward(Vector2 position, int maxSpeed){
		if(accelerationUnlocked)maxSpeed += (int)(100 * (1-speedUpTimer.TimeLeft/speedUpTimer.WaitTime));
		if(Position.DistanceSquaredTo(position)<100*100){
			return;
		}
		var desiredVelocity = Position.DirectionTo(position) * maxSpeed;
		var steering = (desiredVelocity - Velocity) / 20;
		

		rays.Rotation = Velocity.Angle();
		foreach(RayCast2D ray in rays.GetChildren()){
			ray.TargetPosition = new Vector2{X=Velocity.Length(),Y=ray.TargetPosition.Y};
			ray.ForceRaycastUpdate();
			if(ray.IsColliding()){
				var obstacle = (PhysicsBody2D)ray.GetCollider();
				steering += (Position + Velocity - obstacle.Position).Normalized() * 40;
				break;
			}
		}
		Velocity += steering;
		Velocity = Velocity.Normalized() * Math.Clamp(Velocity.Length(), 0, maxSpeed);
		MoveAndSlide();
	}
}
