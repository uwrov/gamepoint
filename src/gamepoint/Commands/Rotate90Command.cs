using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dargon.Robotics.Devices;
using Dargon.Robotics.GamePoint.Subsystems;
using Dargon.Robotics.Subsystems.DriveTrain.Holonomic;
using Dargon.Ryu.Attributes;
using MathNet.Spatial.Euclidean;

namespace Dargon.Robotics.GamePoint.Commands {
   [InjectRequiredFields]
   public class Rotate90Command : ICommand {
      private readonly IGamepad gamepad;
      private readonly HolonomicDriveTrain driveTrain;
      private readonly IGyroscope yawGyroscope;
      private float destAngle;

      public bool IsExecutable => true;

      public bool IsPassive => false;

      public bool IsTriggered => gamepad.A;

      public bool IsForceTriggered => false;

      public int Subsystem => (int)SubsystemFlags.Drive;

      public void Start() {
         float startAngle = yawGyroscope.GetAngle();
         destAngle = startAngle - (float)Math.PI / 2.0f;
         Console.WriteLine(destAngle * 180.0f / (float)Math.PI);
      }

      public CommandStatus RunIteration() {
         float currentAngle = yawGyroscope.GetAngle();
         float angularDistance = currentAngle - destAngle;
         if (currentAngle > destAngle - 0.1 && currentAngle < destAngle + 0.1) {
            driveTrain.SetValues(0.0f, 0.0f, 0.0f, 0.0f);
            return CommandStatus.Complete;
         }
         
         driveTrain.SetValues(0.5f, -0.5f, 0.5f, -0.5f);
         return CommandStatus.Continue;
      }

      public void Cancel() {
         driveTrain.SetValues(0.0f, 0.0f, 0.0f, 0.0f);
      }
   }
}