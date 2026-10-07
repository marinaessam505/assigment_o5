namespace assigment__o5
{



    

    public class DeliveryAddress
    {
        public string City { get; set; }
        public string Street { get; set; }
        public int BuildingNumber { get; set; }

        public DeliveryAddress(string city, string street, int buildingNumber)
        {
            City = city;
            Street = street;
            BuildingNumber = buildingNumber;
        }

        public string GetFullAddress()
        {
            return $"{BuildingNumber}, {Street}, {City}";
        }
    }

    
    // Interfaces
    

    public interface ITrackable
    {
        string GetTrackingStatus();
    }

    public interface IInsurable
    {
        decimal CalculateInsurance();
    }

    

    public abstract partial class Shipment
    {
        private string trackingCode;
        private string description;
        private decimal weight;
        private decimal deliveryFee;

        public static int TotalShipmentsCreated;

        public string TrackingCode
        {
            get { return trackingCode; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Tracking Code cannot be empty.");

                trackingCode = value;
            }
        }

        public string Description
        {
            get { return description; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Description cannot be empty.");

                description = value;
            }
        }

        public decimal Weight
        {
            get { return weight; }
            set
            {
                if (value <= 0)
                    throw new ArgumentException("Weight must be greater than zero.");

                weight = value;
            }
        }

        public decimal DeliveryFee
        {
            get { return deliveryFee; }
            set
            {
                if (value < 0)
                    throw new ArgumentException("Delivery Fee cannot be negative.");

                deliveryFee = value;
            }
        }

        public DeliveryAddress Destination { get; set; }

        public abstract decimal EstimatedCost { get; }

        public abstract void PrintShipment();

        
        static Shipment()
        {
            TotalShipmentsCreated = 0;
            Console.WriteLine("Shipment System Initialized");
        }

        protected Shipment(
            string trackingCode,
            string description,
            decimal weight,
            decimal deliveryFee,
            DeliveryAddress destination,
            string initialStatus = "In Transit")
        {
            TrackingCode = trackingCode;
            Description = description;
            Weight = weight;
            DeliveryFee = deliveryFee;
            Destination = destination;
            TrackingStatus = initialStatus;

            TotalShipmentsCreated++;
        }

        
        public Shipment CopyShipment()
        {
            return DeepCopy();
        }

       
        public Shipment ShallowCopy()
        {
            return (Shipment)this.MemberwiseClone();
        }

        
        public Shipment DeepCopy()
        {
            Shipment copy = (Shipment)this.MemberwiseClone();

            copy.Destination = new DeliveryAddress(
                Destination.City,
                Destination.Street,
                Destination.BuildingNumber
            );

            return copy;
        }

        public static int GetTotalShipmentsCreated()
        {
            return TotalShipmentsCreated;
        }

        
        partial void OnTrackingStatusChanged(string newStatus);
    }


    public partial class Shipment
    {
        public string TrackingStatus { get; private set; } = "In Transit";

        public string GetTrackingStatus()
        {
            return TrackingStatus;
        }

        public void UpdateTrackingStatus(string newStatus)
        {
            TrackingStatus = newStatus;

            OnTrackingStatusChanged(newStatus);
        }

       
        partial void OnTrackingStatusChanged(string newStatus)
        {
            Console.WriteLine($"Tracking status changed to: {newStatus}");
        }
    }

   
    public class StandardShipment : Shipment, ITrackable, IInsurable
    {
        public StandardShipment(
            string trackingCode,
            string description,
            decimal weight,
            decimal deliveryFee,
            DeliveryAddress destination,
            string initialStatus = "In Transit")
            : base(trackingCode, description, weight, deliveryFee, destination, initialStatus)
        {
        }

        public override decimal EstimatedCost
        {
            get { return Weight * 15 + DeliveryFee; }
        }

        public override void PrintShipment()
        {
            Console.WriteLine("Standard Shipment");
            Console.WriteLine();

            Console.WriteLine($"Tracking Code : {TrackingCode}");
            Console.WriteLine($"Description   : {Description}");
            Console.WriteLine($"Estimated Cost: {EstimatedCost} EGP");
        }

        
        string ITrackable.GetTrackingStatus()
        {
            return $"Shipment {TrackingCode} is Ready.";
        }

        public decimal CalculateInsurance()
        {
            return EstimatedCost * 0.05m;
        }
    }

   

    public class ExpressShipment : Shipment, ITrackable, IInsurable
    {
        public ExpressShipment(
            string trackingCode,
            string description,
            decimal weight,
            decimal deliveryFee,
            DeliveryAddress destination,
            string initialStatus = "In Transit")
            : base(trackingCode, description, weight, deliveryFee, destination, initialStatus)
        {
        }

        public override decimal EstimatedCost
        {
            get { return Weight * 35 + DeliveryFee; }
        }

        public override void PrintShipment()
        {
            Console.WriteLine("Express Shipment");
            Console.WriteLine();

            Console.WriteLine($"Tracking Code : {TrackingCode}");
            Console.WriteLine($"Extra Fee     : {DeliveryFee} EGP");
            Console.WriteLine($"Estimated Cost: {EstimatedCost} EGP");
        }

        string ITrackable.GetTrackingStatus()
        {
            return $"Shipment {TrackingCode} is Out for Delivery.";
        }

        public decimal CalculateInsurance()
        {
            return EstimatedCost * 0.08m;
        }
    }

    
    public class InternationalShipment : Shipment, ITrackable, IInsurable
    {
        public string DestinationCountry { get; set; }

        public InternationalShipment(
            string trackingCode,
            string description,
            decimal weight,
            decimal deliveryFee,
            DeliveryAddress destination,
            string destinationCountry,
            string initialStatus = "In Transit")
            : base(trackingCode, description, weight, deliveryFee, destination, initialStatus)
        {
            DestinationCountry = destinationCountry;
        }

        public override decimal EstimatedCost
        {
            get { return Weight * 30 + DeliveryFee; }
        }

        public override void PrintShipment()
        {
            Console.WriteLine("International Shipment");
            Console.WriteLine();

            Console.WriteLine($"Tracking Code       : {TrackingCode}");
            Console.WriteLine($"Destination Country : {DestinationCountry}");
            Console.WriteLine($"Estimated Cost      : {EstimatedCost} EGP");
        }

        string ITrackable.GetTrackingStatus()
        {
            return $"Shipment {TrackingCode} has been Delivered.";
        }

        public decimal CalculateInsurance()
        {
            return EstimatedCost * 0.12m;
        }
    }

    

    public static class DeliveryReport
    {
        public static void PrintShipment(ITrackable shipment)
        {
            Console.WriteLine(shipment.GetTrackingStatus());
        }

        public static void PrintInsurance(IInsurable shipment)
        {
            string shipmentName = shipment.GetType().Name.Replace("Shipment", "");

            Console.WriteLine(
                $"{shipmentName} Shipment Insurance : " +
                $"{shipment.CalculateInsurance():0.00} EGP");
        }
    }

   

    public class DeliveryCenter
    {
        private List<Shipment> shipments = new List<Shipment>();

        public void AddShipment(Shipment shipment)
        {
            shipments.Add(shipment);
        }

        public void PrintAllShipments()
        {
            foreach (Shipment shipment in shipments)
            {
                shipment.PrintShipment();
                Console.WriteLine("------------------------");
            }
        }

        public void PrintTrackingStatuses()
        {
            foreach (Shipment shipment in shipments)
            {
                
                if (shipment is ITrackable trackable)
                {
                    DeliveryReport.PrintShipment(trackable);
                }
            }
        }
    }

   

    public static class DeliveryUtilities
    {
        public static void PrintSeparator(char line = '=')
        {
            Console.WriteLine(new string(line, 42));
        }

        
        public static void PrintSystemTitle(string title, char line = '=')
        {
            PrintSeparator(line);
            Console.WriteLine(title);
            PrintSeparator(line);
        }
    }

    
    public static class ShipmentExtensions
    {
        public static string GetSummary(this Shipment shipment)
        {
            string type = shipment.GetType().Name.Replace("Shipment", "");

            return $"{shipment.TrackingCode} | " +
                   $"{type} | " +
                   $"{shipment.Weight} KG | " +
                   $"{shipment.GetTrackingStatus()}";
        }

        public static bool IsDelivered(this Shipment shipment)
        {
            return shipment.GetTrackingStatus() == "Delivered";
        }
    }

   
    class Program
    {
        static void Main()
        {
            DeliveryUtilities.PrintSystemTitle("Smart Delivery Management System");
            Console.WriteLine();

            
            int alreadyCreated = Shipment.GetTotalShipmentsCreated();
            Console.WriteLine();

            // --------Creating Shipments -----------
            DeliveryUtilities.PrintSystemTitle("Creating Shipments...");
            Console.WriteLine();

            Shipment shipment1 = new StandardShipment(
                "SH001", "Laptop", 3, 50,
                new DeliveryAddress("Cairo", "Main Street", 10),
                "In Transit");
            Console.WriteLine("Standard Shipment Created");

            Shipment shipment2 = new ExpressShipment(
                "SH002", "Phone", 2, 30,
                new DeliveryAddress("Alexandria", "Corniche", 5),
                "Out For Delivery");
            Console.WriteLine("Express Shipment Created");

            Shipment shipment3 = new InternationalShipment(
                "SH003", "Computer", 8, 20,
                new DeliveryAddress("Berlin", "Hauptstrasse", 7),
                "Germany",
                "Delivered");
            Console.WriteLine("International Shipment Created");

            Console.WriteLine();
            Console.WriteLine($"Total Shipments Created : {Shipment.GetTotalShipmentsCreated()}");
            Console.WriteLine();

            // ------- Object Copying --------
            DeliveryUtilities.PrintSystemTitle("Object Copying");
            Console.WriteLine();

            Shipment assignedShipment = shipment1;      

            Console.WriteLine($"Original Shipment  : {shipment1.TrackingCode}");
            Console.WriteLine($"Assigned Shipment  : {assignedShipment.TrackingCode}");
            Console.WriteLine();
            Console.WriteLine($"Same Object : {ReferenceEquals(shipment1, assignedShipment)}");
            Console.WriteLine();

            // ------- Shallow Copy ---------
            DeliveryUtilities.PrintSystemTitle("Shallow Copy", '-');
            Console.WriteLine();

            Shipment shallowCopy = shipment1.ShallowCopy();

            Console.WriteLine($"Original Shipment Address : {shipment1.Destination.City}");
            Console.WriteLine($"Copied Shipment Address   : {shallowCopy.Destination.City}");
            Console.WriteLine();
            Console.WriteLine("Changing copied shipment address...");
            shallowCopy.Destination.City = "Giza";
            Console.WriteLine();




        }
    }
}


















        