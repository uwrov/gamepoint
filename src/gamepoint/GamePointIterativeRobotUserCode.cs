using Dargon.Robotics.Demo.Subsystems;
using Dargon.Robotics.Devices;
using Dargon.Robotics.Subsystems.DriveTrain.Holonomic;
using Dargon.Robotics.Subsystems.DriveTrain.Vertical;


namespace Dargon.Robotics.Demo {
   public class GamePointIterativeRobotUserCode : IterativeRobotUserCode {
      private readonly IGamepad gamepad;
      private readonly HolonomicDriveTrain driveTrain;

      public GamePointIterativeRobotUserCode(IGamepad gamepad, HolonomicDriveTrain driveTrain) {
         this.gamepad = gamepad;
         this.driveTrain = driveTrain;
      }

      public override void OnTick() {
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