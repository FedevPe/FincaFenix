using FincaFenix.EFCore.Options;
using FincaFenix.Entities.POCOEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Reflection;

namespace FincaFenix.EFCore.Context
{
    public class FincaFenixContext : DbContext
    {
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

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=Finca_FenixDB;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False");
            }
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            foreach (var foreignKey in modelBuilder.Model
                    .GetEntityTypes()
                    .SelectMany(e => e.GetForeignKeys()))
            {
                foreignKey.DeleteBehavior = DeleteBehavior.Restrict;
            }

            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}
