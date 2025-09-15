using Microsoft.EntityFrameworkCore;
using CarDealershipApi.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace CarDealershipApi.Data;

public class CarDealershipDbContext : IdentityDbContext<User>
{
    public CarDealershipDbContext(DbContextOptions<CarDealershipDbContext> options) : base(options)
    {
    }

    public DbSet<Car> Cars { get; set; }
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Employee> Employees { get; set; }
    public DbSet<Sale> Sales { get; set; }
    public DbSet<PurchaseRequest> PurchaseRequests { get; set; }
    public DbSet<OtpCode> OtpCodes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure Car entity
        modelBuilder.Entity<Car>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.VIN).IsRequired().HasMaxLength(17);
            entity.Property(e => e.Price).HasColumnType("decimal(18,2)");
            entity.HasIndex(e => e.VIN).IsUnique();
        });

        // Configure Customer entity
        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Email).IsRequired();
            entity.HasIndex(e => e.Email).IsUnique();
        });

        // Configure Employee entity
        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Email).IsRequired();
            entity.Property(e => e.Salary).HasColumnType("decimal(18,2)");
            entity.HasIndex(e => e.Email).IsUnique();
        });

        // Configure Sale entity
        modelBuilder.Entity<Sale>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.SalePrice).HasColumnType("decimal(18,2)");

            entity.HasOne(e => e.Car)
                .WithMany(c => c.Sales)
                .HasForeignKey(e => e.CarId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Customer)
                .WithMany(c => c.Sales)
                .HasForeignKey(e => e.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Employee)
                .WithMany(e => e.Sales)
                .HasForeignKey(e => e.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Configure PurchaseRequest entity
        modelBuilder.Entity<PurchaseRequest>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.RequestedPrice).HasColumnType("decimal(18,2)");

            entity.HasOne(e => e.Car)
                .WithMany()
                .HasForeignKey(e => e.CarId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Customer)
                .WithMany()
                .HasForeignKey(e => e.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Configure OtpCode entity
        modelBuilder.Entity<OtpCode>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Code).IsRequired().HasMaxLength(6);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Purpose).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Metadata).HasMaxLength(500);
            entity.HasIndex(e => new { e.Email, e.Purpose, e.CreatedAt });
        });

        // Seed data
        SeedData(modelBuilder);
    }

    private void SeedData(ModelBuilder modelBuilder)
    {
        // Seed Employees
        modelBuilder.Entity<Employee>().HasData(
            new Employee
            {
                Id = 1,
                FirstName = "John",
                LastName = "Smith",
                Email = "john.smith@dealership.com",
                PhoneNumber = "555-0101",
                Position = "Sales Manager",
                Salary = 75000,
                HireDate = DateTime.UtcNow.AddYears(-2)
            },
            new Employee
            {
                Id = 2,
                FirstName = "Sarah",
                LastName = "Johnson",
                Email = "sarah.johnson@dealership.com",
                PhoneNumber = "555-0102",
                Position = "Sales Representative",
                Salary = 55000,
                HireDate = DateTime.UtcNow.AddYears(-1)
            }
        );

        // Seed Cars
        modelBuilder.Entity<Car>().HasData(
            new Car
            {
                Id = 1,
                Make = "Toyota",
                Model = "Camry",
                Year = 2023,
                Color = "Silver",
                VIN = "1HGBH41JXMN109186",
                Price = 28000,
                Mileage = 15000,
                Transmission = "Automatic",
                FuelType = "Gasoline",
                IsAvailable = true,
                DateAdded = DateTime.UtcNow.AddDays(-30)
            },
            new Car
            {
                Id = 2,
                Make = "Honda",
                Model = "Civic",
                Year = 2022,
                Color = "Blue",
                VIN = "2HGBH41JXMN109187",
                Price = 25000,
                Mileage = 22000,
                Transmission = "Automatic",
                FuelType = "Gasoline",
                IsAvailable = true,
                DateAdded = DateTime.UtcNow.AddDays(-20)
            },
            new Car
            {
                Id = 3,
                Make = "Ford",
                Model = "F-150",
                Year = 2023,
                Color = "Black",
                VIN = "3HGBH41JXMN109188",
                Price = 45000,
                Mileage = 8000,
                Transmission = "Automatic",
                FuelType = "Gasoline",
                IsAvailable = false,
                DateAdded = DateTime.UtcNow.AddDays(-15),
                DateSold = DateTime.UtcNow.AddDays(-5)
            },
            new Car
            {
                Id = 4,
                Make = "Tesla",
                Model = "Model 3",
                Year = 2022,
                Color = "White",
                VIN = "5YJ3E1EA4NF123456",
                Price = 39000,
                Mileage = 18000,
                Transmission = "Automatic",
                FuelType = "Electric",
                IsAvailable = true,
                DateAdded = DateTime.UtcNow.AddDays(-25)
            },
            new Car
            {
                Id = 5,
                Make = "BMW",
                Model = "X5",
                Year = 2021,
                Color = "Gray",
                VIN = "5UXCR6C0XL9A12345",
                Price = 52000,
                Mileage = 35000,
                Transmission = "Automatic",
                FuelType = "Gasoline",
                IsAvailable = true,
                DateAdded = DateTime.UtcNow.AddDays(-40)
            },
            new Car
            {
                Id = 6,
                Make = "Chevrolet",
                Model = "Malibu",
                Year = 2021,
                Color = "Red",
                VIN = "1G1ZD5ST4MF123456",
                Price = 22500,
                Mileage = 28000,
                Transmission = "Automatic",
                FuelType = "Gasoline",
                IsAvailable = true,
                DateAdded = DateTime.UtcNow.AddDays(-35)
            },
            new Car
            {
                Id = 7,
                Make = "Audi",
                Model = "A4",
                Year = 2023,
                Color = "Blue",
                VIN = "WAUENAF40PA123456",
                Price = 38500,
                Mileage = 12000,
                Transmission = "Automatic",
                FuelType = "Gasoline",
                IsAvailable = true,
                DateAdded = DateTime.UtcNow.AddDays(-18)
            },
            new Car
            {
                Id = 8,
                Make = "Nissan",
                Model = "Altima",
                Year = 2022,
                Color = "Silver",
                VIN = "1N4BL4DV6NC123456",
                Price = 26000,
                Mileage = 19500,
                Transmission = "CVT",
                FuelType = "Gasoline",
                IsAvailable = true,
                DateAdded = DateTime.UtcNow.AddDays(-22)
            },
            new Car
            {
                Id = 9,
                Make = "Mercedes-Benz",
                Model = "C-Class",
                Year = 2021,
                Color = "Black",
                VIN = "WDDGF4HB1MR123456",
                Price = 41000,
                Mileage = 24000,
                Transmission = "Automatic",
                FuelType = "Gasoline",
                IsAvailable = true,
                DateAdded = DateTime.UtcNow.AddDays(-50)
            },
            new Car
            {
                Id = 10,
                Make = "Hyundai",
                Model = "Elantra",
                Year = 2023,
                Color = "Green",
                VIN = "KMHL14JA5PA123456",
                Price = 21500,
                Mileage = 9800,
                Transmission = "Manual",
                FuelType = "Gasoline",
                IsAvailable = true,
                DateAdded = DateTime.UtcNow.AddDays(-12)
            },
            new Car
            {
                Id = 11,
                Make = "Subaru",
                Model = "Outback",
                Year = 2022,
                Color = "Brown",
                VIN = "4S4BSANC1N3123456",
                Price = 32000,
                Mileage = 16500,
                Transmission = "CVT",
                FuelType = "Gasoline",
                IsAvailable = true,
                DateAdded = DateTime.UtcNow.AddDays(-28)
            },
            new Car
            {
                Id = 12,
                Make = "Volkswagen",
                Model = "Jetta",
                Year = 2021,
                Color = "White",
                VIN = "3VWC57BU4MM123456",
                Price = 23500,
                Mileage = 31000,
                Transmission = "Automatic",
                FuelType = "Gasoline",
                IsAvailable = true,
                DateAdded = DateTime.UtcNow.AddDays(-60)
            }
        );

        // Seed Customers
        modelBuilder.Entity<Customer>().HasData(
            new Customer
            {
                Id = 1,
                FirstName = "Mike",
                LastName = "Wilson",
                Email = "mike.wilson@email.com",
                PhoneNumber = "555-0201",
                Address = "123 Main St",
                City = "Anytown",
                State = "CA",
                ZipCode = "12345",
                DateCreated = DateTime.UtcNow.AddDays(-10)
            },
            new Customer
            {
                Id = 2,
                FirstName = "Lisa",
                LastName = "Brown",
                Email = "lisa.brown@email.com",
                PhoneNumber = "555-0202",
                Address = "456 Oak Ave",
                City = "Somewhere",
                State = "NY",
                ZipCode = "67890",
                DateCreated = DateTime.UtcNow.AddDays(-5)
            }
        );

        // Seed Sales
        modelBuilder.Entity<Sale>().HasData(
            new Sale
            {
                Id = 1,
                CarId = 3,
                CustomerId = "1", // This will be updated to use actual user ID after seeding
                EmployeeId = 1,
                SalePrice = 43000,
                SaleDate = DateTime.UtcNow.AddDays(-5),
                PaymentMethod = "Bank Loan",
                Notes = "Customer traded in 2019 Honda Accord"
            }
        );
    }
}
