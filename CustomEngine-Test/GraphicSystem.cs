using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Threading;

namespace CustomEngine_Test
{
    internal class GraphicSystem
    {
        private static PerspectiveCamera _camera;
        private static Vector3D _movement = new Vector3D(0, 0, 0);
        private const double Displacement = 0.1;

        public static void Run()
        {
            // Create a new thread
            Thread newWindowThread = new Thread(new ThreadStart(() =>
            {
                // Create our context, and install it:
                SynchronizationContext.SetSynchronizationContext(
                    new DispatcherSynchronizationContext(
                        Dispatcher.CurrentDispatcher));

                // Create and show the Window
                Window wnd = new Window
                {
                    // Define the window basic properties
                    Width = 800,
                    Height = 600,
                    Title = "CustomEngine"
                };
                wnd.KeyDown += Wnd_KeyDown;


                // Create a viewport for 3D rendering
                Viewport3D viewport = new Viewport3D();

                // Create a 3D cube (a complete 3D mesh)
                MeshGeometry3D cubeMesh = new MeshGeometry3D();
                cubeMesh.Positions.Add(new Point3D(0, 0, 0)); // bottom front left
                cubeMesh.Positions.Add(new Point3D(1, 0, 0)); // bottom front right
                cubeMesh.Positions.Add(new Point3D(1, 0, 1)); // bottom back right
                cubeMesh.Positions.Add(new Point3D(0, 0, 1)); // bottom back left
                cubeMesh.Positions.Add(new Point3D(0, 1, 0)); // top front left
                cubeMesh.Positions.Add(new Point3D(1, 1, 0)); // top front right
                cubeMesh.Positions.Add(new Point3D(1, 1, 1)); // top back right
                cubeMesh.Positions.Add(new Point3D(0, 1, 1)); // top back left

                // Front face
                cubeMesh.TriangleIndices.Add(0);
                cubeMesh.TriangleIndices.Add(1);
                cubeMesh.TriangleIndices.Add(5);
                cubeMesh.TriangleIndices.Add(0);
                cubeMesh.TriangleIndices.Add(5);
                cubeMesh.TriangleIndices.Add(4);

                // Right face
                cubeMesh.TriangleIndices.Add(1);
                cubeMesh.TriangleIndices.Add(2);
                cubeMesh.TriangleIndices.Add(6);
                cubeMesh.TriangleIndices.Add(1);
                cubeMesh.TriangleIndices.Add(6);
                cubeMesh.TriangleIndices.Add(5);

                // Back face
                cubeMesh.TriangleIndices.Add(2);
                cubeMesh.TriangleIndices.Add(3);
                cubeMesh.TriangleIndices.Add(7);
                cubeMesh.TriangleIndices.Add(2);
                cubeMesh.TriangleIndices.Add(7);
                cubeMesh.TriangleIndices.Add(6);

                // Left face
                cubeMesh.TriangleIndices.Add(3);
                cubeMesh.TriangleIndices.Add(0);
                cubeMesh.TriangleIndices.Add(4);
                cubeMesh.TriangleIndices.Add(3);
                cubeMesh.TriangleIndices.Add(4);
                cubeMesh.TriangleIndices.Add(7);

                // Top face
                cubeMesh.TriangleIndices.Add(4);
                cubeMesh.TriangleIndices.Add(5);
                cubeMesh.TriangleIndices.Add(6);
                cubeMesh.TriangleIndices.Add(4);
                cubeMesh.TriangleIndices.Add(6);
                cubeMesh.TriangleIndices.Add(7);

                // Bottom face
                cubeMesh.TriangleIndices.Add(3);
                cubeMesh.TriangleIndices.Add(2);
                cubeMesh.TriangleIndices.Add(1);
                cubeMesh.TriangleIndices.Add(3);
                cubeMesh.TriangleIndices.Add(1);
                cubeMesh.TriangleIndices.Add(0);

                // Create a material to apply to the cube
                Material material = new DiffuseMaterial(new SolidColorBrush(Colors.Green));

                // Create a geometry model with the cube mesh and material
                GeometryModel3D cubeModel = new GeometryModel3D(cubeMesh, material);

                // Create a model visual containing the geometry model, and add it to the viewport
                ModelVisual3D modelVisual = new ModelVisual3D
                {
                    Content = cubeModel
                };
                viewport.Children.Add(modelVisual);

                // Add the viewport to the window
                wnd.Content = viewport;

                // Add a light source so the cube can be seen
                DirectionalLight directionalLight = new DirectionalLight
                {
                    Color = Colors.White,
                    Direction = new Vector3D(-1.0, -1.0, -1.0)
                };
                ModelVisual3D lightModel = new ModelVisual3D
                {
                    Content = directionalLight
                };
                viewport.Children.Add(lightModel);

                // Create a rotation transform to rotate the cube
                RotateTransform3D rotateTransform = new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(0, 1, 0), 0), new Point3D(0.5, 0.5, 0.5));

                cubeModel.Transform = rotateTransform;

                // Create a timer to rotate the cube
                DispatcherTimer timer = new DispatcherTimer
                {
                    Interval = TimeSpan.FromMilliseconds(10)
                };
                timer.Tick += (sender, e) =>
                {
                    ((AxisAngleRotation3D)rotateTransform.Rotation).Angle += 1;
                    if (((AxisAngleRotation3D)rotateTransform.Rotation).Angle > 360)
                    {
                        ((AxisAngleRotation3D)rotateTransform.Rotation).Angle = 0;
                    }
                };
                timer.Start();

                // Center the pyramid in the window
                // Adjust the camera's position to zoom out and fully display the pyramid
                _camera = new PerspectiveCamera
                {
                    Position = new Point3D(0.5, 0.5, 6), // Increase the Z value to zoom out
                    LookDirection = new Vector3D(-0.5, -0.5, -5), // Adjust the LookDirection accordingly
                    UpDirection = new Vector3D(0, 1, 0)
                };

                viewport.Camera = _camera;

                wnd.Show();

                // Start the Dispatcher Processing
                Dispatcher.Run();
            }));

            // Set the apartment state
            newWindowThread.SetApartmentState(ApartmentState.STA);

            // Make the thread a background thread
            newWindowThread.IsBackground = true;

            // Start the thread
            newWindowThread.Start();
        }
        private static void Wnd_KeyDown(object sender, KeyEventArgs e)
        {
            const double displacement = 0.1;

            switch (e.Key)
            {
                case Key.Z:
                    _camera.Position = new Point3D(_camera.Position.X, _camera.Position.Y, _camera.Position.Z - displacement);
                    break;
                case Key.S:
                    _camera.Position = new Point3D(_camera.Position.X, _camera.Position.Y, _camera.Position.Z + displacement);
                    break;
                case Key.Q:
                    _camera.Position = new Point3D(_camera.Position.X - displacement, _camera.Position.Y, _camera.Position.Z);
                    break;
                case Key.D:
                    _camera.Position = new Point3D(_camera.Position.X + displacement, _camera.Position.Y, _camera.Position.Z);
                    break;
            }
        }
    }

}

