using System;
using Dargon.Robotics.Demo.Subsystems;
using Dargon.Robotics.DeviceRegistries;
using Dargon.Robotics.Devices;
using Dargon.Robotics.Subsystems.DriveTrain.Holonomic;
using Dargon.Robotics.Subsystems.DriveTrain.Vertical;


namespace Dargon.Robotics.Demo {
   public class GamePointIterativeRobotUserCode : IterativeRobotUserCode {
      private readonly IGamepad gamepad;
      private readonly HolonomicDriveTrain driveTrain;
      private readonly IPositionTracker positionTracker;
      private readonly IDeviceRegistry deviceRegistry;

      public GamePointIterativeRobotUserCode(IGamepad gamepad, HolonomicDriveTrain driveTrain, IPositionTracker positionTracker) {
         this.gamepad = gamepad;
         this.driveTrain = driveTrain;
         this.positionTracker = positionTracker;
         this.deviceRegistry = deviceRegistry;
      }

      public override void OnTick() {
         positionTracker.Update();
         Console.WriteLine(positionTracker.Position);

         if (gamepad.RightShoulder) {
            driveTrain.TankDrive(gamepad.LeftY, gamepad.RightY);
         } else if (gamepad.LeftShoulder) {
            driveTrain.MecanumDrive(gamepad.LeftX, gamepad.LeftY);
         } else {
            driveTrain.TankMecanumDriveHybrid(
               gamepad.LeftX,
               gamepad.LeftY,
               gamepad.RightX,
               0.2f);
         }
      }
   }
}