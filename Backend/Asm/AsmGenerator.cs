using Backend.POLIS;
using Backend.RPN;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Asm
{
    public class AsmGenerator
    {
        public POLISGenerator gen { get; set; }
        public AsmGenerator(string path)
        {
            gen = new POLISGenerator(path);
        }

        public string GenerateNASM()
        {
            var elements = gen.GenerateRPN();
            StringBuilder sb = new StringBuilder();
            return null;
        }
    }
}
