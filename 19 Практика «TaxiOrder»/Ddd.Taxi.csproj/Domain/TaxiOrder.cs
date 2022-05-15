using System;
using System.Globalization;
using System.Collections.Generic;
using System.Linq;
using Ddd.Infrastructure;

namespace Ddd.Taxi.Domain
{
    public class DriversRepository { }

    public class TaxiApi : ITaxiApi<TaxiOrder>
    {
        private DriversRepository DriversRepo { get; }
        private Func<DateTime> CurrentTime { get; }
        private int IdCounter { get; set; }

        public TaxiApi(DriversRepository driversRepo, Func<DateTime> currentTime)
        {
            DriversRepo = driversRepo;
            CurrentTime = currentTime;
        }

        public TaxiOrder CreateOrder(string firstName, string lastName, 
            string street, string building) => 
                new TaxiOrder(IdCounter++, new PersonName(firstName, lastName), 
                new Address(street, building), CurrentTime());

        public void UpdateDestination(TaxiOrder order, string street, string building) => 
            order.UpdateDestination(new Address(street, building));

        public void AssignDriver(TaxiOrder order, int driverId) => 
            order.AssignDriver(driverId, CurrentTime());

        public void UnassignDriver(TaxiOrder order) => 
            order.UnassignDriver();

        public string GetDriverFullInfo(TaxiOrder order) => 
            order.GetDriverFullInfo();

        public string GetShortOrderInfo(TaxiOrder order) => 
            order.GetShortOrderInfo();

        public DateTime GetLastProgressTime(TaxiOrder order) => 
            order.GetLastProgressTime();

        public void Cancel(TaxiOrder order) => 
            order.Cancel(CurrentTime());

        public void StartRide(TaxiOrder order) => 
            order.StartRide(CurrentTime());

        public void FinishRide(TaxiOrder order) => 
            order.FinishRide(CurrentTime());
    }

    public class TaxiOrder : Entity<int>
    {
        private int Identificator { get; }

        public PersonName ClientName { get; private set; }

        public Address Start { get; private set; }

        public Address Destination { get; private set; }

        public Driver Driver { get; private set; }

        public TaxiOrderStatus Status { get; private set; }

        public DateTime CreationTime { get; private set; }

        public DateTime DriverAssignmentTime { get; private set; }

        public DateTime CancelTime { get; private set; }

        public DateTime StartRideTime { get; private set; }

        public DateTime FinishRideTime { get; private set; }

        public TaxiOrder(int id, PersonName clientName, Address startAddress, DateTime dateTime) : base(id)
        {
            Identificator = id;
            ClientName = clientName;
            Start = startAddress;
            CreationTime = dateTime;
        }

        public void Cancel(DateTime cancelTime)
        {
            if (Status == TaxiOrderStatus.InProgress)
                throw new InvalidOperationException();
            Status = TaxiOrderStatus.Canceled;
            CancelTime = cancelTime;
        }

        public void UpdateDestination(Address destination) => 
            Destination = destination;

        public void AssignDriver(int driverId, DateTime assignTime)
        {
            if (Driver == null)
            {
                if (driverId == 15)
                {
                    var name = new PersonName("Drive", "Driverson");
                    Driver = new Driver(driverId, name,
                                       "Lada sedan", "Baklazhan", "A123BT 66");
                    DriverAssignmentTime = assignTime;
                    Status = TaxiOrderStatus.WaitingCarArrival;
                }
                else
                    throw new Exception("Unknown id: " + driverId);
            }
            else
                throw new InvalidOperationException();
        }

        public void UnassignDriver()
        {
            if (Status == TaxiOrderStatus.InProgress || Driver == null)
                throw new InvalidOperationException(Status.ToString());
            Driver = new Driver(null, null, null, null, null);
            Status = TaxiOrderStatus.WaitingForDriver;
        }

        public void StartRide(DateTime startTime)
        {
            if (Driver == null)
                throw new InvalidOperationException();
            Status = TaxiOrderStatus.InProgress;
            StartRideTime = startTime;
        }

        public void FinishRide(DateTime finishTime)
        {
            if (Status != TaxiOrderStatus.InProgress || Driver == null)
                throw new InvalidOperationException();
            Status = TaxiOrderStatus.Finished;
            FinishRideTime = finishTime;
        }

        public string GetDriverFullInfo()
        {
            if (Status == TaxiOrderStatus.WaitingForDriver)
                return null;
            return string.Format(
                "Id: {0} DriverName: {1} Color: {2} CarModel: {3} PlateNumber: {4}",
                Driver.Identificator, FormatName(Driver.Name.FirstName, Driver.Name.LastName), 
                Driver.Car.Color, Driver.Car.Model, Driver.Car.PlateNumber);
        }

        private static string FormatName(string firstName, string lastName) => 
            string.Join(" ", new[] { firstName, lastName }.Where(n => n != null));

        private static string FormatAddress(string street, string building) => 
            string.Join(" ", new[] { street, building }.Where(n => n != null));

        public string GetShortOrderInfo()
        {
            var gotDriver = Driver == null || Driver.Name == null;
            var driver = gotDriver ? "" : FormatName(Driver.Name.FirstName, Driver.Name.LastName);
            var destination = Destination == null ? "" :
                              FormatAddress(Destination.Street, Destination.Building);

            return string.
                Format("OrderId: {0} Status: {1} Client: {2} Driver: {3} From: {4} To: {5} LastProgressTime: {6}", 
                Identificator, Status, FormatName(ClientName.FirstName, ClientName.LastName), 
                driver, FormatAddress(Start.Street, Start.Building), 
                destination, GetLastProgressTime().ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture));
        }

        public DateTime GetLastProgressTime()
        {
            switch (Status)
            {
                case TaxiOrderStatus.Finished:
                    return FinishRideTime;
                case TaxiOrderStatus.WaitingCarArrival:
                    return DriverAssignmentTime;
                case TaxiOrderStatus.Canceled:
                    return CancelTime;
                case TaxiOrderStatus.InProgress:
                    return StartRideTime;
                case TaxiOrderStatus.WaitingForDriver:
                    return CreationTime;
            }
            throw new NotSupportedException(Status.ToString());
        }
    }

    public class Driver : Entity<int>
    {
        public int? Identificator { get; }

        public PersonName Name { get; private set; }

        public Car Car { get; private set; }

        public Driver(int? id, PersonName name, string carModel,
                      string carColor, string carPlateNumber) : base(0)
        {
            Identificator = id;
            Car = new Car(carModel, carColor, carPlateNumber);
            Name = name;
        }
    }

    public class Car : ValueType<Car>
    {
        public string Model { get; private set; }

        public string Color { get; private set; }

        public string PlateNumber { get; private set; }

        public Car(string model, string color, string plateNumber)
        {
            PlateNumber = plateNumber;
            Color = color;
            Model = model;
        }
    }
}
