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

        public int RollWithJrrp(int upper, int jrrp)
        {
            double fr(int x)
            {
                double delta = Math.Round(jrrp - 50.5) * 0.035 / 50;
                double result = ((1 - delta) / upper + delta * (x - 1) / (upper * upper)) * x;
                return result < 1 ? result : 1.0;
            }

            var top = upper;
            var bottom = 1;
            const int precise = 10000;
            var rawNumber = rng.Next(bottom, precise + 1);

            while (top - bottom > 1)
            {
                var calcNumber = precise * fr((bottom + top) / 2);
                if (rawNumber < precise * fr(bottom))
                {
                    top = bottom;
                    break;
                }

                if (rawNumber > calcNumber) bottom = (bottom + top) / 2;
                else top = (bottom + top) / 2;
            }

            return top;
        }
    }
}
