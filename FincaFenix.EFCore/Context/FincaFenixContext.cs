using FincaFenix.Entities.POCOEntities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace FincaFenix.EFCore.Context
{
    public class FincaFenixContext : DbContext
    {

        // DB PRODUCCION "Data Source=SISTEMAS-SERVER\\SQLEXPRESS;Initial Catalog=FincaFenixDB;User ID=sa;Password=Axoft2016;Connect Timeout=30;Encrypt=True;Trust Server Certificate=True;Application Intent=ReadWrite;Multi Subnet Failover=False;"
        // DB TEST "Data Source=SISTEMAS-SERVER\\SQLEXPRESS;Initial Catalog=FincaFenixDBTest;User ID=sa;Password=Axoft2016;Connect Timeout=30;Encrypt=True;Trust Server Certificate=True;Application Intent=ReadWrite;Multi Subnet Failover=False"
        private readonly string localServer = "Data Source=SISTEMAS-SERVER\\SQLEXPRESS;Initial Catalog=FincaFenixDB;User ID=sa;Password=Axoft2016;Connect Timeout=30;Encrypt=True;Trust Server Certificate=True;Application Intent=ReadWrite;Multi Subnet Failover=False";
        public DbSet<DetailRecipeEntity> DetailRecipes { get; set; }
        public DbSet<DetailSectorFarmEntity> DetailSectors { get; set; }
        public DbSet<DetailWorkOrderEntity> DetailWorkOrders { get; set; }
        public DbSet<DiseasePlague_MaterialEntity> DiseasePlague_Materials { get; set; }
        public DbSet<DiseasePlagueEntity> DiseasePlagues { get; set; }
        public DbSet<Employee_FarmEntity> Employee_Farms { get; set; }
        public DbSet<EmployeeEntity> Employees { get; set; }
        public DbSet<FarmEntity> Farms { get; set; }
        public DbSet<FruitEntity> Fruits { get; set; }
        public DbSet<FruitVarietyEntity> FruitVarieties { get; set; }
        public DbSet<MachineEntity> Machines { get; set; }
        public DbSet<MaterialCategoryEntity> MaterialCategories { get; set; }
        public DbSet<MaterialEntity> Materials { get; set; }
        public DbSet<RecipeEntity> Recipes { get; set; }
        public DbSet<RolEntity> Roles { get; set; }
        public DbSet<TaskEntity> Tasks { get; set; }
        public DbSet<UserEntity> Users { get; set; }
        public DbSet<WorkOrderEntity> WorkOrders { get; set; }
        public DbSet<WorkOrderWorkedSectorEntity> WorkOrderWorkedSectors { get; set; }
        public DbSet<CorrelativeNumberEntity> CorrelativeNumber { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(localServer);
            }
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}
