using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snakes_Spel
{
    class Instellingen
    {

        public static int Width { get; set; }
        public static int Height { get; set; }

        public static string directions;

        public Instellingen()
        {
            Width = 15;
            Height = 15;
            directions = "left";
        }


    }
}
