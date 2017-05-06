using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeanCloud;

namespace LeanCloud.Realtime.Test.Integration.WPFNetFx45.Model
{
    [AVClassName("Team")]
    public class Team : AVObject
    {
        [AVFieldName("domain")]
        public string Domain
        {
            get { return GetProperty<string>(null, "Domain"); }
            set { SetProperty(value, "Domain"); }
        }

        [AVFieldName("name")]
        public string Name
        {
            get { return GetProperty<string>(null, "Name"); }
            set { SetProperty(value, "Name"); }
        }
    }
}
