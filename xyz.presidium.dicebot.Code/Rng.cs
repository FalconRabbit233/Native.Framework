using Native.Sdk.Cqp;
using Native.Sdk.Cqp.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xyz.presidium.dicebot.Code.Dicebot.Models;
using xyz.presidium.dicebot.Code.Dicebot.Sqlite;

namespace xyz.presidium.dicebot.Code
{
    public class Rng
    {
        private static Random rng;

        public Rng()
        {
            if (rng == null) rng = new Random();
        }

        /// <summary>
        /// 掷骰子
        /// </summary>
        /// <param name="upper">上界</param>
        /// <returns>1-上界（包含1和上界）</returns>
        public int Roll(int upper)
        {
            return rng.Next(1, upper + 1);
        }
    }
}
