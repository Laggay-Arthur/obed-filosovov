using System;
using System.Collections.Generic;


namespace Philoso_forks.Algorithms
{
    class Futex
    {
        List<Philos> philosis;
        public Futex(ref List<Philos> philosis) { this.philosis = philosis; }
        public void Start(object sender = null, EventArgs e = null)
        { for (byte i = 0; i < 5; i++) philosis[i].eat(); }
        public void Stop() { for (byte i = 0; i < 5; i++) philosis[i].Stop(); }
    }
}