using System;
using System.Collections.Generic;
using System.Text;

namespace Newtonsoft.Json.HttpClientExtension.Tests
{
    public class Circular
    {
        public Circular Link { get; set; }
    }
}
