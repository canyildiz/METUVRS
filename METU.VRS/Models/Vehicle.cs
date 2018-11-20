using System.ComponentModel.DataAnnotations;

namespace METU.VRS.Models
{
    public class Vehicle
    {
        public int ID { get; set; }

        [Required]
        [RegularExpression(@"\d\d[A-Z]{1,4}\d{1,4}", ErrorMessage = "Plate number must follow the pattern, like 06AA1234")]
        [Display(Name = "Plate Number")]
        [MaxLength(10)]
        public string PlateNumber { get; set; }

        [Required]
        [RegularExpression(@"[A-Z]{2}\d{4,6}", ErrorMessage = "Registration number must follow the pattern, like AA123456")]
        [Display(Name = "Registration Number")]
        [MaxLength(8)]
        public string RegistrationNumber { get; set; }

        [Required]
        [Display(Name = "Vehicle Owner Name")]
        [MaxLength(40)]
        public string OwnerName { get; set; }

        [Required]
        [Display(Name = "Vehicle Type")]
        public VehicleType? Type { get; set; } = VehicleType.Car;
    }

    public enum VehicleType
    {
        Car = 10,
        Motorcycle = 20,
        Van = 30,
        Bus = 40,
        Truck = 50,
    }
}