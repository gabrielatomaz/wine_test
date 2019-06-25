using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wine.Client;

namespace Wine.Models
{
    public class ClientItemViewModel
    {
        public string Document { get; set; }
        public string Name { get; set; }
        public Items Item { get; set; }
        public bool NotFound { get; set; } = false;
    }
}
