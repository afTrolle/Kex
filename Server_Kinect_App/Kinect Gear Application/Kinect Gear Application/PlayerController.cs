using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kinect_Gear_Application
{
    static class PlayerController
    {

        private static Object Lock = new Object();

        static public void init()
        {

        }

        static private void function()
        {

            lock (Lock)
            {
                //critical section.
                // Access thread-sensitive resources.
            }

        }

    
    }
}
