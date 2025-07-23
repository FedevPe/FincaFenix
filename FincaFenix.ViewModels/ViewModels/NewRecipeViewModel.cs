using FincaFenix.Entities.DTOs.RecipeDTO;
using FincaFenix.Entities.DTOs.WorkOrderDTOs;
using FincaFenix.UsesCases.Controllers;

namespace FincaFenix.ViewModels.ViewModels
{
    public class NewRecipeViewModel(
        IMaterialController material,
        IMaterialCategoryController categories,
        IMachineController machine) : RecipeWorkOrderDTO
    {
        public decimal TRV { get; set; }
        public decimal Dosage { get; set; }
        public string DosageUnit { get; set; }
        public string Message { get; set; }
        public decimal? TotalAreaSectors { get; set; } = 0;

        public IEnumerable<MachineRecipeDTO>? MachineList { get; set; }
        public MachineRecipeDTO? Machine { get; set; }
        public IEnumerable<MaterialOrderDTO>? Materials { get; set; }
        public IEnumerable<MaterialCategoryDTO>? Categories { get; set; }

        public void InitializeRecipe(decimal? totalSurface)
        {
            TotalAreaSectors = totalSurface;
            AddMachine();
            AddMaterial();
        }

        public async Task LoadInitializeAsync()
        {
            Details = new List<DetailRecipeDTO>();
            await Task.WhenAll(LoadCategoryList(), LoadMaterialList(), LoadMachineList());
        }
        public async Task LoadMaterialList()
        {
            Materials = await material.GetMaterialList();
        }
        public async Task LoadMachineList()
        {
            MachineList = await machine.GetMachines();
        }
        public async Task LoadMaterialListByCategoryId(int categoryId)
        {
            Materials = await material.GetListMaterialByCategoryId(categoryId);
        }
        public async Task LoadCategoryList()
        {
            Categories = await categories.GetAllCategories();
        }
        public void AddMaterial()
        {
            Details.Add(new DetailRecipeDTO());
        }
        public void RemoveMaterial(DetailRecipeDTO material)
        {
            if(material != null && Details.Contains(material))
            {
                Details.Remove(material);
            }
        }
        public void RemoveMachine()
        {
            Machine = null;
        }
        public void AddMachine()
        {
            Machine = new MachineRecipeDTO();
        }
        public decimal CalculateEstimateAmount(decimal TRV, decimal amountRequired, decimal? totalSurface, string measureUnit)
        {
            if (TRV <= 0)
            {
                Message = "TRV debe ser mayor que cero.";
                return 0;
            }

            if (amountRequired <= 0)
            {
                Message = "La cantidad requerida debe ser mayor que cero.";
                return 0;
            }

            if (totalSurface <= 0 || totalSurface == null)
            {
                Message = "La superficie total no es válida para el cálculo.";
                return 0;
            }

            if (measureUnit != "lts" && measureUnit != "kg" )
            {
                decimal estimatedAmount = ((amountRequired * TRV / 1000M) * totalSurface.Value)/1000;
                return estimatedAmount;
            }
            else 
            {
                decimal estimatedAmount = ((amountRequired * TRV / 1000M) * totalSurface.Value);
                return estimatedAmount;
            }    
        }
    }
}

