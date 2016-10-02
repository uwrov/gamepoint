using System;
using Dargon.Robotics.Demo.Subsystems;
using Dargon.Robotics.DeviceRegistries;
using Dargon.Robotics.Devices;
using Dargon.Robotics.Subsystems.DriveTrain.Holonomic;
using Dargon.Robotics.Subsystems.DriveTrain.Vertical;
using MathNet.Spatial.Euclidean;
using MathNet.Spatial.Units;


namespace Dargon.Robotics.Demo {
   public class GamePointIterativeRobotUserCode : IterativeRobotUserCode {
      private readonly IGamepad gamepad;
      private readonly HolonomicDriveTrain driveTrain;
      private readonly IGyroscope yawGyroscope;
      private readonly IPositionTracker positionTracker;
      private int i = 0;
      private Vector2D destination;

      public GamePointIterativeRobotUserCode(IGamepad gamepad, HolonomicDriveTrain driveTrain, Devices devices) {
         this.gamepad = gamepad;
         this.driveTrain = driveTrain;
         this.yawGyroscope = devices.YawGyroscope;
         this.positionTracker = devices.PositionTracker;
      }

      public override void OnTick() {
         positionTracker.Update();
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
               driveTrain.TankDrive((float)(speed / finalRatio), (float)(speed * finalRatio));
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