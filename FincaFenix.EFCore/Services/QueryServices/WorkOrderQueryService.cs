using FincaFenix.EFCore.Context;
using FincaFenix.Entities.DTOs.DetailWorkOrderDTO;
using FincaFenix.Entities.DTOs.WorkOrderDTOs;
using FincaFenix.Gateways.Interfaces.QueryServices;
using Microsoft.EntityFrameworkCore;

namespace FincaFenix.EFCore.Services.QueryServices
{
    public class WorkOrderQueryService : FincaFenixContext, IWorkOrderQueryService
    {
        public async Task<(IEnumerable<ShowWorkOrderCardDTO> WorkOrders , int TotalCount)> GetWorkOrderListPaged(int pageNumber, int pageSize)
        {
            var query = WorkOrders.Where(wo => !wo.IsDeleted && wo.State == "Activo");
            
            var totalCount = await query.CountAsync(); //Cuenta la cantidad de registros que cumplen la condición

            var workOrders = await WorkOrders
                // Ordena por OrderNum para mantener un orden consistente, para que la paginación funcione correctamente
                // si no se ordena, la paginación puede devolver resultados repetidos o saltarse algunos registros en cada pagina
                .OrderBy(wo => wo.Id)
                // Salta los registros de las páginas anteriores, calculando el desplazamiento, define el dice de inicio para la paginación
                .Skip((pageNumber - 1) * pageSize)
                // Toma el número de registros que se especifica en pageSize, define la cantidad de registros a devolver por página
                .Take(pageSize)
                .Select(wo => new ShowWorkOrderCardDTO
                {
                    Id = wo.Id,
                    OrderNum = wo.OrderNum,
                    TaskOrder = wo.Task != null ? new TaskDTO
                    {
                        Id = wo.Task.Id,
                        Description = wo.Task.Description
                    }: null,
                    FarmOrder = wo.Farm != null ? new FarmDTO
                    {
                        Id = wo.Farm.Id,
                        Name = wo.Farm.Name
                    }: null,
                    StartDate = wo.StartDate,
                    Status = wo.State,
                    RelationatedSector = wo.WorkedSectors
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
        public async Task<ShowInfoAddActivityFormDTO> GetWorkOrderInfoById(int id)
        {
            return await WorkOrders
                .Where(wo => wo.Id == id && !wo.IsDeleted && wo.State == "Activo")
                .Select(wo => new ShowInfoAddActivityFormDTO
                {
                    WorkOrder = new WorkOrderInfoDTO
                    {
                        Id = wo.Id,
                        OrderNum = wo.OrderNum,
                        State = wo.State,
                        IsDeleted = wo.IsDeleted
                    },
                    Farm = new FarmDTO
                    {
                        Id = wo.Farm.Id,
                        Name = wo.Farm.Name
                    },
                    Task = new TaskDTO
                    {
                        Id = wo.Task.Id,
                        Description = wo.Task.Description
                    },
                    SectorList = wo.WorkedSectors
                                    .Select(wos => new DetailSectorFarmDTO
                                    {
                                        Id = wos.SectorFarm.Id,
                                        FarmId = wos.SectorFarm.FarmId,
                                        SectorName = wos.SectorFarm.SectorName,

                                    })
                                    .OrderBy(dto => dto.SectorName)
                                    .ToList(),
                    EmployeeList = wo.Farm.FarmList
                                        .Where(ef => !ef.Employee.IsDeleted)
                                        .Select(ef => new EmployeeDTO
                                        {
                                            Id = ef.Employee.Id,
                                            Name = ef.Employee.Name,
                                            LastName = ef.Employee.LastName,
                                        })
                                        .OrderBy(dto => dto.Name)
                                        .ToList()
                })
                .FirstOrDefaultAsync();
        }
    }
}