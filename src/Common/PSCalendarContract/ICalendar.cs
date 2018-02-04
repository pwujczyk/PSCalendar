using PSCalendarContract.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace PSCalendarContract
{
    [ServiceContract]
    public interface ICalendar
    {
        [OperationContract]
        void GetDate();

        [OperationContract]
        void AddEvent(Event @event);

        [OperationContract]
        void ChangeEvent(Event @event);

        [OperationContract]
        void AddPeriodEveent(PeriodicEvent periodEvent);

        [OperationContract]
        List<Event> GetEvents(DateTime start, DateTime end);

        [OperationContract]
        List<PeriodicEvent> GetPeriodEvents(DateTime start, DateTime end);

        [OperationContract]
        bool Delete(int id);
    }
}
