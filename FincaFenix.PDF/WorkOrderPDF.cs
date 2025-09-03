using FincaFenix.Entities.DTOs.ShowWorkOrder;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Reflection;

namespace FincaFenix.PDF
{
    public class WorkOrderPDF : IDocument
    {
        public ShowWorkOrderDTO DTO { get; set; }
        public DocumentMetadata GetMetadata() => DocumentMetadata.Default;
        public WorkOrderPDF(ShowWorkOrderDTO workOrderDTO)
        {
            DTO = workOrderDTO;
        }
        public void Compose(IDocumentContainer container)
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(10);
                page.DefaultTextStyle(x => x.FontSize(9));

                page.Header().Element(BuildHeader);
                page.Content()
                        .Column(column =>
                        {
                            column.Item().Element(BuilGeneralData);
                            column.Item().Element(BuilDescription);
                            column.Item().Element(BuildRecipe);
                            column.Item().Element(BuilMaterialList);
                            column.Item().Element(BuildRegistryActivity);
                        });
                page.Footer().Element(BuildFooter);
            });
        }
        private void BuildHeader(IContainer container)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var img = assembly.GetManifestResourceStream("FincaFenix.PDF.Resources.logo_fenix.png.jpg");

            container.Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.RelativeColumn(1); // Logo
                    columns.RelativeColumn(2); // Numero de orden y tarea
                });
                table.Header(header =>
                {
                    header.Cell().Border(1).Image(img);
                    header.Cell().Border(1).AlignMiddle().Column(column =>
                    {
                        column.Item().Text($"ORDEN DE TRABAJO N° {DTO.OrderNum}").SemiBold().FontSize(16).AlignCenter();
                        column.Item().Text($"{DTO.Task.Description}").SemiBold().FontSize(12).AlignCenter();
                    });
                });
            });
        }
        private void BuilGeneralData(IContainer generalData)
        {
            generalData.PaddingTop(10).Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.ConstantColumn(150); // Finca
                    columns.RelativeColumn(); // Cuadros
                    columns.RelativeColumn(); // Fechas
                    columns.RelativeColumn(); // Solicitante
                });
                table.Cell().Row(1).ColumnSpan(4).Background(Colors.Grey.Lighten2).Element(CellStyle).AlignCenter().Text("DATOS GENERALES").Bold();

                //Finca
                table.Cell().Border(1).Table(table =>
                {
                    table.Header(header =>
                    {
                        header.Cell().Element(CellStyle).AlignLeft().Text("Finca:").SemiBold();
                    });
                    table.ColumnsDefinition(columns =>
                    {
                        columns.RelativeColumn(1);
                    });
                    table.Cell().Padding(3).AlignLeft().Text($"{DTO.Farm.Name}");
                });
                //Cuadros
                table.Cell().Border(1).Table(table =>
                {
                    table.Header(header =>
                    {
                        header.Cell().Element(CellStyle).AlignLeft().Text("Cuadros:").SemiBold();
                    });
                    table.ColumnsDefinition(columns =>
                    {
                        columns.RelativeColumn(1);
                    });
                    table.Cell().Padding(3).AlignLeft().Text(string.Join(" - ", DTO.SectorList.Select(s => s.SectorName)));
                });
                //Fechas
                table.Cell().Border(1).Table(table =>
                {
                    table.Header(header =>
                    {
                        header.Cell().Element(CellStyle).AlignLeft().Text("Fecha:").SemiBold();
                    });
                    table.ColumnsDefinition(columns =>
                    {
                        columns.RelativeColumn();
                    });
                    table.Cell().Padding(3).Text($"Inicio: {DTO.StartDate?.ToString("dd/MM/yyyy") ?? " "}");
                    if (DTO.EndDate.HasValue)
                        table.Cell().Padding(3).Text($"Fin: {DTO.EndDate?.ToString("dd/MM/yyyy") ?? " "}");

                });
                //Solicitante
                table.Cell().Border(1).Table(table =>
                {
                    table.Header(header =>
                    {
                        header.Cell().Element(CellStyle).AlignLeft().Text("Confeccionó:").SemiBold();
                    });
                    table.ColumnsDefinition(columns =>
                    {
                        columns.RelativeColumn(1);
                    });
                    table.Cell().Padding(3).AlignLeft().Text("Rubén Cornejo");
                });
                static IContainer CellStyle(IContainer container) => container.Border(1).Padding(3);
            });
        }
        private void BuilDescription(IContainer description)
        {
            description.Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.RelativeColumn(1); // Descripción
                });
                table.Cell().Row(1).ColumnSpan(1).Background(Colors.Grey.Lighten2).Element(CellStyle).AlignLeft().Text("DESCRIPCIÓN").Bold();
                if (!string.IsNullOrEmpty(DTO.Description))
                {
                    table.Cell().Border(1).Padding(5).AlignLeft().Text(DTO.Description);
                }
                else
                {
                    table.Cell().Border(1).Padding(5).AlignLeft().Text("Orden de trabajo sin descripción");

                }
                static IContainer CellStyle(IContainer container) => container.Border(1).Padding(3);
            });
        }
        private void BuildRecipe(IContainer recipe)
        {
            recipe.PaddingTop(10).Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.ConstantColumn(150); //Header
                    columns.RelativeColumn(); //Numero de receta
                    columns.RelativeColumn(); //Maquinaria y volumen
                    columns.RelativeColumn(); //TRV
                    columns.RelativeColumn(); //Maquinadas
                });
                //Header
                table.Cell().Row(1).ColumnSpan(5).Background(Colors.Grey.Lighten2).Element(CellStyle).AlignCenter().Text("RECETA").Bold();
                if (DTO.Recipe != null)
                {
                    //Numero de receta
                    table.Cell().Border(1).Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn();
                        });
                        table.Cell().AlignMiddle().Padding(3).AlignLeft().Text(x =>
                        {
                            x.Span($"N° Receta: ").Bold();
                            x.Span($"{DTO.Recipe.NumRecipe}");
                        });
                    });
                    //Maquinaria y volumen
                    table.Cell().ColumnSpan(2).Border(1).Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn();
                        });
                        table.Cell().Element(CellStyle).AlignLeft().Text(x =>
                        {
                            x.Span($"Maquinaria: ").Bold();
                            x.Span($"{DTO.Recipe.Machine.Name}");
                        });
                        table.Cell().Element(CellStyle).AlignLeft().Text(x =>
                        {
                            x.Span($"Volumen: ").Bold();
                            x.Span($"{DTO.Recipe.VolumeMachine.ToString("N2")} {DTO.Recipe.VolumeMachineUnit}");
                        });
                    });
                    table.Cell().Border(1).Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn();
                        });
                        table.Cell().Element(CellStyle).AlignLeft().Text(x =>
                        {
                            x.Span($"TRV: ").Bold();
                        });
                        table.Cell().Element(CellStyle).AlignLeft().Text(x =>
                        {
                            x.Span(($"{DTO.Recipe.TRV.ToString("N2")} m3/ha"));
                        });
                    });
                    table.Cell().Border(1).Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn();
                        });
                        table.Cell().Element(CellStyle).AlignLeft().Text($"Maquinadas: ").Bold();
                        table.Cell().Element(CellStyle).AlignLeft().Text(x =>
                        {
                            if (DTO.Recipe != null && DTO.Recipe.VolumeMachine != 0 && DTO.Recipe.TRV != 0)
                            {
                                x.Span($"{((DTO.TotalAreaWorked * DTO.Recipe.TRV) / DTO.Recipe.VolumeMachine)}");
                            }
                            else
                            {
                                x.Span("No disponible");
                            }

                        });
                    });
                }
                else
                {
                    table.Cell().Row(2).ColumnSpan(5).Border(1).Padding(5).AlignCenter().Text("No hay receta asociada a esta orden de trabajo.");
                }
                static IContainer CellStyle(IContainer container) => container.Border(1).Padding(3);
            });
        }
        private void BuilMaterialList(IContainer materialList)
        {
            materialList.PaddingTop(10).Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.ConstantColumn(150); //Header principal
                    columns.RelativeColumn(); //Header secundario
                    columns.RelativeColumn(); //Detalle de materiales
                });
                table.Cell().Row(1).ColumnSpan(3).Background(Colors.Grey.Lighten2).Element(CellStyle).AlignCenter().Text("LISTA DE MATERIALES").Bold();
                if (DTO.Recipe != null && DTO.Recipe.Details.Any())
                {
                    table.Cell().Row(2).ColumnSpan(3).Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(2);
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                        });
                        table.Cell().Element(CellStyle).AlignCenter().Text("Principio Activo").SemiBold();
                        table.Cell().Element(CellStyle).AlignCenter().Text("Producto").SemiBold();
                        table.Cell().Element(CellStyle).AlignCenter().Text("Enfermedad/Plaga").SemiBold();
                        table.Cell().Element(CellStyle).AlignCenter().Text("Cant.Requerida/Dosis").SemiBold();
                        table.Cell().Element(CellStyle).AlignCenter().Text("Cant.Estimada").SemiBold();
                    });
                    table.Cell().Row(3).ColumnSpan(3).Border(1).Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(2); // Principio Activo
                            columns.RelativeColumn(1); // Producto
                            columns.RelativeColumn(); // Enfermedad/Plaga
                            columns.RelativeColumn(); // Cant.Requerida/Dosis
                            columns.RelativeColumn(); // Cant.Estimada
                        });
                        foreach (var material in DTO.Recipe.Details)
                        {
                            table.Cell().Padding(3).AlignLeft().Text(material.Material.ArticleName);
                            table.Cell().Padding(3).AlignLeft().Text(material.Brand);
                            table.Cell().Padding(3).AlignLeft().Text(material.PestDisease);
                            table.Cell().Padding(3).AlignRight().Text($"{material.AmountRequired} {material.AmountRequiredUnit}");
                            table.Cell().Padding(3).AlignRight().Text($"{material.EstimatedAmount} {material.EstimatedAmountUnit}");
                        }
                    });
                }
                else
                {
                    table.Cell().Row(3).ColumnSpan(3).Border(1).Padding(5).AlignCenter().Text("No hay materiales asociados a esta receta.");
                }

                static IContainer CellStyle(IContainer container) => container.Border(1).Padding(3);
            });
        }
        private void BuildRegistryActivity(IContainer registryActivity)
        {
            registryActivity.PaddingTop(10).Border(1).Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.ConstantColumn(150); //Header principal "Registro de Actividades"
                    columns.RelativeColumn(); //Header secundario "Columnas de actividades"
                    columns.RelativeColumn(); //Detalle de actividades
                });
                table.Cell().Row(1).ColumnSpan(3).Background(Colors.Grey.Lighten2).Element(CellStyle).AlignCenter().Text("REGISTRO DE ACTIVIDADES").Bold();
                if (DTO.DetailsWorkOrder != null && DTO.DetailsWorkOrder.Any())
                {
                    table.Cell().Row(2).ColumnSpan(3).Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(1);
                            columns.RelativeColumn(1);
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                            columns.RelativeColumn(0.7F);
                            columns.RelativeColumn(2);
                        });
                        table.Cell().Element(CellStyle).AlignLeft().Text("Fecha").SemiBold();
                        table.Cell().Element(CellStyle).AlignCenter().Text("Nombre Operario").SemiBold();
                        table.Cell().Element(CellStyle).AlignCenter().Text("Cuadro trabajado").SemiBold();
                        table.Cell().Element(CellStyle).AlignCenter().Text("Horas trabajadas").SemiBold();
                        table.Cell().Element(CellStyle).AlignCenter().Text("Rendimiento").SemiBold();
                        table.Cell().Element(CellStyle).AlignCenter().Text("Observaciones").SemiBold();

                    });
                    table.Cell().Row(3).ColumnSpan(3).Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(1);
                            columns.RelativeColumn(1);
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                            columns.RelativeColumn(0.6F);
                            columns.RelativeColumn(2);
                        });
                        foreach (var activity in DTO.DetailsWorkOrder)
                        {
                            table.Cell().Padding(3).AlignLeft().Text($"{activity.ActivityDate.ToString("dd/MM/yyyy")}");
                            table.Cell().Padding(3).AlignLeft().Text($"{activity.Employee.Name} {activity.Employee.LastName}");
                            table.Cell().Padding(3).AlignRight().Text($"{activity.Sector.SectorName}");
                            table.Cell().Padding(3).AlignRight().Text($"{activity.WorkedHours}");
                            table.Cell().Padding(3).AlignRight().Text($"{activity.Performance.ToString("N2")}");
                            table.Cell().Padding(3).AlignLeft().Text($"{activity.Description}");
                        }
                    });
                }
                else
                {
                    table.Cell().Row(2).ColumnSpan(3).Border(1).Padding(5).AlignCenter().Text("No hay actividades registradas para esta orden de trabajo.");
                }
                static IContainer CellStyle(IContainer container) => container.Border(1).Padding(3);
            });
        }
        private void BuildFooter(IContainer footer)
        {

            footer.AlignCenter().BorderTop(1).Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.RelativeColumn(1);
                    columns.RelativeColumn(1);
                    columns.RelativeColumn(1);
                });
                table.Cell().Padding(5).AlignCenter().Text("XX-XX-XX-XXX-001-01").Bold();
                table.Cell().Padding(5).AlignCenter().Text(x =>
                {
                    x.Span($"Finca {DTO.Farm.Name} - Fénix S.A").SemiBold();
                });
                table.Cell().Padding(5).AlignCenter().Text(x =>
                {
                    x.CurrentPageNumber();
                    x.Span(" / ");
                    x.TotalPages();
                });
            });
        }
    }
}
