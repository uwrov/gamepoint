using System;
using System.Threading;
using Dargon.Commons;
using Dargon.Robotics;
using Dargon.Robotics.DebugScene;
using Dargon.Robotics.DeviceRegistries;
using Dargon.Robotics.Devices;
using Dargon.Robotics.Simulations2D;
using Dargon.Robotics.Simulations2D.Devices;
using Dargon.Ryu;
using Dargon.Ryu.Modules;

namespace demo_robot_simulator {
   public class Program {
      private const float kMecanumWheelForceAngle = (float)(Math.PI / 4);
      private const float kMecanumWheelForceMagnitude = 50 * 1000.0f;
      private const float kNewtonMetersPerOzInch = 0.00706155183333f;
      private const float kMotorTorque = 2.42f; //50 * kNewtonMetersPerOzInch;
      private const float kMetersPerInch = 0.0254f;
      private const float kWheelRadius = 2 * kMetersPerInch;
      private const float kWheelForce = kMotorTorque / kWheelRadius;

      public static void Main(string[] args) {
         // create simulation state
         var constants = SimulationConstantsFactory.WaterRobot();
         var motors = SimulationMotorStateFactory.RovDrive(constants.WidthMeters, constants.HeightMeters, kMecanumWheelForceAngle, kWheelForce);
         var wheelEncoders = SimulationWheelShaftEncoderStateFactory.FromMotors(motors, kWheelRadius, 128);
         var yawGyro = new SimulationGyroscopeState("Drive.Gyros.Yaw");
         var robot = new SimulationRobotState(constants.WidthMeters, constants.HeightMeters, constants.Density, motors, wheelEncoders, yawGyro);
         var robotEntity = new SimulationRobotEntity(constants, robot);

         // create robot state
         var deviceRegistry = new DefaultDeviceRegistry();
         foreach (var simulationMotorState in robot.MotorStates) {
            var motor = new SimulationMotorAdapter(simulationMotorState);
            motor.Initialize();
            deviceRegistry.AddDevice(motor.Name, motor);
            motor.Set(0.1f);
         }
         deviceRegistry.AddDevice("Arm.Servos.Wrist", new NullServo("Arm.Servos.Wrist"));
         deviceRegistry.AddDevice("Arm.Servos.LeftClaw", new NullServo("Arm.Servos.LeftClaw"));
         deviceRegistry.AddDevice("Arm.Servos.RightClaw", new NullServo("Arm.Servos.RightClaw"));
         deviceRegistry.AddDevice("Arm.Servos.Elbow", new NullServo("Arm.Servos.Elbow"));
         deviceRegistry.AddDevice("Arm.Servos.InOut", new NullServo("Arm.Servos.InOut"));

         // Debugscene Stuff
         var debugRenderContext = new DebugRenderContext();

         // start robot code in new thread
         new Thread(() => {
            var ryuConfiguration = new RyuConfiguration();
            ryuConfiguration.AdditionalModules.Add(new RyuModule().With(m => {
               m.Required.Singleton<KeyboardGamepad>().Implements<IGamepad>();
               m.Required.Singleton<IDeviceRegistry>(x => deviceRegistry);
               m.Required.Singleton<IDebugRenderContext>(x => debugRenderContext);
            }));

            var ryu = new RyuFactory().Create(ryuConfiguration);
            ryu.GetOrActivate<IRobot>().Run();
         }).Start();

         new Simulation2D(robotEntity).Run();
      }
   }
}
