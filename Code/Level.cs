using Godot;
using System;
using System.Collections.Generic;
using GA.Common;

namespace GA.GameProgramming
{
	public partial class Level : Node2D
	{
		#region Statics
		public static Level Current
		{
			get;
			private set;
		}
		#endregion Statics

		[Export] private Paddle _paddle = null;
		[Export] private Ball _ball = null;
		[Export] private int _wallWidth = 0;
		public Paddle Paddle { get { return _paddle; } }
		public Ball Ball { get { return _ball; } }
		public int WallWidth { get { return _wallWidth; } }

		public Vector2 WindowSize
		{
			get
			{
				return GetViewport().GetVisibleRect().Size;
			}
		}
		public override void _Ready()
		{
			Current = this;

			if (_paddle == null)
			{
				_paddle = this.GetNode<Paddle>();
				// Same as this
				// _paddle = NodeExtensions.GetNode<Paddle>(this);
			}

			if (_ball == null)
			{
				_ball = this.GetNode<Ball>("Ball");
			}
			
			/*
			IList<Ball> balls = this.GetNodes<Ball>(recursive:true);
			GD.Print($"Found {balls.Count} balls.");
			*/
		}

        public override void _Input(InputEvent @event)
        {
            base._Input(@event);
			     
			if (@event.IsActionPressed("TestScore"))
			{
				GameManager.Instance.AddScore(10);
			}
        }
	}
}
