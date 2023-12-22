using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Threading;

namespace CustomEngine_Test
{
    internal class GraphicSystem
   
    {
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
                Window wnd = new Window();

                // Define the window size
                wnd.Width = 800;
                wnd.Height = 600;

                // Create a viewport for 3D rendering
                Viewport3D viewport = new Viewport3D();

                // Create a 3D pyramid (a complete 3D mesh)
                MeshGeometry3D pyramidMesh = new MeshGeometry3D();
                pyramidMesh.Positions.Add(new Point3D(0, 0, 0)); // base
                pyramidMesh.Positions.Add(new Point3D(1, 0, 0)); // base
                pyramidMesh.Positions.Add(new Point3D(0.5, 0, 1)); // top
                pyramidMesh.Positions.Add(new Point3D(0.5, 1, 0.5)); // base

                // Front face
                pyramidMesh.TriangleIndices.Add(0);
                pyramidMesh.TriangleIndices.Add(1);
                pyramidMesh.TriangleIndices.Add(2);

                // Right face
                pyramidMesh.TriangleIndices.Add(1);
                pyramidMesh.TriangleIndices.Add(3);
                pyramidMesh.TriangleIndices.Add(2);

                // Back face
                pyramidMesh.TriangleIndices.Add(3);
                pyramidMesh.TriangleIndices.Add(0);
                pyramidMesh.TriangleIndices.Add(2);

                // Left face
                pyramidMesh.TriangleIndices.Add(0);
                pyramidMesh.TriangleIndices.Add(1);
                pyramidMesh.TriangleIndices.Add(3);

                // Create a material to apply to the pyramid
                Material material = new DiffuseMaterial(new SolidColorBrush(Colors.Green));

                // Create a geometry model with the pyramid mesh and material
                GeometryModel3D pyramidModel = new GeometryModel3D(pyramidMesh, material);

                // Create a model visual containing the geometry model, and add it to the viewport
                ModelVisual3D modelVisual = new ModelVisual3D();
                modelVisual.Content = pyramidModel;
                viewport.Children.Add(modelVisual);

                // Add the viewport to the window
                wnd.Content = viewport;

                // Add a light source so the pyramid can be seen
                DirectionalLight directionalLight = new DirectionalLight();
                directionalLight.Color = Colors.White;
                directionalLight.Direction = new Vector3D(-1.0, -1.0, -1.0);
                ModelVisual3D lightModel = new ModelVisual3D();
                lightModel.Content = directionalLight;
                viewport.Children.Add(lightModel);

                // Create a rotation transform to rotate the pyramid
                RotateTransform3D rotateTransform = new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(0, 1, 0), 0), new Point3D(0.5, 0.5, 0.5));
                
                pyramidModel.Transform = rotateTransform;

                // Create a timer to rotate the pyramid
                DispatcherTimer timer = new DispatcherTimer();
                timer.Interval = TimeSpan.FromMilliseconds(10);
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
                viewport.Camera = new PerspectiveCamera
                {
                    Position = new Point3D(0.5, 0.5, 6), // Increase the Z value to zoom out
                    LookDirection = new Vector3D(-0.5, -0.5, -5), // Adjust the LookDirection accordingly
                    UpDirection = new Vector3D(0, 1, 0)
                };

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
    }

}

