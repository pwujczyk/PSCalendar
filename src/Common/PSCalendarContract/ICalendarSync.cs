using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace PSCalendarContract
{
    [ServiceContract]
    public interface ICalendarSync
    {
        [OperationContract]
        void Sync(DateTime start, DateTime end);

        [OperationContract]
        void AddSyncAccount(string email);

        [OperationContract]
        void AddCalendars();
    }
}
