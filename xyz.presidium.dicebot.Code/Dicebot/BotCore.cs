using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xyz.presidium.dicebot.Code.Dicebot.Models;
using xyz.presidium.dicebot.Code.Dicebot.Sqlite;

namespace xyz.presidium.dicebot.Code.Dicebot
{
    public partial class BotCore
    {

        private SQLiteConnection context;

        public BotCore(SQLiteConnection context)
        {
            this.context = context;
        }
    }
}
