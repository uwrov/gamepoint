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
         Optional.Singleton<IPositionTracker>(ryu => ryu.GetOrActivate<Devices>().PositionTracker);
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

         var frontLeftIncrementalRotaryEncoder = deviceRegistry.GetDevice<IIncrementalRotaryEncoder>("Drive.Motors.FrontLeft.Encoder");
         var frontRightIncrementalRotaryEncoder = deviceRegistry.GetDevice<IIncrementalRotaryEncoder>("Drive.Motors.FrontRight.Encoder");
         var yawGyro = deviceRegistry.GetDevice<IGyroscope>("Drive.Gyroscopes.Yaw");

         var positionTracker = new TankDriveShaftEncodersAndYawGyroscopeBasedPositionTracker("Drive.PositionTracker", yawGyro, frontLeftIncrementalRotaryEncoder, frontLeftIncrementalRotaryEncoder, 5.0f * 0.0254f);
         positionTracker.Initialize();
         deviceRegistry.AddDevice(positionTracker.Name, positionTracker);

         var driveTrain = new HolonomicDriveTrain(frontLeftMotor, frontRightMotor, rearLeftMotor, rearRightMotor);
         return new Devices(driveTrain, positionTracker);
      }
   }
}