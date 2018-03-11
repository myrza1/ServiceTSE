
using Domain;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace ServiceTSE
{
    // ПРИМЕЧАНИЕ. Команду "Переименовать" в меню "Рефакторинг" можно использовать для одновременного изменения имени класса "Service1" в коде, SVC-файле и файле конфигурации.
    // ПРИМЕЧАНИЕ. Чтобы запустить клиент проверки WCF для тестирования службы, выберите элементы Service1.svc или Service1.svc.cs в обозревателе решений и начните отладку.
    public class Service1 : IService1
    {


        public string GetData(int value)
        {
            return string.Format("You entered: {0}", value);
        }

        public CompositeType GetDataUsingDataContract(CompositeType composite)
        {
            if (composite == null)
            {
                throw new ArgumentNullException("composite");
            }
            if (composite.BoolValue)
            {
                composite.StringValue += "Suffix";
            }
            return composite;
        }

        public List<Flight> getFlightArrivals(DateTime date)
        {
            DateTime str = date.AddHours(1);

            var arrFlightAdapter = new DataSet1TableAdapters.ArrivalFlightTableAdapter();
            var data = arrFlightAdapter.GetData();
            DataSet1.ArrivalFlightDataTable table = new DataSet1.ArrivalFlightDataTable();

            List<Flight> flights = new List<Flight>();
            Flight fl;

            foreach (var fls in data)
            {
                fl = new Flight();
                fl.FlightNumber = fls.flightNumber;
                fl.ScheduledTime = fls.scheduledTime;
                fl.FlightUniqueID = fls.arrivalFlightId.ToString();
               // fl.AirlineDesignatorIATA = fls.
                flights.Add(fl);
            }

            
            

            return flights;
        }

        public List<Order> getFlightOrdersArrivals(int flightid)
        {
            var arrOrderAdapter = new DataSet1TableAdapters.ArrivalOrderViewTableAdapter();
            var data = arrOrderAdapter.GetData();
            List<Order> orders = new List<Order>();

            Order or;
            try
            {
                foreach (var item in data)
            {
                if (item.arrival_id.Equals(flightid))
                {
                    or = new Order();
                    or.Mani = item.manni.ToString();
                    or.Name = item.name;
                    or.Status = item.status;
                    or.Start_time = item.start_time.Equals(DBNull.Value) ? DateTime.MinValue : item.start_time;
                        //or.End_time = item.end_time.Equals(DBNull.Value) ? DateTime.MinValue : item.end_time; 
                    or.Flight_id = Convert.ToInt32(item.arrival_id);

                    
                    orders.Add(or);
                }
            }

            }
            catch (Exception e)
            {

               
            }
            

            return orders;
        }

        public  bool updateOrdersArrivalFlights(Order order)
        {
            DataSet1TableAdapters.OrdersTableAdapter ord = new DataSet1TableAdapters.OrdersTableAdapter();
            DataSet1.OrdersDataTable ORDERDT = ord.GetData();
            DataSet1.OrdersRow orderRow = ORDERDT.FindByid(order.Flight_id);

            orderRow.arrival_id = order.Flight_id;
            orderRow.name = order.Name;
            
            

            
            //ord.Update()
            //ord.Insert(order.Name,
            //    null, order.Status,
            //    false, false,null, 
            //    order.Name, order.Start_time,
            //    order.End_time, order.Liters, order.Density, Convert.ToDouble(order.Mani), order.Flight_id,order.Flight_id);

            ord.Update(orderRow);
            return true;
        }
    }
}
