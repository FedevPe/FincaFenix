using System.Reflection;
using System.Security.Claims;
using FincaFenix.EFCore.Context;
using FincaFenix.Entities;
using FincaFenix.Entities.POCOEntities;
using FincaFenix.Entities.Units;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Logging;

namespace FincaFenix.EFCore
{
    public class SeedDataBase
    {
        public static async Task SeedAddPoliciesAsync(
            DbContext context,
            ILogger logger,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var userManager = context.GetService<UserManager<ApplicationUser>>();
                var roleManager = context.GetService<RoleManager<IdentityRole>>();

                var userRoot = new ApplicationUser
                {
                    UserName = "root",
                    Email = "root@root.com",
                    Name = "Federico",
                    LastName = "Perez"
                    
                };

                await userManager.CreateAsync(userRoot, "Password123@");

                var developer = await roleManager.FindByNameAsync("desarrollador");
                var admin = await roleManager.FindByNameAsync("admin");
                var supervisor = await roleManager.FindByNameAsync("supervisor");
                var operario = await roleManager.FindByNameAsync("operario");

                var allPolicies = typeof(PolicyMaster)
                    .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
                    .Where(f => f.FieldType == typeof(string))
                    .Select(f => (string)f.GetValue(null))
                    .ToList();

                var readOnlyPolicies = allPolicies
                    .Where(p => p.EndsWith("_READ"))
                    .ToList();

                await AssignPoliciesToRoleAsync(roleManager, developer, allPolicies);
                await AssignPoliciesToRoleAsync(roleManager, admin, allPolicies);

                var supervisorPolicies = new List<string>
                {
                    PolicyMaster.WORKORDER_READ,
                    PolicyMaster.RECIPE_READ,
                    PolicyMaster.DETAIL_WORKORDER_CREATE,
                    PolicyMaster.DETAIL_WORKORDER_READ,
                    PolicyMaster.TASK_READ,
                    PolicyMaster.FARM_READ,
                    PolicyMaster.SECTOR_FARM_READ,
                    PolicyMaster.MATERIAL_READ,
                    PolicyMaster.MATERIAL_CATEGORY_READ,
                    PolicyMaster.UNIT_OF_MEASURE_READ,
                    PolicyMaster.MACHINE_READ,
                    PolicyMaster.FRUIT_READ,
                    PolicyMaster.FRUIT_VARIETY_READ,
                    PolicyMaster.EMPLOYEE_READ,
                    PolicyMaster.STOCK_READ,
                    PolicyMaster.MOVEMENT_READ,
                    PolicyMaster.RESERVATION_READ,
                    PolicyMaster.CONSUMPTION_READ
                };

                var operarioPolicies = new List<string>
                {
                    PolicyMaster.WORKORDER_READ,
                    PolicyMaster.RECIPE_READ,
                    PolicyMaster.DETAIL_WORKORDER_CREATE,
                    PolicyMaster.DETAIL_WORKORDER_READ,
                    PolicyMaster.STOCK_READ
                };

                await AssignPoliciesToRoleAsync(roleManager, supervisor, supervisorPolicies);
                await AssignPoliciesToRoleAsync(roleManager, operario, operarioPolicies);
            }
            catch (Exception ex)
            {
                logger?.LogWarning(ex, "Error al cargar Roles y Claims.");
            }
        }

        public static async Task SeedInventoryUnitsAsync(
            FincaFenixContext context,
            ILogger logger,
            CancellationToken cancellationToken = default)
        {
            try
            {
                await BackfillMaterialUnitsAsync(context, cancellationToken);
            }
            catch (Exception ex)
            {
                logger?.LogWarning(ex, "Error al asignar unidades de medida base a los materiales.");
            }
        }

        private static async Task BackfillMaterialUnitsAsync(
            FincaFenixContext context,
            CancellationToken cancellationToken)
        {
            var materials = await context.Materials
                .Where(m => m.UnitOfMeasureId == null)
                .ToListAsync(cancellationToken);

            if (materials.Count == 0)
            {
                return;
            }

            var materialIds = materials.Select(m => m.Id).ToList();

            var recipeUnits = await context.DetailRecipes
                .Where(d => d.EstimatedAmountUnit != null && materialIds.Contains(d.MaterialId))
                .Select(d => new { d.MaterialId, d.EstimatedAmountUnit })
                .ToListAsync(cancellationToken);

            var inferredByMaterial = recipeUnits
                .GroupBy(x => x.MaterialId)
                .ToDictionary(
                    g => g.Key,
                    g => UnitOfMeasureCatalog.GetIdByCode(
                        g.GroupBy(x => UnitConverter.Normalize(x.EstimatedAmountUnit))
                            .OrderByDescending(x => x.Count())
                            .Select(x => x.Key)
                            .FirstOrDefault()));

            foreach (var material in materials)
            {
                material.UnitOfMeasureId =
                    inferredByMaterial.TryGetValue(material.Id, out var unitId) && unitId.HasValue
                        ? unitId.Value
                        : GetDefaultUnitForCategory(material.CategoryId);
            }

            await context.SaveChangesAsync(cancellationToken);
        }

        private static int GetDefaultUnitForCategory(int categoryId) => categoryId switch
        {
            1 => UnitOfMeasureCatalog.Litro,       // Herbicidas
            2 => UnitOfMeasureCatalog.Kilogramo,   // Fertilizantes
            3 => UnitOfMeasureCatalog.Kilogramo,   // Fungicidas
            4 => UnitOfMeasureCatalog.Litro,       // Insecticidas
            5 => UnitOfMeasureCatalog.Litro,       // Correctores
            6 => UnitOfMeasureCatalog.Kilogramo,   // Enmiendas
            7 => UnitOfMeasureCatalog.Litro,       // Coadyuvantes
            _ => UnitOfMeasureCatalog.Unidad
        };

        private static async Task AssignPoliciesToRoleAsync(
            RoleManager<IdentityRole> roleManager,
            IdentityRole role,
            List<string> policies)
        {
            if (role is null) return;

            var existingClaims = await roleManager.GetClaimsAsync(role);
            var existingPolicyValues = existingClaims
                .Where(c => c.Type == CustomClaims.POLICIES)
                .Select(c => c.Value)
                .ToHashSet();

            foreach (var policy in policies)
            {
                if (!existingPolicyValues.Contains(policy))
                {
                    await roleManager.AddClaimAsync(role, new Claim(CustomClaims.POLICIES, policy));
                }
            }
        }
    }
}