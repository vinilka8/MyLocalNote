using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Windows.Devices.Geolocation;

namespace My_Local_Note
{
    class ManeuverDescription
    {
        public string Id { get; set; }

        public Geopoint Location { get; set; }

        public string Description { get; set; }
    }
}
