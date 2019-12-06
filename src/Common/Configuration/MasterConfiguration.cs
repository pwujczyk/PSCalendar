using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting;
using System.Text;
using System.Threading.Tasks;
using ProductivityTools.MasterConfiguration;

namespace Configuration
{
    public static class MasterConfiguration
    {
        public static MConfiguration mConfiguration;
        public static MConfiguration MConfiguration
        {
            get
            {
                //AppDomain d = AppDomain.CreateDomain("MasterConfigurationCalendar");
                //ObjectHandle confhandle = d.CreateInstance("AnotherDomain", "MConfiguration");
                //MConfiguration conf = (MConfiguration)confhandle.Unwrap();

                if (mConfiguration == null)
                {
                    mConfiguration = new MConfiguration();

                    mConfiguration.SetConfigurationFileName("MasterConfiguration.xml");
                    mConfiguration.SetApplicationName("PSCalendar");
                    mConfiguration.SetConfigurationFileDirectory(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location));

                }
                return mConfiguration;
            }
        }

    }
}
