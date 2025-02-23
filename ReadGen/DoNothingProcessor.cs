﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadGen
{
    class DoNothingProcessor : ReadGenProcesser
    {
        public override ProcessingReturn executeProcess(ConfigInfo ci)
        {
            ProcessingReturn pr = new ProcessingReturn();
            pr.status = 0;
            pr.description = "Success";
            Console.WriteLine("DoNothingProcessor: Doing Nothing - check the proctype field in the what to do file.");

            return pr;
        }
    }
}
