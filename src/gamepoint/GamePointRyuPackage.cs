using Dargon.Robotics.Demo.Subsystems;
using Dargon.Robotics.DeviceRegistries;
using Dargon.Robotics.Devices;
using Dargon.Robotics.Subsystems.DriveTrain.Holonomic;
using Dargon.Robotics.Subsystems.DriveTrain.Vertical;
using Dargon.Ryu;
using Dargon.Ryu.Modules;

namespace Dargon.Robotics.Demo {
   public class GamePointRyuPackage : RyuModule {
      public GamePointRyuPackage() {
         Optional.Singleton<IterativeRobot>().Implements<IRobot>();
         Optional.Singleton<IterativeRobotConfiguration>(ConstructIterativeRobotConfiguration);
         Optional.Singleton<GamePointIterativeRobotUserCode>().Implements<IterativeRobotUserCode>();

         Optional.Singleton<Devices>(ConstructDevices);
         Optional.Singleton<HolonomicDriveTrain>(ryu => ryu.GetOrActivate<Devices>().DriveTrain);
      }

      private IterativeRobotConfiguration ConstructIterativeRobotConfiguration(IRyuContainer ryu) {
         return new IterativeRobotConfiguration(100);
      }

      private Devices ConstructDevices(IRyuContainer ryu) {
         var deviceRegistry = ryu.GetOrActivate<IDeviceRegistry>();
         var frontLeftMotor = deviceRegistry.GetDevice<IMotor>("Drive.Motors.FrontLeft");
         var frontRightMotor = deviceRegistry.GetDevice<IMotor>("Drive.Motors.FrontRight");
         var rearLeftMotor = deviceRegistry.GetDevice<IMotor>("Drive.Motors.RearLeft");
         var rearRightMotor = deviceRegistry.GetDevice<IMotor>("Drive.Motors.RearRight");
         var driveTrain = new HolonomicDriveTrain(frontLeftMotor, frontRightMotor, rearLeftMotor, rearRightMotor);
         return new Devices(driveTrain);
      }
   }
}