using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BrowserTextRPG.DTOModel.Fight
{
    public class GetHighscoreDto
    {
        public string Name { get; set; }
        public int Fights { get; set; } = 0;
        public int Defeats { get; set; } = 0;
        public int Victories { get; set; } = 0;
    }
}
