using System;
using System.Drawing;
using Dargon.Robotics.Debug;
using Dargon.Robotics.Devices;
using Dargon.Robotics.Devices.DriveTrain.Holonomic;
using Dargon.Robotics.RollbackLogs;
using MathNet.Spatial.Euclidean;
using MathNet.Spatial.Units;

namespace Dargon.Robotics.GamePoint {
   public class GamePointIterativeRobotUserCode : IterativeRobotUserCode {
      private readonly IDebugRenderContext debugRenderContext;
      private readonly IGamepad gamepad;
      private readonly HolonomicDriveTrain driveTrain;
      private readonly IGyroscope yawGyroscope;
      private readonly IPositionTracker positionTracker;
      private readonly IMotionStateSnapshotLog motionLog;
      private int i = 0;
      private Vector2D destination;

      public GamePointIterativeRobotUserCode(IDebugRenderContext debugRenderContext, IGamepad gamepad, HolonomicDriveTrain driveTrain, GamePoint.Devices devices) {
         this.debugRenderContext = debugRenderContext;
         this.gamepad = gamepad;
         this.driveTrain = driveTrain;
         this.yawGyroscope = devices.YawGyroscope;
         this.positionTracker = devices.PositionTracker;
         this.motionLog = devices.MotionLog;
      }

      public override void OnTick() {
         positionTracker.Update();
         motionLog.Update();

         debugRenderContext.BeginScene();
         debugRenderContext.AddQuad(new DebugSceneQuad {
            Color = Color.Magenta,
            Extents = new Vector2D(0.25, 0.25),
            Position = positionTracker.Position,
            Rotation = yawGyroscope.GetAngle()
         });
         debugRenderContext.AddQuad(new DebugSceneQuad {
            Color = Color.Lime,
            Extents = new Vector2D(0.25, 0.25),
            Position = destination,
            Rotation = 0
         });
         var lastSnapshotTime = DateTime.Now;
         foreach (var entry in motionLog.EnumerateSnapshotEntries()) {
            if (Math.Abs((lastSnapshotTime - entry.Timestamp).TotalSeconds) > 0.3) {
               lastSnapshotTime = entry.Timestamp;
               debugRenderContext.AddQuad(new DebugSceneQuad {
                  Color = Color.LightCoral,
                  Extents = new Vector2D(0.10, 0.10),
                  Position = entry.Snapshot.Position,
                  Rotation = entry.Snapshot.Yaw
               });
            } else {
               continue;
            }
         }
         debugRenderContext.EndScene();

//         Console.WriteLine(positionTracker.Position);
         if (!gamepad.LeftShoulder) {
            driveTrain.TankDrive(gamepad.LeftY, gamepad.RightY);
         }

         //         driveTrain.TankDrive(gamepad.LeftY, gamepad.RightY);
         //         if (i++ % 200 < 5) {
         //            driveTrain.TankDrive(1.0f, -1.0f);
         //         } else if (i % 200 < 10) {
         //            driveTrain.TankDrive(1.0f, -0.5f);
         //         } else {
         {
            if (gamepad.RightShoulder) {
               destination = positionTracker.Position + new Vector2D(1, 2).Rotate(Angle.FromRadians(yawGyroscope.GetAngle()));
            }

            if (gamepad.Start) {
               destination = positionTracker.Position + new Vector2D(5, 2).Rotate(Angle.FromRadians(yawGyroscope.GetAngle()));
            }

            var desiredLookat = (destination - positionTracker.Position).Rotate(Angle.FromRadians(-yawGyroscope.GetAngle()));
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
               if (max >1.0) {
                  left /= max;
                  right /= max;
               }
               driveTrain.TankDrive(left, right);
            }

//            Console.WriteLine(positionTracker.Position + " " + yawGyroscope.GetAngle() + " " + desiredLookatNorm + " " + currentLookatNorm + " " + ratioChange + " " + (speed / finalRatio) + " " + (speed * finalRatio));
         }


         //         }

//         if (gamepad.RightShoulder) {
//            driveTrain.TankDrive(gamepad.LeftY, gamepad.RightY);
//         } else if (gamepad.LeftShoulder) {
//            driveTrain.MecanumDrive(gamepad.LeftX, gamepad.LeftY);
//         } else {
//            driveTrain.TankMecanumDriveHybrid(
//               gamepad.LeftX,
//               gamepad.LeftY,
//               gamepad.RightX,
//               0.2f);
//         }
      }
   }
}