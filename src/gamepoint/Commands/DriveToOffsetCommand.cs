﻿using Dargon.Robotics.Devices;
using Dargon.Robotics.GamePoint.Subsystems;
using Dargon.Robotics.Subsystems.DriveTrain.Holonomic;
using Dargon.Ryu.Attributes;
using System;
using System.Drawing;
using Dargon.Robotics.Debug;
using MathNet.Spatial.Euclidean;
using MathNet.Spatial.Units;

namespace Dargon.Robotics.GamePoint.Commands {
   [InjectRequiredFields]
   public class DriveToOffsetCommand : ICommand {
      private readonly IDebugRenderContext debugRenderContext;
      private readonly IGamepad gamepad;
      private readonly HolonomicDriveTrain driveTrain;
      private readonly IGyroscope yawGyroscope;
      private readonly IPositionTracker positionTracker;
      private Vector2D destination;

      public bool IsExecutable => true;
      public bool IsPassive => false;
      public bool IsTriggered => gamepad.LeftShoulder;
      public bool IsForceTriggered => false;
      public int Subsystem => (int)SubsystemFlags.Drive;
      public void Start() {
         positionTracker.Update();
         destination = positionTracker.Position + new Vector2D(5, 2).Rotate(Angle.FromRadians(yawGyroscope.GetAngle()));
      }

      public CommandStatus RunIteration() {
         positionTracker.Update();

         debugRenderContext.AddQuad(new DebugSceneQuad {
            Color = Color.Orange,
            Extents = new Vector2D(0.25, 0.25),
            Position = positionTracker.Position,
            Rotation = yawGyroscope.GetAngle()
         });
         debugRenderContext.AddQuad(new DebugSceneQuad {
            Color = Color.Red,
            Extents = new Vector2D(0.25, 0.25),
            Position = destination,
            Rotation = 0
         });
         var offsetToDestination = destination - positionTracker.Position;
         var desiredLookat = offsetToDestination.Rotate(Angle.FromRadians(-yawGyroscope.GetAngle()));
         var desiredLookatNorm = desiredLookat.Normalize();
         var currentLookatNorm = new Vector2D(0, 1);

         // desired x current = Sin-1(axby-aybx) ~~ axby-aybx for small angle
         var theta = desiredLookatNorm.X * currentLookatNorm.Y -
                     desiredLookatNorm.Y * currentLookatNorm.X;
         var ratioChange = theta;
         if (double.IsNaN(ratioChange)) {
            ratioChange = 0.0;
         }
         var ratio = 1.0f;
         var finalRatio = ratio - ratioChange;

         var speed = 1.0f;
         if (gamepad.LeftShoulder) {
            var left = (float)(speed / finalRatio);
            var right = (float)(speed * finalRatio);
            var max = Math.Max(left, right);
            if (max > 1.0) {
               left /= max;
               right /= max;
            }
            driveTrain.TankDrive(left, right);
         }
         return CommandStatus.Continue;
      }

      public void Cancel() => driveTrain.Halt();
   }
}
