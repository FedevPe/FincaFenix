using FincaFenix.Entities.DTOs.RecipeDTO;
using FincaFenix.Entities.DTOs.WorkOrderDTOs;
using FincaFenix.UsesCases.Controllers;

namespace FincaFenix.ViewModels.ViewModels
{
    public class NewRecipeViewModel(
        IMaterialController material,
        IMaterialCategoryController categories,
        IMachineController machine)
    {
        
        public decimal? TotalAreaSectors { get; set; } = 0;
        public IEnumerable<MachineRecipeDTO>? MachineList { get; set; }
        public MachineRecipeDTO? Machine { get; set; }
        public List<DetailRecipeDTO> Details { get; set; } = new List<DetailRecipeDTO>();
        public IEnumerable<MaterialRecipeDTO>? Materials { get; set; }
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

