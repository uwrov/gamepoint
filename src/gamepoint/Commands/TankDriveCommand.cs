using Dargon.Robotics.Devices;
using Dargon.Robotics.GamePoint.Subsystems;
using Dargon.Robotics.Subsystems.DriveTrain.Holonomic;
using Dargon.Ryu.Attributes;
using System;
using Dargon.Robotics.Subsystems.DriveTrain.Tank;

namespace Dargon.Robotics.GamePoint.Commands {
   [InjectRequiredFields]
   public class TankDriveCommand : ICommand {
      private readonly IGamepad gamepad;
      //private readonly HolonomicDriveTrain driveTrain;
      private readonly TankDriveTrain driveTrain;

      public bool IsExecutable => true;
      public bool IsPassive => true;
      public bool IsTriggered => false;
      public bool IsForceTriggered => Math.Abs(gamepad.LeftY) > 0.5 || Math.Abs(gamepad.RightY) > 0.5;
      public int Subsystem => (int)SubsystemFlags.Drive;
      public void Start() { }

      public CommandStatus RunIteration() {
         //driveTrain.TankDrive(gamepad.LeftY, gamepad.RightY, true);
         driveTrain.SetValues(gamepad.LeftY, gamepad.RightY);
         return CommandStatus.Continue;
      }

      public void Cancel() => driveTrain.SetValues(0.0f, 0.0f);
   }
}
