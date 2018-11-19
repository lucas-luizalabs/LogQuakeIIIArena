using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.JsonPatch.Operations;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LogQuake.API
{
    /// <summary>
    /// Classe utilizada em conjunto com Swagger para efetuar upload de arquivo
    /// </summary>
    public class FileOperationFilter : IOperationFilter
    {
        public void Apply(Operation operation, OperationFilterContext context)
        {
            if (context.ApiDescription.ParameterDescriptions.Any(x => x.ModelMetadata.ContainerType == typeof(IFormFile)))
            {
                operation.Parameters.Clear();
                operation.Parameters.Add(new NonBodyParameter
                {
                    Name = "file", // precisa ser o mesmo nome do parâmetro do método de Upload da GamesController
                    In = "formData",
                    Description = "Selecione o arquivo de log do jogo Quake III Arena.",
                    Required = true,
                    Type = "file",
                });
                operation.Consumes.Add("application/form-data");
            }
        }
    }
}
