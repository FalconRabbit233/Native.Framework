using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xyz.presidium.dicebot.Code.Dicebot.Sqlite;

namespace xyz.presidium.dicebot.Code.Dicebot.Models
{
    [Table("execution_line")]
    class ExecutionLine
    {
        public int Id { get; set; }

        [Column("content")]
        public string Content { get; set; }
    }
}
