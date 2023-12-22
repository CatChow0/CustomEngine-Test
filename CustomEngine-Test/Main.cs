using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Diagnostics;

namespace CustomEngine_Test
{
    internal class MainClass
    {

        static Stopwatch stopwatch = new Stopwatch();

        // Main function of the program (entry point) STA Thread
        [STAThread]
        public static void Main()
        {

            stopwatch.Start();
            double lastTime = stopwatch.Elapsed.TotalSeconds;
            
            // Open a window
            GraphicSystem.Run();
            while (true)
            {
                double currentTime = stopwatch.Elapsed.TotalSeconds;
                double deltaTime = currentTime - lastTime;
                // Update the program every frame using delta time between frames
                //Update(deltaTime);
                lastTime = currentTime;
            }
        }

        // Update function
        //public static void Update(double deltaTime)
        //{
            //Dessin d'une ligne
            
        //}
    }
}
