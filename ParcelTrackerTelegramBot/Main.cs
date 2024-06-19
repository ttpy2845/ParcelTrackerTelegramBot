using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParcelTrackerTelegramBot
{
    public class Program { 
        public static void Main(string[]args) { 
            ParcelTrackerBot parcelTrackerBot = new ParcelTrackerBot();
            parcelTrackerBot.BotStart();
            Console.ReadKey();
    
        }
    
    
    }

}
