using FincaFenix.EFCore.Context;
using FincaFenix.Entities.DTOs.ShowWorkOrder;
using FincaFenix.Entities.DTOs.WorkOrderDTOs;
using FincaFenix.Entities.POCOEntities;
using FincaFenix.Gateways.Interfaces.QueryServices;
using Microsoft.EntityFrameworkCore;

namespace FincaFenix.EFCore.Services.QueryServices
{
    public class WorkOrderQueryService : FincaFenixContext, IWorkOrderQueryService
    {
        public async Task<(IEnumerable<ShowWorkOrderDTO> WorkOrders, int TotalCount)> GetWorkOrderListPaged(int pageNumber, int pageSize, string status)
        {
            var query = WorkOrders.Where(wo => !wo.IsDeleted && wo.Status != status);

            var totalCount = await query.CountAsync(); //Cuenta la cantidad de registros que cumplen la condición

            var workOrders = await WorkOrders
                .Where(wo => !wo.IsDeleted && wo.Status != status)
                // Ordena por OrderNum para mantener un orden consistente, para que la paginación funcione correctamente
                // si no se ordena, la paginación puede devolver resultados repetidos o saltarse algunos registros en cada pagina
                .OrderBy(wo => wo.Id)
                // Salta los registros de las páginas anteriores, calculando el desplazamiento, define el dice de inicio para la paginación
                .Skip((pageNumber - 1) * pageSize)
                // Toma el número de registros que se especifica en pageSize, define la cantidad de registros a devolver por página
                .Take(pageSize)
                .Select(wo => new ShowWorkOrderDTO
                {
                    Id = wo.Id,
                    OrderNum = wo.OrderNum,
                    Task = wo.Task != null ? new TaskDTO
                    {
                        Id = wo.Task.Id,
                        Description = wo.Task.Description
                    } : null,
                    Farm = wo.Farm != null ? new FarmDTO
                    {
                        Id = wo.Farm.Id,
                        Name = wo.Farm.Name
                    } : null,
                    CreatedDate = wo.CreatedDate,
                    StartDate = wo.StartDate,
                    Status = wo.Status,
                    SectorList = wo.WorkedSectors
                        .Select(wos => new DetailSectorFarmDTO
                        {
                            Id = wos.SectorFarm.Id,
                            FarmId = wos.SectorFarm.FarmId,
                            SectorName = wos.SectorFarm.SectorName
                        })
                        .OrderBy(dto => dto.SectorName)
                        .ToList()
                }).ToListAsync();

            return (workOrders, totalCount);
        }
        public async Task<InfoWorkOrderDTO> GetWorkOrderInfoById(int id)
        {
            return await WorkOrders
                .Where(wo => wo.Id == id && !wo.IsDeleted)
                .Select(wo => new InfoWorkOrderDTO
                {
                    Id = wo.Id,
                    OrderNum = wo.OrderNum,
                    TaskOrder = wo.Task != null ? new TaskDTO
                    {
                        Id = wo.Task.Id,
                        Description = wo.Task.Description
                    } : null,
                    FarmOrder = wo.Farm != null ? new FarmDTO
                    {
                        Id = wo.Farm.Id,
                        Name = wo.Farm.Name
                    } : null,
                    CreatedDate = wo.CreatedDate,
                    StartDate = wo.StartDate,
                    Status = wo.Status,
                    RelationatedSector = wo.WorkedSectors
                        .Select(wos => new DetailSectorFarmDTO
                        {
                            Id = wos.SectorFarm.Id,
                            FarmId = wos.SectorFarm.FarmId,
                            SectorName = wos.SectorFarm.SectorName
                        })
                        .OrderBy(dto => dto.SectorName)
                        .ToList(),
                    Description = wo.Description
                })
                .FirstOrDefaultAsync();
        }
        public async Task<WorkOrderEntity> GetWorkOrderAndRecipeByIdWorkorder(int id)
        {
            return await WorkOrders.Where(wo => wo.Id == id)
                .Include(wo => wo.DetailWorkOrderList) // Incluye la colección de Detalles de Orden de Trabajo
                    .ThenInclude(dwo => dwo.Employee)// Incluye el Material para cada Detalle de Orden de Trabajo
                .Include(wo => wo.DetailWorkOrderList)
                    .ThenInclude(dwo => dwo.SectorWorked) // Incluye el Sector Trabajado para cada Detalle de Orden de Trabajo
                .Include(wo => wo.Task)
                .Include(wo => wo.Farm)
                .Include(wo => wo.Recipe) // Incluye la Receta
                    .ThenInclude(r => r.DetailRecipeList) // Incluye la colección de Detalles de Receta
                        .ThenInclude(dr => dr.Material) // Incluye el Material para cada Detalle de Receta
                            .ThenInclude(m => m.Category) // Incluye el Categorua para cada Material
                .Include(wo => wo.Recipe)
                    .ThenInclude(r => r.Machine) // Incluye la Maquinaria para la Receta
                .Include(wo => wo.WorkedSectors) // Incluye la colección de Sectores Trabajados
                    .ThenInclude(ws => ws.SectorFarm) // Incluye SectorFarm para cada Sector Trabajado
                        .ThenInclude(sf => sf.Variety)// Incluye la Finca para cada SectorFarm
                            .ThenInclude(v => v.Fruit) // Incluye la Fruta para cada Variedad
                 .Include(wo => wo.WorkedSectors) // Incluye la colección de Sectores Trabajados
                    .ThenInclude(ws => ws.SectorFarm) // Incluye SectorFarm para cada Sector Trabajado
                        .ThenInclude(sf => sf.Farm)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<WorkOrderEntity>> GetAllWorkOrderList()
        {
            return await WorkOrders.Where(wo => !wo.IsDeleted)
                .Include(wo => wo.DetailWorkOrderList) // Incluye la colección de Detalles de Orden de Trabajo
                    .ThenInclude(dwo => dwo.Employee)// Incluye el Material para cada Detalle de Orden de Trabajo
                .Include(wo => wo.DetailWorkOrderList)
                    .ThenInclude(dwo => dwo.SectorWorked) // Incluye el Sector Trabajado para cada Detalle de Orden de Trabajo
                .Include(wo => wo.Task)
                .Include(wo => wo.Farm)
                .Include(wo => wo.Recipe) // Incluye la Receta
                    .ThenInclude(r => r.DetailRecipeList) // Incluye la colección de Detalles de Receta
                        .ThenInclude(dr => dr.Material) // Incluye el Material para cada Detalle de Receta
                            .ThenInclude(m => m.Category) // Incluye la Categoría para cada Material
                .Include(wo => wo.Recipe)
                    .ThenInclude(r => r.Machine) // Incluye la Maquinaria para la Receta
                .Include(wo => wo.WorkedSectors) // Incluye la colección de Sectores Trabajados
                    .ThenInclude(ws => ws.SectorFarm) // Incluye SectorFarm para cada Sector Trabajado
                        .ThenInclude(sf => sf.Variety)// Incluye la Finca para cada SectorFarm
                            .ThenInclude(v => v.Fruit) // Incluye la Fruta para cada Variedad
                 .Include(wo => wo.WorkedSectors) // Incluye la colección de Sectores Trabajados
                    .ThenInclude(ws => ws.SectorFarm) // Incluye SectorFarm para cada Sector Trabajado
                        .ThenInclude(sf => sf.Farm)
                 .ToListAsync();
        }
    }
}