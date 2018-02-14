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
        void SyncAllAcounts(DateTime start, DateTime end);

        [OperationContract]
        void SyncAccount(DateTime start, DateTime end, string account);

        [OperationContract]
        void AddSyncAccount(string email);

        [OperationContract]
        void AddCalendarsToAccount(string account);


        [OperationContract]
        void ClearAccount(string account,DateTime start, DateTime end);
    }

}
