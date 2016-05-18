using Microsoft.Kinect;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kinect_Gear_Application
{
    static class PlayerController
    {


        // Create and return new Hashtable.
        private static Object hashLock = new object();
        static Hashtable hashtable = new Hashtable();

        private static List<ConnectAndTrackedUser> activeUsers = new List<ConnectAndTrackedUser>(8);

        private static Object IdentifcationLock = new object();
        private static bool isIdentifcationModeEnabled = false;
        private static NetworkThread IdentifcationTemp = null;

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


        //check if idenfication mode is enabled otherwise 
        internal static bool enableIdenticationMode(NetworkThread playerNetworkThread)
        {
            lock (IdentifcationLock)
            {
                if (isIdentifcationModeEnabled)
                {
                    return false;
                }
                else
                {
                    isIdentifcationModeEnabled = true;
                    IdentifcationTemp = playerNetworkThread;
                    return true;
                }
            }
        }

        internal static void IdenticationCompleted(Body player)
        {
            lock (IdentifcationLock)
            {

                lock (hashLock)
                {
                hashtable.Add(player.TrackingId, new ConnectAndTrackedUser(player, IdentifcationTemp));
                //TODO handle that the player has been found
                isIdentifcationModeEnabled = false;
                player = null;
                }
            }
        }

        internal static bool isIdentificationModeEnabled()
        {
            lock (IdentifcationLock)
            {
                return isIdentifcationModeEnabled;
            }
        }

        internal static void updatePlayerPosition(Body UpdatedUser)
        {

            lock (hashLock)
            {

                ConnectAndTrackedUser user = (ConnectAndTrackedUser)hashtable[UpdatedUser.TrackingId];
                if (user != null)
                {
                    user.playerNetworkThread.setPosition(UpdatedUser);
                }
            }
        }
    }



    class ConnectAndTrackedUser
    {
        public NetworkThread playerNetworkThread;
        public Body player;

        public ConnectAndTrackedUser(Body player, NetworkThread identifcationTemp)
        {
            this.player = player;
            this.playerNetworkThread = identifcationTemp;
        }
    }
}
